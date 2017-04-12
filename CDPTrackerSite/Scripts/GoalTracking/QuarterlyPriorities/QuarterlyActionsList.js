/**
* Manages Quarterly Actions List
*/
var QuarterlyActionsList = function (_list, _template) {

    // Private variables
    var self = this;
    var LIST = _list;
    var TEMPLATE = _template;
    var ROW = '.fancy-row';
    var ROWS_CONTAINER = '.fancy-body';
    var ADD_ROW_BUTTON = '.add-row-btn';
    var REMOVE_ROW_BUTTON = '.remove-row-btn';
    var REMOVE_MESSAGE = 'Are you sure you want to remove this quarterly action?';

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
            messages.push('- At least one Quarterly Action is required');

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
                if (!row.due_date || !myApp.Utils.parseDateMDY(row.due_date)) {
                    messages.push(rowNumber + 'Due Date must have MM/DD/YYYY format');
                    $row.find('.txtDueDate').addClass('border-danger');
                }
                if (row.key_initiative % 1 !== 0 || row.key_initiative < 0) {
                    messages.push(rowNumber + 'Select a Key Initiative');
                    $row.find('.cbKeyInitiative').addClass('border-danger');
                }
            });
        }
        
        if (messages.length > 0) {
            messages = ['<strong><u>Quarterly Actions</u></strong>'].concat(messages);
        }

        return messages;
    };

    // Add New row to List
    function addRow() {
        var container = LIST.find(ROWS_CONTAINER);
        var template = jQuery(TEMPLATE).find('> div').clone();
        container.append(template);
        template.fadeIn(150);

        // Add datepicker
        var dp = template.find('input.date');
        dp.removeClass("hasDatepicker").removeAttr('id');
        dp.datepicker({
            changeMonth: true,
            changeYear: true,
            showButtonPanel: true
        });
    };

    // Get all Data
    function getListData() {
        var list = [];

        LIST.find(ROWS_CONTAINER).children().each(function (index) {
            var row = {};
            row.id = jQuery(this).data("quarterly-action-id");
            row.action = jQuery(this).find(".txtAction").val().trim();
            row.due_date = jQuery(this).find(".txtDueDate").val().trim();
            row.key_initiative = jQuery(this).find(".cbKeyInitiative").val();
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
