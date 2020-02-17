IF  EXISTS (SELECT * FROM sys.databases WHERE name = N'StudentManagementApi')
    BEGIN
        DROP DATABASE [StudentManagementApi]
    END;