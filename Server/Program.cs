using BaseLibrary.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Repositories.Implementations;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get JWT section from app settings
builder.Services.Configure<JwtSection>(builder.Configuration.GetSection("JwtSection"));
var jwtSection = builder.Configuration.GetSection(nameof(JwtSection)).Get<JwtSection>();

// on startup
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Error: Couldn't Find your connection string"));
});

// add authentication builder
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSection!.Issuer,
        ValidAudience = jwtSection.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection.Key!)),
    };
});

// add service implementations
builder.Services.AddScoped<IUserAccount, UserAuthenticationRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<ApplicationUser>, UserRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<Bank>, BankRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<UserTransactionAccount>, UserTransactionAccountRepository>();
builder.Services.AddScoped<ITransactionListInterface, TransactionListRepository>();

// add cors and endpoints
builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowBlazorWasm",
    builder => builder
    .WithOrigins("https://localhost:7168", "http://localhost:5139") // specify client end points
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorWasm"); // BlazorWasm

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
