$(document).ready(function () {
    console.log($(window).width());
    $('.carousel').carousel();
    $('.tablet').hide();
    $('.desktop').hide();
    
    if ($(window).width() > 480) {
        $('.tablet').show();
    }

    if ($(window).width() > 1024) {
        $(".desktop").show();
    }

});

CKEDITOR.replace('Ingridients');
CKEDITOR.replace('Instructions');
