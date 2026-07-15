using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Necessário para usar o comando .Include()
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
			// Vai no banco, pega as OSs e JÁ INCLUI os dados do cliente dono de cada uma
			var ordens = _bancoDeDados.OrdensDeServico
									  .Include(os => os.Cliente)
									  .ToList();

			return Ok(ordens);
		}

		[HttpPost]
		public IActionResult CriarNova(OrdemDeServico novaOS)
		{
			// Como a OS está sendo aberta agora, o sistema preenche a data e hora automaticamente!
			novaOS.DataAbertura = DateTime.Now;

			_bancoDeDados.OrdensDeServico.Add(novaOS);
			_bancoDeDados.SaveChanges();

			return Ok(novaOS);
		}

		[HttpGet("{id}/gerar-recibo")]
		public IActionResult GerarReciboTxt(int id)
		{
			// 1. Busca a OS no banco de dados, incluindo os dados do cliente
			var os = _bancoDeDados.OrdensDeServico
								  .Include(o => o.Cliente)
								  .FirstOrDefault(o => o.Id == id);

			// 2. Se não encontrar a OS, retorna Erro 404 (Não Encontrado)
			if (os == null)
			{
				return NotFound("Ordem de Serviço não encontrada.");
			}

			// 3. Monta o texto do recibo
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

			// 4. Converte o texto para um arquivo baixável
			byte[] bytesArquivo = System.Text.Encoding.UTF8.GetBytes(textoRecibo);

			// Retorna o arquivo fisicamente para o navegador fazer o download
			return File(bytesArquivo, "text/plain", $"Recibo_OS_{os.Id}.txt");
		}

		[HttpGet("{id}/gerar-xml")]
		public IActionResult GerarReciboXml(int id)
		{
			// 1. Busca a OS no banco de dados
			var os = _bancoDeDados.OrdensDeServico
								  .Include(o => o.Cliente)
								  .FirstOrDefault(o => o.Id == id);

			if (os == null)
			{
				return NotFound("Ordem de Serviço não encontrada.");
			}

			// 2. Prepara as ferramentas para criar o XML em memória
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(OrdemDeServico));

			// O MemoryStream é como um papel em branco na memória RAM
			using var memoriaStream = new System.IO.MemoryStream();

			// 3. O "serializer" desenha o XML no "papel em branco"
			serializer.Serialize(memoriaStream, os);

			// 4. Prepara o arquivo para o usuário baixar
			byte[] bytesArquivo = memoriaStream.ToArray();

			// O tipo MIME para XML é "application/xml"
			return File(bytesArquivo, "application/xml", $"Recibo_OS_{os.Id}.xml");
		}
	}


}

