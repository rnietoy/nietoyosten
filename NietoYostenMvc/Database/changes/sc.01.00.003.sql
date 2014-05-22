------------------------------- Schema Change script ------------------------------------
--
-- Title:          Upload pictures
-- Description:    Change Albumns, Pictures tables for upload functionality:
--                 add CreatedAt, CreatedBy, UpdatedAt, and UpdatedBy columns.
-- Schema version: 01.00.003
-- Status:         in development

------------------------------ script code ----------------------------------------------

------------------------------------ Changes to Albums -------------------------------------------------
ALTER TABLE Albums ADD [CreatedAt] DATETIME DEFAULT (getdate()) NOT NULL
ALTER TABLE Albums ADD [ModifiedAt] DATETIME DEFAULT (getdate()) NOT NULL

-- Set default value of CreatedBy and ModifiedBy to 10,
-- which is the user ID of rnietoy@gmail.com in the production database.
-- This is so that we populate these fields when the script is deployed.
-- Afterwards, we drop the constraints, since we don't actually want
-- the column to have a default value.
ALTER TABLE [Albums] ADD
  [CreatedBy] int
  CONSTRAINT [DF_Albums_CreatedBy] DEFAULT 10 NOT NULL,
  CONSTRAINT [FK_Albums_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([ID])

ALTER TABLE [Albums] ADD
  [ModifiedBy] int
  CONSTRAINT [DF_Albums_ModifiedBy] DEFAULT 10 NOT NULL,
  CONSTRAINT [FK_Albums_Users_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Users] ([ID])

-- Drop the default constraints for CreatedBy and ModifiedBy here.
-- Note that we named the constraints above so we could reference them here.
ALTER TABLE [Albums] DROP CONSTRAINT [DF_Albums_CreatedBy]
ALTER TABLE [Albums] DROP CONSTRAINT [DF_Albums_ModifiedBy]

-- Add unique constraint for the FolderName column
ALTER TABLE [Albums] ADD CONSTRAINT [UQ_Albums_FolderName] UNIQUE NONCLUSTERED ([FolderName] ASC)


------------------------------------ Changes to Pictures -------------------------------------------------
ALTER TABLE [Pictures] ADD [UploadedAt] DATETIME DEFAULT (getdate()) NOT NULL

-- Set default value of UploadedBy to 10, which is the user ID of rnietoy@gmail.com in the production database.
-- This is so that we populate these fields when the script is deployed.
-- Afterwards, we drop the constraints, since we don't actually want the column to have a default value.
ALTER TABLE [Pictures] ADD
  [UploadedBy] int
  CONSTRAINT [DF_Pictures_UploadedBy] DEFAULT 10 NOT NULL,
  CONSTRAINT [FK_Pictures_Users_UploadedBy] FOREIGN KEY ([UploadedBy]) REFERENCES [dbo].[Users] ([ID])

-- Drop the default constraints for UploadedBy here.
-- Note that we named the constraints above so we could reference them here.
ALTER TABLE [Pictures] DROP CONSTRAINT [DF_Pictures_UploadedBy]