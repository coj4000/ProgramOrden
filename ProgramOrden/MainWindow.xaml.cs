using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProgramLogik;

namespace ProgramOrden
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Logik logikinst = new Logik();
        Stopwatch sw = new Stopwatch();
        private bool noxdone = false;
        private bool ooficedone = false;
        List<string> listviewinf = new List<string>();
        public MainWindow()
        {
            Logik logik = new Logik(@"C:\");
            listviewinf = logik.logicList;
            InitializeComponent();
            listViewFiler.ItemsSource = logik.logicList;
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            logikinst.OpenApp(listViewFiler.SelectedValue.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            
            string listesti = listViewFiler.SelectedValue.ToString();
            bool update = logikinst.FileSiteVersion(listesti);
            if (listesti.Contains("Nox.exe"))
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(listesti).FileVersion;
                string version = versionInfo.Replace("V.", "");

                MessageBox.Show(version);
            }
            if(listesti.Contains("soffice.exe"))
            {
                var versionNavn = FileVersionInfo.GetVersionInfo(listesti).FileDescription;
                string version = versionNavn;

                MessageBox.Show(version);
            }
            
            if (listesti.Contains("Nox.exe") && update == true)
            {
                MessageBoxResult result = MessageBox.Show("Der er en ny version af programmet, Vil du opdatere?", "Spørgsmål", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (File.Exists(@"Overførsler\NoxInstall.exe"))
                    {
                        File.Delete(@"Overførsler\NoxInstall.exe");
                    }
                    MessageBox.Show("Ny version hentes");
                    string url = @"https://www.bignox.com/en/download/fullPackage";

                    WebClient clientweb = new WebClient();
                    clientweb.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Client_DownloadCompleted);
                    clientweb.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_ProgressChanged);
                    sw.Start();
                    try
                    {
                        clientweb.DownloadFileAsync(new Uri(url), @"Overførsler\NoxInstall.exe");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else if (listesti.Contains("Nox.exe") && update == false)
            {
                noxdone = true;
            }
            if (listesti.Contains("soffice.exe") && update == true/* && noxdone == true*/)
            {
                MessageBoxResult result = MessageBox.Show("Der er en ny version af programmet, Vil du opdatere?", "Spørgsmål", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (File.Exists(@"Overførsler\OpenOfficeInstall.exe"))
                    {
                        File.Delete(@"Overførsler\OpenOfficeInstall.exe");
                    }
                    MessageBox.Show("Ny version hentes");
                    string url = @"https://sourceforge.net/projects/openofficeorg.mirror/files/latest/download";

                    WebClient clientweb = new WebClient();
                    clientweb.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Client1_DownloadCompleted);
                    clientweb.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_ProgressChanged);
                    sw.Start();
                    try
                    {
                        clientweb.DownloadFileAsync(new Uri(url), @"Overførsler\OpenOfficeInstall.exe");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Denne Version er den nyeste version af programmet");
            }

        }
        private void Client_ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Calculate download speed and output it to labelSpeed.
            labelspeed.Content = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));

            // Update the progressbar percentage only when the value is not the same.
            progressBar1.Value = e.ProgressPercentage;

            // Show the percentage on our label.
            labelperc.Content = e.ProgressPercentage.ToString() + "%";

            // Update the label with how much data have been downloaded so far and the total size of the file we are currently downloading
            labeldownloadet.Content = string.Format("{0} MB's / {1} MB's",
                (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));
        }

        private void Client_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            sw.Reset();

            MessageBoxResult result = MessageBox.Show("Vil du opdatere programmet nu?", "Spørgsmål", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                UpdateFil(@"Overførsler\NoxInstall.exe");
            }
            labeldownloadet.Content = "";
            labelspeed.Content = "";
            labelperc.Content = "";
            noxdone = true;


        }
        private void Client1_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            sw.Reset();

            MessageBoxResult result = MessageBox.Show("Vil du opdatere programmet nu?", "Spørgsmål", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                UpdateFil(@"Overførsler\OpenOfficeInstall.exe");
            }
            labeldownloadet.Content = "";
            labelspeed.Content = "";
            labelperc.Content = "";
            ooficedone = true;

        }
        private void UpdateFil(string filnavn)
        {
            Process p = new Process();
            p.StartInfo.FileName = filnavn;
            p.StartInfo.Arguments = "/x \"C:\\Application.msi\"/qn";
            p.Start();
        }

        private void Button_Gzip_Click(object sender, RoutedEventArgs e)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("accept", "*/*");
                byte[] filedata = client.DownloadData("https://www.bignox.com/en/download/fullPackage");

                using (MemoryStream ms = new MemoryStream(filedata))
                {
                    using (FileStream decompressedFileStream = File.Create("NoxInstall.exe"))
                    {
                        using (GZipStream decompressionStream = new GZipStream(ms, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(decompressedFileStream);
                        }
                    }
                }
            }
        }

        private void ButtonAuto_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listviewinf)
            {

            bool update = logikinst.FileSiteVersion(item);
            if (item.Contains("Nox.exe"))
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(item).FileVersion;
                string version = versionInfo.Replace("V.", "");

                MessageBox.Show(version);
            }
            if (item.Contains("soffice.exe"))
            {
                var versionNavn = FileVersionInfo.GetVersionInfo(item).FileDescription;
                string version = versionNavn;

                MessageBox.Show(version);
            }

            if (item.Contains("Nox.exe") && update == true)
            {
                MessageBoxResult result = MessageBox.Show("Der er en ny version af programmet, Vil du opdatere?", "Spørgsmål", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (File.Exists(@"Overførsler\NoxInstall.exe"))
                    {
                        File.Delete(@"Overførsler\NoxInstall.exe");
                    }
                    MessageBox.Show("Ny version hentes");
                    string url = @"https://www.bignox.com/en/download/fullPackage";

                    WebClient clientweb = new WebClient();
                    clientweb.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Client_DownloadCompleted);
                    clientweb.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_ProgressChanged);
                    sw.Start();
                    try
                    {
                        clientweb.DownloadFileAsync(new Uri(url), @"Overførsler\NoxInstall.exe");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else if (item.Contains("Nox.exe") && update == false)
            {
                noxdone = true;
            }
            if (item.Contains("soffice.exe") && update == true/* && noxdone == true*/)
            {
                MessageBoxResult result = MessageBox.Show("Der er en ny version af programmet, Vil du opdatere?", "Spørgsmål", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (File.Exists(@"Overførsler\OpenOfficeInstall.exe"))
                    {
                        File.Delete(@"Overførsler\OpenOfficeInstall.exe");
                    }
                    MessageBox.Show("Ny version hentes");
                    string url = @"https://sourceforge.net/projects/openofficeorg.mirror/files/latest/download";

                    WebClient clientweb = new WebClient();
                    clientweb.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Client1_DownloadCompleted);
                    clientweb.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_ProgressChanged);
                    sw.Start();
                    try
                    {
                        clientweb.DownloadFileAsync(new Uri(url), @"Overførsler\OpenOfficeInstall.exe");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Denne Version er den nyeste version af programmet");
            }

            }
        }
    }
}
