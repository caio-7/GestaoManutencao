# API de Gestão de Manutenção 💻🔌

Sistema de Back-end desenvolvido para gerenciar o fluxo de atendimento, clientes e peças de uma bancada de manutenção eletrônica. 

## 🛠️ Tecnologias Utilizadas
* **Linguagem:** C# (.NET 8/ASP.NET Core)
* **Banco de Dados:** SQL Server
* **ORM:** Entity Framework Core
* **Documentação de API:** Swagger (OpenAPI)

## 🚀 Funcionalidades (Atuais)
* **Gestão de Clientes:**
  * Criação automática e inteligente de clientes atrelada à abertura de Ordens de Serviço (lógica "Find or Create").
  * Dados de contato flexíveis (campos opcionais para e-mail, telefone, etc).
* **Gestão de Ordens de Serviço (OS):**
  * Abertura rápida de OS utilizando DTOs.
  * Atualização de status e datas de fechamento independentes.
  * Rota de diagnóstico (`PATCH`) para registrar defeitos técnicos após avaliação na bancada.
  * Exclusão segura de registros.
* **Gestão de Estoque:**
  * Controle de peças cadastradas (CRUD).

## ⚙️ Como executar o projeto
1. Clone este repositório.
2. Abra o projeto no Visual Studio.
3. Abra o **Console do Gerenciador de Pacotes** ou **PowerShell do Desenvolvedor**.
4. Atualize o banco de dados local executando o comando:
   `dotnet ef database update`
5. Execute a aplicação (F5). O navegador abrirá automaticamente na interface do Swagger para testes de rotas.