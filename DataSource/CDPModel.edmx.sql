
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/24/2017 12:21:21
-- Generated from EDMX file: C:\CDPTracker\Development\DataSource\CDPModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [CDPTrack];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK__GoalTrack__Resou__1B0907CE]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GoalTracking] DROP CONSTRAINT [FK__GoalTrack__Resou__1B0907CE];
GO
IF OBJECT_ID(N'[dbo].[FK__Resource__Locati__1CF15040]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Resource] DROP CONSTRAINT [FK__Resource__Locati__1CF15040];
GO
IF OBJECT_ID(N'[dbo].[FK__Resource__ResourceId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OnePagePlan] DROP CONSTRAINT [FK__Resource__ResourceId];
GO
IF OBJECT_ID(N'[dbo].[FK_AnnualPriorities_OnePagePlanId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AnnualPriorities] DROP CONSTRAINT [FK_AnnualPriorities_OnePagePlanId];
GO
IF OBJECT_ID(N'[dbo].[FK_Employee_Area]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Employee] DROP CONSTRAINT [FK_Employee_Area];
GO
IF OBJECT_ID(N'[dbo].[FK_Employee_Position]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Employee] DROP CONSTRAINT [FK_Employee_Position];
GO
IF OBJECT_ID(N'[dbo].[FK_Employee_Position1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Employee] DROP CONSTRAINT [FK_Employee_Position1];
GO
IF OBJECT_ID(N'[dbo].[FK_Employee_Project]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Employee] DROP CONSTRAINT [FK_Employee_Project];
GO
IF OBJECT_ID(N'[dbo].[FK_Employee_Resource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Employee] DROP CONSTRAINT [FK_Employee_Resource];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneralTrainingProgram_TrainingProgramCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GeneralTrainingProgram] DROP CONSTRAINT [FK_GeneralTrainingProgram_TrainingProgramCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneralTrainingProgramDetails_GeneralTrainingProgram]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GeneralTrainingProgramDetails] DROP CONSTRAINT [FK_GeneralTrainingProgramDetails_GeneralTrainingProgram];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneralTrainingProgramDetails_ProgressEnum]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GeneralTrainingProgramDetails] DROP CONSTRAINT [FK_GeneralTrainingProgramDetails_ProgressEnum];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneralTrainingProgramDetails_Resource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GeneralTrainingProgramDetails] DROP CONSTRAINT [FK_GeneralTrainingProgramDetails_Resource];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneralTrainingProgramVideo_GeneralTrainingProgram]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GeneralTrainingProgramVideo] DROP CONSTRAINT [FK_GeneralTrainingProgramVideo_GeneralTrainingProgram];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneralTrainingProgramVideo_Location]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GeneralTrainingProgramVideo] DROP CONSTRAINT [FK_GeneralTrainingProgramVideo_Location];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneralTrainingProgramVisits_GeneralTrainingProgram]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GeneralTrainingProgramVisits] DROP CONSTRAINT [FK_GeneralTrainingProgramVisits_GeneralTrainingProgram];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneralTrainingProgramVisits_Resource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GeneralTrainingProgramVisits] DROP CONSTRAINT [FK_GeneralTrainingProgramVisits_Resource];
GO
IF OBJECT_ID(N'[dbo].[FK_GoalTracking_ProgressEnum]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GoalTracking] DROP CONSTRAINT [FK_GoalTracking_ProgressEnum];
GO
IF OBJECT_ID(N'[dbo].[FK_GoalTracking_SourceId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GoalTracking] DROP CONSTRAINT [FK_GoalTracking_SourceId];
GO
IF OBJECT_ID(N'[dbo].[FK_GoalTracking_TrainingCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GoalTracking] DROP CONSTRAINT [FK_GoalTracking_TrainingCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_Group_SectionAccess_Groups]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Group_SectionAccess] DROP CONSTRAINT [FK_Group_SectionAccess_Groups];
GO
IF OBJECT_ID(N'[dbo].[FK_KPI_QuarterlyPriorities]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[KPI] DROP CONSTRAINT [FK_KPI_QuarterlyPriorities];
GO
IF OBJECT_ID(N'[dbo].[FK_Objective_Category]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Objective] DROP CONSTRAINT [FK_Objective_Category];
GO
IF OBJECT_ID(N'[dbo].[FK_Objective_Resource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Objective] DROP CONSTRAINT [FK_Objective_Resource];
GO
IF OBJECT_ID(N'[dbo].[FK_OnePagePlanId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[KeyThrusts] DROP CONSTRAINT [FK_OnePagePlanId];
GO
IF OBJECT_ID(N'[dbo].[FK_PersonalDevelopment_QuarterlyPriorities]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PersonalDevelopment] DROP CONSTRAINT [FK_PersonalDevelopment_QuarterlyPriorities];
GO
IF OBJECT_ID(N'[dbo].[FK_Position_Area]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Position] DROP CONSTRAINT [FK_Position_Area];
GO
IF OBJECT_ID(N'[dbo].[FK_PositionsHierarchy_Position]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PositionsHierarchy] DROP CONSTRAINT [FK_PositionsHierarchy_Position];
GO
IF OBJECT_ID(N'[dbo].[FK_PositionsHierarchy_Position1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PositionsHierarchy] DROP CONSTRAINT [FK_PositionsHierarchy_Position1];
GO
IF OBJECT_ID(N'[dbo].[fk_Progress]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Objective] DROP CONSTRAINT [fk_Progress];
GO
IF OBJECT_ID(N'[dbo].[FK_QuarterlyActions_QuarterlyPriorities]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[QuarterlyActions] DROP CONSTRAINT [FK_QuarterlyActions_QuarterlyPriorities];
GO
IF OBJECT_ID(N'[dbo].[FK_QuarterlyPriorities_OnePagePlan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[QuarterlyPriorities] DROP CONSTRAINT [FK_QuarterlyPriorities_OnePagePlan];
GO
IF OBJECT_ID(N'[dbo].[FK_Question_Survey]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Question] DROP CONSTRAINT [FK_Question_Survey];
GO
IF OBJECT_ID(N'[dbo].[FK_QuestionResponse_Question]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[QuestionResponse] DROP CONSTRAINT [FK_QuestionResponse_Question];
GO
IF OBJECT_ID(N'[dbo].[FK_QuestionResponse_Response]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[QuestionResponse] DROP CONSTRAINT [FK_QuestionResponse_Response];
GO
IF OBJECT_ID(N'[dbo].[FK_SkillCompassGlossary_Area]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SkillCompassGlossary] DROP CONSTRAINT [FK_SkillCompassGlossary_Area];
GO
IF OBJECT_ID(N'[dbo].[FK_Suggestions_Area]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Suggestions] DROP CONSTRAINT [FK_Suggestions_Area];
GO
IF OBJECT_ID(N'[dbo].[FK_Suggestions_Level]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Suggestions] DROP CONSTRAINT [FK_Suggestions_Level];
GO
IF OBJECT_ID(N'[dbo].[FK_Suggestions_Position]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Suggestions] DROP CONSTRAINT [FK_Suggestions_Position];
GO
IF OBJECT_ID(N'[dbo].[FK_Suggestions_Technologies]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Suggestions] DROP CONSTRAINT [FK_Suggestions_Technologies];
GO
IF OBJECT_ID(N'[dbo].[FK_SurveyResource_Resource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SurveyResource] DROP CONSTRAINT [FK_SurveyResource_Resource];
GO
IF OBJECT_ID(N'[dbo].[FK_SurveyResource_Survey]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SurveyResource] DROP CONSTRAINT [FK_SurveyResource_Survey];
GO
IF OBJECT_ID(N'[dbo].[FK_SurveyResponse_Question]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SurveyResponse] DROP CONSTRAINT [FK_SurveyResponse_Question];
GO
IF OBJECT_ID(N'[dbo].[FK_SurveyResponse_Resource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SurveyResponse] DROP CONSTRAINT [FK_SurveyResponse_Resource];
GO
IF OBJECT_ID(N'[dbo].[FK_SurveyResponse_SurveyResource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SurveyResponse] DROP CONSTRAINT [FK_SurveyResponse_SurveyResource];
GO
IF OBJECT_ID(N'[dbo].[FK_TDURedeem_ResourceId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TDURedeem] DROP CONSTRAINT [FK_TDURedeem_ResourceId];
GO
IF OBJECT_ID(N'[dbo].[FK_TDURedeem_TDURewardId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TDURedeem] DROP CONSTRAINT [FK_TDURedeem_TDURewardId];
GO
IF OBJECT_ID(N'[dbo].[FK_TDUReward_ResourceId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TDUReward] DROP CONSTRAINT [FK_TDUReward_ResourceId];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingCategory_TrainingCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingCategory] DROP CONSTRAINT [FK_TrainingCategory_TrainingCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgram_Position]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgram] DROP CONSTRAINT [FK_TrainingProgram_Position];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgram_TrainingProgramCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgram] DROP CONSTRAINT [FK_TrainingProgram_TrainingProgramCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgramDetails_Resource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgramDetails] DROP CONSTRAINT [FK_TrainingProgramDetails_Resource];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgramDetails_TrainingProgram]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgramDetails] DROP CONSTRAINT [FK_TrainingProgramDetails_TrainingProgram];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgramOnDemandDetails_ProgressEnum]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgramOnDemandDetails] DROP CONSTRAINT [FK_TrainingProgramOnDemandDetails_ProgressEnum];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgramOnDemandDetails_Resource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgramOnDemandDetails] DROP CONSTRAINT [FK_TrainingProgramOnDemandDetails_Resource];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgramOnDemandDetails_TrainingProgramOnDemand]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgramOnDemandDetails] DROP CONSTRAINT [FK_TrainingProgramOnDemandDetails_TrainingProgramOnDemand];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgramOnDemandVisits_Resource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgramOnDemandVisits] DROP CONSTRAINT [FK_TrainingProgramOnDemandVisits_Resource];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgramOnDemandVisits_TrainingProgramOnDemand]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgramOnDemandVisits] DROP CONSTRAINT [FK_TrainingProgramOnDemandVisits_TrainingProgramOnDemand];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgramVideo_Location]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgramVideo] DROP CONSTRAINT [FK_TrainingProgramVideo_Location];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgramVideo_TrainingProgram]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgramVideo] DROP CONSTRAINT [FK_TrainingProgramVideo_TrainingProgram];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgramVisits_Resource]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgramVisits] DROP CONSTRAINT [FK_TrainingProgramVisits_Resource];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainingProgramVisits_TrainingProgram]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingProgramVisits] DROP CONSTRAINT [FK_TrainingProgramVisits_TrainingProgram];
GO
IF OBJECT_ID(N'[dbo].[FK_ValuesInfusion_QuarterlyPriorities]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ValuesInfusion] DROP CONSTRAINT [FK_ValuesInfusion_QuarterlyPriorities];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AnnualPriorities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AnnualPriorities];
GO
IF OBJECT_ID(N'[dbo].[AppError]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AppError];
GO
IF OBJECT_ID(N'[dbo].[Area]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Area];
GO
IF OBJECT_ID(N'[dbo].[Category]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Category];
GO
IF OBJECT_ID(N'[dbo].[CoreValues]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CoreValues];
GO
IF OBJECT_ID(N'[dbo].[Employee]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Employee];
GO
IF OBJECT_ID(N'[dbo].[Employee_Groups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Employee_Groups];
GO
IF OBJECT_ID(N'[dbo].[GeneralTrainingProgram]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GeneralTrainingProgram];
GO
IF OBJECT_ID(N'[dbo].[GeneralTrainingProgramDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GeneralTrainingProgramDetails];
GO
IF OBJECT_ID(N'[dbo].[GeneralTrainingProgramVideo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GeneralTrainingProgramVideo];
GO
IF OBJECT_ID(N'[dbo].[GeneralTrainingProgramVisits]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GeneralTrainingProgramVisits];
GO
IF OBJECT_ID(N'[dbo].[GoalDates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GoalDates];
GO
IF OBJECT_ID(N'[dbo].[GoalTracking]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GoalTracking];
GO
IF OBJECT_ID(N'[dbo].[Group_SectionAccess]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Group_SectionAccess];
GO
IF OBJECT_ID(N'[dbo].[Groups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Groups];
GO
IF OBJECT_ID(N'[dbo].[KeyThrusts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[KeyThrusts];
GO
IF OBJECT_ID(N'[dbo].[KPI]', 'U') IS NOT NULL
    DROP TABLE [dbo].[KPI];
GO
IF OBJECT_ID(N'[dbo].[Level]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Level];
GO
IF OBJECT_ID(N'[dbo].[Location]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Location];
GO
IF OBJECT_ID(N'[dbo].[Objective]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Objective];
GO
IF OBJECT_ID(N'[dbo].[OnePagePlan]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OnePagePlan];
GO
IF OBJECT_ID(N'[dbo].[PersonalDevelopment]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PersonalDevelopment];
GO
IF OBJECT_ID(N'[dbo].[Position]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Position];
GO
IF OBJECT_ID(N'[dbo].[PositionsHierarchy]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PositionsHierarchy];
GO
IF OBJECT_ID(N'[dbo].[ProgressEnum]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProgressEnum];
GO
IF OBJECT_ID(N'[dbo].[Project]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Project];
GO
IF OBJECT_ID(N'[dbo].[QuarterlyActions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[QuarterlyActions];
GO
IF OBJECT_ID(N'[dbo].[QuarterlyPriorities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[QuarterlyPriorities];
GO
IF OBJECT_ID(N'[dbo].[Question]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Question];
GO
IF OBJECT_ID(N'[dbo].[QuestionResponse]', 'U') IS NOT NULL
    DROP TABLE [dbo].[QuestionResponse];
GO
IF OBJECT_ID(N'[dbo].[Resource]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Resource];
GO
IF OBJECT_ID(N'[dbo].[Response]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Response];
GO
IF OBJECT_ID(N'[dbo].[SkillCompassGlossary]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SkillCompassGlossary];
GO
IF OBJECT_ID(N'[dbo].[Sources]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sources];
GO
IF OBJECT_ID(N'[dbo].[Suggestions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Suggestions];
GO
IF OBJECT_ID(N'[dbo].[Survey]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Survey];
GO
IF OBJECT_ID(N'[dbo].[SurveyResource]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SurveyResource];
GO
IF OBJECT_ID(N'[dbo].[SurveyResponse]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SurveyResponse];
GO
IF OBJECT_ID(N'[dbo].[TDURedeem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TDURedeem];
GO
IF OBJECT_ID(N'[dbo].[TDUReward]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TDUReward];
GO
IF OBJECT_ID(N'[dbo].[Technologies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Technologies];
GO
IF OBJECT_ID(N'[dbo].[Term]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Term];
GO
IF OBJECT_ID(N'[dbo].[TrainingCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrainingCategory];
GO
IF OBJECT_ID(N'[dbo].[TrainingProgram]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrainingProgram];
GO
IF OBJECT_ID(N'[dbo].[TrainingProgramCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrainingProgramCategory];
GO
IF OBJECT_ID(N'[dbo].[TrainingProgramDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrainingProgramDetails];
GO
IF OBJECT_ID(N'[dbo].[TrainingProgramOnDemand]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrainingProgramOnDemand];
GO
IF OBJECT_ID(N'[dbo].[TrainingProgramOnDemandDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrainingProgramOnDemandDetails];
GO
IF OBJECT_ID(N'[dbo].[TrainingProgramOnDemandVisits]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrainingProgramOnDemandVisits];
GO
IF OBJECT_ID(N'[dbo].[TrainingProgramVideo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrainingProgramVideo];
GO
IF OBJECT_ID(N'[dbo].[TrainingProgramVisits]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrainingProgramVisits];
GO
IF OBJECT_ID(N'[dbo].[ValuesInfusion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ValuesInfusion];
GO
IF OBJECT_ID(N'[CDPTrackModelStoreContainer].[GoalEnum]', 'U') IS NOT NULL
    DROP TABLE [CDPTrackModelStoreContainer].[GoalEnum];
GO
IF OBJECT_ID(N'[CDPTrackModelStoreContainer].[CDPView]', 'U') IS NOT NULL
    DROP TABLE [CDPTrackModelStoreContainer].[CDPView];
GO
IF OBJECT_ID(N'[CDPTrackModelStoreContainer].[GeneralTrainingProgramVisitsView]', 'U') IS NOT NULL
    DROP TABLE [CDPTrackModelStoreContainer].[GeneralTrainingProgramVisitsView];
GO
IF OBJECT_ID(N'[CDPTrackModelStoreContainer].[Goals]', 'U') IS NOT NULL
    DROP TABLE [CDPTrackModelStoreContainer].[Goals];
GO
IF OBJECT_ID(N'[CDPTrackModelStoreContainer].[GoalsCompletedTotal]', 'U') IS NOT NULL
    DROP TABLE [CDPTrackModelStoreContainer].[GoalsCompletedTotal];
GO
IF OBJECT_ID(N'[CDPTrackModelStoreContainer].[GoalsView]', 'U') IS NOT NULL
    DROP TABLE [CDPTrackModelStoreContainer].[GoalsView];
GO
IF OBJECT_ID(N'[CDPTrackModelStoreContainer].[ResourceGoals]', 'U') IS NOT NULL
    DROP TABLE [CDPTrackModelStoreContainer].[ResourceGoals];
GO
IF OBJECT_ID(N'[CDPTrackModelStoreContainer].[TrainingProgramOnDemandVisitsView]', 'U') IS NOT NULL
    DROP TABLE [CDPTrackModelStoreContainer].[TrainingProgramOnDemandVisitsView];
GO
IF OBJECT_ID(N'[CDPTrackModelStoreContainer].[TrainingProgramVisitsView]', 'U') IS NOT NULL
    DROP TABLE [CDPTrackModelStoreContainer].[TrainingProgramVisitsView];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'GoalTrackings'
CREATE TABLE [dbo].[GoalTrackings] (
    [GoalId] int IDENTITY(1,1) NOT NULL,
    [ResourceId] int  NOT NULL,
    [Goal] nvarchar(1000)  NOT NULL,
    [Progress] int  NOT NULL,
    [VerifiedByManager] bit  NOT NULL,
    [LastUpdate] datetime  NULL,
    [FinishDate] datetime  NULL,
    [ObjectiveId] int  NULL,
    [TrainingCategoryId] int  NULL,
    [TDU] int  NULL,
    [SourceId] int  NULL
);
GO

-- Creating table 'ProgressEnums'
CREATE TABLE [dbo].[ProgressEnums] (
    [Id] int  NOT NULL,
    [Label] nvarchar(50)  NULL
);
GO

-- Creating table 'Resources'
CREATE TABLE [dbo].[Resources] (
    [ResourceId] int  NOT NULL,
    [Name] nvarchar(250)  NULL,
    [Email] nvarchar(100)  NULL,
    [RolId] int  NOT NULL,
    [DomainName] nvarchar(50)  NOT NULL,
    [LastLogin] datetime  NOT NULL,
    [LocationId] int  NULL,
    [ActiveDirectoryId] int  NULL,
    [IsEnable] int  NULL
);
GO

-- Creating table 'Locations'
CREATE TABLE [dbo].[Locations] (
    [ID] int  NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [abbreviation] varchar(3)  NULL
);
GO

-- Creating table 'Objectives'
CREATE TABLE [dbo].[Objectives] (
    [ObjectiveId] int IDENTITY(1,1) NOT NULL,
    [Objective1] nvarchar(1000)  NULL,
    [ResourceId] int  NOT NULL,
    [CategoryId] int  NOT NULL,
    [Year] int  NULL,
    [Quarter] int  NULL,
    [Progress] int  NULL,
    [Duplicated] bit  NULL
);
GO

-- Creating table 'Category'
CREATE TABLE [dbo].[Category] (
    [CategoryId] int IDENTITY(1,1) NOT NULL,
    [Category1] nvarchar(50)  NULL,
    [Visibility] bit  NULL
);
GO

-- Creating table 'Area'
CREATE TABLE [dbo].[Area] (
    [AreaId] int  NOT NULL,
    [Name] varchar(2000)  NOT NULL,
    [ImageCareerPath] varchar(1000)  NULL,
    [ImageSkillCompass] varchar(1000)  NULL
);
GO

-- Creating table 'Employee'
CREATE TABLE [dbo].[Employee] (
    [ResourceId] int  NOT NULL,
    [ManagerId] int  NULL,
    [CurrentPosition] nvarchar(1000)  NULL,
    [AspiringPosition] nvarchar(1000)  NULL,
    [CurrentPositionID] int  NULL,
    [AspiringPositionID] int  NULL,
    [ProjectId] int  NULL,
    [Type] varchar(30)  NULL,
    [AreaId] int  NULL
);
GO

-- Creating table 'Position'
CREATE TABLE [dbo].[Position] (
    [PositionId] int IDENTITY(1,1) NOT NULL,
    [PositionName] varchar(200)  NOT NULL,
    [AreaId] int  NOT NULL,
    [Description] varchar(1000)  NULL
);
GO

-- Creating table 'PositionsHierarchy'
CREATE TABLE [dbo].[PositionsHierarchy] (
    [PositionHierarchyId] int IDENTITY(1,1) NOT NULL,
    [PositionId] int  NOT NULL,
    [NextPosition] int  NOT NULL
);
GO

-- Creating table 'Suggestions'
CREATE TABLE [dbo].[Suggestions] (
    [SuggestionId] int IDENTITY(1,1) NOT NULL,
    [PositionId] int  NOT NULL,
    [TechnologyId] int  NOT NULL,
    [Source] varchar(1000)  NOT NULL,
    [LevelId] int  NOT NULL,
    [Topic] varchar(1000)  NOT NULL,
    [AreaId] int  NULL
);
GO

-- Creating table 'Technologies'
CREATE TABLE [dbo].[Technologies] (
    [TechnologyId] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(100)  NOT NULL
);
GO

-- Creating table 'Level'
CREATE TABLE [dbo].[Level] (
    [LevelId] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(100)  NOT NULL
);
GO

-- Creating table 'TrainingProgram'
CREATE TABLE [dbo].[TrainingProgram] (
    [IdTrainingProgram] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(1000)  NOT NULL,
    [Position] int  NOT NULL,
    [Category] int  NOT NULL,
    [Link] varchar(1000)  NULL,
    [Enable] bit  NULL,
    [StartDate] datetime  NULL,
    [FinishDate] datetime  NULL
);
GO

-- Creating table 'TrainingProgramCategory'
CREATE TABLE [dbo].[TrainingProgramCategory] (
    [IdTrainingProgramCategory] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(1000)  NOT NULL
);
GO

-- Creating table 'TrainingProgramDetails'
CREATE TABLE [dbo].[TrainingProgramDetails] (
    [IdTrainingProgramDetails] int IDENTITY(1,1) NOT NULL,
    [IdTrainingProgram] int  NOT NULL,
    [Status] int  NOT NULL,
    [ResourceId] int  NOT NULL
);
GO

-- Creating table 'GeneralTrainingProgram'
CREATE TABLE [dbo].[GeneralTrainingProgram] (
    [IdGeneralTrainingProgram] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(1000)  NOT NULL,
    [Category] int  NOT NULL,
    [Link] varchar(1000)  NULL,
    [Enabled] bit  NOT NULL,
    [StartDate] datetime  NULL,
    [FinishDate] datetime  NULL
);
GO

-- Creating table 'GeneralTrainingProgramDetails'
CREATE TABLE [dbo].[GeneralTrainingProgramDetails] (
    [IdGeneralTrainingProgramDetails] int IDENTITY(1,1) NOT NULL,
    [IdGeneralTrainingProgram] int  NOT NULL,
    [Status] int  NOT NULL,
    [ResourceId] int  NOT NULL
);
GO

-- Creating table 'TrainingProgramOnDemand'
CREATE TABLE [dbo].[TrainingProgramOnDemand] (
    [IdTrainingProgramOnDemand] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(100)  NULL,
    [Link] varchar(1000)  NULL,
    [StartDate] datetime  NULL,
    [FinishDate] datetime  NULL,
    [Enable] bit  NULL
);
GO

-- Creating table 'TrainingProgramOnDemandDetails'
CREATE TABLE [dbo].[TrainingProgramOnDemandDetails] (
    [IdTrainingProgramOnDemandDetails] int IDENTITY(1,1) NOT NULL,
    [IdTrainingProgramOnDemand] int  NOT NULL,
    [Status] int  NOT NULL,
    [ResourceId] int  NOT NULL
);
GO

-- Creating table 'SkillCompassGlossary'
CREATE TABLE [dbo].[SkillCompassGlossary] (
    [SkillCompassGlossaryId] int IDENTITY(1,1) NOT NULL,
    [AreaId] int  NOT NULL,
    [Name] varchar(100)  NULL,
    [Description] varchar(max)  NULL
);
GO

-- Creating table 'Project'
CREATE TABLE [dbo].[Project] (
    [ProjectId] int IDENTITY(1,1) NOT NULL,
    [Project1] varchar(80)  NULL
);
GO

-- Creating table 'GoalDates'
CREATE TABLE [dbo].[GoalDates] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [date] varchar(8)  NULL
);
GO

-- Creating table 'GoalEnum'
CREATE TABLE [dbo].[GoalEnum] (
    [GoalTypeId] int  NOT NULL,
    [Description] nvarchar(255)  NULL
);
GO

-- Creating table 'Term'
CREATE TABLE [dbo].[Term] (
    [TermId] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL
);
GO

-- Creating table 'TrainingCategory'
CREATE TABLE [dbo].[TrainingCategory] (
    [TrainingCategoryId] int  NOT NULL,
    [Name] nvarchar(100)  NULL,
    [Description] nvarchar(1000)  NULL,
    [TDU] int  NULL,
    [MaxTDU] int  NULL
);
GO

-- Creating table 'GeneralTrainingProgramVideo'
CREATE TABLE [dbo].[GeneralTrainingProgramVideo] (
    [IdGeneralTrainingProgramVideo] int IDENTITY(1,1) NOT NULL,
    [IdGenetalTrainingProgram] int  NOT NULL,
    [IdLocation] int  NULL,
    [LinkVideo] varchar(1000)  NULL
);
GO

-- Creating table 'TrainingProgramVideo'
CREATE TABLE [dbo].[TrainingProgramVideo] (
    [IdTrainingProgramVideo] int IDENTITY(1,1) NOT NULL,
    [IdTrainingProgram] int  NOT NULL,
    [IdLocation] int  NULL,
    [LinkVideo] varchar(1000)  NULL
);
GO

-- Creating table 'GoalsCompletedTotal'
CREATE TABLE [dbo].[GoalsCompletedTotal] (
    [Location] varchar(1)  NOT NULL,
    [Manager] varchar(1)  NOT NULL,
    [ResourceId] int  NOT NULL,
    [DomainName] nvarchar(50)  NOT NULL,
    [Name] nvarchar(250)  NULL,
    [Verified] int  NULL,
    [TillThisMonth] int  NOT NULL
);
GO

-- Creating table 'ResourceGoals'
CREATE TABLE [dbo].[ResourceGoals] (
    [Location] varchar(1)  NOT NULL,
    [Employee] nvarchar(250)  NULL,
    [ResourceId] int  NOT NULL,
    [DomainName] nvarchar(50)  NOT NULL,
    [Goal] nvarchar(1000)  NOT NULL,
    [Verified] int  NOT NULL
);
GO

-- Creating table 'Sources'
CREATE TABLE [dbo].[Sources] (
    [SourceId] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100)  NULL
);
GO

-- Creating table 'GoalsView'
CREATE TABLE [dbo].[GoalsView] (
    [GoalId] int  NOT NULL,
    [ManagerId] int  NOT NULL,
    [Manager] nvarchar(250)  NULL,
    [ResourceId] int  NOT NULL,
    [Employee] nvarchar(250)  NULL,
    [Goal] nvarchar(1000)  NOT NULL,
    [FinishDate] datetime  NULL,
    [Verified] int  NOT NULL,
    [MustCheck] int  NOT NULL,
    [Progress] int  NOT NULL,
    [TDU] int  NULL,
    [Objective] nvarchar(1000)  NULL,
    [TrainingCategory] nvarchar(100)  NULL,
    [ManagerLocation] varchar(50)  NOT NULL,
    [EmployeeLocation] varchar(50)  NOT NULL
);
GO

-- Creating table 'CDPView'
CREATE TABLE [dbo].[CDPView] (
    [PositionName] varchar(200)  NOT NULL,
    [CDP] int  NULL,
    [Name] varchar(50)  NOT NULL,
    [Year] int  NULL,
    [Quarter] int  NULL
);
GO

-- Creating table 'GeneralTrainingProgramVisits'
CREATE TABLE [dbo].[GeneralTrainingProgramVisits] (
    [IdVisit] int IDENTITY(1,1) NOT NULL,
    [IdGeneralTrainingProgram] int  NOT NULL,
    [ResourceId] int  NOT NULL,
    [VisitDate] datetime  NOT NULL
);
GO

-- Creating table 'TrainingProgramOnDemandVisits'
CREATE TABLE [dbo].[TrainingProgramOnDemandVisits] (
    [IdVisit] int IDENTITY(1,1) NOT NULL,
    [IdTrainingProgramOnDemand] int  NOT NULL,
    [ResourceId] int  NOT NULL,
    [VisitDate] datetime  NOT NULL
);
GO

-- Creating table 'TrainingProgramVisits'
CREATE TABLE [dbo].[TrainingProgramVisits] (
    [IdVisit] int IDENTITY(1,1) NOT NULL,
    [IdTrainingProgram] int  NOT NULL,
    [ResourceId] int  NOT NULL,
    [VisitDate] datetime  NOT NULL
);
GO

-- Creating table 'TrainingProgramOnDemandVisitsView'
CREATE TABLE [dbo].[TrainingProgramOnDemandVisitsView] (
    [Name] varchar(100)  NULL,
    [Resource] nvarchar(250)  NULL,
    [Visits] int  NULL,
    [IdVisit] int  NOT NULL,
    [VisitDate] datetime  NOT NULL
);
GO

-- Creating table 'TrainingProgramVisitsView'
CREATE TABLE [dbo].[TrainingProgramVisitsView] (
    [Name] varchar(1000)  NOT NULL,
    [Resource] nvarchar(250)  NULL,
    [Visits] int  NULL,
    [VisitDate] datetime  NOT NULL,
    [Category] varchar(1000)  NOT NULL
);
GO

-- Creating table 'GeneralTrainingProgramVisitsView'
CREATE TABLE [dbo].[GeneralTrainingProgramVisitsView] (
    [Name] varchar(1000)  NOT NULL,
    [Resource] nvarchar(250)  NULL,
    [Visits] int  NULL,
    [VisitDate] datetime  NOT NULL,
    [Category] varchar(1000)  NOT NULL
);
GO

-- Creating table 'TDURedeem'
CREATE TABLE [dbo].[TDURedeem] (
    [TDUReedeemId] int IDENTITY(1,1) NOT NULL,
    [resourceId] int  NOT NULL,
    [QuarterYear] int  NOT NULL,
    [Quarter] int  NOT NULL,
    [TDU] int  NOT NULL,
    [DateReached] datetime  NOT NULL,
    [Redeemed] bit  NULL,
    [Paid] bit  NULL,
    [DateRedeemed] datetime  NULL,
    [DatePaid] datetime  NULL,
    [TDUReward] int  NULL
);
GO

-- Creating table 'TDUReward'
CREATE TABLE [dbo].[TDUReward] (
    [TDURewardId] int IDENTITY(1,1) NOT NULL,
    [resourceId] int  NOT NULL,
    [StartingQuarter] int  NOT NULL,
    [EndingQuarter] int  NOT NULL,
    [StartingYear] int  NOT NULL,
    [EndingYear] int  NOT NULL,
    [TotalTDUReward] int  NOT NULL,
    [Redeemed] bit  NOT NULL,
    [DateRedeemed] datetime  NULL,
    [Paid] bit  NOT NULL,
    [DatePaid] datetime  NULL,
    [ValidForQuarters] int  NOT NULL,
    [DatetoLoseValidity] datetime  NOT NULL
);
GO

-- Creating table 'Question'
CREATE TABLE [dbo].[Question] (
    [QuestionId] int  NOT NULL,
    [SurveyId] int  NOT NULL,
    [Text] varchar(200)  NOT NULL,
    [Updated] datetime  NOT NULL,
    [Sequence] int  NOT NULL,
    [QuestionType] int  NOT NULL,
    [QuestionChild] int  NULL,
    [Required] int  NULL,
    [DisplayWhenValue] int  NULL,
    [VisibleForEmployee] int  NULL
);
GO

-- Creating table 'QuestionResponse'
CREATE TABLE [dbo].[QuestionResponse] (
    [QuestionId] int  NOT NULL,
    [ResponseId] int  NOT NULL,
    [QuestionResponseId] int  NOT NULL
);
GO

-- Creating table 'Response'
CREATE TABLE [dbo].[Response] (
    [ResponseId] int  NOT NULL,
    [Answer] varchar(1000)  NULL
);
GO

-- Creating table 'SurveyResource'
CREATE TABLE [dbo].[SurveyResource] (
    [SurveyResourceId] int  NOT NULL,
    [ResourceId] int  NOT NULL,
    [SurveyId] int  NOT NULL,
    [DateAnswered] datetime  NOT NULL,
    [SurveyType] int  NOT NULL,
    [ResourceEvaluatedId] int  NULL
);
GO

-- Creating table 'Survey'
CREATE TABLE [dbo].[Survey] (
    [SurveyId] int  NOT NULL,
    [Name] varchar(50)  NULL,
    [Description] varchar(50)  NOT NULL,
    [SurveyType] int  NULL,
    [CreatedBy] int  NULL,
    [CreatedTimeStamp] datetime  NULL,
    [Quarter] int  NULL,
    [Year] int  NULL
);
GO

-- Creating table 'SurveyResponse'
CREATE TABLE [dbo].[SurveyResponse] (
    [SurveyResponseId] int IDENTITY(1,1) NOT NULL,
    [QuestionId] int  NOT NULL,
    [ResourceId] int  NOT NULL,
    [ResponseId] int  NULL,
    [ResponseText] varchar(555)  NULL,
    [SurveyResourceId] int  NOT NULL
);
GO

-- Creating table 'Groups'
CREATE TABLE [dbo].[Groups] (
    [GroupId] int  NOT NULL,
    [GroupName] nvarchar(50)  NULL
);
GO

-- Creating table 'Employee_Groups'
CREATE TABLE [dbo].[Employee_Groups] (
    [ResourceId] int  NOT NULL,
    [GroupId] varchar(50)  NOT NULL
);
GO

-- Creating table 'CoreValues'
CREATE TABLE [dbo].[CoreValues] (
    [CoreValuesId] int  NOT NULL,
    [coreValue] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'KeyThrusts'
CREATE TABLE [dbo].[KeyThrusts] (
    [KeyThrustsId] int  NOT NULL,
    [OnePagePlanId] int  NOT NULL,
    [KeyThrust] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'OnePagePlan'
CREATE TABLE [dbo].[OnePagePlan] (
    [OnePagePlanId] int IDENTITY(1,1) NOT NULL,
    [Quarter] int  NULL,
    [year] int  NULL,
    [month1_CoreValueId] int  NULL,
    [month2_CoreValueId] int  NULL,
    [month3_CoreValueId] int  NULL,
    [SG] varchar(150)  NULL,
    [G] varchar(150)  NULL,
    [R] varchar(150)  NULL,
    [ResourceId] int  NOT NULL
);
GO

-- Creating table 'AnnualPriorities'
CREATE TABLE [dbo].[AnnualPriorities] (
    [AnnualPrioritiesId] int IDENTITY(1,1) NOT NULL,
    [OnePagePlanId] int  NOT NULL,
    [AnnualPriorities1] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'KPI'
CREATE TABLE [dbo].[KPI] (
    [KpiId] int IDENTITY(1,1) NOT NULL,
    [KpiName] varchar(255)  NULL,
    [Goal] varchar(255)  NULL,
    [Result] varchar(255)  NULL,
    [QuarterlyPrioritiesId] int  NOT NULL
);
GO

-- Creating table 'PersonalDevelopment'
CREATE TABLE [dbo].[PersonalDevelopment] (
    [PersonalDevelopmentId] int IDENTITY(1,1) NOT NULL,
    [Skill] varchar(255)  NULL,
    [DefinitionSuccess] varchar(255)  NULL,
    [Outcome] varchar(255)  NULL,
    [QuarterlyPrioritiesId] int  NULL
);
GO

-- Creating table 'QuarterlyActions'
CREATE TABLE [dbo].[QuarterlyActions] (
    [QuarterlyActionId] int IDENTITY(1,1) NOT NULL,
    [Action] varchar(255)  NULL,
    [DueDate] datetime  NULL,
    [KeyIniciative] int  NULL,
    [Outcome] varchar(255)  NULL,
    [QuarterlyPrioritiesId] int  NULL
);
GO

-- Creating table 'QuarterlyPriorities'
CREATE TABLE [dbo].[QuarterlyPriorities] (
    [QuarterlyPrioritiesId] int IDENTITY(1,1) NOT NULL,
    [Quarter] int  NULL,
    [Year] int  NULL,
    [ResourceId] int  NULL,
    [CreationDate] datetime  NULL,
    [OnePagePlanId] int  NULL,
    [KeyIssues] varchar(512)  NULL
);
GO

-- Creating table 'ValuesInfusion'
CREATE TABLE [dbo].[ValuesInfusion] (
    [ValuesInfusionId] int IDENTITY(1,1) NOT NULL,
    [Action] varchar(255)  NULL,
    [IsDone] bit  NULL,
    [QuarterlyPrioritiesId] int  NULL
);
GO

-- Creating table 'AppError'
CREATE TABLE [dbo].[AppError] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [errcode] int  NOT NULL,
    [message] nvarchar(150)  NOT NULL
);
GO

-- Creating table 'Group_SectionAccess'
CREATE TABLE [dbo].[Group_SectionAccess] (
    [id] bigint  NOT NULL,
    [GroupId] int  NOT NULL,
    [Section] nvarchar(50)  NOT NULL,
    [Allow] bit  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [GoalId] in table 'GoalTrackings'
ALTER TABLE [dbo].[GoalTrackings]
ADD CONSTRAINT [PK_GoalTrackings]
    PRIMARY KEY CLUSTERED ([GoalId] ASC);
GO

-- Creating primary key on [Id] in table 'ProgressEnums'
ALTER TABLE [dbo].[ProgressEnums]
ADD CONSTRAINT [PK_ProgressEnums]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ResourceId] in table 'Resources'
ALTER TABLE [dbo].[Resources]
ADD CONSTRAINT [PK_Resources]
    PRIMARY KEY CLUSTERED ([ResourceId] ASC);
GO

-- Creating primary key on [ID] in table 'Locations'
ALTER TABLE [dbo].[Locations]
ADD CONSTRAINT [PK_Locations]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ObjectiveId] in table 'Objectives'
ALTER TABLE [dbo].[Objectives]
ADD CONSTRAINT [PK_Objectives]
    PRIMARY KEY CLUSTERED ([ObjectiveId] ASC);
GO

-- Creating primary key on [CategoryId] in table 'Category'
ALTER TABLE [dbo].[Category]
ADD CONSTRAINT [PK_Category]
    PRIMARY KEY CLUSTERED ([CategoryId] ASC);
GO

-- Creating primary key on [AreaId] in table 'Area'
ALTER TABLE [dbo].[Area]
ADD CONSTRAINT [PK_Area]
    PRIMARY KEY CLUSTERED ([AreaId] ASC);
GO

-- Creating primary key on [ResourceId] in table 'Employee'
ALTER TABLE [dbo].[Employee]
ADD CONSTRAINT [PK_Employee]
    PRIMARY KEY CLUSTERED ([ResourceId] ASC);
GO

-- Creating primary key on [PositionId] in table 'Position'
ALTER TABLE [dbo].[Position]
ADD CONSTRAINT [PK_Position]
    PRIMARY KEY CLUSTERED ([PositionId] ASC);
GO

-- Creating primary key on [PositionHierarchyId] in table 'PositionsHierarchy'
ALTER TABLE [dbo].[PositionsHierarchy]
ADD CONSTRAINT [PK_PositionsHierarchy]
    PRIMARY KEY CLUSTERED ([PositionHierarchyId] ASC);
GO

-- Creating primary key on [SuggestionId] in table 'Suggestions'
ALTER TABLE [dbo].[Suggestions]
ADD CONSTRAINT [PK_Suggestions]
    PRIMARY KEY CLUSTERED ([SuggestionId] ASC);
GO

-- Creating primary key on [TechnologyId] in table 'Technologies'
ALTER TABLE [dbo].[Technologies]
ADD CONSTRAINT [PK_Technologies]
    PRIMARY KEY CLUSTERED ([TechnologyId] ASC);
GO

-- Creating primary key on [LevelId] in table 'Level'
ALTER TABLE [dbo].[Level]
ADD CONSTRAINT [PK_Level]
    PRIMARY KEY CLUSTERED ([LevelId] ASC);
GO

-- Creating primary key on [IdTrainingProgram] in table 'TrainingProgram'
ALTER TABLE [dbo].[TrainingProgram]
ADD CONSTRAINT [PK_TrainingProgram]
    PRIMARY KEY CLUSTERED ([IdTrainingProgram] ASC);
GO

-- Creating primary key on [IdTrainingProgramCategory] in table 'TrainingProgramCategory'
ALTER TABLE [dbo].[TrainingProgramCategory]
ADD CONSTRAINT [PK_TrainingProgramCategory]
    PRIMARY KEY CLUSTERED ([IdTrainingProgramCategory] ASC);
GO

-- Creating primary key on [IdTrainingProgramDetails] in table 'TrainingProgramDetails'
ALTER TABLE [dbo].[TrainingProgramDetails]
ADD CONSTRAINT [PK_TrainingProgramDetails]
    PRIMARY KEY CLUSTERED ([IdTrainingProgramDetails] ASC);
GO

-- Creating primary key on [IdGeneralTrainingProgram] in table 'GeneralTrainingProgram'
ALTER TABLE [dbo].[GeneralTrainingProgram]
ADD CONSTRAINT [PK_GeneralTrainingProgram]
    PRIMARY KEY CLUSTERED ([IdGeneralTrainingProgram] ASC);
GO

-- Creating primary key on [IdGeneralTrainingProgramDetails] in table 'GeneralTrainingProgramDetails'
ALTER TABLE [dbo].[GeneralTrainingProgramDetails]
ADD CONSTRAINT [PK_GeneralTrainingProgramDetails]
    PRIMARY KEY CLUSTERED ([IdGeneralTrainingProgramDetails] ASC);
GO

-- Creating primary key on [IdTrainingProgramOnDemand] in table 'TrainingProgramOnDemand'
ALTER TABLE [dbo].[TrainingProgramOnDemand]
ADD CONSTRAINT [PK_TrainingProgramOnDemand]
    PRIMARY KEY CLUSTERED ([IdTrainingProgramOnDemand] ASC);
GO

-- Creating primary key on [IdTrainingProgramOnDemandDetails] in table 'TrainingProgramOnDemandDetails'
ALTER TABLE [dbo].[TrainingProgramOnDemandDetails]
ADD CONSTRAINT [PK_TrainingProgramOnDemandDetails]
    PRIMARY KEY CLUSTERED ([IdTrainingProgramOnDemandDetails] ASC);
GO

-- Creating primary key on [SkillCompassGlossaryId] in table 'SkillCompassGlossary'
ALTER TABLE [dbo].[SkillCompassGlossary]
ADD CONSTRAINT [PK_SkillCompassGlossary]
    PRIMARY KEY CLUSTERED ([SkillCompassGlossaryId] ASC);
GO

-- Creating primary key on [ProjectId] in table 'Project'
ALTER TABLE [dbo].[Project]
ADD CONSTRAINT [PK_Project]
    PRIMARY KEY CLUSTERED ([ProjectId] ASC);
GO

-- Creating primary key on [ID] in table 'GoalDates'
ALTER TABLE [dbo].[GoalDates]
ADD CONSTRAINT [PK_GoalDates]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [GoalTypeId] in table 'GoalEnum'
ALTER TABLE [dbo].[GoalEnum]
ADD CONSTRAINT [PK_GoalEnum]
    PRIMARY KEY CLUSTERED ([GoalTypeId] ASC);
GO

-- Creating primary key on [TermId] in table 'Term'
ALTER TABLE [dbo].[Term]
ADD CONSTRAINT [PK_Term]
    PRIMARY KEY CLUSTERED ([TermId] ASC);
GO

-- Creating primary key on [TrainingCategoryId] in table 'TrainingCategory'
ALTER TABLE [dbo].[TrainingCategory]
ADD CONSTRAINT [PK_TrainingCategory]
    PRIMARY KEY CLUSTERED ([TrainingCategoryId] ASC);
GO

-- Creating primary key on [IdGeneralTrainingProgramVideo] in table 'GeneralTrainingProgramVideo'
ALTER TABLE [dbo].[GeneralTrainingProgramVideo]
ADD CONSTRAINT [PK_GeneralTrainingProgramVideo]
    PRIMARY KEY CLUSTERED ([IdGeneralTrainingProgramVideo] ASC);
GO

-- Creating primary key on [IdTrainingProgramVideo] in table 'TrainingProgramVideo'
ALTER TABLE [dbo].[TrainingProgramVideo]
ADD CONSTRAINT [PK_TrainingProgramVideo]
    PRIMARY KEY CLUSTERED ([IdTrainingProgramVideo] ASC);
GO

-- Creating primary key on [Location], [Manager], [ResourceId], [DomainName], [TillThisMonth] in table 'GoalsCompletedTotal'
ALTER TABLE [dbo].[GoalsCompletedTotal]
ADD CONSTRAINT [PK_GoalsCompletedTotal]
    PRIMARY KEY CLUSTERED ([Location], [Manager], [ResourceId], [DomainName], [TillThisMonth] ASC);
GO

-- Creating primary key on [Location], [ResourceId], [DomainName], [Goal], [Verified] in table 'ResourceGoals'
ALTER TABLE [dbo].[ResourceGoals]
ADD CONSTRAINT [PK_ResourceGoals]
    PRIMARY KEY CLUSTERED ([Location], [ResourceId], [DomainName], [Goal], [Verified] ASC);
GO

-- Creating primary key on [SourceId] in table 'Sources'
ALTER TABLE [dbo].[Sources]
ADD CONSTRAINT [PK_Sources]
    PRIMARY KEY CLUSTERED ([SourceId] ASC);
GO

-- Creating primary key on [GoalId], [ResourceId], [Goal], [Verified], [MustCheck], [Progress], [ManagerLocation], [EmployeeLocation] in table 'GoalsView'
ALTER TABLE [dbo].[GoalsView]
ADD CONSTRAINT [PK_GoalsView]
    PRIMARY KEY CLUSTERED ([GoalId], [ResourceId], [Goal], [Verified], [MustCheck], [Progress], [ManagerLocation], [EmployeeLocation] ASC);
GO

-- Creating primary key on [PositionName], [Name] in table 'CDPView'
ALTER TABLE [dbo].[CDPView]
ADD CONSTRAINT [PK_CDPView]
    PRIMARY KEY CLUSTERED ([PositionName], [Name] ASC);
GO

-- Creating primary key on [IdVisit] in table 'GeneralTrainingProgramVisits'
ALTER TABLE [dbo].[GeneralTrainingProgramVisits]
ADD CONSTRAINT [PK_GeneralTrainingProgramVisits]
    PRIMARY KEY CLUSTERED ([IdVisit] ASC);
GO

-- Creating primary key on [IdVisit] in table 'TrainingProgramOnDemandVisits'
ALTER TABLE [dbo].[TrainingProgramOnDemandVisits]
ADD CONSTRAINT [PK_TrainingProgramOnDemandVisits]
    PRIMARY KEY CLUSTERED ([IdVisit] ASC);
GO

-- Creating primary key on [IdVisit] in table 'TrainingProgramVisits'
ALTER TABLE [dbo].[TrainingProgramVisits]
ADD CONSTRAINT [PK_TrainingProgramVisits]
    PRIMARY KEY CLUSTERED ([IdVisit] ASC);
GO

-- Creating primary key on [IdVisit], [VisitDate] in table 'TrainingProgramOnDemandVisitsView'
ALTER TABLE [dbo].[TrainingProgramOnDemandVisitsView]
ADD CONSTRAINT [PK_TrainingProgramOnDemandVisitsView]
    PRIMARY KEY CLUSTERED ([IdVisit], [VisitDate] ASC);
GO

-- Creating primary key on [Name], [VisitDate], [Category] in table 'TrainingProgramVisitsView'
ALTER TABLE [dbo].[TrainingProgramVisitsView]
ADD CONSTRAINT [PK_TrainingProgramVisitsView]
    PRIMARY KEY CLUSTERED ([Name], [VisitDate], [Category] ASC);
GO

-- Creating primary key on [Name], [VisitDate], [Category] in table 'GeneralTrainingProgramVisitsView'
ALTER TABLE [dbo].[GeneralTrainingProgramVisitsView]
ADD CONSTRAINT [PK_GeneralTrainingProgramVisitsView]
    PRIMARY KEY CLUSTERED ([Name], [VisitDate], [Category] ASC);
GO

-- Creating primary key on [TDUReedeemId] in table 'TDURedeem'
ALTER TABLE [dbo].[TDURedeem]
ADD CONSTRAINT [PK_TDURedeem]
    PRIMARY KEY CLUSTERED ([TDUReedeemId] ASC);
GO

-- Creating primary key on [TDURewardId] in table 'TDUReward'
ALTER TABLE [dbo].[TDUReward]
ADD CONSTRAINT [PK_TDUReward]
    PRIMARY KEY CLUSTERED ([TDURewardId] ASC);
GO

-- Creating primary key on [QuestionId] in table 'Question'
ALTER TABLE [dbo].[Question]
ADD CONSTRAINT [PK_Question]
    PRIMARY KEY CLUSTERED ([QuestionId] ASC);
GO

-- Creating primary key on [QuestionResponseId] in table 'QuestionResponse'
ALTER TABLE [dbo].[QuestionResponse]
ADD CONSTRAINT [PK_QuestionResponse]
    PRIMARY KEY CLUSTERED ([QuestionResponseId] ASC);
GO

-- Creating primary key on [ResponseId] in table 'Response'
ALTER TABLE [dbo].[Response]
ADD CONSTRAINT [PK_Response]
    PRIMARY KEY CLUSTERED ([ResponseId] ASC);
GO

-- Creating primary key on [SurveyResourceId] in table 'SurveyResource'
ALTER TABLE [dbo].[SurveyResource]
ADD CONSTRAINT [PK_SurveyResource]
    PRIMARY KEY CLUSTERED ([SurveyResourceId] ASC);
GO

-- Creating primary key on [SurveyId] in table 'Survey'
ALTER TABLE [dbo].[Survey]
ADD CONSTRAINT [PK_Survey]
    PRIMARY KEY CLUSTERED ([SurveyId] ASC);
GO

-- Creating primary key on [SurveyResponseId] in table 'SurveyResponse'
ALTER TABLE [dbo].[SurveyResponse]
ADD CONSTRAINT [PK_SurveyResponse]
    PRIMARY KEY CLUSTERED ([SurveyResponseId] ASC);
GO

-- Creating primary key on [GroupId] in table 'Groups'
ALTER TABLE [dbo].[Groups]
ADD CONSTRAINT [PK_Groups]
    PRIMARY KEY CLUSTERED ([GroupId] ASC);
GO

-- Creating primary key on [ResourceId], [GroupId] in table 'Employee_Groups'
ALTER TABLE [dbo].[Employee_Groups]
ADD CONSTRAINT [PK_Employee_Groups]
    PRIMARY KEY CLUSTERED ([ResourceId], [GroupId] ASC);
GO

-- Creating primary key on [CoreValuesId] in table 'CoreValues'
ALTER TABLE [dbo].[CoreValues]
ADD CONSTRAINT [PK_CoreValues]
    PRIMARY KEY CLUSTERED ([CoreValuesId] ASC);
GO

-- Creating primary key on [KeyThrustsId] in table 'KeyThrusts'
ALTER TABLE [dbo].[KeyThrusts]
ADD CONSTRAINT [PK_KeyThrusts]
    PRIMARY KEY CLUSTERED ([KeyThrustsId] ASC);
GO

-- Creating primary key on [OnePagePlanId] in table 'OnePagePlan'
ALTER TABLE [dbo].[OnePagePlan]
ADD CONSTRAINT [PK_OnePagePlan]
    PRIMARY KEY CLUSTERED ([OnePagePlanId] ASC);
GO

-- Creating primary key on [AnnualPrioritiesId] in table 'AnnualPriorities'
ALTER TABLE [dbo].[AnnualPriorities]
ADD CONSTRAINT [PK_AnnualPriorities]
    PRIMARY KEY CLUSTERED ([AnnualPrioritiesId] ASC);
GO

-- Creating primary key on [KpiId] in table 'KPI'
ALTER TABLE [dbo].[KPI]
ADD CONSTRAINT [PK_KPI]
    PRIMARY KEY CLUSTERED ([KpiId] ASC);
GO

-- Creating primary key on [PersonalDevelopmentId] in table 'PersonalDevelopment'
ALTER TABLE [dbo].[PersonalDevelopment]
ADD CONSTRAINT [PK_PersonalDevelopment]
    PRIMARY KEY CLUSTERED ([PersonalDevelopmentId] ASC);
GO

-- Creating primary key on [QuarterlyActionId] in table 'QuarterlyActions'
ALTER TABLE [dbo].[QuarterlyActions]
ADD CONSTRAINT [PK_QuarterlyActions]
    PRIMARY KEY CLUSTERED ([QuarterlyActionId] ASC);
GO

-- Creating primary key on [QuarterlyPrioritiesId] in table 'QuarterlyPriorities'
ALTER TABLE [dbo].[QuarterlyPriorities]
ADD CONSTRAINT [PK_QuarterlyPriorities]
    PRIMARY KEY CLUSTERED ([QuarterlyPrioritiesId] ASC);
GO

-- Creating primary key on [ValuesInfusionId] in table 'ValuesInfusion'
ALTER TABLE [dbo].[ValuesInfusion]
ADD CONSTRAINT [PK_ValuesInfusion]
    PRIMARY KEY CLUSTERED ([ValuesInfusionId] ASC);
GO

-- Creating primary key on [Id] in table 'AppError'
ALTER TABLE [dbo].[AppError]
ADD CONSTRAINT [PK_AppError]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [id] in table 'Group_SectionAccess'
ALTER TABLE [dbo].[Group_SectionAccess]
ADD CONSTRAINT [PK_Group_SectionAccess]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ResourceId] in table 'GoalTrackings'
ALTER TABLE [dbo].[GoalTrackings]
ADD CONSTRAINT [FK__GoalTrack__Resou__3A81B327]
    FOREIGN KEY ([ResourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK__GoalTrack__Resou__3A81B327'
CREATE INDEX [IX_FK__GoalTrack__Resou__3A81B327]
ON [dbo].[GoalTrackings]
    ([ResourceId]);
GO

-- Creating foreign key on [Progress] in table 'GoalTrackings'
ALTER TABLE [dbo].[GoalTrackings]
ADD CONSTRAINT [FK_GoalTracking_ProgressEnum]
    FOREIGN KEY ([Progress])
    REFERENCES [dbo].[ProgressEnums]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GoalTracking_ProgressEnum'
CREATE INDEX [IX_FK_GoalTracking_ProgressEnum]
ON [dbo].[GoalTrackings]
    ([Progress]);
GO

-- Creating foreign key on [LocationId] in table 'Resources'
ALTER TABLE [dbo].[Resources]
ADD CONSTRAINT [FK__Resource__Locati__4F7CD00D]
    FOREIGN KEY ([LocationId])
    REFERENCES [dbo].[Locations]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK__Resource__Locati__4F7CD00D'
CREATE INDEX [IX_FK__Resource__Locati__4F7CD00D]
ON [dbo].[Resources]
    ([LocationId]);
GO

-- Creating foreign key on [ObjectiveId] in table 'GoalTrackings'
ALTER TABLE [dbo].[GoalTrackings]
ADD CONSTRAINT [FK_GoalTracking_Objective]
    FOREIGN KEY ([ObjectiveId])
    REFERENCES [dbo].[Objectives]
        ([ObjectiveId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GoalTracking_Objective'
CREATE INDEX [IX_FK_GoalTracking_Objective]
ON [dbo].[GoalTrackings]
    ([ObjectiveId]);
GO

-- Creating foreign key on [ResourceId] in table 'Objectives'
ALTER TABLE [dbo].[Objectives]
ADD CONSTRAINT [FK_Objective_Resource]
    FOREIGN KEY ([ResourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Objective_Resource'
CREATE INDEX [IX_FK_Objective_Resource]
ON [dbo].[Objectives]
    ([ResourceId]);
GO

-- Creating foreign key on [CategoryId] in table 'Objectives'
ALTER TABLE [dbo].[Objectives]
ADD CONSTRAINT [FK_Objective_Category]
    FOREIGN KEY ([CategoryId])
    REFERENCES [dbo].[Category]
        ([CategoryId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Objective_Category'
CREATE INDEX [IX_FK_Objective_Category]
ON [dbo].[Objectives]
    ([CategoryId]);
GO

-- Creating foreign key on [ResourceId] in table 'Employee'
ALTER TABLE [dbo].[Employee]
ADD CONSTRAINT [FK_Employee_Resource]
    FOREIGN KEY ([ResourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [ManagerId] in table 'Employee'
ALTER TABLE [dbo].[Employee]
ADD CONSTRAINT [FK_Manager_Resource]
    FOREIGN KEY ([ManagerId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Manager_Resource'
CREATE INDEX [IX_FK_Manager_Resource]
ON [dbo].[Employee]
    ([ManagerId]);
GO

-- Creating foreign key on [AreaId] in table 'Position'
ALTER TABLE [dbo].[Position]
ADD CONSTRAINT [FK_Position_Area]
    FOREIGN KEY ([AreaId])
    REFERENCES [dbo].[Area]
        ([AreaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Position_Area'
CREATE INDEX [IX_FK_Position_Area]
ON [dbo].[Position]
    ([AreaId]);
GO

-- Creating foreign key on [CurrentPositionID] in table 'Employee'
ALTER TABLE [dbo].[Employee]
ADD CONSTRAINT [FK_Employee_Position]
    FOREIGN KEY ([CurrentPositionID])
    REFERENCES [dbo].[Position]
        ([PositionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Employee_Position'
CREATE INDEX [IX_FK_Employee_Position]
ON [dbo].[Employee]
    ([CurrentPositionID]);
GO

-- Creating foreign key on [AspiringPositionID] in table 'Employee'
ALTER TABLE [dbo].[Employee]
ADD CONSTRAINT [FK_Employee_Position1]
    FOREIGN KEY ([AspiringPositionID])
    REFERENCES [dbo].[Position]
        ([PositionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Employee_Position1'
CREATE INDEX [IX_FK_Employee_Position1]
ON [dbo].[Employee]
    ([AspiringPositionID]);
GO

-- Creating foreign key on [PositionId] in table 'PositionsHierarchy'
ALTER TABLE [dbo].[PositionsHierarchy]
ADD CONSTRAINT [FK_PositionsHierarchy_Position]
    FOREIGN KEY ([PositionId])
    REFERENCES [dbo].[Position]
        ([PositionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PositionsHierarchy_Position'
CREATE INDEX [IX_FK_PositionsHierarchy_Position]
ON [dbo].[PositionsHierarchy]
    ([PositionId]);
GO

-- Creating foreign key on [NextPosition] in table 'PositionsHierarchy'
ALTER TABLE [dbo].[PositionsHierarchy]
ADD CONSTRAINT [FK_PositionsHierarchy_Position1]
    FOREIGN KEY ([NextPosition])
    REFERENCES [dbo].[Position]
        ([PositionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PositionsHierarchy_Position1'
CREATE INDEX [IX_FK_PositionsHierarchy_Position1]
ON [dbo].[PositionsHierarchy]
    ([NextPosition]);
GO

-- Creating foreign key on [PositionId] in table 'Suggestions'
ALTER TABLE [dbo].[Suggestions]
ADD CONSTRAINT [FK_Suggestions_Position]
    FOREIGN KEY ([PositionId])
    REFERENCES [dbo].[Position]
        ([PositionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Suggestions_Position'
CREATE INDEX [IX_FK_Suggestions_Position]
ON [dbo].[Suggestions]
    ([PositionId]);
GO

-- Creating foreign key on [TechnologyId] in table 'Suggestions'
ALTER TABLE [dbo].[Suggestions]
ADD CONSTRAINT [FK_Suggestions_Technologies]
    FOREIGN KEY ([TechnologyId])
    REFERENCES [dbo].[Technologies]
        ([TechnologyId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Suggestions_Technologies'
CREATE INDEX [IX_FK_Suggestions_Technologies]
ON [dbo].[Suggestions]
    ([TechnologyId]);
GO

-- Creating foreign key on [LevelId] in table 'Suggestions'
ALTER TABLE [dbo].[Suggestions]
ADD CONSTRAINT [FK_Suggestions_Level]
    FOREIGN KEY ([LevelId])
    REFERENCES [dbo].[Level]
        ([LevelId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Suggestions_Level'
CREATE INDEX [IX_FK_Suggestions_Level]
ON [dbo].[Suggestions]
    ([LevelId]);
GO

-- Creating foreign key on [Position] in table 'TrainingProgram'
ALTER TABLE [dbo].[TrainingProgram]
ADD CONSTRAINT [FK_TrainingProgram_Position]
    FOREIGN KEY ([Position])
    REFERENCES [dbo].[Position]
        ([PositionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgram_Position'
CREATE INDEX [IX_FK_TrainingProgram_Position]
ON [dbo].[TrainingProgram]
    ([Position]);
GO

-- Creating foreign key on [Category] in table 'TrainingProgram'
ALTER TABLE [dbo].[TrainingProgram]
ADD CONSTRAINT [FK_TrainingProgram_TrainingProgramCategory]
    FOREIGN KEY ([Category])
    REFERENCES [dbo].[TrainingProgramCategory]
        ([IdTrainingProgramCategory])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgram_TrainingProgramCategory'
CREATE INDEX [IX_FK_TrainingProgram_TrainingProgramCategory]
ON [dbo].[TrainingProgram]
    ([Category]);
GO

-- Creating foreign key on [IdTrainingProgram] in table 'TrainingProgramDetails'
ALTER TABLE [dbo].[TrainingProgramDetails]
ADD CONSTRAINT [FK_TrainingProgramDetails_TrainingProgram]
    FOREIGN KEY ([IdTrainingProgram])
    REFERENCES [dbo].[TrainingProgram]
        ([IdTrainingProgram])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgramDetails_TrainingProgram'
CREATE INDEX [IX_FK_TrainingProgramDetails_TrainingProgram]
ON [dbo].[TrainingProgramDetails]
    ([IdTrainingProgram]);
GO

-- Creating foreign key on [ResourceId] in table 'TrainingProgramDetails'
ALTER TABLE [dbo].[TrainingProgramDetails]
ADD CONSTRAINT [FK_TrainingProgramDetails_Resource]
    FOREIGN KEY ([ResourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgramDetails_Resource'
CREATE INDEX [IX_FK_TrainingProgramDetails_Resource]
ON [dbo].[TrainingProgramDetails]
    ([ResourceId]);
GO

-- Creating foreign key on [Status] in table 'TrainingProgramDetails'
ALTER TABLE [dbo].[TrainingProgramDetails]
ADD CONSTRAINT [FK_TrainingProgramDetails_ProgressEnum]
    FOREIGN KEY ([Status])
    REFERENCES [dbo].[ProgressEnums]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgramDetails_ProgressEnum'
CREATE INDEX [IX_FK_TrainingProgramDetails_ProgressEnum]
ON [dbo].[TrainingProgramDetails]
    ([Status]);
GO

-- Creating foreign key on [Category] in table 'GeneralTrainingProgram'
ALTER TABLE [dbo].[GeneralTrainingProgram]
ADD CONSTRAINT [FK_GeneralTrainingProgram_TrainingProgramCategory]
    FOREIGN KEY ([Category])
    REFERENCES [dbo].[TrainingProgramCategory]
        ([IdTrainingProgramCategory])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneralTrainingProgram_TrainingProgramCategory'
CREATE INDEX [IX_FK_GeneralTrainingProgram_TrainingProgramCategory]
ON [dbo].[GeneralTrainingProgram]
    ([Category]);
GO

-- Creating foreign key on [IdGeneralTrainingProgram] in table 'GeneralTrainingProgramDetails'
ALTER TABLE [dbo].[GeneralTrainingProgramDetails]
ADD CONSTRAINT [FK_GeneralTrainingProgramDetails_GeneralTrainingProgram]
    FOREIGN KEY ([IdGeneralTrainingProgram])
    REFERENCES [dbo].[GeneralTrainingProgram]
        ([IdGeneralTrainingProgram])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneralTrainingProgramDetails_GeneralTrainingProgram'
CREATE INDEX [IX_FK_GeneralTrainingProgramDetails_GeneralTrainingProgram]
ON [dbo].[GeneralTrainingProgramDetails]
    ([IdGeneralTrainingProgram]);
GO

-- Creating foreign key on [ResourceId] in table 'GeneralTrainingProgramDetails'
ALTER TABLE [dbo].[GeneralTrainingProgramDetails]
ADD CONSTRAINT [FK_GeneralTrainingProgramDetails_Resource]
    FOREIGN KEY ([ResourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneralTrainingProgramDetails_Resource'
CREATE INDEX [IX_FK_GeneralTrainingProgramDetails_Resource]
ON [dbo].[GeneralTrainingProgramDetails]
    ([ResourceId]);
GO

-- Creating foreign key on [Status] in table 'GeneralTrainingProgramDetails'
ALTER TABLE [dbo].[GeneralTrainingProgramDetails]
ADD CONSTRAINT [FK_GeneralTrainingProgramDetails_ProgressEnum]
    FOREIGN KEY ([Status])
    REFERENCES [dbo].[ProgressEnums]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneralTrainingProgramDetails_ProgressEnum'
CREATE INDEX [IX_FK_GeneralTrainingProgramDetails_ProgressEnum]
ON [dbo].[GeneralTrainingProgramDetails]
    ([Status]);
GO

-- Creating foreign key on [ResourceId] in table 'TrainingProgramOnDemandDetails'
ALTER TABLE [dbo].[TrainingProgramOnDemandDetails]
ADD CONSTRAINT [FK_TrainingProgramOnDemandDetails_Resource]
    FOREIGN KEY ([ResourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgramOnDemandDetails_Resource'
CREATE INDEX [IX_FK_TrainingProgramOnDemandDetails_Resource]
ON [dbo].[TrainingProgramOnDemandDetails]
    ([ResourceId]);
GO

-- Creating foreign key on [IdTrainingProgramOnDemand] in table 'TrainingProgramOnDemandDetails'
ALTER TABLE [dbo].[TrainingProgramOnDemandDetails]
ADD CONSTRAINT [FK_TrainingProgramOnDemandDetails_TrainingProgramOnDemand]
    FOREIGN KEY ([IdTrainingProgramOnDemand])
    REFERENCES [dbo].[TrainingProgramOnDemand]
        ([IdTrainingProgramOnDemand])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgramOnDemandDetails_TrainingProgramOnDemand'
CREATE INDEX [IX_FK_TrainingProgramOnDemandDetails_TrainingProgramOnDemand]
ON [dbo].[TrainingProgramOnDemandDetails]
    ([IdTrainingProgramOnDemand]);
GO

-- Creating foreign key on [Status] in table 'TrainingProgramOnDemandDetails'
ALTER TABLE [dbo].[TrainingProgramOnDemandDetails]
ADD CONSTRAINT [FK_TrainingProgramOnDemandDetails_ProgressEnum]
    FOREIGN KEY ([Status])
    REFERENCES [dbo].[ProgressEnums]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgramOnDemandDetails_ProgressEnum'
CREATE INDEX [IX_FK_TrainingProgramOnDemandDetails_ProgressEnum]
ON [dbo].[TrainingProgramOnDemandDetails]
    ([Status]);
GO

-- Creating foreign key on [AreaId] in table 'SkillCompassGlossary'
ALTER TABLE [dbo].[SkillCompassGlossary]
ADD CONSTRAINT [FK_SkillCompassGlossary_Area]
    FOREIGN KEY ([AreaId])
    REFERENCES [dbo].[Area]
        ([AreaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SkillCompassGlossary_Area'
CREATE INDEX [IX_FK_SkillCompassGlossary_Area]
ON [dbo].[SkillCompassGlossary]
    ([AreaId]);
GO

-- Creating foreign key on [ProjectId] in table 'Employee'
ALTER TABLE [dbo].[Employee]
ADD CONSTRAINT [FK_Employee_Project]
    FOREIGN KEY ([ProjectId])
    REFERENCES [dbo].[Project]
        ([ProjectId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Employee_Project'
CREATE INDEX [IX_FK_Employee_Project]
ON [dbo].[Employee]
    ([ProjectId]);
GO

-- Creating foreign key on [AreaId] in table 'Employee'
ALTER TABLE [dbo].[Employee]
ADD CONSTRAINT [FK_Employee_Area]
    FOREIGN KEY ([AreaId])
    REFERENCES [dbo].[Area]
        ([AreaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Employee_Area'
CREATE INDEX [IX_FK_Employee_Area]
ON [dbo].[Employee]
    ([AreaId]);
GO

-- Creating foreign key on [Progress] in table 'Objectives'
ALTER TABLE [dbo].[Objectives]
ADD CONSTRAINT [fk_Progress]
    FOREIGN KEY ([Progress])
    REFERENCES [dbo].[ProgressEnums]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'fk_Progress'
CREATE INDEX [IX_fk_Progress]
ON [dbo].[Objectives]
    ([Progress]);
GO

-- Creating foreign key on [TrainingCategoryId] in table 'GoalTrackings'
ALTER TABLE [dbo].[GoalTrackings]
ADD CONSTRAINT [FK_GoalTracking_TrainingCategory]
    FOREIGN KEY ([TrainingCategoryId])
    REFERENCES [dbo].[TrainingCategory]
        ([TrainingCategoryId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GoalTracking_TrainingCategory'
CREATE INDEX [IX_FK_GoalTracking_TrainingCategory]
ON [dbo].[GoalTrackings]
    ([TrainingCategoryId]);
GO

-- Creating foreign key on [TrainingCategoryId] in table 'TrainingCategory'
ALTER TABLE [dbo].[TrainingCategory]
ADD CONSTRAINT [FK_TrainingCategory_TrainingCategory]
    FOREIGN KEY ([TrainingCategoryId])
    REFERENCES [dbo].[TrainingCategory]
        ([TrainingCategoryId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [IdGenetalTrainingProgram] in table 'GeneralTrainingProgramVideo'
ALTER TABLE [dbo].[GeneralTrainingProgramVideo]
ADD CONSTRAINT [FK_GeneralTrainingProgramVideo_GeneralTrainingProgram]
    FOREIGN KEY ([IdGenetalTrainingProgram])
    REFERENCES [dbo].[GeneralTrainingProgram]
        ([IdGeneralTrainingProgram])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneralTrainingProgramVideo_GeneralTrainingProgram'
CREATE INDEX [IX_FK_GeneralTrainingProgramVideo_GeneralTrainingProgram]
ON [dbo].[GeneralTrainingProgramVideo]
    ([IdGenetalTrainingProgram]);
GO

-- Creating foreign key on [IdLocation] in table 'GeneralTrainingProgramVideo'
ALTER TABLE [dbo].[GeneralTrainingProgramVideo]
ADD CONSTRAINT [FK_GeneralTrainingProgramVideo_Location]
    FOREIGN KEY ([IdLocation])
    REFERENCES [dbo].[Locations]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneralTrainingProgramVideo_Location'
CREATE INDEX [IX_FK_GeneralTrainingProgramVideo_Location]
ON [dbo].[GeneralTrainingProgramVideo]
    ([IdLocation]);
GO

-- Creating foreign key on [IdLocation] in table 'TrainingProgramVideo'
ALTER TABLE [dbo].[TrainingProgramVideo]
ADD CONSTRAINT [FK_TrainingProgramVideo_Location]
    FOREIGN KEY ([IdLocation])
    REFERENCES [dbo].[Locations]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgramVideo_Location'
CREATE INDEX [IX_FK_TrainingProgramVideo_Location]
ON [dbo].[TrainingProgramVideo]
    ([IdLocation]);
GO

-- Creating foreign key on [IdTrainingProgram] in table 'TrainingProgramVideo'
ALTER TABLE [dbo].[TrainingProgramVideo]
ADD CONSTRAINT [FK_TrainingProgramVideo_TrainingProgram]
    FOREIGN KEY ([IdTrainingProgram])
    REFERENCES [dbo].[TrainingProgram]
        ([IdTrainingProgram])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgramVideo_TrainingProgram'
CREATE INDEX [IX_FK_TrainingProgramVideo_TrainingProgram]
ON [dbo].[TrainingProgramVideo]
    ([IdTrainingProgram]);
GO

-- Creating foreign key on [SourceId] in table 'GoalTrackings'
ALTER TABLE [dbo].[GoalTrackings]
ADD CONSTRAINT [FK_GoalTracking_SourceId]
    FOREIGN KEY ([SourceId])
    REFERENCES [dbo].[Sources]
        ([SourceId])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GoalTracking_SourceId'
CREATE INDEX [IX_FK_GoalTracking_SourceId]
ON [dbo].[GoalTrackings]
    ([SourceId]);
GO

-- Creating foreign key on [AreaId] in table 'Suggestions'
ALTER TABLE [dbo].[Suggestions]
ADD CONSTRAINT [FK_Suggestions_Area]
    FOREIGN KEY ([AreaId])
    REFERENCES [dbo].[Area]
        ([AreaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Suggestions_Area'
CREATE INDEX [IX_FK_Suggestions_Area]
ON [dbo].[Suggestions]
    ([AreaId]);
GO

-- Creating foreign key on [IdGeneralTrainingProgram] in table 'GeneralTrainingProgramVisits'
ALTER TABLE [dbo].[GeneralTrainingProgramVisits]
ADD CONSTRAINT [FK_GeneralTrainingProgramVisits_GeneralTrainingProgram]
    FOREIGN KEY ([IdGeneralTrainingProgram])
    REFERENCES [dbo].[GeneralTrainingProgram]
        ([IdGeneralTrainingProgram])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneralTrainingProgramVisits_GeneralTrainingProgram'
CREATE INDEX [IX_FK_GeneralTrainingProgramVisits_GeneralTrainingProgram]
ON [dbo].[GeneralTrainingProgramVisits]
    ([IdGeneralTrainingProgram]);
GO

-- Creating foreign key on [ResourceId] in table 'GeneralTrainingProgramVisits'
ALTER TABLE [dbo].[GeneralTrainingProgramVisits]
ADD CONSTRAINT [FK_GeneralTrainingProgramVisits_Resource]
    FOREIGN KEY ([ResourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneralTrainingProgramVisits_Resource'
CREATE INDEX [IX_FK_GeneralTrainingProgramVisits_Resource]
ON [dbo].[GeneralTrainingProgramVisits]
    ([ResourceId]);
GO

-- Creating foreign key on [ResourceId] in table 'TrainingProgramOnDemandVisits'
ALTER TABLE [dbo].[TrainingProgramOnDemandVisits]
ADD CONSTRAINT [FK_TrainingProgramOnDemandVisits_Resource]
    FOREIGN KEY ([ResourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgramOnDemandVisits_Resource'
CREATE INDEX [IX_FK_TrainingProgramOnDemandVisits_Resource]
ON [dbo].[TrainingProgramOnDemandVisits]
    ([ResourceId]);
GO

-- Creating foreign key on [ResourceId] in table 'TrainingProgramVisits'
ALTER TABLE [dbo].[TrainingProgramVisits]
ADD CONSTRAINT [FK_TrainingProgramVisits_Resource]
    FOREIGN KEY ([ResourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgramVisits_Resource'
CREATE INDEX [IX_FK_TrainingProgramVisits_Resource]
ON [dbo].[TrainingProgramVisits]
    ([ResourceId]);
GO

-- Creating foreign key on [IdTrainingProgram] in table 'TrainingProgramVisits'
ALTER TABLE [dbo].[TrainingProgramVisits]
ADD CONSTRAINT [FK_TrainingProgramVisits_TrainingProgram]
    FOREIGN KEY ([IdTrainingProgram])
    REFERENCES [dbo].[TrainingProgram]
        ([IdTrainingProgram])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgramVisits_TrainingProgram'
CREATE INDEX [IX_FK_TrainingProgramVisits_TrainingProgram]
ON [dbo].[TrainingProgramVisits]
    ([IdTrainingProgram]);
GO

-- Creating foreign key on [IdTrainingProgramOnDemand] in table 'TrainingProgramOnDemandVisits'
ALTER TABLE [dbo].[TrainingProgramOnDemandVisits]
ADD CONSTRAINT [FK_TrainingProgramOnDemandVisits_TrainingProgramOnDemand]
    FOREIGN KEY ([IdTrainingProgramOnDemand])
    REFERENCES [dbo].[TrainingProgramOnDemand]
        ([IdTrainingProgramOnDemand])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingProgramOnDemandVisits_TrainingProgramOnDemand'
CREATE INDEX [IX_FK_TrainingProgramOnDemandVisits_TrainingProgramOnDemand]
ON [dbo].[TrainingProgramOnDemandVisits]
    ([IdTrainingProgramOnDemand]);
GO

-- Creating foreign key on [resourceId] in table 'TDURedeem'
ALTER TABLE [dbo].[TDURedeem]
ADD CONSTRAINT [FK_TDURedeem_ResourceId]
    FOREIGN KEY ([resourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TDURedeem_ResourceId'
CREATE INDEX [IX_FK_TDURedeem_ResourceId]
ON [dbo].[TDURedeem]
    ([resourceId]);
GO

-- Creating foreign key on [resourceId] in table 'TDUReward'
ALTER TABLE [dbo].[TDUReward]
ADD CONSTRAINT [FK_TDUReward_ResourceId]
    FOREIGN KEY ([resourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TDUReward_ResourceId'
CREATE INDEX [IX_FK_TDUReward_ResourceId]
ON [dbo].[TDUReward]
    ([resourceId]);
GO

-- Creating foreign key on [TDUReward] in table 'TDURedeem'
ALTER TABLE [dbo].[TDURedeem]
ADD CONSTRAINT [FK_TDURedeem_TDURewardId]
    FOREIGN KEY ([TDUReward])
    REFERENCES [dbo].[TDUReward]
        ([TDURewardId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TDURedeem_TDURewardId'
CREATE INDEX [IX_FK_TDURedeem_TDURewardId]
ON [dbo].[TDURedeem]
    ([TDUReward]);
GO

-- Creating foreign key on [QuestionId] in table 'QuestionResponse'
ALTER TABLE [dbo].[QuestionResponse]
ADD CONSTRAINT [FK_QuestionResponse_Question]
    FOREIGN KEY ([QuestionId])
    REFERENCES [dbo].[Question]
        ([QuestionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_QuestionResponse_Question'
CREATE INDEX [IX_FK_QuestionResponse_Question]
ON [dbo].[QuestionResponse]
    ([QuestionId]);
GO

-- Creating foreign key on [ResponseId] in table 'QuestionResponse'
ALTER TABLE [dbo].[QuestionResponse]
ADD CONSTRAINT [FK_QuestionResponse_Response]
    FOREIGN KEY ([ResponseId])
    REFERENCES [dbo].[Response]
        ([ResponseId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_QuestionResponse_Response'
CREATE INDEX [IX_FK_QuestionResponse_Response]
ON [dbo].[QuestionResponse]
    ([ResponseId]);
GO

-- Creating foreign key on [ResourceId] in table 'SurveyResource'
ALTER TABLE [dbo].[SurveyResource]
ADD CONSTRAINT [FK_SurveyResource_Resource]
    FOREIGN KEY ([ResourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SurveyResource_Resource'
CREATE INDEX [IX_FK_SurveyResource_Resource]
ON [dbo].[SurveyResource]
    ([ResourceId]);
GO

-- Creating foreign key on [SurveyId] in table 'Question'
ALTER TABLE [dbo].[Question]
ADD CONSTRAINT [FK_Question_Survey]
    FOREIGN KEY ([SurveyId])
    REFERENCES [dbo].[Survey]
        ([SurveyId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Question_Survey'
CREATE INDEX [IX_FK_Question_Survey]
ON [dbo].[Question]
    ([SurveyId]);
GO

-- Creating foreign key on [QuestionId] in table 'SurveyResponse'
ALTER TABLE [dbo].[SurveyResponse]
ADD CONSTRAINT [FK_SurveyResponse_Question]
    FOREIGN KEY ([QuestionId])
    REFERENCES [dbo].[Question]
        ([QuestionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SurveyResponse_Question'
CREATE INDEX [IX_FK_SurveyResponse_Question]
ON [dbo].[SurveyResponse]
    ([QuestionId]);
GO

-- Creating foreign key on [ResourceId] in table 'SurveyResponse'
ALTER TABLE [dbo].[SurveyResponse]
ADD CONSTRAINT [FK_SurveyResponse_Resource]
    FOREIGN KEY ([ResourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SurveyResponse_Resource'
CREATE INDEX [IX_FK_SurveyResponse_Resource]
ON [dbo].[SurveyResponse]
    ([ResourceId]);
GO

-- Creating foreign key on [SurveyId] in table 'SurveyResource'
ALTER TABLE [dbo].[SurveyResource]
ADD CONSTRAINT [FK_SurveyResource_Survey]
    FOREIGN KEY ([SurveyId])
    REFERENCES [dbo].[Survey]
        ([SurveyId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SurveyResource_Survey'
CREATE INDEX [IX_FK_SurveyResource_Survey]
ON [dbo].[SurveyResource]
    ([SurveyId]);
GO

-- Creating foreign key on [SurveyResourceId] in table 'SurveyResponse'
ALTER TABLE [dbo].[SurveyResponse]
ADD CONSTRAINT [FK_SurveyResponse_SurveyResource]
    FOREIGN KEY ([SurveyResourceId])
    REFERENCES [dbo].[SurveyResource]
        ([SurveyResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SurveyResponse_SurveyResource'
CREATE INDEX [IX_FK_SurveyResponse_SurveyResource]
ON [dbo].[SurveyResponse]
    ([SurveyResourceId]);
GO

-- Creating foreign key on [OnePagePlanId] in table 'KeyThrusts'
ALTER TABLE [dbo].[KeyThrusts]
ADD CONSTRAINT [FK_OnePagePlanId]
    FOREIGN KEY ([OnePagePlanId])
    REFERENCES [dbo].[OnePagePlan]
        ([OnePagePlanId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OnePagePlanId'
CREATE INDEX [IX_FK_OnePagePlanId]
ON [dbo].[KeyThrusts]
    ([OnePagePlanId]);
GO

-- Creating foreign key on [ResourceId] in table 'OnePagePlan'
ALTER TABLE [dbo].[OnePagePlan]
ADD CONSTRAINT [FK__Resource__ResourceId]
    FOREIGN KEY ([ResourceId])
    REFERENCES [dbo].[Resources]
        ([ResourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK__Resource__ResourceId'
CREATE INDEX [IX_FK__Resource__ResourceId]
ON [dbo].[OnePagePlan]
    ([ResourceId]);
GO

-- Creating foreign key on [OnePagePlanId] in table 'AnnualPriorities'
ALTER TABLE [dbo].[AnnualPriorities]
ADD CONSTRAINT [FK_AnnualPriorities_OnePagePlanId]
    FOREIGN KEY ([OnePagePlanId])
    REFERENCES [dbo].[OnePagePlan]
        ([OnePagePlanId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AnnualPriorities_OnePagePlanId'
CREATE INDEX [IX_FK_AnnualPriorities_OnePagePlanId]
ON [dbo].[AnnualPriorities]
    ([OnePagePlanId]);
GO

-- Creating foreign key on [QuarterlyPrioritiesId] in table 'KPI'
ALTER TABLE [dbo].[KPI]
ADD CONSTRAINT [FK_KPI_QuarterlyPriorities]
    FOREIGN KEY ([QuarterlyPrioritiesId])
    REFERENCES [dbo].[QuarterlyPriorities]
        ([QuarterlyPrioritiesId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_KPI_QuarterlyPriorities'
CREATE INDEX [IX_FK_KPI_QuarterlyPriorities]
ON [dbo].[KPI]
    ([QuarterlyPrioritiesId]);
GO

-- Creating foreign key on [OnePagePlanId] in table 'QuarterlyPriorities'
ALTER TABLE [dbo].[QuarterlyPriorities]
ADD CONSTRAINT [FK_QuarterlyPriorities_OnePagePlan]
    FOREIGN KEY ([OnePagePlanId])
    REFERENCES [dbo].[OnePagePlan]
        ([OnePagePlanId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_QuarterlyPriorities_OnePagePlan'
CREATE INDEX [IX_FK_QuarterlyPriorities_OnePagePlan]
ON [dbo].[QuarterlyPriorities]
    ([OnePagePlanId]);
GO

-- Creating foreign key on [QuarterlyPrioritiesId] in table 'PersonalDevelopment'
ALTER TABLE [dbo].[PersonalDevelopment]
ADD CONSTRAINT [FK_PersonalDevelopment_QuarterlyPriorities]
    FOREIGN KEY ([QuarterlyPrioritiesId])
    REFERENCES [dbo].[QuarterlyPriorities]
        ([QuarterlyPrioritiesId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PersonalDevelopment_QuarterlyPriorities'
CREATE INDEX [IX_FK_PersonalDevelopment_QuarterlyPriorities]
ON [dbo].[PersonalDevelopment]
    ([QuarterlyPrioritiesId]);
GO

-- Creating foreign key on [QuarterlyPrioritiesId] in table 'QuarterlyActions'
ALTER TABLE [dbo].[QuarterlyActions]
ADD CONSTRAINT [FK_QuarterlyActions_QuarterlyPriorities]
    FOREIGN KEY ([QuarterlyPrioritiesId])
    REFERENCES [dbo].[QuarterlyPriorities]
        ([QuarterlyPrioritiesId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_QuarterlyActions_QuarterlyPriorities'
CREATE INDEX [IX_FK_QuarterlyActions_QuarterlyPriorities]
ON [dbo].[QuarterlyActions]
    ([QuarterlyPrioritiesId]);
GO

-- Creating foreign key on [QuarterlyPrioritiesId] in table 'ValuesInfusion'
ALTER TABLE [dbo].[ValuesInfusion]
ADD CONSTRAINT [FK_ValuesInfusion_QuarterlyPriorities]
    FOREIGN KEY ([QuarterlyPrioritiesId])
    REFERENCES [dbo].[QuarterlyPriorities]
        ([QuarterlyPrioritiesId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ValuesInfusion_QuarterlyPriorities'
CREATE INDEX [IX_FK_ValuesInfusion_QuarterlyPriorities]
ON [dbo].[ValuesInfusion]
    ([QuarterlyPrioritiesId]);
GO

-- Creating foreign key on [GroupId] in table 'Group_SectionAccess'
ALTER TABLE [dbo].[Group_SectionAccess]
ADD CONSTRAINT [FK_Group_SectionAccess_Groups]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[Groups]
        ([GroupId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Group_SectionAccess_Groups'
CREATE INDEX [IX_FK_Group_SectionAccess_Groups]
ON [dbo].[Group_SectionAccess]
    ([GroupId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------