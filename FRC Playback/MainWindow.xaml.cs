using FRC_Playback.TBAUtils;
using MediaToolkit.Model;
using MediaToolkit.Options;
using MediaToolkit;
using System;
using System.Windows;
using TBAAPI.V3Client.Api;
using TBAAPI.V3Client.Client;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

using Match = TBAAPI.V3Client.Model.Match;
using Microsoft.Win32;
using System.Windows.Forms;

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

        string lastVideoPath = string.Empty;

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

            lastVideoPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"FRCPlayback_video.{streamInfo.Container}");

            IProgress<double> progress = new Progress<double>(percent =>
            {
                VideoDownloadProgress.Value = percent;
            });
            await youtube.Videos.Streams.DownloadAsync(streamInfo, lastVideoPath, progress);
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

        private void ExactFrames_Click(object sender, RoutedEventArgs e)
        {
            using (var engine = new Engine())
            {
                var mp4 = new MediaFile { Filename = lastVideoPath };

                engine.GetMetadata(mp4);

                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.ShowDialog();

                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.Cancel)
                    return;

                var i = 0;
                while (i < mp4.Metadata.Duration.TotalSeconds)
                {
                    var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(i) };
                    var outputFile = new MediaFile { Filename = string.Format("{0}\\image-{1}.jpeg", dialog.SelectedPath, i) };
                    engine.GetThumbnail(mp4, outputFile, options);
                    i++;
                }
            }
        }
    }
}
