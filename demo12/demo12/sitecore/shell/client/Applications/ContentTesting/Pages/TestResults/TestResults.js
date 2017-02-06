define(["sitecore"], function (_sc) {
  var initLoad = true;

  var TestResults = _sc.Definitions.App.extend({
    initialized: function () {
      this.ConversionRateDataSource.on("change:items", this.populateDetails, this);
      this.ConversionRateDataSource.on("change:conversions", this.populateConversions, this);
      this.ConversionRateDataSource.on("change:conversionRate", this.populateConversionRate, this);

      this.GoalsListFiltered.on("change:selectedItem", this.populateConversionRateList, this);

      this.on("change:selectedTestValue", this.notifyTestValueChange, { app: this });
      this.on("change:selectedExperience", this.notifyExperienceChange, { app: this });

      this.ConversionRateIndicator.on("change:selectedItem", this.setSelectedExperience, { control: this.ConversionRateIndicator, app: this });
      this.EngagementValueIndicator.on("change:selectedItem", this.setSelectedExperience, { control: this.EngagementValueIndicator, app: this });
      this.VariablesValueIndicator.on("change:selectedItem", this.setSelectedTestValue, { control: this.VariablesValueIndicator, app: this });
      
      this.ConversionRateIndicator.on("change:items", this.resetListSelection, this);

      this.TabControl.on("change:selectedTab", this.setDetailComponentVisibility, this);
      this.EngagementValueIndicator.on("change:items", this.setDetailComponentVisibility, this);

      this.setDetailComponentVisibility();

      this.ExperienceEngagementDataSource.on("change:isBusy", this.selectBestExperience, { ds: this.ExperienceEngagementDataSource, app: this, list: this.EngagementValueIndicator });
      
      this.TestDefinitionDataSource.on("change:conversions", this.handleTestObjectives, this);
      
      this.TestDefinitionDataSource.on("change:testType", this.handleTestType, this);
      
      this.set("detailsTitleText", this.DetailPanelTitle.get("text"));

    },

    close: function () {
      var frame = window.frameElement;
      $(frame).hide();
    },
    
    resetListSelection: function()
    {
      this.resetIndicatorSelection(this.ConversionRateIndicator, this.setSelectedExperience, { control: this.ConversionRateIndicator, app: this });
    },

    setSelectedTestValue: function () {
      var selectedItem = this.control.get("selectedItem");
      if (!selectedItem) {
        return;
      }
      this.app.set("selectedTestValue", {}, {silent: true});
      this.app.set("selectedTestValue", {
        TestValueId: selectedItem.ValueId,
        ExperienceName: selectedItem.title
      });
    },
    
    setSelectedExperience: function () {
      var selected = this.control.get("selectedItem");
      if (!selected) {
        return;
      }
      
      this.app.set("selectedExperience", {}, {silent: true});

      this.app.set("selectedExperience", {
        Combination: selected.Combination,
        GoalId: selected.GoalId,
        Description: selected.Description,
        ExperienceName: selected.title
      });
    },
    
    handleTestObjectives: function()
    {
      var conversions = this.TestDefinitionDataSource.get("conversions");
      if (conversions !== undefined && conversions.length > 0)
      {
        this.TabControl.viewModel.$el.find("ul li")[1].click();
        var conversion = conversions[0];
        this.GoalsListFiltered.set("selectedItem", conversion);
      }
    },
    
    handleTestType: function()
    {
      var testType = this.TestDefinitionDataSource.get("testType");
      if (testType !== undefined && testType === 'Page')
      {
        var variablesTab = "{637B17F4-D56F-443A-BA2F-16BC56D0B88F}";
        this.TabControl.viewModel.$el.find("li[data-tab-id='"+variablesTab+"']").hide();
        var thumbnailPanel = this.PreviewContent.viewModel.$el.find("[data-sc-id='ThumbnailPanel']");
        var detailPanel = this.PreviewContent.viewModel.$el.find("[data-sc-id='DetailPanel']");
        thumbnailPanel.removeClass("col-md-6");
        thumbnailPanel.addClass("col-md-12");
        detailPanel.hide();
      }
    },

    notifyTestValueChange: function() {
      var testValue = this.app.get("selectedTestValue");
      if (!testValue) {
        return;
      }
      
      var detailText = this.app.get("detailsTitleText")+" - "+testValue.ExperienceName;
      this.app.DetailPanelTitle.set("text", detailText);

      this.app.TopClicksToPagesDataSource.set("valueId", testValue.TestValueId);
      this.app.TopClicksToPagesDataSource1.set("valueId", testValue.TestValueId);

      this.app.TopGoalsDataSource.set("valueId", testValue.TestValueId);
      this.app.TopGoalsDataSource1.set("valueId", testValue.TestValueId);
      
      this.app.SiteUsageDataSource.set("valueId", testValue.TestValueId);
      
      this.app.ExperienceKPIDataSource.set("variationId", testValue.TestValueId);
      
      this.app.ReachDataSource.set("valueId", testValue.TestValueId);
    },

    notifyExperienceChange: function () {
      var experience = this.app.get("selectedExperience");
      if (!experience) {
        return;
      }

      var combinationstring = "";

      for (var i = 0; i < experience.Combination.length; i++) {
        combinationstring += experience.Combination[i].toString();
      }
      
      var detailText = this.app.get("detailsTitleText")+" - "+experience.ExperienceName;
      this.app.DetailPanelTitle.set("text", detailText);

      this.app.TopGoalsDataSource.set("combination", combinationstring);
      this.app.TopGoalsDataSource1.set("combination", combinationstring);

      this.app.TopClicksToPagesDataSource.set("combination", combinationstring);
      this.app.TopClicksToPagesDataSource1.set("combination", combinationstring);
      
      this.app.SiteUsageDataSource.set("combination", combinationstring);

      this.app.ExperienceKPIDataSource.set("goalId", experience.GoalId);
      this.app.ExperienceKPIDataSource.set("combination", combinationstring);

      this.app.ThumbnailImage.set("combination", combinationstring);
      if (initLoad) {
        this.app.ThumbnailImage.set("isBusy", false);
        this.app.ThumbnailImage.refresh();
      }
      initLoad = false;

      this.app.DescriptionDetailList.set("items", experience.Description);
      
      this.app.ReachDataSource.set("combination", combinationstring);
      
      this.app.TestActions.set("combination", combinationstring);
    },
    
    populateDetails:function() {
      this.DetailPanelBorder.set("isVisible", true);
      //this.BorderButtons.set("isVisible", true);
    },

    populateConversionRateList: function () {
      var currentGoal = this.GoalsListFiltered.get("selectedItem");
      var goalId = currentGoal.guid;
      this.ConversionRateDataSource.set("goalId", goalId);
      
      //this.ChooseGoalText.set("isVisible", false);
    },

    showMoreConversions: function () {
      this.ConversionRateDataSource.fetch();
    },

    populateConversions: function() {
      this.ConversionsLabel.set("value", this.ConversionRateDataSource.get("conversions"));
    },
    
    populateConversionRate: function () {
      this.ConversionRate.set("value", this.ConversionRateDataSource.get("conversionRate"));
    },

    setDetailComponentVisibility: function () {
      var selectedTab = this.TabControl.get("selectedTab");

      var engagementValueTab = selectedTab === "{898887C4-E6EE-4563-9C44-BB5F6BD1303D}";
      var conversionRateTab = selectedTab === "{346F89EE-DD0D-4397-8765-ABD65AA923C3}";
      var variablesTab = selectedTab === "{637B17F4-D56F-443A-BA2F-16BC56D0B88F}";

      this.PreviewContent.viewModel.$el.toggle(engagementValueTab || conversionRateTab);
      this.ConversionKPIBorder.viewModel.$el.toggle(conversionRateTab);
      
      this.DetailPanelBorder.set("isVisible", true);
      this.DetailPanelTitle.set("isVisible", true);
      this.WinnerButton.set("isVisible", true);
      //this.BorderButtons.set("isVisible", true);
      
      if (engagementValueTab)
      {
        this.resetIndicatorSelection(this.EngagementValueIndicator, this.setSelectedExperience, { control: this.EngagementValueIndicator, app: this });
      }
      
      if (variablesTab)
      {
        this.resetIndicatorSelection(this.VariablesValueIndicator, this.notifyTestValueChange, { app: this });
        this.WinnerButton.set("isVisible", false);
        //this.BorderButtons.set("isVisible", false);
      }
      
      if (conversionRateTab)
      {
        var selectedGoal = this.GoalsListFiltered.get("selectedItem");
        if (selectedGoal === undefined || selectedGoal === "")
        {
          this.DetailPanelBorder.set("isVisible", false);
          this.DetailPanelTitle.set("isVisible", false);
          this.WinnerButton.set("isVisible", false);
          this.ConversionRate.set("isVisible", false);
          // this.BorderButtons.set("isVisible", false);
          //this.GoalDetailsBorder.set("isVisible", false);
          //this.ChooseGoalText.set("isVisible", true);
        }
        else
        {
          //this.ChooseGoalText.set("isVisible", false);
          this.WinnerButton.set("isVisible", true);
          this.ConversionRate.set("isVisible", true);
          this.resetIndicatorSelection(this.ConversionRateIndicator, this.setSelectedExperience, { control: this.ConversionRateIndicator, app: this });
        }
      }
    },
    
    resetIndicatorSelection: function(component, refreshcallback, callbackparametrs)
    {
      var items = component.get("items");
        if (items !== undefined && items.length > 0)
        {
            component.set("selectedItem", items[0].items[0]);
            refreshcallback.call(callbackparametrs);
        }
    },
    
    getMoreTopGoalsData: function() {
      this.TopGoalsDataSource1.fetch();
    },
    
    getMoreTopClicksToPagesData: function () {
      this.TopClicksToPagesDataSource1.fetch();
    },
    
    isExperienceTabActive: function() {
      var selectedTab = this.TabControl.get("selectedTab");

      var engagementValueTab = selectedTab === "{F1E087C9-7B6F-4253-A66C-CC871AAD7D85}";
      var conversionRateTab = selectedTab === "{7BD8A1B1-BECB-4FC5-A3CB-55FAC26641D6}";

      return engagementValueTab || conversionRateTab;
    },

    selectBestExperience: function() {
      if (!this.ds.get("isBusy")) {
        var max = null;
        var maxItem = null;
          
        _.each(this.list.get("items"), function (item) {
          if (item.Value > max) {
            max = item.Value;
            maxItem = item;
          }
        });

        this.app.set("selectedExperience", maxItem);
      }
    },

    pickWinner: function () {
      // Resize dialog if there are rules to keep
      var height = 150;
      if (this.RulesToKeepBorder.get("isVisible")) {
        height = 400;
      }

      this.ConfirmDialogWindow.viewModel.$el.find('.modal-body').parent().data("height", height);
      this.ConfirmDialogWindow.show();
    },

    confirmWinner: function () {
      this.WinnerButton.set("isEnabled", false);
      this.ServerProgressIndicator.set("isBusy", true);

      var ids = [];
      var cbs = this.RulesToKeepList.viewModel.$el.find("input:checked");
      _.each(cbs, function (cb) {
        if (cb.id.indexOf("rulekeep_" == 0)) {
          ids.push(cb.id.substr(9));
        }
      });

      this.ConfirmDialogWindow.hide();

      this.TestActions.stopTest(this.stopTestSucceeded, this.stopTestError, this, ids);
    },
    
    stopTestSucceeded: function(data) {
      this.ServerProgressIndicator.set("isBusy", false);

      if (data.succeed === true ) {
        var topwindow = window.parent.parent;
        topwindow.location = topwindow.location.href;
      }
      else {
        this.stopTestError(data.errorMessage);
      }
    },

    stopTestError: function(message) {
      this.ServerProgressIndicator.set("isBusy", false);
      var text = message;
      if (text ==  undefined || text == ''){
        text = this.Texts.get("An error occurred");
        if (!text) {
          text = "An error occurred";
        }
      }
      
      alert(text);
      this.WinnerButton.set("isEnabled", true);
    },
    
    excludeVariant: function() {
      alert("coming soon...");
    }

  });

  return TestResults;
});