using APBD2;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var deviceManager = new DeviceManager();

// GET: All devices 
app.MapGet("/devices", () =>
{
    var all = deviceManager.GetAllDevices();
    return Results.Ok(all.Select(d => new { d.Id, d.Name, Type = d.GetType().Name }));
});

// GET: Device by ID 
app.MapGet("/devices/{id:int}", (int id) =>
{
    try
    {
        var device = deviceManager.GetDeviceById(id);
        return Results.Ok(device);
    }
    catch
    {
        return Results.NotFound("Device not found");
    }
});

// POST: Add device
app.MapPost("/devices", (Device device) =>
{
    deviceManager.AddDevice(device);
    return Results.Created($"/devices/{device.Id}", device);
});

// PUT: Update device
app.MapPut("/devices/{id:int}", (int id, Dictionary<string, object> updates) =>
{
    try
    {
        foreach (var kv in updates)
        {
            deviceManager.EditDeviceData(id, kv.Key, kv.Value);
        }

        return Results.Ok(deviceManager.GetDeviceById(id));
    }
    catch
    {
        return Results.NotFound("Device not found");
    }
});

// DELETE: Remove device
app.MapDelete("/devices/{id:int}", (int id) =>
{
    try
    {
        deviceManager.RemoveDevice(id);
        return Results.Ok();
    }
    catch
    {
        return Results.NotFound("Device not found");
    }
});

app.Run();