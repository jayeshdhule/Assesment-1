using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AvalphaTechnologies.CommissionCalculator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Allow the frontend dev server to call the API
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontendDev", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // In development show swagger UI and avoid forcing HTTPS redirects so CORS preflight
                // requests are not turned into 307 redirects which browsers block for OPTIONS.
                app.UseSwagger();
                app.UseSwaggerUI();
                // Do not call UseHttpsRedirection() in Development to avoid preflight redirect issues
            }
            else
            {
                // In Production we want to redirect HTTP -> HTTPS
                app.UseHttpsRedirection();
            }

            // Apply CORS policy (must be before authorization and endpoint mapping)
            app.UseCors("AllowFrontendDev");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
