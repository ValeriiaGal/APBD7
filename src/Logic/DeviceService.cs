namespace APBD2;
using Microsoft.Data.SqlClient;


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
                command.Parameters.AddWithValue("@Id", id);

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
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO Device (Id, Name, IsEnabled) VALUES (@Id, @Name, @IsEnabled)", connection);

                device.Id = Guid.NewGuid().ToString(); 

                command.Parameters.AddWithValue("@Id", device.Id);
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@IsEnabled", device.IsTurnedOn);

                return command.ExecuteNonQuery() > 0;
            }
        }


        public bool UpdateDevice(Device device)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", device.Id);
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@IsEnabled", device.IsTurnedOn);

                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteDevice(string id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Device WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                return command.ExecuteNonQuery() > 0;
            }
        }
    }
