// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var ResetPasswordLogSpan;
var resetPasswordForm;
var resetPasswordButton;
$(document).ready(() => {
    resetPasswordForm = $('#resetPasswordForm');
    ResetPasswordLogSpan = $('#resetPasswordLogSpan');
    resetPasswordButton = $('#resetPasswordButton');
    resetPasswordButton.on('click', tryFindUser)
});
function tryFindUser(e) {
    e.preventDefault();
    ResetPasswordLogSpan.empty();
    $.ajax({
        type: "get",
        url: "/Account/FindSuggestedUser",
        data: resetPasswordForm.serialize(),
        success: function () {
            resetPasswordForm.submit();
        },
        error: function (xhr) {
            ResetPasswordLogSpan.append("<p>" + xhr.responseText + "</p>")
        }
    });
}