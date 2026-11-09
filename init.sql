IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TaxiDispatcherDB')
    CREATE DATABASE TaxiDispatcherDB;
GO
USE TaxiDispatcherDB;
GO

DROP TABLE IF EXISTS Invoices;
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS Tariffs;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS CarClasses;
DROP TABLE IF EXISTS OrderStatuses;
DROP TABLE IF EXISTS Roles;
GO

CREATE TABLE Roles (
    role_id int IDENTITY(1,1) PRIMARY KEY,
    role_name nvarchar(50) NOT NULL UNIQUE
);

CREATE TABLE Users (
    user_id int IDENTITY(1,1) PRIMARY KEY,
    login nvarchar(50) NOT NULL UNIQUE,
    last_name nvarchar(60) NULL,
    first_name nvarchar(60) NULL,
    middle_name nvarchar(60) NULL,
    phone nvarchar(20) NULL,
    role_id int NOT NULL,
    password_hash nvarchar(255) NULL,
    FOREIGN KEY (role_id) REFERENCES Roles(role_id)
);

CREATE TABLE CarClasses (
    class_id int IDENTITY(1,1) PRIMARY KEY,
    class_name nvarchar(50) NOT NULL UNIQUE
);

CREATE TABLE OrderStatuses (
    status_id int IDENTITY(1,1) PRIMARY KEY,
    status_name nvarchar(50) NOT NULL UNIQUE
);

CREATE TABLE Tariffs (
    tariff_id int IDENTITY(1,1) PRIMARY KEY,
    tariff_name nvarchar(50) NOT NULL UNIQUE,
    base_fare decimal(8,2) NOT NULL CHECK (base_fare >= 0),
    price_per_km decimal(8,2) NOT NULL CHECK (price_per_km >= 0),
    min_fare decimal(8,2) NOT NULL CHECK (min_fare >= 0),
    car_class_id int NOT NULL,
    FOREIGN KEY (car_class_id) REFERENCES CarClasses(class_id)
);

CREATE TABLE Orders (
    order_id int IDENTITY(1,1) PRIMARY KEY,
    order_date datetime2 NOT NULL DEFAULT GETDATE(),
    client_name nvarchar(150) NOT NULL,
    client_phone nvarchar(20) NOT NULL,
    pickup_address nvarchar(250) NOT NULL,
    destination_address nvarchar(250) NOT NULL,
    distance_km decimal(6,2) NULL CHECK (distance_km > 0),
    status_id int NOT NULL,
    tariff_id int NULL,
    FOREIGN KEY (status_id) REFERENCES OrderStatuses(status_id),
    FOREIGN KEY (tariff_id) REFERENCES Tariffs(tariff_id)
);

CREATE TABLE Invoices (
    invoice_id int IDENTITY(1,1) PRIMARY KEY,
    order_id int NOT NULL UNIQUE,
    amount decimal(10,2) NOT NULL CHECK (amount >= 0),
    payment_status nvarchar(50) NOT NULL,
    FOREIGN KEY (order_id) REFERENCES Orders(order_id)
);
GO

INSERT INTO Roles (role_name) VALUES 
(N'Администратор'), 
(N'Диспетчер');

INSERT INTO CarClasses (class_name) VALUES 
(N'Эконом'), 
(N'Комфорт'), 
(N'Бизнес');

INSERT INTO OrderStatuses (status_name) VALUES 
(N'Новый'), 
(N'Назначен'), 
(N'В пути'), 
(N'Завершён'), 
(N'Отменён');

INSERT INTO Tariffs (tariff_name, base_fare, price_per_km, min_fare, car_class_id) VALUES 
(N'Эконом стандарт', 100.00, 20.00, 150.00, 1),
(N'Комфорт плюс', 150.00, 30.00, 250.00, 2),
(N'Бизнес премиум', 300.00, 50.00, 500.00, 3);

INSERT INTO Users (login, last_name, first_name, middle_name, phone, role_id, password_hash) VALUES 
(N'admin', N'Сидоров', N'Алексей', N'Петрович', N'+79991112233', 1, N'$2a$11$0R9.w.7mB/1c1N7g4hK9O.s2L/5j3k4l5m6n7o8p9q0r1s2t3u4v');

INSERT INTO Users (login, last_name, first_name, middle_name, phone, role_id, password_hash) VALUES 
(N'disp', N'Иванова', N'Мария', N'Сергеевна', N'+79992223344', 2, N'$2a$11$0R9.w.7mB/1c1N7g4hK9O.s2L/5j3k4l5m6n7o8p9q0r1s2t3u4v');
GO

SELECT 'Роли' as [Таблица], COUNT(*) as [Кол-во] FROM Roles
UNION ALL SELECT 'Пользователи', COUNT(*) FROM Users
UNION ALL SELECT 'Классы авто', COUNT(*) FROM CarClasses
UNION ALL SELECT 'Тарифы', COUNT(*) FROM Tariffs
UNION ALL SELECT 'Статусы', COUNT(*) FROM OrderStatuses;
GO
