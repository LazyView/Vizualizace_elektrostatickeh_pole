namespace UPG_SP_2024
{
    public partial class MainForm : Form
    {
        int deltaX;
        int deltaY;
        public string parameter;
        public int gridCellWidth;
        public int gridCellHeight;
        public MainForm(string parameter, int gridGapWidth, int gridGapHeight)
        {
            this.parameter = parameter;
            this.gridCellWidth = gridGapWidth;
            this.gridCellHeight = gridGapHeight;
            InitializeComponent();
            // Store the parameter in a field or use it to initialize the DrawingPanel
            drawingPanel.parameter = parameter;
            drawingPanel.gridCellWidth = this.gridCellWidth;
            drawingPanel.gridCellHeight = this.gridCellHeight;
            this.Size = new Size(800, 600);
            BackColor = Color.LightGoldenrodYellow;
        }

        private void drawingPanel_Paint(object sender, PaintEventArgs e)
        {
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void drawingPanel_MouseClick(object sender, MouseEventArgs e)
        {

            drawingPanel.mouseClick = true;

        }

        private void drawingPanel_MousePressed(object sender, MouseEventArgs e)
        {
            deltaX = e.X - drawingPanel.Width / 2;
            deltaY = e.Y - drawingPanel.Height / 2;
            drawingPanel.lastMousePos = new Point(deltaX, deltaY);
            if (drawingPanel.isHit)
            {
                drawingPanel.isDragging = true;
            }

        }

        private void drawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            // Vypoèítat posun v ose Y
            if (drawingPanel.isDragging)
            {
                drawingPanel.lastMousePos = new Point(e.X - drawingPanel.Width / 2, e.Y - drawingPanel.Height / 2);
            }
        }
        private void drawingPanel_MouseReleased(object sender, MouseEventArgs e)
        {
            if (drawingPanel.isDragging)
            {
                drawingPanel.isDragging = false;
            }
        }

        private void Bttn_Smaller_Click(object sender, EventArgs e)
        {
            drawingPanel.bttn_Smaller = true;
        }

        private void Bttn_Bigger_Click(object sender, EventArgs e)
        {
            drawingPanel.bttn_Bigger = true;
        }

        private void Add_Probe_Click(object sender, EventArgs e)
        {
            if (!drawingPanel.placeProbe)
            {
                drawingPanel.placeProbe = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
