using Microsoft.Data.SqlClient;

namespace APBD2;

public class DeviceService : IDeviceService
{
    private readonly string _connectionString;

    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<Device> GetAllDevices()
    {
        List<Device> devices = [];

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var command = new SqlCommand("SELECT Id, Name, IsEnabled FROM Device", connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            devices.Add(new Device
            {
                Id = reader["Id"].ToString(),
                Name = reader["Name"].ToString(),
                IsTurnedOn = (bool)reader["IsEnabled"]
            });
        }

        return devices;
    }

    public Device GetDeviceById(string id)
    {
        if (!int.TryParse(id, out int deviceId))
            throw new ArgumentException("Invalid ID format.");

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var command = new SqlCommand("SELECT Id, Name, IsEnabled FROM Device WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", deviceId);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            throw new Exception("Device not found");

        var idFromDb = reader["Id"].ToString();
        var name = reader["Name"].ToString();
        var isEnabled = (bool)reader["IsEnabled"];

        reader.Close();
        
        var checkSmartwatch = new SqlCommand("SELECT BatteryPercentage FROM Smartwatch WHERE DeviceId = @DeviceId", connection);
        checkSmartwatch.Parameters.AddWithValue("@DeviceId", deviceId);
        var smartwatchBattery = checkSmartwatch.ExecuteScalar();

        if (smartwatchBattery != null)
        {
            return new Smartwatch
            {
                Id = idFromDb,
                Name = name,
                IsTurnedOn = isEnabled,
                Battery = (int)smartwatchBattery
            };
        }

        var checkPC = new SqlCommand("SELECT OperatingSystem FROM PersonalComputer WHERE DeviceId = @DeviceId", connection);
        checkPC.Parameters.AddWithValue("@DeviceId", deviceId);
        var operatingSystem = checkPC.ExecuteScalar();

        if (operatingSystem != null)
        {
            return new PersonalComputer
            {
                Id = idFromDb,
                Name = name,
                IsTurnedOn = isEnabled,
                OperatingSystem = operatingSystem.ToString()
            };
        }

        var checkEmbedded = new SqlCommand("SELECT IpAddress, NetworkName FROM Embedded WHERE DeviceId = @DeviceId", connection);
        checkEmbedded.Parameters.AddWithValue("@DeviceId", deviceId);

        using var embeddedReader = checkEmbedded.ExecuteReader();
        if (embeddedReader.Read())
        {
            return new EmbeddedDevice
            {
                Id = idFromDb,
                Name = name,
                IsTurnedOn = isEnabled,
                IpAddress = embeddedReader["IpAddress"].ToString(),
                NetworkName = embeddedReader["NetworkName"].ToString()
            };
        }
        
        return new Device
        {
            Id = idFromDb,
            Name = name,
            IsTurnedOn = isEnabled
        };
    }

    public bool CreateDevice(Device device)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var insertDevice = new SqlCommand(
                "INSERT INTO Device (Name, IsEnabled) OUTPUT INSERTED.Id VALUES (@Name, @IsEnabled)", connection, transaction);
            insertDevice.Parameters.AddWithValue("@Name", device.Name);
            insertDevice.Parameters.AddWithValue("@IsEnabled", device.IsTurnedOn);

            var insertedId = (int)insertDevice.ExecuteScalar();
            device.Id = insertedId.ToString();

            if (device is Smartwatch smartwatch)
            {
                var insertSmartwatch = new SqlCommand(
                    "INSERT INTO Smartwatch (BatteryPercentage, DeviceId) VALUES (@Battery, @DeviceId)", connection, transaction);
                insertSmartwatch.Parameters.AddWithValue("@Battery", smartwatch.Battery);
                insertSmartwatch.Parameters.AddWithValue("@DeviceId", insertedId);
                insertSmartwatch.ExecuteNonQuery();
            }
            else if (device is PersonalComputer pc)
            {
                var insertPC = new SqlCommand(
                    "INSERT INTO PersonalComputer (OperatingSystem, DeviceId) VALUES (@OS, @DeviceId)", connection, transaction);
                insertPC.Parameters.AddWithValue("@OS", pc.OperatingSystem);
                insertPC.Parameters.AddWithValue("@DeviceId", insertedId);
                insertPC.ExecuteNonQuery();
            }
            else if (device is EmbeddedDevice embedded)
            {
                var insertEmbedded = new SqlCommand(
                    "INSERT INTO Embedded (IpAddress, NetworkName, DeviceId) VALUES (@IP, @Network, @DeviceId)", connection, transaction);
                insertEmbedded.Parameters.AddWithValue("@IP", embedded.IpAddress);
                insertEmbedded.Parameters.AddWithValue("@Network", embedded.NetworkName);
                insertEmbedded.Parameters.AddWithValue("@DeviceId", insertedId);
                insertEmbedded.ExecuteNonQuery();
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }

    public bool UpdateDevice(Device device)
    {
        if (!int.TryParse(device.Id, out int deviceId))
            throw new ArgumentException("Invalid ID format.");

        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var updateDevice = new SqlCommand(
                "UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id", connection, transaction);
            updateDevice.Parameters.AddWithValue("@Name", device.Name);
            updateDevice.Parameters.AddWithValue("@IsEnabled", device.IsTurnedOn);
            updateDevice.Parameters.AddWithValue("@Id", deviceId);

            if (updateDevice.ExecuteNonQuery() == 0)
                throw new Exception("Device not found.");

            if (device is Smartwatch smartwatch)
            {
                var updateSmartwatch = new SqlCommand(
                    "UPDATE Smartwatch SET BatteryPercentage = @Battery WHERE DeviceId = @DeviceId", connection, transaction);
                updateSmartwatch.Parameters.AddWithValue("@Battery", smartwatch.Battery);
                updateSmartwatch.Parameters.AddWithValue("@DeviceId", deviceId);
                updateSmartwatch.ExecuteNonQuery();
            }
            else if (device is PersonalComputer pc)
            {
                var updatePC = new SqlCommand(
                    "UPDATE PersonalComputer SET OperatingSystem = @OS WHERE DeviceId = @DeviceId", connection, transaction);
                updatePC.Parameters.AddWithValue("@OS", pc.OperatingSystem);
                updatePC.Parameters.AddWithValue("@DeviceId", deviceId);
                updatePC.ExecuteNonQuery();
            }
            else if (device is EmbeddedDevice embedded)
            {
                var updateEmbedded = new SqlCommand(
                    "UPDATE Embedded SET IpAddress = @IP, NetworkName = @Network WHERE DeviceId = @DeviceId", connection, transaction);
                updateEmbedded.Parameters.AddWithValue("@IP", embedded.IpAddress);
                updateEmbedded.Parameters.AddWithValue("@Network", embedded.NetworkName);
                updateEmbedded.Parameters.AddWithValue("@DeviceId", deviceId);
                updateEmbedded.ExecuteNonQuery();
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }

    public bool DeleteDevice(string id)
    {
        if (!int.TryParse(id, out int deviceId))
            return false;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var command = new SqlCommand("DELETE FROM Device WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", deviceId);

        return command.ExecuteNonQuery() > 0;
    }
}
