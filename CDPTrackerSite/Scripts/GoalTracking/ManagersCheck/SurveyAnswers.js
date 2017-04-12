/**
* Manages the Survey Answers
*/
var SurveyAnswers = function () {
    // Private variables
    var self = this;
    var QUESTIONS_CONTAINER = '#questionsContainer';
    var QUESTION = '.question';
    var MULTIPLE_CHOICE_QUESTION = 1;
    var OPEN_ANSWER_QUESTION = 2;

    // Validates and Gets Survey Data
    self.getSurveyData = function () {
        var questionsContainer = jQuery(QUESTIONS_CONTAINER);
        var unansweredQuestions = [];
        var survey = {
            surveyId: questionsContainer.attr('data-surveyid'),
            resourceEvaluatedId: questionsContainer.attr('data-evaluatedid'),
        };
        var answers = [];

        questionsContainer.find(QUESTION).each(function (index) {
            var question = jQuery(this);
            var questionType = question.data('question-type');
            var questionId = question.attr('data-questionid');
            //var sequence = question.attr('data-question-sequence');
            var sequence = question.find('.panel-title').text();
            sequence = sequence.slice(0, sequence.indexOf('.'));

            var answer = {
                questionId: questionId
            };
            var result = null;

            question.removeClass('empty-answer');

            // Multiple Choice Question
            if (questionType == MULTIPLE_CHOICE_QUESTION) {
                result = question.find('input[type=radio]:checked');

                if (result.length == 0) {
                    unansweredQuestions.push(sequence);
                    question.addClass('empty-answer');
                } else {
                    answer.responseId = result.val();
                    answers.push(answer);
                }
            }

            // Open Answer Question
            if (questionType == OPEN_ANSWER_QUESTION) {
                var required = question.attr('answer-required');
                result = question.find('textarea').val().trim();

                if (result == '') {
                    if (required == '1') {
                        question.addClass('empty-answer');
                        unansweredQuestions.push(sequence);
                         
                    }
                }
                else {
                    answer.responseText = result;
                    answers.push(answer);
                }
            }
        });

        // Display error message if necessary
        if (unansweredQuestions.length > 0) {
            myApp.Dialogs.alertDanger('Error','The following questions must be answered: ' + unansweredQuestions.join(', '));
            answers = null;
        }

        survey.answers = answers;
        return survey;
    };
};
