CREATE TABLE [dbo].[Workout] (
  [WorkoutId] INT PRIMARY KEY IDENTITY(1,1),
  [WorkoutTypeId] INT,
  [WorkoutName] VARCHAR(100),
  [CoachId] VARCHAR(450),
  [Notes] VARCHAR(150),
);

CREATE TABLE [dbo].[WorkoutBlock] (
  [BlockId] INT PRIMARY KEY IDENTITY(1,1),
  [WorkoutId] INT,
  [BlockName] VARCHAR(50),
  [BlockType] INT,
  CONSTRAINT [FK_WorkoutBlock_Workout]
    FOREIGN KEY ([WorkoutId])
      REFERENCES [Workout]([WorkoutId])
      ON DELETE CASCADE
);

CREATE TABLE [dbo].[WorkoutInfo] (
  [WorkoutID] INT PRIMARY KEY,
  [Description] VARCHAR(150),
  [Difficulty] VARCHAR(30),
  [TeamID] INT,
  [CoachId] VARCHAR(450),
  [SharePublic] BIT,
  CONSTRAINT [FK_WorkoutInfo_Workout]
    FOREIGN KEY ([WorkoutID])
      REFERENCES [Workout]([WorkoutId])
      ON DELETE CASCADE
);

CREATE TABLE [dbo].[AssignedWorkouts] (
  [Index] INT PRIMARY KEY IDENTITY(1,1),
  [WorkoutID] INT,
  [WorkoutDate] DATETIME,
  [Notes] VARCHAR(150),
  [AssigneeId] VARCHAR(450),
  [ReportBack] BIT,
  [WorkoutName] VARCHAR(100)
  CONSTRAINT [FK_AssignedWorkouts_Workout]
    FOREIGN KEY ([WorkoutID])
      REFERENCES [Workout]([WorkoutId])
      ON DELETE CASCADE
);

CREATE TABLE [dbo].[Parameter] (
  [ParameterId] INT PRIMARY KEY IDENTITY(1,1),
  [BlockId] INT,
  [sValue1]  VARCHAR(100),
  [sValue2] VARCHAR(100),
  [tTime1] TIME,
  [tTime2] TIME,
  [sDistance1] VARCHAR(50),
  [sDistance2] VARCHAR(50),
  [ParameterType] INT,
  [BlockType] INT,
  CONSTRAINT [FK_Parameter_WorkoutBlock]
    FOREIGN KEY ([BlockId])
      REFERENCES [WorkoutBlock]([BlockId])
);

CREATE TABLE [dbo].[WorkoutHistory] (
  [Index] INT PRIMARY KEY IDENTITY(1,1),
  [UserId] VARCHAR(450),
  [WorkoutDate] VARCHAR(30),
  [Notes] VARCHAR(150),
  [WorkoutID] INT,
  CONSTRAINT [FK_WorkoutHistory_Workout]
    FOREIGN KEY ([WorkoutID])
      REFERENCES [Workout]([WorkoutId])
      ON DELETE CASCADE
);


CREATE TABLE [dbo].[WorkoutReports] (
  [WorkoutId] INT PRIMARY KEY,
  [AssigneeID] VARCHAR(450),
  [StarRating] SMALLINT,
  [Notes] VARCHAR(500),
  [DateReported] DATETIMEOFFSET,
  CONSTRAINT [FK_WorkoutReports_Workout]
    FOREIGN KEY ([WorkoutId])
      REFERENCES [Workout]([WorkoutId])
      ON DELETE CASCADE
);
