using ElectricFieldVis;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace UPG_SP_2024
{

    /// <summary>
    /// The main panel with the custom visualization
    /// </summary>
    public class DrawingPanel : Panel
    {
        //------- public variables -------- //
        public string parameter;
        public int[] coordinates;
        public int gridCellWidth;
        public int gridCellHeight;

        //---- buttons handeling ----//
        public bool bttn_Bigger;
        public bool bttn_Smaller;

        //---- Mouse handeling ----//
        public Point lastMousePos;
        public Point mouseMoveNum;
        public bool isDragging;
        public bool mouseClick;
        public bool isHit;

        //------- private variables -------//
        private float scale;
        private int m_Start;
        private float elapsed;

        //---- Storing the values for positions and charges ----//
        private double[,] chargePositions;
        private double[] chargePower;

        //---- Instances needed to create a graph ----//
        private ObservableCollection<double> value;
        private ObservableCollection<string> time;
        public Graph graph;

        //---- Instances of charges ----//
        public Charge ch1;
        public Charge ch2;
        private Charge ch3;
        private Charge ch4;
        private Charge[] charge;

        //---- Instances of Grid and Probe ----//
        private Probe p;
        private Probe pStatic;
        private Grid grid;
        private bool probePlaced;
        public bool placeProbe;

        //---- Instance of time in seconds (used for graph)----//
        private double timeV;

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingPanel" /> class
        /// </summary>
        public DrawingPanel(string parameter, int gridCellWidth, int gridCellHeight)
        {

            this.Text = "Electrostatic Field Visualization";
            var timer = new System.Windows.Forms.Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 50;
            m_Start = Environment.TickCount;
            timer.Start();
            // Anti-flickerring.
            DoubleBuffered = true;
            parameter = "1";
            //---- Setting bools to starting value ---//
            probePlaced = false;
            isDragging = false;
            isHit = false;
            //---- Setting up the field ----//
            if (parameter != null)
            {
                Scenarious_Charges(parameter);
            }



            //---- Assigning instances for graph ----//
            value = [];
            time = [];
            timeV = 0;
            graph = new Graph(value, time);
        }

        /// <summary>
        /// Invalidates the drawingPanel on each time tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"> that contains the event data. </param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        /// <summary>Main function where everything draws on the drawingPanel</summary>
        /// <remarks>Raises the <see cref="E:System.Windows.Forms.Control.Paint">Paint</see> event.</remarks>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs">PaintEventArgs</see> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //-------------------------------//
            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.TranslateTransform(this.Width / 2, this.Height / 2);
            parameter = "1";
            //-------------------------------//
            var scalenumber = 0.005f;
            var scalex = this.Width * scalenumber;
            var scaley = this.Height * scalenumber;
            scale = Math.Min(scalex, scaley);

            //---- Updating the charges, probe and background based on the scenario ----//
            Scenarious(g, parameter);

            //---- Charges become able to move ----//
            charge = [ch1, ch2, ch3, ch4];
            foreach (Charge cha in charge)
            {
                Charge_Hit(g, cha);
                Move_Charge(g, cha);
            }
            //---- Open a new tab with a graph, once closed, cannot be opened again ----//
            if (probePlaced && !graph.IsDisposed)
            {
                graph.Show();
                graph.Invalidate();
            }
            Probe_Static(g);
        }

        /// <summary>
        /// Chooses right scenario and draws it onto drawingPanel.
        /// </summary>
        /// <param name="g"> Graphics content </param>
        /// <param name="parameter"> Decides which scenerio to play </param>
        /// <param name="scale"> Scaling variable </param>
        private void Scenarious(Graphics g, string parameter)
        {

            elapsed = (Environment.TickCount - m_Start) / 1000f;
            switch (parameter)
            {
                case "":
                    chargePositions = new double[1, 2] { { ch1.GetCoord()[0], ch1.GetCoord()[1] } };
                    chargePower = [ch1.GetCharge()];
                    break;
                case "0":
                    chargePositions = new double[1, 2] { { ch1.GetCoord()[0], ch1.GetCoord()[1] } };
                    chargePower = [ch1.GetCharge()];
                    break;

                case "1":
                    chargePositions = new double[2, 2] { { ch1.GetCoord()[0], ch1.GetCoord()[1] }, { ch2.GetCoord()[0], ch2.GetCoord()[1] } };
                    chargePower = [ch1.GetCharge(), ch2.GetCharge()];
                    break;

                case "2":
                    chargePositions = new double[2, 2] {
                        { ch1.GetCoord()[0], ch1.GetCoord()[1] },
                        { ch2.GetCoord()[0], ch2.GetCoord()[1] }
                    };
                    chargePower = [ch1.GetCharge(), ch2.GetCharge()];
                    break;

                case "3":
                    chargePositions = new double[4, 2] {
                        { ch1.GetCoord()[0], ch1.GetCoord()[1] },
                        { ch2.GetCoord()[0], ch2.GetCoord()[1] },
                        { ch3.GetCoord()[0], ch3.GetCoord()[1] },
                        { ch4.GetCoord()[0], ch4.GetCoord()[1] }
                    };
                    chargePower = [ch1.GetCharge(), ch2.GetCharge(), ch3.GetCharge(), ch4.GetCharge()];
                    break;

                case "4":
                    //-------------------------------//
                    ch1 = new(new double[] { -1, 0 }, elapsed, 1);
                    ch2 = new(new double[] { 1, 0 }, elapsed, 2);
                    //-------------------------------//
                    chargePositions = new double[2, 2] {
                        { ch1.GetCoord()[0], ch1.GetCoord()[1] },
                        { ch2.GetCoord()[0], ch2.GetCoord()[1] }
                    };
                    chargePower = [ch1.GetCharge(), ch2.GetCharge()];
                    break;

            }

            //---- Creating GraphicsPath of the charges ----//
            ch1?.Create_Charge(g, scale);
            ch2?.Create_Charge(g, scale);
            ch3?.Create_Charge(g, scale);
            ch4?.Create_Charge(g, scale);


            //---- Assigning instances of grid and probe ----//
            grid = new(gridCellWidth, gridCellHeight, this.Width, this.Height, chargePower, chargePositions);
            p = new(elapsed, this.Width, this.Height, chargePower, chargePositions);

            //---- Drawing all components ----//
            grid.Add_Heat_Map_Background(g, scale);
            grid.BackgroundGrid(g, scale);
            grid.Add_Grid_Vectors(g, scale);
            ch1?.Draw_Charge(g, scale);
            ch2?.Draw_Charge(g, scale);
            ch3?.Draw_Charge(g, scale);
            ch4?.Draw_Charge(g, scale);
            p.AddVector(g, scale);

        }

        /// <summary>
        /// Loads the data into Charges.
        /// </summary>
        /// <param name="parameter"> Parameter which decides the scenario </param>
        private void Scenarious_Charges(string parameter)
        {
            switch (parameter)
            {
                case "":
                    ch1 = new(new double[] { 0, 0 }, 1);
                    break;
                case "0":
                    ch1 = new(new double[] { 0, 0 }, 1);
                    break;
                case "1":
                    ch1 = new(new double[] { -1, 0 }, 1);
                    ch2 = new(new double[] { 1, 0 }, 1);
                    break;
                case "2":
                    ch1 = new(new double[] { -1, 0 }, -1);
                    ch2 = new(new double[] { 1, 0 }, 2);
                    break;
                case "3":
                    ch1 = new(new double[] { -1, -1 }, 1);
                    ch2 = new(new double[] { 1, -1 }, 2);
                    ch3 = new(new double[] { 1, 1 }, -3);
                    ch4 = new(new double[] { -1, 1 }, -4);
                    break;
            }
        }

        /// <summary>
        /// Creates a static probe when mouse is clicked.
        /// </summary>
        /// <param name="g"> Graphics context </param>
        private void Probe_Static(Graphics g)
        {
            if (mouseClick && placeProbe)
            {
                pStatic = new Probe([lastMousePos.X, lastMousePos.Y], this.Width, this.Height, chargePower, chargePositions);

                probePlaced = true;
                placeProbe = false;
            }
            if (probePlaced)
            {
                pStatic.set_Charge_Power_And_Positions(chargePower, chargePositions);
                var timeCur = (Environment.TickCount - m_Start) / 1000;
                pStatic.AddProbeStatic(g, scale);
                if (timeV < timeCur)
                {
                    value.Add(pStatic.Calculate_Intensity_Static(scale) / 1000000000);
                    time.Add(Convert.ToString((Environment.TickCount - m_Start) / 1000));
                    timeV = timeCur;
                }


            }
        }

        /// <summary>
        /// Moves charge when clicked and dragged with mouse
        /// </summary>
        /// <param name="ch"> Instance of the charge, which is suppose to be moved </param>
        public void Move_Charge(Graphics g, Charge ch)
        {
            if (ch != null && ch.Is_Charge_Hit(lastMousePos.X, lastMousePos.Y) && isDragging)
            {
                ch.SetCoord([lastMousePos.X, -lastMousePos.Y], scale);
                ch.Create_Charge(g, scale);
            }
            if (ch != null && !ch.Is_Charge_Hit(lastMousePos.X, lastMousePos.Y))
            {

            }

        }

        /// <summary>
        /// Decides if the charge was clicked by a mouse or not
        /// </summary>
        /// <param name="ch"> Instance of the charge, which is suppose to be clicked on </param>
        private void Charge_Hit(Graphics g, Charge ch)
        {
            if (ch != null && ch.Is_Charge_Hit(lastMousePos.X, lastMousePos.Y))
            {
                isHit = true;
                ch.brush.CenterColor = Color.Black;
                ch.brush.InterpolationColors = new ColorBlend()
                {
                    Colors = [Color.FromArgb(150, 255, 255, 255),
                        Color.FromArgb(150, 255, 255, 255)],
                    Positions = [0f, 1f],
                };
                ch.Draw_Charge(g, scale);
                if (bttn_Bigger)
                {
                    ch.Make_Charge_Bigger();
                    this.Invalidate();
                    bttn_Bigger = false;
                }
                else if (bttn_Smaller)
                {
                    ch.Make_Charge_Smaller();
                    this.Invalidate();
                    bttn_Smaller = false;
                }
            }
        }

        /// <summary>
        /// Fires the event indicating that the panel has been resized. Inheriting controls should use this in favor of actually listening to the event, but should still call <span class="keyword">base.onResize</span> to ensure that the event is fired for external listeners.
        /// </summary>
        /// <param name="eventargs">An <see cref="T:System.EventArgs">EventArgs</see> that contains the event data.</param>
        protected override void OnResize(EventArgs eventargs)
        {
            this.Invalidate();  //ensure repaint

            base.OnResize(eventargs);
        }
    }
}
