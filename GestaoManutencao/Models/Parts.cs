using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoManutencao.Models
{
	public class Parts
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Obrigatório nome da peça")]
		public string Nome { get; set; }
		public int Quantidade { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal Preco { get; set; }
	}
}
