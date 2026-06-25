using System;
using Game.Components.Character;
using Game.Components.Common;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000888 RID: 2184
	public class TooltipCombatSkillGridItem : MonoBehaviour
	{
		// Token: 0x060068EF RID: 26863 RVA: 0x003034B5 File Offset: 0x003016B5
		public void Set(int equipType, sbyte count)
		{
			this.Set(equipType, count, false);
		}

		// Token: 0x060068F0 RID: 26864 RVA: 0x003034C4 File Offset: 0x003016C4
		public void Set(int equipType, sbyte count, bool showSpecialBack)
		{
			string spriteName = string.Format("{0}{1}", "ui9_icon_mousetip_equip_type_grid_", equipType - 1);
			this.icon.SetSprite(spriteName, false, null);
			ImageDigits imageDigits = this.countDigits;
			if (imageDigits != null)
			{
				imageDigits.SetInt((int)count, 1, 2, "White");
			}
			string equipTypeName = (equipType == 5) ? LocalStringManager.Get("LK_CombatSkill_EquipType_Generic") : LocalStringManager.Get(string.Format("LK_CombatSkill_EquipType_{0}", equipType));
			bool flag = equipType != 5;
			if (flag)
			{
				this.nameText.text = equipTypeName.SetColor(NeiliAllocationTypes.GetColorByType((byte)(equipType - 1)));
			}
			else
			{
				this.nameText.text = equipTypeName;
			}
			bool flag2 = this.specialBack;
			if (flag2)
			{
				this.specialBack.enabled = showSpecialBack;
			}
		}

		// Token: 0x04004B0E RID: 19214
		[SerializeField]
		private CImage icon;

		// Token: 0x04004B0F RID: 19215
		[SerializeField]
		private ImageDigits countDigits;

		// Token: 0x04004B10 RID: 19216
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04004B11 RID: 19217
		[SerializeField]
		private CImage specialBack;
	}
}
