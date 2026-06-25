using System;

namespace UnityEngine.UI
{
	// Token: 0x02000FAB RID: 4011
	public static class LoopScrollSizeUtils
	{
		// Token: 0x0600B87B RID: 47227 RVA: 0x00541B84 File Offset: 0x0053FD84
		public static float GetPreferredHeight(RectTransform item)
		{
			ILayoutElement minLayoutElement;
			float minHeight = LayoutUtility.GetLayoutProperty(item, (ILayoutElement e) => e.minHeight, 0f, out minLayoutElement);
			ILayoutElement preferredLayoutElement;
			float preferredHeight = LayoutUtility.GetLayoutProperty(item, (ILayoutElement e) => e.preferredHeight, 0f, out preferredLayoutElement);
			float result = Mathf.Max(minHeight, preferredHeight);
			bool flag = preferredLayoutElement == null && minLayoutElement == null;
			if (flag)
			{
				result = item.rect.height;
			}
			bool flag2 = result <= 0f;
			if (flag2)
			{
				Debug.LogWarning("[GetPreferredHeight] 0 height for " + item.name);
			}
			return result;
		}

		// Token: 0x0600B87C RID: 47228 RVA: 0x00541C48 File Offset: 0x0053FE48
		public static float GetPreferredWidth(RectTransform item)
		{
			ILayoutElement minLayoutElement;
			float minWidth = LayoutUtility.GetLayoutProperty(item, (ILayoutElement e) => e.minWidth, 0f, out minLayoutElement);
			ILayoutElement preferredLayoutElement;
			float preferredWidth = LayoutUtility.GetLayoutProperty(item, (ILayoutElement e) => e.preferredWidth, 0f, out preferredLayoutElement);
			float result = Mathf.Max(minWidth, preferredWidth);
			bool flag = preferredLayoutElement == null && minLayoutElement == null;
			if (flag)
			{
				result = item.rect.width;
			}
			bool flag2 = result <= 0f;
			if (flag2)
			{
				Debug.LogWarning("[GetPreferredWidth] 0 width for " + item.name);
			}
			return result;
		}
	}
}
