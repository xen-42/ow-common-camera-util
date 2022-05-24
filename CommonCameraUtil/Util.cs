using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCameraUtil
{
    public static class Util
    {
        public static void WriteError(string msg)
        {
            CommonCameraUtil.Instance.ModHelper.Console.WriteLine(msg, MessageType.Error);
        }

        public static void WriteWarning(string msg)
        {
            CommonCameraUtil.Instance.ModHelper.Console.WriteLine(msg, MessageType.Warning);
        }

        public static void Write(string msg)
        {
            CommonCameraUtil.Instance.ModHelper.Console.WriteLine(msg, MessageType.Info);
        }
    }
}
