/**
* Manages the Priority Actions
*/
var PriorityManager = function (priorityConfig, taskListManager, priorityRequestsHandler, Utils) {

    // Private variables
    var self = this;
    var SAVE_PRIORITY_BUTTON = '#savePriority';
    var DELETE_PRIORITY_BUTTON = '#deletePriority';
    var CYCLE_PRIORITIES_URL = '/GoalTracking/PrioritiesManagement';
    var REMOVE_PRIORITY = 'Are you sure you want to delete this priority?';
    var PRIORITY_HAS_VERIFIED_TASKS = 'This priority can not be deleted because it has dependencies with one or more tasks.';
    var IS_NEW_PRIORITY = true;
    var IS_PROCESS_RUNNING = false;

    
    // Initialize functionality
    self.init = function () {
        // Check if we are adding or updating a priority
        if (priorityConfig.priorityId % 1 === 0 && priorityConfig.priorityId > 0) {
            IS_NEW_PRIORITY = false;

            // Attach Delete Priority Event
            jQuery(DELETE_PRIORITY_BUTTON).click(deleteAction);
        }
         
        // Attach Save Priority Event
        jQuery(SAVE_PRIORITY_BUTTON).click(saveAction);

        // Init Task List Actions
        taskListManager.init();
    };

    // Delete Action
    function deleteAction() {
        if (priorityConfig.isPriorityVerified) {
            myApp.Dialogs.alertDanger('Error',PRIORITY_HAS_VERIFIED_TASKS);

        } else {
            myApp.Dialogs.alertConfirm('Remove',REMOVE_PRIORITY, function (result) {
                if (result) {
                    deletePriority();
                }
            });
        }
    }

    // Save Action
    function saveAction() {
        if (IS_PROCESS_RUNNING == false) {
            IS_PROCESS_RUNNING = true;
            var priorityData = getPriorityData();

            if (priorityData) {
                if (IS_NEW_PRIORITY) {
                    addPriority(priorityData);
                } else {
                    updatePriority(priorityData);
                }
            }

            // Lets give 2 second delay until next time user can click save button again
            setTimeout(function () {
                IS_PROCESS_RUNNING = false;
            }, 2000);
        }
    }

    // Add Priority
    function addPriority(priorityData) {
        priorityRequestsHandler
            .addPriority(priorityData)
            .success(function () {
                myApp.Dialogs.alertSuccess('Created','Priority Created!', {
                    callback: function () {
                        redirectToCyclePriorities();
                    }
                });
            });
    };

    // Update Priority
    function updatePriority(priorityData) {
        priorityRequestsHandler
            .updatePriority(priorityData)
            .success(function () {
                myApp.Dialogs.alertSuccess('Updated','Priority Updated!', {
                    callback: function () {
                        redirectToCyclePriorities();
                    }
                });
            });
    };

    // Delete Priority
    function deletePriority() {
        priorityRequestsHandler
            .deletePriority(priorityConfig.priorityId)
            .success(function () {
                myApp.Dialogs.alertSuccess('Deleted','Priority Deleted!', {
                    callback: function () {
                        redirectToCyclePriorities();
                    }
                });
            });
    };

    // Get all the Priority Data
    function getPriorityData() {
        var data = {};

        try {
            data.priorityId = priorityConfig.priorityId;
            data.priorityDescription = jQuery('#Priority').val();
            data.categoryId = jQuery('#Category').val();
            data.priorityYear = jQuery('#Year').val();
            data.priorityQuarter = jQuery('#Quarter').val();
            data.tasks = taskListManager.getTaskListData();

            validatePriorityData(data);

        } catch (err) {
            data = undefined;
            myApp.Dialogs.alertDanger('Error',err);
        }

        return data;
    }

    // Validate all the Priority Data
    function validatePriorityData(priorityData) {
        var messages = [];

        if (!priorityData.priorityDescription || priorityData.priorityDescription == '') {
            messages.push('Prority description cannot be empty');
        }
        if (priorityData.categoryId % 1 !== 0 || priorityData.categoryId < 0) {
            messages.push('Select a category');
        }
        if (priorityData.priorityYear % 1 !== 0 || priorityData.priorityYear < 0) {
            messages.push('Select a year');
        }
        if (priorityData.priorityQuarter % 1 !== 0 || priorityData.priorityQuarter < 0) {
            messages.push('Select a quarter');
        }
        if (priorityData.tasks.length == 1) {
            var taskData = priorityData.tasks[0];

            if (taskData.taskId % 1 === 0 && taskData.taskId == 0 &&
                taskData.taskDescription == '' && taskData.taskFinishDate == ''
            ) {
                priorityData.tasks = [];
            }
        }

        jQuery.each(priorityData.tasks, function (index, taskData) {
            var taskNumber = index + 1;

            if (taskData.taskId % 1 !== 0 || taskData.taskId < 0) {
                messages.push('Task ' + taskNumber + ': Invalid task Id');
            }
            if (!taskData.taskDescription || taskData.taskDescription == '') {
                messages.push('Task ' + taskNumber + ': Task description cannot be empty');
            }
            if (taskData.trainingCategoryId % 1 !== 0 || taskData.trainingCategoryId < 0) {
                messages.push('Task ' + taskNumber + ': Select a category');
            }
            if (taskData.sourceId % 1 !== 0 || taskData.sourceId < 0) {
                messages.push('Task ' + taskNumber + ': Select a source');
            }
            if (taskData.taskTdu < 0) {
                messages.push('Task ' + taskNumber + ': Number of TDUs must be greater than 0');
            }
            if (!taskData.taskFinishDate || !Utils.parseDate(taskData.taskFinishDate)) {
                messages.push('Task ' + taskNumber + ': Finish by Date must have MM/DD/YYYY format');
            }
        });
        
        if (messages.length > 0) {
            throw messages.join('<br>');
        }
    }

    function redirectToCyclePriorities() {
        window.location.href = CYCLE_PRIORITIES_URL;
    }
};
