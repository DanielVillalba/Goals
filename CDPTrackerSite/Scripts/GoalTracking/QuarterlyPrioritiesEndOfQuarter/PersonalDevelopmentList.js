﻿/**
* Manages Personal Development List
*/
var PersonalDevelopmentList = function (_list, _template) {

    // Private variables
    var self = this;
    var LIST = _list;
    var TEMPLATE = _template;
    var ROW = '.fancy-row';
    var ROWS_CONTAINER = '.fancy-body';
    var ADD_ROW_BUTTON = '.add-row-btn';
    var REMOVE_ROW_BUTTON = '.remove-row-btn';
    var REMOVE_MESSAGE = 'Are you sure you want to remove this personal development?';

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
            messages.push('- At least one Personal Development is required');

        } else {
            jQuery.each(rows, function (index, row) {
                var rowNumber = 'Row ' + (index + 1) + ': ';
                var $row = LIST.find(ROWS_CONTAINER).children().eq(index);

                if (row.id % 1 !== 0 || row.id < 0) {
                    messages.push(rowNumber + 'Invalid Id');
                }
                if (!row.skill || row.skill == '') {
                    messages.push(rowNumber + 'Skill is required');
                    $row.find('.txtSkill').addClass('border-danger');
                }
                if (!row.definition_of_success || row.definition_of_success == '') {
                    messages.push(rowNumber + 'Definition of Success is required');
                    $row.find('.txtDefinitionOfSuccess').addClass('border-danger');
                }
                if (!row.outcome || row.outcome == '') {
                    messages.push(rowNumber + 'Outcome is required');
                    $row.find('.txtOutcome').addClass('border-danger');
                }
            });
        }

        if (messages.length > 0) {
            messages = ['<strong><u>Personal Development</u></strong>'].concat(messages);
        }

        return messages;
    };

    // Add New Row to List
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
            row.id = jQuery(this).data("personal-development-id");
            row.skill = jQuery(this).find(".txtSkill").val().trim();
            row.definition_of_success = jQuery(this).find(".txtDefinitionOfSuccess").val().trim();
            row.outcome = jQuery(this).find(".txtOutcome").val().trim();
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