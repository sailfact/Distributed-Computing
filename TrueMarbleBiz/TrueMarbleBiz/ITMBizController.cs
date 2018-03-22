using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleBiz
{
    [ServiceContract]
    public interface ITMBizController
    {
        [OperationContract]
        int GetNumTilesAcross(int zoom);

        [OperationContract]
        int GetNumTilesDown(int zoom);

        [OperationContract]
        byte[] LoadTile(int zoom, int x, int y);

        [OperationContract]
        bool VerifyTiles();

    }
}
