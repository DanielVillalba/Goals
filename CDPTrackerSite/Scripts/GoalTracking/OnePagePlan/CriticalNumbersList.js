/**
* Manages List
*/
var CriticalNumbersList = function (_list) {

    // Private variables
    var self = this;
    var LIST = _list;
    var TEMPLATE = '';
    var ROW = '.fancy-row';
    var ROWS_CONTAINER = '.fancy-body';
    var ADD_ROW_BUTTON = '.add-row-btn';
    var REMOVE_ROW_BUTTON = '.remove-row-btn';
    var REMOVE_MESSAGE = 'Are you sure you want to remove this item?';

    // Initialize
    self.init = function () {

    };

    // Get data
    self.getListData = function () {
        return getListData();
    };

    // Validate Data
    self.validateData = function (rows) {
        var messages = [];

        if (!rows || rows.length != 3) {
            messages.push('- Three Critical Numbers are required');

        } else {
            jQuery.each(rows, function (index, row) {
                var $row = LIST.find(ROWS_CONTAINER).children().eq(index);
                var rowNumber = $row.find('.row-label').html() + ': ';

                if (!row.critical_number || row.critical_number == '') {
                    messages.push(rowNumber + 'Critical Number is required');
                    $row.find('.txtCriticalNumber').addClass('border-danger');

                }
                /*
                if (row.id % 1 !== 0 || row.id < 0) {
                    messages.push(rowNumber + 'Invalid Id');
                }
                if (!row.critical_number || row.critical_number == '') {
                    messages.push(rowNumber + 'Critical Number is required');
                    $row.find('.txtCriticalNumber').addClass('border-danger');

                }
                if (parseInt(row.critical_number) > 2147483647) {
                    messages.push(rowNumber + 'Critical Number exceed maximun value');
                    $row.find('.txtCriticalNumber').addClass('border-danger');
                }
                else if (myApp.Utils.isInteger(row.critical_number) == false) {
                    messages.push(rowNumber + 'Critical Number must be an integer');
                    $row.find('.txtCriticalNumber').addClass('border-danger');
                }*/
            });
        }

        if (messages.length > 0) {
            messages = ['<strong><u>Critical Numbers</u></strong>'].concat(messages);
        }

        return messages;
    };

    // Get all Data
    function getListData() {
        var list = [];

        LIST.find(ROWS_CONTAINER).children().each(function (index) {
            var row = {};
            row.id = jQuery(this).data("critical-number-id");
            row.critical_number = jQuery(this).find(".txtCriticalNumber").val();
            list.push(row);
        });

       // console.debug("", list);

        return list;
    }
};
