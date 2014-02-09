UPDATE Pictures
SET Title = LEFT(Title, CHARINDEX('.', Title) - 1)
WHERE CHARINDEX('.', Title) <> 0
