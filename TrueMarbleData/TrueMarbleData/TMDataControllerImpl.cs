using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleData
{
    // server object that inplements the interface for the dll functions
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
                        UseSynchronizationContext = false)]
    internal class TMDataControllerImpl : ITMDataController
    {
        // Constructor
        TMDataControllerImpl()
        {
            Console.WriteLine("New Client has Connected");
        }

        //Deconstructor
        ~TMDataControllerImpl()
        {
            Console.WriteLine("Client Disconnected");
        }

        // GetNumTilesAcross
        // returns number of tiles across depending on the level of zoom
        public int GetNumTilesAcross(int zoom)
        {
            int across, down;

            if (TMDLLWrapper.GetNumTiles(zoom, out across, out down) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetNumTiles'");
            }

            return across;
        }

        // GetNumTilesDown
        // returns number of tiles down depending on the level of zoom
        public int GetNumTilesDown(int zoom)
        {
            int across, down;
            if (TMDLLWrapper.GetNumTiles(zoom, out across, out down) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetNumTiles'");
            }

            return down;
        }

        // GetTileHeight
        // returns tile height
        // always 256 
        public int GetTileHeight()
        {
            int height, width;
            if (TMDLLWrapper.GetTileSize(out width, out height) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetTileSize'");
            }

            return height;
        }

        // GetTileWidth
        // returns tile width 
        // always 256
        public int GetTileWidth()
        {
            int height, width;
            if (TMDLLWrapper.GetTileSize(out width, out height) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetTileSize'");
            }

            return width;
        }


        // LoadTile
        // takes zoom, x, and y
        // returns byte array containing the jpg
        public byte[] LoadTile(int zoom, int x, int y)
        {
            int height;
            int width;
            byte[] array = null;
            int size;

            if (TMDLLWrapper.GetTileSize(out width, out height) != 1)      // get height and width
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetTileSize'");
            }

            size = width * height * 3;  // determine size
            array = new byte[size];     // allocate buffer

            // check if coordinates are valid
            if (!CheckCoordinates(zoom, x, y))
            {
                throw new FaultException("Error in function LoadTiles 'Coordinates Are Not Valid");
            }

            if (TMDLLWrapper.GetTileImageAsRawJPG(zoom, x, y, array, size, ref size) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetTileImageAsRawJPG'");
            }

            return array;
        }

        private bool CheckCoordinates(int zoom, int x, int y)
        {
            return (x < GetNumTilesAcross(zoom))&&(y < GetNumTilesDown(zoom) );
        }
    }
}