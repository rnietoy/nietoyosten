------------------------------- Schema Change script ------------------------------------
--
-- Title:          remove_dots_from_picture_titles
-- Description:    Remove's dots from picture titles, as they cause problems in URLs.
-- Schema version: 01.00.001
-- Status:         deployed

------------------------------ script code ----------------------------------------------
UPDATE Pictures
SET Title = LEFT(Title, CHARINDEX('.', Title) - 1)
WHERE CHARINDEX('.', Title) <> 0
