using MeganUploadFiles.Authentication;
using MeganUploadFiles.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IStorageService, StorageService>();
//builder.Services.AddSingleton<IAppConfiguration, AppConfiguration>();
builder.Services.ConfigureJWT(true, "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAp3CVbF+TZDrO6IjeFIsbWhi4vjxkLrPy2ygWBbXse7ycd2daJrFXzKmlWECrSw7wbBcv4KO0PFVtb9s5PCJGmDZOZR02xY8DgUauU+S1EWEjmEdjeC8puEOPoM/YauNWWqAtJt8146WNTN07/UsvH7YZogKcmrl7PgmEMQtHhHYCWuoe+/27Mm4UffaIWIWrdVw970SFQ2PPAYns8j9NxWHMXUQZFewEKn1mw7RrByiP99PIOMoS7GtvnkWSls9VEXrsYelVjTEaXKiaKHYX6trzP4Zg9odOTMlULmeRLYs4lVCBC/lAg/BzM+3h/3I/1xLWy9S0sfwj3YCTboBL5wIDAQAB");
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen((c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebApi", Version = "v1" });
    //First we define the security scheme
    c.AddSecurityDefinition("Bearer", //Name the security scheme
        new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Type = SecuritySchemeType.Http, //We set the scheme type to http since we're using bearer authentication
            Scheme = JwtBearerDefaults.AuthenticationScheme //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
        });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = JwtBearerDefaults.AuthenticationScheme, //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },new List<string>()
                    }
                });
}));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowPort",
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowPort");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
