-- Create Albums table
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Albums](
  [Id] [int] IDENTITY(1,1) NOT NULL,
  [Title] [nvarchar](64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  [FolderName] [nvarchar](64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  CONSTRAINT [PK_Albums] PRIMARY KEY CLUSTERED 
  (
	  [Id] ASC
  ) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

-- Create Pictures table
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pictures](
  [Id] [int] IDENTITY(1,1) NOT NULL,
  [AlbumId][int] NOT NULL,
  [Title] [nvarchar](64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  [FileName] [nvarchar](64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  CONSTRAINT [PK_Pictures] PRIMARY KEY CLUSTERED 
  (
    [Id] ASC
  ) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

ALTER TABLE [dbo].[Pictures]  WITH CHECK ADD CONSTRAINT [FK_Pictures_Albums] FOREIGN KEY([AlbumId])
REFERENCES [dbo].[Albums] ([Id])
GO
ALTER TABLE [dbo].[Pictures] CHECK CONSTRAINT [FK_Pictures_Albums]
GO