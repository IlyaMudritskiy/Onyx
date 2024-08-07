using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Onyx.Models.Domain;
using Onyx.Models;
using Onyx.Repositories;
using Onyx.Services.Validators;
using Onyx.Services;
using System.Text;
using FluentValidation.AspNetCore;
using Onyx.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

namespace Onyx.Startup
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterMVC(services);
            RegisterDatabases(services, configuration);
            RegisterMappers(services);
            RegisterRepositories(services);
            RegisterValidators(services);
            RegisterAuthentication(services, configuration);
            RegisterSignalR(services);

            return services;
        }

        private static void RegisterMVC(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Onyx API",
                    Version = "v1",
                });

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            },
                            Scheme = "Oauth2",
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }

        private static void RegisterDatabases(IServiceCollection services, IConfiguration configuration)
        {
            // Mongo Db
            services.Configure<MongoDBSettings>(configuration.GetSection("MDBProcessDataConnection"));
            services.AddSingleton<MDBProcessDataService>();

            // SQL Server
            services.AddDbContext<UserIdentitiesContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("SQLSUsersAuthTest"))
            );
        }

        private static void RegisterMappers(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ProcessDataMapper));
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IProcessDataRepository, MongoProcessDataRepository>();
            services.AddScoped<IAuthRepository, UserAuthRepository>();
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddScoped<IValidator<DUTHeaderModel>, DUTHeaderModelValidator>();
            services.AddScoped<IValidator<NewProcessDataModel>, NewProcessDataModelValidator>();
            services.AddScoped<IValidator<ProcessDataModel>, ProcessDataModelValidator>();
        }

        private static void RegisterAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityCore<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("UsersAuth")
                .AddEntityFrameworkStores<UserIdentitiesContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
                    )
                });
        }

        private static void RegisterSignalR(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("localhost") // Replace with your client's origin
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }
    }
}
