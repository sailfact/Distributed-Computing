using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleBiz
{
    [ServiceContract(CallbackContract = typeof(ITMBizControllerCallback))]
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

        [OperationContract]
        void VerifyTilesAsync();

        [OperationContract]
        void VerifyTiles_OnComplete(IAsyncResult res);
    }

    [ServiceContract]
    public interface ITMBizControllerCallback
    {
        void OnVerificationComplete(bool result);
    }
}
