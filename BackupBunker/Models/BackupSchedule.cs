using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupBunker.Models
{
    public class BackupSchedule
    {
        public string BackupId { get; set; }
        public List<TimeOnly> AtTimes { get; set; }

        public BackupSchedule(string backup_id, List<TimeOnly> atTimes)
        {
            this.BackupId = backup_id;
            this.AtTimes = atTimes;
        }
    }
}
