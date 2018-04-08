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
        int GetNumTilesAcross(int zoom, out int across);

        [OperationContract]
        int GetNumTilesDown(int zoom, out int down);

        [OperationContract]
        byte[] LoadTile(int zoom, int x, int y);

        [OperationContract]
        bool VerifyTiles();

        [OperationContract]
        void VerifyTilesAsync();

        [OperationContract]
        void VerifyTiles_OnComplete(IAsyncResult res);

        [OperationContract]
        void AddHistEntry(int x, int y, int zoom);

        [OperationContract]
        void GetCurrHistEntry(out int x, out int y, out int zoom);

        [OperationContract]
        void HistBack(out int x, out int y, out int zoom);

        [OperationContract]
        void HistForward(out int x, out int y, out int zoom);

        [OperationContract]
        BrowseHistory GetFullHistory();

        [OperationContract]
        void SetFullHistory(BrowseHistory hist);
    }

    [ServiceContract]
    public interface ITMBizControllerCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnVerificationComplete(bool result);
    }
}
