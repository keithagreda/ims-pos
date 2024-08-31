using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PMSIMSWebApi;
using PMSIMSWebApi.Entities;
using POSIMSWebApi.Auth;
using POSIMSWebApi.AuthServices;
using POSIMSWebApi.Interceptors;
using POSIMSWebApi.Interfaces;
using POSIMSWebApi.Mapping;
using POSIMSWebApi.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder
    .Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.AddSerilog(logger);

// Add services to the container.
builder.Services.AddDbContext<POSIMSDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);
builder
    .Services.AddIdentity<ApplicationIdentityUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider("userIdentity", typeof(DataProtectorTokenProvider<ApplicationIdentityUser>));

builder
    .Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(option =>
    {
        option.SaveToken = false;

        option.RequireHttpsMetadata = false;

        option.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,

            ValidateAudience = true,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            ClockSkew = TimeSpan.Zero,

            ValidAudience = builder.Configuration["JWT:ValidAudience"],

            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!)
            )
        };
    });

builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IProductPricesServices, ProductPricesServices>();
builder.Services.AddScoped<IUserAuthenticationServices, UserAuthenticationServices>();
builder.Services.AddScoped<SoftDeleteInterceptor>();
builder.Services.AddTransient<ProductMapper>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<AppExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler(_ => { });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
