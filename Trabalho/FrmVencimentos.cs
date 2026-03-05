using CLUSA.Services;
using CLUSA.Repositories;
using System.Data;

namespace Trabalho
{
    public partial class FrmVencimentos : Form
    {
        private readonly RepositorioVencimento _repoVencimento;
        private readonly RepositorioLog _repoLog;
        public string _logadoNome;
        public FrmVencimentos()
        {
            InitializeComponent();
            _repoVencimento = new RepositorioVencimento();
            _repoLog = new RepositorioLog();
            // Configurações visuais do Grid
            ConfigurarGrid();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Carrega o Grid
            await AtualizarGrid();

            // Roda a verificação de e-mails em segundo plano
            await VerificarNotificacoesAutomaticas();
        }

        private async System.Threading.Tasks.Task VerificarNotificacoesAutomaticas()
        {
            var todos = await _repoVencimento.ObterTodosAsync();

            DateTime hoje = DateTime.Today;

            DateTime dataMinima = hoje.AddDays(14);
            DateTime dataMaxima = hoje.AddMonths(1);

            int emailsEnviados = 0;

            foreach (var item in todos)
            {
                if (item.DataUltimaNotificacao.HasValue)
                {
                    TimeSpan tempoDesdeUltimoAviso = hoje - item.DataUltimaNotificacao.Value;
                    if (tempoDesdeUltimoAviso.TotalDays < 20)
                    {
                        continue;
                    }
                }

                string avisos = "";

                if (EstaNaJanela(item.DataVencimentoRadar, dataMinima, dataMaxima))
                    avisos += $"- RADAR vence em {item.DataVencimentoRadar:dd/MM/yyyy}\n";

                if (EstaNaJanela(item.DataVencimentoProcuracao, dataMinima, dataMaxima))
                    avisos += $"- PROCURAÇÃO vence em {item.DataVencimentoProcuracao:dd/MM/yyyy}\n";

                if (EstaNaJanela(item.DataVencimentoEcac, dataMinima, dataMaxima))
                    avisos += $"- ECAC vence em {item.DataVencimentoEcac:dd/MM/yyyy}\n";

                if (EstaNaJanela(item.DataVencimentoSigvig, dataMinima, dataMaxima))
                    avisos += $"- SIGVIG vence em {item.DataVencimentoSigvig:dd/MM/yyyy}\n";

                if (EstaNaJanela(item.DataVencimentoLecom, dataMinima, dataMaxima))
                    avisos += $"- LECOM vence em {item.DataVencimentoLecom:dd/MM/yyyy}\n";

                if (EstaNaJanela(item.DataVencimentoAzeite, dataMinima, dataMaxima))
                    avisos += $"- AZEITE vence em {item.DataVencimentoAzeite:dd/MM/yyyy}\n";

                if (EstaNaJanela(item.DataVencimentoVinho, dataMinima, dataMaxima))
                    avisos += $"- VINHO vence em {item.DataVencimentoVinho:dd/MM/yyyy}\n";

                if (!string.IsNullOrEmpty(avisos))
                {
                    string textoCnpjs = (item.Cnpjs != null && item.Cnpjs.Count > 0)
                                        ? string.Join(", ", item.Cnpjs)
                                        : "Nenhum CNPJ registado";

                    string assunto = $"[ALERTA] Vencimentos Próximos: {item.Importador}";

                    // Dica: Use <br> se quiser garantir quebra de linha no HTML do email
                    string corpo = $"O cliente <b>{item.Importador}</b> tem documentos a vencer em breve.<br><br>" +
                                   $"CNPJs Vinculados: {textoCnpjs}<br><br>" +
                                   $"Itens a vencer:<br>{avisos.Replace("\n", "<br>")}<br>" +
                                   $"Por favor, verifique no sistema.";

                    // --- AQUI A MUDANÇA ---
                    // Usa o novo método que aceita só texto com a conta corporativa
                    await EmailService.EnviarFollowUpTextoAsync(assunto, corpo);
                    // ----------------------

                    await _repoLog.RegistrarLogAsync("Notificação", _logadoNome, $"E-mail automático enviado para {item.Importador}", "Enviado pelo sistema Anti-Spam");

                    item.DataUltimaNotificacao = hoje;
                    await _repoVencimento.AtualizarAsync(item);

                    emailsEnviados++;
                }
            }
            if (emailsEnviados > 0)
            {
                MessageBox.Show($"{emailsEnviados} notificação(ões) enviada(s).", "Notificação Automática");
            }
        }

        private bool EstaNaJanela(DateTime? dataDoBanco, DateTime min, DateTime max)
        {
            if (!dataDoBanco.HasValue) return false;

            DateTime data = dataDoBanco.Value.Date;

            // Retorna VERDADEIRO se a data estiver entre o Mínimo (2 semanas) e o Máximo (1 Mês)
            return data >= min.Date && data <= max.Date;
        }

        private void ConfigurarGrid()
        {
            // Opcional: Deixa o grid com seleção de linha inteira
            DGVVencimentos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGVVencimentos.MultiSelect = false;
            DGVVencimentos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGVVencimentos.ReadOnly = true;
        }

        private async Task AtualizarGrid()
        {
            try
            {
                var listaDoBanco = await _repoVencimento.ObterTodosAsync();

                var listaParaExibir = listaDoBanco.Select(x => new
                {
                    Id = x.Id,
                    Importador = x.Importador,

                    // As datas (mantivemos a lógica do traço se for nulo)
                    Radar = x.DataVencimentoRadar.HasValue ? x.DataVencimentoRadar.Value.ToShortDateString() : "-",
                    Procuração = x.DataVencimentoProcuracao.HasValue ? x.DataVencimentoProcuracao.Value.ToShortDateString() : "-",
                    ECAC = x.DataVencimentoEcac.HasValue ? x.DataVencimentoEcac.Value.ToShortDateString() : "-",
                    SIGVIG = x.DataVencimentoSigvig.HasValue ? x.DataVencimentoSigvig.Value.ToShortDateString() : "-",
                    LECOM = x.DataVencimentoLecom.HasValue ? x.DataVencimentoLecom.Value.ToShortDateString() : "-",
                    Azeite = x.DataVencimentoAzeite.HasValue ? x.DataVencimentoAzeite.Value.ToShortDateString() : "-",
                    Vinho = x.DataVencimentoVinho.HasValue ? x.DataVencimentoVinho.Value.ToShortDateString() : "-",


                    // --- NOVA COLUNA DE CNPJs ---
                    // Verifica se a lista existe. Se sim, junta com ", ". Se não, põe um traço.
                    CNPJs = (x.Cnpjs != null && x.Cnpjs.Count > 0)
                            ? string.Join(", ", x.Cnpjs)
                            : "-"
                }).ToList();

                DGVVencimentos.DataSource = listaParaExibir;

                // --- AJUSTES VISUAIS ---

                // Esconde o ID
                if (DGVVencimentos.Columns["Id"] != null)
                    DGVVencimentos.Columns["Id"].Visible = false;

                // Renomeia o cabeçalho da coluna de CNPJs para ficar mais bonito
                if (DGVVencimentos.Columns["CNPJs"] != null)
                {
                    DGVVencimentos.Columns["CNPJs"].HeaderText = "CNPJs da Empresa";
                    // Opcional: Define uma largura mínima pois CNPJs ocupam espaço
                    DGVVencimentos.Columns["CNPJs"].MinimumWidth = 200;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados: " + ex.Message);
            }
        }

        // BOTÃO ADICIONAR (toolStripButton1 na sua imagem)
        private async void btnAdicionar_Click(object sender, EventArgs e)
        {
            // Abre o formulário de modificação como um diálogo (modal)
            FrmModificaVencimentos frm = new FrmModificaVencimentos() { _logadoNome = _logadoNome };

            // O Form.ShowDialog pausa este código até o outro fechar
            frm.ShowDialog();

            // Quando fechar, atualiza o grid para mostrar o novo registro
            await AtualizarGrid();
        }
        private async void btnEditar_Click(object sender, EventArgs e)
        {
            // 1. Verifica se o usuário selecionou alguma linha
            if (DGVVencimentos.SelectedRows.Count > 0)
            {
                // 2. Pega o ID oculto da linha selecionada
                // "Id" vem do objeto anônimo que criamos no método AtualizarGrid
                string idSelecionado = DGVVencimentos.SelectedRows[0].Cells["Id"].Value.ToString();

                // 3. Cria o formulário PASSANDO O ID
                // Isso ativa o modo de edição dentro do FrmModificaVencimentos
                FrmModificaVencimentos frm = new FrmModificaVencimentos(idSelecionado) { _logadoNome = _logadoNome };

                // 4. Abre a janela e espera ela fechar
                frm.ShowDialog();

                // 5. Quando voltar, atualiza o grid para exibir as alterações
                await AtualizarGrid();
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma linha para editar.");
            }
        }
        // BOTÃO REMOVER (toolStripButton4 na sua imagem)
        private async void btnRemover_Click(object sender, EventArgs e)
        {
            // Verifica se tem linha selecionada
            if (DGVVencimentos.SelectedRows.Count > 0)
            {
                // Pega o ID da linha selecionada (Coluna "Id" que ocultamos ou a primeira célula)
                // Nota: Cells[0] assume que Id é a primeira propriedade do objeto anônimo criado no Select
                string idParaRemover = DGVVencimentos.SelectedRows[0].Cells["Id"].Value.ToString();
                string nomeImportador = DGVVencimentos.SelectedRows[0].Cells["Importador"].Value.ToString();

                var confirmacao = MessageBox.Show(
                    "Tem certeza que deseja remover este vencimento?",
                    "Confirmar Exclusão",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmacao == DialogResult.Yes)
                {
                    try
                    {
                        await _repoVencimento.ExcluirAsync(idParaRemover);
                        await _repoLog.RegistrarLogAsync("Exclusão", _logadoNome, $"O usuário removeu o vencimento de {nomeImportador}", $"ID Removido: {idParaRemover}");
                        await AtualizarGrid(); // Recarrega a tabela
                        MessageBox.Show("Removido com sucesso.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao excluir: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione uma linha para remover.");
            }
        }
    }
}