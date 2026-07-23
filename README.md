# 🔧 Sistema de Gestão de Manutenção

Um sistema Full-Stack desenvolvido para o controle eficiente de ordens de serviço (OS) e cadastro de clientes, projetado especificamente para o fluxo de trabalho de uma assistência técnica de manutenção eletrônica.

## 🚀 Arquitetura do Projeto

O sistema foi construído utilizando uma arquitetura moderna, separando a interface do usuário (Front-end) da lógica de negócios e acesso a dados (Back-end), com implantação automatizada na nuvem.

* **Front-end:** Desenvolvido em **Vue.js**, oferecendo uma interface reativa e rápida (Single Page Application). Hospedado na **Vercel**.
* **Back-end:** API RESTful desenvolvida em **C# (.NET 8)**, garantindo alta performance, segurança e tipagem forte. Hospedada no **Microsoft Azure** (App Service).
* **Banco de Dados:** Relacional, gerenciado via **Entity Framework Core**.
* **CI/CD:** Pipeline de implantação contínua configurado com **GitHub Actions** para o servidor do Azure.

## ✨ Funcionalidades

### 👥 Gestão de Clientes
* Cadastro completo de clientes (Nome, Telefone, Email, Endereço).
* Formatação automática de nomes (capitalização inteligente).
* Edição e exclusão com travas de segurança.

### 📋 Ordens de Serviço (OS)
* Abertura de nova OS vinculada a clientes cadastrados.
* Registro detalhado de equipamento e defeito relatado.
* Controle de status (Aberta, Em Análise, Aguardando Peça, Concluída).
* Precificação e controle de datas (Abertura e Fechamento).
* Atualização em tempo real na interface gráfica.

## 🛠️ Tecnologias Utilizadas

**Front-end:**
* Vue.js 3 (Composition API)
* Fetch API para integrações assíncronas
* CSS3 nativo com design responsivo e modular

**Back-end:**
* C# .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* CORS configurado para segurança de requisições

