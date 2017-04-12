/**
* Manages the Cycle Priorities Actions
*/
var CyclePrioritiesManager = function (cyclePrioritiesRequestHandler) {

    // Private variables
    var self = this;
    var CYCLE_PRIORITIES_URL = '/GoalTracking/PrioritiesManagement';
    var TASK_STATUS_BUTTON = '.changeStatusButton';
    var QUARTER_SEARCH_BUTTON = "#quarterButton";
    var TASK_ROW = '.priority-task-row';
    var CONFIRM_TASK_STATUS_CHANGE = 'Do you want to change task status to ';
    var STATUS_NOT_STARTED = 0;
    var STATUS_STARTED = 1;
    var STATUS_COMPLETED = 2;
    
    // Initialize functionality
    self.init = function () {
        // Attach Change Status Task Event 
        jQuery(document).on('click', TASK_STATUS_BUTTON, confirmTaskStatus);

        // Attach searchPriorities event
        jQuery(document).on('click', QUARTER_SEARCH_BUTTON, searchPrioritiesQuarter);
    };

    // Change task status   
    function confirmTaskStatus() {
        var currentTask = jQuery(this).closest(TASK_ROW);
        var taskId = currentTask.data('task-id');
        var confirmMessage = createConfirmMessage(currentTask);

        myApp.Dialogs.alertConfirm("Change Status",confirmMessage, function (result) {
            if (result) {
                changeTaskStatus(taskId);
            }
        });
    };

    // Create the confirm change status message
    function createConfirmMessage(currentTask) {
        var taskStatus = currentTask.data('task-status');
        var taskStatusLabel = currentTask.find('img').attr('title');
        var taskGoal = currentTask.find('div:nth-child(2)').text().trim();
        var newStatus = '';

        switch (taskStatus) {
            case STATUS_NOT_STARTED:
                newStatus = 'Started';
                break;
            case STATUS_STARTED:
                newStatus = 'Completed';
                break;
            case STATUS_COMPLETED:
                newStatus = 'Not Started';
                break;
            default:
                myApp.Dialogs.alertDanger('Error','Task status error [' + taskStatus + ']');
                return;
        }

        return 'Do you want to change <b>' + taskGoal + '</b> status from <b>' + taskStatusLabel + '</b> to <b>' + newStatus + '</b>?';
    }

    // Change Task Status 
    function changeTaskStatus(taskId) {
        cyclePrioritiesRequestHandler
            .changeTaskStatus(taskId)
            .success(function () {
                redirectToCyclePriorities();
            });
    };

    function searchPrioritiesQuarter() {
        var quarter = $("#ListOfQuarters").val();
        var year = $("#ListOfYears").val();
        window.location.href = CYCLE_PRIORITIES_URL + "?quarter=" + quarter + "&year=" + year;
    };

    // Redirects to cycle priorities
    function redirectToCyclePriorities() {
        window.location.href = CYCLE_PRIORITIES_URL;
    }
};
