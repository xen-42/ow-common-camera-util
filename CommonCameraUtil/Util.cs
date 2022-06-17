using OWML.Common;
using System.Linq;
using UnityEngine;

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

        // Lifted from NH
        public static GameObject Find(string path)
        {
            var go = GameObject.Find(path);

            var names = path.Split(new char[] { '\\', '/' });
            if (go == null)
            {

                // Get the root object and hope its the right one
                var root = GameObject.Find(names[0]);
                if (root == null) root = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().Where(x => x.name.Equals(names[0])).FirstOrDefault();

                var t = root?.transform;
                if (t != null)
                {
                    for (int i = 1; i < names.Length; i++)
                    {
                        var child = t.transform.Find(names[i]);

                        if (child == null)
                        {
                            foreach (Transform c in t.GetComponentsInChildren<Transform>(true))
                            {
                                if (c.name.Equals(names[i]))
                                {
                                    child = c;
                                    break;
                                }
                            }
                        }

                        if (child == null)
                        {
                            t = null;
                            break;
                        }

                        t = child;
                    }
                }

                go = t?.gameObject;
            }

            if (go == null) WriteError($"Could not find gameobject [{path}]");

            return go;
        }
    }
}
