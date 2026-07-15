using Microsoft.AspNetCore.Mvc;
using GestaoManutencao.Models;
using GestaoManutencao.Data; // Precisamos avisar onde está o OficinaContext
using System.Linq;

namespace GestaoManutencao.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ClienteController : ControllerBase
	{
		// Variável que vai segurar a conexão com o banco
		private readonly OficinaContext _bancoDeDados;

		// Construtor: Quando a API ligar, ela "injeta" o banco de dados aqui automaticamente
		public ClienteController(OficinaContext bancoDeDados)
		{
			_bancoDeDados = bancoDeDados;
		}

		[HttpGet]
		public IActionResult PegarTodos()
		{
			// Vai no banco de dados, na tabela Clientes, transforma em lista e devolve
			var clientes = _bancoDeDados.Clientes.ToList();
			return Ok(clientes);
		}

		[HttpPost]
		public IActionResult Cadastrar(Cliente novoCliente)
		{
			// Prepara o novo cliente para ser salvo
			_bancoDeDados.Clientes.Add(novoCliente);

			// Dá o comando "Commit" para gravar de fato no disco rígido
			_bancoDeDados.SaveChanges();

			return Ok(novoCliente);
		}
	}
}