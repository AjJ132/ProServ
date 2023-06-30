CREATE TABLE [dbo].[UserSubscription] (
  [UserId] [nvarchar](450) NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES AspNetUsers(Id),
  [SubscriptionType] [nvarchar](50),
  [SubscriptionEndDate] [DATETIME],
  [SubscriptionStartDate] [DATETIME],
  [DiscountsApplied] [nvarchar](50),
  [AutoRenew] [bit],
  [SubscriptionStatus] [bit],
  [SubscriptionOnboarding] [bit],
  [SubscriptionCancelDate] [DateTime]
);

CREATE TABLE [UserCoachingStyle] (
  [UserId] [nvarchar](450) NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES AspNetUsers(Id),
  [CoachingStyle] [nvarchar](50),
  [Notes] [nvarchar](100),
  [WeightsIntensity] [nvarchar](50),
  [InjuryHistory] [bit],
  [InjuryHistoryNotes] [nvarchar](250),
  [IntrestedInDiet] [bit],
  [DietIntensity] [Int]
);

CREATE TABLE [UserInformation] (
  [UserId] NVARCHAR(450) NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
  [FirstName] NVARCHAR(20) NULL,
  [LastName] NVARCHAR(20) NULL,
  [City] NVARCHAR(20) NULL,
  [State] NVARCHAR(20) NULL,
  [Height] NVARCHAR(10) NULL,
  [Weight] INT NULL,
  [IsInHighschool] BIT NULL,
  [School] NVARCHAR(30) NULL,
  [Birthday] DATETIME NULL,
  [Gender] NVARCHAR(10) NULL,
  [UserType] NVARCHAR(15) NULL,
  [ReportsTo] NVARCHAR(450) NULL,
  [DateCreated] DATETIME NULL,
  [LastAccessed] DATETIME NULL,
  [ActiveUser] BIT NULL
);


CREATE TABLE [UserGoals] (
  [GoalIndex] INT IDENTITY(1,1) PRIMARY KEY,
  [UserId] NVARCHAR(450) NOT NULL,
  [Goal] NVARCHAR(MAX) NULL,
  [GoalStartDate] DATETIME NULL,
  [GoalEndDate] DATETIME NULL,
  [CompletedGoal] BIT NOT NULL,
  [DateCompleted] DATETIME NULL,
  FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE [ReportedInjuries] (
  [UserId] [nvarchar](450) NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
  [InjuryDate] [DATETIME],
  [DuringWorkout] [bit],
  [Severity] [int],
  [Comments] [nvarchar](250)
);
CREATE TABLE [UserTrackRecords] (
  [UserId] [nvarchar](450) NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
  [_400Time] TIME,
  [_800Time] TIME,
  [_1KTime] TIME,
  [_1500Time] TIME,
  [_1600Time] TIME,
  [_3KTime] TIME,
  [_3200Time] TIME,
  [_5KTime] TIME,
  [_8kTime] TIME,
  [_10Ktime] TIME
);

CREATE TABLE [UserProfile] (
  [UserId] [nvarchar](450) NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
  [BestEvent] [nvarchar](50),
  [PrimaryGoal] [nvarchar](200),
  [WhyRun] [nvarchar](max),
  [RecordOrOlympics] [nvarchar] (100)
);

CREATE TABLE [ProfileOnboarding](
    [UserId] [nvarchar](450) NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES AspNetUsers(ID) ON DELETE CASCADE,
    [Completed][bit]
);