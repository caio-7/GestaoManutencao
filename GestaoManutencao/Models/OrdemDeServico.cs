using Microsoft.EntityFrameworkCore;
namespace GestaoManutencao.Models
{
	public class OrdemDeServico
	{
		public int Id { get; set; }
		public string Descricao { get; set; }
		public DateTime DataAbertura { get; set; }
		public DateTime? DataFechamento { get; set; }

		[Precision(10, 2)]
		public decimal ValorTotal { get; set; }
		public int ClienteId { get; set; }
		public Cliente? Cliente { get; set; }
	}
}
