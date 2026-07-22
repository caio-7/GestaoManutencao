
using Microsoft.EntityFrameworkCore;

namespace GestaoManutencao
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("PermitirFrontEndVue", policy =>
				{
					policy.WithOrigins("http://localhost:5173") // A porta onde o Front-end está rodando
						  .AllowAnyHeader()
						  .AllowAnyMethod();
				});
			});

			builder.Services.AddDbContext<GestaoManutencao.Data.OficinaContext>(options =>
	        options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoLocal")));

			builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

			app.UseCors("PermitirFrontEndVue");

			app.MapControllers();

            app.Run();
        }
    }
}
