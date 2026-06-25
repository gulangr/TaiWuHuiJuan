using System;
using Config;
using Game.Views.MouseTips;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ECF RID: 3791
	public class PoisonIconAndCountCell : MonoBehaviour, ICellContent<ItemDisplayData>, ICellContent
	{
		// Token: 0x0600AF0B RID: 44811 RVA: 0x004FC19F File Offset: 0x004FA39F
		public static string GetPoisonTypeIcon(sbyte type)
		{
			return string.Format("{0}{1}", "ui9_icon_poison_0_", type);
		}

		// Token: 0x0600AF0C RID: 44812 RVA: 0x004FC1B6 File Offset: 0x004FA3B6
		public static string GetPoisonLevelIcon(int levelIconIndex)
		{
			return string.Format("{0}{1}", "ui9_icon_venom_", levelIconIndex);
		}

		// Token: 0x0600AF0D RID: 44813 RVA: 0x004FC1D0 File Offset: 0x004FA3D0
		public void SetData(ItemDisplayData data)
		{
			bool flag = data == null;
			if (flag)
			{
				this.poisonItem.gameObject.SetActive(false);
			}
			else
			{
				MedicineItem configData = Medicine.Instance[data.RealKey.TemplateId];
				sbyte type = configData.PoisonType;
				short poisonValue = configData.EffectValue;
				sbyte poisonLevel = (sbyte)configData.EffectThresholdValue;
				bool show = poisonValue > 0;
				this.poisonItem.gameObject.SetActive(show);
				bool flag2 = show;
				if (flag2)
				{
					string typeIcon = PoisonIconAndCountCell.GetPoisonTypeIcon(type);
					int levelIconIndex = TooltipCombatSkill.GetPoisonLevelIconIndex(poisonLevel);
					string levelIcon = PoisonIconAndCountCell.GetPoisonLevelIcon(levelIconIndex);
					string value = poisonValue.ToString().SetColor("brightblue");
					this.poisonItem.Set("", value, false, typeIcon, levelIcon, (int)poisonLevel);
				}
			}
		}

		// Token: 0x04008790 RID: 34704
		[SerializeField]
		private TooltipItemPropertyPoison poisonItem;
	}
}
