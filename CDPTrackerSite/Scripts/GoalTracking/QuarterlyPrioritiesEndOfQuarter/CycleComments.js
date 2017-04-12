/**
* Manages Cycle cooments
*/
var CycleComments = function (container) {

    // Private variables
    var self = this;
    var CONTAINER = container;

    // Initialize
    self.init = function () {

    };

    // Get data
    self.getListData = function () {
        return getListData();
    };

    // Validate Data
    self.validateData = function (cycle_comments) {
        var messages = [];
        var $row = CONTAINER.find('.txtCycleComments');

        if (!cycle_comments || cycle_comments == '') {
            messages.push('- Cycle Comments are required');
            $row.addClass('border-danger');            
        }

        if (messages.length > 0) {
            messages = ['<strong><u>Key Issues & Closing Comments</u></strong>'].concat(messages);
        }

        return messages;
    };

    // Get all Data
    function getListData() {
        return CONTAINER.find('.txtCycleComments').val().trim();
    }
};
