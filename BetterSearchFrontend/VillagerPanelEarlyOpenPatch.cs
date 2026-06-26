using System;
using System.Collections.Generic;
using System.Reflection;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views;
using Game.Views.Bottom;
using HarmonyLib;
using UnityEngine;

namespace BetterSearchFrontend
{
	// Token: 0x0200000B RID: 11
	internal sealed class VillagerPanelEarlyOpenPatch : BaseFrontPatch
	{
		// Token: 0x0600004B RID: 75 RVA: 0x00003735 File Offset: 0x00001935
		public override void OnModSettingUpdate(string modIdStr)
		{
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003738 File Offset: 0x00001938
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ViewBottom), "HandleHotKeyCommon")]
		private static void ViewBottom_HandleHotKeyCommon_Postfix(ViewBottom __instance)
		{
			if (__instance == null || VillagerPanelEarlyOpenPatch.IsVillageManagementUnlocked())
			{
				return;
			}
			if (!MainInterfaceFunctionCommandKit.VillagerList.Check(__instance.Element, false, false, false, true, false))
			{
				return;
			}
			UIElement.TaiwuVillagers.SetOnInitArgs(EasyPool.Get<ArgumentBox>());
			UIManager.Instance.MaskUI(UIElement.TaiwuVillagers);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x0000378C File Offset: 0x0000198C
		internal static void ApplyExpelControlsVisibility(ViewTaiwuVillagers view, bool villagerScopeActive)
		{
			if (view == null)
			{
				return;
			}
			FieldInfo idsField = VillagerPanelEarlyOpenPatch.IdsField;
			if (((idsField != null) ? idsField.GetValue(view) : null) is List<int>)
			{
				return;
			}
			bool flag = villagerScopeActive && VillagerPanelEarlyOpenPatch.IsVillageManagementUnlocked();
			FieldInfo multiSelectField = VillagerPanelEarlyOpenPatch.MultiSelectField;
			CToggle ctoggle = ((multiSelectField != null) ? multiSelectField.GetValue(view) : null) as CToggle;
			if (!flag && ctoggle != null && ctoggle.isOn)
			{
				ctoggle.isOn = false;
			}
			FieldInfo bottomGroupField = VillagerPanelEarlyOpenPatch.BottomGroupField;
			GameObject gameObject = ((bottomGroupField != null) ? bottomGroupField.GetValue(view) : null) as GameObject;
			if (gameObject != null)
			{
				gameObject.SetActive(flag);
			}
			Transform transform;
			if (ctoggle == null)
			{
				transform = null;
			}
			else
			{
				Transform transform2 = ctoggle.transform;
				transform = ((transform2 != null) ? transform2.parent : null);
			}
			Transform transform3 = transform;
			if (transform3 != null)
			{
				transform3.gameObject.SetActive(flag);
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003850 File Offset: 0x00001A50
		internal static bool IsVillageManagementUnlocked()
		{
			bool result;
			try
			{
				FunctionLockManager instance = SingletonObject.getInstance<FunctionLockManager>();
				result = (instance != null && instance.IsFunctionUnlock(10));
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x04000027 RID: 39
		internal const byte VillageManagementFunctionId = 10;

		// Token: 0x04000028 RID: 40
		private static readonly FieldInfo IdsField = AccessTools.Field(typeof(ViewTaiwuVillagers), "_ids");

		// Token: 0x04000029 RID: 41
		private static readonly FieldInfo BottomGroupField = AccessTools.Field(typeof(ViewTaiwuVillagers), "bottomGroup");

		// Token: 0x0400002A RID: 42
		private static readonly FieldInfo MultiSelectField = AccessTools.Field(typeof(ViewTaiwuVillagers), "multiSelect");
	}
}
