using FRC_Playback.TBAUtils;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using TBAAPI.V3Client.Api;
using TBAAPI.V3Client.Client;
using TBAAPI.V3Client.Model;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using Match = TBAAPI.V3Client.Model.Match;

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
            }, eventAPIInstance, districtAPIInstance, matchAPIInstance);

            bro.Show();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Match match = matchAPIInstance.GetMatch(MatchKeyInput.Text);

            DownloadVideo(string.Format("https://www.youtube.com/watch?v={0}", match.Videos[0].Key));
        }
    }
}
