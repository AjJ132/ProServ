CREATE TABLE Teams
(
    TeamID INT PRIMARY KEY IDENTITY(1,1),
    TeamName VARCHAR(100),
    Location VARCHAR(100),
    CoachesCode VARCHAR(10),
    UsersCode VARCHAR(5),
    Terminated bit
);

CREATE TABLE TeamInfo
(
    TeamID INT PRIMARY KEY,
    DateCreated DATETIME,
    OwnerID VARCHAR(450),
    IsSchoolOrganization BIT,
    TeamPackageID INT,
    TimeChanged INT,
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID)
);

CREATE TABLE TeamPackage
(
    PackageID INT PRIMARY KEY,
    TeamID INT,
    PackageStart DATETIME,
    PackageEnd DATETIME,
    FOREIGN KEY (TeamID) REFERENCES Teams(TeamID)
);
