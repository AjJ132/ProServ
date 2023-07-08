CREATE VIEW View_TeamDetails AS
SELECT 
    UI.UserId,
    CONCAT(UI.FirstName, ' ', UI.LastName) AS FullName,
    
    TP.PackageID,
    TP.PackageStart,
    TP.PackageEnd,
    TI.DateCreated AS TeamInfoDateCreated,
    TI.OwnerID AS TeamInfoOwnerID,
    TI.IsSchoolOrganization,
    TI.TeamPackageID,
    TI.TimeChanged,
    TI.TeamSport,
    T.TeamName,
    T.Location,
    T.CoachesCode,
    T.UsersCode,
    T.Terminated,
    T.OwnerID AS TeamOwnerID
FROM 
    UserInformation AS UI
JOIN 
    Teams AS T ON UI.TeamID = T.TeamID
JOIN 
    TeamInfo AS TI ON T.TeamID = TI.TeamID
JOIN 
    TeamPackage AS TP ON T.TeamID = TP.TeamID;
