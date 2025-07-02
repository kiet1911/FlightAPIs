IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF SCHEMA_ID(N'HangFire') IS NULL EXEC(N'CREATE SCHEMA [HangFire];');

CREATE TABLE [Admin] (
    [id] int NOT NULL IDENTITY,
    [name] nvarchar(255) NULL,
    [email] nvarchar(255) NOT NULL,
    [cccd] nvarchar(255) NULL,
    [address] nvarchar(255) NULL,
    [phone_number] nvarchar(255) NULL,
    [password] nvarchar(255) NOT NULL,
    [user_type] int NOT NULL,
    CONSTRAINT [PK__Admin__3213E83F52636478] PRIMARY KEY ([id])
);

CREATE TABLE [HangFire].[AggregatedCounter] (
    [Key] nvarchar(100) NOT NULL,
    [Value] bigint NOT NULL,
    [ExpireAt] datetime NULL,
    CONSTRAINT [PK_HangFire_CounterAggregated] PRIMARY KEY ([Key])
);

CREATE TABLE [AirPort] (
    [id] int NOT NULL IDENTITY,
    [name] nvarchar(255) NOT NULL,
    [code] nvarchar(255) NOT NULL,
    [address] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_AirPort] PRIMARY KEY ([id])
);

CREATE TABLE [HangFire].[Counter] (
    [Key] nvarchar(100) NOT NULL,
    [Id] bigint NOT NULL IDENTITY,
    [Value] int NOT NULL,
    [ExpireAt] datetime NULL,
    CONSTRAINT [PK_HangFire_Counter] PRIMARY KEY ([Key], [Id])
);

CREATE TABLE [FlightSchedules] (
    [id] int NOT NULL IDENTITY,
    [plane_id] int NOT NULL,
    [from_airport] int NOT NULL,
    [to_airport] int NOT NULL,
    [departures_at] datetime NOT NULL,
    [arrivals_at] datetime NOT NULL,
    [cost] int NOT NULL,
    [code] nvarchar(255) NOT NULL,
    [totalSeats] int NULL,
    [status_fs] nvarchar(50) NULL,
    [bookedSeats] int NULL,
    [availableSeats] int NULL,
    CONSTRAINT [PK_FlightSchedules] PRIMARY KEY ([id])
);

CREATE TABLE [HangFire].[Hash] (
    [Key] nvarchar(100) NOT NULL,
    [Field] nvarchar(100) NOT NULL,
    [Value] nvarchar(max) NULL,
    [ExpireAt] datetime2 NULL,
    CONSTRAINT [PK_HangFire_Hash] PRIMARY KEY ([Key], [Field])
);

CREATE TABLE [HangFire].[Job] (
    [Id] bigint NOT NULL IDENTITY,
    [StateId] bigint NULL,
    [StateName] nvarchar(20) NULL,
    [InvocationData] nvarchar(max) NOT NULL,
    [Arguments] nvarchar(max) NOT NULL,
    [CreatedAt] datetime NOT NULL,
    [ExpireAt] datetime NULL,
    CONSTRAINT [PK_HangFire_Job] PRIMARY KEY ([Id])
);

CREATE TABLE [HangFire].[JobQueue] (
    [Id] bigint NOT NULL IDENTITY,
    [Queue] nvarchar(50) NOT NULL,
    [JobId] bigint NOT NULL,
    [FetchedAt] datetime NULL,
    CONSTRAINT [PK_HangFire_JobQueue] PRIMARY KEY ([Queue], [Id])
);

CREATE TABLE [HangFire].[List] (
    [Id] bigint NOT NULL IDENTITY,
    [Key] nvarchar(100) NOT NULL,
    [Value] nvarchar(max) NULL,
    [ExpireAt] datetime NULL,
    CONSTRAINT [PK_HangFire_List] PRIMARY KEY ([Key], [Id])
);

CREATE TABLE [Payments] (
    [id] int NOT NULL IDENTITY,
    [email_Payment] nvarchar(255) NULL,
    [name_Payment] nvarchar(255) NULL,
    [PayerID_Payment] nvarchar(255) NULL,
    [UserID] int NULL,
    CONSTRAINT [PK__Payments__3213E83F4822C229] PRIMARY KEY ([id])
);

CREATE TABLE [Plane] (
    [id] int NOT NULL IDENTITY,
    [name] nvarchar(255) NOT NULL,
    [code] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_Plane] PRIMARY KEY ([id])
);

CREATE TABLE [Role] (
    [id] int NOT NULL IDENTITY,
    [name_role] nvarchar(255) NULL,
    [status] nvarchar(255) NULL,
    CONSTRAINT [PK__Role__3213E83F03C0FCA5] PRIMARY KEY ([id])
);

CREATE TABLE [HangFire].[Schema] (
    [Version] int NOT NULL,
    CONSTRAINT [PK_HangFire_Schema] PRIMARY KEY ([Version])
);

CREATE TABLE [HangFire].[Server] (
    [Id] nvarchar(200) NOT NULL,
    [Data] nvarchar(max) NULL,
    [LastHeartbeat] datetime NOT NULL,
    CONSTRAINT [PK_HangFire_Server] PRIMARY KEY ([Id])
);

CREATE TABLE [HangFire].[Set] (
    [Key] nvarchar(100) NOT NULL,
    [Value] nvarchar(256) NOT NULL,
    [Score] float NOT NULL,
    [ExpireAt] datetime NULL,
    CONSTRAINT [PK_HangFire_Set] PRIMARY KEY ([Key], [Value])
);

CREATE TABLE [User] (
    [id] int NOT NULL IDENTITY,
    [name] nvarchar(255) NULL,
    [email] nvarchar(255) NOT NULL,
    [cccd] nvarchar(255) NULL,
    [address] nvarchar(255) NULL,
    [phone_number] nvarchar(255) NULL,
    [password] nvarchar(255) NOT NULL,
    [user_type] int NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([id])
);

CREATE TABLE [Seats] (
    [id] int NOT NULL IDENTITY,
    [flight_schedules_id] int NOT NULL,
    [seat] nvarchar(255) NULL,
    [isbooked] int NULL,
    [Version] rowversion NOT NULL,
    [BookingExpiration] datetime NULL,
    CONSTRAINT [PK__Seats__3213E83FB74D5565] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Seats_FlightSchedules] FOREIGN KEY ([flight_schedules_id]) REFERENCES [FlightSchedules] ([id])
);

CREATE TABLE [HangFire].[JobParameter] (
    [JobId] bigint NOT NULL,
    [Name] nvarchar(40) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_HangFire_JobParameter] PRIMARY KEY ([JobId], [Name]),
    CONSTRAINT [FK_HangFire_JobParameter_Job] FOREIGN KEY ([JobId]) REFERENCES [HangFire].[Job] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [HangFire].[State] (
    [Id] bigint NOT NULL IDENTITY,
    [JobId] bigint NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    [Reason] nvarchar(100) NULL,
    [CreatedAt] datetime NOT NULL,
    [Data] nvarchar(max) NULL,
    CONSTRAINT [PK_HangFire_State] PRIMARY KEY ([JobId], [Id]),
    CONSTRAINT [FK_HangFire_State_Job] FOREIGN KEY ([JobId]) REFERENCES [HangFire].[Job] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Baggage] (
    [id] int NOT NULL IDENTITY,
    [carryon_bag] int NOT NULL,
    [signed_luggage] int NOT NULL,
    [code] nchar(255) NOT NULL,
    [user_id] int NOT NULL,
    CONSTRAINT [PK__Baggage__3213E83F3EBC3A8A] PRIMARY KEY ([id]),
    CONSTRAINT [FK_Baggage_user_id] FOREIGN KEY ([user_id]) REFERENCES [User] ([id])
);

CREATE TABLE [TicketManager] (
    [id] int NOT NULL IDENTITY,
    [flight_schedules_id] int NOT NULL,
    [user_id] int NOT NULL,
    [status] int NOT NULL,
    [code] nchar(255) NOT NULL,
    [seat_location] int NULL,
    [pay_id] int NULL,
    CONSTRAINT [PK_TicketManager] PRIMARY KEY ([id]),
    CONSTRAINT [FK_TicketManager_FlightSchedules] FOREIGN KEY ([flight_schedules_id]) REFERENCES [FlightSchedules] ([id]),
    CONSTRAINT [FK_TicketManager_User] FOREIGN KEY ([user_id]) REFERENCES [User] ([id])
);

CREATE INDEX [IX_Baggage_user_id] ON [Baggage] ([user_id]);

CREATE INDEX [IX_Seats_flight_schedules_id] ON [Seats] ([flight_schedules_id]);

CREATE INDEX [IX_TicketManager_flight_schedules_id] ON [TicketManager] ([flight_schedules_id]);

CREATE INDEX [IX_TicketManager_user_id] ON [TicketManager] ([user_id]);

COMMIT;
GO

