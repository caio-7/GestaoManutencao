Sistema de Gestão de Manutenção (API REST)

API RESTful desenvolvida em C# e .NET 8 para gerenciar o fluxo operacional de uma bancada de manutenção eletrônica. O sistema automatiza o cadastro de clientes, o controle de ordens de serviço (OS) e a geração de relatórios de saída para equipamentos reparados.

## 🚀 Tecnologias Utilizadas

*   **Linguagem:** C#
*   **Framework:** .NET 8 (Web API)
*   **Acesso a Dados (ORM):** Entity Framework Core
*   **Banco de Dados:** SQL Server (LocalDB)
*   **Documentação e Testes:** Swagger / OpenAPI

## ⚙️ Arquitetura e Funcionalidades

O projeto foi estruturado utilizando o padrão de arquitetura MVC (Model-View-Controller) no back-end, focando em separação de responsabilidades e injeção de dependência.

*   **Gestão de Clientes:** Operações de CRUD para cadastro e consulta de clientes.
*   **Controle de Ordens de Serviço:** Abertura de OS com relacionamento direto de chaves estrangeiras (Foreign Keys) com a entidade de Cliente utilizando o recurso de *Eager Loading* do EF Core.
*   **Exportação de Dados:** Geração dinâmica de recibos de ordens de serviço e exportação em formatos de arquivo físico (`.txt` e `.xml` serializado) para integração de sistemas e entrega ao cliente final.

## 🛠️ Como executar o projeto localmente

1. Clone este repositório no seu ambiente (Windows ou Linux):
   `git clone https://github.com/caio-7/GestaoManutencao.git`
2. Acesse a pasta do projeto e restaure as dependências do .NET.
3. Certifique-se de ter o SQL Server (ou LocalDB) instalado e atualize o banco de dados rodando o comando no terminal:
   `dotnet ef database update`
4. Execute a aplicação:
   `dotnet run`
5. Acesse a interface interativa do Swagger através da porta gerada no `localhost` para testar os endpoints.
