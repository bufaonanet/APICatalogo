using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Filters;
using APICatalogo.Logging;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace APICatalogo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("SqliteConnectionString"))
            );

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            //JWT
            //Adiciona o manipulador de autenticao e define
            //o esuqema de autenticacao usando: Bearer
            //balida o emissor, e audiencia e a chave
            //usando a chave secreta valida e assinatura
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = Configuration["TokenConfiguration:Issuer"],
                    ValidAudience = Configuration["TokenConfiguration:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddTransient<IMeuServico, MeuServico>();
            services.AddScoped<ApiLoggingFilter>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddCors();

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "CatalogoAPI",
                    Description = "Catálogo de Produtos e Categorias",
                    TermsOfService = new Uri("https://github.com/bufaonanet/CleanArchMvc"),
                    Contact = new OpenApiContact
                    {
                        Name = "douglas",
                        Email = "douglas.bufaonanet@outlook.com",
                        Url = new Uri("https://github.com/bufaonanet/CleanArchMvc"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Usar sobre LICX",
                        Url = new Uri("https://github.com/bufaonanet/CleanArchMvc"),
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    //definir configuracoes
                    In = ParameterLocation.Header,
                    Description = "Copiar 'bearer' + 'token'",
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
                          new string[] {}
                      }
                });
            });

            services.AddControllers()
                .AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy());           
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
            {
                LogLevel = LogLevel.Information
            }));

            // Use odata route debug, /$odata
            app.UseODataRouteDebug();

            // Add OData /$query middleware
            app.UseODataQueryRequest();

            //Adicionando o middleware de tratamento de erros global
            app.ConfigureExceptionHandler();

            //Adiciona o moddleware para redirecionar p ara https
            app.UseHttpsRedirection();

            //Adiciona o middleware de roteamento
            app.UseRouting();

            //Middlewares de autenticação e autorização
            app.UseAuthentication();
            app.UseAuthorization();

            //Habilira o middleware para servir o Swagger
            //gerado como um endpoint  JSON
            app.UseSwagger();

            //Registra o gerador Swagger definindo um ou mais documentos Swagger
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "Cátalogo de Produtos e Categorias");
            });

            app.UseCors(opt =>
                opt.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            //Adiciona o middleare que executa o endpoint do request atual
            app.UseEndpoints(endpoints =>
            {
                //Adiciona os endpoints pra as Actions
                //dos controladores sem especificar rotas
                endpoints.MapControllers();
            });
        }
    }
}