CREATE TABLE [dbo].[Sessions] (
    [ID]              INT       IDENTITY (1, 1) NOT NULL,
    [AccountId]       INT       NOT NULL,
    [OpenDate]        DATETIME  NOT NULL,
    [LastRenewalDate] DATETIME  NULL,
    [CloseDate]       DATETIME  NULL,
    [SessionId]       CHAR (36) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SessionsAccountId] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Accounts] ([ID]) ON DELETE CASCADE ON UPDATE CASCADE,
    UNIQUE NONCLUSTERED ([SessionId] ASC)
);

