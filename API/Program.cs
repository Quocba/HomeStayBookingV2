﻿using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using API.Services;
using DataAccess.Context;
using DataAccess.Handlers;
using DataAccess.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using API.Middlewares;
using BusinessObject.Interfaces;
using DataAccess.Repositories;
using BusinessObject.Settings;
using DataAccess.EmailHandler;
using Microsoft.Extensions.FileProviders;
using PayOSService.Services;
using PayOSService.Config;
using API.BackgroundService;
using BusinessObject.Entities;
using BusinessObject.Shares;


var builder = WebApplication.CreateBuilder(args);
//bool checkPass = false;
//int count = 0;

//do
//{
//    var startUpKey = builder.Configuration["StartupKey:Key"];

//    Console.Write("Please enter startup key: ");
//    string pass = Console.ReadLine();

//    string hashedPass = Util.GenerateMD5(pass);

//    if (startUpKey.Equals(hashedPass))
//    {
//        Console.WriteLine("Startup API Success");
//        break; 
//    }
//    else
//    {
//        Console.WriteLine("Startup password incorrect");
//        count++;
//        if (count == 3)
//        {
//            Console.WriteLine("Too many incorrect attempts. Exiting...");
//            break;  
//        }
//        checkPass = true; 
//    }

//} while (checkPass);


// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IPayOSService, PayOSService.Services.PayOSService>();
builder.Services.AddScoped<TTLockService>();
builder.Services.Configure<PayOSConfig>(
    builder.Configuration.GetSection(PayOSConfig.ConfigName));

builder.Services.AddScoped<IGoogleLoginService, GoogleLoginService>();

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"))
    .AddPolicy("ManagerPolicy", policy =>
        policy.RequireRole("Manager"))
    .AddPolicy("UserPolicy", policy =>
        policy.RequireRole("User"));


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped<IRepository<Booking>, Repository<Booking>>();
builder.Services.AddScoped<IRepository<HomeStay>, Repository<HomeStay>>();
builder.Services.AddScoped<IRepository<Calendar>, Repository<Calendar>>();

builder.Services.AddSingleton<IHostedService, BookingService>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(MyAllowSpecificOrigins);


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "images")),
    RequestPath = "/images"
});
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ApiUserIdMiddleware>();
app.Run();
