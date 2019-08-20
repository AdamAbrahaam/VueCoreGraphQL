using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VueCoreGraphQL.Data;
using VueCoreGraphQL.GraphQL;
using VueCoreGraphQL.Repositories;

namespace VueCoreGraphQL
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        private readonly IConfiguration _config;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });


            services.AddControllers();

            // DbContext
            services.AddDbContext<CarvedRockDbContext>(options =>
                options.UseSqlServer(_config.GetConnectionString("CarvedRock")));
            services.AddScoped<ProductRepository>();
            services.AddScoped<ProductReviewRepository>();

            // GraphQL
            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(    // if something asks for IDependencyResolver return a new FuncDependencyResolver instance
                s.GetRequiredService));

            services.AddScoped<CarvedRockSchema>();

            services.AddGraphQL(o => { o.ExposeExceptions = true; })    // Register all the types GrahpQL.Net uses and additional options ( o.XY )
                .AddGraphTypes(ServiceLifetime.Scoped)
                .AddUserContextBuilder(httpContext => httpContext.User)             // User context(property) provider for authorization, whenever a user context is needed this will be executed
                .AddDataLoader();

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CarvedRockDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            // GraphQL
            app.UseCors(builder =>
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseGraphQL<CarvedRockSchema>();     // In argument we can specify the endpoint URI, /GraphQl default 

            // Sets up the default GraphQL playground at /ui/Playground
            // Project properties -> Debug -> Launch browser -> ui/Playground (starts the browser with the playground)
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());   // To play around with the API without web application ( web app not yet created )


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Fill the database
            dbContext.Seed();
        }
    }
}
