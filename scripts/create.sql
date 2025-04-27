
DROP TABLE IF EXISTS Embedded;
DROP TABLE IF EXISTS Smartwatch;
DROP TABLE IF EXISTS PersonalComputer;
DROP TABLE IF EXISTS Device;

CREATE TABLE Device (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(255) NOT NULL,
                        IsEnabled BIT NOT NULL
);


CREATE TABLE Smartwatch (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            BatteryPercentage INT NOT NULL,
                            DeviceId INT NOT NULL,
                            CONSTRAINT FK_Smartwatch_Device FOREIGN KEY (DeviceId) REFERENCES Device(Id) ON DELETE CASCADE
);

CREATE TABLE PersonalComputer (
                                  Id INT IDENTITY(1,1) PRIMARY KEY,
                                  OperatingSystem NVARCHAR(255) NOT NULL,
                                  DeviceId INT NOT NULL,
                                  CONSTRAINT FK_PersonalComputer_Device FOREIGN KEY (DeviceId) REFERENCES Device(Id) ON DELETE CASCADE
);

CREATE TABLE Embedded (
                          Id INT IDENTITY(1,1) PRIMARY KEY,
                          IpAddress NVARCHAR(50) NOT NULL,
                          NetworkName NVARCHAR(255) NOT NULL,
                          DeviceId INT NOT NULL,
                          CONSTRAINT FK_Embedded_Device FOREIGN KEY (DeviceId) REFERENCES Device(Id) ON DELETE CASCADE
);
 