using System.Text;
using HappyTailBackend.Data;
using HappyTailBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


var builder = WebApplication.CreateBuilder(args);
// MySQL DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("DB CONNECTION: " + connectionString);

// Controllers
builder.Services.AddControllers();



// Inject password from env var
var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
Console.WriteLine("DB PASSWORD: " + password);
if (!string.IsNullOrEmpty(password))
{
    var builderConn = new MySqlConnector.MySqlConnectionStringBuilder(connectionString);
    builderConn.Password = password;
    connectionString = builderConn.ConnectionString;
}



builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 36))
    )
);







// Auth service
builder.Services.AddScoped<AuthService>();






// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token ONLY"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});




// -------------------- BUILD APP --------------------

var app = builder.Build();



// -------------------- DB CONNECTION CHECK --------------------

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();

    if (db.Database.CanConnect())
        Console.WriteLine(" MySQL DB connected successfully on port 3306");
    else
        Console.WriteLine(" MySQL DB connection failed");
}

// -------------------- MIDDLEWARE --------------------

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
