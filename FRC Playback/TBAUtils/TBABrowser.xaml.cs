using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        TBACache tbaData;
        string tbaDataCachePath;

        EventApi eventAPIInstance;
        DistrictApi districtAPIInstance;

        public TBABrowser()
        {
            InitializeComponent();

            tbaDataCachePath = System.IO.Path.GetTempPath() + "FRCPlayback_TBADataCache.json";
            CachePath.Text = tbaDataCachePath;

            FileStream fileStream;
            if (!File.Exists(tbaDataCachePath))
                fileStream = File.Create(tbaDataCachePath);
            else
                fileStream = new FileStream(tbaDataCachePath, FileMode.Open, FileAccess.ReadWrite);

            Configuration.Default.BasePath = "https://www.thebluealliance.com/api/v3";
            Configuration.Default.ApiKey.Add("X-TBA-Auth-Key", "Ow8SODFIWtXsxk9KbI0tDPooPibzYcyzhSiJ9saUWQXGIf5ENQ8pwnb1Bi2gwxhj");

            eventAPIInstance = new EventApi(Configuration.Default);
            districtAPIInstance = new DistrictApi(Configuration.Default);

            /*
            try
            {
                tbaData = JsonSerializer.Deserialize<TBACache>(fileStream);
                fileStream.Close();
            }
            catch (JsonException e)
            {
                fileStream.Close();
                pushDataInCache();
            }*/

            fileStream.Close();
            buildBasicCacheIndex();

            TBASearchBar.Items.Clear();

            /*

            List<DistrictList> districts = districtAPIInstance.GetDistrictsByYear(DateTime.Now.Year);
            foreach (DistrictList district in districts)
            {
                TreeViewItem districtItem = new TreeViewItem();
                districtItem.Header = district.DisplayName;
                districtItem.AllowDrop = true;
                districtItem.Expanded += new RoutedEventHandler((sender, args) =>
                {
                    List<EventSimple> events = districtAPIInstance.GetDistrictEventsSimple(district.Key);
                    
                    foreach (EventSimple simpleEvent in events)
                    {
                        TreeViewItem eventItem = new TreeViewItem();
                        eventItem.Header = simpleEvent.Name;
                        eventItem.AllowDrop = true;
                        eventItem.Expanded += new RoutedEventHandler((sender, args) =>
                        {
                            List<Match> matches = eventAPIInstance.GetEventMatches(simpleEvent.Key);

                            foreach (Match match in matches)
                            {
                                TreeViewItem matchItem = new TreeViewItem();
                                matchItem.Header = match.Key;
                                matchItem.AllowDrop = true;
                                matchItem.Selected += new RoutedEventHandler((sender, args) =>
                                {
                                    RedAllianceTeam1Number.Text = match.Alliances.Red.TeamKeys[0].Substring(3);
                                    RedAllianceTeam2Number.Text = match.Alliances.Red.TeamKeys[1].Substring(3);
                                    RedAllianceTeam3Number.Text = match.Alliances.Red.TeamKeys[2].Substring(3);

                                    BlueAllianceTeam1Number.Text = match.Alliances.Blue.TeamKeys[0].Substring(3);
                                    BlueAllianceTeam2Number.Text = match.Alliances.Blue.TeamKeys[1].Substring(3);
                                    BlueAllianceTeam3Number.Text = match.Alliances.Blue.TeamKeys[2].Substring(3);

                                    TBASearchBar.Text = match.Key;
                                });

                                eventItem.Items.Add(matchItem);
                            }
                        });

                        districtItem.Items.Add(eventItem);
                    }
                });

                TBATreeView.Items.Add(districtItem);
            } */
        }

        void buildBasicCacheIndex()
        {
            List<DistrictList> districts = districtAPIInstance.GetDistrictsByYear(DateTime.Now.Year);

            List<DistrictCache> districtCacheObj = new List<DistrictCache>();
            foreach (DistrictList district in districts)
            {
                districtCacheObj.Add(new DistrictCache(district.Key, district.DisplayName, new EventCache[] { }));
            }

            tbaData = new TBACache("Now", new List<YearCache>());
            tbaData.years.Add(new YearCache(DateTime.Now.Year, districtCacheObj));

            File.WriteAllText(tbaDataCachePath, JsonConvert.SerializeObject(tbaData, Formatting.Indented));
        }

    }
}
