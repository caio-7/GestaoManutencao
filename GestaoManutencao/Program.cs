
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
					policy.AllowAnyOrigin() 
						  .AllowAnyHeader()
						  .AllowAnyMethod();
				});
			});

			builder.Services.AddDbContext<GestaoManutencao.Data.OficinaContext>(options =>
	        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

			
			using (var scope = app.Services.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<GestaoManutencao.Data.OficinaContext>();
				db.Database.Migrate();
			}

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			
			app.UseCors("PermitirFrontEndVue");

			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
    }
}
