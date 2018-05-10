using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TMWebService" in code, svc and config file together.
public class TMWebService : ITMWebService
{
    public int GetNumTilesAcross(int zoom)
    {
        return 0;
    }

    public int GetNumTilesDown(int zoom)
    {
        return 0;
    }
}
