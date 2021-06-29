using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MsaasBackend.Hubs;
using MsaasBackend.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MsaasBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private const string ClientWebAllowSpecificOrigins = "clientWebAllowSpecificOrigins";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtOptions>(Configuration.GetSection("JwtSettings"));
            services.AddDbContext<DataContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("Msaas")));

            services.AddCors(options =>
            {
                options.AddPolicy(name: ClientWebAllowSpecificOrigins,
                    builder => { builder.WithOrigins("https://client_msaas.app.ncj.wiki/"); });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                    opt =>
                    {
                        var key = Encoding.UTF8.GetBytes(Configuration["JwtSettings:SigningKey"]);
                        opt.RequireHttpsMetadata = false;
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateAudience = false,
                            ValidateIssuer = false
                        };
                        opt.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                                // If the request is for hubs...
                                var path = context.HttpContext.Request.Path.Value;
                                path ??= "/";
                                if (!string.IsNullOrEmpty(accessToken) && path.Contains("/hubs"))
                                {
                                    // Read the token out of the query string
                                    context.Token = accessToken;
                                }

                                return Task.CompletedTask;
                            }
                        };
                    })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options => Configuration.Bind("CookieSettings", options));

            services.AddControllers();
            services.AddSignalR();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "MsaasBackend", Version = "v1"});
                c.CustomOperationIds(apiDesc =>
                    apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var baseUrl = Configuration.GetValue<string>("SubDirectory");

            app.UsePathBase(baseUrl);

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint(Path.Join(baseUrl, "swagger/v1/swagger.json"), "MsaasBackend v1"));

            if (Configuration["EnableHttps"] == "true")
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseCors(ClientWebAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/Hubs/Chat");
            });
        }
    }
}