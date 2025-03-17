using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using PasswordKeeper.DAO;
using PasswordKeeper.DataAccess;

namespace PasswordKeeperServer;

/// <summary>
/// The ASP.NET Core entry point.
/// </summary>
public static class Program
{
    /// <summary>
    /// The entry point for the ASP.NET Core application. Sets up the web host, configures authentication and authorization,
    /// establishes the database context, and configures the HTTP request pipeline.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add JWT-bearer authentication and authorization
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Program.PseudoDomain,
                    ValidAudience = Program.PseudoDomain,
                    IssuerSigningKey = new SymmetricSecurityKey(JwtKey),
                };
            });

        // Add authorization with requiring authorization by default.
        // For routes without a [AllowAnonymous] attribute, the user must be authenticated.
        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
        
        _connectionString =
            builder.Configuration.GetValue<string>("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        // Add the database context
        builder.Services.AddDbContextFactory<Entities>(options => options.UseMySQL(_connectionString));
        
        builder.Services.AddSingleton<Users>();
        
        // Add services to the container
        builder.Services.AddControllers();

        // Add automapper DI
        builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }

    private static string? _connectionString;
    private static byte[]? _jwtKey;

    internal const string PseudoDomain = "password_keeper_server.com";
    
    /// <summary>
    /// A property to get the JWT key. If the database is empty, a new random key is generated and stored there.
    /// </summary>
    internal static byte[] JwtKey
    {
        get
        {
            // If the key is already in memory, return it
            if (_jwtKey == null)
            {
                // Check the database for the key
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
                connection.Open();
        
                using var command = connection.CreateCommand();
        
                command.CommandText = @"SELECT JwtSecurityKey FROM KeyData";
                var key = (string?)command.ExecuteScalar();
                if (key != null)
                {
                    // If the key is in the database, use it
                    _jwtKey = Convert.FromBase64String(key);
                    connection.Close();
                    return _jwtKey;
                }
                
                // If the key is not in the database, generate a new one
                var randomBytes = new byte[32];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomBytes);
                _jwtKey = randomBytes;

                // Store the key in the database
                using var insertKeyCommand = connection.CreateCommand();
                insertKeyCommand.CommandText = "INSERT INTO KeyData (JwtSecurityKey) VALUES (@key)";
                insertKeyCommand.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@key",
                    Value = Convert.ToBase64String(_jwtKey),
                    MySqlDbType = MySqlDbType.Text,
                });
                
                insertKeyCommand.ExecuteNonQuery();
                connection.Close();
            }    
            
            // Return the key
            return _jwtKey;
        }
    } 
}