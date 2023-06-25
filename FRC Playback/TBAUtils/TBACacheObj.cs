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
        public string lastUpdatedTime { get; set; }
        public List<YearCache> years { get; set; }

        public TBACache(string lastUpdatedTime, List<YearCache> years)
        {
            this.lastUpdatedTime = lastUpdatedTime;
            this.years = years;
        }
    }

    public class YearCache {
        public int year { get; set; }
        public List<DistrictCache> districts { get; set; }

        public YearCache(int year, List<DistrictCache> districts)
        {
            this.year = year;
            this.districts = districts;
        }
    }
    public class DistrictCache
    {
        public string districtKey { get; set; }
        public string districtName { get; set; }
        public EventCache[] events { get; set; }

        public DistrictCache(string districtKey, string districtName, EventCache[] events)
        {
            this.districtKey = districtKey;
            this.districtName = districtName;
            this.events = events;
        }
    }

    public class EventCache
    {
        public string eventKey { get; set; }
        public MatchCache[] matchCaches { get; set; }
    }

    public class MatchCache
    {
        public string matchKey { get; set; }

        public string blue1 { get; set; }
        public string blue2 { get; set; }
        public string blue3 { get; set; }

        public string red1 { get; set; }
        public string red2 { get; set; }
        public string red3 { get; set; }
    }
}
