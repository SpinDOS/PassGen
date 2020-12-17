// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let runningRequestInfo = null;
let hasPassword = false;
let hasError= false;

function changePageVisualState() {
    const btn = $("#btnGenerate");
    const progressSpinner = $(btn.children()[0]);
    const text = $(btn.children()[1]);
    
    const isRequestInProgress = runningRequestInfo != null;
    
    btn.prop("disabled", isRequestInProgress || !($("#targetSite").val()) || !($("#salt").val()));

    progressSpinner.css({display: isRequestInProgress ? "inline-block" : "none"});
    text.text(isRequestInProgress ? "Generating password" : "Generate password");

    $("#errorLabel").collapse(hasError ? "show" : "hide");
    $("#generatedPasswordGroup").collapse(hasPassword ? "show" : "hide");
}

function changeState(currentRequestInfo, generatedPassword, error) {
    if (runningRequestInfo != null) {
        runningRequestInfo.aborted = true;
        runningRequestInfo.ajaxRequest.abort();
    }
    runningRequestInfo = currentRequestInfo;

    const txtGeneratedPassword = $("#generatedPassword");
    txtGeneratedPassword.val(generatedPassword || "");
    txtGeneratedPassword.change();
    hasPassword = !!generatedPassword;
    
    $("#errorLabel").text(error || "");
    hasError = !!error;
    
    changePageVisualState();
}

function onInputChange() {
    changeState(null, null, null);
}

function onSuccess(data) {
    data = data || {};
    const generatedPassword = data.generatedPassword;
    if (generatedPassword) {
        changeState(null, generatedPassword, null);
    } else {
        changeState(null, null, "Invalid server response: empty GeneratedPassword");
    }
}

function onError(error) {
    changeState(null, null, error.statusText || "Unknown error");
}

function generatePasswordClick(event) {
    event.preventDefault();
    const currentRequestInfo = {};
    currentRequestInfo.aborted = false;
    currentRequestInfo.ajaxRequest = $.ajax({
        method: "POST",
        url: "/api/v1/GeneratePassword",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        },
        data: JSON.stringify({ TargetSite: $("#targetSite").val(), Salt: $("#salt").val() }),
        dataType: "json",
        success: onSuccess,
        error: function (error) {
            if (!currentRequestInfo.aborted) {
                onError(error.statusText);
            }
        }
    });
    changeState(currentRequestInfo, null, null);
}

$(document).ready(function() {
    $("#salt").on("keyup change", onInputChange);
    $("#targetSite").on("keyup change", onInputChange);
    $("#btnGenerate").on("click", generatePasswordClick);
    
    $(".collapse").on("hidden.bs.collapse shown.bs.collapse", changePageVisualState);

    changePageVisualState();
});