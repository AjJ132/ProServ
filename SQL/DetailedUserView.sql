CREATE VIEW DetailedUserView AS
SELECT 
    UI.*,
    AU.Email,
    UR.RoleId
FROM 
    [dbo].[UserInformation] UI
LEFT JOIN 
    [dbo].[AspNetUsers] AU 
ON 
    UI.UserId = AU.Id
LEFT JOIN 
    [dbo].[AspNetUserRoles] UR 
ON 
    UI.UserId = UR.UserId;
