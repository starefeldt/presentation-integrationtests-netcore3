IF EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_SCHEMA = 'TheSchema' 
            AND  TABLE_NAME = 'Students')
    BEGIN
        DROP TABLE [Students];
    END