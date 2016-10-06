var columnNumber = 0;

var setCursorWait = function () {
    $('html,body').css('cursor', 'wait');

}
var setCursorDone = function () {
    $('html,body').css('cursor', 'auto');
}

$(document).ready(function () {
    $('body').on("submit", ".legislator-zip", function (event) {
        event.preventDefault();
        setCursorWait();
        $.ajax({
            url: '/Home/LocateLegislator',
            type: 'GET',
            data: $(this).serialize(),
            dataType: 'html',
            success: function (result) {
            setCursorDone();
                $('#lookup').html(result);
            }
        });
    });
    $('body').on("submit", ".column-selector", function (event) {
        event.preventDefault();
        setCursorWait();
        $.ajax({
            url: '/Home/ViewHistoryLookup',
            type: 'get',
            data: $(this).serialize(),
            dataType: 'html',
            success: function (result) {
            setCursorDone();
                if (columnNumber >= 3) {
                    $('#result' + (columnNumber - 3)).hide();
                    $('#result-container').append('<div class="col-sm-4" id="result' + columnNumber + '"></div>');
                }
                $('#result' + columnNumber).html(result);
                columnNumber++;
            }
        });
    });
});

var legislatorDetail = function (id) {
    //$('span').hide();
    setCursorWait();
    $.ajax({
        url: '/Home/LegislatorDetail',
        type: 'get',
        data: { columnId: columnNumber, bioguide: id },
        dataType: 'html',
        success: function (result) {
            setCursorDone();
            if (columnNumber >= 3) {
                $('#result' + (columnNumber - 3)).hide();
                $('#result-container').append('<div class="col-sm-4" id="result' + columnNumber + '"></div>');
            }
            $('#result' + columnNumber).html(result);
            columnNumber++;
            updateViewHistory();
        }
    });
};

var committeeDetail = function (committeeId) {
    setCursorWait();
    $.ajax({
        url: '/Home/CommitteeDetail',
        type: 'get',
        data: { columnId: columnNumber, committeeId: committeeId },
        dataType: 'html',
        success: function (result) {
            setCursorDone();
            if (columnNumber >= 3) {
                $('#result' + (columnNumber - 3)).hide();
                $('#result-container').append('<div class="col-sm-4" id="result' + columnNumber + '"></div>');
            }
            $('#result' + columnNumber).html(result);
            columnNumber++;
            updateViewHistory();

        }
    });
};

var billDetail = function (billId) {
    setCursorWait();
    $.ajax({
        url: '/Home/BillDetail',
        type: 'get',
        data: { columnId: columnNumber, billId: billId },
        dataType: 'html',
        success: function (result) {
            setCursorDone();
            if (columnNumber >= 3) {
                $('#result' + (columnNumber - 3)).hide();
                $('#result-container').append('<div class="col-sm-4" id="result' + columnNumber + '"></div>');
            }
            $('#result' + columnNumber).html(result);
            columnNumber++;
            updateViewHistory();

        }
    });
};

var committeeLegislatorDetail = function (id) {
    setCursorWait();
    $.ajax({
        url: '/Home/LegislatorDetail',
        type: 'get',
        data: { columnId: columnNumber, bioguide: id },
        dataType: 'html',
        success: function (result) {
            setCursorDone();
            if (columnNumber >= 3) {
                $('#result' + (columnNumber - 3)).hide();
                $('#result-container').append('<div class="col-sm-4" id="result' + columnNumber + '"></div>');
            }
            $('#result' + columnNumber).html(result);
            columnNumber++;
            updateViewHistory();
        }
    });
};

var updateViewHistory = function () {
    setCursorWait();
    $.ajax({
        url: '/Home/ViewHistory',
        type: 'get',
        dataType: 'html',
        success: function (result) {
            setCursorDone();
            $('.view-history-form').html(result);
        }
    });
};