------------------------------- Schema Change script ------------------------------------
--
-- Title:          Add password reset tokens table
-- Description:    Add password reset tokens table
-- Schema version: 01.00.006

------------------------------ script code ----------------------------------------------
CREATE TABLE [dbo].[PasswordResetTokens] (
    [ID]                int           IDENTITY(1,1) NOT NULL,
    [HashedToken]       nvarchar(255) NOT NULL,
    [UserID]            int	           NOT NULL,
    [CreatedAt]         DATETIME       DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_PasswordResetTokens] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PasswordResetTokens_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID])
);