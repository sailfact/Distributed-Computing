using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleBiz
{
    /// <summary>
    /// HistEntry
    /// nested class for BrowseHistory
    /// stores infomation of a tile
    /// zoom,x,y
    /// </summary>
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
    /// <summary>
    /// BrowseHistory
    /// stores a list of history entries for all the tiles viewed by the client
    /// </summary>
    [DataContract]
    public class BrowseHistory
    {
        [DataMember]
        public List<HistEntry> History { get; set; }
        [DataMember]
        public int CurEntryIdx { get; set; }
        /// <summary>
        /// Constructor for BrowseHistory
        /// creates new history list sets current index to -1
        /// </summary>
        public BrowseHistory()
        {
            History = new List<HistEntry>();
            CurEntryIdx = -1;
        }
        /// <summary>
        /// AddHistEntry
        /// adds new entry to history list
        /// if there are history entries infront of the current
        /// they are removed
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
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
        /// <summary>
        /// GetCurrHist
        /// </summary>
        /// <returns>history entry at the current index</returns>
        public HistEntry GetCurrHist()
        {
            return History[CurEntryIdx];
        }
        /// <summary>
        /// GetHistBack
        /// </summary>
        /// <returns>previous history entry if it exists</returns>
        public HistEntry GetHistBack()
        {
            if (CurEntryIdx > 0)
            {
                CurEntryIdx--;
            }
            return History[CurEntryIdx];
        }
        /// <summary>
        /// GetHistForward
        /// </summary>
        /// <returns>next history entry if it exists</returns>
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
