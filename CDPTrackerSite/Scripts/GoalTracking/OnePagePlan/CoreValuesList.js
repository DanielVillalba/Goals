/**
* Manages Cores Values List
*/
var CoreValuesList = function (_list, selectedQuarter, val1, val2, val3) {

    // Private variables
    var self = this;
    var LIST = _list;
    var cv1 = val1;
    var cv2 = val2;
    var cv3 = val3;

    var TEMPLATE = '';
    var ROW = '.fancy-row';
    var ROWS_CONTAINER = '.fancy-body';
    var ADD_ROW_BUTTON = '.add-row-btn';
    var REMOVE_ROW_BUTTON = '.remove-row-btn';
    var REMOVE_MESSAGE = 'Are you sure you want to remove this item?';

    // Initialize
    self.init = function () {
        // Set correct labels and core values depending on the selected quarter
      
        setSelectedQuarterValues(selectedQuarter);
    };

    // Get data
    self.getListData = function () {
        return getListData();
    };

    // Validate Data
    self.validateData = function (rows) {
        var messages = [];
        var selectedIds = [];

        if (!rows || rows.length == 0) {
            messages.push('- At least one Core Value is required');

        } else {
            jQuery.each(rows, function (index, row) {
                var $row = LIST.find(ROWS_CONTAINER).children().eq(index);
                var rowNumber = $row.find('.row-label').html() + ': ';
                
                if (row.id % 1 !== 0 || row.id < 0) {
                    messages.push(rowNumber + 'Invalid Id');
                }
                if (!row.core_value || row.core_value == '') {
                    messages.push(rowNumber + 'Core Value is required');
                    $row.find('.cbCoreValue').addClass('border-danger');
                } else {
                    if (selectedIds.length > 0 && selectedIds.indexOf(row.core_value) >= 0) {
                        messages.push(rowNumber + 'Core Value cannot be repeated');
                        $row.find('.cbCoreValue').addClass('border-danger');
                    }
                    selectedIds.push(row.core_value);
                }
            });
        }

        if (messages.length > 0) {
            messages = ['<strong><u>Core Values</u></strong>'].concat(messages);
        }

        return messages;
    };

    // Get all Data
    function getListData() {
        var list = [];

        LIST.find(ROWS_CONTAINER).children().each(function (index) {
            var row = {};
           
            row.id = jQuery(this).data("core-value-id");
            row.core_value = jQuery(this).find(".cbCoreValue").val();
            list.push(row);
        });
       // myapp.Dialogs.alertDanger(list[0].id,list[0].core_value);
       // console.debug("", list);
        return list;
    }

    function setSelectedQuarterValues(selectedQuarter) {
        if (!selectedQuarter || !myApp.Utils.isInteger(selectedQuarter) || selectedQuarter < 0 || selectedQuarter > 4) {
            myApp.Dialogs.alertDanger('Error', 'Problem getting the selected quarter');

        } else {
            switch (parseInt(selectedQuarter)) {
                case 1:
                    setFirstQuarterValues();
                    break;
                case 2:
                    setSecondQuarterValues();
                    break;
                case 3:
                    setThirdQuarterValues();
                    break;
                case 4:
                    setFourthQuarterValues();
                    break;
            }
        }
    }

    function setFirstQuarterValues() {
       

        setQuarterValues([
            { label: 'January', id: cv1 },
            { label: 'February', id: cv2 },
            { label: 'March', id: cv3 }
        ]);
    }
    function setSecondQuarterValues() {
        setQuarterValues([
            { label: 'April', id: cv1 },
            { label: 'May', id: cv2 },
            { label: 'June', id: cv3 }
        ]);
    }

    function setThirdQuarterValues() {
        setQuarterValues([
            { label: 'July', id: cv1 },
            { label: 'August', id: cv2 },
            { label: 'September', id: cv3 }
        ]);
    }

    function setFourthQuarterValues() {
        setQuarterValues([
            { label: 'October', id: cv1 },
            { label: 'November', id: cv2},
            { label: 'December', id: cv3 }
        ]);
    }

    function setQuarterValues(quarterValues) {
        var list = LIST.find(ROWS_CONTAINER).children();

        quarterValues.forEach(function (value, index) {
            var $row = list.eq(index);
            $row.find('.row-label').html(value.label);
            $row.find('.cbCoreValue').val(value.id);
        });
    }
};
