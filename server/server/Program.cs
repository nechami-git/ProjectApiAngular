using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using server.BLL;
using server.BLL.Intarfaces;
using server.DAL;
using server.DAL.Interfaces;
using server.Middleware;
using server.Models.DTO;
using System.Text;
using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();


// Add services to the container.
//builder.Services.AddDbContext<ChineseSaleContext>(option => option.UseSqlServer("Data Source = srv2\\pupils; Initial Catalog = Chinese_sale_327738688; Integrated Security = True; Trust Server Certificate=True"));
// builder.Services.AddDbContext<ChineseSaleContext>(option =>
//     option.UseInMemoryDatabase("ChineseSaleDb"));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("CRITICAL ERROR: Connection string 'DefaultConnection' not found!");
}

builder.Services.AddDbContext<ChineseSaleContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        builder =>
        {
            builder.WithOrigins("http://localhost:8080", "http://localhost:4200") 
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .WithExposedHeaders("Content-Disposition");
        });
});


//user
builder.Services.AddScoped<IUserDal, UserDal>();
builder.Services.AddScoped<IUserBLL, UserBLL>();

//donor
builder.Services.AddScoped<IDonorDAL, DonorDAL>();
builder.Services.AddScoped<IDonorBLL, DonorBLL>();

//category
builder.Services.AddScoped<ICategoryDAL, CategoryDAL>();
builder.Services.AddScoped<ICategoryBLL, CategoryBLL>();

//gift
builder.Services.AddScoped<IGiftDAL, GiftDAL>();
builder.Services.AddScoped<IGiftBLL, GiftBLL>();

//ticket
builder.Services.AddScoped<ITicketDAL, TicketDAL>();
builder.Services.AddScoped<IPurchaseBLL, PurchaseBLL>();

//cart
builder.Services.AddScoped<ICartBLL, CartBLL>();
//email
builder.Services.AddScoped<IEmailBLL, EmailBLL>();

//token
builder.Services.AddScoped<ITokenBLL, TokenBLL>();




builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Swagger - add a named doc
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chinese Sale", Version = "v1" });
    // �����: ����� ������� �-JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // �����: ����� ��� ������� ����� �� ������ ���
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});



//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        var keyString = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(keyString))
            throw new InvalidOperationException("Jwt:Key is not configured.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString))
        };
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chinese Sale V1");
        c.RoutePrefix = "swagger"; // default; index will be at /swagger/index.html
    });
}


app.UseHttpsRedirection();

app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseCors("AllowAngular");

app.UseAuthentication();

app.UseAuthorization();
app.UseDefaultFiles(); 
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ChineseSaleContext>();
        context.Database.EnsureCreated(); // יוצר את ה-DB והטבלאות אם הם חסרים
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}
app.Run();
