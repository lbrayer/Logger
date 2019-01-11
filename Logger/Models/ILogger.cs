using System;

namespace Logger.Models
{
    /// <summary>
    /// Interface ILogger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Fonction writeLog
        /// </summary>
        /// <param name="logMessage"></param>
        void WriteLog(string logMessage);

        /// <summary>
        /// Fonction writeLog qui spécifie le composant
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="component"></param>
        void WriteLog(string logMessage, string component);

        /// <summary>
        /// Fonction writeLog qui spécifie le type de log
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="logType"></param>
        void WriteLog(string logMessage, UInt16 logType);

        /// <summary>
        /// Fonction writeLog qui spécifie le composant et type de log
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="component"></param>
        /// <param name="logType"></param>
        void WriteLog(string logMessage, string component, UInt16 logType);

    }
}
