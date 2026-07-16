using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace GestaoManutencao.Models
{
	public class OrdemDeServico
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Obrigatório descrição do serviço")]
		public string Descricao { get; set; }

		public string? Defeito { get; set; }

		public String Status { get; set; } = "Aberta";
		public DateTime DataAbertura { get; set; }
		public DateTime? DataFechamento { get; set; }

		[Precision(10, 2)]
		public decimal ValorTotal { get; set; }
		public int ClienteId { get; set; }
		public Cliente? Cliente { get; set; }
	}
}
