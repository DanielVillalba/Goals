/**
* Manages the TeamMembersInput Actions
*/
var TeamMembersInputManager = function (surveyGenerator, surveyAnswers, teamMembersInputRequestHandler) {
    // Private variables
    var self = this;
    var QUESTIONS_CONTAINER = '#questionsContainer';
    var SAVE_SURVEY_BUTTON = '#saveSurvey';
    var IS_PROCESS_RUNNING = false;
    var INDEX = '/GoalTracking/Index/';
    
    // Initialize
    self.init = function (survey) {
        // Generate Survey
        surveyGenerator.generateSurvey(survey);

        // Attach save survey event
        jQuery(document).on('click', SAVE_SURVEY_BUTTON, validateSurvey);
    };

    // Validate Survey Event
    function validateSurvey() {
        if (IS_PROCESS_RUNNING == false) {
            IS_PROCESS_RUNNING = true;
            var surveyData = surveyAnswers.getSurveyData();

            if (surveyData.answers) {
                var questionsContainer = jQuery(QUESTIONS_CONTAINER);
                surveyId = questionsContainer.attr("data-surveyid");
                surveyData.surveyId = surveyId;
                saveSurvey(surveyData);
            }

            // Lets give 2 second delay until next time user can click save button again
            setTimeout(function () {
                IS_PROCESS_RUNNING = false;
            }, 2000);
        }
    }

    // Save Survey
    function saveSurvey(surveyData) {
        teamMembersInputRequestHandler
            .saveSurvey(surveyData)
            .success(function () {
                myApp.Dialogs.alertSuccess('Saved','Survey Saved!', {
                    callback: function () {
                        redirectToIndex();
                    }
                });
            });
    };

    function redirectToIndex() {
        window.location.href = INDEX;
    }
};
