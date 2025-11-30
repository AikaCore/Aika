var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

configuration.AddJsonFile("aika-config.json");

var app = builder.Build();

app.Run();