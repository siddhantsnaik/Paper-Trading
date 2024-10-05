using Microsoft.Extensions.DependencyInjection;
using OnPaper_Auth.DependencyInj;
using OnPaper_Auth.Services;
using Serilog;

var AllowSpecificOrigins = "_AllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost",
                                              "http://localhost:1420")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

var log = new LoggerConfiguration()
    .Enrich.FromLogContext()    
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
FirebaseAppInj.ConfigureServices(builder.Services);

// Register AuthenticationService
builder.Services.AddTransient<AuthenticationService>();
builder.Services.AddTransient<UserDataServices>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(AllowSpecificOrigins);

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("IS DEV");
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
