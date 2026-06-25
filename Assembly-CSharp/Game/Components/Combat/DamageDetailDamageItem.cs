using System;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Components.Combat
{
	// Token: 0x02000F0A RID: 3850
	public class DamageDetailDamageItem : MonoBehaviour
	{
		// Token: 0x0600B163 RID: 45411 RVA: 0x0050D0A4 File Offset: 0x0050B2A4
		public void Set(int sectionIndex, DefeatMarkKey markKey, int value)
		{
			EMarkType type = markKey.Type;
			if (!true)
			{
			}
			string text;
			switch (type)
			{
			case EMarkType.Outer:
				text = "ui9_icon_bodypart_big_outter_" + markKey.BodyPart.ToString();
				goto IL_94;
			case EMarkType.Inner:
				text = "ui9_icon_bodypart_big_inner_" + markKey.BodyPart.ToString();
				goto IL_94;
			case EMarkType.Poison:
				text = "ui9_back_poison_big_7_" + markKey.PoisonType.ToString();
				goto IL_94;
			case EMarkType.Mind:
				text = "sp_combat_icon_shishen";
				goto IL_94;
			}
			text = "sp_combat_icon_zhongchuang_0";
			IL_94:
			if (!true)
			{
			}
			string icon = text;
			string prefix = (sectionIndex < 0) ? string.Empty : string.Format("第{0}段攻击 ", sectionIndex);
			this.Set(icon, prefix + value.ToString());
		}

		// Token: 0x0600B164 RID: 45412 RVA: 0x0050D17C File Offset: 0x0050B37C
		public void Set(DefeatMarkKey markKey, int value)
		{
			this.Set(-1, markKey, value);
		}

		// Token: 0x0600B165 RID: 45413 RVA: 0x0050D189 File Offset: 0x0050B389
		public void Set(string icon, string value)
		{
			CImage cimage = this.imageIcon;
			if (cimage != null)
			{
				cimage.SetSprite(icon, false, null);
			}
			this.textValue.text = value;
		}

		// Token: 0x0600B166 RID: 45414 RVA: 0x0050D1B0 File Offset: 0x0050B3B0
		public void Set(Sprite iconSprite, string value)
		{
			bool flag = this.imageIcon;
			if (flag)
			{
				this.imageIcon.sprite = iconSprite;
			}
			this.textValue.text = value;
		}

		// Token: 0x0600B167 RID: 45415 RVA: 0x0050D1E8 File Offset: 0x0050B3E8
		public void SetValueColor(Color color)
		{
			bool flag = this.textValue != null;
			if (flag)
			{
				this.textValue.color = color;
			}
		}

		// Token: 0x0400897A RID: 35194
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x0400897B RID: 35195
		[SerializeField]
		private TextMeshProUGUI textValue;
	}
}
