using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleBiz
{
    [DataContract]
    public class BizServerFault
    {
        [DataMember]
        public string Operation { get; set; }
        [DataMember]
        public string Message { get; set; }

        public BizServerFault(string op, string msg)
        {
            this.Operation = op;
            this.Message = msg;
        }
    }
}
