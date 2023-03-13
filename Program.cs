global using Projects.Models;
global using Projects.DTOS.Characters;
global using Projects.DTOS.Users;
global using Projects.Services.CharacterService;
global using AutoMapper;
global using Microsoft.EntityFrameworkCore;
global using Projects.Data;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Swashbuckle.AspNetCore.Filters;
global using Microsoft.OpenApi.Models;
global using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connection = builder.Configuration.GetConnectionString( "DefaultConnection" );
builder.Services.AddDbContext<DataContext>(
    options => options.UseMySql( connection, ServerVersion.AutoDetect( connection ))
);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c => {
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
        Description = """Standard Authorization header using the Bearer scheme. Example "bearer {token}" """,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper( typeof( Program ).Assembly );
builder.Services.AddScoped<ICharacterService, CharacterService>(); // this line inject the service to all the controllers
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddAuthentication( JwtBearerDefaults.AuthenticationScheme )
    .AddJwtBearer( options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes( builder.Configuration.GetSection( "AppSettings:Token" ).Value! )
            ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();