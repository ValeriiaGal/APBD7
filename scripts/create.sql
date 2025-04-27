
IF OBJECT_ID('Embedded', 'U') IS NOT NULL DROP TABLE Embedded;
IF OBJECT_ID('PersonalComputer', 'U') IS NOT NULL DROP TABLE PersonalComputer;
IF OBJECT_ID('Smartwatch', 'U') IS NOT NULL DROP TABLE Smartwatch;
IF OBJECT_ID('Device', 'U') IS NOT NULL DROP TABLE Device;


CREATE TABLE Device (
                        Id NVARCHAR(50) PRIMARY KEY,    
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
