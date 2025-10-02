using CLUSA;
using MongoDB.Driver;
using System.ComponentModel;
using System.Diagnostics;

namespace Trabalho
{
    public partial class frmSantos : Form
    {
        private readonly RepositorioProcesso _repositorio;
        private int _estadoOrdenacaoRefUsa = 0;
        private DataGridViewColumn? _colunaOrdenada;
        private ListSortDirection _direcaoOrdenacao;
        private List<Processo> _listaOriginal = new();

        public frmSantos()
        {
            InitializeComponent();
            _repositorio = new RepositorioProcesso();
            this.Shown += FrmSantos_Shown;
        }
        private async void FrmSantos_Shown(object? sender, EventArgs e)
        {
            try
            {
                ConfigurarColunasDataGridViewProcesso();
                await CarregarDadosAsync();

                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

                PopularComboBoxDePesquisa();

                if (CmbPesquisar.Items.Count > 0)
                {
                    CmbPesquisar.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o formulário: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task CarregarDadosAsync()
        {
            try
            {
                var registros = await _repositorio.ListarExcetoSufixoRefUsaAsync("ITJ");
                var registrosOrdenados = registros.OrderBy(p => ExtrairAnoNumero(p.Ref_USA)).ToList();
                _listaOriginal = registrosOrdenados;

                BsProcesso.DataSource = registrosOrdenados;
                DGVSantos.DataSource = BsProcesso;
                BsProcesso.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ConfigurarColunasDataGridViewProcesso()
        {
            DGVSantos.Columns.Clear();

            // --- Configuração Geral da Grade ---
            DGVSantos.AutoGenerateColumns = false;
            DGVSantos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Mantém o preenchimento
            DGVSantos.RowHeadersVisible = false;
            DGVSantos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            DGVSantos.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            DGVSantos.ShowCellToolTips = true;
            var dateCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" };

            // --- Adicionando as Colunas com Largura Mínima ---

            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Ref_USA",
                HeaderText = "Ref. USA",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 90
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SR",
                HeaderText = "S. Ref",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 80
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Importador",
                HeaderText = "Importador",
                FillWeight = 140,
                MinimumWidth = 140 // <-- MUDANÇA
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Exportador",
                HeaderText = "Exportador",
                FillWeight = 140,
                MinimumWidth = 140 // <-- MUDANÇA
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Produto",
                HeaderText = "Produto",
                FillWeight = 180,
                MinimumWidth = 200 // <-- MUDANÇA
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Container",
                HeaderText = "Container",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 120
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PortoDestino",
                HeaderText = "Porto Destino",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 110
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Veiculo",
                HeaderText = "Veículo",
                FillWeight = 110,
                MinimumWidth = 120 // <-- MUDANÇA
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataDeAtracacao",
                HeaderText = "Data de Atracação",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 70 // <-- MUDANÇA
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "OrgaosAnuentesString",
                HeaderText = "Anuente",
                FillWeight = 90,
                MinimumWidth = 100 // <-- MUDANÇA
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FreeTime",
                HeaderText = "F.T (dias)",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 70 // <-- MUDANÇA
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VencimentoFreeTime",
                HeaderText = "Venc. F. Time",
                DefaultCellStyle = dateCellStyle,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 100 // <-- MUDANÇA
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VencimentoFMA",
                HeaderText = "Venc. FMA",
                DefaultCellStyle = dateCellStyle,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 100 // <-- MUDANÇA
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DI",
                HeaderText = "DI",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 120
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "HistoricoDoProcesso",
                HeaderText = "Histórico",
                FillWeight = 200,
                MinimumWidth = 200 // <-- MUDANÇA
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Pendencia",
                HeaderText = "Pendência",
                FillWeight = 160,
                MinimumWidth = 160 // <-- MUDANÇA
            });
            DGVSantos.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status",
                FillWeight = 110,
                MinimumWidth = 120 // <-- MUDANÇA
            });

            // Aplica um estilo padrão a todas as colunas
            foreach (DataGridViewColumn coluna in DGVSantos.Columns)
            {
                coluna.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                coluna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }

        // Cole este método dentro de frmSantos.cs e também em frmItajai.cs

        private async void DGV_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Garante que o clique não foi no cabeçalho da coluna
            if (e.RowIndex < 0) return;

            // Pega o processo selecionado na linha clicada a partir do BindingSource
            if (BsProcesso.Current is not Processo processoSelecionado)
            {
                MessageBox.Show("Não foi possível identificar o processo selecionado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Abre o formulário de edição passando o processo selecionado
                using var frm = new FrmModificaProcesso
                {
                    processo = processoSelecionado,
                    Modo = "Visualizar",
                    Visualização = true // <-- Esta é a chave para travar os campos na tela de edição
                };

                frm.ShowDialog();

                // Após fechar a tela de visualização, recarrega a grade para garantir que os
                // dados estejam sempre atualizados, caso outro usuário tenha feito alguma alteração.
                await CarregarDadosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir a tela de visualização: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopularComboBoxDePesquisa()
        {
            var camposIgnorados = new HashSet<string>
            {
                "Id", "OrgaosAnuentesEnvolvidos",
                "PossuiEmbarque", "VencimentoFreeTime", "VencimentoFMA"
            };

            CmbPesquisar.Items.Clear();

            foreach (DataGridViewColumn coluna in DGVSantos.Columns)
            {
                if (!string.IsNullOrEmpty(coluna.DataPropertyName) && !camposIgnorados.Contains(coluna.DataPropertyName))
                {
                    CmbPesquisar.Items.Add(new DisplayItem(coluna.DataPropertyName, coluna.HeaderText));
                }
            }
        }

        private async Task ConfigurarAutoCompletarParaPesquisaAsync()
        {
            if (CmbPesquisar.SelectedItem is not DisplayItem campoSelecionado) return;

            try
            {
                // MUDANÇA: Chamada assíncrona ao repositório.
                var valores = await _repositorio.ObterValoresUnicosAsync(campoSelecionado.DataPropertyName);
                var collection = new AutoCompleteStringCollection();
                collection.AddRange(valores.ToArray());

                TxtPesquisar.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                TxtPesquisar.AutoCompleteSource = AutoCompleteSource.CustomSource;
                TxtPesquisar.AutoCompleteCustomSource = collection;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao configurar o autocompletar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private async void BtnAdicionar_Click(object sender, EventArgs e)
        {
            var processo = new Processo();
            OrigemProcesso Santos = OrigemProcesso.Santos;
            using var frm = new FrmModificaProcesso { processo = processo, Modo = "Adicionar", Origem = Santos };

            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // A nova versão do repositório cuida de TUDO (salvar em PROCESSO, MAPA, ANVISA, etc.)
                    await _repositorio.CreateAsync(processo);
                    await CarregarDadosAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao adicionar o processo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            await CarregarDadosAsync();
        }
        private async void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (BsProcesso.Current is not Processo processoSelecionado)
            {
                MessageBox.Show("Nenhum processo selecionado para exclusão.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show($"Tem certeza que deseja excluir o processo '{processoSelecionado.Ref_USA}'?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    // O repositório exclui o processo principal e TODOS os relacionados.
                    await _repositorio.DeleteAsync(processoSelecionado.Id.ToString());
                    BsProcesso.Remove(processoSelecionado);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir o processo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private async void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (BsProcesso.Current is not Processo processoSelecionado)
            {
                MessageBox.Show("Nenhum processo selecionado para edição.", "Aviso");
                return;
            }

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Modo = "Editar" };
            frm.ShowDialog();

            await CarregarDadosAsync();
        }

        private async void BtnPesquisar_Click(object sender, EventArgs e)
        {
            if (CmbPesquisar.SelectedItem is not DisplayItem campoSelecionado)
            {
                MessageBox.Show("Selecione um campo para pesquisar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var pesquisa = TxtPesquisar.Text;
            if (string.IsNullOrWhiteSpace(pesquisa))
            {
                // Se a pesquisa estiver vazia, recarrega todos os dados originais.
                BsProcesso.DataSource = _listaOriginal;
                BsProcesso.ResetBindings(false);
                return;
            }

            try
            {
                // MUDANÇA: Chamada assíncrona.
                var resultados = await _repositorio.PesquisarAsync(campoSelecionado.DataPropertyName, pesquisa);
                BsProcesso.DataSource = resultados;
                BsProcesso.ResetBindings(false);

                if (!resultados.Any())
                {
                    MessageBox.Show("Nenhum resultado encontrado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao pesquisar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void DGVSantos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || BsProcesso.Current is not Processo processoSelecionado) return;

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Visualização = true, Modo = "Visualizar" };
            frm.ShowDialog();

            await CarregarDadosAsync();
        }

        private async void BtnExportar_Click(object sender, EventArgs e)
        {
            // Obtém a lista de importadores únicos do repositório
            var importadores = await _repositorio.ObterValoresUnicosAsync("Importador");

            // Exibe um formulário para seleção do importador
            using var form = new ImporterSelectionForm(importadores);
            if (form.ShowDialog() == DialogResult.OK)
            {
                string importador = form.SelectedImporter;

                // 1) Cria sem using
                var progressForm = new ProgressForm();
                progressForm.Show(this);       // exibe modeless, com o próprio Form como owner


                await Task.Run(() =>
                {
                    string pdfPath = "";
                    string? mensagemErro = null;

                    try
                    {
                        pdfPath = PythonRunner.ExecutarExportador(importador).Trim();
                    }
                    catch (Exception ex)
                    {
                        mensagemErro = $"Erro durante exportação: {ex.Message}";
                    }

                    Invoke(new Action(() =>
                    {
                        progressForm.Close();
                        progressForm.Dispose();

                        if (mensagemErro != null)
                        {
                            MessageBox.Show(mensagemErro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var resp = MessageBox.Show(
                            "Exportação concluída. Deseja abrir o PDF?",
                            "Resultado",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (resp == DialogResult.Yes && File.Exists(pdfPath))
                        {
                            try
                            {
                                // Abre o PDF com o aplicativo padrão, usando o Explorer
                                Process.Start("explorer.exe", pdfPath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Erro ao tentar abrir o PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                    }));
                });
            }
        }


        private async void CmbPesquisar_SelectedIndexChanged(object? sender, EventArgs e)
        {
            await ConfigurarAutoCompletarParaPesquisaAsync();
        }
        private async void BtnCancelar_Click(object sender, EventArgs e)
        {
            await CarregarDadosAsync();
            TxtPesquisar.Clear();
        }

        private void FrmProcesso_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TxtPesquisar.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }


        public class DisplayItem
        {
            public string DataPropertyName { get; }
            public string HeaderText { get; }

            public DisplayItem(string dataPropertyName, string headerText)
            {
                DataPropertyName = dataPropertyName;
                HeaderText = headerText;
            }

            public override string ToString() => HeaderText;
        }
        private bool IsValueEmpty(object? value)
        {
            if (value == null || value == DBNull.Value)
            {
                return true;
            }
            if (value is string str)
            {
                return string.IsNullOrWhiteSpace(str);
            }
            return false;
        }
        private (int ano, int numero) ExtrairAnoNumero(string refUsa)
        {
            if (string.IsNullOrWhiteSpace(refUsa)) return (0, 0);
            string refLimpa = refUsa.Split(' ').FirstOrDefault() ?? refUsa;
            var partes = refLimpa.Split('/');
            int numero = 0, ano = 0;
            if (partes.Length == 2)
            {
                int.TryParse(partes[0], out numero);
                int.TryParse(partes[1], out ano);
            }
            return (ano, numero);
        }


        // Substitua seu método de clique no cabeçalho por este
        private void DGV_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView dgv) return;
            var novaColuna = dgv.Columns[e.ColumnIndex];
            if (novaColuna.SortMode == DataGridViewColumnSortMode.NotSortable) return;
            if (BsProcesso.DataSource is not List<Processo> listaParaOrdenar) return;

            // 1. Determina a Direção da Ordenação (mesma lógica de antes)
            ListSortDirection direcao;
            if (_colunaOrdenada == null || _colunaOrdenada.Name != novaColuna.Name)
            {
                direcao = ListSortDirection.Ascending;
            }
            else
            {
                direcao = (_direcaoOrdenacao == ListSortDirection.Ascending)
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }

            _colunaOrdenada = novaColuna;
            _direcaoOrdenacao = direcao;

            IEnumerable<Processo> listaOrdenada;

            // 2. Aplica a Lógica de Ordenação em Dois Níveis
            if (novaColuna.DataPropertyName == "Ref_USA")
            {
                // --- LÓGICA ESPECIAL PARA REF_USA ---
                var orderedByEmptiness = listaParaOrdenar
                    // NÍVEL 1: Jogar Ref_USA vazias para o final
                    .OrderBy(p => IsValueEmpty(p.Ref_USA) ? 1 : 0);

                listaOrdenada = direcao == ListSortDirection.Ascending
                    // NÍVEL 2: Ordenar as restantes pelo critério especial
                    ? orderedByEmptiness.ThenBy(p => ExtrairAnoNumero(p.Ref_USA))
                    : orderedByEmptiness.ThenByDescending(p => ExtrairAnoNumero(p.Ref_USA));
            }
            else
            {
                // --- LÓGICA GENÉRICA PARA OUTRAS COLUNAS ---
                var propInfo = typeof(Processo).GetProperty(novaColuna.DataPropertyName);
                if (propInfo == null) return;

                var orderedByEmptiness = listaParaOrdenar
                    // NÍVEL 1: Jogar valores vazios da coluna genérica para o final
                    .OrderBy(p => IsValueEmpty(propInfo.GetValue(p)) ? 1 : 0);

                // NÍVEL 2: Ordenar os valores restantes, com tratamento para datas
                if (propInfo.PropertyType == typeof(DateTime) || propInfo.PropertyType == typeof(DateTime?))
                {
                    listaOrdenada = direcao == ListSortDirection.Ascending
                        ? orderedByEmptiness.ThenBy(p => (DateTime?)propInfo.GetValue(p) ?? DateTime.MinValue)
                        : orderedByEmptiness.ThenByDescending(p => (DateTime?)propInfo.GetValue(p) ?? DateTime.MinValue);
                }
                else
                {
                    listaOrdenada = direcao == ListSortDirection.Ascending
                        ? orderedByEmptiness.ThenBy(p => propInfo.GetValue(p))
                        : orderedByEmptiness.ThenByDescending(p => propInfo.GetValue(p));
                }
            }

            // 3. Atualiza o DataGridView (mesma lógica de antes)
            BsProcesso.DataSource = listaOrdenada.ToList();
            BsProcesso.ResetBindings(false);

            // 4. Atualiza a Seta Visual (Glyph) no Cabeçalho (mesma lógica de antes)
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.HeaderCell.SortGlyphDirection = (column.Name == novaColuna.Name)
                    ? (direcao == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending)
                    : SortOrder.None;
            }
        }

        private void BtnAjuda_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;
        }
    }
}
