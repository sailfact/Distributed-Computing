using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleData
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
                        UseSynchronizationContext = false)]
    internal class TMDataControllerImpl : ITMDataController
    {
        TMDataControllerImpl()
        {
            Console.WriteLine("New Client has Connected");
        }

        ~TMDataControllerImpl()
        {
            Console.WriteLine("Client Disconnected");
        }

        public int GetNumTilesAcross(int zoom)
        {
            int across, down;

            if (TMDLLWrapper.GetNumTiles(zoom, out across, out down) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetNumTiles'");
            }

            return across;
        }

        public int GetNumTilesDown(int zoom)
        {
            int across, down;
            if (TMDLLWrapper.GetNumTiles(zoom, out across, out down) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetNumTiles'");
            }

            return down;
        }

        public int GetTileHeight()
        {
            int height, width;
            if (TMDLLWrapper.GetTileSize(out width, out height) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetTileSize'");
            }

            return height;
        }

        public int GetTileWidth()
        {
            int height, width;
            if (TMDLLWrapper.GetTileSize(out width, out height) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetTileSize'");
            }

            return width;
        }

        public byte[] LoadTile(int zoom, int x, int y)
        {
            int height;
            int width;
            byte[] array = null;
            int size;

            if (TMDLLWrapper.GetTileSize(out width, out height) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetTileSize'");
            }
            size = width * height * 3;
            array = new byte[size];

            if (TMDLLWrapper.GetTileImageAsRawJPG(zoom, x, y, array, size, ref size) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetTileImageAsRawJPG'");
            }

            return array;
        }
    }
}