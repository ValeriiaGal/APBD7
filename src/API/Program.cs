using APBD2;


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
    catch
    {
        return Results.NotFound();
    }
});

app.MapPost("/api/devices", (IDeviceService service, Device device) =>
{
    if (string.IsNullOrEmpty(device.Name))
        return Results.BadRequest("Device name cannot be empty");

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
