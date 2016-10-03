var columnNumber = 0;

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
                $('#lookup').html(result);
            }
        });
    });
});

var legislatorDetail = function (id) {
    //$('span').hide();
    console.log(id);
    $.ajax({
        url: '/Home/LegislatorDetail',
        type: 'get',
        data: { columnId: columnNumber, bioguide: id },
        dataType: 'html',
        success: function (result) {
            if (columnNumber >= 3) {
                $('#result' + (columnNumber - 3)).hide();
                $('#result-container').append('<div class="col-sm-4" id="result' + columnNumber + '"></div>');
            }
            $('#result' + columnNumber).html(result);
            columnNumber ++;
            //$('span#' + id).show();
            //$('span#' + id).html(result);
        }
    });
};

var committeeDetail = function (committeeId) {
    console.log(committeeId);
    $.ajax({
        url: '/Home/CommitteeDetail',
        type: 'get',
        data: { columnId: columnNumber, committeeId: committeeId },
        dataType: 'html',
        success: function (result) {
            if (columnNumber >= 3) {
                $('#result' + (columnNumber - 3)).hide();
                $('#result-container').append('<div class="col-sm-4" id="result' + columnNumber + '"></div>');
            }
            $('#result' + columnNumber).html(result);
            columnNumber ++;
        }
    });
};

var billDetail = function (billId) {
    console.log(billId);
    $.ajax({
        url: '/Home/BillDetail',
        type: 'get',
        data: { columnId: columnNumber, billId: billId },
        dataType: 'html',
        success: function (result) {
            if (columnNumber >= 3) {
                $('#result' + (columnNumber - 3)).hide();
                $('#result-container').append('<div class="col-sm-4" id="result' + columnNumber + '"></div>');
            }
            $('#result' + columnNumber).html(result);
            columnNumber ++;
        }
    });
};

var committeeLegislatorDetail = function (id) {
    $.ajax({
        url: '/Home/LegislatorDetail',
        type: 'get',
        data: { columnId: columnNumber, bioguide: id },
        dataType: 'html',
        success: function (result) {
            if (columnNumber >= 3) {
                $('#result' + (columnNumber - 3)).hide();
                $('#result-container').append('<div class="col-sm-4" id="result' + columnNumber + '"></div>');
            }
            $('#result' + columnNumber).html(result);
            columnNumber ++;
        }
    });
};