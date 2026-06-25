using System;
using System.Text;
using Config;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F2B RID: 3883
	public class Identity : MonoBehaviour
	{
		// Token: 0x0600B297 RID: 45719 RVA: 0x00514298 File Offset: 0x00512498
		public void Set(CharacterDisplayData data, bool isReclusiveChar = false, bool isShowBack = true, bool isShowOrganization = false)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this.Set(data.OrgInfo, (int)data.TemplateId, data.Gender, data.PhysiologicalAge, isReclusiveChar, data, isShowOrganization);
				this.propertyItem.SetShowBack(isShowBack);
			}
		}

		// Token: 0x0600B298 RID: 45720 RVA: 0x005142E8 File Offset: 0x005124E8
		public void Set(OrganizationInfo orgInfo, int templateId, sbyte gender, short physiologicalAge, bool isReclusiveChar, CharacterDisplayData data = null, bool isShowOrganization = false)
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				int iconIndex = CommonUtils.GetIdentityIconIndex(orgInfo.Grade);
				string identityString = CommonUtils.GetIdentityStringWithSpecialCharacterConfig(templateId, orgInfo, gender, physiologicalAge, isReclusiveChar);
				if (isShowOrganization)
				{
					identityString = CommonUtils.GetOrganizationGradeString(orgInfo, gender, physiologicalAge, templateId);
				}
				this.propertyItem.Set(this.sprites[iconIndex], LanguageKey.LK_Main_SummaryInfo_Identity.Tr(), identityString, null, false);
			}
			bool flag2 = this.identityTip != null && data != null;
			if (flag2)
			{
				this.identityTip.Type = TipType.Simple;
				this.identityTip.enabled = true;
				this.identityTip.IsLanguageKey = false;
				string gradeInfoName = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Identity);
				string gradeInfoDesc = Identity.BuildIdentityTipContent(data);
				this.identityTip.PresetParam = new string[]
				{
					gradeInfoName,
					gradeInfoDesc
				};
			}
		}

		// Token: 0x0600B299 RID: 45721 RVA: 0x005143D0 File Offset: 0x005125D0
		public void SetEmpty()
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				this.propertyItem.Set(string.Empty, string.Empty, string.Empty, null, false);
			}
			bool flag2 = this.identityTip != null;
			if (flag2)
			{
				this.identityTip.enabled = false;
			}
		}

		// Token: 0x0600B29A RID: 45722 RVA: 0x00514430 File Offset: 0x00512630
		private static string BuildIdentityTipContent(CharacterDisplayData data)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Identity_TipContent));
			sb.AppendLine();
			sb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_Treasury_Fixed_Contribution, string.Format("+{0}", data.ContributionPerMonth).SetColor("pinkyellow")));
			OrganizationItem orgCfg = Organization.Instance[data.OrgInfo.OrgTemplateId];
			bool flag = orgCfg != null;
			if (flag)
			{
				bool isChild = CommonUtils.CheckCharIsChild(data.OrgInfo, data.PhysiologicalAge);
				bool flag2 = isChild;
				if (flag2)
				{
					OrganizationMemberItem memberCfg = OrganizationMember.Instance[orgCfg.Members[(int)data.OrgInfo.Grade]];
					sb.Append(LanguageKey.LK_Main_SummaryInfo_Identity_PrefixDot.Tr());
					sb.AppendFormat(LanguageKey.LK_Main_SummaryInfo_Identity_Child.Tr(), new object[]
					{
						memberCfg.IdentityActiveAge,
						orgCfg.Name,
						data.OrgInfo.Grade,
						memberCfg.GradeName
					});
				}
				bool flag3 = data.OrgInfo.Grade > 0;
				bool flag4 = flag3;
				if (flag4)
				{
					sbyte templateId = orgCfg.TemplateId;
					if (templateId >= 21)
					{
						if (templateId > 35 && templateId - 36 > 2)
						{
							goto IL_14F;
						}
					}
					else
					{
						if (templateId < 1)
						{
							goto IL_14F;
						}
						if (templateId > 15)
						{
							goto IL_14F;
						}
					}
					bool flag5 = true;
					goto IL_152;
					IL_14F:
					flag5 = false;
					IL_152:
					flag4 = flag5;
				}
				bool flag6 = flag4;
				if (flag6)
				{
					bool flag7 = isChild;
					if (flag7)
					{
						sb.AppendLine();
					}
					sb.Append(LanguageKey.LK_Main_SummaryInfo_Identity_PrefixDot.Tr());
					bool flag8 = orgCfg.TemplateId == 12 && data.OrgInfo.Grade > 6;
					if (flag8)
					{
						sb.AppendLine((data.OrgInfo.Grade == 8) ? LanguageKey.LK_Main_SummaryInfo_Identity_Wuxian8.Tr() : LanguageKey.LK_Main_SummaryInfo_Identity_Wuxian7.Tr());
					}
					else
					{
						sb.AppendFormat((orgCfg.Hereditary ? LanguageKey.LK_Main_SummaryInfo_Identity_Inherit : LanguageKey.LK_Main_SummaryInfo_Identity_Normal).Tr(), new object[]
						{
							orgCfg.Name,
							data.OrgInfo.Grade,
							OrganizationMember.Instance[orgCfg.Members[(int)data.OrgInfo.Grade]].GradeName,
							(int)(data.OrgInfo.Grade - 1),
							OrganizationMember.Instance[orgCfg.Members[(int)(data.OrgInfo.Grade - 1)]].GradeName
						});
					}
				}
			}
			CommonUtils.AppendOrganizationMemberPotentialSuccessorIdentityTip(sb, data);
			bool flag9 = data.MerchantExpData != null && data.MerchantType >= 0;
			if (flag9)
			{
				string merchantTypeName = MerchantType.Instance[data.MerchantType].Name.SetColor("brightblue");
				string typeStr = LanguageKey.LK_Main_SummaryInfo_Identity_Merchant_Type.TrFormat(merchantTypeName);
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine(typeStr);
				sb.AppendLine(LanguageKey.LK_Main_SummaryInfo_Identity_Merchant_Favorability.Tr());
				sbyte index = 0;
				while ((int)index < data.MerchantExpData.Favorabilitys.Length)
				{
					int favorability = data.MerchantExpData.Favorabilitys[(int)index];
					LanguageKey merchantTypeKey = LanguageKey.LK_Main_SummaryInfo_Identity_Merchant_Favorability_0 + (int)index;
					sbyte merchantLevel = data.MerchantExpData.GetMerchantLevel(index);
					LanguageKey merchantLevelStr = CommonUtils.TraditionalChineseNumber[(int)(merchantLevel + 1)];
					string content = merchantTypeKey.TrFormat(favorability, merchantLevelStr.Tr());
					bool isCurrent = data.MerchantType == index;
					bool flag10 = isCurrent;
					if (flag10)
					{
						content = content.SetColor("brightblue");
					}
					sb.AppendLine(content);
					index += 1;
				}
			}
			return sb.ToString().ColorReplace();
		}

		// Token: 0x04008A97 RID: 35479
		[Header("身份组件")]
		[SerializeField]
		private PropertyItem propertyItem;

		// Token: 0x04008A98 RID: 35480
		[SerializeField]
		private TooltipInvoker identityTip;

		// Token: 0x04008A99 RID: 35481
		[Header("身份图标")]
		[SerializeField]
		private Sprite[] sprites;
	}
}
