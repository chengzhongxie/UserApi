using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using User.API.Data;
using User.API.Dtos;
using User.API.Filters;
using DotNetCore.CAP;

namespace User.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UserContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.Configure<ServiceDisvoveryOptions>(Configuration.GetSection("ServiceDiscovery"));// json 配置文件映射到实体类

            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDisvoveryOptions>>().Value;

                if (!string.IsNullOrEmpty(serviceConfiguration.Consul.HttpEndpoint))
                {
                    // if not configured, the client will use the default value "127.0.0.1:8500"
                    cfg.Address = new Uri(serviceConfiguration.Consul.HttpEndpoint);
                }
            }));

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = "user_api";
                    options.Authority = "http://localhost:8080";
                });

            services.AddCap(options =>
            {
                options.UseEntityFramework<UserContext>()
                .UseRabbitMQ("localhost")
                .UseDashboard()
                .UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = "localhost";
                    d.DiscoveryServerPort = 8500;
                    d.CurrentNodeHostName = "localhost";
                    d.CurrentNodePort = 5800;
                    d.NodeId = "1";
                    d.NodeName = "CAP No.1 Node";
                });
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(GlobalExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime application, IOptions<ServiceDisvoveryOptions> serviceOptions, IConsulClient consul)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            if (serviceOptions.Value.Enable)// 是否启用Consul
            {
                // 启动时注册服务
                application.ApplicationStarted.Register(() =>
                {
                    RegisterService(app, serviceOptions, consul);
                });
                // 停止的时候移除服务
                application.ApplicationStopped.Register(() =>
                {
                    DeRegisterService(app, serviceOptions, consul);
                });
            }
            app.UseAuthentication();
            app.UseMvc();
            UserContextSeed.SeedAsync(app, loggerFactory).Wait();
        }

        /// <summary>
        /// 启动时注册服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="serviceOptions"></param>
        /// <param name="consul"></param>
        private void RegisterService(IApplicationBuilder app, IOptions<ServiceDisvoveryOptions> serviceOptions, IConsulClient consul)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";

                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(address, "HealthCheck").OriginalString
                };

                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    Address = address.Host,
                    ID = serviceId,
                    Name = serviceOptions.Value.ServiceName,
                    Port = address.Port
                };

                consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                //appLife.ApplicationStopping.Register(() =>
                //{
                //    consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                //});
            }
        }

        /// <summary>
        /// 停止的时候移除服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="serviceOptions"></param>
        /// <param name="consul"></param>
        private void DeRegisterService(IApplicationBuilder app, IOptions<ServiceDisvoveryOptions> serviceOptions, IConsulClient consul)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";
                consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
            }
        }
    }
}
