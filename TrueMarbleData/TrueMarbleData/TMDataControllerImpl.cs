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
        /// GetNumTilesAcross
        /// returns number of tiles across depending on the level of zoom
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns>
        /// returns across or -1 if error
        /// </returns>
        public int GetNumTilesAcross(int zoom, out string errorMsg)
        {
            errorMsg = null;
            try
            {
                if (TMDLLWrapper.GetNumTiles(zoom, out int across, out int down) != 1)
                {
                    errorMsg = "Error in DLL Function 'GetNumTiles'";
                    Console.WriteLine(errorMsg);
                    return -1;  // error
                }
                return across;
            }
            catch (DllNotFoundException e)
            {
                errorMsg = "Error: DLL Not found\n";
                Console.WriteLine(errorMsg + e.Message);
                return -1;
            }
            
        }

        /// <summary>
        /// GetNumTilesDown
        /// returns number of tiles down depending on the level of zoom
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns>
        /// returns down or -1 if error
        /// </returns>
        public int GetNumTilesDown(int zoom, out string errorMsg)
        {
            errorMsg = null;
            try
            {
                if (TMDLLWrapper.GetNumTiles(zoom, out int across, out int down) != 1)
                {
                    errorMsg = "Error in DLL Function 'GetNumTiles'";
                    Console.WriteLine(errorMsg);
                    return -1;  // error
                }
                return down;
            }
            catch (DllNotFoundException e)
            {
                errorMsg = "Error: DLL Not found\n";
                Console.WriteLine(errorMsg + e.Message);
                return -1;
            }
        }


        /// <summary>
        /// GetTileHeight
        /// returns tile height
        /// always 256 
        /// </summary>
        /// <returns>
        /// returns height or -1 if error
        /// </returns>
        public int GetTileHeight(out string errorMsg)
        {
            errorMsg = null;
            try
            {
                if (TMDLLWrapper.GetTileSize(out int width, out int height) != 1)
                {
                    Console.WriteLine("Error in DLL Function 'GetTileSize'");
                    return -1;  // error
                }
                return height;
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine("Error in Function GetTileHeight:\n" + e.Message);
                return -1;
            }
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
            try
            {
                if (TMDLLWrapper.GetTileSize(out int width, out int height) != 1)
                {
                    Console.WriteLine("Error in DLL Function 'GetTileSize'");
                    return -1;  // error
                }

                return width;
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine("Error in Function GetTileHeight:\n" + e.Message);
                return -1;
            }
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
        public byte[] LoadTile(int zoom, int x, int y, out string errMsg)
        {
            int size;
            byte[] array = null;
            errMsg = null;
            try
            {
                if (TMDLLWrapper.GetTileSize(out int width, out int height) != 1)      // get height and width
                {
                    errMsg = "Error with DLL function 'GetTileSize'";
                    Console.WriteLine(errMsg);
                    return null;
                }

                size = width * height * 3;  // determine size
                array = new byte[size];     // allocate buffer

                if (TMDLLWrapper.GetNumTiles(zoom, out int across, out int down) != 1)
                {
                    errMsg = "Error in DLL Function 'GetNumTiles'";
                    Console.WriteLine(errMsg);
                    return null;
                }

                if ((x < across && y < down))// check if coordinates are valid
                {
                    if (TMDLLWrapper.GetTileImageAsRawJPG(zoom, x, y, array, size, ref size) != 1)
                    {
                        errMsg = "Error in DLL Function 'GetTileImageAsRawJPG'";
                        Console.WriteLine(errMsg);
                        return null;
                    }
                }
            }
            catch (DllNotFoundException e)
            {
                errMsg = "Error in Function 'LoadTile'";
                Console.WriteLine(errMsg + e.Message);
            }

            return array;   // will be null if failure
        }
    }
}