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
			
			var clientes = _bancoDeDados.Clientes.ToList();
			return Ok(clientes);
		}

		[HttpPost]
		public IActionResult Cadastrar(Cliente novoCliente)
		{
			
			_bancoDeDados.Clientes.Add(novoCliente);

			
			_bancoDeDados.SaveChanges();

			return Ok(novoCliente);
		}
	}
}