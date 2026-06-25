using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameData.Adventure;
using GameData.Utilities;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class AdventureMajorEventTool
{
	// Token: 0x06001754 RID: 5972 RVA: 0x0008F6A4 File Offset: 0x0008D8A4
	public static string GetAdventureMajorEventNodeIcon(EAdventureMajorEventNodeType nodeType, int nodeStyle, sbyte stateType)
	{
		bool flag = nodeType == EAdventureMajorEventNodeType.Check;
		string result;
		if (flag)
		{
			result = AdventureMajorEventTool.GetCheckNodeIconName(nodeType, nodeStyle, stateType);
		}
		else
		{
			result = AdventureMajorEventTool.GetNodeIconName(nodeType, stateType);
		}
		return result;
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x0008F6D0 File Offset: 0x0008D8D0
	public static string GetNodeIconName(EAdventureMajorEventNodeType nodeType, sbyte nodePassType)
	{
		return string.Format("ui9_adventure_major_node_{0}_{1}", nodeType.ToInt(), nodePassType);
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x0008F704 File Offset: 0x0008D904
	public static string GetCheckNodeIconName(EAdventureMajorEventNodeType nodeType, int nodeStyle, sbyte nodePassType)
	{
		return string.Format("ui9_adventure_major_node_{0}_{1}_{2}", nodeType.ToInt(), nodeStyle, nodePassType);
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x0008F73C File Offset: 0x0008D93C
	public static string GetNodeDecorateName(EAdventureMajorEventNodeType nodeType)
	{
		return string.Format("ui9_adventure_major_node_decorate_{0}", nodeType.ToInt());
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x0008F768 File Offset: 0x0008D968
	public static string GetLineSpriteName(sbyte passType, sbyte lineStyle = 0)
	{
		Tester.Assert(passType <= 3, "");
		bool flag = passType < 3;
		string result;
		if (flag)
		{
			result = string.Format("ui9_adventure_major_road_{0}", passType);
		}
		else
		{
			result = string.Format("ui9_adventure_major_road_{0}_{1}", passType, lineStyle);
		}
		return result;
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x0008F7C0 File Offset: 0x0008D9C0
	public static sbyte GetPassedLineStyle(EAdventureMajorEventNodeType currentType, EAdventureMajorEventNodeType nextType)
	{
		bool flag = currentType == EAdventureMajorEventNodeType.Check && nextType == EAdventureMajorEventNodeType.Check;
		sbyte result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			bool flag2 = currentType != EAdventureMajorEventNodeType.Check && nextType != EAdventureMajorEventNodeType.Check;
			if (flag2)
			{
				result = 2;
			}
			else
			{
				result = 1;
			}
		}
		return result;
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x0008F7FC File Offset: 0x0008D9FC
	public static Vector3 GetCenterPos(RectTransform start, RectTransform end)
	{
		Vector3 centerPos = new Vector3((start.localPosition.x + end.localPosition.x) / 2f, (start.localPosition.y + end.localPosition.y) / 2f, 0f);
		return centerPos;
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x0008F858 File Offset: 0x0008DA58
	public static float GetLineWidth(RectTransform start, RectTransform end)
	{
		return Vector3.Distance(start.localPosition, end.localPosition);
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x0008F87C File Offset: 0x0008DA7C
	public static Quaternion GetQuaternion(RectTransform start, RectTransform end)
	{
		Vector3 direction = end.localPosition - start.localPosition;
		return Quaternion.LookRotation(Vector3.forward, direction);
	}

	// Token: 0x0600175D RID: 5981 RVA: 0x0008F8B0 File Offset: 0x0008DAB0
	public static void EditInt(ref int item, string str, [CallerMemberName] string caller = "caller")
	{
		int integer;
		bool flag = int.TryParse(str, out integer);
		if (flag)
		{
			item = integer;
		}
		else
		{
			Debug.LogError("input [" + str + "] is not a valid integer for " + caller);
		}
	}

	// Token: 0x0600175E RID: 5982 RVA: 0x0008F8E8 File Offset: 0x0008DAE8
	public static void EditUInt(ref uint item, string str, [CallerMemberName] string caller = "caller")
	{
		uint integer;
		bool flag = uint.TryParse(str, out integer);
		if (flag)
		{
			item = integer;
		}
		else
		{
			Debug.LogError("input [" + str + "] is not a valid integer for " + caller);
		}
	}

	// Token: 0x040012D1 RID: 4817
	public const string AdventureMajorEventResourcePath = "AdventureMajorEvent/";

	// Token: 0x040012D2 RID: 4818
	public static readonly Dictionary<int, string> CheckNodeStyleDict = new Dictionary<int, string>
	{
		{
			0,
			LocalStringManager.Get(LanguageKey.LK_MajorEventEditor_CheckNodeStyle_0)
		},
		{
			1,
			LocalStringManager.Get(LanguageKey.LK_MajorEventEditor_CheckNodeStyle_1)
		},
		{
			2,
			LocalStringManager.Get(LanguageKey.LK_MajorEventEditor_CheckNodeStyle_2)
		},
		{
			3,
			LocalStringManager.Get(LanguageKey.LK_MajorEventEditor_CheckNodeStyle_3)
		},
		{
			4,
			LocalStringManager.Get(LanguageKey.LK_MajorEventEditor_CheckNodeStyle_4)
		}
	};

	// Token: 0x020012E4 RID: 4836
	public static class NodePassType
	{
		// Token: 0x04009BEF RID: 39919
		public const sbyte Preview = 0;

		// Token: 0x04009BF0 RID: 39920
		public const sbyte Passable = 1;

		// Token: 0x04009BF1 RID: 39921
		public const sbyte NotPassable = 2;

		// Token: 0x04009BF2 RID: 39922
		public const sbyte Passed = 3;

		// Token: 0x04009BF3 RID: 39923
		public const sbyte Current = 4;
	}

	// Token: 0x020012E5 RID: 4837
	public static class PassedLineStyle
	{
		// Token: 0x04009BF4 RID: 39924
		public const sbyte ShortLine = 0;

		// Token: 0x04009BF5 RID: 39925
		public const sbyte MiddleLine = 1;

		// Token: 0x04009BF6 RID: 39926
		public const sbyte LongLine = 2;
	}
}
