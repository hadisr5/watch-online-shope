var KTAppOptions = {
    colors: {
        state: {
            brand: "#5d78ff",
            dark: "#282a3c",
            light: "#ffffff",
            primary: "#5867dd",
            success: "#34bfa3",
            info: "#36a3f7",
            warning: "#ffb822",
            danger: "#fd3995"
        },
        base: {
            label: ["#c5cbe3", "#a1a8c3", "#3d4465", "#3e4466"],
            shape: ["#f0f3ff", "#d9dffa", "#afb4d4", "#646c9a"]
        }
    }
};

function hashChange() {
    titlePage(""), $("#Content").html('<div style="text-align:center"><div class="loader loader--style3" title="2"><svg version="1.1" id="loader-1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" width="40px" height="40px" viewBox="0 0 50 50" style="enable-background:new 0 0 50 50;" xml:space="preserve"><path fill="#000" d="M43.935,25.145c0-10.318-8.364-18.683-18.683-18.683c-10.318,0-18.683,8.365-18.683,18.683h4.068c0-8.071,6.543-14.615,14.615-14.615c8.072,0,14.615,6.543,14.615,14.615H43.935z" transform="rotate(81.738 25 25)"><animateTransform attributeType="xml" attributeName="transform" type="rotate" from="0 25 25" to="360 25 25" dur="0.6s" repeatCount="indefinite"></animateTransform></path></svg></div></div>');
    var t = window.location.hash.substring(1);
    "" != t.trim() && $.ajax({
        type: "GET",
        url: t,
        data: {
            data: "check"
        },
        success: function (t) {
            $("#Content").html(t), $("#pageLoad_progress").css("display", "none")
        },
        progress: function (t) {
            if (t.lengthComputable) {
                $("#pageLoad_progress").css("display", "block"), $("#pageLoad_progress").css("width", "5%");
                var e = t.loaded / t.total * 100;
                console.log(e), $("#pageLoad_progress").css("width", e + "%")
            } else console.warn("Content Length not reported!")
        }
    })
}

function titlePage(t) {
    $(".kt-subheader__title").html(t)
}
$(window).on("hashchange", function () {
    hashChange()
}), $(document).ready(function () {
    hashChange()
}), $(document).on("click", '[type="submit"]', function (t) {
    var selected = $(this);
    t.preventDefault();
    try {
        for (instance in CKEDITOR.instances) CKEDITOR.instances[instance].updateElement()
    } catch (t) { }
    $.ajax({
        type: "POST",
        url: $($(selected).closest('.myForm')).attr("action"),
        data: $($(selected).closest('.myForm')).serialize(),
        success: function (t) {
            if (t.title != null && t.title == 'error') {
                swal.fire("خطا !", t.desc, "error")

            } else {
                swal.fire("انجام شد !", t.success, "success"),
                window.location = $($(selected).closest('.myForm')).attr("data-redirect"), hashChange()
            }

        }
    })
}), $(document).on("click", '[data-role="remove"]', function () {
    Swal.mixin({
        customClass: {
            confirmButton: "btn btn-success",
            cancelButton: "btn btn-danger"
        },
        buttonsStyling: !1
    }).fire({
        title: "آیا مطمئن هستید؟",
        text: "پس از حذف این آیتم نمیتوانید آن را بازیابی کنید !",
        type: "warning",
        showCancelButton: !0,
        confirmButtonText: "بله, مطمئنم!",
        cancelButtonText: "خیر , منصرف شدم",
        reverseButtons: !0
    }).then(t => {
        if (t.value) {
            var e = $(this).attr("data-url");
            $.ajax({
                type: "POST",
                url: e,
                success: function (t) {
                    swal.fire("انجام شد", "آیتم مورد نظر با موفقیت حذف شد", "success"), hashChange()
                }
            })
        } else t.dismiss, Swal.DismissReason.cancel
    })
}), $(document).on("click", "#newProp", function () {
    $(".moreProp").append('<div><input type="text" class="form-control propInput" placeholder="ویژگی دسته بندی" name="prop"><img src="/theme/img/minus.svg" class="minus" alt="Alternate Text" /></div>')
}), $(document).on("click", ".minus", function () {
    $(this).parent().remove()
}), $(document).on("click", ".removeExisting", function () {
    var t = $(this);
    $.ajax({
        type: "POST",
        url: "/categories/RemoveCat",
        data: "id=" + $(t).data("id"),
        success: function (t) {
            swal.fire("انجام شد !", t.success, "success")
        }
    }), $(this).parent().remove()
}), $(document).on("click", "#imgValues>div>span", function () {
    $(this).parent().remove(), '<img src="/Theme/img/noPhoto.png" alt="Alternate Text" id="nophoto" class="hidden">' == $("#imgValues").html().trim() && $("#nophoto").removeClass("hidden")
}), $(document).on("change", ".catBox", function () {
    var t = $(this);
    this.checked ? $.ajax({
        type: "POST",
        url: "/admin/products/checkCatProperties",
        data: "id=" + $(this).data("id"),
        success: function (e) {
            for (var n = '<table class="table table-hover">', a = e.length, s = 0; s < a; s++) n = n + "<tr><td>" + e[s].title + '</td><td><input name="' + e[s].id + '" type="text" class="form-control"/></td> </tr>';
            n += "</table>", $('.catProperties[data-id="' + $(t).data("id") + '"]').html(n)
        }
    }) : $('.catProperties[data-id="' + $(t).data("id") + '"]').empty()
}), $(document).on("change", 'input:radio[name="Inventory"]', function () {
    $(this).is(":checked") && 1 == $(this).hasClass("checkOnly") ? $("#InventoryContainer").removeClass("hidden") : ($('input:text[name="Inventory"]').val(""), $("#InventoryContainer").addClass("hidden"))
}), $(document).on("keyup", ".searchInTable", function () {
    var t = $(this);
    if ($(".ntFound").each(function () {
            $(this).remove()
    }), $(".myClass").each(function () {
            var t = $(this).text();
            $(this).parent().find(".myClass").replaceWith(t)
    }), "" !== t.val().trim()) {
        if ($(".table-hover>tbody>Tr").each(function () {
                var e = $(this);
                if ($(this).text().indexOf($(t).val()) <= -1) $(this).addClass("hiddenTr");
        else {
                    var n = new RegExp(t.val(), "g"),
                        a = '<span class="myClass">' + t.val() + "</span>",
                        s = $(e).children().last();
                    $(e).children().last().remove(), $(e).html($(e).html().replace(n, a)), $(e).append("<td>" + $(s).html() + "</td>"), $(this).removeClass("hiddenTr")
        }
        }), 0 == $(".table-hover>tbody>Tr").not(".hiddenTr").length) {
            var e = $(".table-hover>tbody>Tr").children().length;
            $(".table-hover>tbody").append('<tr class="ntFound"><td  colspan="' + e + '">هیچ نتیجه ای برای ' + $(t).val() + " یافت نشد ...</td></tr>")
        }
    } else $(".hiddenTr").each(function () {
        $(this).removeClass("hiddenTr")
    })
}), $(document).on("change", "#statusOrder", function () {
    $("#submit").click()
}), $(document).on("change", "#filterOrders", function () {
    var t = $(this);
    "noFilter" == $(t).val() ? $(".statusFilter").each(function () {
        $(this).parent().removeClass("hidden")
    }) : $(".statusFilter").each(function () {
        $(this).text().trim() != $(t).val().trim() ? $(this).parent().addClass("hidden") : $(this).parent().removeClass("hidden")
    })
});