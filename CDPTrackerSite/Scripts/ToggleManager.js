var ToggleManager = function () {
    var TOGGLE_ALL_BUTTON = '.toggleSmallButton';
    var IMG_TOGGLE_ALL = '#imgToggleAll';
    var LBL_TOGGLE_ALL = "#labelToggle";
    
    // Initialize functionality
    this.init = function () {
        //Toggle All
        jQuery(document).on('click', TOGGLE_ALL_BUTTON, changeExpandCollapseAll);
    };

    function changeExpandCollapseAll() {
        var currentStatus = jQuery(IMG_TOGGLE_ALL).attr('src');
        var allPanels = jQuery('.employeeGoalsContainer .employeeGoals .panel-collapse');

        if (currentStatus.indexOf("expand") >= 0) {
            jQuery(IMG_TOGGLE_ALL).attr('src', '/Content/images/collapse.png');
            jQuery(LBL_TOGGLE_ALL).text('Collapse All');
            allPanels.collapse('show');
        }
        else {
            jQuery(IMG_TOGGLE_ALL).attr('src', '/Content/images/expand.png');
            jQuery(LBL_TOGGLE_ALL).text('Expand All');
            allPanels.collapse('hide');
        }
    };
}
