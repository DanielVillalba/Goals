/**
* Provides any ajax request needed
*/
var OnePagePlanRequestsHandler = function () {

    // Private variables
    var self = this;
    var ADD_ONE_PAGE_PLAN_URL = './AddOnePagePlan';
    var UPDATE_ONE_PAGE_PLAN_URL = './UpdateOnePagePlan';


    // Add
    self.addQuarterlyPriorities = function (data) {
        return myApp.Requests.postRequest({
            'url': ADD_ONE_PAGE_PLAN_URL,
            'data': { data: JSON.stringify(data) }
        });
    };

    // Update
    self.updateOnePagePlan = function (data) {
        return myApp.Requests.postRequest({
            'url': UPDATE_ONE_PAGE_PLAN_URL,
            'data': { data: JSON.stringify(data) }
        });
    };
};
