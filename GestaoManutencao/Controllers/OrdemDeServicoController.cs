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
		public IActionResult CriarNova([FromBody] NovaOsDTO formulario)
		{
			
			var clienteExistente = _bancoDeDados.Clientes
				.FirstOrDefault(c => c.Nome.ToLower() == formulario.NomeCliente.ToLower());

			
			int idDoClienteParaAOs;

			
			if (clienteExistente != null)
			{
				
				idDoClienteParaAOs = clienteExistente.Id;
			}
			else
			{
				
				var novoCliente = new Cliente
				{
					Nome = formulario.NomeCliente,
					Telefone = formulario.Telefone
					
				};

				_bancoDeDados.Clientes.Add(novoCliente);
				_bancoDeDados.SaveChanges(); 

				
				idDoClienteParaAOs = novoCliente.Id;
			}

			
			var novaOS = new OrdemDeServico
			{
				Descricao = formulario.Descricao,
				Defeito = formulario.Defeito,
				ClienteId = idDoClienteParaAOs,
				DataAbertura = DateTime.Now,
				Status = "Aberta"
			};

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

		[HttpPatch("{id}/atualizar-status")]
		public IActionResult AtualizarStatus(int id, [FromBody] string novoStatus)
		{
			var os = _bancoDeDados.OrdensDeServico.Find(id);

			if (os == null)
			{
				return NotFound("Ordem de Serviço não encontrada");
			}

			os.Status = novoStatus;
			_bancoDeDados.SaveChanges();

			return Ok(new
			{
				Mensagem = "Status da OS atualizado com sucesso",
				NumeroOS = os.Id,
				NovoStatus = os.Status
			});
		}

		[HttpPatch("{id}/atualizar-data-encerramento")]
		public IActionResult AtualizarEncerramento(int id, [FromBody] DateTime dataManual)
		{
			var os = _bancoDeDados.OrdensDeServico.Find(id);

			if (os == null)
			{
				return NotFound("Ordem de Serviço não encontrada");
			}

			os.DataFechamento = dataManual;
			_bancoDeDados.SaveChanges();

			return Ok(new
			{
				Mensagem = "Data de fechamento da OS atualizada com sucesso",
				NumeroOS = os.Id,
				DataFechamento = os.DataFechamento
			});
		}

		[HttpDelete("{id}")]
		public IActionResult DeletarOrdem(int id)
		{
			var os = _bancoDeDados.OrdensDeServico.Find(id);

			if (os == null)
			{
				return NotFound("Ordem de Serviço não encontrada.");
			}

			_bancoDeDados.OrdensDeServico.Remove(os);
			_bancoDeDados.SaveChanges();

			return Ok(new
			{
				Mensagem = "Ordem de serviço excluída com sucesso",
				NumeroOS = id
			});
		}

		[HttpPatch("{id}/atualizar-defeito")]
		public IActionResult AtualizarDefeito(int id, [FromBody] string? novoDefeito)
		{
			var os = _bancoDeDados.OrdensDeServico.Find(id);

			if (os == null)
			{
				return NotFound("Ordem de Serviço não encontrada.");
			}

			// Atualiza a coluna Defeito com o diagnóstico técnico da bancada
			os.Defeito = novoDefeito;
			_bancoDeDados.SaveChanges();

			return Ok(new
			{
				Mensagem = "Diagnóstico do defeito registrado com sucesso",
				NumeroOS = os.Id,
				Equipamento = os.Descricao,
				Defeito = os.Defeito
			});
		}

		[HttpPut("{id}")]
		public IActionResult AtualizarOrdemCompleta(int id, [FromBody] OrdemDeServico dadosAtualizados)
		{
			// 1. Busca a OS original no banco de dados
			var os = _bancoDeDados.OrdensDeServico.Find(id);

			if (os == null)
			{
				return NotFound("Ordem de Serviço não encontrada.");
			}

			// 2. Transfere os dados novos que vieram do Vue para a OS do banco
			os.Descricao = dadosAtualizados.Descricao;
			os.Defeito = dadosAtualizados.Defeito;
			os.Status = dadosAtualizados.Status;
			os.ValorTotal = dadosAtualizados.ValorTotal;
			os.DataFechamento = dadosAtualizados.DataFechamento;

			// 3. Salva tudo de uma vez
			_bancoDeDados.SaveChanges();

			return Ok(new
			{
				Mensagem = "OS atualizada com sucesso na bancada!",
				NumeroOS = os.Id
			});
		}

	}


}

