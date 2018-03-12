using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleData
{
    class TMDLLWrapper
    {
        [DllImport("TrueMarbleDLL.dll")]
        public static extern int GetTileSize(out int width, out int height);

        [DllImport("TrueMarbleDLL.dll")]
        public static extern int GetNumTiles(int zoomLevel, out int numTilesX, out int numTilesY);

        [DllImport("TrueMarbleDLL.dll")]
        public static extern int GetTileImageAsRawJPG(int zoomLevel, int tileX, int tileY, byte[] imageBuf, int bufSize, ref int jpgSize);

        [DllImport("TrueMarbleDLL.dll")]
        public static extern int GetTileImageAsRawJPG_dbg(int zoomLevel, int tileX, int tileY, byte[] imageBuf, int bufSize, ref int jpgSize, ref string sFile, ref string sErr);
    }
}
