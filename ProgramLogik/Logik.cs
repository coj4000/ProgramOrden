using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using RestSharp;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLogik
{
    public class Logik
    {

        public List<string> logicList = new List<string>();
        public List<string> listeStrings;
        public Logik()
        {

        }
        public Logik(string path)
        {
            DrevSkan(path);
        }
        public void OpenApp(string path)
        {
            Process.Start(path);
        }
        public void FindDirectories(string path, List<string> files)
        {
            try
            {

                Directory.GetFiles(path, @"*.exe").Where(x => !x.EndsWith(@".dll"))
                    .ToList()
                    .ForEach(s => files.Add(s));

                Directory.GetDirectories(path).Where(x => !x.Contains(@"Recycle.Bin")).Where(x => !x.Contains(@"Documents and Settings")).Where(x => !x.Contains(@"Users")).Where(x => !x.Contains(@"Recovery")).Where(x => !x.Contains(@"System Volume"))
               .ToList()
               .ForEach(s => FindDirectories(s, files));
            }
            catch { }
        }
        public void DrevSkan(string path)
        {
            SetAccessRule(@"C:\");
            //DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            //FileInfo[] info = di.GetFiles("*.exe");
            //foreach (FileInfo i in info)
            //{
            //    logicList.Add(i.Name);
            //}
            listeStrings = new List<string>();
            FindDirectories(path, listeStrings);
            foreach (string item in listeStrings)
            {
                if (item.Contains("Nox.exe"))
                {
                    logicList.Add(item);
                }
            }
            //logicList = listeStrings;
        }
        public static void SetAccessRule(string directory)
        {
            System.Security.AccessControl.DirectorySecurity sec = System.IO.Directory.GetAccessControl(directory);
            FileSystemAccessRule accRule = new FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, FileSystemRights.FullControl, AccessControlType.Allow);
            sec.AddAccessRule(accRule);
        }
        public void UpdateFromFileVersion(string filnavn)
        {

            //if (FileSiteVersion(filnavn, version) && downloadCompleted ==true && filnavn.Contains("Nox"))
            //{
            //    Process p = new Process();
            //    p.StartInfo.FileName = @"C:\NoxInstall.exe";
            //    p.StartInfo.Arguments = "/x \"C:\\Application.msi\"/qn";
            //    p.Start();
            //}
        }
        public bool FileSiteVersion(string filename)
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(filename).FileVersion;
            string version = versionInfo.Replace("V.", "");

            string pageinfo = "";
            bool oldversion = false;
            if (filename.Contains("Nox"))
            {
                //var client = new RestClient("https://www.bignox.com/");
                //var reguest = new RestRequest("", Method.GET);

                //client.ExecuteAsync(reguest, response =>
                //{
                //    pageinfo = response.Content.ToString();
                //});
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    pageinfo = client.DownloadString(@"https://www.bignox.com/");
                }
                if (!pageinfo.Contains(version))
                {
                    oldversion = true;
                }


            }
            return oldversion;
        }


    }
}
