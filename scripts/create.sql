
DROP TABLE IF EXISTS Device;
DROP TABLE IF EXISTS Smartwatch;
DROP TABLE IF EXISTS PersonalComputer;
DROP TABLE IF EXISTS Embedded;



CREATE TABLE Device
(
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    Name NVARCHAR(255) NOT NULL,
    IsEnabled BIT NOT NULL
);


CREATE TABLE Smartwatch (
                            Id INT PRIMARY KEY IDENTITY(1,1), 
                            BatteryPercentage INT NOT NULL,
                            DeviceId NVARCHAR(50) NOT NULL,    
                            CONSTRAINT FK_Smartwatch_Device FOREIGN KEY (DeviceId) REFERENCES Device(Id) ON DELETE CASCADE
);


CREATE TABLE PersonalComputer (
                                  Id INT PRIMARY KEY IDENTITY(1,1),
                                  OperationSystem NVARCHAR(255) NOT NULL,
                                  DeviceId NVARCHAR(50) NOT NULL,
                                  CONSTRAINT FK_PersonalComputer_Device FOREIGN KEY (DeviceId) REFERENCES Device(Id) ON DELETE CASCADE
);


CREATE TABLE Embedded (
                          Id INT PRIMARY KEY IDENTITY(1,1),
                          IpAddress VARCHAR(50) NOT NULL,
                          NetworkName VARCHAR(255) NOT NULL,
                          DeviceId NVARCHAR(50) NOT NULL,
                          CONSTRAINT FK_Embedded_Device FOREIGN KEY (DeviceId) REFERENCES Device(Id) ON DELETE CASCADE
);
