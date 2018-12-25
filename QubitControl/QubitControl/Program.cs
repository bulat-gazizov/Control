using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QubitControl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if debug

            MessageBox.Show("Attach debugger");
#endif 
            if (args == null || args.Length == 0)
                Application.Run(new frmMain());
            else
            {
                Application.Run(new frmMain(args[0]));
            }
        }
    }
}
