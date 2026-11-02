IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TaxiDispatcherDB')
    CREATE DATABASE TaxiDispatcherDB;
GO
USE TaxiDispatcherDB;
GO
IF OBJECT_ID('Roles', 'U') IS NULL
BEGIN
    CREATE TABLE Roles (
        role_id int IDENTITY(1,1) PRIMARY KEY,
        role_name nvarchar(50) NOT NULL UNIQUE
    );
    CREATE TABLE Users (
        user_id int IDENTITY(1,1) PRIMARY KEY,
        login nvarchar(50) NOT NULL UNIQUE,
        full_name nvarchar(150) NOT NULL,
        phone nvarchar(20) NULL,
        role_id int NOT NULL,
        FOREIGN KEY (role_id) REFERENCES Roles(role_id)
    );
END
