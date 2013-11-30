-- Users
SELECT [ApplicationId],[UserId],[UserName],[LoweredUserName],[MobileAlias],[IsAnonymous],[LastActivityDate] from [nietoyosten].[dbo].[aspnet_Users]

-- Membership
SELECT [ApplicationId],[UserId],[Password],[PasswordFormat],[PasswordSalt],[MobilePIN],[Email],[LoweredEmail],[PasswordQuestion],[PasswordAnswer],[IsApproved],[IsLockedOut],[CreateDate],[LastLoginDate],[LastPasswordChangedDate],[LastLockoutDate],[FailedPasswordAttemptCount],[FailedPasswordAttemptWindowStart],[FailedPasswordAnswerAttemptCount],[FailedPasswordAnswerAttemptWindowStart],[Comment] from [nietoyosten].[dbo].[aspnet_Membership]

-- Query users from DB:
SELECT u.UserName, u.LastActivityDate, m.Email, m.IsApproved, m.LastLoginDate, r.RoleName
  from aspnet_Users u inner join aspnet_Membership m on m.UserId = u.UserId
  left join aspnet_UsersInRoles ur on u.UserId = ur.UserId
  left join aspnet_Roles r on r.RoleId = ur.RoleId

-- Roles
SELECT [ApplicationId],[RoleId],[RoleName],[LoweredRoleName],[Description] from [nietoyosten].[dbo].[aspnet_Roles]

-- Users in roles
SELECT [UserId],[RoleId] from [nietoyosten].[dbo].[aspnet_UsersInRoles]

-- Get UserId
select UserId from aspnet_Users where UserName='rafa'

-- Get RoleId
SELECT RoleId FROM aspnet_Roles WHERE RoleName='admin'

-- Select roles of one user
SELECT RoleId from aspnet_UsersInRoles where UserId=(select UserId from aspnet_Users where UserName='rafa')

-- Add role to user
INSERT INTO aspnet_UsersInRoles (UserId, RoleId)
VALUES ((select UserId from aspnet_Users where UserName='papi'), (SELECT RoleId FROM aspnet_Roles WHERE RoleName='admin'))


