/**
* Manages the Generation of the Survey
*/
var SurveyGenerator = function () {
    // Private variables
    var self = this;
    var QUESTIONS_CONTAINER = '#questionsContainer';
    var MULTIPLE_CHOICE_QUESTION = 1;
    var OPEN_ANSWER_QUESTION = 2;

    // Generates Survey given survey data with questions
    self.generateSurvey = function (survey) {
        var questionsContainer = jQuery(QUESTIONS_CONTAINER);
        questionsContainer.attr('data-surveyid', survey.surveyId)

        for (var i = 0; i < survey.questions.length; i++) {
            var template = null;
            var question = survey.questions[i];
            var questionNumber = i + 1;

            // Multiple Choice Question
            if (question.questionType == MULTIPLE_CHOICE_QUESTION) {
                template = jQuery('#questionType1').clone();                
                template.find('.panel-title').html(question.sequence + '. * ' + question.text);
                template.find('.question').attr('data-questionid', question.questionId);
                template.find('.question').attr('data-question-sequence', question.sequence);

                var answersContainer = template.find('.answers-container');
                $.each(question.responses, function (index, val) {
                    templateAnswer = jQuery('#answerType1').children().clone();
                    var input = templateAnswer.find('input');
                    var label = templateAnswer.find('label');
                    var rdCounter = 'rd' + i + '' + (index);

                    input.attr('id', rdCounter);
                    input.attr('value', val.responseId);
                    input.attr('name', 'question-radio-' + question.sequence);
                    label.attr('for', rdCounter);
                    label.text(val.answer);

                    answersContainer.append(templateAnswer);
                });
            }

            // Open Answer Question
            if (question.questionType == OPEN_ANSWER_QUESTION) {
                template = jQuery('#questionType2').clone();
                template.find('.question').attr('answer-required', question.required);
                template.find('.panel-title').html(question.sequence + '. * ' + question.text);
                template.find('.question').attr('data-questionid', question.questionId);
            }

            if (question.questionChild !== null) {
                template.find('.question').addClass('parentQuestion');
                template.find('.question').attr('data-question-childId', question.questionChild);
                template.find('.question').attr('data-question-child-value', question.displayWhenValue);
            }

            if (typeof question.questionParent !== "undefined") {
                template.find('.question').attr('data-question-parentId', question.questionParent.questionId);
                template.find('.question').hide();
            }

            questionsContainer.append(template.html());
        }
    };
};
