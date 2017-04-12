/**
* Provides ajax requests for Cycle Priorities module
*/
var CyclePrioritiesRequestHandler = function () {

    // Private variables
    var self = this;
    var CHANGE_TASK_STATUS_URL = './ChangeTaskStatus';

    // Change Priority Status
    self.changeTaskStatus = function (taskId) {
        return myApp.Requests.postRequest({
            'url': CHANGE_TASK_STATUS_URL + '?taskId=' + taskId
        });
    };
};
