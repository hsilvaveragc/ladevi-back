using System.Net;
using System.Text;
using Microsoft.AspNetCore.Identity;
using LadeviVentasApi.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Diagnostics;
using LadeviVentasApi.Extensions.Startup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using LadeviVentasApi.Services.Xubio;
using LadeviVentasApi.Helpers;

namespace LadeviVentasApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting process {0}", Process.GetCurrentProcess().Id);

            // Crear el builder de la aplicación
            var builder = WebApplication.CreateBuilder(args);

            // Detectar si estamos en modo testing
            var isTesting = args.Contains("--environment=Testing") ||
                            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing";

            if (isTesting)
            {
                builder.Environment.EnvironmentName = "Testing";
            }

            // Seteo la configuración de la aplicación
            builder.SetupConfiguration();

            ConfigureWebApplication(builder);
            var app = builder.Build();
            RunMigration(builder, app);
            ConfigureMiddleware(app);
            app.Run();
        }

        private static void ConfigureWebApplication(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<HttpLoggingHandler>();
            builder.Services.AddHttpClient()
                            .AddHttpClient("WsHttpClient")
                            .AddHttpMessageHandler<HttpLoggingHandler>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
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
                    ValidIssuer = builder.Configuration["Issuer"],
                    ValidAudience = builder.Configuration["Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"]))
                };
            });

            var isIntegrationTesting = builder.Environment.EnvironmentName == "Testing";
            var ciCdMode = Environment.GetEnvironmentVariable("CI_CD_MODE");
            var connectionString = builder.Configuration["DefaultConnection"];


            if (isIntegrationTesting && ciCdMode != "true")
            {
                connectionString = connectionString.Replace("Database=", "Database=testing_");
            }

            builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseNpgsql(connectionString, o => o.CommandTimeout(Convert.ToInt32(TimeSpan.FromMinutes(10).TotalSeconds))));

            //migramos automaticamente la db
            if (isIntegrationTesting)
            {
                builder.Services.AddTransient<Controllers.ApplicationUsersController>();
                builder.Services.AddTransient<Controllers.CountryController>();
                builder.Services.AddTransient<Controllers.StateController>();
                builder.Services.AddTransient<Controllers.DistrictController>();
                builder.Services.AddTransient<Controllers.CityController>();
            }

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders();

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // Configuración de CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                );
            });

            if (!isIntegrationTesting)
            {
                builder.Services.AddTransient<IEmailSender, EmailSender>(i =>
                    new EmailSender(
                        builder.Configuration["EmailSenderHost"],
                        builder.Configuration.GetValue<int>("EmailSenderPort"),
                        builder.Configuration.GetValue<bool>("EmailSenderEnableSSL"),
                        builder.Configuration["EmailSenderUserName"],
                        builder.Configuration["EmailSenderPassword"]
                    )
                );
            }
            else
            {
                builder.Services.AddTransient<IEmailSender, MockEmailSender>();
            }

            builder.Services.AddHttpContextAccessor();

            CultureInfo customCulture = new CultureInfo("es-ES");
            customCulture.NumberFormat.NumberDecimalSeparator = ",";
            customCulture.NumberFormat.CurrencyDecimalSeparator = ",";
            customCulture.NumberFormat.NumberGroupSeparator = ".";
            customCulture.NumberFormat.CurrencyGroupSeparator = ".";
            CultureInfo[] supportedCultures = new[]
            {
                customCulture
            };

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(customCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };
            });

            builder.Services.Configure<Configuration>(builder.Configuration.GetSection("AppKeys"));

            builder.Services.AddControllers()
                            .AddNewtonsoftJson(options =>
                            {
                                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error;
                                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                                {
                                    NamingStrategy = new CamelCaseNamingStrategy()
                                };
                                options.SerializerSettings.Culture = customCulture; //CultureInfo.CurrentCulture;
                            });


            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ladevi Ventas API", Version = "v1" });
                c.OperationFilter<SwaggerAuthHeaderOperation>();
            });

            builder.Services.AddSingleton<XubioService>();
        }

        private static void RunMigration(WebApplicationBuilder builder, WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    using var context = (DbContext)scope.ServiceProvider.GetService(typeof(ApplicationDbContext));
                    if (builder.Environment.EnvironmentName == "Testing")
                    {
                        Console.WriteLine($"{DateTime.UtcNow:O} | Integration testing mode: the database will be dropped and recreated.");
                        var connectionString = builder.Configuration["DEFAULTCONNECTION"];
                        Console.WriteLine($"{DateTime.UtcNow:O} | target connstr: {connectionString}");
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();  // ✅ Esto crea TODO basado en el modelo actual
                    }
                    else
                    {

                        Console.WriteLine($"{DateTime.UtcNow:O} | Migrations will run now! Context: {context.GetType().Name}");
                        var connectionString = builder.Configuration["DEFAULTCONNECTION"];
                        Console.WriteLine($"{DateTime.UtcNow:O} | target connstr: {connectionString}");
                        context.Database.SetCommandTimeout(TimeSpan.FromHours(2));
                        context.Database.Migrate();
                        Console.WriteLine($"{DateTime.UtcNow:O} | Migrations run ok!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            app.UseRequestLocalization();

            //System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var error = context.Features.Get<IExceptionHandlerFeature>() as ExceptionHandlerFeature;
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    if (error.Error != null)
                    {
                        await context.Response.WriteAsync(new ErrorDetails
                        {
                            Errors = new Dictionary<string, string> { { "error", error.Error.Message } }
                        }.ToString());
                    }
                    else
                    {
                        await context.Response.WriteAsync(new ErrorDetails
                        {
                            Errors = new Dictionary<string, string> { { "error", "unhandled" } }
                        }.ToString());
                    }
                });
            });

            if (!app.Environment.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors();
            app.UseRouting();

            // Configuración de Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ladevi Ventas API V1");
            });

            // Solo usar HTTPS redirection en producción
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            Console.WriteLine("=======================STARTING=======================");
            app.MapGet("/", () => Results.Redirect("/swagger/index.html"));
        }

        public class ErrorDetails
        {
            public Dictionary<string, string> Errors { get; set; }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
        }
    }

    public class MockEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"Mock Email sent to {email}: {subject}");
            return Task.CompletedTask;
        }
    }
}
