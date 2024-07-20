$(document).ready(function() {
    $("[data-show-hide-password-for]").on("click", function (event) {
        event.preventDefault();

        const this_ = $(this);
        const targetTxtId = this_.attr("data-show-hide-password-for");
        const txtBox = $("#" + targetTxtId);
        const icon = this_.children();

        if (txtBox.attr("type") === "text") {
            txtBox.attr("type", "password");
            icon.removeClass("fa-eye-slash");
            icon.addClass("fa-eye");
        } else if(txtBox.attr("type") === "password") {
            txtBox.attr("type", "text");
            icon.addClass("fa-eye-slash");
            icon.removeClass("fa-eye");
        }
    });
});