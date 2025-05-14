using System.Data;
using System.Data.SqlClient;
using APBD2;
using Microsoft.Data.SqlClient;

namespace APBD2.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly string _connectionString;

    public DeviceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<Device> GetAllDevices()
    {
        var devices = new List<Device>();

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
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var command = new SqlCommand("SELECT Id, Name, IsEnabled FROM Device WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            throw new Exception("Device not found");

        var device = new Device
        {
            Id = reader["Id"].ToString(),
            Name = reader["Name"].ToString(),
            IsTurnedOn = (bool)reader["IsEnabled"]
        };

        return device;
    }

    public bool AddDeviceUsingProcedure(Device device)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            string deviceId = Guid.NewGuid().ToString();
            device.Id = deviceId;

            SqlCommand cmd;

            switch (device)
            {
                case Smartwatch sw:
                    cmd = new SqlCommand("AddSmartwatch", connection, transaction)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                    cmd.Parameters.AddWithValue("@Name", sw.Name);
                    cmd.Parameters.AddWithValue("@IsEnabled", sw.IsTurnedOn);
                    cmd.Parameters.AddWithValue("@BatteryPercentage", sw.Battery);
                    break;

                case PersonalComputer pc:
                    cmd = new SqlCommand("AddPersonalComputer", connection, transaction)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                    cmd.Parameters.AddWithValue("@Name", pc.Name);
                    cmd.Parameters.AddWithValue("@IsEnabled", pc.IsTurnedOn);
                    cmd.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);
                    break;

                case EmbeddedDevice ed:
                    cmd = new SqlCommand("AddEmbedded", connection, transaction)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                    cmd.Parameters.AddWithValue("@Name", ed.Name);
                    cmd.Parameters.AddWithValue("@IsEnabled", ed.IsTurnedOn);
                    cmd.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                    cmd.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                    break;

                default:
                    return false;
            }

            cmd.ExecuteNonQuery();
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
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var updateCmd = new SqlCommand(@"
                UPDATE Device 
                SET Name = @Name, IsEnabled = @IsEnabled 
                WHERE Id = @Id AND RowVersion = @RowVersion", connection, transaction);

            updateCmd.Parameters.AddWithValue("@Name", device.Name);
            updateCmd.Parameters.AddWithValue("@IsEnabled", device.IsTurnedOn);
            updateCmd.Parameters.AddWithValue("@Id", device.Id);
            updateCmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value = device.RowVersion;

            if (updateCmd.ExecuteNonQuery() == 0)
                throw new DBConcurrencyException("Optimistic locking failed. The device has been modified by another user.");

            if (device is Smartwatch sw)
            {
                var cmd = new SqlCommand(
                    "UPDATE Smartwatch SET BatteryPercentage = @Battery WHERE DeviceId = @DeviceId", connection, transaction);
                cmd.Parameters.AddWithValue("@Battery", sw.Battery);
                cmd.Parameters.AddWithValue("@DeviceId", device.Id);
                cmd.ExecuteNonQuery();
            }
            else if (device is PersonalComputer pc)
            {
                var cmd = new SqlCommand(
                    "UPDATE PersonalComputer SET OperatingSystem = @OS WHERE DeviceId = @DeviceId", connection, transaction);
                cmd.Parameters.AddWithValue("@OS", pc.OperatingSystem);
                cmd.Parameters.AddWithValue("@DeviceId", device.Id);
                cmd.ExecuteNonQuery();
            }
            else if (device is EmbeddedDevice ed)
            {
                var cmd = new SqlCommand(
                    "UPDATE Embedded SET IpAddress = @IP, NetworkName = @Network WHERE DeviceId = @DeviceId", connection, transaction);
                cmd.Parameters.AddWithValue("@IP", ed.IpAddress);
                cmd.Parameters.AddWithValue("@Network", ed.NetworkName);
                cmd.Parameters.AddWithValue("@DeviceId", device.Id);
                cmd.ExecuteNonQuery();
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
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var cmd = new SqlCommand("DELETE FROM Device WHERE Id = @Id", connection);
        cmd.Parameters.AddWithValue("@Id", id);

        return cmd.ExecuteNonQuery() > 0;
    }
}
