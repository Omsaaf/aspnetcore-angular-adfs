using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AdfsExample
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
            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://testadfs2.adfs.3dsemantix.com/adfs";
                    options.Audience = "https://localhost:44320/";
                    //options.Audience = "microsoft:identityserver:090888c3-f4fd-4b12-91b7-9691787c21e3";
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = "http://TestADFS2.adfs.3dsemantix.com/adfs/services/trust"//"https://testadfs2.adfs.3dsemantix.com/adfs/services/trust"
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async ctx =>
                        {
                            var claims = new List<Claim>
                            {
                                new Claim("GivenType", "GivenValue")
                            };

                            ctx.Principal.AddIdentity(new ClaimsIdentity(claims));
                        }
                    };
                });

            services.AddCors();
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                   .AddAuthenticationSchemes("Bearer")
                   .RequireAuthenticatedUser()
                   .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Given Policy", policy => policy.RequireClaim("GivenType", "GivenValue"));
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseCors(cors =>
            {
                cors.AllowAnyHeader();
                cors.AllowAnyMethod();
                cors.AllowAnyOrigin();
            });
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
