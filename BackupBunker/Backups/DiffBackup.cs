using BackupBunker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupBunker.Backups
{
    public class DiffBackup : BackupTemplate
    {
        public override AdvancedBackup ActualBackup { get; set; }

        public DiffBackup(AdvancedBackup backup)
        {
            this.ActualBackup = backup;

            int day = (int)DateTime.Today.DayOfWeek;

            if (this.ActualBackup.OnDaysOfWeek[day + 1])
            {
                Console.WriteLine(DateTime.Now + " : making DIFF");
                InitializeFolder();
            }
        }

        public override void InitializeFolder()
        {
            foreach (string path_to in this.ActualBackup.Paths_To)
            {
                Directory.CreateDirectory(path_to + "\\DIFF");

                DirectoryInfo dir_info = new(path_to + "\\DIFF");

                if (dir_info.GetDirectories().Count() == 5)
                {
                    List<DirectoryInfo> dir_sorted = dir_info.GetDirectories().OrderBy(dir => dir.CreationTime).ToList();

                    Directory.Delete(dir_sorted[0].FullName, true);
                    Directory.Delete(dir_sorted[1].FullName, true);
                }
            }

            foreach (string path in this.ActualBackup.Paths_To)
            {
                DirectoryInfo dir_info = new(path + "\\DIFF");

                if (!dir_info.GetDirectories().Any(dir => dir.Name == "BASE"))
                {
                    MakeBaseBackup(path);
                }
            }

            MakeBackup();
        }

        public void MakeBaseBackup(string path)
        {
            Directory.CreateDirectory(path + "\\DIFF\\BASE");

            foreach (string path_from in this.ActualBackup.Paths_From)
            {
                string format_time = DateTime.Now.ToString("d.M H-mm");

                string end_path = path + "\\DIFF\\BASE\\" + path_from.Split('\\').Last();

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
            string format_time = DateTime.Now.ToString("d.M H-m");

            foreach (string path_from in this.ActualBackup.Paths_From)
            {
                foreach (string path_to in this.ActualBackup.Paths_To)
                {
                    string end_path = path_to + "\\DIFF\\DIFF - " + format_time + "\\" + path_from.Split('\\').Last();

                    Directory.CreateDirectory(end_path);

                    List<string> things_to_backup = GetMeFilesToBackup(path_from, path_to + "\\DIFF\\BASE");

                    foreach (string part_of_path in things_to_backup)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(end_path + part_of_path));

                        File.Copy(path_from + part_of_path, end_path + part_of_path, true);
                    }
                }
            }
        }

        public List<string> GetMeFilesToBackup(string p_from, string p_base)
        {
            string[] allPaths = Directory.GetFiles(p_from, "*", SearchOption.AllDirectories);
            List<string> main = new();

            foreach (string path in allPaths)
            {
                main.Add(path.Substring(p_from.Length));
            }

            string p_base_folder = p_base + "\\" + p_from.Split('\\').Last();

            string[] allPaths1 = Directory.GetFiles(p_base_folder, "*", SearchOption.AllDirectories);
            List<string> second = new();

            foreach (string path in allPaths1)
            {
                second.Add(path.Substring(p_base_folder.Length));
            }

            List<string> files_to_backup = main.Except(second).ToList();

            return files_to_backup;
        }
    }
}
