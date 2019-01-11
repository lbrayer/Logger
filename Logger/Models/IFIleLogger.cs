using System;

namespace Logger.Models
{
    public interface IFileLogger : ILogger
    {

        /// <summary>
        /// Durée de rétention des fichiers en jours
        /// </summary>
        UInt16 FileRetention {
            get;
            set;
        }

        /// <summary>
        /// Emplacement du fichier de log
        /// </summary>
        string LogPath {
            get;
            set;
        }

        /// <summary>
        /// Permet de supprimer les fichiers de log datant de plus de "fileRetention" jours
        /// </summary>
        void RemoveOldFiles();

    }
}
