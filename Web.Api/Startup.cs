using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using MailScheduler.Config;
using MailScheduler.Repositories;
using MailScheduler.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace MailScheduler
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
            // Settings for application
            var configSection = Configuration.GetSection(nameof(AppSettings));
            services.Configure<AppSettings>(configSection);
            services.AddSingleton<IAppSettings>(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);

            // Services
            services.AddScoped<ISchedulerService, SchedulerService>();
            services.AddScoped<IMailerService, MailerService>();

            // Repositories
            services.AddScoped<ISchedulerRepository, SchedulerRepository>();

            // Hangfire Background Jobs
            var migrationOptions = new MongoMigrationOptions
            {
                MigrationStrategy = new DropMongoMigrationStrategy(),
                BackupStrategy = new NoneMongoBackupStrategy(),
            };

            var adminDb = configSection.GetSection("Database").GetSection("AdminDb").Value;
            var adminUsername = configSection.GetSection("Database").GetSection("AdminUsername").Value;
            var adminPassword = configSection.GetSection("Database").GetSection("AdminPassword").Value;
            var serverUrl = configSection.GetSection("Database").GetSection("ServerUrl").Value;

            var databaseName = configSection.GetSection("Database").GetSection("Name").Value;

            var mongoClientSettings = new MongoClientSettings
            {
                Credential = MongoCredential.CreateCredential(adminDb, adminUsername, adminPassword),
                Server = new MongoServerAddress(serverUrl, 27017), // TODO: Add port into db settins, shouldn't really matter.
            };

            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                config.UseSimpleAssemblyNameTypeSerializer();
                config.UseRecommendedSerializerSettings();
                config.UseMongoStorage(mongoClientSettings, databaseName, new MongoStorageOptions { MigrationOptions = migrationOptions });
            });
            services.AddHangfireServer();

            // Controllers
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Expose hangfire dashboard
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                //Authorization = new[] { }
            });

            // TODO: Move CRON to constant file (TO-TODO: Make CRON adjustable from LimeSurvey)
            RecurringJob.AddOrUpdate<IMailerService>(x => x.AssessAndSendMail(), "*/5 * * * *");
        }
    }
}
