var MyResponsesScripts = function () {

    // Private variables
    var self = this;
    var MY_RESPONSES_URL = '/GoalTracking/TeamMembersInputReportLEAA';
    var QUARTER_SEARCH_BUTTON = "#quarterButton";

    // Initialize functionality
    self.init = function () {
        // Attach searchEmployeeGoals event
        jQuery(document).on('click', QUARTER_SEARCH_BUTTON, searchGoalsQuarter);
    };

    function searchGoalsQuarter() {
        var quarter = $("#ListOfQuarters").val();
        var year = $("#ListOfYears").val();
        window.location.href = MY_RESPONSES_URL + "?quarter=" + quarter + "&year=" + year;
    };

};