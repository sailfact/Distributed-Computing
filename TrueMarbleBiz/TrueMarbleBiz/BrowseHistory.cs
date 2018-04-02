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

        public void AddHistEntry(int x, int y, int zoom)
        {
            if (History.Count() < CurEntryIdx)
            {
                // reallocate list with up until current entry
                History.RemoveRange(CurEntryIdx+1, History.Count);
            }
            CurEntryIdx++;
            History.Insert(CurEntryIdx ,new HistEntry(x, y, zoom));
            
        }

        public HistEntry GetCurrHist()
        {
            return History[CurEntryIdx];
        }

        public HistEntry GetHistBack()
        {
            if (CurEntryIdx > 0)
            {
                CurEntryIdx--;
            }

            return History[CurEntryIdx];
        }

        public HistEntry GetHistForward()
        {
            if (CurEntryIdx + 1 < History.Count)   // Make sure there is a entry ahead
            {
                CurEntryIdx++;  // increment current idx
            }

            return History[CurEntryIdx];
        }
    }
}
