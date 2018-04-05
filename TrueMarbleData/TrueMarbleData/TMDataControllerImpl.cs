using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleData
{
    /// <summary>
    /// server object that inplements the interface for the dll functions
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
                        UseSynchronizationContext = false)]
    internal class TMDataControllerImpl : ITMDataController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        TMDataControllerImpl()
        {
            Console.WriteLine("New Client has Connected");
        }

        /// <summary>
        /// Deconstructor
        /// </summary>
        ~TMDataControllerImpl()
        {
            Console.WriteLine("Client Disconnected");
        }

        /// <summary>
        /// GetNumTilesAcross
        /// returns number of tiles across depending on the level of zoom
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public int GetNumTilesAcross(int zoom)
        {
            int across, down;

            if (TMDLLWrapper.GetNumTiles(zoom, out across, out down) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetNumTiles'");
            }

            return across;
        }

        /// <summary>
        /// GetNumTilesDown
        /// returns number of tiles down depending on the level of zoom
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public int GetNumTilesDown(int zoom)
        {
            int across, down;
            if (TMDLLWrapper.GetNumTiles(zoom, out across, out down) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetNumTiles'");
            }

            return down;
        }

        /// <summary>
        /// GetTileHeight
        /// returns tile height
        /// always 256 
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public int GetTileHeight()
        {
            int height, width;
            if (TMDLLWrapper.GetTileSize(out width, out height) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetTileSize'");
            }

            return height;
        }

        /// <summary>
        /// GetTileWidth
        /// returns tile width 
        /// always 256
        /// </summary>
        /// <returns></returns>
        public int GetTileWidth()
        {
            int height, width;
            if (TMDLLWrapper.GetTileSize(out width, out height) != 1)
            {
                throw new FaultException("Error in function 'TMDLLWrapper.GetTileSize'");
            }

            return width;
        }


        /// <summary>
        /// LoadTile
        /// takes zoom, x, and y
        /// returns byte array containing the jpg
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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

        /// <summary>
        /// CheckCoordinates
        /// checks whether coordinates are valid
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool CheckCoordinates(int zoom, int x, int y)
        {
            return (x < GetNumTilesAcross(zoom))&&(y < GetNumTilesDown(zoom) );
        }
    }
}