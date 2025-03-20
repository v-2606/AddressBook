using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.EntityFrameworkCore;
using System.Text;

using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer;
using AutoMapper;
using BusinessLayer.AddressBookValidator;
using FluentValidation;
using BussinessLayer.Interface;
using BussinessLayer.Service;
using RepositoryLayer.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.DTO;
using StackExchange.Redis;
using RabbitMQLayer.Interface;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IAddressBookBL, AddressBookBL>();

builder.Services.AddScoped<IAddressBookRL, AddressBookRL>();
builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddScoped<IUserRL, UserRL>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IValidator<RegisterDTO>, RegisterValidator>();

builder.Services.AddAutoMapper(typeof(BusinessLayer.AutoMapperProfile.AutoMapperProfile));
builder.Services.AddValidatorsFromAssemblyContaining<AddressBookValidator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

//redis connection

var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddSingleton<RedisCacheService>();


// Database Connection

var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
builder.Services.AddDbContext<UserContext>(options => options.UseSqlServer(connectionString));


// RabbitMQ Dependencies (Event Publisher and Consumer)
builder.Services.AddSingleton<IEventPublisher, EventPublisher>();
builder.Services.AddHostedService<RabbitMQConsumer>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.UseRouting();

//Swagger Enable

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); //  Controllers ko register kar raha hai(UserController,v)
});


app.Run();
