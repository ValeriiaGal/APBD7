
INSERT INTO Device (Id, Name, IsEnabled)
VALUES
    ('1', 'Galaxy Fcuit', 1),
    ('2', 'Apple Watch', 1),
    ('3', 'Xiaomi Band', 0),
    ('4', 'Dell XPS', 1),
    ('5', 'MacBook Pro', 1),
    ('6', 'Lenovo ThinkPad', 0),
    ('7', 'Sensor A1', 1),
    ('8', 'Camera X3', 1),
    ('9', 'Alarm Unit', 0),
    ('10', 'Huaweiy Watch', 1),
    ('11', 'Asus ROG', 1),
    ('12', 'Temperature Sensor', 1),
    ('13', 'Fitbit Sense', 0),
    ('14', 'Acer Aspire', 0),
    ('15', 'Gateway Device', 1);


INSERT INTO Smartwatch (BatteryPercentage, DeviceId)
VALUES
    (75, '1'),
    (90, '2'),
    (65, '3'),
    (80, '10'),
    (55, '13');


INSERT INTO PersonalComputer (OperationSystem, DeviceId)
VALUES
    ('Windows 11', '4'),
    ('macOS Ventura', '5'),
    ('Ubuntu 22.04', '6'),
    ('Windows 10', '11'),
    ('Linux Mint', '14');


INSERT INTO Embedded (IpAddress, NetworkName, DeviceId)
VALUES
    ('192.168.1.1', 'MD Ltd. Network', '7'),
    ('192.168.0.10', 'MD Ltd. Network', '8'),
    ('10.0.0.5', 'MD Ltd. Alarm', '9'),
    ('172.16.0.12', 'MD Ltd. Climate', '12'),
    ('192.168.100.1', 'MD Ltd. Hub', '15');
