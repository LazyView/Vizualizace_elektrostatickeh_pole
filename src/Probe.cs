namespace ElectricFieldVis
{
    internal class Probe
    {
        //coordinates of the vector.
        private float[] vectorPosition;
        private double[] charges;
        private double[,] positions;
        private readonly double[] probe_Position;
        private float elapsed;
        double width;
        double height;

        /// <summary>
        /// Constructor for moving probe.
        /// </summary>
        /// <param name="elapsed"> Scaling variable </param>
        /// <param name="width"> Width of the drawingPanel </param>
        /// <param name="height"> Height of the drawingPanel </param>
        /// <param name="charges"> Array of charges </param>
        /// <param name="positions"> Array of array for charge positions </param>
        public Probe(float elapsed, double width, double height, double[] charges, double[,] positions)
        {
            this.elapsed = elapsed;
            this.width = width;
            this.height = height;
            this.charges = charges;
            this.positions = positions;
            probe_Position = [MathF.PI / 6f, MathF.PI / 6f];
        }

        /// <summary>
        /// Constructor for static probe.
        /// </summary>
        /// <param name="mouse_Position"> Position of the mouse when clicked </param>
        /// <param name="width"> Width of the drawingPanel </param>
        /// <param name="height"> Height of the drawingPanel </param>
        /// <param name="charges"> Array of charges </param>
        /// <param name="positions"> Array of array for charge positions </param>
        public Probe(double[] mouse_Position, double width, double height, double[] charges, double[,] positions)
        {
            probe_Position = mouse_Position;
            this.width = width;
            this.height = height;
            this.charges = charges;
            this.positions = positions;
        }

        /// <summary>
        /// Sets charges and positions to new ones
        /// </summary>
        /// <param name="charges"> Array of charges power </param>
        /// <param name="positions"> Array of arrays of charges positions </param>
        public void set_Charge_Power_And_Positions(double[] charges, double[,] positions)
        {
            this.charges = charges;
            this.positions = positions;
        }

        /// <summary>
        /// Calculates intensity for the moving probe.
        /// </summary>
        /// <returns> Intensity of the field at moving probe position. </returns>
        public double Calculate_Intensity()
        {
            // Rotation around a circle at the speed of PI/6.
            double X = Math.Cos(probe_Position[0] * elapsed);
            double Y = -Math.Sin(probe_Position[1] * elapsed);

            // Defining size of vector.
            vectorPosition = new float[4] { 0, 0, 0, 0 };
            double epsilonZero = 8.854e-12;
            double firstPart = (1 / (4 * Math.PI * epsilonZero));
            double[] coordXY = new double[2] { 0, 0 };

            for (int i = 0; i < charges.Length; i++)
            {
                double chargeX = positions[i, 0];
                double chargeY = -positions[i, 1];
                double nX = X - chargeX;
                double nY = Y - chargeY;
                double magnitude = Math.Pow(nX * nX + nY * nY, 1.5);
                coordXY[0] += (charges[i] * (nX / magnitude));
                coordXY[1] += (charges[i] * (nY / magnitude));
            }
            // Vector of intensity.
            double[] coulombXY = new double[2] { coordXY[0] * firstPart, coordXY[1] * firstPart };

            // Calculation of magnitude.
            double nMagnitude = Math.Sqrt(coulombXY[0] * coulombXY[0] + coulombXY[1] * coulombXY[1]);

            // Assigning final coordinates of vector.
            vectorPosition[0] = (float)X;
            vectorPosition[1] = (float)Y;
            vectorPosition[2] = (float)(X + (coulombXY[0] / nMagnitude));
            vectorPosition[3] = (float)(Y + (coulombXY[1] / nMagnitude));

            return nMagnitude;

        }

        /// <summary>
        /// Calculates intensity for static probe.
        /// </summary>
        /// <param name="scale"> Scaling variable </param>
        /// <returns> Intensity of the field at static probe position </returns>
        public double Calculate_Intensity_Static(float scale)
        {
            double X = ((probe_Position[0]) / (50 * scale));
            double Y = ((probe_Position[1]) / (50 * scale));

            // Defining size of vector.
            vectorPosition = new float[4] { 0, 0, 0, 0 };
            double epsilonZero = 8.854e-12;
            double firstPart = (1 / (4 * Math.PI * epsilonZero));
            double[] coordXY = new double[2] { 0, 0 };

            for (int i = 0; i < charges.Length; i++)
            {
                double chargeX = positions[i, 0];
                double chargeY = -positions[i, 1];
                double nX = X - chargeX;
                double nY = Y - chargeY;
                double magnitude = Math.Pow(nX * nX + nY * nY, 1.5);
                coordXY[0] += (charges[i] * (nX / magnitude));
                coordXY[1] += (charges[i] * (nY / magnitude));
            }
            // Vector of intensity.
            double[] coulombXY = new double[2] { coordXY[0] * firstPart, coordXY[1] * firstPart };

            // Calculation of magnitude.
            double nMagnitude = Math.Sqrt(coulombXY[0] * coulombXY[0] + coulombXY[1] * coulombXY[1]);

            // Assigning final coordinates of vector.
            vectorPosition[0] = (float)X;
            vectorPosition[1] = (float)Y;
            vectorPosition[2] = (float)(X + (coulombXY[0] / nMagnitude));
            vectorPosition[3] = (float)(Y + (coulombXY[1] / nMagnitude));

            return nMagnitude;

        }

        /// <summary>
        /// Draws vector and its description.
        /// </summary>
        /// <param name="g"> Graphics context </param>
        /// <param name="scale"> Scaling variable </param>
        public void AddVector(Graphics g, float scale)
        {
            double result = Calculate_Intensity();
            // Scaling the coordinates.
            vectorPosition[0] *= (50 * scale);
            vectorPosition[1] *= (50 * scale);
            vectorPosition[2] *= (50 * scale);
            vectorPosition[3] *= (50 * scale);

            // Calculate the direction vector and its length.
            double u_x = vectorPosition[2] - vectorPosition[0];
            double u_y = vectorPosition[3] - vectorPosition[1];
            double length = (float)Math.Sqrt(u_x * u_x + u_y * u_y);

            // Normalize the direction vector if length is greater than zero.
            if (length > 0)
            {
                u_x /= length;
                u_y /= length;
            }
            else
            {
                // If length is zero, default direction (arbitrary).
                u_x = 1;
                u_y = 0;
            }

            // Calculate length and position of the arrowhead.
            double tipLength = 5f * scale; // Length of the arrowhead.
            double tipAngle = (double)(Math.PI / 6); // 30 degrees for arrowhead angle.

            // Position of the arrow shaft end.
            float c_x = vectorPosition[2];
            float c_y = vectorPosition[3];

            // Calculate the points of the arrowhead.
            double angle1 = Math.Atan2(u_y, u_x) + tipAngle; // Right side of the arrowhead.
            double angle2 = Math.Atan2(u_y, u_x) - tipAngle; // Left side of the arrowhead.

            float e_x1 = (float)(c_x - tipLength * Math.Cos(angle1));
            float e_y1 = (float)(c_y - tipLength * Math.Sin(angle1));
            float e_x2 = (float)(c_x - tipLength * Math.Cos(angle2));
            float e_y2 = (float)(c_y - tipLength * Math.Sin(angle2));

            // Font and position of vector description.
            Font f = new Font("Arial", 4f * scale);

            string resultStr = "|E| = " + Math.Round(result / 1000000000, 2) + " GN/C";
            float stringX = vectorPosition[0] - g.MeasureString(resultStr, f).Width / 2;
            float stringY = vectorPosition[1] - g.MeasureString(resultStr, f).Height;

            // Assign size of the probe.
            float size = 2f * scale;
            Pen pen2 = new Pen(Brushes.Black, 2f);
            // Drawing the probe, arrow and description.
            g.DrawLine(pen2, vectorPosition[0], vectorPosition[1], vectorPosition[2], vectorPosition[3]); // Draw the arrow line.
            g.DrawLine(pen2, c_x, c_y, e_x1, e_y1); // Right side of the arrowhead.
            g.DrawLine(pen2, c_x, c_y, e_x2, e_y2); // Left side of the arrowhead.
            g.FillEllipse(Brushes.Black, (vectorPosition[0] - size / 2), (vectorPosition[1] - size / 2), size, size);
            g.DrawString(resultStr, f, Brushes.Black, stringX, stringY);
        }

        /// <summary>
        /// Draws a static probe
        /// </summary>
        /// <param name="g"> Graphic context </param>
        /// <param name="scale"> Scaling variable </param>
        public void AddProbeStatic(Graphics g, float scale)
        {
            // Assign size of the probe.
            float size = 3f * scale;
            double result = Calculate_Intensity_Static(scale);
            Font f = new Font("Arial", 4f * scale);
            string resultStr = "|E| = " + Math.Round(result / 1000000000, 2) + " GN/C";
            float stringX = (float)probe_Position[0] - g.MeasureString(resultStr, f).Width / 2;
            float stringY = (float)probe_Position[1] - g.MeasureString(resultStr, f).Height;
            g.FillEllipse(Brushes.Violet, ((float)probe_Position[0] - size / 2), ((float)probe_Position[1] - size / 2), size, size);
            g.DrawString(resultStr, f, Brushes.Black, stringX, stringY);
        }
    }

}
