using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleData
{
    // Remote interface for DLL functions
    [ServiceContract]
    public interface ITMDataController
    {
        [OperationContract]
        int GetTileWidth(out int width);

        [OperationContract]
        int GetTileHeight(out int height);

        [OperationContract]
        int GetNumTilesAcross(int zoom, out int across);

        [OperationContract]
        int GetNumTilesDown(int zoom, out int down);

        [OperationContract]
        int LoadTile(int zoom, int x, int y, byte[] array);
    }
}
