
-- Create FacebookUserIds table
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FacebookUserIds](
  [UserId] [uniqueidentifier] NOT NULL,
  [FbUid][nvarchar](64) COLLATE SQL_Latin1_General_CP1_CI_AS,
  CONSTRAINT [PK_FacebookUserIds] PRIMARY KEY NONCLUSTERED 
  (
	[UserId] ASC
  ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

ALTER TABLE [dbo].[FacebookUserIds]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FacebookUserIds]  WITH CHECK ADD CONSTRAINT [FK_FacebookUserIds_aspnet_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO
ALTER TABLE [dbo].[FacebookUserIds] CHECK CONSTRAINT [FK_FacebookUserIds_aspnet_Users]
GO
