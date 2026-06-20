using BuildingBlocks.Tenancy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTenancy();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseTenancy();

app.MapGet("/", () => "UserService running");

app.Run();