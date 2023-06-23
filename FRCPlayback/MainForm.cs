using MaterialSkin;
using MaterialSkin.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Media;
using TBAAPI.V3Client.Api;
using TBAAPI.V3Client.Client;
using TBAAPI.V3Client.Model;

namespace FRCPlayback
{
    public partial class FRCPlayback : MaterialForm
    {
        Collection<string> results = new Collection<string>();


        Collection<string> events = new Collection<string>();


        public FRCPlayback()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            Configuration.Default.BasePath = "https://www.thebluealliance.com/api/v3";
            Configuration.Default.ApiKey.Add("X-TBA-Auth-Key", "Ow8SODFIWtXsxk9KbI0tDPooPibzYcyzhSiJ9saUWQXGIf5ENQ8pwnb1Bi2gwxhj");

            var apiInstance = new EventApi(Configuration.Default);
            List<EventSimple> eventsSimple = apiInstance.GetEventsByYearSimple(DateTime.Now.Year);
            foreach (EventSimple simpleEvent in eventsSimple)
            {
                events.Add(simpleEvent.Key);
            }

        }

        public void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void MatchInput_TextChanged(object sender, EventArgs e)
        {
            results.Clear();
            SuggestionsList.Clear();
            
            foreach (string eventKey in events)
            {
                if (eventKey.ToLower().Contains(MatchInput.Text.ToLower()))
                {
                    results.Add(eventKey);

                }
            }
            SuggestionsList.AddItems(results.ToArray<string>());
        }

        private void SuggestionsButton_Click(object sender, EventArgs e)
        {
            SuggestionsList.Clear();
            SuggestionsList.AddItems(events.ToArray());
            Debug.WriteLine("Suggested: " + events);
        }

        private void MatchInput_Enter(object sender, EventArgs e)
        {
            SuggestionsList.Visible = true;
        }

        private void MatchInput_Leave(object sender, EventArgs e)
        {
            //SuggestionsList.Visible = false;
        }

        private void FRCPlayback_MouseClick(object sender, MouseEventArgs e)
        {

        }
    }
}