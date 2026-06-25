using System;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F16 RID: 3862
	public class CharacterAttainmentItem : MonoBehaviour
	{
		// Token: 0x0600B1EC RID: 45548 RVA: 0x00510A48 File Offset: 0x0050EC48
		public void SetCombatSkillType(sbyte type)
		{
			this.titleName.text = CombatSkillType.Instance[type].Name;
			CImage cimage = this.nameIcon;
			if (cimage != null)
			{
				cimage.SetSprite(string.Format("{0}{1}", "ui9_back_attainments_combat_3_", type), false, null);
			}
		}

		// Token: 0x0600B1ED RID: 45549 RVA: 0x00510A9C File Offset: 0x0050EC9C
		public void SetLifeSkillType(sbyte type)
		{
			this.titleName.text = LifeSkillType.Instance[type].Name;
			CImage cimage = this.nameIcon;
			if (cimage != null)
			{
				cimage.SetSprite(string.Format("{0}{1}", "ui9_back_attainments_life_3_", type), false, null);
			}
		}

		// Token: 0x0600B1EE RID: 45550 RVA: 0x00510AEF File Offset: 0x0050ECEF
		public void SetCombatSkillTypeTitle(sbyte type)
		{
			this.titleName.text = CombatSkillType.Instance[type].Name;
		}

		// Token: 0x0600B1EF RID: 45551 RVA: 0x00510B0E File Offset: 0x0050ED0E
		public void SetLifeSkillTypeTitle(sbyte type)
		{
			this.titleName.text = LifeSkillType.Instance[type].Name;
		}

		// Token: 0x0600B1F0 RID: 45552 RVA: 0x00510B2D File Offset: 0x0050ED2D
		public void SetQualification(int value)
		{
			this.SetQualificationText(((short)value).SetValueColor());
		}

		// Token: 0x0600B1F1 RID: 45553 RVA: 0x00510B3E File Offset: 0x0050ED3E
		public void SetQualificationText(string value)
		{
			this.qualificationTxt.text = value.ColorReplace();
		}

		// Token: 0x0600B1F2 RID: 45554 RVA: 0x00510B53 File Offset: 0x0050ED53
		public void SetAttainment(short value)
		{
			this.SetAttainmentText(((int)value).SetColorByValue());
		}

		// Token: 0x0600B1F3 RID: 45555 RVA: 0x00510B63 File Offset: 0x0050ED63
		public void SetAttainmentText(string content)
		{
			this.attainmentTxt.text = CharacterAttainmentItem.FormatAttainmentDisplayText(content);
		}

		// Token: 0x0600B1F4 RID: 45556 RVA: 0x00510B78 File Offset: 0x0050ED78
		private static string FormatAttainmentDisplayText(string content)
		{
			bool flag = content.IsNullOrEmpty() || content == "-";
			string result;
			if (flag)
			{
				result = content.ColorReplace();
			}
			else
			{
				short value;
				bool flag2 = short.TryParse(content, out value);
				if (flag2)
				{
					result = ((int)value).SetColorByValue();
				}
				else
				{
					result = content.ColorReplace();
				}
			}
			return result;
		}

		// Token: 0x0600B1F5 RID: 45557 RVA: 0x00510BC8 File Offset: 0x0050EDC8
		public void SetAttainmentLevelDesc(string desc)
		{
			this.qualificationLevelTxt.text = desc;
		}

		// Token: 0x040089EA RID: 35306
		[SerializeField]
		private CImage nameIcon;

		// Token: 0x040089EB RID: 35307
		[SerializeField]
		private TextMeshProUGUI titleName;

		// Token: 0x040089EC RID: 35308
		[SerializeField]
		private TextMeshProUGUI qualificationLevelTxt;

		// Token: 0x040089ED RID: 35309
		[SerializeField]
		private TextMeshProUGUI qualificationTxt;

		// Token: 0x040089EE RID: 35310
		[SerializeField]
		private TextMeshProUGUI attainmentTxt;

		// Token: 0x040089EF RID: 35311
		public CToggle Toggle;
	}
}
