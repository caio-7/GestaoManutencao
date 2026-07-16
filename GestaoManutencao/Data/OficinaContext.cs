using Microsoft.EntityFrameworkCore;
using GestaoManutencao.Models;

namespace GestaoManutencao.Data
{
	
	public class OficinaContext : DbContext
	{
		
		public OficinaContext(DbContextOptions<OficinaContext> options) : base(options)
		{
		}

		
		public DbSet<Cliente> Clientes { get; set; }
		public DbSet<OrdemDeServico> OrdensDeServico { get; set; }
		public DbSet<Parts> Parts { get; set; }
	}
}