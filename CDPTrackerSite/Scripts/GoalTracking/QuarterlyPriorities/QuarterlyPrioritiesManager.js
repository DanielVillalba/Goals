/**
* Manages the Quarterly Priorites
*/
var QuarterlyPrioritiesManager = function (
        config,
        quarterlyPrioritiesRequestsHandler,
        kpiList,
        quarterlyActionsList,
        valuesInfusionList,
        personalDevelopmentList
    ) {

    // Private variables
    var self = this;
    var SAVE_QUARTERLY_PRIORITIES_BUTTON = '#saveQuarterlyPriorities';
    var CYCLE_PRIORITIES_URL = '/GoalTracking/QuarterlyPrioritiesEndOfQuarter';
    var IS_NEW_REGISTRY = true;
    var IS_PROCESS_RUNNING = false;


    // Initialize functionality
    self.init = function () {        
        // Check if we are adding or updating a quarterly priority
        if (config.quarterlyPriorityId > 0) {
            IS_NEW_REGISTRY = false;
        }

        // Attach Save Priority Event
        jQuery(SAVE_QUARTERLY_PRIORITIES_BUTTON).click(saveAction);

        // Init Actions
        kpiList.init();
        quarterlyActionsList.init();
        valuesInfusionList.init();
        personalDevelopmentList.init();
    };


    // Save Action
    function saveAction() {
        if (IS_PROCESS_RUNNING == false) {
            IS_PROCESS_RUNNING = true;
            var data = getQuarterlyPrioritiesData();
       
            if (data) {
                if (IS_NEW_REGISTRY) {
                    addQuarterlyPriorities(data);
                } else {
                    updateQuarterlyPriorities(data);
                }
            }

            // Lets give 2 second delay until next time user can click save button again
            setTimeout(function () {
                IS_PROCESS_RUNNING = false;
            }, 2000);
        }
    }

    // Add Quarterly Priorities
    function addQuarterlyPriorities(data) {
        // @TODO
       // myApp.Dialogs.alertDanger('Error', 'Work in Progress');
       // return;

        quarterlyPrioritiesRequestsHandler
            .addQuarterlyPriorities(data)
            .success(function () {
                myApp.Dialogs.alertSuccess('Created', 'Quarterly Priorities Created!', {
                    callback: function () {
                        redirectToCyclePriorities();
                    }
                });
            });
    };

    // Update Quarterly Priorities
    function updateQuarterlyPriorities(data) {
        // @TODO
        //myApp.Dialogs.alertDanger('Error', 'Work in Progress');
       // return;

        quarterlyPrioritiesRequestsHandler
            .updateQuarterlyPriorities(data)
            .success(function () {
                myApp.Dialogs.alertSuccess('Updated', 'Quarterly Priorities Updated!', {
                    callback: function () {
                        redirectToCyclePriorities();
                    }
                });
            });
    };

    // Get Data
    function getQuarterlyPrioritiesData() {
        var data = {};

        try {
            data.kpi = kpiList.getListData();
            data.quarterly_actions = quarterlyActionsList.getListData();
            data.values_infusion = valuesInfusionList.getListData();
            data.personal_development = personalDevelopmentList.getListData();
            data.quarterlyPriorityId = config.quarterlyPriorityId;
    
            data.onePagePlanId = config.onePagePlanId;
            validateData(data);

        } catch (err) {
            data = undefined;
            myApp.Dialogs.alertDanger('Error', err);
        }

        return data;
    }

    // Validate Data
    function validateData(data) {
        var messages = [];
        
        // Remove red borders
        jQuery('.border-danger').removeClass('border-danger');

        // Validate each module
        messages = moduleValidateData(messages, kpiList, data.kpi);
        messages = moduleValidateData(messages, quarterlyActionsList, data.quarterly_actions);
        messages = moduleValidateData(messages, valuesInfusionList, data.values_infusion);
        messages = moduleValidateData(messages, personalDevelopmentList, data.personal_development);

        if (messages.length > 0) {
            throw messages.join('<br>');
        }
    }

    function moduleValidateData(messages, module, data) {
        var tmp = module.validateData(data);

        if (tmp.length > 0) {
            if (messages.length > 0) {
                messages = messages.concat(['']);
            }
            messages = messages.concat(tmp);
        }

        return messages;
    }

    function redirectToCyclePriorities() {
        window.location.href = CYCLE_PRIORITIES_URL;
    }
};
