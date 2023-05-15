using BackupBunker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupBunker.Backups
{
    public class IncrBackup : BackupTemplate
    {
        public override AdvancedBackup ActualBackup { get; set; }

        public IncrBackup(AdvancedBackup backup)
        {
            this.ActualBackup = backup;

            int day = (int)DateTime.Today.DayOfWeek;

            if (this.ActualBackup.OnDaysOfWeek[day + 1])
            {
                Console.WriteLine(DateTime.Now + " : making INCR");
                InitializeFolder();
            }
        }

        public override void InitializeFolder()
        {
            foreach (string path_to in this.ActualBackup.Paths_To)
            {
                Directory.CreateDirectory(path_to + "\\INCR");
            }

            foreach (string path in this.ActualBackup.Paths_To)
            {
                DirectoryInfo dir_info = new(path + "\\INCR");

                if (dir_info.GetDirectories().Count() == 0)
                {
                    MakeFirstBackup(path);
                }
                else
                {
                    MakeBackup();
                }
            }
        }

        public void MakeFirstBackup(string path)
        {
            foreach (string path_from in this.ActualBackup.Paths_From)
            {
                string format_time = DateTime.Now.ToString("d.M HH-mm");

                string end_path = path + "\\INCR\\INCR - " + format_time + "\\" + path_from.Split('\\').Last();

                Directory.CreateDirectory(end_path);

                foreach (string sourcePath in Directory.GetFiles(path_from, "*", SearchOption.AllDirectories))
                {
                    string destPath = Path.Combine(end_path, sourcePath.Substring(path_from.Length + 1));

                    Directory.CreateDirectory(Path.GetDirectoryName(destPath));

                    File.Copy(sourcePath, destPath, true);
                }
            }
        }

        public override void MakeBackup()
        {
            string format_time = DateTime.Now.ToString("d.M HH-mm");

            foreach (string path_from in this.ActualBackup.Paths_From)
            {
                foreach (string path_to in this.ActualBackup.Paths_To)
                {
                    string end_path = path_to + "\\INCR\\INCR - " + format_time + "\\" + path_from.Split('\\').Last();

                    Directory.CreateDirectory(end_path);

                    DirectoryInfo dir_info = new(path_to + "\\INCR");

                    string path_diffirence = dir_info.GetDirectories().OrderBy(dir => dir.CreationTime).First().FullName;


                    List<string> things_to_backup = GetMeFilesToBackup(path_from, path_to + "\\INCR");

                    foreach (string part_of_path in things_to_backup)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(end_path + part_of_path));

                        File.Copy(path_from + part_of_path, end_path + part_of_path, true);
                    }
                }
            }
        }

        public List<string> GetMeFilesToBackup(string p_from, string p_to)
        {
            string[] allPaths = Directory.GetFiles(p_from, "*", SearchOption.AllDirectories);
            List<string> main = new();

            foreach (string path in allPaths)
            {
                main.Add(path.Substring(p_from.Length));
            }

            List<string> second = new();

            DirectoryInfo dir_info = new(p_to);

            foreach (DirectoryInfo main_path in dir_info.GetDirectories())
            {
                string end_folder = main_path + "\\" + p_from.Split('\\').Last();

                string[] allPaths1 = Directory.GetFiles(end_folder, "*", SearchOption.AllDirectories);

                foreach (string sub_path in allPaths1)
                {
                    second.Add(sub_path.Substring(end_folder.Length));
                }
            }

            List<string> files_to_backup = main.Except(second).ToList();

            return files_to_backup;
        }
    }
}
