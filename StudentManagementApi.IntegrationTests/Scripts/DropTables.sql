IF EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_SCHEMA = 'dbo' 
            AND  TABLE_NAME = 'Student')
    BEGIN
        DROP TABLE [Student];
    END