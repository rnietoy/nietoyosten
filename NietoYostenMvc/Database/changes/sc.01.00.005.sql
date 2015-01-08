------------------------------- Schema Change script ------------------------------------
--
-- Title:          Add clustered index to aspnet_Membership
-- Description:    Add clustered index to aspnet_Membership
-- Schema version: 01.00.005

------------------------------ script code ----------------------------------------------
ALTER TABLE [aspnet_Membership] ADD CONSTRAINT [PK_aspnet_Membership] PRIMARY KEY CLUSTERED ([UserId] ASC)