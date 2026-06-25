using System;
using GameData.Utilities;
using UnityEngine;

namespace FrameWork.Tools
{
	// Token: 0x02001032 RID: 4146
	public static class RectTransformUtilityAdvanced
	{
		// Token: 0x0600BD79 RID: 48505 RVA: 0x0056116C File Offset: 0x0055F36C
		public static Rect PointToRect(Vector2 posA, Vector2 posB)
		{
			Vector2 min = new Vector2(Mathf.Min(posA.x, posB.x), Mathf.Min(posA.y, posB.y));
			Vector2 max = new Vector2(Mathf.Max(posA.x, posB.x), Mathf.Max(posA.y, posB.y));
			return new Rect(min, max - min);
		}

		// Token: 0x0600BD7A RID: 48506 RVA: 0x005611E0 File Offset: 0x0055F3E0
		public static Rect ToRectInParent(this RectTransform rt)
		{
			Tester.Assert(rt.anchorMin == rt.anchorMax, "ToRectInParent does not support different anchors, use ToWorldRect instead.");
			Vector2 pos = rt.anchoredPosition;
			Vector2 size = rt.sizeDelta;
			Vector2 pivotOffset = Vector2.Scale(size, rt.pivot);
			Vector2 rectMin = pos - pivotOffset;
			return new Rect(rectMin, size);
		}

		// Token: 0x0600BD7B RID: 48507 RVA: 0x0056123C File Offset: 0x0055F43C
		public static Rect PointToWorldRect(this RectTransform rt, Vector2 posA, Vector2 posB)
		{
			Vector3 worldPosA = rt.TransformPoint(posA);
			Vector3 worldPosB = rt.TransformPoint(posB);
			return RectTransformUtilityAdvanced.PointToRect(worldPosA, worldPosB);
		}

		// Token: 0x0600BD7C RID: 48508 RVA: 0x0056127C File Offset: 0x0055F47C
		public static Rect ToWorldRect(this RectTransform rt)
		{
			if (RectTransformUtilityAdvanced._cacheCorners == null)
			{
				RectTransformUtilityAdvanced._cacheCorners = new Vector3[4];
			}
			rt.GetWorldCorners(RectTransformUtilityAdvanced._cacheCorners);
			Vector3 bottomLeft = RectTransformUtilityAdvanced._cacheCorners[0];
			Vector3 topRight = RectTransformUtilityAdvanced._cacheCorners[2];
			Vector3 size = topRight - bottomLeft;
			return new Rect(bottomLeft, size);
		}

		// Token: 0x0600BD7D RID: 48509 RVA: 0x005612E0 File Offset: 0x0055F4E0
		public static Vector2 ScreenToLocalPointInParent(this RectTransform rt, Vector2 screenPoint)
		{
			Transform parent = rt.parent;
			bool flag = parent == null;
			if (flag)
			{
				throw new Exception(string.Format("Cannot transform point in {0} because no parent", rt));
			}
			RectTransform parentRt = parent as RectTransform;
			bool flag2 = parentRt == null;
			if (flag2)
			{
				throw new Exception(string.Format("Cannot transform point in {0} because parent is not rect transform", rt));
			}
			return parentRt.ScreenToLocalPoint(screenPoint);
		}

		// Token: 0x0600BD7E RID: 48510 RVA: 0x00561344 File Offset: 0x0055F544
		public static Vector2 ScreenToLocalPoint(this RectTransform rt, Vector2 screenPoint)
		{
			Camera uiCamera = UIManager.Instance.UiCamera;
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPoint, uiCamera, out localPoint);
			return localPoint;
		}

		// Token: 0x0600BD7F RID: 48511 RVA: 0x00561370 File Offset: 0x0055F570
		public static Vector2 LocalToScreenPoint(this RectTransform rt, Vector2 localPoint)
		{
			Vector3 worldPoint = rt.TransformPoint(localPoint);
			Camera uiCamera = UIManager.Instance.UiCamera;
			return RectTransformUtility.WorldToScreenPoint(uiCamera, worldPoint);
		}

		// Token: 0x040091CE RID: 37326
		private static Vector3[] _cacheCorners;
	}
}
