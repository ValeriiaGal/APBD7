using APBD2;
using APBD2.Repositories;
using System.Text.Json.Nodes;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("UniversityDatabase");

builder.Services.AddTransient<IDeviceRepository, DeviceRepository>(_ => new DeviceRepository(connectionString));
builder.Services.AddTransient<IDeviceService, DeviceService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/devices", (IDeviceService service) =>
{
    return Results.Ok(service.GetAllDevices());
});

app.MapGet("/api/devices/{id}", (IDeviceService service, string id) =>
{
    try
    {
        var device = service.GetDeviceById(id);
        return Results.Ok(device);
    }
    catch (Exception ex)
    {
        return Results.NotFound($"Device with ID '{id}' was not found.");
    }
});


app.MapPost("/api/devices", async (HttpRequest request, IDeviceService service) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();

    try
    {
        var json = JsonNode.Parse(body);

        string deviceType = json?["deviceType"]?.ToString();
        string name = json?["name"]?.ToString();
        bool isTurnedOn = json?["isTurnedOn"]?.GetValue<bool>() ?? false;

        if (string.IsNullOrEmpty(deviceType) || string.IsNullOrEmpty(name))
            return Results.BadRequest("deviceType and name must be provided.");

        string newIdPrefix = deviceType.ToLower() switch
        {
            "smartwatch" => "SW-",
            "personalcomputer" => "P-",
            "embeddeddevice" => "ED-",
            _ => null
        };

        if (newIdPrefix == null)
            return Results.BadRequest("Invalid deviceType.");

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        string newId = await GenerateNextDeviceIdAsync(connection, deviceType);


        Device device;

        switch (deviceType.ToLower())
        {
            case "smartwatch":
                if (json?["battery"] is null)
                    return Results.BadRequest("Missing battery for smartwatch.");
                int battery = json["battery"].GetValue<int>();
                device = new Smartwatch { Id = newId, Name = name, Battery = battery };
                break;

            case "personalcomputer":
                string os = json?["operatingSystem"]?.ToString();
                if (string.IsNullOrEmpty(os))
                    return Results.BadRequest("Missing operatingSystem for PC.");
                device = new PersonalComputer { Id = newId, Name = name, OperatingSystem = os };
                break;

            case "embeddeddevice":
                string ip = json?["ipAddress"]?.ToString();
                string network = json?["networkName"]?.ToString();
                if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(network))
                    return Results.BadRequest("Missing IP or network name.");
                device = new EmbeddedDevice { Id = newId, Name = name, IpAddress = ip, NetworkName = network };
                break;

            default:
                return Results.BadRequest("Invalid deviceType.");
        }

        if (isTurnedOn) device.TurnOn();
        else device.TurnOff();

        try
        {
            if (service.CreateDevice(device))
                return Results.Created($"/api/devices/{device.Id}", device);

            return Results.BadRequest("Device creation failed.");
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }

    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Invalid JSON: {ex.Message}");
    }
})
.Accepts<string>("application/json");

app.MapPut("/api/devices", (IDeviceService service, Device device) =>
{
    if (service.UpdateDevice(device))
        return Results.Ok(device);

    return Results.NotFound();
});

app.MapDelete("/api/devices/{id}", (IDeviceService service, string id) =>
{
    if (service.DeleteDevice(id))
        return Results.Ok($"Device with ID '{id}' has been deleted.");

    return Results.NotFound($"Device with ID '{id}' does not exist.");
});


app.Run();

static async Task<string> GenerateNextDeviceIdAsync(SqlConnection connection, string deviceType)
{
    string prefix = deviceType.ToLower() switch
    {
        "smartwatch" => "SW-",
        "personalcomputer" => "P-",
        "embeddeddevice" => "ED-",
        _ => throw new Exception("Invalid deviceType")
    };

    var cmd = new SqlCommand(
        @"SELECT MAX(CAST(SUBSTRING(Id, LEN(@Prefix) + 1, LEN(Id)) AS INT)) 
      FROM Device 
      WHERE Id LIKE @Prefix + '%' AND ISNUMERIC(SUBSTRING(Id, LEN(@Prefix) + 1, LEN(Id))) = 1", connection);

    
    cmd.Parameters.AddWithValue("@Prefix", prefix);

    object result = await cmd.ExecuteScalarAsync();

    int next = (result != DBNull.Value && result != null) ? Convert.ToInt32(result) + 1 : 1;

    return $"{prefix}{next}";
}
