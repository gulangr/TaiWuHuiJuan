using System;
using Coffee.UIExtensions;
using TMPro;
using UnityEngine;

namespace Game.Views.LegendaryBook
{
	// Token: 0x0200098A RID: 2442
	public class LegendaryBookBonusItem : MonoBehaviour
	{
		// Token: 0x06007594 RID: 30100 RVA: 0x0036C5F9 File Offset: 0x0036A7F9
		public void SetupTips()
		{
			this.MouseTip.enabled = true;
			this.MouseTip.Type = TipType.LegendaryBookBonus;
		}

		// Token: 0x06007595 RID: 30101 RVA: 0x0036C618 File Offset: 0x0036A818
		public void SetNameText(string value)
		{
			this.NameUnlocked.GetComponent<TextMeshProUGUI>().text = value;
			this.NameCanUnlock.GetComponent<TextMeshProUGUI>().text = value;
			this.NameCannotUnlock.GetComponent<TextMeshProUGUI>().text = value;
			bool flag = this.NameHolder != null;
			if (flag)
			{
				this.NameHolder.SetActive(!string.IsNullOrEmpty(value));
			}
		}

		// Token: 0x06007596 RID: 30102 RVA: 0x0036C684 File Offset: 0x0036A884
		public void SetSkillTypeIcon(sbyte currCombatSkillType)
		{
			bool flag = this.SkillTypeBtnImage;
			if (flag)
			{
				this.SkillTypeBtnImage.SetSprite(string.Format("ui9_sp_legendbook_btn_{0}_normol", currCombatSkillType), false, null);
			}
			bool flag2 = this.SkillTypeHighlight;
			if (flag2)
			{
				this.SkillTypeHighlight.SetSprite(string.Format("ui9_sp_legendbook_btn_{0}_hover", currCombatSkillType), false, null);
			}
			bool flag3 = this.SkillTypeUnlocked;
			if (flag3)
			{
				this.SkillTypeUnlocked.SetSprite(string.Format("ui9_sp_legendbook_btn_{0}_jiadian", currCombatSkillType), false, null);
			}
			bool flag4 = this.SkillTypeHighlight_1;
			if (flag4)
			{
				this.SkillTypeHighlight_1.SetSprite(string.Format("ui9_sp_legendbook_btn_{0}_jiadian", currCombatSkillType), false, null);
			}
			bool flag5 = this.SkillTypeDisable;
			if (flag5)
			{
				this.SkillTypeDisable.SetSprite(string.Format("ui9_sp_legendbook_btn_{0}_dissable", currCombatSkillType), false, null);
			}
		}

		// Token: 0x0400583A RID: 22586
		public int UserInt;

		// Token: 0x0400583B RID: 22587
		public GameObject Unlocked;

		// Token: 0x0400583C RID: 22588
		public GameObject DisableObj;

		// Token: 0x0400583D RID: 22589
		public GameObject BtnImage;

		// Token: 0x0400583E RID: 22590
		public GameObject NameUnlocked;

		// Token: 0x0400583F RID: 22591
		public GameObject NameCanUnlock;

		// Token: 0x04005840 RID: 22592
		public GameObject NameCannotUnlock;

		// Token: 0x04005841 RID: 22593
		public GameObject NameHolder;

		// Token: 0x04005842 RID: 22594
		public UIParticle EffActive;

		// Token: 0x04005843 RID: 22595
		public TooltipInvoker MouseTip;

		// Token: 0x04005844 RID: 22596
		[SerializeField]
		private CImage SkillTypeBtnImage;

		// Token: 0x04005845 RID: 22597
		[SerializeField]
		private CImage SkillTypeHighlight;

		// Token: 0x04005846 RID: 22598
		[SerializeField]
		private CImage SkillTypeUnlocked;

		// Token: 0x04005847 RID: 22599
		[SerializeField]
		private CImage SkillTypeHighlight_1;

		// Token: 0x04005848 RID: 22600
		[SerializeField]
		private CImage SkillTypeDisable;
	}
}
