/**
* Manages a Fancys Table
*/
var FancyTableManager = function (fancyTable) {

    var self = this;
    //var LIST = _list;
    var TEMPLATE = _template;
    var ROW = '.fancy-row';
    var ROWS_CONTAINER = '.fancy-body';
    var ADD_ROW_BUTTON = '.add-row-btn';
    var REMOVE_ROW_BUTTON = '.remove-row-btn';
    var REMOVE_MESSAGE = 'Are you sure you want to remove this Annual Priority?';


    self.init = function () {
        // Attach Add Row Event
        fancyTable.find(ADD_ROW_BUTTON).click(addRow);

        // Attach Remove Row Event
        fancyTable.find(ROWS_CONTAINER).on('click', REMOVE_ROW_BUTTON, confirmRemoveRow);

        // Add a new empty Row
        if (fancyTable.find(ROWS_CONTAINER).children().length == 0) {
            addRow();
        }
    };


    // Add New row to List
    function addRow() {
        var container = fancyTable.find(ROWS_CONTAINER);
        var template = jQuery(TEMPLATE).find('> div').clone();
        container.append(template);
        template.fadeIn(150);
    };

    // Get all Data
    function getListData() {
        var list = [];

        fancyTable.find(ROWS_CONTAINER).children().each(function (index) {
            var row = {};
            row.id = jQuery(this).data("annual-priority-id");
            row.annual_priority = jQuery(this).find(".txtAnnualPriority").val().trim();
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
