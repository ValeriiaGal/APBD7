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

        var command = new SqlCommand("SELECT Id, Name, IsEnabled, RowVersion FROM Device", connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            devices.Add(new Device
            {
                Id = reader["Id"].ToString(),
                Name = reader["Name"].ToString(),
                IsTurnedOn = (bool)reader["IsEnabled"],
                RowVersion = (byte[])reader["RowVersion"]
            });
        }

        return devices;
    }

    public Device GetDeviceById(string id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var command = new SqlCommand("SELECT Id, Name, IsEnabled, RowVersion FROM Device WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            throw new Exception("Device not found");

        return new Device
        {
            Id = reader["Id"].ToString(),
            Name = reader["Name"].ToString(),
            IsTurnedOn = (bool)reader["IsEnabled"],
            RowVersion = (byte[])reader["RowVersion"]
        };
    }

    public bool AddDeviceUsingProcedure(Device device)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            if (string.IsNullOrWhiteSpace(device.Id))
                throw new InvalidOperationException("Device ID must be assigned before adding to DB.");

            SqlCommand cmd = device switch
            {
                Smartwatch sw when sw.Battery < 0 || sw.Battery > 100 => throw new ArgumentException("Battery level must be between 0 and 100."),
                Smartwatch sw when sw.Battery < 11 && sw.IsTurnedOn => throw new ArgumentException("Smartwatch can't be turned on when battery level is below 11"),

                PersonalComputer pc when string.IsNullOrWhiteSpace(pc.OperatingSystem) && pc.IsTurnedOn =>
                    throw new ArgumentException("There should be an operating system for PC to turn on"),

                EmbeddedDevice ed when !ed.NetworkName.Contains("MD Ltd.") && ed.IsTurnedOn =>
                    throw new ArgumentException("Embedded Device network must be MD Ltd. to be turned on"),

                EmbeddedDevice ed when !System.Text.RegularExpressions.Regex.IsMatch(ed.IpAddress,
                    @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$") =>
                    throw new ArgumentException("Invalid IP address."),

                Smartwatch sw => new SqlCommand("AddSmartwatch", connection, transaction)
                {
                    CommandType = CommandType.StoredProcedure,
                    Parameters =
                    {
                        new("@DeviceId", sw.Id),
                        new("@Name", sw.Name),
                        new("@IsEnabled", sw.IsTurnedOn),
                        new("@BatteryPercentage", sw.Battery)
                    }
                },

                PersonalComputer pc => new SqlCommand("AddPersonalComputer", connection, transaction)
                {
                    CommandType = CommandType.StoredProcedure,
                    Parameters =
                    {
                        new("@DeviceId", pc.Id),
                        new("@Name", pc.Name),
                        new("@IsEnabled", pc.IsTurnedOn),
                        new("@OperationSystem", pc.OperatingSystem)
                    }
                },

                EmbeddedDevice ed => new SqlCommand("AddEmbedded", connection, transaction)
                {
                    CommandType = CommandType.StoredProcedure,
                    Parameters =
                    {
                        new("@DeviceId", ed.Id),
                        new("@Name", ed.Name),
                        new("@IsEnabled", ed.IsTurnedOn),
                        new("@IpAddress", ed.IpAddress),
                        new("@NetworkName", ed.NetworkName)
                    }
                },

                _ => throw new InvalidOperationException("Unsupported device type.")
            };

            cmd.ExecuteNonQuery();
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("AddDevice failed: " + ex.Message);
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

            SqlCommand cmd = device switch
            {
                Smartwatch sw => new SqlCommand(
                    "UPDATE Smartwatch SET BatteryPercentage = @Battery WHERE DeviceId = @DeviceId", connection, transaction)
                {
                    Parameters =
                    {
                        new("@Battery", sw.Battery),
                        new("@DeviceId", sw.Id)
                    }
                },

                PersonalComputer pc => new SqlCommand(
                    "UPDATE PersonalComputer SET OperatingSystem = @OS WHERE DeviceId = @DeviceId", connection, transaction)
                {
                    Parameters =
                    {
                        new("@OS", pc.OperatingSystem),
                        new("@DeviceId", pc.Id)
                    }
                },

                EmbeddedDevice ed => new SqlCommand(
                    "UPDATE Embedded SET IpAddress = @IP, NetworkName = @Network WHERE DeviceId = @DeviceId", connection, transaction)
                {
                    Parameters =
                    {
                        new("@IP", ed.IpAddress),
                        new("@Network", ed.NetworkName),
                        new("@DeviceId", ed.Id)
                    }
                },

                _ => null
            };

            cmd?.ExecuteNonQuery();
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("UpdateDevice failed: " + ex.Message);
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