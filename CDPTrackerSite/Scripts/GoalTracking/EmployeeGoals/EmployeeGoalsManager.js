/**
* Manages the Employee Goals Actions
*/
var EmployeeGoalsManager = function (employeeGoalsRequestHandler) {

    // Private variables
    var self = this;
    var EMPLOYEE_GOALS_URL = '/GoalTracking/EmployeeGoals';
    var TASK_VERIFY_BUTTON = '.changeStatusButton';
    var TASK_ROW = '.priority-task-row';
    var VERIFIED_BY_MANAGER = 'Verified by manager';
    var QUARTER_SEARCH_BUTTON = "#quarterButton";

    // Initialize functionality
    self.init = function () {
        // Attach Change Status Task Event 
        jQuery(document).on('click', TASK_VERIFY_BUTTON, confirmVerifyTask);
        // Attach searchEmployeeGoals event
        jQuery(document).on('click', QUARTER_SEARCH_BUTTON, searchEmployeeGoalsQuarter);
    };

    // Change task status   
    function confirmVerifyTask() {
        var currentTask = jQuery(this).closest(TASK_ROW);
        var taskId = currentTask.data('task-id');
        var confirmMessage = createConfirmMessage(currentTask);

        myApp.Dialogs.alertConfirm('Verify Task',confirmMessage, function (result) {
            if (result) {
                verifyTask(taskId);
            }
        });
    };

    // Create the confirm change status message
    function createConfirmMessage(currentTask) {
        var taskGoal = currentTask.find('div:nth-child(2)').text().trim();
        var taskStatusLabel = currentTask.find('img').attr('title');
        var option = 'VERIFY';

        if (taskStatusLabel == VERIFIED_BY_MANAGER) {
            option = 'UNVERIFY';
        }

        return 'Do you want to ' + option + ' task <b>' + taskGoal + '</b>?';
    }

    // Change Task Status 
    function verifyTask(taskId) {
        employeeGoalsRequestHandler
            .verifyTask(taskId)
            .success(function () {
                searchEmployeeGoalsQuarter();
            });
    };

    function searchEmployeeGoalsQuarter() {
        var quarter = $("#ListOfQuarters").val();
        var year = $("#ListOfYears").val();
        window.location.href = EMPLOYEE_GOALS_URL + "?quarter=" + quarter + "&year=" + year;
    };

    // Redirects to employee goals
    function redirectToEmployeeGoals() {
        window.location.href = EMPLOYEE_GOALS_URL;
    }
};
