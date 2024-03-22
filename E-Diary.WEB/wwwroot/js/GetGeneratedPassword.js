var nameOfPasswordField = "Password"
var nameOfPasswordGenerateButton = "GeneratePassword"
$("[name=" + nameOfPasswordGenerateButton + "]").click(function () {
    $.ajax({
        url: "/Account/GetGeneratedPassword",
        type: "GET",
        dataType: "JSON",
        success: function (pwd) {
            $("[name=" + nameOfPasswordField + "]").val(pwd);
        },
        error: function (xhr, status, err) { console.log(err); }
    })
});