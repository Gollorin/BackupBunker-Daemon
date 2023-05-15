using BackupBunker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupBunker.Backups
{
    public abstract class BackupTemplate
    {
        public abstract AdvancedBackup ActualBackup { get; set; }

        public abstract void InitializeFolder();

        public abstract void MakeBackup();
    }
}
