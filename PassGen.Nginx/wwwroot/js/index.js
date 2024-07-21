function changePageVisualState() {
    $("#btnGenerate").prop("disabled", !($("#targetSite").val()) || !($("#salt").val()));
    const hasGeneratedPassword = !!($("#generatedPassword").val());
    $("#generatedPasswordGroup").collapse(hasGeneratedPassword ? "show" : "hide");
}

function changeState(generatedPassword) {
    const txtGeneratedPassword = $("#generatedPassword");
    txtGeneratedPassword.val(generatedPassword || "");
    txtGeneratedPassword.change();
    changePageVisualState();
}

function onInputKeyup(event) {
    const enterKeyCode = 13;
    const isEnterKeyup = (event.key === 'Enter' || event.keyCode === enterKeyCode);
    if (!isEnterKeyup) {
        changeState(null);
    }
}

function generatePasswordClick(event) {
    event.preventDefault();
    const generatedPassword = generatePassword($("#targetSite").val(), $("#salt").val());
    changeState(generatedPassword);
}

$(document).ready(function() {
    $(document).on("keyup", "#targetSite, #salt", onInputKeyup);
    $("#btnGenerate").on("click", generatePasswordClick);
    $(".collapse").on("hidden.bs.collapse shown.bs.collapse", changePageVisualState);
    changePageVisualState();
});
