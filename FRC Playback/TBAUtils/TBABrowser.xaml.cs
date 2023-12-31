using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

namespace FRC_Playback.TBAUtils
{
    /// <summary>
    /// Interaction logic for TBABrowser.xaml
    /// </summary>
    public partial class TBABrowser : Window
    {
        Collection<string> events = new Collection<string>();

        Action<List<string>> callback;

        TBACache tbaData;
        string tbaDataCachePath;

        EventApi eventAPIInstance;
        DistrictApi districtAPIInstance;
        MatchApi matchAPIInstance;

        private class TreeHeaderObj
        {
            string header;
            string key;
            int id; // 0 Year, 1 District, 2 Events, 3 Match

            public TreeHeaderObj(int id, string header, string key)
            {
                this.id = id;
                this.header = header;
                this.key = key;
            }

            public override string ToString()
            {
                return this.header;
            }

            public string GetKey()
            {
                return this.key;
            }

            public int GetId()
            {
                return this.id;
            }
        }

        public TBABrowser(Action<List<string>> callback, EventApi eventApi, DistrictApi districtApi, MatchApi matchApi)
        {
            InitializeComponent();

            this.callback = callback;

            //////////////////////Blue Alliance Client Setup//////////////////////

            eventAPIInstance = eventApi;
            districtAPIInstance = districtApi;
            matchAPIInstance = matchApi;

            //////////////////////TBA Cache Reader//////////////////////

            // Get the file path for the TBA Cache Data
            tbaDataCachePath = System.IO.Path.GetTempPath() + "FRCPlayback_TBADataCache.json";

            // Open the file stream if the file exists otherwise create a new file
            FileStream fileStream;
            if (!File.Exists(tbaDataCachePath))
                fileStream = File.Create(tbaDataCachePath);
            else
                fileStream = new FileStream(tbaDataCachePath, FileMode.Open, FileAccess.ReadWrite);

            // Start the Deserialization of the JSON object using a Steam to conserve memory
            var serializer = new Newtonsoft.Json.JsonSerializer();
            using (var textReader = new StreamReader(fileStream))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                tbaData = serializer.Deserialize<TBACache>(jsonReader);
            }

            fileStream.Close(); // Close the file stream because it is no longer needed.

            buildBasicCache(false);

            TBASearchBar.Items.Clear();

            foreach (YearCache year in tbaData.years)
            {
                TreeViewItem yearItem = new TreeViewItem();
                yearItem.Header = new TreeHeaderObj(0, year.year.ToString(), year.year.ToString());
                yearItem.AllowDrop = true;
                yearItem.Expanded += new RoutedEventHandler((sender, args) =>
                {
                    if (sender == TBATreeView.SelectedItem)
                        onYearItemSelected(year, yearItem);
                });

                TBATreeView.Items.Insert(0, yearItem);
            }
        }

        void onYearItemSelected(YearCache year, TreeViewItem yearItem)
        {
            if (isRewriteRequired(year.lastUpdateTime, year.year, year.districts.Count))
            {
                buildBasicDistrictCache(year.year);
                writeOutCache();
            }

            yearItem.Items.Clear();

            foreach (DistrictCache district in year.districts)
            {
                TreeViewItem districtItem = new TreeViewItem();
                districtItem.Header = new TreeHeaderObj(1, district.districtName, district.districtKey);
                districtItem.AllowDrop = true;
                districtItem.Expanded += new RoutedEventHandler((sender, args) => {
                    if (sender == TBATreeView.SelectedItem)
                        onDistrictItemSelected(year, district, districtItem);
                });

                yearItem.Items.Add(districtItem);
            }
        }

        void onDistrictItemSelected(YearCache year, DistrictCache district, TreeViewItem districtItem)
        {
            if (isRewriteRequired(district.lastUpdateTime, year.year, district.events.Count))
            {
                buildBasicEventCache(year.year, district.districtKey, district.districtName);
                writeOutCache();
            }

            districtItem.Items.Clear();

            foreach (EventCache eventSimple in district.events)
            {
                TreeViewItem eventItem = new TreeViewItem();
                eventItem.Header = new TreeHeaderObj(2, eventSimple.eventName, eventSimple.eventKey);
                eventItem.AllowDrop = true;
                eventItem.Expanded += new RoutedEventHandler((sender, args) =>
                {
                    if(sender == TBATreeView.SelectedItem)
                        onEventItemSelected(year, district, eventSimple, eventItem);
                });

                districtItem.Items.Add(eventItem);
            }
        }

        void onEventItemSelected(YearCache year, DistrictCache district, EventCache eventSimple, TreeViewItem eventItem)
        {
            if (isRewriteRequired(eventSimple.lastUpdateTime, year.year, eventSimple.matchCaches.Count))
            {
                buildBasicMatchCache(year.year, district.districtKey, eventSimple.eventKey, eventSimple.eventName);
                writeOutCache();
            }

            eventItem.Items.Clear();

            foreach (MatchCache match in eventSimple.matchCaches)
            {
                TreeViewItem matchItem = new TreeViewItem();
                matchItem.Header = new TreeHeaderObj(3, match.matchKey, match.matchKey);
                matchItem.AllowDrop = true;
                matchItem.Selected += new RoutedEventHandler((sender, args) => {
                    if (sender == TBATreeView.SelectedItem)
                        onMatchItemSelected(match);
                });

                eventItem.Items.Add(matchItem);
            }

        }

        void onMatchItemSelected(MatchCache match)
        {
            BlueAllianceTeam1Number.Text = match.blue1;
            BlueAllianceTeam2Number.Text = match.blue2;
            BlueAllianceTeam3Number.Text = match.blue3;
            RedAllianceTeam1Number.Text = match.red1;
            RedAllianceTeam2Number.Text = match.red2;
            RedAllianceTeam3Number.Text = match.red3;
            var embed = "<html><head>" +
                        "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=Edge\"/>" +
                        "</head><body>" +
                        //"<iframe width=\"368\" height=\"207\" src=\"{0}\"" +
                        "<iframe frameborder=\"0\" style=\"overflow: hidden; height: 90%; width: 90%; position: absolute;\" src=\"{0}\"" +
                        "frameborder = \"0\" allow = \"autoplay; encrypted-media\" allowfullscreen></iframe>" +
                        "</body></html>";
            var url = "https://www.youtube.com/embed/" + matchAPIInstance.GetMatch(match.matchKey).Videos[0].Key;
            string previewHtml = string.Format(embed, url);
            VideoPreview.NavigateToString(previewHtml);
        }

        void buildBasicCache(bool forceAllUpdate)
        {
            //If there was no base input from the file then create the new structure
            if(tbaData == null)
                tbaData = new TBACache(DateTime.Now.ToLongDateString(), new List<YearCache>());

            // For each year check for the basic data
            for(int i = 2010; i < DateTime.Now.Year; i++)
            {
                // Set a flag and check every value in the json for a match
                bool yearFlag = false;
                foreach (YearCache year in tbaData.years)
                {
                    if(year.year == i)
                        yearFlag = true;
                }

                // If there is no match or there is a force update pull the data from TBA
                if (!yearFlag || forceAllUpdate)
                    buildBasicDistrictCache(i);
            }

            // For the latest year repull the data no matter what
            buildBasicDistrictCache(DateTime.Now.Year);

            writeOutCache();
        }

        void buildBasicDistrictCache(int year)
        {
            List<DistrictList> districts = districtAPIInstance.GetDistrictsByYear(year);
            string dateString = DateTime.Now.ToString();

            List<DistrictCache> districtCacheObj = new List<DistrictCache>();
            foreach (DistrictList district in districts)
            {
                districtCacheObj.Add(new DistrictCache(dateString, district.Key, district.DisplayName, new List<EventCache>()));
            }

            int yearIdx = findYearIndex(year);

            if (yearIdx >= 0)
            {
                YearCache yearItem = tbaData.years[yearIdx];// = new YearCache(dateString, year, districtCacheObj);
                yearItem.lastUpdateTime = dateString;
                yearItem.year = year;
                yearItem.districts = districtCacheObj;
            } 
            else
                tbaData.years.Add(new YearCache(dateString, year, districtCacheObj));
        }

        void buildBasicEventCache(int year, string districtKey, string districtName)
        {
            List<EventSimple> events = districtAPIInstance.GetDistrictEventsSimple(districtKey);
            string dateString = DateTime.Now.ToString();

            List<EventCache> eventCacheObj = new List<EventCache>();
            foreach (EventSimple eventSimple in events)
            {
                eventCacheObj.Add(new EventCache(dateString, eventSimple.Key, eventSimple.Name, new List<MatchCache>()));
            }

            int yearIdx = findYearIndex(year);
            int districtIdx = findDistrictIndex(yearIdx, districtKey);

            if (districtIdx >= 0) {
                DistrictCache districtItem = tbaData.years[yearIdx].districts[districtIdx];
                districtItem.lastUpdateTime = dateString;
                districtItem.districtKey = districtKey;
                districtItem.districtName = districtName;
                districtItem.events = eventCacheObj;
            } else
                tbaData.years[yearIdx].districts.Add(new DistrictCache(dateString, districtKey, districtName, eventCacheObj));
        }

        void buildBasicMatchCache(int year, string districtKey, string eventKey, string eventName)
        {
            List<MatchSimple> matches = eventAPIInstance.GetEventMatchesSimple(eventKey);
            string dateString = DateTime.Now.ToString();

            List<MatchCache> matchCacheObj = new List<MatchCache>();
            foreach (MatchSimple match in matches)
            {
                matchCacheObj.Add(new MatchCache(dateString, match.Key,
                    match.Alliances.Blue.TeamKeys[0].Substring(3),
                    match.Alliances.Blue.TeamKeys[1].Substring(3),
                    match.Alliances.Blue.TeamKeys[2].Substring(3),
                    match.Alliances.Red.TeamKeys[0].Substring(3),
                    match.Alliances.Red.TeamKeys[1].Substring(3),
                    match.Alliances.Red.TeamKeys[2].Substring(3)));
            }

            int yearIdx = findYearIndex(year);
            int districtIdx = findDistrictIndex(yearIdx, districtKey);
            int eventIdx = findEventIndex(yearIdx, districtIdx, eventKey);

            if (eventIdx >= 0)
            {
                EventCache eventItem = tbaData.years[yearIdx].districts[districtIdx].events[eventIdx]; // = new EventCache(dateString, eventKey, eventName, matchCacheObj);
                eventItem.lastUpdateTime = dateString;
                eventItem.eventKey = eventKey;
                eventItem.eventName = eventName;
                eventItem.matchCaches = matchCacheObj;
            }
            else
                tbaData.years[yearIdx].districts[districtIdx].events.Add(new EventCache(dateString, eventKey, eventName, matchCacheObj));
        }

        public bool isRewriteRequired(string lastUpdateTime, int year, int itemCount)
        {
            TimeSpan timeFromLastUpdate = DateTime.Now.Subtract(DateTime.Parse(lastUpdateTime));

            return timeFromLastUpdate > TimeSpan.FromDays(10) || (DateTime.Now.Year == year && timeFromLastUpdate > TimeSpan.FromHours(4)) || itemCount == 0;
        }

        public void writeOutCache()
        {
            // Write out the changes that were made
            File.WriteAllText(tbaDataCachePath, JsonConvert.SerializeObject(tbaData, Formatting.Indented));
        }

        int findYearIndex(int year)
        {
            for(int i = 0; i < tbaData.years.Count; i++)
            {
                if (tbaData.years[i].year == year)
                {
                    return i;
                }
            }

            return -1;
        }

        int findDistrictIndex(int yearIdx, string districtKey)
        {
            for (int i = 0; i < tbaData.years[yearIdx].districts.Count; i++)
            {
                if (tbaData.years[yearIdx].districts[i].districtKey.Equals(districtKey))
                {
                    return i;
                }
            }

            return -1;
        }

        int findEventIndex(int yearIdx, int districtIdx, string eventKey)
        {
            for (int i = 0; i < tbaData.years[yearIdx].districts[districtIdx].events.Count; i++)
            {
                if (tbaData.years[yearIdx].districts[districtIdx].events[i].eventKey.Equals(eventKey))
                {
                    return i;
                }
            }

            return -1;
        }

        private void TBABrowserOKButton_Click(object sender, RoutedEventArgs e)
        {
            if(TBATreeView.SelectedItem == null)
            {
                this.Close();
                return;
            }

            TreeHeaderObj selected = ((TreeHeaderObj) ((TreeViewItem) TBATreeView.SelectedItem).Header);
            List<string> returner = new List<string>();

            if (selected.GetId() == 2)
            {
                List<MatchSimple> matches = eventAPIInstance.GetEventMatchesSimple(selected.GetKey());

                foreach (MatchSimple match in matches)
                {
                    returner.Add(match.Key.ToString());
                }

            }
            else if (selected.GetId() == 3)
            {
                returner.Add(selected.GetKey());
            }

            this.callback.Invoke(returner);

            this.Close();
        }

        private void TBABrowserCancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(tbaDataCachePath);

            buildBasicCache(false);
        }
    }
}
