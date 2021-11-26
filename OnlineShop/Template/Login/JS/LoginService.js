
$(document).on('submit', '.form-account', function (event) {
    validateInputs(event.target);
    if ($('.WrongFormatInput').length != 0) {
        event.preventDefault();
    }
})


$(document).on('keyup', 'input[data-validator]', function () {
    validateInputs($(this));
})
$(document).on('focusout', 'input[data-validator]', function () {
    validateInputs($(this));
})
function validateInputs(obj) {
    var selected = $(obj);
    if ($(selected).attr('data-validator') == 'mobile') {
        if ($(selected).val().length == 11 && $.isNumeric($(selected).val()) == true && $(selected).val().substr(0, 2) == '09') {
            $(selected).removeClass('WrongFormatInput');
            $(selected).parent().find('label').find('i').removeClass('WrongFormatInput');
            $(selected).parent().find('.validationError').addClass('hidden')

        } else {
            $(selected).addClass('WrongFormatInput');
            $(selected).parent().find('.validationError').removeClass('hidden')
            $(selected).parent().find('label').find('i').addClass('WrongFormatInput');
        }
    }
    if ($(selected).attr('data-validator') == 'nationalCode') {
        if ($(selected).val().length == 10 && $.isNumeric($(selected).val()) == true) {
            $(selected).removeClass('WrongFormatInput');
            $(selected).parent().find('label').find('i').removeClass('WrongFormatInput');
            $(selected).parent().find('.validationError').addClass('hidden')

        } else {
            $(selected).addClass('WrongFormatInput');
            $(selected).parent().find('.validationError').removeClass('hidden')
            $(selected).parent().find('label').find('i').addClass('WrongFormatInput');
        }
    }
    if ($(selected).attr('data-validator') == 'email') {
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        var vli = regex.test($(selected).val());
        if (vli == true) {
            $(selected).removeClass('WrongFormatInput');
            $(selected).parent().find('label').find('i').removeClass('WrongFormatInput');
            $(selected).parent().find('.validationError').addClass('hidden')
        } else {
            $(selected).addClass('WrongFormatInput');
            $(selected).parent().find('.validationError').removeClass('hidden')
            $(selected).parent().find('label').find('i').addClass('WrongFormatInput');
        }
    }
}
$('html').click(function (e) {
    if (!$(e.target).hasClass('input[data-validator]')) {
        $('.validationError').each(function () {
            $(this).addClass('hidden')
        })
    }
});