/**
* Manages the Generation of the Survey
*/
var SurveyGenerator = function () {
    // Private variables
    var self = this;
    var QUESTIONS_CONTAINER = '#questionsContainer';
    var QUESTION = '.question';
    var INPUT_CHECKBOX = "parentQuestion";
    var MULTIPLE_CHOICE_QUESTION = 1;
    var OPEN_ANSWER_QUESTION = 2;

    // Generates Survey given survey data with questions
    self.generateSurvey = function (survey) {
        var questionsContainer = jQuery(QUESTIONS_CONTAINER);
        questionsContainer.attr('data-surveyid', survey.surveyId)

        console.log(survey)

        for (var i = 0; i < survey.questions.length; i++) {
            var template = null;
            var question = survey.questions[i];
            var questionNumber = i + 1;

            // Multiple Choice Question
            if (question.questionType == MULTIPLE_CHOICE_QUESTION) {
                template = jQuery('#questionType1').clone();
                template.find('.panel-title').html("* "+ question.text);
                template.find('.question').attr('data-questionid', question.questionId);
                template.find('.question').attr('data-question-sequence', question.sequence);

                $.each(question.responses, function (index, val) {
                    if (val.answer.length > 62) {
                        template.find('.question').addClass("longAns");
                    }
                });
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

                // question number 7 it'snt required

                console.log(question.titule + " " + question.required)

                if (question.required) {
                    template.find('.question').attr('answer-required', question.required);
                    template.find('.panel-title').html("* " + question.text);
                } else {
                    template.find('.panel-title').html(question.text);
                }
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

    self.showSurveyResponses = function (answers) {

        var questionsContainer = jQuery(QUESTIONS_CONTAINER);
        questionsContainer.addClass("disableFunctionality");

        questionsContainer.find(QUESTION).each(function (index) {
            var question = jQuery(this);
            var questionType = question.data('question-type');
            var questionId = question.attr('data-questionid');
            var answerId = -1;
            var answerText;

            $.each(answers.surveyResponses,function (pIndex, pEl) {
                if (pEl.questionId == questionId) {
                    if (pEl.responseId != pEl && pEl.responseId != 'undefined') {
                        answerId = pEl.responseId;
                        answerText = pEl.responseText;
                    }
                    return false;
                }
            });

            // Multiple Choice Question
            if (questionType == MULTIPLE_CHOICE_QUESTION) {
                var inputSelected = question.find("input[value='" + answerId + "']")
                inputSelected.attr('checked', true);
                    var child = $(inputSelected).closest('.parentQuestion').attr('data-question-childid');
                    var childVal = $(inputSelected).closest('.parentQuestion').attr('data-question-child-value');
                    var question = jQuery('div[data-questionid = "' + child + '"]');
                    if (answerId == childVal) { question.slideDown(); }
                    else { question.slideUp(); }
            }

            // Open Answer Question
            if (questionType == OPEN_ANSWER_QUESTION) {
                var textArea = question.find('textarea');
                textArea.val(answerText);
            }


        });

    };

};
