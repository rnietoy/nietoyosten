------------------------------- Schema Change script ------------------------------------
--
-- Title:          Convert Users.FacebookUserID from int to long
-- Description:    Convert Users.FacebookUserID from int to long
-- Schema version: 01.00.008

ALTER TABLE [Users] ALTER COLUMN [FacebookUserID] bigint
