function enableDisableCopyToClipboardBtn(btnCopyToClipboard, txtBoxToCopy) {
    btnCopyToClipboard.prop("disabled", !txtBoxToCopy.val());
}

function copyTextToClipboard(txtBoxToCopy) {
    const textToCopy = txtBoxToCopy.val();

    const temp = $("<input>");
    temp.css({position: "absolute", left: "-9999px", width: "1000px"});
    $("body").append(temp);
    temp.val(textToCopy).select();
    document.execCommand("copy");
    temp.remove();
}

function showCopyToClipboardTooltip(btnCopyToClipboard) {
    const timerKey = "copy_to_clipboard_timer";
    const tooltipTimeout = 3000;
    
    btnCopyToClipboard.tooltip("show");
    clearTimeout(btnCopyToClipboard.data(timerKey));
    
    const copyToClipboardTimer = setTimeout(function () {
        btnCopyToClipboard.tooltip("hide");
    }, tooltipTimeout);

    btnCopyToClipboard.data(timerKey, copyToClipboardTimer);
}

$(document).ready(function() {
    $("[data-copy-to-clipboard-for]").each(function () {
        const btnCopyToClipboard = $(this);
        const txtBoxToCopy = $("#" + btnCopyToClipboard.attr("data-copy-to-clipboard-for"));

        txtBoxToCopy.on("keyup change", function () {
            enableDisableCopyToClipboardBtn(btnCopyToClipboard, txtBoxToCopy);
        });
        btnCopyToClipboard.on("click", function () {
            copyTextToClipboard(txtBoxToCopy);
            showCopyToClipboardTooltip(btnCopyToClipboard);
        });
    });
});