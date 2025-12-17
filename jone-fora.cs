using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace jone_fora
{
    public partial class JoneFora : Form
    {
        #region Win32

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(
            IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 2;

        #endregion

        #region State

        private readonly List<Log> _logs;
        private readonly List<Data> _datas;
        private readonly Data _data;
        private TimeSpan _cronometro;

        private const int SnapMargin = 10;
        private System.Windows.Forms.Timer _hoverTimer;

        #endregion

        public JoneFora()
        {
            InitializeComponent();

            _logs = Get<Log>.Lista();
            _datas = Get<Data>.Lista();

            _data = _datas.Find(x => x.DataAtual == DateOnly.FromDateTime(DateTime.Today))
                    ?? CreateTodayData();

            _cronometro = _data.QuantidadeMinutos;

            ShowInTaskbar = false;
            Opacity = 0.7;
        }

        private Data CreateTodayData()
        {
            var data = new Data();
            _datas.Add(data);
            return data;
        }

        #region Lifecycle

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            AtualizarLabels();
            EnableDrag(this);
            StartHoverEffect();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            var area = Screen.PrimaryScreen!.WorkingArea;
            Location = new Point(area.Right - Width - 10, area.Top + 10);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_EXITSIZEMOVE = 0x232;
            const int WM_NCLBUTTONDBLCLK = 0xA3;

            if (m.Msg == WM_NCLBUTTONDBLCLK)
                return;

            if (m.Msg == WM_EXITSIZEMOVE)
                SnapToNearestCorner();

            base.WndProc(ref m);
        }

        #endregion

        #region UI Logic

        private void AtualizarLabels()
        {
            var totalDias = _datas.Count;
            var totalSaidas = _datas.Sum(x => x.QuantidadeSaidas);
            var totalTempoTicks = _datas.Sum(x => x.QuantidadeMinutos.Ticks);

            var mediaTempo = totalSaidas == 0
                ? TimeSpan.Zero
                : TimeSpan.FromTicks(totalTempoTicks / totalSaidas);

            var mediaSaidas = totalDias == 0
                ? 0
                : Math.Floor((double)totalSaidas / totalDias);

            CronometroLabel.Text = _cronometro.ToString(@"hh\:mm\:ss");
            DataLabel.Text = _data.DataAtual.ToString("dd/MM/yyyy");
            SaidaLabel.Text = $"Saídas      : {_data.QuantidadeSaidas}";
            SaidaPorDiaLabel.Text = $"Saída/Dia   : {mediaSaidas}";
            TempoPorSaidaLabel.Text = $"Tempo/Saída : {mediaTempo:mm\\:ss}";
        }

        private void EventoBt_Click(object sender, EventArgs e)
        {
            if (EventoBt.Text == "Saiu")
                _data.QuantidadeSaidas++;

            Cronometro.Enabled = !Cronometro.Enabled;

            _logs.Add(new Log
            {
                DataHora = DateTime.Now,
                Evento = EventoBt.Text
            });

            EventoBt.Text = EventoBt.Text == "Saiu" ? "Voltou" : "Saiu";
            AtualizarLabels();
        }

        private void Cronometro_Tick(object sender, EventArgs e)
        {
            _cronometro = _cronometro.Add(TimeSpan.FromSeconds(1));
            CronometroLabel.Text = _cronometro.ToString(@"hh\:mm\:ss");
        }

        private void FecharBt_Click(object sender, EventArgs e)
        {
            _data.QuantidadeMinutos = _cronometro;
            Get<Log>.Salvar(_logs);
            Get<Data>.Salvar(_datas);
            Close();
        }

        #endregion

        #region Drag & Snap

        private void EnableDrag(Control control)
        {
            control.MouseDown += (_, e) =>
            {
                if (e.Button != MouseButtons.Left)
                    return;

                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
            };

            foreach (Control child in control.Controls)
                if (child is not Button)
                    EnableDrag(child);
        }

        private void SnapToNearestCorner()
        {
            var area = Screen.FromControl(this).WorkingArea;

            bool left = Left + Width / 2 < area.Left + area.Width / 2;
            bool top = Top + Height / 2 < area.Top + area.Height / 2;

            var target = new Point(
                left ? area.Left + SnapMargin : area.Right - Width - SnapMargin,
                top ? area.Top + SnapMargin : area.Bottom - Height - SnapMargin
            );

            AnimateTo(target);
        }

        private void AnimateTo(Point target)
        {
            var timer = new System.Windows.Forms.Timer { Interval = 15 };
            var start = Location;
            int step = 0;
            const int steps = 8;

            timer.Tick += (_, _) =>
            {
                step++;

                if (step >= steps)
                {
                    Location = target;
                    timer.Dispose();
                    return;
                }

                float t = (float)step / steps;
                t *= t;

                Location = new Point(
                    start.X + (int)((target.X - start.X) * t),
                    start.Y + (int)((target.Y - start.Y) * t)
                );
            };

            timer.Start();
        }

        #endregion

        #region Hover

        private void StartHoverEffect()
        {
            _hoverTimer = new System.Windows.Forms.Timer { Interval = 50 };
            _hoverTimer.Tick += (_, _) =>
            {
                var inside = ClientRectangle.Contains(PointToClient(Cursor.Position));
                Opacity = inside ? 1.0 : 0.7;
            };
            _hoverTimer.Start();
        }

        #endregion
    }
}