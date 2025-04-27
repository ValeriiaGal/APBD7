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

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT Id, Name, IsEnabled FROM Device", connection);

            using (var reader = command.ExecuteReader())
            {
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
        }

        return devices;
    }

    public Device GetDeviceById(string id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT Id, Name, IsEnabled FROM Device WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", int.Parse(id)); 

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Device
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString(),
                        IsTurnedOn = (bool)reader["IsEnabled"]
                    };
                }
                else
                {
                    throw new Exception("Device not found");
                }
            }
        }
    }

    public bool CreateDevice(Device device)
    {
        const string insertString = 
            "INSERT INTO Device (Name, IsEnabled) OUTPUT INSERTED.Id VALUES (@Name, @IsEnabled)";

        using (var connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(insertString, connection);
            command.Parameters.AddWithValue("@Name", device.Name);
            command.Parameters.AddWithValue("@IsEnabled", device.IsTurnedOn);

            connection.Open();
            var insertedId = command.ExecuteScalar(); // Get the generated Id

            if (insertedId != null)
            {
                device.Id = insertedId.ToString(); // Save generated Id back into your object
                return true;
            }

            return false;
        }
    }

    
    public bool UpdateDevice(Device device)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand(
                "UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id", connection);
            
            command.Parameters.AddWithValue("@Name", device.Name);
            command.Parameters.AddWithValue("@IsEnabled", device.IsTurnedOn);
            command.Parameters.AddWithValue("@Id", int.Parse(device.Id));

            return command.ExecuteNonQuery() > 0;
        }
    }

    public bool DeleteDevice(string id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("DELETE FROM Device WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", int.Parse(id));

            return command.ExecuteNonQuery() > 0;
        }
    }
}
