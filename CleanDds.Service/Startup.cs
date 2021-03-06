﻿using System;
using System.Reflection;
using CleanDds.Application.Interfaces;
using CleanDds.Infrastructure.Seeding;
using CleanDds.Persistance;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanDds.Service
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
            services.AddDbContext<InMemDatabaseService>(x => x.UseInMemoryDatabase("in-mem"));
            services.AddTransient<IDatabaseService, InMemDatabaseService>();
            services.AddTransient<ISeedingService, SeedingService>();

            services.AddMediatR(new[] 
            {
                Assembly.Load("CleanDds.Application.CommandStack"),
                Assembly.Load("CleanDds.Application.QueryStack")
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            var seeding = serviceProvider.GetService<ISeedingService>();
            seeding.SeedRates(Configuration["RatesUrl"]);
            seeding.SeedTransactions(Configuration["TransactionsUrl"]);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
