function enableDisableCopyToClipboardBtn(btnCopyToClipboard, txtBoxToCopy) {
    btnCopyToClipboard.prop("disabled", !txtBoxToCopy.val());
}

function showCopyToClipboardTooltip(btnCopyToClipboard, tooltipTitle) {
    const timerKey = "copy_to_clipboard_timer";
    const tooltipTimeout = 3000;

    clearTimeout(btnCopyToClipboard.data(timerKey));

    btnCopyToClipboard
        .attr("title", tooltipTitle)
        .attr("data-original-title", tooltipTitle)
        .tooltip("update")
        .tooltip("show");

    const copyToClipboardTimer = setTimeout(function () {
        btnCopyToClipboard.tooltip("hide");
    }, tooltipTimeout);
    btnCopyToClipboard.data(timerKey, copyToClipboardTimer);
}

function copyTextToClipboard(btnCopyToClipboard, txtBoxToCopy) {
    const textToCopy = txtBoxToCopy.val();
    navigator.clipboard.writeText(textToCopy).then(function () {
        showCopyToClipboardTooltip(btnCopyToClipboard, "Copied");
    }, function () {
        showCopyToClipboardTooltip(btnCopyToClipboard, "Error: failed to copy");
    });
}

$(document).ready(function() {
    $("[data-copy-to-clipboard-for]").each(function () {
        const btnCopyToClipboard = $(this);
        const txtBoxToCopy = $("#" + btnCopyToClipboard.attr("data-copy-to-clipboard-for"));

        txtBoxToCopy.on("keyup change", function () {
            enableDisableCopyToClipboardBtn(btnCopyToClipboard, txtBoxToCopy);
        });
        btnCopyToClipboard.on("click", function () {
            copyTextToClipboard(btnCopyToClipboard, txtBoxToCopy);
        });
    });
});
