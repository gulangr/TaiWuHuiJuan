using System;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SettlementInformation
{
	// Token: 0x0200078E RID: 1934
	public class SettlementInformationGradeInfo : MonoBehaviour
	{
		// Token: 0x06005DCF RID: 24015 RVA: 0x002B228E File Offset: 0x002B048E
		public void SetCharContainerExpandStatus(bool isExpand)
		{
			this.charContainerLayoutElement.preferredHeight = (isExpand ? this.expandHeight : this.collapsedHeight);
		}

		// Token: 0x06005DD0 RID: 24016 RVA: 0x002B22AE File Offset: 0x002B04AE
		private void Awake()
		{
			this.toggle.onValueChanged.ResetListener(delegate(bool isOn)
			{
				this.selected.SetActive(isOn);
				this.label.text = this._text.SetColor(isOn ? "black" : "brightyellow");
			});
		}

		// Token: 0x06005DD1 RID: 24017 RVA: 0x002B22D0 File Offset: 0x002B04D0
		public void SetTaiwu(OrganizationItem configData, sbyte grade, int count)
		{
			bool flag = count != -1;
			if (flag)
			{
				this.curCount = count;
			}
			this._text = string.Format("{0}{1}{2}", OrganizationMember.Instance[configData.Members[(int)grade]].GradeName, LanguageKey.LK_Colon_Symbol.Tr(), this.curCount);
			this.label.text = this._text.SetColor(this.toggle.isOn ? "black" : "brightyellow");
			this.displayer.enabled = false;
		}

		// Token: 0x06005DD2 RID: 24018 RVA: 0x002B236C File Offset: 0x002B056C
		public void Set(OrganizationItem configData, short orgId, sbyte grade, int currentCount, bool isUp, bool isDown)
		{
			this.UpdateGradeTips((int)orgId, (int)grade);
			base.gameObject.SetActive(true);
			OrganizationMemberItem organizationMemberConfig = OrganizationMember.Instance[configData.Members[(int)grade]];
			sbyte maxCount = organizationMemberConfig.Amount;
			if (isUp)
			{
				maxCount = organizationMemberConfig.UpAmount;
			}
			else if (isDown)
			{
				maxCount = organizationMemberConfig.DownAmount;
			}
			string format = "{0}({1}/{2})";
			object gradeName = organizationMemberConfig.GradeName;
			this.curCount = currentCount;
			this._text = string.Format(format, gradeName, currentCount, maxCount);
			this.label.text = this._text.SetColor(this.toggle.isOn ? "black" : "brightyellow");
		}

		// Token: 0x06005DD3 RID: 24019 RVA: 0x002B2424 File Offset: 0x002B0624
		private void UpdateGradeTips(int orgTemplateId, int grade)
		{
			bool flag = this.displayer == null;
			if (!flag)
			{
				bool flag2 = grade > 0;
				bool flag3 = flag2;
				OrganizationItem orgCfg;
				if (flag3)
				{
					orgCfg = Organization.Instance[orgTemplateId];
					bool flag4;
					if (orgCfg != null)
					{
						sbyte templateId = orgCfg.TemplateId;
						if (templateId >= 21)
						{
							if (templateId > 35 && templateId - 36 > 2)
							{
								goto IL_5E;
							}
						}
						else
						{
							if (templateId < 1)
							{
								goto IL_5E;
							}
							if (templateId > 15)
							{
								goto IL_5E;
							}
						}
						flag4 = true;
						goto IL_61;
					}
					IL_5E:
					flag4 = false;
					IL_61:
					flag3 = flag4;
				}
				bool flag5 = flag3;
				if (flag5)
				{
					this.displayer.Type = TipType.Simple;
					this.displayer.enabled = true;
					this.displayer.IsLanguageKey = false;
					bool flag6 = this.displayer.PresetParam.Length != 2;
					if (flag6)
					{
						this.displayer.PresetParam = new string[2];
					}
					this.displayer.PresetParam[0] = string.Format("{0}<color=#GradeColor_{1}>{2}</color>", orgCfg.Name, grade, OrganizationMember.Instance[orgCfg.Members[grade]].GradeName).ColorReplace();
					this.displayer.PresetParam[1] = ((orgCfg.TemplateId == 12 && grade > 6) ? ((grade == 8) ? LanguageKey.LK_Main_SummaryInfo_Identity_Wuxian8.Tr() : LanguageKey.LK_Main_SummaryInfo_Identity_Wuxian7.Tr()) : (orgCfg.Hereditary ? LanguageKey.LK_Main_SummaryInfo_Identity_Inherit : LanguageKey.LK_Main_SummaryInfo_Identity_Normal).TrFormat(new object[]
					{
						orgCfg.Name,
						grade,
						OrganizationMember.Instance[orgCfg.Members[grade]].GradeName,
						grade - 1,
						OrganizationMember.Instance[orgCfg.Members[grade - 1]].GradeName
					})).ColorReplace();
				}
				else
				{
					this.displayer.enabled = false;
				}
			}
		}

		// Token: 0x0400409C RID: 16540
		[SerializeField]
		private TMP_Text label;

		// Token: 0x0400409D RID: 16541
		[SerializeField]
		private TooltipInvoker displayer;

		// Token: 0x0400409E RID: 16542
		[SerializeField]
		private CToggle toggle;

		// Token: 0x0400409F RID: 16543
		[SerializeField]
		private GameObject selected;

		// Token: 0x040040A0 RID: 16544
		[SerializeField]
		private float expandHeight = 380f;

		// Token: 0x040040A1 RID: 16545
		[SerializeField]
		private float collapsedHeight = 184f;

		// Token: 0x040040A2 RID: 16546
		[SerializeField]
		private LayoutElement charContainerLayoutElement;

		// Token: 0x040040A3 RID: 16547
		[SerializeField]
		internal RectTransform charContainer;

		// Token: 0x040040A4 RID: 16548
		[SerializeField]
		internal int curCount;

		// Token: 0x040040A5 RID: 16549
		private string _text;
	}
}
