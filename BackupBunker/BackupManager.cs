using BackupBunker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupBunker
{
    public class BackupManager
    {
        private List<Backup> MyBackupsRaw { get; set; }
        private List<BackupSchedule> MyBackupsSchedule { get; set; }
        private string PathToData { get; set; }

        public BackupManager(List<Backup> my_backups, string data_path)
        {
            this.MyBackupsRaw = my_backups;
            this.MyBackupsSchedule = new();

            this.PathToData = data_path;

            CheckMyBackups();
            WhenItsTime();
        }

        // TODO: zkontrolovat jestli už ten back up nemá
        private void CheckMyBackups()
        {
            CheckBackupDirectory();

            foreach (Backup backup in this.MyBackupsRaw)
            {
                List<TimeOnly> temp_time = ConvertToTimeOnlyList(backup.AtTimes);

                AdvancedBackup temp_back = new AdvancedBackup()
                {
                    Id = backup.Id,
                    Name = backup.Name,
                    Type = backup.Type,
                    Paths_From = backup.Paths_From,
                    Paths_To = backup.Paths_To,
                    OnDaysOfWeek = backup.OnDaysOfWeek,
                    AtTimes = temp_time,
                    CreateAt = backup.CreateAt,
                };

                MakeBackupJson(temp_back);

                BackupSchedule temp_sched = new(backup.Id, temp_time);

                this.MyBackupsSchedule.Add(temp_sched);
            }
        }

        private List<TimeOnly> ConvertToTimeOnlyList(List<string> timeStrings)
        {
            List<TimeOnly> timeList = new();

            foreach (string timeString in timeStrings)
            {
                if (TimeOnly.TryParse(timeString, out TimeOnly time))
                {
                    timeList.Add(time);
                    Console.WriteLine(time);
                }
            }

            return timeList;
        }

        private void CheckBackupDirectory()
        {
            if (!Directory.Exists(this.PathToData + "\\Backups"))
                Directory.CreateDirectory(this.PathToData + "\\Backups");
            else
            {
                DirectoryInfo backups_directory = new(this.PathToData + "\\Backups");

                foreach (FileInfo dir_backup in backups_directory.GetFiles())
                {
                    File.Delete(dir_backup.FullName);
                }
            }
        }

        private void MakeBackupJson(AdvancedBackup backup)
        {
            string backup_json = JsonConvert.SerializeObject(backup);

            File.WriteAllText(this.PathToData + "\\Backups\\" + backup.Id + ".json", backup_json);
        }

        private void WhenItsTime()
        {
            BackupMachine backupMachine = new BackupMachine(this.PathToData);

            while (true)
            {
                TimeOnly actualTime = TimeOnly.FromDateTime(DateTime.Now);

                foreach (BackupSchedule schedule in this.MyBackupsSchedule)
                {
                    foreach (TimeOnly backup in schedule.AtTimes)
                    {
                        if (actualTime.Hour == backup.Hour && actualTime.Minute == backup.Minute)
                        {
                            backupMachine.MakeBackup(schedule.BackupId);
                        }
                    }
                }

                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }
    }
}
