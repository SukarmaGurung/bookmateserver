using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server1.Models;
using server1.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS Configuration - Allow frontend to access backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // your frontend URL
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var config = builder.Configuration;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
        };
    });

// Add authorization services
builder.Services.AddAuthorization();

builder.Services.AddControllers().AddApplicationPart(typeof(BookController).Assembly);


builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<CloudinaryService>();


// Add email services
builder.Services.AddScoped<IEmailService, EmailService>();


// Add controllers (API)
builder.Services.AddControllers();

// Add Swagger with Bearer token support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token in this format: Bearer {your token here}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Get the database context
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Check if any categories exist
    if (!dbContext.Categories.Any())
    {
        // Create the list of system categories
        var systemCategories = new List<Category>
        {
            new Category { Name = "All Books", Description = "All available books", Type = "System" },
            new Category { Name = "Bestsellers", Description = "Top selling books", Type = "System" },
            new Category { Name = "Award Winners", Description = "Books that have won awards", Type = "System" },
            new Category { Name = "New Releases", Description = "Books published in last 3 months", Type = "System" },
            new Category { Name = "New Arrivals", Description = "Recently added books", Type = "System" },
            new Category { Name = "Coming Soon", Description = "Upcoming books", Type = "System" },
            new Category { Name = "Deals", Description = "Books on sale", Type = "System" }
        };

        // Add them to database
        await dbContext.Categories.AddRangeAsync(systemCategories);
        await dbContext.SaveChangesAsync();
    }
}

// Enable CORS
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware for authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers (routes)
app.MapControllers();

// Run the application
app.Run();
