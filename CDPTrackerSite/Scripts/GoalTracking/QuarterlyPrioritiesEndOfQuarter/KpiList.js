/**
* Manages KPI List
*/
var KpiList = function (_list, _template) {

    // Private variables
    var self = this;
    var LIST = _list;
    var TEMPLATE = _template;
    var ROW = '.fancy-row';
    var ROWS_CONTAINER = '.fancy-body';
    var ADD_ROW_BUTTON = '.add-row-btn';
    var REMOVE_ROW_BUTTON = '.remove-row-btn';
    var REMOVE_MESSAGE = 'Are you sure you want to remove this Kpi?';

    // Initialize
    self.init = function () {
        // Attach Add Row Event
        LIST.find(ADD_ROW_BUTTON).click(addRow);

        // Attach Remove Row Event
        LIST.find(ROWS_CONTAINER).on('click', REMOVE_ROW_BUTTON, confirmRemoveRow);

        // Add a new empty Row
        if (LIST.find(ROWS_CONTAINER).children().length == 0) {
            addRow();
        }
    };

    // Get data
    self.getListData = function () {
        return getListData();
    };

    // Validate Data
    self.validateData = function (rows) {
        var messages = [];

        if (!rows || rows.length == 0) {
            messages.push('- At least one KPI is required');

        } else {
            jQuery.each(rows, function (index, row) {
                var rowNumber = 'Row ' + (index + 1) + ': ';
                var $row = LIST.find(ROWS_CONTAINER).children().eq(index);

                if (row.id % 1 !== 0 || row.id < 0) {
                    messages.push(rowNumber + 'Invalid Id');
                }
                if (!row.name || row.name == '') {
                    messages.push(rowNumber + 'KPI Name is required');
                    $row.find('.txtName').addClass('border-danger');
                }
                if (!row.goal || row.goal == '') {
                    messages.push(rowNumber + 'Goal is required');
                    $row.find('.txtGoal').addClass('border-danger');
                }
                if (!row.results || row.results == '') {
                    messages.push(rowNumber + 'Results are required');
                    $row.find('.txtResults').addClass('border-danger');
                }
            });
        }

        if (messages.length > 0) {
            messages = ['<strong><u>KPI</u></strong>'].concat(messages);
        }

        return messages;
    };

    // Add New row to List
    function addRow() {
        var container = LIST.find(ROWS_CONTAINER);
        var template = jQuery(TEMPLATE).find('> div').clone();
        container.append(template);
        template.fadeIn(150);
    };

    // Get all Data
    function getListData() {
        var list = [];

        LIST.find(ROWS_CONTAINER).children().each(function (index) {
            var row = {};
            row.id = jQuery(this).data("kpi-id");
            row.name = jQuery(this).find(".txtName").val().trim();
            row.goal = jQuery(this).find(".txtGoal").val().trim();
            row.results = jQuery(this).find(".txtResults").val().trim();
            list.push(row);
        });

        return list;
    }

    // Confirm removal of selected row
    function confirmRemoveRow() {
        var currentRow = jQuery(this).closest(ROW);

        myApp.Dialogs.alertConfirm('Remove', REMOVE_MESSAGE, function (result) {
            if (result) {
                removeRow(currentRow);
            }
        });
    };

    // Removes row
    function removeRow(row) {
        row.slideUp(1000).remove();
    }
};
