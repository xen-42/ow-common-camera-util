using UnityEngine;

namespace CommonCameraUtil;

public class OWLayer
{
	public static int Default => LayerMask.NameToLayer(nameof(Default));
	public static int VisibleToProbe => LayerMask.NameToLayer(nameof(VisibleToProbe));
}
