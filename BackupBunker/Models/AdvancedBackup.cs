using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupBunker.Models
{
    public class AdvancedBackup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public List<string> Paths_From { get; set; }
        public List<string> Paths_To { get; set; }
        public List<bool> OnDaysOfWeek { get; set; }
        public List<TimeOnly> AtTimes { get; set; }
        public string CreateAt { get; set; }
    }
}
