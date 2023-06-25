using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FRC_Playback.TBAUtils
{
    public class TBACache
    {
        public string lastUpdateTime { get; set; }
        public List<YearCache> years { get; set; }

        public TBACache(string lastUpdatedTime, List<YearCache> years)
        {
            this.lastUpdateTime = lastUpdatedTime;
            this.years = years;
        }
    }

    public class YearCache {

        public string lastUpdateTime { get; set; }
        public int year { get; set; }
        public List<DistrictCache> districts { get; set; }

        public YearCache(string lastUpdateTime, int year, List<DistrictCache> districts)
        {
            this.lastUpdateTime = lastUpdateTime;
            this.year = year;
            this.districts = districts;
        }
    }
    public class DistrictCache
    {

        public string lastUpdateTime { get; set; }
        public string districtKey { get; set; }
        public string districtName { get; set; }
        public List<EventCache> events { get; set; }

        public DistrictCache(string lastUpdateTime, string districtKey, string districtName, List<EventCache> events)
        {
            this.lastUpdateTime = lastUpdateTime;
            this.districtKey = districtKey;
            this.districtName = districtName;
            this.events = events;
        }
    }

    public class EventCache
    {

        public string lastUpdateTime { get; set; }
        public string eventKey { get; set; }
        public string eventName { get; set; } 
        public List<MatchCache> matchCaches { get; set; }

        public EventCache(string lastUpdateTime, string eventKey, string eventName, List<MatchCache> matchCaches)
        {
            this.lastUpdateTime = lastUpdateTime;
            this.eventKey = eventKey;
            this.eventName = eventName;
            this.matchCaches = matchCaches;
        }
    }

    public class MatchCache
    {
        public string lastUpdateTime { get; set; }

        public string matchKey { get; set; }

        public string blue1 { get; set; }
        public string blue2 { get; set; }
        public string blue3 { get; set; }

        public string red1 { get; set; }
        public string red2 { get; set; }
        public string red3 { get; set; }

        public MatchCache(string lastUpdateTime, string matchKey, string blue1, string blue2, string blue3, string red1, string red2, string red3)
        {
            this.lastUpdateTime = lastUpdateTime;
            this.matchKey = matchKey;
            this.blue1 = blue1;
            this.blue2 = blue2;
            this.blue3 = blue3;
            this.red1 = red1;
            this.red2 = red2;
            this.red3 = red3;
        }
    }
}
