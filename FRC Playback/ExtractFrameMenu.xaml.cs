using MediaToolkit.Model;
using MediaToolkit.Options;
using MediaToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

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

        private void extract(object sender, string videoDir, string saveDir, double interval)
        {

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
                        var outputFile = new MediaFile { Filename = string.Format("{0}\\image-{1}.jpeg", saveDir, idx) };
                        engine.GetThumbnail(mp4, outputFile, options);

                        worker.ReportProgress((int) ((currentTime / mp4.Metadata.Duration.TotalSeconds) * 100));
                        MessageBox.Show("Report: " + (int)((currentTime / mp4.Metadata.Duration.TotalSeconds) * 100));
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
