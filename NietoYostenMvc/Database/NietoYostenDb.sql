-----------------------------------------------------------------------------------------
--                   DB schema for nietoyosten.com MVC version                        --
-----------------------------------------------------------------------------------------

----------------------------------------- Users -----------------------------------------
CREATE TABLE [dbo].[Users] (
    [ID]             INT            IDENTITY (1, 1) NOT NULL,
    [Email]          NVARCHAR (255) NOT NULL,
    [HashedPassword] NVARCHAR (255) NULL,
    [LastLogin]      DATETIME       NULL,
    [CreatedAt]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [IsApproved]     BIT            DEFAULT (0) NOT NULL,
    [Role]           NVARCHAR (16)  DEFAULT ('friend') NOT NULL,
    [FacebookUserID] INT            NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED ([Email] ASC)
);

----------------------------------------- Sections ---------------------------------------
CREATE TABLE [dbo].[Sections] (
	[ID]                int          IDENTITY(1,1) NOT NULL,
	[Name]              nvarchar(64) NOT NULL,
	[ParentSectionID]   int          NULL,
    CONSTRAINT [PK_Sections] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Sections_Sections] FOREIGN KEY ([ParentSectionID]) REFERENCES [dbo].[Sections] ([ID])
);

----------------------------------------- Articles ---------------------------------------
CREATE TABLE [dbo].[Articles] (
	[ID]    		    int 				    IDENTITY(1,1) NOT NULL,
	[Title] 			  nvarchar(100)		NOT NULL,
	[IntroText] 		nvarchar(max)   NULL,
	[Content] 			nvarchar(max)   NULL,
	[SectionID] 		int 			      NOT NULL,
	[CreatedBy] 		int					    NOT NULL,
	[ModifiedBy] 		int					    NOT NULL,
	[CreatedAt] 		DATETIME 			  DEFAULT (getdate()) NOT NULL,
	[UpdatedAt] 		datetime        DEFAULT (getdate()) NOT NULL,
	[IsPublished] 	bit 				    DEFAULT (0) NOT NULL,
  [IsPublic]      bit             DEFAULT (0) NOT NULL,
    CONSTRAINT [PK_Articles] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Articles_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_Articles_Users_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_Articles_Sections] FOREIGN KEY ([SectionID]) REFERENCES [dbo].[Sections] ([ID])
);

----------------------------------------- HomePageArticles ---------------------------------------
CREATE TABLE [dbo].[HomePageArticles] (
  [ID]            int         IDENTITY(1,1) NOT NULL,
  [Position]      INT         NOT NULL,
  [Enabled]       BIT         DEFAULT(0) NOT NULL,
  [ArticleID]     INT         NOT NULL,
  CONSTRAINT [PK_HomePageArticles] PRIMARY KEY CLUSTERED ([ID] ASC),
  CONSTRAINT [FK_HomePageArticles_Articles] FOREIGN KEY ([ArticleID]) REFERENCES [dbo].[Articles] ([ID])
);

----------------------------------------- WeblinkCategories ---------------------------------------
CREATE TABLE [dbo].[WeblinkCategories] (
	[ID]   int             IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(50)    NOT NULL,
	CONSTRAINT [PK_WeblinkCategories] PRIMARY KEY CLUSTERED ([ID] ASC)
);

----------------------------------------- Weblinks ---------------------------------------
CREATE TABLE [dbo].[Weblinks] (
	[ID]           int             IDENTITY(1,1) NOT NULL,
	[Title]        nvarchar(100)   NULL,
	[Url]          varchar(256)    NOT NULL,
	[Description]  nvarchar(255)   NULL,
	[CategoryID]   int             NOT NULL,
	CONSTRAINT [PK_Weblinks] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_Weblinks_WeblinkCategories] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[WeblinkCategories] ([ID])
);

--------------------------------------- Albums ---------------------------------------------------
CREATE TABLE [dbo].[Albums] (
  [ID]          int             IDENTITY(1,1) NOT NULL,
  [Title]       nvarchar(64)    NOT NULL,
  [FolderName]  nvarchar(64)    NOT NULL,
  CONSTRAINT [PK_Albums] PRIMARY KEY CLUSTERED ([ID] ASC)
);

--------------------------------------- Pictures -------------------------------------------------
CREATE TABLE [dbo].[Pictures] (
  [ID]          INT           IDENTITY(1,1) NOT NULL,
  [AlbumID]     INT           NOT NULL,
  [Title]       nvarchar(64)  NOT NULL,
  [FileName]    nvarchar(64)  NOT NULL,
  CONSTRAINT [PK_Pictures] PRIMARY KEY CLUSTERED ([ID] ASC),
  CONSTRAINT [FK_Pictures_Albums] FOREIGN KEY ([AlbumID]) REFERENCES [dbo].[Albums] ([ID])
);
