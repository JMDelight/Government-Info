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

var committeeDetail = function (committeeId) {

    console.log(id);
    $.ajax({
        url: '/Home/CommitteeDetail',
        type: 'get',
        data: { committeeId: committeeId },
        dataType: 'html',
        success: function (result) {
            //$('span#' + id).show();
            //$('span#' + id).html(result);
            console.log(result);
        }
    });
}

var billDetail = function (billId) {

    console.log(id);
    $.ajax({
        url: '/Home/BillDetail',
        type: 'get',
        data: { billId: billId },
        dataType: 'html',
        success: function (result) {
            //$('span#' + id).show();
            //$('span#' + id).html(result);
            console.log(result);
        }
    });
}