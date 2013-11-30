ALTER TABLE [dbo].[Article] ADD [Published] [bit] DEFAULT 0 NOT NULL
UPDATE Article SET Published=1 