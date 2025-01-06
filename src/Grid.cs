using System.ComponentModel.DataAnnotations;
using System.IO.IsolatedStorage;

namespace ElectricFieldVis
{

    /// <summary>
    /// Constructor of class Grid, takes most of parameters common for all functions.
    /// </summary>
    /// <param name="gridCellWidth"> Grid cell width </param>
    /// <param name="gridCellHeight"> Grid cell height </param>
    /// <param name="width"> Width of the drawingPanel </param>
    /// <param name="height"> Height of the drawingPanel</param>
    /// <param name="charges"> Array of the charge </param>
    /// <param name="positions"> Array of position of charges arrays </param>
    internal class Grid(int gridCellWidth, int gridCellHeight, int width, int height, double[] charges, double[,] positions)
    {
        private float[] gridPosition = new float[4];
        private int gridCellWidth = gridCellWidth;
        private int gridCellHeight = gridCellHeight;
        private double[] charges = charges;
        private double[,] positions = positions;
        private int width = width;
        private int height = height;


        /// <summary>
        /// Draws a grid in the background.
        /// </summary>
        /// <param name="g"> Graphics context </param>
        /// <param name="x"> Width gap in between lines </param>
        /// <param name="y"> Height gap in between lines</param>
        /// <param name="width"> Width of the drawingPanel </param>
        /// <param name="height"> Height of the drawingPanel</param>
        public void BackgroundGrid(Graphics g, float scale)
        {
            var s = g.Transform;
            Pen p = new Pen(Brushes.Black, 1f);
            g.TranslateTransform(-width / 2, -height / 2);
            for (int i = 0; i < height; i = i + gridCellHeight)
            {
                g.DrawLine(p, 0, i, width, i);

                for (int j = 0; j < width; j = j + gridCellWidth)
                {
                    g.DrawLine(p, j, 0, j, height);
                }
            }
            g.Transform = s;
        }

        /// <summary>
        /// Calculates the coordinates for the arrowheads in the grid.
        /// </summary>
        /// <param name="x"> x position in the electrostat. field (without scale)</param>
        /// <param name="y"> y position in the electrostat. field (without scale)</param>
        public double Calculate_Grid_Vector_Coord_And_Intensity(double x, double y)
        {
            double X = x;
            double Y = y;

            // Defining size of vector.
            gridPosition = new float[4] { 0, 0, 0, 0 };
            double epsilonZero = 8.854e-12;
            double firstPart = (1 / (4 * Math.PI * epsilonZero));
            double[] coordXY = new double[2] { 0, 0 };
            double[] coordXY_Color_Scale = new double[2] { 0, 0 };
            for (int i = 0; i < charges.Length; i++)
            {
                double nX = X - positions[i, 0];
                double nY = Y + positions[i, 1];
                double magnitude = Math.Pow(nX * nX + nY * nY, 1.5);
                coordXY[0] += (charges[i] * (nX / magnitude));
                coordXY[1] += (charges[i] * (nY / magnitude));
                coordXY_Color_Scale[0] += (Math.Pow(Math.Abs(charges[i]), 2) * charges[i] * (nX / magnitude));
                coordXY_Color_Scale[1] += (Math.Pow(Math.Abs(charges[i]), 2) * charges[i] * (nY / magnitude));
            }
            // Vector of intensity.
            double[] coulombXY = [coordXY[0] * firstPart, coordXY[1] * firstPart];
            double[] coulombXY_Color_Scale = [coordXY_Color_Scale[0] * firstPart, coordXY_Color_Scale[1] * firstPart];
            // Calculation of magnitude.
            double nMagnitude = Math.Sqrt(coulombXY[0] * coulombXY[0] + coulombXY[1] * coulombXY[1]);

            // Assigning final coordinates of vector.
            gridPosition[0] = (float)X;
            gridPosition[1] = (float)Y;
            gridPosition[2] = (float)(X + (coulombXY[0] / nMagnitude));
            gridPosition[3] = (float)(Y + (coulombXY[1] / nMagnitude));
            return Math.Sqrt(coulombXY_Color_Scale[0] * coulombXY_Color_Scale[0] + coulombXY_Color_Scale[1] * coulombXY_Color_Scale[1]);
        }

        /// <summary>
        /// Draws a vector using coordinates from Calculate_Intensity_Static.
        /// </summary>
        /// <param name="g"> Graphics context </param>
        /// <param name="scale"> Scaling variable </param>
        public void Grid_Vector(Graphics g, float scale)
        {
            gridPosition[0] *= 50 * scale;
            gridPosition[1] *= 50 * scale;
            gridPosition[2] *= 50 * scale;
            gridPosition[3] *= 50 * scale;


            double tipLength = 5f * Math.Min(gridCellWidth / 15, gridCellHeight / 15); // Length of the arrowhead.
            double tipAngle = (double)(Math.PI / 6); // 30 degrees for arrowhead angle.

            //---- Calculates the center of the shell. ----//
            float c_x = gridPosition[0] + gridCellWidth / 2;
            float c_y = gridPosition[1] + gridCellHeight / 2;

            //---- Calculates the direction vector. ----//
            double u_x = gridPosition[2] - gridPosition[0];
            double u_y = gridPosition[3] - gridPosition[1];

            //---- Calculates the points of the arrowhead. ----//
            double angle1 = Math.Atan2(u_y, u_x) + tipAngle; // Right side of the arrowhead.
            double angle2 = Math.Atan2(u_y, u_x) - tipAngle; // Left side of the arrowhead.

            float e_x1 = (float)(c_x - tipLength * Math.Cos(angle1));
            float e_y1 = (float)(c_y - tipLength * Math.Sin(angle1));
            float e_x2 = (float)(c_x - tipLength * Math.Cos(angle2));
            float e_y2 = (float)(c_y - tipLength * Math.Sin(angle2));

            Pen p = new Pen(Brushes.Black, 1f);

            g.DrawLine(p, c_x, c_y, e_x1, e_y1); // Right side of the arrowhead.
            g.DrawLine(p, c_x, c_y, e_x2, e_y2); // Left side of the arrowhead.


        }

        /// <summary>
        /// Applies the two functions above to calculate the coordinates and draw all needed vectors in the background.
        /// </summary>
        /// <param name="g"> Grapics context </param>
        /// <param name="scale"> Scaling variable </param>
        public void Add_Grid_Vectors(Graphics g, float scale)
        {
            for (int y = -height / 2; y < height / 2; y += gridCellHeight)
            {
                for (int x = -width / 2; x < width / 2; x += gridCellWidth)
                {
                    // Getting non-scaled values for x and y
                    double X = x / (50 * scale);
                    double Y = y / (50 * scale);
                    Calculate_Grid_Vector_Coord_And_Intensity(X, Y);
                    Grid_Vector(g, scale);
                }
            }
        }

        /// <summary>
        /// Draws a heatmap as a background.
        /// </summary>
        /// <param name="g"> Graphics context </param>
        /// <param name="scale"> Scale variable </param>
        public void Add_Heat_Map_Background(Graphics g, float scale)
        {
            double maxIntensity = 0;
            // First iterates through the field to look for the maximum value
            for (int y = -height / 2; y < height / 2; y += 5)
            {
                for (int x = -width / 2; x < width / 2; x += 5)
                {
                    double X = x / (50 * scale);
                    double Y = y / (50 * scale);
                    double intensity = Calculate_Grid_Vector_Coord_And_Intensity(X, Y);

                    // Ignore super-high values in order to get reasonable maximum. (Inside of the Charge)
                    if (intensity > 10000000000)
                    {
                        continue;
                    };

                    // Update maxIntensity if a higher intensity is found.
                    if (intensity > maxIntensity)
                    {
                        maxIntensity = intensity;
                    }
                }
            }

            double minIntensity = Double.MaxValue;
            // Second iteration draws the background
            for (int y = -height / 2; y < height / 2; y += 10)
            {
                for (int x = -width / 2; x < width / 2; x += 10)
                {

                    double X = x / (50 * scale);
                    double Y = y / (50 * scale);
                    double intensity = Calculate_Grid_Vector_Coord_And_Intensity(X, Y);
                    if (intensity < minIntensity)
                    {
                        minIntensity = intensity;
                    }
                    Color col = GetColorForIntensity(intensity, minIntensity, maxIntensity);
                    Brush br = new SolidBrush(col);
                    g.FillRectangle(br, x - 5 / 2, y - 5 / 2, 10, 10);
                }
            }
        }

        /// <summary>
        /// Calculates the normalized value for colors, which decides what shade and color will be used.
        /// </summary>
        /// <param name="intensity"> Intensity of the field at specific point </param>
        /// <param name="maxIntensity"> Maximum itensity of the field </param>
        /// <param name="minIntensity"> Minimum intensity of the field </param>
        /// <returns> Return a result of function Choose_color </returns>
        public Color GetColorForIntensity(double intensity, double minIntensity, double maxIntensity)
        {
            // Normalize the intensity between 0 and 1.
            double normalized = (maxIntensity + minIntensity) / (maxIntensity + intensity);
            Color green = Color.FromArgb(0, 255, 0);    // Green
            Color yellow = Color.FromArgb(255, 255, 0); // Yellow

            // Interpolate between Yellow and Green based on normalized intensity.
            return Choose_Color(yellow, green, normalized);
        }

        /// <summary>
        /// Calculates how much of red, green and blue color should be used in order to achieve desired color.
        /// </summary>
        /// <param name="color1"> Yellow color </param>
        /// <param name="color2"> Green color </param>
        /// <param name="t"> normalized value </param>
        /// <returns> returns a specific rgb color </returns>
        public Color Choose_Color(Color color1, Color color2, double t)
        {
            int r = (int)(color1.R + t * (color2.R - color1.R));
            int g = (int)(color1.G + t * (color2.G - color1.G));
            int b = (int)(color1.B + t * (color2.B - color1.B));
            if (r <= 255 && g <= 255 && b <= 255)
            {
                return Color.FromArgb(r, g, b);
            }
            else
            {
                return Color.FromArgb(0, 0, 0);
            }
        }
    }
}
