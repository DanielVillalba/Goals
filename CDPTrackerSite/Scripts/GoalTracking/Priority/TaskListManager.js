/**
* Manages Task List
*/
var TaskListManager = function () {

    // Private variables
    var self = this;
    var TASK_LIST = '#taskList';
    var TASK_ROW = '.task-row';
    var TEMPLATE = '#taskTemplate > div';
    var ADD_TASK_BUTTON = '#addTaskButton';
    var REMOVE_TASK_BUTTON = '.removeTaskButton';
    var REMOVE_MESSAGE = 'Are you sure you want to remove this task?';

    // Initialize
    self.init = function () {
        // Attach Add Task Event
        jQuery(ADD_TASK_BUTTON).click(addTask);

        // Attach Remove Task Event
        jQuery(document).on('click', REMOVE_TASK_BUTTON, confirmRemoveTask);

        // Add a new empty Task
        if (jQuery(TASK_LIST).children().length == 0) {
            addTask();
        }
    };

    // Get task data
    self.getTaskListData = function () {
        return getTaskListData();
    };

    // Add Task to List
    function addTask() {
        $('.txtFinishBy').datepicker("destroy");
        $('.txtFinishBy').removeClass("hasDatepicker").removeAttr('id');
        var tasksList = jQuery(TASK_LIST);
        var template = jQuery(TEMPLATE).clone();
        tasksList.append(template);
        template.fadeIn(150);
        $('.txtFinishBy').datepicker();
    };

    // Get all Tasks Data
    function getTaskListData() {
        var taskList = [];

        jQuery(TASK_LIST).find(TASK_ROW).each(function (index) {
            var task = {};
            task.taskId = jQuery(this).data("task-id");
            task.taskDescription = jQuery(this).find(".txtTask").val().trim();
            task.taskProgress = jQuery(this).find(".txtProgress").val().trim();
            task.trainingCategoryId = jQuery(this).find(".cbCategory").val();
            task.sourceId = jQuery(this).find(".cbSource").val();
            task.taskTdu = jQuery(this).find(".txtTdus").val().trim();
            task.taskFinishDate = jQuery(this).find(".txtFinishBy").datepicker("getDate");
            task.taskFinishDate = $.datepicker.formatDate("yy-mm-dd", task.taskFinishDate);
            taskList.push(task);
        });

        return taskList;
    }

    // Confirm removal of selected task
    function confirmRemoveTask() {
        var currentTask = jQuery(this).closest(TASK_ROW);

        myApp.Dialogs.alertConfirm('Remove', REMOVE_MESSAGE, function (result) {
            if (result) {
                removeTask(currentTask);
            }
        });
    };

    // Removes task
    function removeTask(taskRow) {
        taskRow.slideUp(1000).remove();
    }
};
