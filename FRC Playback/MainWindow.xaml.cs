using FRC_Playback.TBAUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
        MatchApi matchApiInstance = new MatchApi(Configuration.Default);
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void DownloadVideo(string videoKey)
        {
            string videoURL = string.Format("https://www.youtube.com/watch?v={0}", videoKey);

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
            MessageBox.Show("Finished downloading match video!");
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            TBABrowser bro = new TBABrowser((stringList) =>
            {
                MessageBox.Show(stringList[0]);
                DownloadVideo(matchApiInstance.GetMatch(stringList[0]).Videos[0].Key);
            });
            bro.Show();
        }
    }
}
