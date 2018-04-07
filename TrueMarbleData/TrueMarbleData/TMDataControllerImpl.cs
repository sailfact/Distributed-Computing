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
        /// 1 for success
        /// 0 for failure
        /// </returns>
        public int GetNumTilesAcross(int zoom, out int across)
        {
            if (TMDLLWrapper.GetNumTiles(zoom, out across, out int down) != 1)
            {
                Console.WriteLine("Error in DLL Function 'GetNumTiles'");
                return 0;
            }

            return 1;
        }

        /// <summary>
        /// GetNumTilesDown
        /// returns number of tiles down depending on the level of zoom
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public int GetNumTilesDown(int zoom, out int down)
        {
            if (TMDLLWrapper.GetNumTiles(zoom, out int across, out down) != 1)
            {
                Console.WriteLine("Error in DLL Function 'GetNumTiles'");
                return 0;
            }

            return 1;
        }

        /// <summary>
        /// GetTileHeight
        /// returns tile height
        /// always 256 
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public int GetTileHeight(out int width)
        {
            if (TMDLLWrapper.GetTileSize(out width, out int height) != 1)
            {
                Console.WriteLine("Error in DLL Function 'GetTileSize'");
                return 0;
            }

            return 1;
        }

        /// <summary>
        /// GetTileWidth
        /// returns tile width 
        /// always 256
        /// </summary>
        /// <returns></returns>
        public int GetTileWidth(out int width)
        {
            if (TMDLLWrapper.GetTileSize(out width, out int height) != 1)
            {
                Console.WriteLine("Error in DLL Function 'GetTileSize'");
                return 0;
            }

            return 1;
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
        /// 1 for success 
        /// 0 for failure
        /// Byte array containing raw JPG
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int LoadTile(int zoom, int x, int y, byte[] array)
        {
            int size;

            if (TMDLLWrapper.GetTileSize(out int width, out int height) != 1)      // get height and width
            {
                Console.WriteLine("Error with DLL function 'GetTileSize'");
                return 0;
            }

            size = width * height * 3;  // determine size
            array = new byte[size];     // allocate buffer

            
            if (TMDLLWrapper.GetNumTiles(zoom, out int across, out int down) != 1)
            {
                Console.WriteLine("Error in DLL Function 'GetNumTiles'");
                return 0;
            }

            // check if coordinates are valid
            if (!(x < across && y < down))
            {
                return 0;
            }

            if (TMDLLWrapper.GetTileImageAsRawJPG(zoom, x, y, array, size, ref size) != 1)
            {
                Console.WriteLine("Error in DLL Function 'GetTileImageAsRawJPG'");
                return 0;
            }

            return 1;   // success
        }
    }
}