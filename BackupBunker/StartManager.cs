using BackupBunker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BackupBunker
{
    public class StartManager
    {
        private HttpClient _httpClient;
        private const string API_URL = @"https://localhost:7187/api/BackupBunker/";
        private readonly string DATAPATH;
        private string MyId { get; set; }

        public StartManager()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(API_URL);

            this.DATAPATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\BB";

            if (!Directory.Exists(DATAPATH))
                Directory.CreateDirectory(DATAPATH);
        }

        public async Task StartAsync()
        {
            string path = this.DATAPATH + "\\id.txt";

            if (!File.Exists(path))
            {
                string id = await GetIdAsync();

                this.MyId = id;

                File.WriteAllText(path, id);
                File.SetAttributes(path, FileAttributes.Hidden);
            } else
            {
                string id = File.ReadAllText(path);
                this.MyId = id;
            }

            List<Backup> backups = await GetMyBackups();
            _ = new BackupManager(backups, this.DATAPATH);
        }

        private async Task<string> GetIdAsync()
        {
            Console.Clear();

            Console.WriteLine("Enter email:");
            string email = Console.ReadLine();

            Console.WriteLine("Enter password:");
            string password = Console.ReadLine();

            string id = await Login(email, password);
            return id;
        }

        public async Task<string> Login(string email, string password)
        {
            User user = new()
            {
                Email = email,
                Password = password
            };

            var response = await this._httpClient.PostAsJsonAsync(API_URL + "/login", user);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<JustId>(json).Id;
            } else
            {
                string id = await GetIdAsync();
                return id;
            }
        }

        public async Task<List<Backup>> GetMyBackups()
        {
            string backups_json = await this._httpClient.GetStringAsync("find-backups-by-id/" + MyId);

            List<Backup> backups = JsonConvert.DeserializeObject<List<Backup>>(backups_json);

            return backups;
        }
    }
}
