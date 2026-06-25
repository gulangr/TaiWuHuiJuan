using System;
using Config;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Components.Combat
{
	// Token: 0x02000F07 RID: 3847
	public class CombatDamageDetailSectionItem : MonoBehaviour
	{
		// Token: 0x0600B15A RID: 45402 RVA: 0x0050CDC0 File Offset: 0x0050AFC0
		public void Set(DefeatMarkKey markKey, int value)
		{
			this.bgImage.SetSprite(CombatDamageDetailSectionItem.GetBgName(markKey), false, null);
			this.titleLabel.text = CombatDamageDetailSectionItem.GetDamageTypeName(markKey);
			this.valueLabel.text = value.ToString();
		}

		// Token: 0x0600B15B RID: 45403 RVA: 0x0050CDFC File Offset: 0x0050AFFC
		private static string GetBgName(DefeatMarkKey markKey)
		{
			EMarkType type = markKey.Type;
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case EMarkType.Outer:
				result = "ui9_back_section_damage_detail_out";
				goto IL_8D;
			case EMarkType.Inner:
				result = "ui9_back_section_damage_detail_in";
				goto IL_8D;
			case EMarkType.Poison:
				result = ((markKey.PoisonType >= 0 && markKey.PoisonType <= 5) ? string.Format("ui9_back_section_damage_detail_poison_{0}", markKey.PoisonType) : "ui9_back_section_damage_detail_poison_0");
				goto IL_8D;
			case EMarkType.Mind:
				result = "ui9_back_section_damage_detail_fatal_mind";
				goto IL_8D;
			case EMarkType.Fatal:
				result = "ui9_back_section_damage_detail_fatal_mind";
				goto IL_8D;
			}
			result = "ui9_back_section_damage_detail_out";
			IL_8D:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600B15C RID: 45404 RVA: 0x0050CEA0 File Offset: 0x0050B0A0
		private static string GetDamageTypeName(DefeatMarkKey markKey)
		{
			EMarkType type = markKey.Type;
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case EMarkType.Outer:
				result = LocalStringManager.Get(LanguageKey.LK_Out_Injury);
				goto IL_87;
			case EMarkType.Inner:
				result = LocalStringManager.Get(LanguageKey.LK_Inner_Injury);
				goto IL_87;
			case EMarkType.Poison:
				result = Poison.Instance[markKey.PoisonType].Name;
				goto IL_87;
			case EMarkType.Mind:
				result = LocalStringManager.Get(LanguageKey.LK_Combat_MarkName_Mind);
				goto IL_87;
			case EMarkType.Fatal:
				result = LocalStringManager.Get(LanguageKey.LK_Combat_MarkType_Fatal);
				goto IL_87;
			}
			result = CombatConstants.ParseMarkType(markKey);
			IL_87:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04008972 RID: 35186
		[SerializeField]
		private CImage bgImage;

		// Token: 0x04008973 RID: 35187
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x04008974 RID: 35188
		[SerializeField]
		private TextMeshProUGUI valueLabel;
	}
}
