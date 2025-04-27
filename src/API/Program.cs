using APBD2;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var deviceManager = new DeviceManager();

// get all devices
app.MapGet("/devices", () =>
{
    var all = deviceManager.GetAllDevices();
    return Results.Ok(all.Select(d => new { d.Id, d.Name, Type = d.GetType().Name }));
});

// get device by id
app.MapGet("/devices/{id}", (string id) =>
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

// Add Smartwatch
app.MapPost("/devices/smartwatches", (Smartwatch device) =>
{
    deviceManager.AddDevice(device);
    return Results.Created($"/devices/{device.Id}", device);
});

// Add Personal Computer
app.MapPost("/devices/personalcomputers", (PersonalComputer device) =>
{
    deviceManager.AddDevice(device);
    return Results.Created($"/devices/{device.Id}", device);
});

// Add Embedded Device
app.MapPost("/devices/embeddeddevices", (EmbeddedDevice device) =>
{
    deviceManager.AddDevice(device);
    return Results.Created($"/devices/{device.Id}", device);
});


// Edit
app.MapPut("/devices/smartwatches/{id}", (string id, Smartwatch updatedDevice) =>
{
    try
    {
        deviceManager.EditDeviceData(id, "Name", updatedDevice.Name);
        deviceManager.EditDeviceData(id, "Battery", updatedDevice.Battery);
        deviceManager.EditDeviceData(id, "IsTurnedOn", updatedDevice.IsTurnedOn);
        return Results.Ok(deviceManager.GetDeviceById(id));
    }
    catch
    {
        return Results.NotFound("Smartwatch not found");
    }
});

app.MapPut("/devices/personalcomputers/{id}", (string id, PersonalComputer updatedDevice) =>
{
    try
    {
        deviceManager.EditDeviceData(id, "Name", updatedDevice.Name);
        deviceManager.EditDeviceData(id, "OperatingSystem", updatedDevice.OperatingSystem);
        deviceManager.EditDeviceData(id, "IsTurnedOn", updatedDevice.IsTurnedOn);
        return Results.Ok(deviceManager.GetDeviceById(id));
    }
    catch
    {
        return Results.NotFound("PC not found");
    }
});

app.MapPut("/devices/embeddeddevices/{id}", (string id, EmbeddedDevice updatedDevice) =>
{
    try
    {
        deviceManager.EditDeviceData(id, "Name", updatedDevice.Name);
        deviceManager.EditDeviceData(id, "IpAddress", updatedDevice.IpAddress);
        deviceManager.EditDeviceData(id, "NetworkName", updatedDevice.NetworkName);
        deviceManager.EditDeviceData(id, "IsTurnedOn", updatedDevice.IsTurnedOn);
        return Results.Ok(deviceManager.GetDeviceById(id));
    }
    catch
    {
        return Results.NotFound("Embedded device not found");
    }
});


// DELETE
app.MapDelete("/devices/{id}", (string id) =>
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger"));
//test

app.MapControllers();

app.Run();
