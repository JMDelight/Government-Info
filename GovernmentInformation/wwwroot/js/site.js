$(document).ready(function () {
    $('body').on("submit", ".legislator-zip", function (event) {
        event.preventDefault();
        console.log("Hello");
        $.ajax({
            url: '/Home/LocateLegislator',
            type: 'GET',
            data: $(this).serialize(),
            dataType: 'html',
            success: function (result) {
                console.log('Hello');
                $('#result1').html(result);
            }
        });
    });
})

var legislatorDetail = function (id) {
    $('span').hide();
    console.log(id);
    $.ajax({
        url: '/Home/LegislatorDetail',
        type: 'get',
        data: { bioguide: id },
        dataType: 'html',
        success: function (result) {
            $('span#' + id).show();
            $('span#' + id).html(result);
        }
    });
}