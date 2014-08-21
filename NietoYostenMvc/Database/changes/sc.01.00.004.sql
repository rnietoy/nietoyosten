------------------------------- Schema Change script ------------------------------------
--
-- Title:          Add Family section
-- Description:    Add Family section
-- Schema version: 01.00.004

------------------------------ script code ----------------------------------------------
INSERT INTO [Sections] (Name, ParentSectionID) VALUES ('Family', NULL)