using System;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000874 RID: 2164
	public class MouseTipProfessionSkillEncyclopedia : MouseTipBase
	{
		// Token: 0x17000C7C RID: 3196
		// (get) Token: 0x0600683A RID: 26682 RVA: 0x002FA456 File Offset: 0x002F8656
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600683B RID: 26683 RVA: 0x002FA45C File Offset: 0x002F865C
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get("ProfessionSkillId", out this._skillId);
			int skillTemplateId;
			bool flag = int.TryParse(this._skillId, out skillTemplateId);
			ProfessionSkillItem skillConfig;
			if (flag)
			{
				skillConfig = ProfessionSkill.Instance[skillTemplateId];
			}
			else
			{
				skillConfig = ProfessionSkill.Instance[this._skillId];
			}
			this.txtDesc.text = skillConfig.Desc.ColorReplace();
			this.txtTitle.text = skillConfig.Name;
			this.txtFuncDesc.text = skillConfig.FunctionalDesc.ColorReplace();
			this.txtFuncDesc.transform.GetComponent<TMPTextSpriteHelper>().Parse();
			bool isNotPassive = skillConfig.Type != EProfessionSkillType.Passive;
			this.coolDownLayout.SetActive(isNotPassive);
			this.otherText.gameObject.SetActive(false);
			this.coolDown.text = LocalStringManager.GetFormat(LanguageKey.LK_ProfessionSkill_CoolDown, skillConfig.SkillCoolDown).ColorReplace();
			bool showExp = skillConfig.ExpCost > 0;
			this.expHolder.SetActive(showExp);
			bool flag2 = showExp;
			if (flag2)
			{
				this.expCost.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_ProfessionTip_ExpCost, skillConfig.ExpCost).ColorReplace();
			}
			bool showTime = skillConfig.TimeCost > 0;
			this.timeLayout.SetActive(showTime);
			bool flag3 = showTime;
			if (flag3)
			{
				this.timeCost.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_ProfessionTip_TimeCost, skillConfig.TimeCost).ColorReplace();
			}
			bool showResources = false;
			int i;
			int j;
			for (i = 0; i < this.resourceHolder.transform.childCount; i = j + 1)
			{
				Transform child = this.resourceHolder.transform.GetChild(i);
				ResourceInfo resourceInfo = skillConfig.ResourcesCost.Find((ResourceInfo r) => (int)r.ResourceType == i && r.ResourceCount > 0);
				bool flag4 = (int)resourceInfo.ResourceType == i && resourceInfo.ResourceCount > 0;
				if (flag4)
				{
					showResources = true;
					ResourceTypeItem resourceConfig = ResourceType.Instance[i];
					Refers refers = child.GetComponent<Refers>();
					refers.CGet<CImage>("Icon").SetSprite(resourceConfig.Icon, false, null);
					refers.CGet<TextMeshProUGUI>("Name").SetText(resourceConfig.Name, true);
					refers.CGet<TextMeshProUGUI>("Value").text = resourceInfo.ResourceCount.ToString();
					child.gameObject.SetActive(true);
				}
				else
				{
					child.gameObject.SetActive(false);
				}
				j = i;
			}
			this.resourceHolder.SetActive(showResources);
			bool noneCost = !showTime && !showExp && !showResources;
			this.costLayout.SetActive(!noneCost);
			this.extraLineBuilder.Clear();
			int templateId = skillConfig.TemplateId;
			int num = templateId;
			if (num != 25)
			{
				if (num != 33)
				{
					if (num == 49)
					{
						this.extraLineBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_LegendaryInfected_TravelingBuddhistMonkSkill1_Tip).ColorReplace());
						this.<Init>g__RefreshNotMeetText|17_0(this.extraLineBuilder);
					}
				}
				else
				{
					this.extraLineBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_LegendaryInfected_AristocratSkill1_Tip).ColorReplace());
					this.<Init>g__RefreshNotMeetText|17_0(this.extraLineBuilder);
				}
			}
			else
			{
				this.extraLineBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_LegendaryInfected_BuddhistMonkSkill1_Tip).ColorReplace());
				this.<Init>g__RefreshNotMeetText|17_0(this.extraLineBuilder);
			}
		}

		// Token: 0x0600683D RID: 26685 RVA: 0x002FA81C File Offset: 0x002F8A1C
		[CompilerGenerated]
		private void <Init>g__RefreshNotMeetText|17_0(StringBuilder extraLineStringBuilder)
		{
			string content = extraLineStringBuilder.ToString();
			this.otherText.text = content;
			this.otherText.transform.GetComponent<TMPTextSpriteHelper>().Parse();
			this.otherText.gameObject.SetActive(!content.IsNullOrEmpty());
		}

		// Token: 0x040049C8 RID: 18888
		[SerializeField]
		private TextMeshProUGUI txtDesc;

		// Token: 0x040049C9 RID: 18889
		[SerializeField]
		private TextMeshProUGUI txtTitle;

		// Token: 0x040049CA RID: 18890
		[SerializeField]
		private TextMeshProUGUI txtFuncDesc;

		// Token: 0x040049CB RID: 18891
		[SerializeField]
		private TextMeshProUGUI otherText;

		// Token: 0x040049CC RID: 18892
		[SerializeField]
		private TextMeshProUGUI coolDown;

		// Token: 0x040049CD RID: 18893
		[SerializeField]
		private GameObject coolDownLayout;

		// Token: 0x040049CE RID: 18894
		[SerializeField]
		private GameObject timeLayout;

		// Token: 0x040049CF RID: 18895
		[SerializeField]
		private TextMeshProUGUI timeCost;

		// Token: 0x040049D0 RID: 18896
		[SerializeField]
		private GameObject resourceHolder;

		// Token: 0x040049D1 RID: 18897
		[SerializeField]
		private GameObject expHolder;

		// Token: 0x040049D2 RID: 18898
		[SerializeField]
		private TextMeshProUGUI expCost;

		// Token: 0x040049D3 RID: 18899
		[SerializeField]
		private GameObject costLayout;

		// Token: 0x040049D4 RID: 18900
		private string _skillId;

		// Token: 0x040049D5 RID: 18901
		private int _skillEffectCount;

		// Token: 0x040049D6 RID: 18902
		private StringBuilder extraLineBuilder = new StringBuilder();
	}
}
