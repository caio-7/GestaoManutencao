using GestaoManutencao.Data;
using GestaoManutencao.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoManutencao.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PartsController : ControllerBase
	{

		private readonly OficinaContext _bancoDeDados;

		public PartsController(OficinaContext bancoDeDados)
		{
			_bancoDeDados = bancoDeDados;
		}

		[HttpGet]
		public IActionResult ListarParts()
		{
			var parts = _bancoDeDados.Parts.ToList();
			return Ok(parts);
		}

		[HttpPost]
		public IActionResult CadastrarPart(Parts novaPart)
		{
			_bancoDeDados.Parts.Add(novaPart);
			_bancoDeDados.SaveChanges();
			return Ok(novaPart);
		}

		[HttpDelete("{id}")]
		public IActionResult DeletarPart(int id)
		{
			var part = _bancoDeDados.Parts.Find(id);

			if (part == null)
			{
				return NotFound("Peça não encontrada");
			}

			_bancoDeDados.Parts.Remove(part);
			_bancoDeDados.SaveChanges();

			return Ok("Peça deletada com sucesso");
		}

		[HttpPatch("{id}/usar-part")]
		public IActionResult UsarPart(int id, [FromBody] int quantidadeUsada)
		{
			var part = _bancoDeDados.Parts.Find(id);

			if (part == null)
			{
				return NotFound("Peça não encontrada");
			}

			if (part.Quantidade < quantidadeUsada)
			{
				return BadRequest(new
				{
					Mensagem = "Quantidade insuficiente em estoque",
					EstoqueAtual = part.Quantidade,
					QuantidadeSolicitada = quantidadeUsada
				});
			}

			part.Quantidade -= quantidadeUsada;
			_bancoDeDados.SaveChanges();

			return Ok(new
			{
				Mensagem = "Baixa concluída com sucesso",
				Peça = part.Nome,
				EstoqueAtualizado = part.Quantidade
			});
				
		}
	}
}
