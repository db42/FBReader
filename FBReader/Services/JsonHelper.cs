using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace FBReader.Services
{
    public class JsonHelper
    {
        public object ParseJson(byte[] jsonResponse, Type returnObjectType)
        {

            MemoryStream stream = new MemoryStream(jsonResponse);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(returnObjectType);

            Object returnObject;
            returnObject = ser.ReadObject(stream);

            return returnObject;
        }
    }
}
