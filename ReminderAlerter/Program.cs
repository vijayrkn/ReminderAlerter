using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddCors(options => options.AddPolicy("ApiCorsPolicy", corsBuilder =>
{
    corsBuilder.WithOrigins(builder.Configuration["SignalRClient"]).AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();
app.UseResponseCompression();
app.UseCors("ApiCorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapHub<ReminderHub>("/reminder");
await using var scope = app.Services?.GetService<IServiceScopeFactory>()?.CreateAsyncScope();
var context = scope?.ServiceProvider.GetRequiredService<IHubContext<ReminderHub>>();
app.MapGet("/broadcast", async (string reminder) =>
{
    await context!.Clients.All.SendAsync("ReceiveMessage", $"{DateTime.Now.ToString("MMMM dd H:mm:ss tt")}: {reminder}");
});

app.Run();

public class ReminderHub : Hub { }