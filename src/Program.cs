using System.Windows.Forms;

namespace UPG_SP_2024
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string parameter;
            int gridGapWidth;
            int gridGapHeight;
            if (args.Length > 0)
            {
                // Use the provided parameters
                parameter = args[0][..1];

                gridGapWidth = Convert.ToInt32(args[0].Substring(1, 2));
                gridGapHeight = Convert.ToInt32(args[0].Substring(4, 2));
            }
            else
            {
                parameter = "0";
                gridGapWidth = 30;
                gridGapHeight = 30;
            }
            // Create the MainForm and pass the parameters to it
            using (var mainForm = new MainForm(parameter, gridGapWidth, gridGapHeight))
            {
                mainForm.Show();
                Application.Run(mainForm);
            }
        }
    }
}