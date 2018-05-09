(function() {
    $.ready(function () {

        

        var $loginToggle = $("#loginToggle");
        var $popupForm = $(".popup-form");

        $loginToggle.on("click",
            function() {
                $popupForm.toggle(1000);
            });
    });
}())