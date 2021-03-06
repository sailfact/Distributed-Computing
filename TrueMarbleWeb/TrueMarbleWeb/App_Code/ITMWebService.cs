﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITMWebService" in both code and config file together.
[ServiceContract]
public interface ITMWebService
{
    [OperationContract]
    int GetNumTilesAcross(int zoom);

    [OperationContract]
    int GetNumTilesDown(int zoom);

    [OperationContract]
    byte[] LoadTile(int zoom, int x, int y);
}
