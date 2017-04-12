/**
* Provides ajax requests for Employee Goals module
*/
var EmployeeGoalsRequestHandler = function () {

    // Private variables
    var self = this;
    var VERIFY_TASK_URL = './VerifyTask';

    // Verify Task
    self.verifyTask = function (taskId) {
        return myApp.Requests.postRequest({
            'url': VERIFY_TASK_URL + '?taskId=' + taskId
        });
    };
};