-- Verbs Table
CREATE TABLE [dbo].[Verbs] (
	[Id]                INT            IDENTITY (1, 1) NOT NULL,
	[Description]       NVARCHAR (MAX) NOT NULL,
	[Infinative]        NVARCHAR (25)  NOT NULL,
	[EnglishInfinative] NVARCHAR (40)  NOT NULL,
	PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [AK_Verbs_Infinative] UNIQUE NONCLUSTERED ([Infinative] ASC)
);


-- ConjugationRules Table
CREATE TABLE [dbo].[ConjugationRules] (
	[Id]          INT            IDENTITY (1, 1) NOT NULL,
	[TenseId]     INT            NOT NULL,
	[Name]        NVARCHAR (50)  NOT NULL,
	[Description] NVARCHAR (MAX) NOT NULL,
	[Type]        INT            NOT NULL,
	[IsRegular]   INT            NOT NULL,
	PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [AK_ConjugationRules_name] UNIQUE NONCLUSTERED ([Name] ASC)
);


 -- Persons Table
--CREATE TABLE [dbo].[Persons]
--(
--	[Id] INT IDENTITY NOT NULL PRIMARY KEY,
--	[SpanishExpression] NVARCHAR(15) NOT NULL,
--	[Description] NVARCHAR(MAX) NOT NULL,
--	[Plurality] INT NOT NULL,
--	[Formality] INT NOT NULL,
--	[Gender] INT NOT NULL,
--	[Order] INT NOT NULL
--);

-- Tenses Table
CREATE TABLE [dbo].[Tenses] (
	[Id]                   INT            IDENTITY (1, 1) NOT NULL,
	[Name]                 NVARCHAR (30)  NOT NULL,
	[Description]          NVARCHAR (MAX) NOT NULL,
	[Time]                 INT            NULL,
	[RegularConjugationRuleId] INT		  NOT NULL,
	[PersonsCount]         INT            NULL,
	PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [AK_Tenses_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



-- ConjugationRulesInstructions Table
CREATE TABLE [dbo].[ConjugationRulesInstructions] (
	[Id]                INT           IDENTITY (1, 1) NOT NULL,
	[ConjugationRuleId] INT           NOT NULL,
	[PersonId]          INT           NOT NULL,
	[VerbType]          INT           NOT NULL,
	[Suffix]            NVARCHAR (15) NULL,
	PRIMARY KEY CLUSTERED ([Id] ASC), 
	CONSTRAINT [AK_ConjugationRulesInstructions_Clustered] UNIQUE ([ConjugationRuleId],[PersonId],[VerbType])
);



-- VerbsConjugationRules Table
CREATE TABLE [dbo].[VerbsConjugationRules]
(
	[Id] INT IDENTITY NOT NULL PRIMARY KEY,	
	[VerbId] INT NOT NULL,
	[ConjugationRuleId] INT NOT NULL,
	[ConjugationData] NVARCHAR (MAX)
);

-- ConjugationRulePersons
CREATE TABLE [dbo].[ConjugationRulePersons]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ConjugationRuleId] INT NOT NULL,
	[PersonId] INT NOT NULL, 
    CONSTRAINT [AK_ConjugationRulePersons_Complex] UNIQUE ([ConjugationRuleId], [PersonId])
);