CREATE TABLE TeamPackages
(
    PackageID int PRIMARY KEY IDENTITY(1,1),
    PackageName VARCHAR(100),
    PackageDescription varchar(max),
    PackagePrice MONEY,
    PackageMaxMembers INT,
    PackageMaxAssistantCoaches INT,
    IsPublic BIT
);


INSERT INTO TeamPackages (PackageName, PackageDescription, PackagePrice, PackageMaxMembers, PackageMaxAssistantCoaches, IsPublic)
VALUES ('Pro Pacakge', 'Our most premium package with all the benefits. Great for full time coaches and organizations', 40, 100, 3, 1);

INSERT INTO TeamPackages (PackageName, PackageDescription, PackagePrice, PackageMaxMembers, PackageMaxAssistantCoaches, IsPublic)
VALUES ('Starter Package', 'For small teams and great for coaches starting their buisness. Does allow a trial!', 20, 10, 1, 1);

INSERT INTO TeamPackages (PackageName, PackageDescription, PackagePrice, PackageMaxMembers, PackageMaxAssistantCoaches, IsPublic)
VALUES ('Sarah Custom Package', 'Sarah Hendrick - PacePerfect - Custom', 20, 100, 5, 0);
