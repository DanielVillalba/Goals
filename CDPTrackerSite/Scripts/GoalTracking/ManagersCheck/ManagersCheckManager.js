/**
* Manages the Manager's Check Actions
*/
var ManagersCheckManager = function (surveyGenerator, surveyAnswers, managersCheckRequestHandler) {
    // Private variables
    var self = this;
    var QUESTIONS_CONTAINER = '#questionsContainer';
    var SAVE_SURVEY_BUTTON = '#saveSurvey';
    var GO_BACK_BUTTON = '#returnBtn';
    var MANAGER_DIV = '#managerDiv';
    var INPUT_CHECKBOX = ".parentQuestion .answer input";
    var IS_PROCESS_RUNNING = false;
    var INDEX = '/GoalTracking/Index/';
    var MANAGERS_CHECKLIST_URL = '/GoalTracking/ManagersCheckList';

    // Initialize
    self.init = function (survey) {
        // Generate Survey
        surveyGenerator.generateSurvey(survey);
        jQuery(GO_BACK_BUTTON).hide();

        if (survey.answers.surveyResponses != null) {
            surveyGenerator.showSurveyResponses(survey.answers);
            jQuery(MANAGER_DIV).show();
        }

        if(survey.answers.surveyResponses != null || !survey.canConsult) {
            jQuery(SAVE_SURVEY_BUTTON).hide();
            jQuery(GO_BACK_BUTTON).show();
        }

        // Attach save survey event
        jQuery(document).on('click', SAVE_SURVEY_BUTTON, validateSurvey);

        // Attach toggle question to parents
        jQuery(document).on('click', INPUT_CHECKBOX, toggleQuestion);
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
        managersCheckRequestHandler
            .saveSurvey(surveyData)
            .success(function () {
                myApp.Dialogs.alertSuccess('Saved','Survey Saved!', {
                    callback: function () {
                        redirectToIndex();
                    }
                });
            });
    };

    // Hide Show Question on parent answer question
    function toggleQuestion(ev) {
        var value = ev.currentTarget.value;
        var child = $(ev.currentTarget).closest('.parentQuestion').attr('data-question-childid');
        var childVal = $(ev.currentTarget).closest('.parentQuestion').attr('data-question-child-value');
        var question = jQuery('div[data-questionid = "' + child + '"]');

        if (value == childVal)
        {
            question.addClass('answer-required'); 
            question.attr('answer-required', '1'); // we sure that textarea is required
            question.slideDown();
        }
        else
        {
            question.slideUp();
            question.attr('answer-required', '0'); // remove that textarea required
            question.removeClass('answer-required empty-answer') // remove class for empy answer
            question.find('textarea').val('');
           
        }
    };

    function redirectToIndex() {
        window.location.href = MANAGERS_CHECKLIST_URL;
    }
};
