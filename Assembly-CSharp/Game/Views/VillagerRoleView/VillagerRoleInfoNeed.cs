using System;
using System.Collections.Generic;
using Config;
using DisplayConfig;
using TMPro;
using UnityEngine;

namespace Game.Views.VillagerRoleView
{
	// Token: 0x0200073C RID: 1852
	public class VillagerRoleInfoNeed : MonoBehaviour
	{
		// Token: 0x0600598F RID: 22927 RVA: 0x00298BE8 File Offset: 0x00296DE8
		public void Refresh(VillagerRoleItem roleConfig)
		{
			this._config = roleConfig;
			this.RefreshPersonality();
			this.RefreshAttainment();
		}

		// Token: 0x06005990 RID: 22928 RVA: 0x00298C00 File Offset: 0x00296E00
		private void RefreshPersonality()
		{
			PersonalityItem displayConfig = Personality.Instance[(int)this._config.PersonalityType];
			this.personalityIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_building_personality_big_", displayConfig.TemplateId), false, null);
			this.personalityLabel.text = displayConfig.Name;
		}

		// Token: 0x06005991 RID: 22929 RVA: 0x00298C60 File Offset: 0x00296E60
		private void RefreshAttainment()
		{
			this._attainmentItems.Clear();
			foreach (sbyte type in this._config.LearnableLifeSkillTypes)
			{
				this._attainmentItems.Add(VillagerRoleInfoNeed.AttainmentItem.CreateLife(type));
			}
			sbyte[] learnableCombatSkillTypes = this._config.LearnableCombatSkillTypes;
			int num = 0;
			if (num < learnableCombatSkillTypes.Length)
			{
				sbyte type2 = learnableCombatSkillTypes[num];
				this._attainmentItems.Add(VillagerRoleInfoNeed.AttainmentItem.CreateCombat(0));
			}
			CommonUtils.PrepareExtraItemInfo prepareExtraItemInfo = new CommonUtils.PrepareExtraItemInfo
			{
				TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems,
				ExtraItemCount = 1
			};
			CommonUtils.PrepareEnoughChildren(this.attainmentItemRoot.transform, this.attainmentItemTemplate.gameObject, this._attainmentItems.Count, new CommonUtils.PrepareExtraItemInfo?(prepareExtraItemInfo));
			for (int i = 0; i < this._attainmentItems.Count; i++)
			{
				VillagerRoleInfoNeed.AttainmentItem item = this._attainmentItems[i];
				Refers refers = this.attainmentItemRoot.GetChild(i + 1).GetComponent<Refers>();
				CImage icon = refers.CGet<CImage>("Icon");
				TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
				GameObject strip = refers.CGet<GameObject>("Strip");
				item.SetIcon(icon);
				item.SetLabel(label);
				strip.SetActive(i > 0);
			}
		}

		// Token: 0x04003D97 RID: 15767
		private VillagerRoleItem _config;

		// Token: 0x04003D98 RID: 15768
		private readonly List<VillagerRoleInfoNeed.AttainmentItem> _attainmentItems = new List<VillagerRoleInfoNeed.AttainmentItem>();

		// Token: 0x04003D99 RID: 15769
		[SerializeField]
		private CImage personalityIcon;

		// Token: 0x04003D9A RID: 15770
		[SerializeField]
		private TextMeshProUGUI personalityLabel;

		// Token: 0x04003D9B RID: 15771
		[SerializeField]
		private RectTransform attainmentItemRoot;

		// Token: 0x04003D9C RID: 15772
		[SerializeField]
		private Refers attainmentItemTemplate;

		// Token: 0x02001C12 RID: 7186
		private struct AttainmentItem
		{
			// Token: 0x0600E5FC RID: 58876 RVA: 0x005EF250 File Offset: 0x005ED450
			public static VillagerRoleInfoNeed.AttainmentItem CreateCombat(sbyte type)
			{
				return new VillagerRoleInfoNeed.AttainmentItem
				{
					_isCombatSkill = true,
					_type = type
				};
			}

			// Token: 0x0600E5FD RID: 58877 RVA: 0x005EF27C File Offset: 0x005ED47C
			public static VillagerRoleInfoNeed.AttainmentItem CreateLife(sbyte type)
			{
				return new VillagerRoleInfoNeed.AttainmentItem
				{
					_isCombatSkill = false,
					_type = type
				};
			}

			// Token: 0x0600E5FE RID: 58878 RVA: 0x005EF2A8 File Offset: 0x005ED4A8
			public void SetIcon(CImage icon)
			{
				icon.SetSprite(this._isCombatSkill ? string.Format("{0}{1}", "ui9_back_combatskill_small_1_", this._type) : string.Format("{0}{1}", "ui9_icon_craftsmanship_big_2_", this._type), false, null);
			}

			// Token: 0x0600E5FF RID: 58879 RVA: 0x005EF2FD File Offset: 0x005ED4FD
			public void SetLabel(TextMeshProUGUI label)
			{
				label.text = (this._isCombatSkill ? LocalStringManager.Get(LanguageKey.LK_CombatSkill_2) : LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", this._type)));
			}

			// Token: 0x0400BF73 RID: 49011
			private bool _isCombatSkill;

			// Token: 0x0400BF74 RID: 49012
			private sbyte _type;
		}
	}
}
