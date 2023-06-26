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

        MatchApi matchAPIInstance;
        EventApi eventAPIInstance;
        DistrictApi districtAPIInstance;

        public MainWindow()
        {
            InitializeComponent();

            Configuration.Default.BasePath = "https://www.thebluealliance.com/api/v3";
            Configuration.Default.ApiKey.Add("X-TBA-Auth-Key", "Ow8SODFIWtXsxk9KbI0tDPooPibzYcyzhSiJ9saUWQXGIf5ENQ8pwnb1Bi2gwxhj");

            matchAPIInstance = new MatchApi(Configuration.Default);
            eventAPIInstance = new EventApi(Configuration.Default);
            districtAPIInstance = new DistrictApi(Configuration.Default);
        }

        private async void DownloadVideo(string videoURL)
        {
            var youtube = new YoutubeClient();
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoURL);
            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

            string videoFullPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"FRCPlayback_video.{streamInfo.Container}");

            IProgress<double> progress = new Progress<double>(percent =>
            {
                VideoDownloadProgress.Value = percent;
            });
            await youtube.Videos.Streams.DownloadAsync(streamInfo, videoFullPath, progress);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            TBABrowser bro = new TBABrowser((stringList) =>
            {
                MatchKeyInput.Text = stringList[0];
            }, eventAPIInstance, districtAPIInstance);

            bro.Show();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Match match = matchAPIInstance.GetMatch(MatchKeyInput.Text);

            DownloadVideo(string.Format("https://www.youtube.com/watch?v={0}", match.Videos[0].Key));
        }
    }
}
