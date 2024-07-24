CREATE TABLE [dbo].[Todos]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(256) NOT NULL,
    [Description] NVARCHAR(2048) NULL,
    [StartDate] DATE NOT NULL,
    [StartTime] TIME(7) NOT NULL,
    [FinishDate] DATE NULL,
    [FinishTime] TIME(7) NULL,
    [CreationDate] DATETIME NOT NULL DEFAULT getutcdate(),
    [LastModificationDate] DATETIME NULL,

    PRIMARY KEY([Id]),
    FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers]([Id])
)