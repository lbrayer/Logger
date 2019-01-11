using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger;

namespace LoggerConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            // nom de l'application
            var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            // version de l'application
            var appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // emplacement du fichier de log
            var logPath = @"C:\Temp\MyTestLogger.log";
            CMLogger logger = new CMLogger(logPath, true);

            logger.WriteLog(new String('=', 60));
            logger.WriteLog($"Démarrage de l'application \"{appName}\"");
            logger.WriteLog($"Version : {appVersion}");

            logger.WriteLog("LOG SUCCES", (UInt16)CMLogger.LogType.SUCCESS);
            logger.WriteLog("LOG WARNING", (UInt16)CMLogger.LogType.WARNING);
            logger.WriteLog("LOG ERROR", (UInt16)CMLogger.LogType.ERROR);

            logger.WriteLog($"Fermeture de l'application \"{appName}\"");
            logger.WriteLog(new String('=', 60));

            // fin de l'application
            Console.ReadKey();

            // code de retour de l'application
            return 0;
        }
    }
}
