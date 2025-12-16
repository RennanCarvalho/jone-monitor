using System.Windows.Forms;

namespace jone_fora
{
    public partial class JoneFora : Form
    {
        private List<Log> _logs = new();
        private List<Data> _datas = new();
        private Data? _data;
        private TimeSpan _cronometro;
        private bool _clickThrough = true;

        public JoneFora()
        {
            InitializeComponent();

            _logs = Get<Log>.Lista();
            _datas = Get<Data>.Lista();
            _data = _datas.Find(x => x.DataAtual == DateOnly.FromDateTime(DateTime.Today));

            if (_data is null)
            {
                _data = new();
                _datas.Add(_data);
            }

            _cronometro = _data.QuantidadeMinutos;
            ShowInTaskbar = false;


        }

        private void JoneFora_Load(object sender, EventArgs e)
        {
            AtualizarLabels();
        }

        private void AtualizarLabels()
        {
            var totalDias = _datas.Count;
            var totalSaidas = _datas.Sum(x => x.QuantidadeSaidas);
            var totalTempoTicks = _datas.Sum(x => x.QuantidadeMinutos.Ticks);

            // média de tempo por saída
            var mediaTempo = totalSaidas == 0
                ? TimeSpan.Zero
                : TimeSpan.FromTicks(totalTempoTicks / totalSaidas);

            // média de saída por dia
            var mediaSaidas = totalDias == 0
                ? 0
                : (double)totalSaidas / totalDias;

            /* Labels */
            CronometroLabel.Text = _cronometro.ToString(@"hh\:mm\:ss");
            DataLabel.Text = $"{_data!.DataAtual:dd/MM/yyyy}";
            SaidaLabel.Text = $"Saídas      : {_data.QuantidadeSaidas}";
            SaidaPorDiaLabel.Text = $"Saída/Dia   : {mediaSaidas}";
            TempoPorSaidaLabel.Text = $"Tempo/Saída : {mediaTempo:mm\\:ss}";
        }

        private void EventoBt_Click(object sender, EventArgs e)
        {
            if (EventoBt.Text == "Saiu") _data!.QuantidadeSaidas++;
            Cronometro.Enabled = !Cronometro.Enabled;
            _logs.Add(new Log { DataHora = DateTime.Now, Evento = EventoBt.Text });
            EventoBt.Text = EventoBt.Text == "Saiu" ? "Voltou" : "Saiu";
            SaidaLabel.Text = $"Saídas      : {_data!.QuantidadeSaidas.ToString()}";
            AtualizarLabels();
        }

        private void Cronometro_Tick(object sender, EventArgs e)
        {
            _cronometro = _cronometro.Add(TimeSpan.FromSeconds(1));
            CronometroLabel.Text = _cronometro.ToString(@"hh\:mm\:ss");
        }

        private void JoneFora_FormClosing(object sender, FormClosingEventArgs e)
        {
            _data!.QuantidadeMinutos = _cronometro;
            Get<Log>.Salvar(_logs);
            Get<Data>.Salvar(_datas);
        }

        private void JoneFora_Shown(object sender, EventArgs e)
        {
            var area = Screen.PrimaryScreen!.WorkingArea;
            this.Location = new Point(area.Right - this.Width - 10, area.Top + 10);
        }

        private void JoneFora_DoubleClick(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000080;
                return cp;
            }
        }
    }
}
