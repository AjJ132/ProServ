CREATE VIEW UserView AS
SELECT 
    UI.FirstName,
    UI.LastName,
    UI.UserType,
    AU.Email,
    UR.RoleId
FROM 
    [dbo].[UserInformation] UI
JOIN 
    [dbo].[AspNetUsers] AU 
ON 
    UI.UserId = AU.Id
JOIN 
    [dbo].[AspNetUserRoles] UR 
ON 
    UI.UserId = UR.UserId;
