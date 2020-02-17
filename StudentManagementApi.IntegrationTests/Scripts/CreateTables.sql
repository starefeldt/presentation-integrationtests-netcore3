IF NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'TheSchema' 
                 AND  TABLE_NAME = 'Students')
    BEGIN
        CREATE TABLE [Students] (
            [Id] int NOT NULL IDENTITY,
            [FirstName] nvarchar(max) NULL,
            [LastName] nvarchar(max) NULL,
            [SocialSecurityNumber] nvarchar(max) NULL,
            [Created] datetimeoffset NOT NULL,
            CONSTRAINT [PK_Students] PRIMARY KEY ([Id])
        );
    END