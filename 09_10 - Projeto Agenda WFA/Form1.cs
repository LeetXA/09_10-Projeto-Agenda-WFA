using _09_24___Projeto_Agenda;

namespace _09_10___Projeto_Agenda_WFA
{
    public partial class Form1 : Form
    {

        private readonly Contatos agenda = new Contatos();
        private readonly List<Telefone> telefones = new List<Telefone>();

        private TextBox txtNome = new TextBox();
        private TextBox txtEmail = new TextBox();
        private DateTimePicker dtNascimento = new DateTimePicker();
        private ComboBox cmbTipo = new ComboBox();
        private TextBox txtNumero = new TextBox();
        private CheckBox chkPrincipal = new CheckBox();
        private ListBox lstTelefones = new ListBox();
        private DataGridView gridContatos = new DataGridView();

        public Form1()
        {
            InitializeComponent();
            MontarTela();
            AtualizarLista();
        }

        private void MontarTela()
        {
            Text = "Agenda de Contatos";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(950, 700);
            MinimumSize = new Size(850, 600);

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(15);
            layout.ColumnCount = 2;
            layout.RowCount = 2;
            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 45));
            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 55));
            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 390));
            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            Controls.Add(layout);

            // Painel de cadastro
            GroupBox cadastro = new GroupBox();
            cadastro.Text = "Dados do contato";
            cadastro.Dock = DockStyle.Fill;
            layout.Controls.Add(cadastro, 0, 0);

            TableLayoutPanel campos = new TableLayoutPanel();
            campos.Dock = DockStyle.Fill;
            campos.Padding = new Padding(10);
            campos.ColumnCount = 2;
            campos.AutoScroll = true;
            campos.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 125));
            campos.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            cadastro.Controls.Add(campos);

            AdicionarCampo(campos, "Nome:", txtNome);
            AdicionarCampo(campos, "E-mail:", txtEmail);

            dtNascimento.Format = DateTimePickerFormat.Custom;
            dtNascimento.CustomFormat = "dd/MM/yyyy";
            dtNascimento.MaxDate = DateTime.Today;
            AdicionarCampo(campos, "Nascimento:", dtNascimento);

            cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipo.Items.AddRange(new object[]
            {
                "Celular", "Residencial", "Trabalho", "Outro"
            });
            cmbTipo.SelectedIndex = 0;
            AdicionarCampo(campos, "Tipo:", cmbTipo);
            AdicionarCampo(campos, "Número:", txtNumero);

            chkPrincipal.Text = "Telefone principal";
            chkPrincipal.AutoSize = true;
            AdicionarCampo(campos, "", chkPrincipal);

            Button btnAddTelefone = new Button();
            btnAddTelefone.Text = "Adicionar telefone";
            btnAddTelefone.AutoSize = true;
            btnAddTelefone.Click += (s, e) => AdicionarTelefone();
            AdicionarCampo(campos, "", btnAddTelefone);

            lstTelefones.Height = 70;
            lstTelefones.Dock = DockStyle.Fill;
            AdicionarCampo(campos, "Telefones:", lstTelefones);

            FlowLayoutPanel botoes = new FlowLayoutPanel();
            botoes.Dock = DockStyle.Fill;
            botoes.AutoSize = true;

            botoes.Controls.Add(CriarBotao("Adicionar", (s, e) =>
                SalvarContato(false)));
            botoes.Controls.Add(CriarBotao("Alterar", (s, e) =>
                SalvarContato(true)));
            botoes.Controls.Add(CriarBotao("Remover", (s, e) =>
                RemoverContato()));
            botoes.Controls.Add(CriarBotao("Limpar", (s, e) =>
                LimparCampos()));

            AdicionarCampo(campos, "", botoes);

            // Painel de pesquisa e listagem
            GroupBox lista = new GroupBox();
            lista.Text = "Contatos cadastrados";
            lista.Dock = DockStyle.Fill;
            layout.Controls.Add(lista, 1, 0);
            layout.SetRowSpan(lista, 2);

            gridContatos.Dock = DockStyle.Fill;
            gridContatos.ReadOnly = true;
            gridContatos.AllowUserToAddRows = false;
            gridContatos.AllowUserToDeleteRows = false;
            gridContatos.MultiSelect = false;
            gridContatos.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            gridContatos.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            gridContatos.RowHeadersVisible = false;

            gridContatos.Columns.Add("Nome", "Nome");
            gridContatos.Columns.Add("Email", "E-mail");
            gridContatos.Columns.Add("Telefone", "Telefone principal");

            gridContatos.SelectionChanged +=
                (s, e) => SelecionarContato();

            lista.Controls.Add(gridContatos);

            Label lblBusca = new Label();
            lblBusca.Text = "Selecione um contato na lista para consultar.";
            lblBusca.Dock = DockStyle.Bottom;
            lblBusca.Height = 30;
            lista.Controls.Add(lblBusca);
        }

        private void AdicionarCampo(
            TableLayoutPanel painel, string titulo, Control controle)
        {
            int linha = painel.RowCount++;
            painel.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            Label label = new Label();
            label.Text = titulo;
            label.AutoSize = true;
            label.Anchor = AnchorStyles.Left;
            label.Padding = new Padding(0, 6, 0, 0);

            controle.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            controle.Margin = new Padding(3, 4, 3, 4);

            painel.Controls.Add(label, 0, linha);
            painel.Controls.Add(controle, 1, linha);
        }

        private Button CriarBotao(
            string texto, EventHandler acao)
        {
            Button botao = new Button();
            botao.Text = texto;
            botao.AutoSize = true;
            botao.Click += acao;
            return botao;
        }

        private void AdicionarTelefone()
        {
            string numero = txtNumero.Text.Trim();

            if (string.IsNullOrWhiteSpace(numero) ||
                !numero.All(char.IsDigit))
            {
                MessageBox.Show(
                    "Digite um telefone usando somente números.");
                txtNumero.Focus();
                return;
            }

            if (chkPrincipal.Checked)
            {
                foreach (Telefone t in telefones)
                    t.Principal = false;
            }

            Telefone novo = new Telefone(
                cmbTipo.Text, numero, chkPrincipal.Checked);

            telefones.Add(novo);
            AtualizarTelefones();

            txtNumero.Clear();
            chkPrincipal.Checked = false;
        }

        private void AtualizarTelefones()
        {
            lstTelefones.Items.Clear();

            foreach (Telefone t in telefones)
                lstTelefones.Items.Add(t.ToString());
        }

        private Contato MontarContato()
        {
            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(
                    "Preencha o nome e o e-mail.");
            }

            DateTime data = dtNascimento.Value.Date;

            Contato contato = new Contato(
                email,
                nome,
                new Data(data.Day, data.Month, data.Year));

            foreach (Telefone t in telefones)
            {
                contato.AdicionarTelefone(
                    new Telefone(t.Tipo, t.Numero, t.Principal));
            }

            return contato;
        }

        private void SalvarContato(bool alterar)
        {
            try
            {
                Contato contato = MontarContato();
                bool sucesso;

                if (alterar)
                    sucesso = agenda.Alterar(contato);
                else
                    sucesso = agenda.Adicionar(contato);

                if (!sucesso)
                {
                    MessageBox.Show(alterar
                        ? "Contato não encontrado para alteração."
                        : "Esse e-mail já está cadastrado.");
                    return;
                }

                MessageBox.Show(alterar
                    ? "Contato alterado com sucesso!"
                    : "Contato adicionado com sucesso!");

                AtualizarLista();
                LimparCampos();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Atenção");
            }
        }

        private void SelecionarContato()
        {
            if (gridContatos.SelectedRows.Count == 0)
                return;

            int indice = gridContatos.SelectedRows[0].Index;
            var contatos = agenda.Listar();

            if (indice < 0 || indice >= contatos.Count)
                return;

            Contato contato = contatos[indice];

            txtNome.Text = contato.Nome;
            txtEmail.Text = contato.Email;

            dtNascimento.Value = new DateTime(
                contato.DtNasc.Ano,
                contato.DtNasc.Mes,
                contato.DtNasc.Dia);

            telefones.Clear();

            foreach (Telefone t in contato.Telefones)
            {
                telefones.Add(new Telefone(
                    t.Tipo, t.Numero, t.Principal));
            }

            AtualizarTelefones();
        }

        private void RemoverContato()
        {
            if (gridContatos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um contato para remover.");
                return;
            }

            int indice = gridContatos.SelectedRows[0].Index;
            var contatos = agenda.Listar();

            if (indice < 0 || indice >= contatos.Count)
                return;

            Contato contato = contatos[indice];

            DialogResult resposta = MessageBox.Show(
                $"Deseja remover {contato.Nome}?",
                "Confirmar remoção",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resposta != DialogResult.Yes)
                return;

            if (agenda.Remover(contato))
            {
                AtualizarLista();
                LimparCampos();
                MessageBox.Show("Contato removido com sucesso!");
            }
        }

        private void AtualizarLista()
        {
            gridContatos.Rows.Clear();

            foreach (Contato c in agenda.Listar())
            {
                gridContatos.Rows.Add(
                    c.Nome,
                    c.Email,
                    c.GetTelefonePrincipal());
            }
        }

        private void LimparCampos()
        {
            txtNome.Clear();
            txtEmail.Clear();
            dtNascimento.Value = DateTime.Today;
            cmbTipo.SelectedIndex = 0;
            txtNumero.Clear();
            chkPrincipal.Checked = false;
            telefones.Clear();
            AtualizarTelefones();
            gridContatos.ClearSelection();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
