using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleData
{
    /// <summary>
    /// server object that inplements the interface for the dll functions
    /// </summary>
    [ServiceBehavior(InstanceContextMode =InstanceContextMode.Single
        ,ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false)]
    internal class TMDataControllerImpl : ITMDataController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TMDataControllerImpl()
        {
            Console.WriteLine("Server Created");
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
        /// <returns>
        /// returns across or -1 if error
        /// </returns>
        public int GetNumTilesAcross(int zoom)
        {
            if (TMDLLWrapper.GetNumTiles(zoom, out int across, out int down) != 1)
            {
                Console.WriteLine("Error in DLL Function 'GetNumTiles'");
                return -1;
            }

            return across;
        }

        /// <summary>
        /// GetNumTilesDown
        /// returns number of tiles down depending on the level of zoom
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns>
        /// returns down or -1 if error
        /// </returns>
        public int GetNumTilesDown(int zoom)
        {
            if (TMDLLWrapper.GetNumTiles(zoom, out int across, out int down) != 1)
            {
                Console.WriteLine("Error in DLL Function 'GetNumTiles'");
                return -1;
            }

            return down;
        }

        /// <summary>
        /// GetTileHeight
        /// returns tile height
        /// always 256 
        /// </summary>
        /// <returns>
        /// returns height or -1 if error
        /// </returns>
        public int GetTileHeight()
        {
            if (TMDLLWrapper.GetTileSize(out int width, out int height) != 1)
            {
                Console.WriteLine("Error in DLL Function 'GetTileSize'");
                return -1;  // error
            }

            return height;
        }

        /// <summary>
        /// GetTileWidth
        /// returns tile width 
        /// always 256
        /// </summary>
        /// <returns>
        /// returns width or -1 or error
        /// </return>
        public int GetTileWidth()
        {
            if (TMDLLWrapper.GetTileSize(out int width, out int height) != 1)
            {
                Console.WriteLine("Error in DLL Function 'GetTileSize'");
                return -1;
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
        /// <returns>
        /// returns byte array of raw JPG or null if error
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] LoadTile(int zoom, int x, int y)
        {
            int size;
            byte[] array;
            if (TMDLLWrapper.GetTileSize(out int width, out int height) != 1)      // get height and width
            {
                Console.WriteLine("Error with DLL function 'GetTileSize'");
            }

            size = width * height * 3;  // determine size
            array = new byte[size];     // allocate buffer

            
            if (TMDLLWrapper.GetNumTiles(zoom, out int across, out int down) != 1)
            {
                Console.WriteLine("Error in DLL Function 'GetNumTiles'");
                return null;
            }

            // check if coordinates are valid
            if (!(x < across && y < down))
            {
                return null;
            }

            if (TMDLLWrapper.GetTileImageAsRawJPG(zoom, x, y, array, size, ref size) != 1)
            {
                Console.WriteLine("Error in DLL Function 'GetTileImageAsRawJPG'");
                return null;
            }

            return array;   // success
        }
    }
}