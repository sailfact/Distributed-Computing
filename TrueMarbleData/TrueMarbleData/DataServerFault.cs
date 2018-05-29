using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleData
{
    [DataContract]
    public class DataServerFault
    {
        [DataMember]
        public string Operation { get; set; }
        [DataMember]
        public string Message { get; set; }

        public DataServerFault(string op, string msg)
        {
            this.Operation = op;
            this.Message = msg;
        }
    }
}
