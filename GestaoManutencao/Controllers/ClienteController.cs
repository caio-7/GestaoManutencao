using Microsoft.AspNetCore.Mvc;
using GestaoManutencao.Models;
using GestaoManutencao.Data;
using System.Linq;

namespace GestaoManutencao.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ClienteController : ControllerBase
	{
		private readonly OficinaContext _bancoDeDados;

		public ClienteController(OficinaContext bancoDeDados)
		{
			_bancoDeDados = bancoDeDados;
		}

		[HttpGet]
		public IActionResult PegarTodos()
		{
			// O comando OrderBy faz o trabalho pesado de ordenar alfabeticamente direto no banco!
			var clientes = _bancoDeDados.Clientes.OrderBy(c => c.Nome).ToList();
			return Ok(clientes);
		}

		[HttpPost]
		public IActionResult Cadastrar(Cliente novoCliente)
		{
			bool nomeExiste = _bancoDeDados.Clientes.Any(c => c.Nome.ToLower() == novoCliente.Nome.ToLower());
			if (nomeExiste)
			{
				return BadRequest("Já existe um cliente cadastrado com este exato nome.");
			}

			_bancoDeDados.Clientes.Add(novoCliente);
			_bancoDeDados.SaveChanges();
			return Ok(novoCliente);
		}

		// NOVO MÉTODO: Recebe a edição do cliente
		[HttpPut("{id}")]
		public IActionResult Atualizar(int id, [FromBody] Cliente dadosAtualizados)
		{

			bool nomeExiste = _bancoDeDados.Clientes.Any(c => c.Nome.ToLower() == dadosAtualizados.Nome.ToLower() && c.Id != id);
			if (nomeExiste)
			{
				return BadRequest("Já existe outro cliente usando este nome.");
			}

			var clienteOriginal = _bancoDeDados.Clientes.Find(id);

			if (clienteOriginal == null)
			{
				return NotFound("Cliente não encontrado.");
			}

			clienteOriginal.Nome = dadosAtualizados.Nome;
			clienteOriginal.Telefone = dadosAtualizados.Telefone;
			clienteOriginal.Email = dadosAtualizados.Email;
			clienteOriginal.Endereco = dadosAtualizados.Endereco;

			_bancoDeDados.SaveChanges();

			return Ok(clienteOriginal);
		}

		[HttpDelete("{id}")]
		public IActionResult Deletar(int id)
		{
			var cliente = _bancoDeDados.Clientes.Find(id);
			if (cliente == null)
			{
				return NotFound("Cliente não encontrado.");
			}

			_bancoDeDados.Clientes.Remove(cliente);
			_bancoDeDados.SaveChanges();

			return Ok();
		}

	}
}