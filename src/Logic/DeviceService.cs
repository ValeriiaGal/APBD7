using System.Data;
using Microsoft.Data.SqlClient;
using APBD2;



public class DeviceService : IDeviceService
{
    private readonly string _connectionString;

    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<Device> GetAllDevices()
    {
        var devices = new List<Device>();

        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand("SELECT Id, Name, IsEnabled FROM Device", connection))
        {
            connection.Open();
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
        }

        return devices;
    }

    public Device GetDeviceById(string id)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("SELECT Id, Name, IsEnabled FROM Device WHERE Id = @id", connection);
        command.Parameters.AddWithValue("@id", id);

        connection.Open();
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Device
            {
                Id = reader["Id"].ToString(),
                Name = reader["Name"].ToString(),
                IsTurnedOn = (bool)reader["IsEnabled"]
            };
        }

        throw new Exception("Device not found");
    }

    public bool CreateDevice(Device device)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("INSERT INTO Device (Name, IsEnabled) VALUES (@name, @enabled)", connection);
        command.Parameters.AddWithValue("@name", device.Name);
        command.Parameters.AddWithValue("@enabled", device.IsTurnedOn);

        connection.Open();
        int result = command.ExecuteNonQuery();
        return result > 0;
    }

    public bool UpdateDevice(Device device)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("UPDATE Device SET Name = @name, IsEnabled = @enabled WHERE Id = @id", connection);
        command.Parameters.AddWithValue("@id", device.Id);
        command.Parameters.AddWithValue("@name", device.Name);
        command.Parameters.AddWithValue("@enabled", device.IsTurnedOn);

        connection.Open();
        int result = command.ExecuteNonQuery();
        return result > 0;
    }

    public bool DeleteDevice(string id)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("DELETE FROM Device WHERE Id = @id", connection);
        command.Parameters.AddWithValue("@id", id);

        connection.Open();
        int result = command.ExecuteNonQuery();
        return result > 0;
    }
}
