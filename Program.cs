using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PrintLabel
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)//&& System.Diagnostics.Debugger.IsAttached)
            {
                Scheduler printLabel = new Scheduler();
                printLabel.Start();

                Console.WriteLine("Premi un tasto per USCIRE...");
                Console.ReadKey();

                printLabel.Stop();
            }
            else
            {
                ServiceBase.Run(new Scheduler());
            }
        }
    }
}
