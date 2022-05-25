using OWML.Common;

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
