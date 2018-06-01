using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ServiceModel;

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
        private Dictionary<string, TileCoord>[] m_landMarks;
        /// <summary>
        /// Constructor
        /// </summary>
        public TMDataControllerImpl()
        {
            m_landMarks = new Dictionary<string, TileCoord>[6];
            SetTable();
            Console.WriteLine("Server Created");
        }

        /// <summary>
        /// SetTable
        /// 
        /// </summary>
        private void SetTable()
        {
            int pX = 69, pY = 28;
            int aX = 74, aY = 29;
            int mX = 76, mY = 30;
            TileCoord coord;

            for (int i = 0; i < 6; i++)
            {
                coord.x = pX;
                coord.y = pY;
                m_landMarks[i].Add("Perth", coord);
                coord.x = aX;
                coord.y = aY;
                m_landMarks[i].Add("Adelaide", coord);
                coord.x = mX;
                coord.y = mY;
                m_landMarks[i].Add("Melbourne", coord);
                aX = aX / 2;
                aY = aY / 2;
                pX = pX / 2;
                pY = pY / 2;
                mX = mX / 2;
                mY = mY / 2;
            }
        }

        /// <summary>
        /// GetLandmarkCoords
        /// gets the dictionary for the current zoom level
        /// returns the x and y coords for the landmark
        /// </summary>
        /// <param name="landmarkName"></param>
        /// <param name="zoom"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GetLandmarkCoords(string landmarkName, int zoom, out int x, out int y)
        {
            x = m_landMarks[zoom][landmarkName].x;
            y = m_landMarks[zoom][landmarkName].y;
        }

        /// <summary>
        /// GetNumTilesAcross
        /// returns number of tiles across depending on the level of zoom
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns>
        /// returns across or -1 if error
        /// aswell as a error message via reference
        /// </returns>
        public int GetNumTilesAcross(int zoom)
        {
            try
            {
                if (TMDLLWrapper.GetNumTiles(zoom, out int across, out int down) != 1)
                {
                    throw new FaultException<DataServerFault>(new DataServerFault("Error in Function 'DGDataController.GetNumTilesAcross'", "Invalid Zoom Parameter"));
                }
                return across;
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine(e.Message);
                throw new FaultException<DataServerFault>(new DataServerFault("Error in Function 'DGDataController.GetNumTilesAcross'", "TrueMarbleDLL.dll is missing"));
            }
        }
        /// <summary>
        /// GetNumTilesDown
        /// returns number of tiles down depending on the level of zoom
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns>
        /// returns down or -1 if error
        /// aswell as a error message via reference
        /// </returns>
        public int GetNumTilesDown(int zoom)
        {
            try
            {
                if (TMDLLWrapper.GetNumTiles(zoom, out int across, out int down) != 1)
                {
                    throw new FaultException<DataServerFault>(new DataServerFault("Error in Function 'DGDataController.GetNumTilesDown'", "Invalid Zoom Parameter"));
                }
                return down;
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine(e.Message);
                throw new FaultException<DataServerFault>(new DataServerFault("Error in Function 'DGDataController.GetNumTilesDown'", "TrueMarbleDLL.dll is missing"));
            }
        }
        /// <summary>
        /// GetTileHeight
        /// returns tile height
        /// always 256 
        /// </summary>
        /// <returns>
        /// returns height or -1 if error
        /// aswell as a error message via reference
        /// </returns>
        public int GetTileHeight()
        {
            try
            {
                if (TMDLLWrapper.GetTileSize(out int width, out int height) != 1)
                {
                    throw new FaultException<DataServerFault>(new DataServerFault("Error in Function 'DGDataController.GetTileHeight'", "Returned error"));
                }
                return height;
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine(e.Message);
                throw new FaultException<DataServerFault>(new DataServerFault("Error in Function 'DGDataController.GetTileHeight'", "TrueMarbleDLL.dll is missing"));
            }
        }
        /// <summary>
        /// GetTileWidth
        /// returns tile width 
        /// always 256
        /// </summary>
        /// <returns>
        /// returns width or -1 or error
        /// aswell as a error message via reference
        /// </return>
        public int GetTileWidth()
        {
            try
            {
                if (TMDLLWrapper.GetTileSize(out int width, out int height) != 1)
                {
                    throw new FaultException<DataServerFault>(new DataServerFault("Error in Function 'DGDataController.GetTileWidth'", "Returned error"));
                }
                return width;
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine(e.Message);
                throw new FaultException<DataServerFault>(new DataServerFault("Error in Function 'DGDataController.GetTileWidth'", "TrueMarbleDLL.dll is missing"));
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
        /// aswell as a error message via reference
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] LoadTile(int zoom, int x, int y)
        {
            int size;
            byte[] array = null;
            try
            {
                if (TMDLLWrapper.GetTileSize(out int width, out int height) != 1)
                    throw new FaultException<DataServerFault>(new DataServerFault("Error in Function 'DGDataController.LoadTile'", "Returned error"));

                size = width * height * 3;  // determine size
                array = new byte[size];     // allocate buffer

                if (TMDLLWrapper.GetNumTiles(zoom, out int across, out int down) != 1)
                    throw new FaultException<DataServerFault>(new DataServerFault("Error in Function 'DGDataController.LoadTile'", "Invalid Zoom Parameter"));

                if (x < across && y < down)// check if coordinates are valid
                {
                    if (TMDLLWrapper.GetTileImageAsRawJPG(zoom, x, y, array, size, ref size) != 1)
                        throw new FaultException<DataServerFault>(new DataServerFault("Error in Function 'DGDataController.LoadTile'", "Error retrieving tiles"));
                }
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine(e.Message);
                throw new FaultException<DataServerFault>(new DataServerFault("Error in Function 'DGDataController.LoadTile'", "TrueMarbleDLL.dll is missing"));
            }
            return array;   // will be null if failure
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] GetLandMarkList()
        {
            string[] names = new string[3];
            int i = 0;
            foreach (var item in m_landMarks[0])
            {
                names[i] = item.Key;
                i++;
            }

            return names;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    struct TileCoord
    {
        public int x;
        public int y;
    };
}