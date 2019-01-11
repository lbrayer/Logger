using System;
using System.IO;
using System.Linq;
using Logger.Models;

namespace Logger
{
    public class CMLogger : IFileLogger
    {
        /// <summary>
        /// Nombre de jour
        /// </summary>
        private const ushort _defaultFileRetention = 60;

        /// <summary>
        /// Emplacement du fichier de log
        /// </summary>
        private string _logPath;

        /// <summary>
        /// Durée de rétention des fichiers de logs (en jours)
        /// </summary>
        private ushort _fileRetention = 60;

        /// <summary>
        /// Id du process courant
        /// </summary>
        private readonly int _pID;

        /// <summary>
        /// Nom du composant par défaut
        /// </summary>
        private readonly string _defaultComponent;

        /// <summary>
        /// Indique si le message doit être affiché dans la console
        /// </summary>
        private readonly bool _doLogConsole;

        /// <summary>
        /// Couleur d'avant plan (police) par défaut de la console
        /// </summary>
        private readonly ConsoleColor _defaultConsoleFGColor;

        // type de log : INFORMATION, SUCCESS, WARNING, ERROR
        public enum LogType : UInt16 { INFORMATION, SUCCESS, WARNING, ERROR }

        /// <summary>
        /// Constructeur
        /// 60 jours de rétention de logs par défaut
        /// </summary>
        /// <param name="logPath"></param>
        public CMLogger (string logPath) {
            if (string.IsNullOrEmpty(logPath))
            {
                throw new Exception("Le chemin d'accès au fichier de log n'est pas valide : null ou vide");
            }

            _logPath = logPath;
            _fileRetention = _defaultFileRetention;
            _pID = System.Diagnostics.Process.GetCurrentProcess().Id;
            _defaultComponent = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            _doLogConsole = false;
            _defaultConsoleFGColor = Console.ForegroundColor;
        }

        /// <summary>
        /// Constructeur spécifiant le chemin d'accès au log et durée de rétention des fichiers
        /// </summary>
        /// <param name="logPath"></param>
        /// <param name="fileRetention"></param>
        public CMLogger(string logPath, ushort fileRetention) {
            if (string.IsNullOrEmpty(logPath))
            {
                throw new Exception("Le chemin d'accès au fichier de log n'est pas valide : null ou vide");
            }

            _logPath = logPath;
            _fileRetention = fileRetention;
            _pID = System.Diagnostics.Process.GetCurrentProcess().Id;
            _defaultComponent = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            _doLogConsole = false;
            _defaultConsoleFGColor = Console.ForegroundColor;
        }

        /// <summary>
        /// Constructeur
        /// 60 jours de rétention de logs pa défaut
        /// indication du log dans la console : true | false
        /// </summary>
        /// <param name="logPath"></param>
        /// <param name="doLogConsole"></param>
        public CMLogger(string logPath, bool doLogConsole)
        {
            if (string.IsNullOrEmpty(logPath))
            {
                throw new Exception("Le chemin d'accès au fichier de log n'est pas valide : null ou vide");
            }

            _logPath = logPath;
            _fileRetention = _defaultFileRetention;
            _pID = System.Diagnostics.Process.GetCurrentProcess().Id;
            _defaultComponent = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            _doLogConsole = doLogConsole;
            _defaultConsoleFGColor = Console.ForegroundColor;
        }

        /// <summary>
        /// Constructeur spécifiant le chemin d'accès au log et durée de rétention des fichiers
        /// indication du log dans la console : true | false
        /// </summary>
        /// <param name="logPath"></param>
        /// <param name="fileRetention"></param>
        /// /// <param name="doLogConsole"></param>
        public CMLogger(string logPath, ushort fileRetention, bool doLogConsole)
        {
            if (string.IsNullOrEmpty(logPath))
            {
                throw new Exception("Le chemin d'accès au fichier de log n'est pas valide : null ou vide");
            }

            _logPath = logPath;
            _fileRetention = fileRetention;
            _pID = System.Diagnostics.Process.GetCurrentProcess().Id;
            _defaultComponent = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            _doLogConsole = doLogConsole;
            _defaultConsoleFGColor = Console.ForegroundColor;
        }

        /// <summary>
        /// Get / Set logPath
        /// </summary>
        public string LogPath { get => _logPath; set => _logPath = value; }

        /// <summary>
        /// Get / set fileRetention
        /// </summary>
        public ushort FileRetention { get => _fileRetention; set => _fileRetention = value; }

        /// <summary>
        /// Permet d'écrire un message de log dans le fichier
        /// </summary>
        /// <param name="logMessage"></param>
        public void WriteLog(string logMessage)
        {
            WriteLog(logMessage, _defaultComponent, (UInt16)LogType.INFORMATION);
        }

        /// <summary>
        /// Permet d'écrire un message de log dans le fichier en spécifiant le composant
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="component"></param>
        public void WriteLog(string logMessage, string component) {
            WriteLog(logMessage, component, (UInt16)LogType.INFORMATION);
        }

        /// <summary>
        /// Permet d'écrire un message de log dans le fichier en spécifiant le type de log
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="logType"></param>
        public void WriteLog(string logMessage, UInt16 logType)
        {
            WriteLog(logMessage, _defaultComponent, logType);
        }

        /// <summary>
        /// Permet d'écrire un message de log dans le fichier en spécifiant le composant et type de log
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="component"></param>
        /// <param name="logType"></param>
        public void WriteLog(string logMessage, string component, UInt16 logType) {
            // récupération du message formaté
            string formatedLogMesg = GetFormatedLogMessage(logMessage, component, logType, _pID);

            DirectoryInfo parentDirInfo = Directory.GetParent(LogPath);
            if (!parentDirInfo.Exists)
            {
                Directory.CreateDirectory(parentDirInfo.FullName);
            }

            // écriture du message formaté dans le fichier
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(_logPath, true)) {
                file.WriteLine(formatedLogMesg);
            }

            // écriture dans la console
            if (_doLogConsole) {
                Console.ForegroundColor = GetConsoleFGColor(logType);
                Console.WriteLine(logMessage);
            }
        }

        /// <summary>
        /// Permet de formater le message de log
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="component"></param>
        /// <param name="logType"></param>
        /// <param name="pID"></param>
        /// <returns></returns>
        private string GetFormatedLogMessage(string logMessage, string component, UInt16 logType, int pID) {
            string logTypeStr = GetLogType(logType);
            string dateTimeStr = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss.ffffff");

            //$Local: l_CMLog = "{0} `$$<{1}><{2} {3}><thread={4}>" - f($Type + ":" + $Message), $Component, (Get - Date - Format "MM-dd-yyyy"), (Get - Date - Format "HH:mm:ss.ffffff"), $g_PID
            string formatedLogMesg = $"{logTypeStr} : {logMessage} $$<{component}><{dateTimeStr}><thread={pID}>";

            return formatedLogMesg;
        }

        /// <summary>
        /// Renvoie le type de log sous forme de chaine de caractères
        /// </summary>
        /// <param name="logType"></param>
        /// <returns></returns>
        private string GetLogType(UInt16 logType) {
            string logTypeStr = "";
            UInt16 rightAlignLength = 11;

            switch (logType) {
                case (UInt16)LogType.INFORMATION:
                    logTypeStr = "INFORMATION";
                    break;
                case (UInt16)LogType.SUCCESS:
                    logTypeStr = "SUCCESS";
                    break;
                case (UInt16)LogType.WARNING:
                    logTypeStr = "WARNING";
                    break;
                case (UInt16)LogType.ERROR:
                    logTypeStr = "ERROR";
                    break;
            }

            return logTypeStr.PadRight(rightAlignLength);
        }

        /// <summary>
        /// Permet de récupérer la couleur d'avant plan (police) de la console en fonction du type de log
        /// </summary>
        /// <param name="logType"></param>
        /// <returns></returns>
        private ConsoleColor GetConsoleFGColor(UInt16 logType) {
            ConsoleColor fgColor = _defaultConsoleFGColor;

            switch (logType)
            {
                case (UInt16)LogType.INFORMATION:
                    fgColor = _defaultConsoleFGColor;
                    break;
                case (UInt16)LogType.SUCCESS:
                    fgColor = ConsoleColor.Green;
                    break;
                case (UInt16)LogType.WARNING:
                    fgColor = ConsoleColor.Yellow;
                    break;
                case (UInt16)LogType.ERROR:
                    fgColor = ConsoleColor.Red;
                    break;
            }

            return fgColor;
        }

        /// <summary>
        /// Permet de supprimer les anciens fichiers de logs
        /// </summary>
        public void RemoveOldFiles()
        {
            // récupération du répertoire contenant les fichiers de logs
            DirectoryInfo parentDirInfo = Directory.GetParent(LogPath);
            if (!parentDirInfo.Exists)
            {
                Directory.CreateDirectory(parentDirInfo.FullName);
                return;
            }

            // récupération de la date courante
            var currentDateTime = DateTime.Now;

            // recherche des fichiers de logs dont la date est supérieure à la durée de rétention
            // des fichier de log
            var filesToDelete = parentDirInfo.EnumerateFiles().Where(fInfo =>
            {
                if (fInfo.Extension != ".log") {
                    return false;
                }

                var lastWTime = fInfo.LastWriteTime;
                var dateTS = currentDateTime - lastWTime;
                if (dateTS.TotalDays > FileRetention)
                {
                    return true;
                }
                else {
                    return false;
                }
            });

            // suppression des fichiers
            filesToDelete.ToList().ForEach(fInfo => {
                fInfo.Delete();
            });
        }

    }
}
