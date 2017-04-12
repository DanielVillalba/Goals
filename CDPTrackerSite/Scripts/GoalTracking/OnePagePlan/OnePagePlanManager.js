/**
* Manages One Page Plan
*/
var OnePagePlanManager = function (
        config,
        onePagePlanRequestsHandler,
        coreValuesList,
        keyThrustsList,
        annualPrioritiesList,
        criticalNumbersList
    ) {

    // Private variables
    var self = this;
    var SAVE_ONE_PAGE_PLAN_BUTTON = '#saveOnePagePlanBtn';
    var CYCLE_PRIORITIES_URL = '/GoalTracking/index';
    var IS_NEW_REGISTRY = true;
    var IS_PROCESS_RUNNING = false;


    // Initialize functionality
    self.init = function () {
        // Check if we are adding or updating

        //console.debug("OnePageId", config.onePagePlanId);
        if (config.onePagePlanId == 0) {
            IS_NEW_REGISTRY = true;
        }
        else { IS_NEW_REGISTRY = false; }

     

        // Init Actions
        coreValuesList.init();
        keyThrustsList.init();
        annualPrioritiesList.init();
        criticalNumbersList.init();

        // Attach Save Event
        jQuery(SAVE_ONE_PAGE_PLAN_BUTTON).click(saveAction);

    };


    // Save Action
    function saveAction() {
        if (IS_PROCESS_RUNNING == false)
        {
            IS_PROCESS_RUNNING = true;
          
          
           // console.debug("editar registro", data);

            var data = getModuleData();
            if (data)
            {
                if (IS_NEW_REGISTRY) {
                    AddOnePagePlan(data);
                }
                else
                {
                   // console.debug("editar registro", data);
                    UpdateOnePagePlan(data);
                }

            }

                      

            // Lets give 2 second delay until next time user can click save button again
            setTimeout(function () {
                IS_PROCESS_RUNNING = false;
            }, 2000);
        }
    }

    // Add One Page Plan
    function AddOnePagePlan(data) {
        // @TODO
       
      
        $.ajax({
            type: "post",
            url: "AddOnePagePlan",
            data: { data: JSON.stringify(data) },
            success: function (response) {
                myApp.Dialogs.alertSuccess('Inserted', 'One Page Plan, Created!', {
                    callback: function (response) {
                        redirectToCyclePriorities();
                    }
                });
            },
            error: function (response) { },
        })

     
    };

    // Update OnePage Plan
    function UpdateOnePagePlan(data) {
        

        $.ajax({
            type: "post",
            url: "UpdateOnePagePlan",
            data: { data: JSON.stringify(data) },
            success: function (response) {
                myApp.Dialogs.alertSuccess('Updated', 'One Page Plan, Updated!', {
                    callback: function (response) {
                        redirectToCyclePriorities();
                    }
                });
            },
            error: function (response) { },
        })
    };

    // Get Data
    function getModuleData() {
        var data = {};

        try {
            data.core_values = coreValuesList.getListData();
            data.key_thrusts = keyThrustsList.getListData();
            data.annual_priorities = annualPrioritiesList.getListData();
            data.critical_numbers = criticalNumbersList.getListData();
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
        messages = moduleValidateData(messages, coreValuesList, data.core_values);
        messages = moduleValidateData(messages, keyThrustsList, data.key_thrusts);
        messages = moduleValidateData(messages, annualPrioritiesList, data.annual_priorities);
        messages = moduleValidateData(messages, criticalNumbersList, data.critical_numbers);

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
