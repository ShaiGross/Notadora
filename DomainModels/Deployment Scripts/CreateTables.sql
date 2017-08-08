

CREATE TABLE [dbo].[VerbsConjugationRules] (

    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [VerbId]            INT            NOT NULL,
    [ConjugationRuleId] INT            NOT NULL,
    [ConjugationData]   NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[ConjugationRules] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [TenseId]      INT            NOT NULL,
    [Name]         NVARCHAR (50)  NOT NULL,
    [Description]  NVARCHAR (MAX) NOT NULL,
    [Type]         INT            NOT NULL,
    [IsRegular]    BIT            NOT NULL,
    [PersonCount]  INT            DEFAULT ((-1)) NOT NULL,
    [PatternIndex] INT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [AK_ConjugationRules_name] UNIQUE NONCLUSTERED ([Name] ASC)
);

CREATE TABLE [dbo].[Verbs] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Description]       NVARCHAR (MAX) NOT NULL,
    [Infinative]        NVARCHAR (25)  NOT NULL,
    [EnglishInfinative] NVARCHAR (40)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [AK_Verbs_Infinative] UNIQUE NONCLUSTERED ([Infinative] ASC)
);

CREATE TABLE [dbo].[Tenses] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [Name]                     NVARCHAR (30)  NOT NULL,
    [Description]              NVARCHAR (MAX) NOT NULL,
    [Time]                     INT            NULL,
    [RugularConjugationRuleId] INT            NOT NULL,
    [PersonsCount]             INT            NULL,
	[Enabled]				   BIT			  NOT NULL Default 0
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [AK_Tenses_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

CREATE TABLE [dbo].[Persons] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [SpanishExpression] NVARCHAR (15)  NULL,
    [Description]       NVARCHAR (MAX) NOT NULL,
    [Plurality]         INT            NOT NULL,
    [Formality]         INT            NOT NULL,
    [Gender]            INT            NOT NULL,
    [Order]             INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[ConjugationRulesInstructions] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [ConjugationRuleId] INT           NOT NULL,
    [PersonId]          INT           NOT NULL,
    [VerbType]          INT           NOT NULL,
    [Suffix]            NVARCHAR (15) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [AK_ConjugationRulesInstructions_Clustered] UNIQUE NONCLUSTERED ([ConjugationRuleId] ASC, [PersonId] ASC, [VerbType] ASC)
);

CREATE TABLE [dbo].[ConjugationRulePersons] (
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    [ConjugationRuleId] INT NOT NULL,
    [PersonId]          INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [AK_ConjugationRulePersons_Complex] UNIQUE NONCLUSTERED ([ConjugationRuleId] ASC, [PersonId] ASC)
);

CREATE TABLE [dbo].[ConjugationMatches] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [VerbId]            INT           NOT NULL,
    [ConjugationRuleId] INT           NOT NULL,
    [PersonId]          INT           NULL,
    [ConjugationString] NVARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[TensePersons]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[TenseId] INT NOT NULL,
	[PersonId] INT NOT NULL
);