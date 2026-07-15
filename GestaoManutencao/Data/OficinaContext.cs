using Microsoft.EntityFrameworkCore;
using GestaoManutencao.Models;

namespace GestaoManutencao.Data
{
	// O " : DbContext " avisa que esta classe herda os superpoderes do pacote do Entity Framework que você instalou
	public class OficinaContext : DbContext
	{
		// Este construtor vai receber as configurações de conexão mais para frente
		public OficinaContext(DbContextOptions<OficinaContext> options) : base(options)
		{
		}

		// Os DbSets são as declarações de quais classes devem virar tabelas no banco de dados!
		public DbSet<Cliente> Clientes { get; set; }
		public DbSet<OrdemDeServico> OrdensDeServico { get; set; }
	}
}