/**
* Manages Values Infusion List
*/
var ValuesInfusionList = function (_list) {

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

        if (!rows || rows.length == 0) {
            messages.push('- At least one Value Infusion is required');

        } else {
            jQuery.each(rows, function (index, row) {
                var rowNumber = 'Row ' + (index + 1) + ': ';
                var $row = LIST.find(ROWS_CONTAINER).children().eq(index);

                if (row.id % 1 !== 0 || row.id < 0) {
                    messages.push(rowNumber + 'Invalid Id');
                }
                if (!row.action || row.action == '') {
                    messages.push(rowNumber + 'Action is required');
                    $row.find('.txtAction').addClass('border-danger');
                }
            });
        }

        if (messages.length > 0) {
            messages = ['<strong><u>Values Infusion</u></strong>'].concat(messages);
        }

        return messages;
    };

    // Get all Data
    function getListData() {
        var list = [];

        LIST.find(ROWS_CONTAINER).children().each(function (index) {
            var row = {};
            row.id = jQuery(this).data("value-infusion-id");
            row.action = jQuery(this).find(".txtAction").val().trim();
            row.value_checked = (jQuery(this).find('.chValue').is(':checked') ? 1 : 0);
            //console.debug(row);
            list.push(row);
        });

        return list;
    }
};
