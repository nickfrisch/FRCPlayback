using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using TBAAPI.V3Client.Api;
using TBAAPI.V3Client.Client;
using TBAAPI.V3Client.Model;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace FRC_Playback
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Collection<string> results = new Collection<string>();

        Collection<string> events = new Collection<string>();

        public MainWindow()
        {
            InitializeComponent();
        }
        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        private async void DownloadVideo(string videoURL)
        {
            var youtube = new YoutubeClient();
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoURL);
            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

            string videoFullPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\" + "videos", $"video.{streamInfo.Container}");

            IProgress<double> progress = new Progress<double>(percent =>
            {
                VideoDownloadProgress.Value = percent;
            });
            await youtube.Videos.Streams.DownloadAsync(streamInfo, videoFullPath, progress);

            
            /*
            var client = new HttpClient();
            long? totalByte = 0;
            using (Stream output = File.OpenWrite(videoFullPath))
            {
                Debug.WriteLine("File: " + videoFullPath);
                using (var request = new HttpRequestMessage(HttpMethod.Head, videoURL))
                {
                    totalByte = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result.Content.Headers.ContentLength;

                }
                using (var input = await client.GetStreamAsync(videoURL))
                {
                    byte[] buffer = new byte[16 * 1024];
                    int read;
                    int totalRead = 0;
                    Debug.WriteLine("Download Started");
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                        totalRead += read;
                        Debug.Write($"\rDownloading " + (double)totalRead/(double)totalByte +  "%...");
                    }
                    Debug.WriteLine("Download Complete");
                }
            }*/
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Configuration.Default.BasePath = "https://www.thebluealliance.com/api/v3";
            Configuration.Default.ApiKey.Add("X-TBA-Auth-Key", "Ow8SODFIWtXsxk9KbI0tDPooPibzYcyzhSiJ9saUWQXGIf5ENQ8pwnb1Bi2gwxhj");

            var apiInstance = new MatchApi(Configuration.Default);
            Match match = apiInstance.GetMatch("2023mijac_qm50");

            DownloadVideo(string.Format("https://www.youtube.com/watch?v={0}", match.Videos[0].Key));
        }
    }
}
