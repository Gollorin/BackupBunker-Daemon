using BackupBunker.Backups;
using BackupBunker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupBunker
{
    public class BackupMachine
    {
        private AdvancedBackup ActualBackup { get; set; }
        private string PathToData { get; set; }

        public BackupMachine(string path)
        {
            this.PathToData = path;    
        }

        public async Task MakeBackup(string id)
        {
            FindBackupFile(id);

            if (this.ActualBackup.Type == "FULL")
            {
                _ = new FullBackup(this.ActualBackup);
            } else if(this.ActualBackup.Type == "DIFF")
            {
                _ = new DiffBackup(this.ActualBackup);
            } else if (this.ActualBackup.Type == "INCR")
            {
                _ = new IncrBackup(this.ActualBackup);
            }
        }

        public void FindBackupFile(string id)
        {
            string backup_json = File.ReadAllText(this.PathToData + "\\Backups\\" + id + ".json");

            this.ActualBackup = JsonConvert.DeserializeObject<AdvancedBackup>(backup_json);
        }
    }
}
