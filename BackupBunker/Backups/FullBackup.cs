using BackupBunker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupBunker.Backups
{
    public class FullBackup : BackupTemplate
    {
        public override AdvancedBackup ActualBackup { get; set; }

        public FullBackup(AdvancedBackup backup)
        {
            this.ActualBackup = backup;

            int day = (int)DateTime.Today.DayOfWeek;

            if (ActualBackup.OnDaysOfWeek[day + 1])
            {
                Console.WriteLine(DateTime.Now + " : making FULL");

                InitializeFolder();
                MakeBackup();
            }
        }

        public override void InitializeFolder()
        {
            foreach (string path_to in this.ActualBackup.Paths_To)
            {
                DirectoryInfo dir_info = new(path_to);

                if (dir_info.GetDirectories().Count() == 5)
                {
                    List<DirectoryInfo> dir_sorted = dir_info.GetDirectories().OrderBy(dir => dir.CreationTime).ToList();

                    string path_to_oldest = dir_sorted[0].FullName;

                    Directory.Delete(path_to_oldest, true);
                }
            }
        }

        public override void MakeBackup()
        {
            foreach (string path_from in this.ActualBackup.Paths_From)
            {
                foreach (string path_to in this.ActualBackup.Paths_To)
                {
                    string format_time = DateTime.Now.ToString("d.M H-mm");

                    string end_path = path_to + "\\FULL - " + format_time + "\\" + path_from.Split('\\').Last();

                    Directory.CreateDirectory(end_path);

                    foreach (string sourcePath in Directory.GetFiles(path_from, "*", SearchOption.AllDirectories))
                    {
                        string destPath = Path.Combine(end_path, sourcePath.Substring(path_from.Length + 1));

                        Directory.CreateDirectory(Path.GetDirectoryName(destPath));

                        File.Copy(sourcePath, destPath, true);
                    }
                }
            }
        }
    }
}
