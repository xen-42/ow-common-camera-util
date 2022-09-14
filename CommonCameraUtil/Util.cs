using OWML.Common;
using System.Linq;
using UnityEngine;

namespace CommonCameraUtil;

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

    public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
    }

    public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 rhs = point - lineStart;
        Vector3 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector3 lhs = vector2;
        if (magnitude > 1E-06f)
        {
            lhs = (Vector3)(lhs / magnitude);
        }
        float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
        return (lineStart + ((Vector3)(lhs * num2)));
    }
}
