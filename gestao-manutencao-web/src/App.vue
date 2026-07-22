<script setup>
    import { ref, onMounted } from 'vue';

    // ==========================================
    // 1. CONTROLE DE NAVEGAÇÃO (ABAS)
    // ==========================================
    const abaAtual = ref('OS'); // O sistema liga já na tela de OS

    function mudarAba(nomeDaAba) {
        abaAtual.value = nomeDaAba;
        // Quando troca de aba, atualizamos os dados direto do banco
        if (nomeDaAba === 'Clientes') buscarClientes();
        if (nomeDaAba === 'OS') buscarOrdensDeServico();
    }

    // ==========================================
    // 2. LÓGICA DE CLIENTES
    // ==========================================
    const clientes = ref([]);
    const mostrarModalCliente = ref(false);
    const novoCliente = ref({ id: null, nome: '', telefone: '', email: '', endereco: '' });
	const modoExclusao = ref(false);

    async function buscarClientes() {
        try {
            const resposta = await fetch('https://localhost:7187/api/Cliente');
            clientes.value = await resposta.json();
        } catch (erro) {
            console.error('Erro ao buscar clientes:', erro);
        }
    }

    function abrirFormularioCliente() {
        novoCliente.value = { id: null, nome: '', telefone: '', email: '', endereco: '' };
        mostrarModalCliente.value = true;
    }

    function fecharFormularioCliente() {
        mostrarModalCliente.value = false;
    }

	async function salvarCliente() {
		try {
			const pacoteCliente = {
				id: novoCliente.value.id || 0,
				nome: formatarNome(novoCliente.value.nome), // <-- Aplica a formatação aqui
				telefone: novoCliente.value.telefone || null,
				email: novoCliente.value.email || null,
				endereco: novoCliente.value.endereco || null
			};

			const isEdicao = pacoteCliente.id > 0;
			const urlDaApi = isEdicao
				? `https://localhost:7187/api/Cliente/${pacoteCliente.id}`
				: 'https://localhost:7187/api/Cliente';

			const resposta = await fetch(urlDaApi, {
				method: isEdicao ? 'PUT' : 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(pacoteCliente)
			});

			if (resposta.ok) {
				fecharFormularioCliente();
				buscarClientes();
			} else {
				// Se cair na nossa trava de nome duplicado (BadRequest 400), avisa o usuário:
				const erroDoBackend = await resposta.text();
				alert(erroDoBackend);
			}
		} catch (erro) {
			console.error('Erro ao salvar cliente:', erro);
		}
	}

	// FUNÇÃO NOVA: Deletar cliente
	async function deletarCliente(id, nome) {
		if (confirm(`Atenção: Deseja realmente excluir o cliente ${nome}?`)) {
			try {
				const resposta = await fetch(`https://localhost:7187/api/Cliente/${id}`, {
					method: 'DELETE'
				});

				if (resposta.ok) {
					buscarClientes(); // Atualiza a tabela fazendo a linha sumir
				} else {
					alert("Falha ao excluir o cliente.");
				}
			} catch (erro) {
				console.error('Erro na exclusão:', erro);
			}
		}
	}

	// NOVO: Função que joga os dados da linha clicada para dentro do modal
	function abrirParaEditarCliente(cliente) {
		novoCliente.value = {
			id: cliente.id,
			nome: cliente.nome,
			telefone: cliente.telefone || '',
			email: cliente.email || '',
			endereco: cliente.endereco || ''
		};
		mostrarModalCliente.value = true;
    }

	function formatarNome(nomeOriginal) {
		if (!nomeOriginal) return '';
		const preposicoes = ['de', 'da', 'do', 'das', 'dos']; // Não recebem maiúscula

		return nomeOriginal.toLowerCase().split(' ').map(palavra => {
			if (preposicoes.includes(palavra)) return palavra;
			return palavra.charAt(0).toUpperCase() + palavra.slice(1);
		}).join(' ');
	}

    // ==========================================
    // 3. LÓGICA DE ORDENS DE SERVIÇO (OS)
    // ==========================================
    const ordensServico = ref([]);
    const mostrarModal = ref(false);
	const modoExclusaoOS = ref(false);

    const novaOs = ref({
        id: null,
        nomeCliente: '',
        descricao: '',
        defeito: '',
        status: 'Aberta',
        valorTotal: '',
        dataAbertura: '',
        dataFechamento: ''
    });

    async function buscarOrdensDeServico() {
        try {
            const resposta = await fetch('https://localhost:7187/api/OrdemDeServico');
            ordensServico.value = await resposta.json();
        } catch (erro) {
            console.error('Erro ao buscar OS:', erro);
        }
    }

    function abrirFormulario() {
        novaOs.value = { id: null, nomeCliente: '', descricao: '', defeito: '', status: 'Aberta', valorTotal: '', dataAbertura: '', dataFechamento: '' };
        mostrarModal.value = true;
    }

    function fecharFormulario() {
        mostrarModal.value = false;
    }

    function abrirParaEditar(os) {
        novaOs.value = {
            id: os.id,
			clienteId: os.clienteId || (os.cliente ? os.cliente.id : ''),
            descricao: os.descricao || '',
            defeito: os.defeito || '',
            status: os.status || 'Aberta',
            valorTotal: os.valorTotal || '',
            dataAbertura: os.dataAbertura,
            dataFechamento: os.dataFechamento ? os.dataFechamento.split('T')[0] : ''
        };
        mostrarModal.value = true;
    }

    async function salvarOS() {
        try {
            const defeitoFormatado = novaOs.value.defeito || null;
            const dataFechamentoFormatada = novaOs.value.dataFechamento || null;
            const valorFormatado = novaOs.value.valorTotal ? parseFloat(novaOs.value.valorTotal) : 0;

			if (!novaOs.value.clienteId) {
				alert("Por favor, selecione um cliente.");
				return;
            }

            const clienteEscolhido = clientes.value.find(c => c.id === novaOs.value.clienteId);

			const nomeReal = clienteEscolhido ? clienteEscolhido.nome : "";

            const pacoteOS = {
                id: novaOs.value.id || 0,
                clienteId: novaOs.value.clienteId,
				nomeCliente: nomeReal,
                descricao: novaOs.value.descricao,
                defeito: defeitoFormatado,
                status: novaOs.value.status,
                dataAbertura: novaOs.value.dataAbertura || new Date().toISOString(),
                dataFechamento: dataFechamentoFormatada,
                valorTotal: valorFormatado,
                
            };

            const isEdicao = pacoteOS.id > 0;
            const urlDaApi = isEdicao
                ? `https://localhost:7187/api/OrdemDeServico/${pacoteOS.id}`
                : 'https://localhost:7187/api/OrdemDeServico';

            const resposta = await fetch(urlDaApi, {
                method: isEdicao ? 'PUT' : 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(pacoteOS)
            });

            if (resposta.ok) {
                fecharFormulario();
                buscarOrdensDeServico();
            } else {
                alert("Falha ao salvar. Verifique o console.");
            }
        } catch (erro) {
            console.error('Erro na requisição:', erro);
        }
    }

    async function deletarOS(id) {
        if (confirm(`Atenção: Tem certeza que deseja excluir a OS #${id}? Essa ação não tem volta.`)) {
            try {
                const resposta = await fetch(`https://localhost:7187/api/OrdemDeServico/${id}`, {
                    method: 'DELETE'
                });

                if (resposta.ok) {
                    buscarOrdensDeServico();
                } else {
                    alert("Falha ao excluir.");
                }
            } catch (erro) {
                console.error('Erro na requisição de exclusão:', erro);
            }
        }
    }

    // Quando o sistema inicia, busca logo tudo do banco
    onMounted(() => {
        buscarOrdensDeServico();
        buscarClientes();
    });
</script>

<template>
    <div class="app-container">
        <!-- NAVEGAÇÃO -->
        <nav class="menu-superior">
            <div class="logo">🔧 Gestão Manutenção</div>
            <div class="abas">
                <button :class="{ ativo: abaAtual === 'OS' }" @click="mudarAba('OS')">Ordens de Serviço</button>
                <button :class="{ ativo: abaAtual === 'Clientes' }" @click="mudarAba('Clientes')">Clientes</button>
            </div>
        </nav>

        <!-- TELA 1: ORDENS DE SERVIÇO -->
        <main v-if="abaAtual === 'OS'">
            <div class="cabecalho">
                <h1>Ordens de Serviço</h1>
                
                <div class="botoes-acao">
                    
                    <button class="btn-cancelar" @click="modoExclusaoOS = !modoExclusaoOS">
                        {{ modoExclusaoOS ? 'Cancelar Exclusão' : '🗑️ Modo Excluir' }}
                    </button>
                    <button class="btn-novo" @click="abrirFormulario">+ Nova OS</button>
                </div>
            </div>

            <div class="tabela-container">
                <table>
                    <thead>
                        <tr>
                            <th>Nº da OS</th>
                            <th>Cliente</th>
                            <th>Equipamento</th>
                            <th>Defeito Relatado</th>
                            <th class="col-status">Status</th>
                            <th class="col-acoes">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="os in ordensServico" :key="os.id">
                            <td><strong>#{{ os.id }}</strong></td>
                            <td>{{ os.nomeCliente || os.cliente?.nome }}</td>
                            <td>{{ os.descricao }}</td>
                            <td>{{ os.defeito }}</td>
                            <td>{{ os.status }}</td>
                            <td>
                                <div class="botoes-acao">
                                    <!-- Botão de editar sempre visível -->
                                    <button class="btn-editar" @click="abrirParaEditar(os)">✏️ Editar</button>

                                    <!-- Botão de lixeira oculto, só aparece se o modoExclusaoOS for true -->
                                    <button class="btn-excluir" v-if="modoExclusaoOS" @click="deletarOS(os.id)">🗑️</button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- MODAL DA OS -->
            <div v-if="mostrarModal" class="modal-overlay">
                <div class="modal-conteudo">
                    <h2>{{ novaOs.id ? 'Editar OS #' + novaOs.id : 'Cadastrar Nova OS' }}</h2>

                    <div class="grupo-input">
                        <label>Cliente <span class="asterisco">*</span></label>
                        <select v-model="novaOs.clienteId">
                            <option disabled value="">Selecione um cliente...</option>

                            <!-- O v-for vai criar uma opção para cada cliente cadastrado no banco -->
                            <option v-for="cliente in clientes" :key="cliente.id" :value="cliente.id">
                                {{ cliente.nome }}
                            </option>
                        </select>
                    </div>

                    <div class="grupo-input">
                        <label>Equipamento <span class="asterisco">*</span></label>
                        <input type="text" v-model="novaOs.descricao" placeholder="Ex: Placa Mãe ECS P4VMM2..." />
                    </div>

                    <div class="grupo-input">
                        <label>Defeito Relatado</label>
                        <input type="text" v-model="novaOs.defeito" placeholder="Ex: Não dá vídeo" />
                    </div>

                    <div class="campos-lado-a-lado">
                        <div class="grupo-input">
                            <label>Status</label>
                            <select v-model="novaOs.status">
                                <option value="Aberta">Aberta</option>
                                <option value="Em Análise">Em Análise</option>
                                <option value="Aguardando Peça">Aguardando Peça</option>
                                <option value="Concluída">Concluída</option>
                            </select>
                        </div>
                        <div class="grupo-input">
                            <label>Valor Total (R$)</label>
                            <input type="number" step="0.01" v-model="novaOs.valorTotal" placeholder="0,00" />
                        </div>
                    </div>

                    <div class="grupo-input">
                        <label>Data de Fechamento</label>
                        <input type="date" v-model="novaOs.dataFechamento" />
                    </div>

                    <div class="modal-botoes">
                        <button class="btn-cancelar" @click="fecharFormulario">Cancelar</button>
                        <button class="btn-salvar" @click="salvarOS">Salvar OS</button>
                    </div>
                </div>
            </div>
        </main>

        
        <!-- TELA 2: CLIENTES -->
        <main v-if="abaAtual === 'Clientes'">
            <div class="cabecalho">
                <h1>Clientes</h1>
                <div class="botoes-acao">
                    <!-- O botão que liga/desliga o modo de exclusão -->
                    <button class="btn-cancelar" @click="modoExclusao = !modoExclusao">
                        {{ modoExclusao ? 'Cancelar Exclusão' : '🗑️ Modo Excluir' }}
                    </button>
                    <button class="btn-novo" @click="abrirFormularioCliente">+ Novo Cliente</button>
                </div>
            </div>

            <div class="tabela-container">
                <table>
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Nome</th>
                            <th>Telefone</th>
                            <th>Email</th>
                            <th class="col-acoes">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(cliente, index) in clientes" :key="cliente.id">
                            <td><strong>{{ index + 1 }}</strong></td>
                            <td>{{ cliente.nome }}</td>
                            <td>{{ cliente.telefone || '---' }}</td>
                            <td>{{ cliente.email || '---' }}</td>
                            <td>
                                <div class="botoes-acao">
                                    <!-- Botão de editar sempre visível -->
                                    <button class="btn-editar" @click="abrirParaEditarCliente(cliente)">✏️ Editar</button>

                                    <!-- Botão de lixeira só aparece se clicar no botão mestre do cabeçalho -->
                                    <button class="btn-excluir" v-if="modoExclusao" @click="deletarCliente(cliente.id, cliente.nome)">🗑️</button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- MODAL DE CLIENTE COMPLETO -->
            <div v-if="mostrarModalCliente" class="modal-overlay">
                <div class="modal-conteudo">
                    <!-- Título dinâmico -->
                    <h2>{{ novoCliente.id ? 'Editar Cliente' : 'Cadastrar Novo Cliente' }}</h2>

                    <div class="grupo-input">
                        <label>Nome <span class="asterisco">*</span></label>
                        <input type="text" v-model="novoCliente.nome" placeholder="Ex: João Silva" />
                    </div>
                    <div class="grupo-input">
                        <label>Telefone</label>
                        <input type="text" v-model="novoCliente.telefone" placeholder="(11) 99999-9999" />
                    </div>
                    <div class="grupo-input">
                        <label>Email</label>
                        <input type="email" v-model="novoCliente.email" placeholder="joao@email.com" />
                    </div>

                    <div class="modal-botoes">
                        <button class="btn-cancelar" @click="fecharFormularioCliente">Cancelar</button>
                        <button class="btn-salvar" @click="salvarCliente">Salvar Cliente</button>
                    </div>
                </div>
            </div>
        </main>
    </div>
</template>

<style scoped>
     /* ==========================================
    ESTILOS GERAIS E NAVEGAÇÃO
    ========================================== */
     .app-container {
         min-height: 100vh;
         background-color: #f8fafc;
     }

     .menu-superior {
         background-color: #1e293b;
         color: white;
         display: flex;
         justify-content: space-between;
         align-items: center;
         padding: 0 30px;
         height: 60px;
         box-shadow: 0 2px 4px rgba(0,0,0,0.1);
     }

     .logo {
         font-weight: bold;
         font-size: 1.2rem;
         color: #38bdf8;
     }

     .abas button {
         background: none;
         border: none;
         color: #cbd5e1;
         padding: 0 20px;
         height: 60px;
         font-size: 1rem;
         font-weight: 600;
         cursor: pointer;
         transition: all 0.2s;
     }

         .abas button:hover {
             color: white;
             background-color: #334155;
         }

         .abas button.ativo {
             color: #38bdf8;
             border-bottom: 3px solid #38bdf8;
             background-color: #0f172a;
         }

     /* ==========================================
    ESTILOS DAS TABELAS E BOTÕES (O QUE JÁ TÍNHAMOS)
    ========================================== */
     main {
         padding: 30px;
         font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
         max-width: 1000px;
         margin: 0 auto;
     }

     .cabecalho {
         display: flex;
         justify-content: space-between;
         align-items: center;
         border-bottom: 2px solid #eaeaea;
         padding-bottom: 15px;
         margin-bottom: 20px;
     }

     h1 {
         color: #2c3e50;
         margin: 0;
     }

     .btn-novo {
         background-color: #10b981;
         color: white;
         border: none;
         padding: 10px 24px;
         border-radius: 6px;
         font-size: 1rem;
         font-weight: 600;
         cursor: pointer;
         transition: background-color 0.2s;
     }

         .btn-novo:hover {
             background-color: #059669;
         }

     .tabela-container {
         background: #ffffff;
         border-radius: 8px;
         box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
         overflow: hidden;
     }

     table {
         width: 100%;
         border-collapse: collapse;
         text-align: left;
     }

     th, td {
         padding: 15px;
         border-bottom: 1px solid #f0f0f0;
     }

     th {
         background-color: #f8f9fa;
         font-weight: 600;
         color: #333;
     }

     tr:hover {
         background-color: #f1f7ff;
     }

     .col-status {
         width: 160px;
     }

     .col-acoes {
         width: 200px;
     }

     .botoes-acao {
         display: flex;
         gap: 8px;
     }

     .btn-editar {
         background-color: #f59e0b;
         color: white;
         border: none;
         padding: 6px 12px;
         border-radius: 4px;
         cursor: pointer;
         font-weight: 600;
         font-size: 0.9rem;
     }

         .btn-editar:hover {
             background-color: #d97706;
         }

     .btn-excluir {
         background-color: #ef4444;
         color: white;
         border: none;
         padding: 6px 12px;
         border-radius: 4px;
         cursor: pointer;
         font-weight: 600;
         font-size: 0.9rem;
     }

         .btn-excluir:hover {
             background-color: #dc2626;
         }

     /* ==========================================
    ESTILOS DO MODAL
    ========================================== */
     .modal-overlay {
         position: fixed;
         top: 0;
         left: 0;
         width: 100vw;
         height: 100vh;
         background-color: rgba(0, 0, 0, 0.6);
         display: flex;
         justify-content: center;
         align-items: center;
         z-index: 1000;
     }

     .modal-conteudo {
         background: white;
         padding: 30px;
         border-radius: 8px;
         width: 400px;
         box-shadow: 0 10px 25px rgba(0,0,0,0.2);
     }

         .modal-conteudo h2 {
             margin-top: 0;
             color: #2c3e50;
             border-bottom: 1px solid #eaeaea;
             padding-bottom: 10px;
         }

     .grupo-input {
         display: flex;
         flex-direction: column;
         margin-bottom: 15px;
     }

         .grupo-input label {
             font-weight: 600;
             margin-bottom: 5px;
             color: #555;
             font-size: 0.9rem;
         }

         .grupo-input input, select {
             padding: 10px;
             border: 1px solid #ccc;
             border-radius: 4px;
             font-size: 1rem;
         }

     .campos-lado-a-lado {
         display: flex;
         gap: 15px;
     }

         .campos-lado-a-lado .grupo-input {
             flex: 1;
         }

     .asterisco {
         color: #ef4444;
         font-weight: bold;
     }

     .modal-botoes {
         display: flex;
         justify-content: flex-end;
         gap: 10px;
         margin-top: 25px;
     }

     .btn-cancelar {
         background-color: #f1f5f9;
         color: #475569;
         border: none;
         padding: 10px 20px;
         border-radius: 6px;
         cursor: pointer;
         font-weight: 600;
     }

     .btn-salvar {
         background-color: #3b82f6;
         color: white;
         border: none;
         padding: 10px 20px;
         border-radius: 6px;
         cursor: pointer;
         font-weight: 600;
     }
</style>