
INSERT INTO Device (Name, IsEnabled) VALUES
                                         ('Galaxy Fit', 1),
                                         ('Apple Watch', 1),
                                         ('Xiaomi Band', 1),
                                         ('Dell XPS', 0),
                                         ('MacBook Pro', 1),
                                         ('Lenovo ThinkPad', 0),
                                         ('Sensor A1', 1),
                                         ('Camera X3', 0),
                                         ('Alarm Unit', 0),
                                         ('Huawei Watch', 1),
                                         ('Asus ROG', 0),
                                         ('Temperature Sensor', 1),
                                         ('Fitbit Sense', 1),
                                         ('Acer Aspire', 0),
                                         ('Gateway Device', 1);

INSERT INTO Smartwatch (BatteryPercentage, DeviceId) VALUES
                                                         (75, 1),
                                                         (90, 2),
                                                         (65, 3),
                                                         (80, 10),
                                                         (55, 13);

INSERT INTO PersonalComputer (OperatingSystem, DeviceId) VALUES
                                                             ('Windows 11', 4),
                                                             ('macOS Ventura', 5),
                                                             ('Ubuntu 22.04', 6),
                                                             ('Windows 10', 11),
                                                             ('Linux Mint', 14);

INSERT INTO Embedded (IpAddress, NetworkName, DeviceId) VALUES
                                                            ('192.168.1.1', 'MD Ltd. Network', 7),
                                                            ('192.168.0.10', 'MD Ltd. Network', 8),
                                                            ('10.0.0.5', 'MD Ltd. Alarm', 9),
                                                            ('172.16.0.12', 'MD Ltd. Climate', 12),
                                                            ('192.168.100.1', 'MD Ltd. Hub', 15);
 