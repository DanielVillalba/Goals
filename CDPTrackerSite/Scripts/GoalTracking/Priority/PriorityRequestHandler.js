/**
* Provides any ajax request needed
*/
var PriorityRequestHandler = function () {

    // Private variables
    var self = this;
    var ADD_PRIORITY_URL = './AddPriority';
    var UPDATE_PRIORITY_URL = './UpdatePriority';
    var DELETE_PRIORITY_URL = './DeletePriority';
    

    // Add Priority
    self.addPriority = function (priorityData) {
        return myApp.Requests.postRequest({
            'url': ADD_PRIORITY_URL,
            'data': { data: JSON.stringify(priorityData) }
        });
    };

    // Update Priority
    self.updatePriority = function (priorityData) {
        return myApp.Requests.postRequest({
            'url': UPDATE_PRIORITY_URL,
            'data': { data: JSON.stringify(priorityData) }
        });
    };

    // Delete Priority
    self.deletePriority = function (priorityId) {
        return myApp.Requests.postRequest({
            'url': DELETE_PRIORITY_URL + '?priorityId=' + priorityId
        });
    };
};
