using APBD2;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("UniversityDatabase");

builder.Services.AddTransient<IDeviceService, DeviceService>(_ => new DeviceService(connectionString));

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

// GET all devices
app.MapGet("/api/devices", (IDeviceService service) =>
{
    return Results.Ok(service.GetAllDevices());
});

// GET device by ID
app.MapGet("/api/devices/{id}", (IDeviceService service, string id) =>
{
    try
    {
        var device = service.GetDeviceById(id);
        return Results.Ok(device);
    }
    catch
    {
        return Results.NotFound();
    }
});

// POST (create) a new device (dynamic, based on deviceType)
app.MapPost("/api/devices", async (IDeviceService service, HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();

    var json = JsonNode.Parse(body);

    string deviceType = json?["deviceType"]?.ToString();
    string name = json?["name"]?.ToString();
    bool isTurnedOn = json?["isTurnedOn"]?.GetValue<bool>() ?? false;

    if (string.IsNullOrEmpty(deviceType) || string.IsNullOrEmpty(name))
        return Results.BadRequest("deviceType and name must be provided.");

    Device device;

    switch (deviceType.ToLower())
    {
        case "smartwatch":
            int battery = json?["battery"]?.GetValue<int>() ?? 0;
            device = new Smartwatch
            {
                Name = name,
                Battery = battery
            };
            break;

        case "personalcomputer":
            string os = json?["operatingSystem"]?.ToString() ?? "Unknown OS";
            device = new PersonalComputer
            {
                Name = name,
                OperatingSystem = os
            };
            break;

        case "embeddeddevice":
            string ip = json?["ipAddress"]?.ToString() ?? "0.0.0.0";
            string network = json?["networkName"]?.ToString() ?? "Unknown Network";
            device = new EmbeddedDevice
            {
                Name = name,
                IpAddress = ip,
                NetworkName = network
            };
            break;

        default:
            return Results.BadRequest("Invalid deviceType.");
    }

    if (isTurnedOn) device.TurnOn();
    else device.TurnOff();

    if (service.CreateDevice(device))
        return Results.Created($"/api/devices/{device.Id}", device);

    return Results.BadRequest();
});


app.MapPut("/api/devices", (IDeviceService service, Device device) =>
{
    if (service.UpdateDevice(device))
        return Results.Ok(device);

    return Results.NotFound();
});


app.MapDelete("/api/devices/{id}", (IDeviceService service, string id) =>
{
    if (service.DeleteDevice(id))
        
        return Results.Ok();

    return Results.NotFound();
});

app.Run();
