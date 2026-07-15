using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using GestaoManutencao.Models;
using GestaoManutencao.Data;
using System;
using System.Linq;

namespace GestaoManutencao.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OrdemDeServicoController : ControllerBase
	{
		private readonly OficinaContext _bancoDeDados;

		public OrdemDeServicoController(OficinaContext bancoDeDados)
		{
			_bancoDeDados = bancoDeDados;
		}

		[HttpGet]
		public IActionResult PegarTodas()
		{
			
			var ordens = _bancoDeDados.OrdensDeServico
									  .Include(os => os.Cliente)
									  .ToList();

			return Ok(ordens);
		}

		[HttpPost]
		public IActionResult CriarNova(OrdemDeServico novaOS)
		{
			
			novaOS.DataAbertura = DateTime.Now;

			_bancoDeDados.OrdensDeServico.Add(novaOS);
			_bancoDeDados.SaveChanges();

			return Ok(novaOS);
		}

		[HttpGet("{id}/gerar-recibo")]
		public IActionResult GerarReciboTxt(int id)
		{
			
			var os = _bancoDeDados.OrdensDeServico
								  .Include(o => o.Cliente)
								  .FirstOrDefault(o => o.Id == id);

			
			if (os == null)
			{
				return NotFound("Ordem de Serviço não encontrada.");
			}

			
			string textoRecibo = $"--- RECIBO DE MANUTENCAO ---\n" +
								 $"Data: {DateTime.Now:dd/MM/yyyy HH:mm}\n" +
								 $"OS Numero: {os.Id}\n" +
								 $"Cliente: {os.Cliente.Nome}\n" +
								 $"Telefone: {os.Cliente.Telefone}\n" +
								 $"----------------------------\n" +
								 $"Equipamento/Defeito: {os.Descricao}\n" +
								 $"Valor Total: R$ {os.ValorTotal}\n" +
								 $"----------------------------\n" +
								 $"Obrigado pela preferencia!";

			
			byte[] bytesArquivo = System.Text.Encoding.UTF8.GetBytes(textoRecibo);

			
			return File(bytesArquivo, "text/plain", $"Recibo_OS_{os.Id}.txt");
		}

		[HttpGet("{id}/gerar-xml")]
		public IActionResult GerarReciboXml(int id)
		{
			
			var os = _bancoDeDados.OrdensDeServico
								  .Include(o => o.Cliente)
								  .FirstOrDefault(o => o.Id == id);

			if (os == null)
			{
				return NotFound("Ordem de Serviço não encontrada.");
			}

			
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(OrdemDeServico));

			
			using var memoriaStream = new System.IO.MemoryStream();

			
			serializer.Serialize(memoriaStream, os);

			
			byte[] bytesArquivo = memoriaStream.ToArray();

			
			return File(bytesArquivo, "application/xml", $"Recibo_OS_{os.Id}.xml");
		}
	}


}

