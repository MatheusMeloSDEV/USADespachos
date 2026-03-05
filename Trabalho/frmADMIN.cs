using CLUSA.Repositories;
using CLUSA.Models;

namespace Trabalho
{
    public partial class FrmAdmin : Form
    {
        private readonly RepositorioUsers repositorio;
        private readonly RepositorioLog _logRepo;
        public string usuarioLogadoNome;

        public FrmAdmin()
        {
            InitializeComponent();
            repositorio = new RepositorioUsers();
            _logRepo = new RepositorioLog();
            usuarioLogadoNome = FrmLogin.Instance?.Logado?.Usuario ?? "Desconhecido";
        }

        private async void FrmADMIN_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

                // 1. Carrega os Usuários
                await AtualizarGridUsuarios();

                // 2. Carrega os Logs (AQUI ESTAVA FALTANDO)
                await AtualizarGridLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}");
            }
        }

        // --- Método Específico para o Grid de Usuários ---
        private async Task AtualizarGridUsuarios()
        {
            var listaUsuarios = await repositorio.FindAllAsync();

            // Configuração para evitar o Grid Cinza
            DGVAdmin.DataSource = null;
            DGVAdmin.AutoGenerateColumns = true;

            BSAdmin.DataSource = listaUsuarios;
            DGVAdmin.DataSource = BSAdmin;

            if (DGVAdmin.Columns["Username"] != null)
            {
                DGVAdmin.Columns["Username"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // Esconde colunas técnicas
            if (DGVAdmin.Columns["PreferenciasGrids"] != null) DGVAdmin.Columns["PreferenciasGrids"].Visible = false;
            if (DGVAdmin.Columns["Password"] != null) DGVAdmin.Columns["Password"].Visible = false;
            if (DGVAdmin.Columns["Id"] != null) DGVAdmin.Columns["Id"].Visible = false;
            if (DGVAdmin.Columns["_id"] != null) DGVAdmin.Columns["_id"].Visible = false;
        }

        // --- Método Específico para o Grid de Logs (dgvLogs) ---
        private async Task AtualizarGridLogs()
        {
            try
            {
                // Busca os últimos 50 logs do repositório
                var listaLogs = await _logRepo.ObterUltimosAsync(50);

                // Configuração de segurança para evitar o Grid Cinza
                dgvLogs.DataSource = null;
                dgvLogs.AutoGenerateColumns = true;
                dgvLogs.DataSource = listaLogs;

                // --- Formatação Visual ---

                // 1. Esconde IDs
                if (dgvLogs.Columns["Id"] != null) dgvLogs.Columns["Id"].Visible = false;
                if (dgvLogs.Columns["_id"] != null) dgvLogs.Columns["_id"].Visible = false;

                // Variável auxiliar para controlar a ordem exata das colunas da esquerda para a direita
                int indexOrdem = 0;

                // 2. Formata Data (Posição 0)
                if (dgvLogs.Columns["DataHora"] != null)
                {
                    dgvLogs.Columns["DataHora"].HeaderText = "Data/Hora";
                    dgvLogs.Columns["DataHora"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                    dgvLogs.Columns["DataHora"].Width = 140;
                    dgvLogs.Columns["DataHora"].DisplayIndex = indexOrdem++;
                }

                // 3. Formata Autor (Posição 1 - Logo após a Data)
                if (dgvLogs.Columns["Autor"] != null)
                {
                    dgvLogs.Columns["Autor"].HeaderText = "Usuário";
                    dgvLogs.Columns["Autor"].Width = 100;
                    dgvLogs.Columns["Autor"].DisplayIndex = indexOrdem++;
                }

                // 4. Formata Ação (Posição 2)
                if (dgvLogs.Columns["TipoAcao"] != null)
                {
                    dgvLogs.Columns["TipoAcao"].HeaderText = "Ação";
                    dgvLogs.Columns["TipoAcao"].Width = 70; // Aumentado um pouquinho para caber "Exclusão"
                    dgvLogs.Columns["TipoAcao"].DisplayIndex = indexOrdem++;
                }

                // 5. Ajusta Mensagem (Posição 3)
                if (dgvLogs.Columns["Mensagem"] != null)
                {
                    dgvLogs.Columns["Mensagem"].Width = 250;
                    dgvLogs.Columns["Mensagem"].DisplayIndex = indexOrdem++;
                }

                // 6. Ajusta Detalhes (Posição 4)
                if (dgvLogs.Columns["Detalhes"] != null)
                {
                    dgvLogs.Columns["Detalhes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Preenche o resto
                    dgvLogs.Columns["Detalhes"].DisplayIndex = indexOrdem++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao carregar logs: " + ex.Message);
            }
        }

        // --- ATUALIZE OS BOTÕES PARA RECARREGAR OS LOGS ---

        private async void BtnAdicionar_Click(object sender, EventArgs e)
        {
            var novoUsuario = new Users();
            using var form = new FrmModificaAdmin(novoUsuario);

            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await repositorio.CreateAsync(novoUsuario);

                    await _logRepo.RegistrarLogAsync("Criação Usuário", usuarioLogadoNome, $"Novo usuário '{novoUsuario.Username}' criado.", $"Admin: {usuarioLogadoNome}");

                    await AtualizarGridUsuarios();
                    await AtualizarGridLogs(); // <--- Atualiza o log na tela
                }
                catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
            }
        }

        private async void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (BSAdmin.Current is not Users usuarioParaExcluir) return;
            if (usuarioParaExcluir.Username == usuarioLogadoNome) { MessageBox.Show("Não pode excluir a si mesmo."); return; }

            if (MessageBox.Show($"Excluir '{usuarioParaExcluir.Username}'?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    await repositorio.DeleteAsync(usuarioParaExcluir);

                    await _logRepo.RegistrarLogAsync("Exclusão Usuário", usuarioLogadoNome, $"Usuário '{usuarioParaExcluir.Username}' excluído.", $"Admin: {usuarioLogadoNome}");

                    await AtualizarGridUsuarios();
                    await AtualizarGridLogs(); // <--- Atualiza o log na tela
                }
                catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
            }
        }

        private async void BtnEditar_Click(object sender, EventArgs e)
        {
            if (BSAdmin.Current is not Users usuarioSelecionado) return;

            using var form = new FrmModificaAdmin(usuarioSelecionado);

            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await repositorio.UpdateAsync(usuarioSelecionado);

                    await _logRepo.RegistrarLogAsync("Edição Usuário", usuarioLogadoNome, $"Usuário '{usuarioSelecionado.Username}' editado.", $"Admin: {usuarioLogadoNome}");

                    await AtualizarGridUsuarios();
                    await AtualizarGridLogs(); // <--- Atualiza o log na tela
                }
                catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
            }
        }
    }
}