/**
* Provides ajax requests for Team Members Input module
*/
var TeamMembersInputRequestHandler = function () {

    // Private variables
    var self = this;
    var SAVE_TEAM_MEMBERS_INPUT_SURVEY = './SaveTeamMembersInputSurvey';

    // Save Team Members Input Survey
    self.saveSurvey = function (surveyData) {
        return myApp.Requests.postRequest({
            'url': SAVE_TEAM_MEMBERS_INPUT_SURVEY,
            'data': { data: JSON.stringify(surveyData) }
        });
    };
};