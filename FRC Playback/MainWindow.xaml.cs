﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TBAAPI.V3Client.Api;
using TBAAPI.V3Client.Client;
using TBAAPI.V3Client.Model;

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

            Configuration.Default.BasePath = "https://www.thebluealliance.com/api/v3";
            Configuration.Default.ApiKey.Add("X-TBA-Auth-Key", "Ow8SODFIWtXsxk9KbI0tDPooPibzYcyzhSiJ9saUWQXGIf5ENQ8pwnb1Bi2gwxhj");

            var apiInstance = new EventApi(Configuration.Default);
            List<EventSimple> eventsSimple = apiInstance.GetEventsByYearSimple(DateTime.Now.Year);
            foreach (EventSimple simpleEvent in eventsSimple)
            {
                events.Add(simpleEvent.Key);
            }
        }
    }
}
