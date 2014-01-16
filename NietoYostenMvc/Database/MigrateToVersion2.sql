-----------------------------------------------------------------------------------------
--    Migrate data from old NietoYosten DB 1.0 into NietoYosten DB 2.0 (MVC version)   --
-----------------------------------------------------------------------------------------


--------------------------------------------------------------------------------------------
-- Import data

--------------------------------------------------------------------------------------------
-- Users
INSERT INTO Users (Email, LastLogin, CreatedAt, UpdatedAt, IsApproved, Roles, FacebookUserID)
SELECT
  M.Email as Email,
  M.LastLoginDate as LastLogin,
  M.CreateDate as CreatedAt,
  M.LastPasswordChangedDate as UpdatedAt,
  M.IsApproved as IsApproved,
  Roles = CASE
    WHEN 'admin' IN
      (select R.RoleName 
      from [nietoyosten_old].[dbo].[aspnet_Roles] R
      inner join [nietoyosten_old].[dbo].[aspnet_UsersInRoles] UR on UR.RoleId = R.RoleId
      inner join [nietoyosten_old].[dbo].[aspnet_Users] U on U.UserId = UR.UserId
      where U.UserId = M.UserId)
    THEN 'admin'
    WHEN 'family' IN
      (select R.RoleName 
      from [nietoyosten_old].[dbo].[aspnet_Roles] R
      inner join [nietoyosten_old].[dbo].[aspnet_UsersInRoles] UR on UR.RoleId = R.RoleId
      inner join [nietoyosten_old].[dbo].[aspnet_Users] U on U.UserId = UR.UserId
      where U.UserId = M.UserId)
    THEN 'family'
    ELSE 'friend'
  END,
  FacebookUserID = (SELECT FbUid FROM [nietoyosten_old].[dbo].[FacebookUserIds] WHERE UserId = M.UserId)
FROM [nietoyosten_old].[dbo].[aspnet_Membership] M

--------------------------------------------------------------------------------------------
-- Sections
SET IDENTITY_INSERT Sections ON
INSERT INTO Sections (ID, Name, ParentSectionID)
SELECT * FROM [nietoyosten_old].[dbo].[Sections]
SET IDENTITY_INSERT [Sections] OFF

--------------------------------------------------------------------------------------------
-- Articles
SET IDENTITY_INSERT Articles ON
INSERT INTO Articles (ID, Title, IntroText, Content, SectionID, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt, IsPublished)
SELECT
  A.ArticleId,
  A.Title, 
  A.IntroText, 
  A.Content, 
  A.SectionId,
  CreatedBy = (SELECT [ID] FROM [Users] WHERE [Email] = (SELECT Email FROM [nietoyosten_old].[dbo].[aspnet_Membership] WHERE UserId = A.CreatedBy)),
  ModifiedBy = COALESCE (
    (SELECT [ID] FROM [Users] WHERE [Email] = (SELECT Email FROM [nietoyosten_old].[dbo].[aspnet_Membership] WHERE UserId = A.ModifiedBy)),
	(SELECT [ID] FROM [Users] WHERE [Email] = (SELECT Email FROM [nietoyosten_old].[dbo].[aspnet_Membership] WHERE UserId = A.CreatedBy))
	),
  CreatedAt = A.DateCreated,
  UpdatedAt = COALESCE (A.DateModified, A.DateCreated),
  IsPublished = A.Published
FROM [nietoyosten_old].[dbo].[Article] A
SET IDENTITY_INSERT Articles OFF

--------------------------------------------------------------------------------------------
-- HomePageArticles
SET IDENTITY_INSERT HomePageArticles ON
INSERT INTO HomePageArticles (ID, Position, Enabled, ArticleID)
SELECT * FROM [nietoyosten_old].[dbo].[HomePageArticles]
SET IDENTITY_INSERT HomePageArticles OFF

--------------------------------------------------------------------------------------------
-- WeblinkCategories
SET IDENTITY_INSERT WeblinkCategories ON
INSERT INTO WeblinkCategories (ID, Name)
SELECT * FROM [nietoyosten_old].[dbo].[WeblinkCategories]
SET IDENTITY_INSERT WeblinkCategories OFF

--------------------------------------------------------------------------------------------
-- Weblinks
SET IDENTITY_INSERT Weblinks ON
INSERT INTO Weblinks (ID, Title, Url, Description, CategoryID)
SELECT * FROM [nietoyosten_old].[dbo].[Weblinks]
SET IDENTITY_INSERT Weblinks OFF

--------------------------------------------------------------------------------------------
-- Albums
SET IDENTITY_INSERT Albums ON
INSERT INTO Albums (ID, Title, FolderName)
SELECT * FROM [nietoyosten_old].[dbo].[Albums]
SET IDENTITY_INSERT Albums OFF

--------------------------------------------------------------------------------------------
-- Pictures
SET IDENTITY_INSERT Pictures ON
INSERT INTO Pictures (ID, AlbumID, Title, FileName)
SELECT * FROM [nietoyosten_old].[dbo].[Pictures]
SET IDENTITY_INSERT Pictures OFF

--------------------------------------------------------------------------------------------
-- aspnet_Membership

-- Copy aspnet_Membership table into new DB. This table is temporary. We need the table to
-- authenticate users. When users authenticate we will populate Users.HashedPassword.
-- Once all users have their HashedPassword column set, we can drop this table.
SELECT * INTO aspnet_Membership FROM [nietoyosten_old].[dbo].[aspnet_Membership]
