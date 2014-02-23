ALTER TABLE Albums ADD [CreatedAt] DATETIME DEFAULT (getdate()) NOT NULL
ALTER TABLE Albums ADD [ModifiedAt] DATETIME DEFAULT (getdate()) NOT NULL

-- Set default value of CreatedBy and ModifiedBy to 10,
-- which is the user ID of rnietoy@gmail.com in the production database.
ALTER TABLE [Albums] ADD
  [CreatedBy] int
  CONSTRAINT [DF_Albums_CreatedBy] DEFAULT 10 NOT NULL,
  CONSTRAINT [FK_Albums_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([ID])

ALTER TABLE [Albums] ADD
  [ModifiedBy] int
  CONSTRAINT [DF_Albums_ModifiedBy] DEFAULT 10 NOT NULL,
  CONSTRAINT [FK_Albums_Users_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Users] ([ID])

ALTER TABLE [Albums] DROP CONSTRAINT [DF_Albums_CreatedBy]
ALTER TABLE [Albums] DROP CONSTRAINT [DF_Albums_ModifiedBy]