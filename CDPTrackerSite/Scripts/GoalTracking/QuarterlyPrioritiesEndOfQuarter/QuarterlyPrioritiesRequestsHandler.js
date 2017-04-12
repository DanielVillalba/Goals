/**
* Provides any ajax request needed
*/
var QuarterlyPrioritiesRequestsHandler = function () {

    // Private variables
    var self = this;
    var ADD_QUARTERLY_PRIORITIES_URL = './AddQuarterlyPrioritiesEndOfQuarter';
    var UPDATE_QUARTERLY_PRIORITIES_URL = './UpdateQuarterlyPrioritiesEndOfQuarter';


    // Add Quarterly Priorities
    self.addQuarterlyPriorities = function (data) {
        return myApp.Requests.postRequest({
            'url': ADD_QUARTERLY_PRIORITIES_URL,
            'data': { data: JSON.stringify(data) }
        });
    };

    // Update Quarterly Priorities
    self.updateQuarterlyPriorities = function (data) {
        return myApp.Requests.postRequest({
            'url': UPDATE_QUARTERLY_PRIORITIES_URL,
            'data': { data: JSON.stringify(data) }
        });
    };
};
