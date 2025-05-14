
DROP TABLE IF EXISTS Embedded;
DROP TABLE IF EXISTS Smartwatch;
DROP TABLE IF EXISTS PersonalComputer;
DROP TABLE IF EXISTS Device;

CREATE TABLE Device (
                        Id NVARCHAR(20) PRIMARY KEY,
                        Name NVARCHAR(255) NOT NULL,
                        IsEnabled BIT NOT NULL
);


CREATE TABLE Smartwatch (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            BatteryPercentage INT NOT NULL,
                            DeviceId NVARCHAR(20) NOT NULL, FOREIGN KEY (DeviceId) REFERENCES Device(Id) 
);

CREATE TABLE PersonalComputer (
                                  Id INT IDENTITY(1,1) PRIMARY KEY,
                                  OperationSystem NVARCHAR(255) NOT NULL,
                                  DeviceId NVARCHAR(20) NOT NULL, FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);

CREATE TABLE Embedded (
                          Id INT IDENTITY(1,1) PRIMARY KEY,
                          IpAddress NVARCHAR(50) NOT NULL,
                          NetworkName NVARCHAR(255) NOT NULL,
                          DeviceId NVARCHAR(20) NOT NULL, FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);
ALTER TABLE Device
    ADD RowVersion ROWVERSION;