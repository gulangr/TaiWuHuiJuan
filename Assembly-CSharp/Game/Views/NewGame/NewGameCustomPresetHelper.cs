using System;
using System.Collections.Generic;
using GameData.Domains.Global;

namespace Game.Views.NewGame
{
	// Token: 0x020007E4 RID: 2020
	public static class NewGameCustomPresetHelper
	{
		// Token: 0x06006278 RID: 25208 RVA: 0x002D1B34 File Offset: 0x002CFD34
		public static CustomProtagonistPreset GetCustomProtagonistPreset()
		{
			return NewGameCustomPresetHelper._customProtagonistPreset;
		}

		// Token: 0x06006279 RID: 25209 RVA: 0x002D1B4B File Offset: 0x002CFD4B
		public static void SetCustomProtagonistPreset(CustomProtagonistPreset preset)
		{
			NewGameCustomPresetHelper._customProtagonistPreset = (preset ?? new CustomProtagonistPreset());
		}

		// Token: 0x0600627A RID: 25210 RVA: 0x002D1B5D File Offset: 0x002CFD5D
		public static void SaveCustomProtagonistPreset()
		{
			GlobalDomainMethod.Call.SetCustomProtagonistPreset(NewGameCustomPresetHelper._customProtagonistPreset);
		}

		// Token: 0x0600627B RID: 25211 RVA: 0x002D1B6C File Offset: 0x002CFD6C
		public static int GetCurrentPresetIndex()
		{
			return NewGameCustomPresetHelper._customProtagonistPreset.CurrentPresetIndex;
		}

		// Token: 0x0600627C RID: 25212 RVA: 0x002D1B88 File Offset: 0x002CFD88
		public static int GetPresetCount()
		{
			return NewGameCustomPresetHelper._customProtagonistPreset.Presets.Count;
		}

		// Token: 0x0600627D RID: 25213 RVA: 0x002D1BAC File Offset: 0x002CFDAC
		public static bool CanAddPreset()
		{
			return NewGameCustomPresetHelper._customProtagonistPreset.CanAdd;
		}

		// Token: 0x0600627E RID: 25214 RVA: 0x002D1BC8 File Offset: 0x002CFDC8
		public static bool CanDeletePreset()
		{
			return NewGameCustomPresetHelper._customProtagonistPreset.CanDelete;
		}

		// Token: 0x0600627F RID: 25215 RVA: 0x002D1BE4 File Offset: 0x002CFDE4
		public static void ChangePreset(int newPresetIndex)
		{
			NewGameCustomPresetHelper._customProtagonistPreset.ChangePreset(newPresetIndex);
			NewGameCustomPresetHelper.SaveCustomProtagonistPreset();
		}

		// Token: 0x06006280 RID: 25216 RVA: 0x002D1BF9 File Offset: 0x002CFDF9
		public static void AddPreset()
		{
			NewGameCustomPresetHelper._customProtagonistPreset.AddPreset();
			NewGameCustomPresetHelper.SaveCustomProtagonistPreset();
		}

		// Token: 0x06006281 RID: 25217 RVA: 0x002D1C0D File Offset: 0x002CFE0D
		public static void ClonePreset()
		{
			NewGameCustomPresetHelper._customProtagonistPreset.ClonePreset();
			NewGameCustomPresetHelper.SaveCustomProtagonistPreset();
		}

		// Token: 0x06006282 RID: 25218 RVA: 0x002D1C21 File Offset: 0x002CFE21
		public static void ClearPreset()
		{
			NewGameCustomPresetHelper._customProtagonistPreset.ClearPreset();
			NewGameCustomPresetHelper.SaveCustomProtagonistPreset();
		}

		// Token: 0x06006283 RID: 25219 RVA: 0x002D1C35 File Offset: 0x002CFE35
		public static void DeletePreset()
		{
			NewGameCustomPresetHelper._customProtagonistPreset.DeletePreset();
			NewGameCustomPresetHelper.SaveCustomProtagonistPreset();
		}

		// Token: 0x06006284 RID: 25220 RVA: 0x002D1C4C File Offset: 0x002CFE4C
		public static CustomProtagonistPresetItem GetPresetItem(int index)
		{
			IReadOnlyList<CustomProtagonistPresetItem> presets = NewGameCustomPresetHelper._customProtagonistPreset.Presets;
			bool flag = index >= 0 && index < presets.Count;
			CustomProtagonistPresetItem result;
			if (flag)
			{
				result = presets[index];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06006285 RID: 25221 RVA: 0x002D1C8C File Offset: 0x002CFE8C
		public static CustomProtagonistPresetItem GetCurrentPresetItem()
		{
			return NewGameCustomPresetHelper.GetPresetItem(NewGameCustomPresetHelper.GetCurrentPresetIndex());
		}

		// Token: 0x06006286 RID: 25222 RVA: 0x002D1CA8 File Offset: 0x002CFEA8
		public static void UpdatePresetItem(int index, CustomProtagonistPresetItem item)
		{
			bool flag = index >= 0 && index < NewGameCustomPresetHelper._customProtagonistPreset.Presets.Count;
			if (flag)
			{
				List<CustomProtagonistPresetItem> newList = new List<CustomProtagonistPresetItem>(NewGameCustomPresetHelper._customProtagonistPreset.Presets);
				newList[index] = item;
				NewGameCustomPresetHelper._customProtagonistPreset.OverwritePresets(newList, NewGameCustomPresetHelper._customProtagonistPreset.CurrentPresetIndex);
				NewGameCustomPresetHelper.SaveCustomProtagonistPreset();
			}
		}

		// Token: 0x06006287 RID: 25223 RVA: 0x002D1D0A File Offset: 0x002CFF0A
		public static void UpdateCurrentPresetItem(CustomProtagonistPresetItem item)
		{
			NewGameCustomPresetHelper.UpdatePresetItem(NewGameCustomPresetHelper._customProtagonistPreset.CurrentPresetIndex, item);
		}

		// Token: 0x0400448D RID: 17549
		public const int MaxCustomPresets = 5;

		// Token: 0x0400448E RID: 17550
		private static CustomProtagonistPreset _customProtagonistPreset = new CustomProtagonistPreset();
	}
}
