using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Shares
{
    public static class IdUtility
    {
        public static long GetNewID()
        {
            long id = long.Parse(DateUtility.GetCurrentDateTimeAsString("yyyyMMddHHmmssffff")) - 200000000000000000;

            return id;
        }
    }
}
