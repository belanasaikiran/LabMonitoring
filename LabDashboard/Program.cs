using Microsoft.EntityFrameworkCore;
using LabServer;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LabContext>();

var app = builder.Build();

// Requirement Check: Dynamic Dashboard / Web-based solutions
// This endpoint returns the latest health status of all machines
app.MapGet("/api/status", async (LabContext db) =>
{
    var latestLogs = await db.Logs.GroupBy(l => l.MacAddress).Select(g => g.OrderByDescending(x => x.TimeStamp).FirstOrDefault()).ToListAsync();
    return Results.Ok(latestLogs);
});

// Endpoint to get historical data for a specific Machine
app.MapGet("/api/history/{mac}", async (string mac, LabContext db) =>
{
    var decodedMac = Uri.UnescapeDataString(mac);
    var history = await db.Logs.Where(l => l.MacAddress == decodedMac).OrderByDescending(x => x.TimeStamp).Take(10).ToListAsync();
    return Results.Ok(history);
});

// Endpoint to get historical data for a specific Machine
app.MapGet("/api/history", async (LabContext db) =>
{
    var history = await db.Logs.OrderByDescending(x => x.TimeStamp).Take(10).ToListAsync();
    return Results.Ok(history);
});

app.Run();

