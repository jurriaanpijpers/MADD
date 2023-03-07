using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace MADD
{
    class Program
    {
        /// <summary>
        /// Call EXE with paramters like:
        /// brand=OV username=DD12345 password=1q2w3e4r folder=C:\
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var dictionary = args
                .Select(a => a.Split('='))
                .ToDictionary(a => a[0], a => a.Length == 2 ? a[1] : null);

            string brand = "", username = "", password = "", folder = "";

            if (dictionary.Count != 0)
            {
                dictionary.TryGetValue("brand", out  brand);
                dictionary.TryGetValue("username", out  username);
                dictionary.TryGetValue("password", out  password);
                dictionary.TryGetValue("folder", out  folder);

            }
            else
            {
                Console.WriteLine("Insert brand (OV,AP,AC):");
                brand = Console.ReadLine();
                Console.WriteLine("Insert D code username:");
                username = Console.ReadLine();
                Console.WriteLine("Insert password:");
                password = Console.ReadLine();
                Console.WriteLine("Download to folder:");
                folder = Console.ReadLine();
            }


            DownloadMADD(brand.ToUpperInvariant(), username, password, folder);
        }


        public static void DownloadMADD(string brand,  string username, string password, string toFolder)
        {
            string baseUrl = "";
            if (brand == "OV")
            {
                baseUrl=    "https://edoc-partners.opel.com";
            }
            else if (brand == "AC")
            {
                baseUrl = "https://edoc-partners.citroen.com";
            }
            else if(brand == "AP")
            {
                baseUrl = "https://edoc-partners.peugeot.com";
            }

            GetFilter[] filters = GetFilters(username, password, baseUrl);
            string rrdi = filters.FirstOrDefault().name;
            Folder[] folders = GetFolders(rrdi, username, password, baseUrl);

            foreach (Folder folder in folders)
            {
                string folderPath = Path.Combine(toFolder, folder.folderName);
                Directory.CreateDirectory(folderPath);
                File[] files = GetFiles(rrdi, username, password, baseUrl, folder.folderName);
                foreach (File file in files)
                {
                    DownloadFile(rrdi, username, password, baseUrl, file.id, folderPath);
                }
            }
        }

       
        public static void DownloadFile(string rrdi, string username, string password, string baseurl, string id, string toFolder)
        {
            string url = $"{baseurl}/api/files/{id}/{rrdi}/0";
            string encoded = Convert
               .ToBase64String(Encoding.GetEncoding("ISO-8859-1")
               .GetBytes(username + ":" + password));

            using (var client = new HttpClient())
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add("Authorization", "Basic " + encoded);
                HttpResponseMessage response = client.SendAsync(requestMessage).Result;

                if (response.Content.Headers.ContentDisposition == null)
                {
                    IEnumerable<string> contentDisposition;
                    if (response.Content.Headers.TryGetValues("Content-Disposition", out contentDisposition))
                    {
                        var cd = contentDisposition.ToArray()[0].TrimEnd(';');
                        cd = Helpers.RemoveInvalidChars(cd);
                        response.Content.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse(cd);
                    }
                }

                string filename = response.Content.Headers.ContentDisposition.FileName;



                string path = Path.Combine(toFolder, filename);
                using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    var stream = response.Content.ReadAsStreamAsync();
                    stream.Result.CopyTo(fs);
                }


            }


        }


        public static Folder[] GetFolders(string rrdi, string username, string password, string baseurl)
        {
            string baseUrl = $"{baseurl}/api/documents/folders/{rrdi}";
            string encoded = Convert
                 .ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                 .GetBytes(username + ":" + password));
            Folder[] folders = null;


            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, baseUrl);
                requestMessage.Headers.Add("Authorization", "Basic " + encoded);
                HttpResponseMessage response = client.SendAsync(requestMessage).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    folders = JsonConvert.DeserializeObject<Folder[]>(result);
                }
            }
            return folders;
        }


        public static File[] GetFiles(string rrdi, string username, string password, string baseurl, string folderName)
        {
            string baseUrl = $"{baseurl}/api/documents/search_documents";
            string encoded = Convert
                 .ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                 .GetBytes(username + ":" + password));
            File[] files = null;


            PostFilter f = new PostFilter()
            {
                documentName = "*",
                filterName = $"{rrdi}",
                folderName = folderName
            };

            string jsonMessage = JsonConvert.SerializeObject(f);

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, baseUrl)
                {
                    Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json")
                };
                requestMessage.Headers.Add("Authorization", "Basic " + encoded);
                HttpResponseMessage response = client.SendAsync(requestMessage).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    files = JsonConvert.DeserializeObject<File[]>(result);
                }
            }

            return files;
        }
        public static GetFilter[] GetFilters(string username, string password, string baseurl)
        {
            string baseUrl = $"{baseurl}/api/filters";
            string encoded = Convert
                 .ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                 .GetBytes(username + ":" + password));
            GetFilter[] filters = null;



            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, baseUrl);
                requestMessage.Headers.Add("Authorization", "Basic " + encoded);
                HttpResponseMessage response = client.SendAsync(requestMessage).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    filters = JsonConvert.DeserializeObject<GetFilter[]>(result);
                }
            }

            return filters;
        }

    }
}
