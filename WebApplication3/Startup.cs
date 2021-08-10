using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApplication3.DbBase;
using WebApplication3.Helper;
using WebApplication3.Models;
using ErrorContext = Newtonsoft.Json.Serialization.ErrorContext;

namespace WebApplication3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LotteryDb>(option =>
                option.UseMySql(Configuration.GetConnectionString("DotNetCoreMySQLAppConnection"),
                    new MySqlServerVersion(new Version(5, 7, 26))).UseLoggerFactory(LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                })).EnableSensitiveDataLogging());
            services.Configure<BookstoreDatabaseSettings>(Configuration.GetSection("BookstoreDatabaseSettings"));
            services.AddSingleton<IBookstoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<BookstoreDatabaseSettings>>()
                    .Value); //接口解析的时候带上 bookstoredatabasesetting实例的值
            services.AddSingleton<BookService>(); //此处bookservice 实例化是后台
            //services.AddTransient<ErrorMiddleware>();

            services.AddControllers().AddNewtonsoftJson(op =>
                op.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
            //Newtonsoft 设置
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter());
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                return settings;
            };
          
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VV";
                options.SubstituteApiVersionInUrl = true;
            });
            
            //services.AddControllers();
         

            // services.AddSwaggerGen(c =>
            // {
            //     // //c.SwaggerDoc("v1", new OpenApiInfo {Title = "WebApplication3", Version = "v1"});
            //     // //c.AddServer(new OpenApiServer() { Url = "http://localhost:5010", Description = "地址1" });
            //     // // c.SwaggerDoc("v1.0", new Info
            //     // // {
            //     // //     Title = "My APIs",
            //     // //     Version = "v1.0",
            //     // //     Description = "REST APIs "
            //     // // });
            //     // c.SwaggerDoc("v1", new OpenApiInfo
            //     // {
            //     //     Version = "v1",
            //     //     Title = "ToDo API",
            //     //     Description = "A simple example ASP.NET Core Web API",
            //     //     TermsOfService = new Uri("https://example.com/terms"),
            //     //     Contact = new OpenApiContact
            //     //     {
            //     //         Name = "Shayne Boyer",
            //     //         Email = string.Empty,
            //     //         Url = new Uri("https://twitter.com/spboyer"),
            //     //     },
            //     //     License = new OpenApiLicense
            //     //     {
            //     //         Name = "Use under LICX",
            //     //         Url = new Uri("https://example.com/license"),
            //     //     }
            //     // });
            //     //
            //     // c.SwaggerDoc("v2", new OpenApiInfo
            //     // {
            //     //     Version = "v2",
            //     //     Title = "test2",
            //     //     Description = "test2",
            //     //     TermsOfService = new Uri("https://example.com/terms"),
            //     //     Contact = new OpenApiContact
            //     //     {
            //     //         Name = "helo",
            //     //         Email = string.Empty,
            //     //         Url = new Uri("https://twitter.com/spboyer"),
            //     //     },
            //     //     License = new OpenApiLicense
            //     //     {
            //     //         Name = "Use under LICX",
            //     //         Url = new Uri("https://example.com/license"),
            //     //     }
            //     // });
            //  
            //     var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            //     foreach (var description in provider.ApiVersionDescriptions)
            //     {
            //         c.SwaggerDoc(
            //             description.GroupName,
            //             new OpenApiInfo
            //             {
            //                 Title = "AgileTea Learning API",
            //                 Version = description.ApiVersion.ToString()
            //             });
            //     }
            //
            //     var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //     var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //     c.IncludeXmlComments(xmlPath);  
            // });
            //
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenConfigurationOptions>();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
          
            if (env.IsDevelopment())
            { 
                app.UseExceptionHandler("/error-local-development");
               //app.UseDeveloperExceptionPage();
                //  app.UseDeveloperExceptionPage();
                // void Action(IApplicationBuilder builder) =>
                //     builder.Run(async context =>
                //     {
                //         context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                //         context.Response.ContentType = "application/json";
                //
                //         var exception = context.Features.Get<IExceptionHandlerFeature>();
                //         if (exception != null)
                //         {
                //             var error = new ReturnData<int>(500,exception.Error.Message,new List<int>());
                //             var errObj = JsonConvert.SerializeObject(error);
                //
                //             await context.Response.WriteAsync(errObj).ConfigureAwait(false);
                //         }
                //     });
                // //
                // app.UseExceptionHandler(Action);
                // app.UseExceptionHandler(appBuilder =>
                // {
                //     appBuilder.Use(async (ctx, next) =>
                //     {
                //         ctx.Request.Path = "/error-local-development";
                //         await next();
                //     });
                // });   
                //app.UseExceptionHandler("/error");
                //app.UseExceptionHandler("/error");
                //app.UseHsts();

               // options.OperationFilter<AddApiVersionExampleValueOperationFilter>();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"../swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("v2/swagger.json", "WebApplication3 v2"));

            }
            //app.UseMiddleware<ErrorMiddleware>();
            //app.UseStatusCodePages();
           // app.UseRequestError();
           //app.UseRequestError();
          
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
         

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }

    public class Info : OpenApiInfo
    {
    }


    public class ErrorMessage
    {
        public string Message { get; set; }
        public string Stacktrace { get; set; }
    }


    public class ErrorMiddleware:IMiddleware
    {

        // IMyScopedService is injected into Invoke
        public async Task InvokeAsync(HttpContext httpContext,RequestDelegate next)
        {
            httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var exception = httpContext.Features.Get<IExceptionHandlerFeature>();
            Console.WriteLine(exception);
            if (exception != null)
            {
                var error = new ErrorMessage()
                {
                    Stacktrace = exception.Error.StackTrace,
                    Message = exception.Error.Message
                };
                var errObj = JsonConvert.SerializeObject(error);

                await httpContext.Response.WriteAsync(errObj).ConfigureAwait(false);
            }

            await next(httpContext);
        }

      
    }
    
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestError(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorMiddleware>();
        }
    }
    
    /*
     * 加载swagger 配置
     */
    public class SwaggerGenConfigurationOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        public SwaggerGenConfigurationOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title = "AgileTea Learning API",
                        Version = description.ApiVersion.ToString()
                    });
            }
        }
    }
    
    public class AddApiVersionExampleValueOperationFilter : IOperationFilter
    {
        private const string ApiVersionQueryParameter = "api-version";
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiVersionParameter = operation.Parameters.SingleOrDefault(p => p.Name == ApiVersionQueryParameter);
            if (apiVersionParameter == null)
            {
                // maybe we should warn the user if they are using this filter without applying the QueryStringApiVersionReader as the ApiVersionReader
                return;
            }
            // get the [ApiVersion("VV")] attribute
            var attribute = context?.MethodInfo?.DeclaringType?
                .GetCustomAttributes(typeof(ApiVersionAttribute), false)
                .Cast<ApiVersionAttribute>()
                .SingleOrDefault();
            // extract the value of the api version
            var version = attribute?.Versions?.SingleOrDefault()?.ToString();
            // may be we should warn if we find un-versioned ApiControllers/ operations?
            if (version != null)
            {
                apiVersionParameter.Example = new OpenApiString(version);
                apiVersionParameter.Schema.Example = new OpenApiString(version);
            }
        }
    }
}