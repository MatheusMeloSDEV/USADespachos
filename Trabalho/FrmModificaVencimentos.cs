using CLUSA.Helpers;
using CLUSA.Models;
using CLUSA.Repositories;
using System.ComponentModel; // Necessário para o BindingList

namespace Trabalho
{
    public partial class FrmModificaVencimentos : Form
    {
        private readonly RepositorioVencimento _repoVencimento;
        private readonly RepositorioLog _repoLog;
        private string _idEdicao = null;
        public string _logadoNome;

        // Lista dinâmica para controlar a grid de eventos
        private BindingList<EventoVencimento> _listaEventos = new BindingList<EventoVencimento>();

        public class ImportadorOpcao { public string Nome { get; set; } public List<string> Cnpjs { get; set; } }

        public FrmModificaVencimentos(string idParaEditar = null)
        {
            InitializeComponent();
            _repoVencimento = new RepositorioVencimento();
            _repoLog = new RepositorioLog();
            _idEdicao = idParaEditar;

            CarregarComboBoxImportador();
            ConfigurarTelaDeEventos();

            if (!string.IsNullOrEmpty(_idEdicao))
            {
                CarregarDadosParaEdicao();
                this.Text = "Editar Vencimentos";
            }
        }

        private void ConfigurarTelaDeEventos()
        {
            // Opções de Tags disponíveis
            cbTagEvento.Items.AddRange(new string[] { "Radar", "Procuração", "ECAC", "SIGVIG", "LECOM", "Azeite", "Vinho" });
            cbTagEvento.DropDownStyle = ComboBoxStyle.DropDownList;

            // Vincula a lista ao GridView
            dgvEventos.DataSource = _listaEventos;
            dgvEventos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEventos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEventos.AllowUserToAddRows = false; // Usuário adiciona pelo botão
        }

        // Evento do botão de adicionar evento
        private void btnAdicionarEvento_Click(object sender, EventArgs e)
        {
            if (cbTagEvento.SelectedItem == null)
            {
                MessageBox.Show("Selecione uma Tag (Ex: Radar, Procuração).");
                return;
            }

            _listaEventos.Add(new EventoVencimento
            {
                Tag = cbTagEvento.SelectedItem.ToString(),
                Data = dtpDataEvento.Value.Date
            });
        }

        // Lógica de LOAD
        private async void CarregarDadosParaEdicao()
        {
            try
            {
                var v = await _repoVencimento.ObterPorIdAsync(_idEdicao);
                if (v != null)
                {
                    int index = cbImportador.FindStringExact(v.Importador);
                    if (index >= 0) cbImportador.SelectedIndex = index;

                    // Carrega os eventos salvos para a grid
                    _listaEventos.Clear();
                    if (v.Eventos != null)
                    {
                        foreach (var ev in v.Eventos)
                        {
                            _listaEventos.Add(ev);
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        // Lógica de SAVE
        private async void btnEnviar_Click(object sender, EventArgs e)
        {
            if (cbImportador.SelectedItem is ImportadorOpcao itemSelecionado)
            {
                try
                {
                    var vencimento = new Vencimento
                    {
                        Id = _idEdicao,
                        Importador = itemSelecionado.Nome,
                        Cnpjs = itemSelecionado.Cnpjs,
                        Eventos = _listaEventos.ToList(), // Salva a lista dinâmica
                        DataUltimaNotificacao = null
                    };

                    if (string.IsNullOrEmpty(_idEdicao))
                    {
                        await _repoVencimento.AdicionarAsync(vencimento);
                        await _repoLog.RegistrarLogAsync("Criação", _logadoNome, $"Novo vencimento criado para {vencimento.Importador}");
                    }
                    else
                    {
                        await _repoVencimento.AtualizarAsync(vencimento);
                        await _repoLog.RegistrarLogAsync("Edição", _logadoNome, $"Vencimento de {vencimento.Importador} alterado.", $"ID: {_idEdicao}");
                    }

                    MessageBox.Show("Salvo com sucesso!");
                    this.Close();
                }
                catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
            }
            else { MessageBox.Show("Selecione um importador."); }
        }

        private void CarregarComboBoxImportador()
        {
            var dadosBrutos = DadosEstaticos.ObterListaCNPJs();
            var listaParaCombo = dadosBrutos
                .GroupBy(x => x.Nome)
                .Select(g => new ImportadorOpcao { Nome = g.Key, Cnpjs = g.Select(i => i.Cnpj).ToList() })
                .OrderBy(x => x.Nome)
                .ToList();
            cbImportador.DataSource = listaParaCombo;
            cbImportador.DisplayMember = "Nome";
        }
    }
}