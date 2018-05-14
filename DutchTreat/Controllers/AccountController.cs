using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;



namespace DutchTreat.Controllers
{
   public class AccountController : Controller
   {
      private readonly ILogger<AccountController> _logger;
      private readonly SignInManager<StoreUser> _signInManager;
      private readonly UserManager<StoreUser> _userManager;
      private readonly IConfiguration _config;


      public AccountController(ILogger<AccountController> logger,
         SignInManager<StoreUser> signInManager, UserManager<StoreUser> userManager, IConfiguration config)
      {
         _logger = logger;
         _signInManager = signInManager;
         _userManager = userManager;
         _config = config;
      }

      public IActionResult Login()
      {
         //has already someone logged in?
         if (this.User.Identity.IsAuthenticated)
         {
            return RedirectToAction("Index", "App");
         }

         return View("Login");
      }

      [HttpPost]
      public async Task<IActionResult> Login(LoginViewModel model)
      {
         if (ModelState.IsValid)
         {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
               if (Request.Query.Keys.Contains("ReturnUrl"))
               {
                  RedirectToAction(Request.Query["ReturnUrl"].First());
               }

               return RedirectToAction("Shop", "App");
            }
         }
         ModelState.AddModelError("", "Failed to login");
         return View();
      }

      [HttpGet]
      public async Task<IActionResult> Logout()
      {
         await _signInManager.SignOutAsync();

         return RedirectToAction("Index", "App");
      }

      [HttpPost]
      public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
      {
         //check model state to verify that we do want to create a token
         if (ModelState.IsValid)
         {
            //FindByNameAsync accepts a username and returns the type 
            StoreUser user = await _userManager.FindByNameAsync(model.Username);

            if (user != null)
            {

               Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
               if (result.Succeeded) //there are more results such as locked out etc
               {
                  //create all the claims that are going to be part of the JWT 
                  var claims = new[]
                  {
                     new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName) 
                  };

                  //Next we are going to crate a token but first we need to encrypt it, so create a key
                  //some parts (sensitive material) is encrypte and rest are not
                  //NOTE: The same key goes into Startup otherwise it will not authorise
                  var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));

                  //A wrapper class for properties that are used for signature valdiation - note it used the symetric key created above
                  SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                  //create a token
                  var token = new JwtSecurityToken(
                     _config["Tokens:Issuer"],
                     _config["Tokens:Audience"],
                     claims,
                     expires: DateTime.Now.AddMinutes(30),
                     signingCredentials: credentials);

                  var results = new
                  {
                     token = new JwtSecurityTokenHandler().WriteToken(token), //this creates a string value of the token object created above
                     expiration = token.ValidTo
                  };

                  return Created("", results);
               }
            }

         }

         return BadRequest();
      }

   }
}