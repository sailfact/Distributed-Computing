﻿using System;
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
            if (TMDLLWrapper.GetNumTiles(zoom, out int across, out int down) != 1)
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
        {;
            if (TMDLLWrapper.GetNumTiles(zoom, out int across, out int down) != 1)
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
            if (TMDLLWrapper.GetTileSize(out int width, out int height) != 1)
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
            if (TMDLLWrapper.GetTileSize(out int width, out int height) != 1)
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
            byte[] array = null;
            int height = 0;
            int width = 0;
            try
            {
                height = GetTileHeight();    // get height and width
                width = GetTileWidth();
            }
            catch (FaultException e1)
            {
                Console.WriteLine(e1.Message);
                Environment.Exit(1);
            }

            int size = width * height * 3;  // determine size
            array = new byte[size];     // allocate buffer

            // check if coordinates are valid
            if (CheckCoordinates(zoom, x, y))
            {
                if (TMDLLWrapper.GetTileImageAsRawJPG(zoom, x, y, array, size, ref size) != 1)
                {
                    throw new FaultException("Error: retrieving Raw JPG");
                }
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
            return (x < GetNumTilesAcross(zoom)) && (y < GetNumTilesDown(zoom));
        }
    }
}