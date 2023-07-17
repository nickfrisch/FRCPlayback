using MediaToolkit.Model;
using MediaToolkit.Options;
using MediaToolkit;
using System;
using System.Windows;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace FRC_Playback
{
    /// <summary>
    /// Interaction logic for ExtractFrameMenu.xaml
    /// </summary>
    public partial class ExtractFrameMenu : Window
    {

        BackgroundWorker bw = new BackgroundWorker();

        public ExtractFrameMenu(string defaultPath)
        {
            InitializeComponent();

            VideoDirectory.Text = defaultPath;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string videoDir = VideoDirectory.Text;
            string saveDir = SaveDirectory.Text;
            double interval = Double.Parse(IntervalInput.Text);

            bw.ProgressChanged += (sender, args) => 
            {
                ExtractionProgress.Value = args.ProgressPercentage;
            };

            bw.DoWork += (sender, args) =>
            {
                var worker = sender as BackgroundWorker;

                using (var engine = new Engine())
                {
                    var mp4 = new MediaFile { Filename = videoDir };

                    engine.GetMetadata(mp4);

                    var idx = 0;
                    double currentTime = 0;
                    TimeSpan span = TimeSpan.FromSeconds(interval);

                    while (currentTime < mp4.Metadata.Duration.TotalSeconds)
                    {
                        var options = new ConversionOptions { Seek = span };
                        var outputFile = new MediaFile { Filename = string.Format("{0}\\image-{1}-{2}.jpeg", saveDir, idx, DateTime.UtcNow.ToString("yyyy_MM_ddTHH_mm_ss_fffffffZ")) };
                        engine.GetThumbnail(mp4, outputFile, options);

                        worker.ReportProgress((int) ((currentTime / mp4.Metadata.Duration.TotalSeconds) * 100));
                        currentTime += span.TotalSeconds;
                        idx++;
                    }
                }
            };

            bw.WorkerReportsProgress = true;

            bw.RunWorkerAsync();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
                return;
            else
                SaveDirectory.Text = dialog.SelectedPath;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BrowseVideoLocationButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
                return;
            else
                VideoDirectory.Text = dialog.FileName;
        }
    }
}
