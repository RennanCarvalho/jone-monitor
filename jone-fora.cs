using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace jone_fora
{
    public partial class JoneFora : Form
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(
            IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);


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
            EnableDrag(this);
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
                : Math.Floor((double)totalSaidas / totalDias);

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

        private const int _margin = 10;

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;
            const int HTCAPTION = 2;
            const int WM_EXITSIZEMOVE = 0x232;
            const int WM_NCLBUTTONDBLCLK = 0xA3;

            if (m.Msg == WM_NCLBUTTONDBLCLK)
                return; // bloqueia maximizar

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                if ((int)m.Result == HTCLIENT)
                {
                    m.Result = (IntPtr)HTCAPTION;
                    return;
                }
                return;
            }
            if (m.Msg == WM_EXITSIZEMOVE)
            {
                // aqui é o "MouseUp" real do drag nativo
                SnapToNearestCorner();
            }

            base.WndProc(ref m);
        }

        private void SnapToNearestCorner()
        {
            var screen = Screen.FromControl(this);
            var area = screen.WorkingArea;

            int centerX = Left + Width / 2;
            int centerY = Top + Height / 2;

            int screenCenterX = area.Left + area.Width / 2;
            int screenCenterY = area.Top + area.Height / 2;

            bool isLeft = centerX < screenCenterX;
            bool isTop = centerY < screenCenterY;

            int x = isLeft
                ? area.Left + _margin
                : area.Right - Width - _margin;

            int y = isTop
                ? area.Top + _margin
                : area.Bottom - Height - _margin;

            AnimateToLocation(new Point(x, y));
        }
        private void AnimateToLocation(Point target)
        {
            // Implementação simples de animação
            System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 15;
            int steps = 8;
            int currentStep = 0;

            Point start = this.Location;

            animationTimer.Tick += (s, e) =>
            {
                currentStep++;
                if (currentStep >= steps)
                {
                    this.Location = target;
                    animationTimer.Stop();
                    animationTimer.Dispose();
                }
                else
                {
                    float t = (float)currentStep / steps;
                    // Easing function simples (quadrática)
                    t = t * t;

                    int x = start.X + (int)((target.X - start.X) * t);
                    int y = start.Y + (int)((target.Y - start.Y) * t);

                    this.Location = new Point(x, y);
                }
            };

            animationTimer.Start();
        }

        private void EnableDrag(Control control)
        {
            control.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, 0xA1, (IntPtr)2, IntPtr.Zero);
                }
            };

            foreach (Control child in control.Controls)
                EnableDrag(child);
        }

        System.Windows.Forms.Timer hoverTimer;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Opacity = 0.7;

            hoverTimer = new System.Windows.Forms.Timer();
            hoverTimer.Interval = 50; // rápido o suficiente
            hoverTimer.Tick += HoverTimer_Tick;
            hoverTimer.Start();
        }

        private void HoverTimer_Tick(object? sender, EventArgs e)
        {
            var mousePos = this.PointToClient(Cursor.Position);
            bool inside = this.ClientRectangle.Contains(mousePos);

            this.Opacity = inside ? 1.0 : 0.7;
        }
    }
}
