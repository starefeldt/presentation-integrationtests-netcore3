IF  NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'StudentManagementApi')
    BEGIN
        CREATE DATABASE [StudentManagementApi]
    END;