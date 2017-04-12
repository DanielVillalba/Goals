/**
* Manages the Manager's CheckList Actions
* @param {object} listGenerator some parameter
*/
var ManagersCheckListManager = function (listGenerator) {
    // Private variables
    var self = this;
    
    var MANAGERS_CHECKLIST_URL = '/GoalTracking/ManagersCheckList';
    var MANAGERS_CHECK_URL = '/GoalTracking/ManagersCheck';
    var INDEX = '/GoalTracking/Index';
    var QUARTER_SEARCH_BUTTON = "#quarterButton";
    var MANAGERS_CHECK_BUTTON = '.managersCheckButton';

    // Initialize
    self.init = function (members) {
        // Attach searchQuarter event
        jQuery(document).on('click', QUARTER_SEARCH_BUTTON, searchQuarter);
        jQuery(document).on('click', MANAGERS_CHECK_BUTTON, goToManagersCheckSurvey);
    };
    
    function searchQuarter() {
        var quarter = $("#ListOfQuarters").val();
        var year = $("#ListOfYears").val();
        window.location.href = MANAGERS_CHECKLIST_URL + "?quarter=" + quarter + "&year=" + year;
    };

    function goToManagersCheckSurvey(evt) {
        var quarter = $("#ListOfQuarters").val();
        var year = $("#ListOfYears").val();
        var id = evt.currentTarget.getAttribute("resourceId");
        window.location.href = MANAGERS_CHECK_URL + "?id="+id+"&quarter=" + quarter + "&year=" + year;
    };

    function redirectToIndex() {
        window.location.href = INDEX;
    }


};
