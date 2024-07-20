// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let hasPassword = false;

function changePageVisualState() {
    $("#btnGenerate").prop("disabled", !($("#targetSite").val()) || !($("#salt").val()));
    $("#generatedPasswordGroup").collapse(hasPassword ? "show" : "hide");
}

function changeState(generatedPassword) {
    const txtGeneratedPassword = $("#generatedPassword");
    txtGeneratedPassword.val(generatedPassword || "");
    txtGeneratedPassword.change();
    hasPassword = !!generatedPassword;
    changePageVisualState();
}

function onInputChange() {
    changeState(null);
}

function generatePasswordClick(event) {
    event.preventDefault();
    let generatedPassword = generatePassword($("#targetSite").val(), $("#salt").val());
    changeState(generatedPassword);
}

$(document).ready(function() {
    $("#salt").on("keyup change", onInputChange);
    $("#targetSite").on("keyup change", onInputChange);
    $("#btnGenerate").on("click", generatePasswordClick);

    $(".collapse").on("hidden.bs.collapse shown.bs.collapse", changePageVisualState);

    changePageVisualState();
});
