/**
* Provides ajax requests for Manager's Check module
*/
var ManagersCheckRequestHandler = function () {

    // Private variables
    var self = this;
    var SAVE_MANAGERS_CHECK_SURVEY = './SaveManagersCheckSurvey';

    // Save Manager's Check Survey
    self.saveSurvey = function (surveyData) {
        return myApp.Requests.postRequest({
            'url': SAVE_MANAGERS_CHECK_SURVEY,
            'data': { data: JSON.stringify(surveyData) }
        });
    };
};