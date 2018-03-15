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
        int GetTileWidth();

        [OperationContract]
        int GetTileHeight();

        [OperationContract]
        int GetNumTilesAcross(int zoom);

        [OperationContract]
        int GetNumTilesDown(int zoom);

        [OperationContract]
        byte[] LoadTile(int zoom, int x, int y);
    }
}
