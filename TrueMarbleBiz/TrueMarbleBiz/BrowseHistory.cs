using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleBiz
{
    [DataContract]
    public class HistEntry
    {
        [DataMember]
        public int X { get; set; }

        [DataMember]
        public int Y { get; set; }

        [DataMember]
        public int Zoom { get; set; }
        public HistEntry(int x, int y, int zoom)
        {
            this.X = x;
            this.Y = y;
            this.Zoom = zoom;
        }
    }

    [DataContract]
    public class BrowseHistory
    {
        [DataMember]
        public List<HistEntry> History { get; set; }
        [DataMember]
        public int CurEntryIdx { get; set; }

        public BrowseHistory()
        {
            History = new List<HistEntry>();
            CurEntryIdx = -1;
        }

        public void AddHistEntry(int x ,int y, int zoom)
        {
            History.Add(new HistEntry(x, y, zoom));
            CurEntryIdx++;
        }

        public HistEntry GetCurrHist()
        {
            return History[CurEntryIdx];
        }
    }


    
}
