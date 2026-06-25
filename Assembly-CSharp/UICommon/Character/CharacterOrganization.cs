using System;
using System.Text;
using CharacterDataMonitor;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005E1 RID: 1505
	public class CharacterOrganization : CharacterUIElement
	{
		// Token: 0x170008F6 RID: 2294
		// (get) Token: 0x06004716 RID: 18198 RVA: 0x00214977 File Offset: 0x00212B77
		private DetailInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as DetailInfoMonitor;
			}
		}

		// Token: 0x06004717 RID: 18199 RVA: 0x00214984 File Offset: 0x00212B84
		public CharacterOrganization(Refers orgRefers, Refers identityRefers)
		{
			bool flag = orgRefers == null && identityRefers == null;
			if (flag)
			{
				throw new Exception("orgRefer and gradeRefers can not be both null! failed to create CharacterSamsaraInfo element!");
			}
			bool flag2 = null != orgRefers;
			if (flag2)
			{
				this._organizationItem = new InfoItem(orgRefers);
				string orgInfoName = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Organization);
				this._organizationItem.SetInfoName(orgInfoName);
			}
			bool flag3 = null != identityRefers;
			if (flag3)
			{
				this._identityItem = new InfoItem(identityRefers);
				string gradeInfoName = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Identity);
				this._identityItem.SetInfoName(gradeInfoName);
			}
		}

		// Token: 0x06004718 RID: 18200 RVA: 0x00214A20 File Offset: 0x00212C20
		internal override void BindEvent()
		{
			this._basicMonitor.AddGenderListener(new Action(this.FillElement));
			this._ageHealthMonitor.AddActualAgeListener(new Action(this.FillElement));
			this.Item.AddOnOrganizationInfoListener(new Action(this.FillElement));
		}

		// Token: 0x06004719 RID: 18201 RVA: 0x00214A7C File Offset: 0x00212C7C
		public override void UnbindEvent()
		{
			this._basicMonitor.RemoveGenderListener(new Action(this.FillElement));
			this._ageHealthMonitor.RemoveActualAgeListener(new Action(this.FillElement));
			this.Item.RemoveOnOrganizationInfoListener(new Action(this.FillElement));
		}

		// Token: 0x0600471A RID: 18202 RVA: 0x00214AD8 File Offset: 0x00212CD8
		public override void FillElement()
		{
			InfoItem organizationItem = this._organizationItem;
			bool flag;
			if (organizationItem == null || !organizationItem.HasValidElement())
			{
				InfoItem identityItem = this._identityItem;
				flag = (identityItem == null || !identityItem.HasValidElement());
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				base.CharacterId = -1;
			}
			else
			{
				bool flag3 = this.Item == null || !this.Item.Init || !this._basicMonitor.Init || !this._ageHealthMonitor.Init;
				if (!flag3)
				{
					bool flag4 = this._organizationItem != null;
					if (flag4)
					{
						string text;
						this._organizationItem.SetInfoValue(CommonUtils.TryGetCharacterSpecialGradeName((int)this._ageHealthMonitor.TemplateId, out text) ? "-" : SingletonObject.getInstance<WorldMapModel>().GetSettlementName(this.Item.OrganizationInfo));
					}
					bool flag5 = this._identityItem != null;
					if (flag5)
					{
						this._identityItem.SetIcon(CommonUtils.GetIdentityIcon(this.Item.OrganizationInfo.Grade));
						string identityString = CommonUtils.GetIdentityStringWithSpecialCharacterConfig((int)this._ageHealthMonitor.TemplateId, this.Item.OrganizationInfo, this._basicMonitor.Gender, this._ageHealthMonitor.PhysiologicalAge, this.Item.IsReclusiveChar);
						this._identityItem.SetInfoValue(identityString);
					}
					bool flag6 = this._organizationItem != null || this._identityItem != null;
					if (flag6)
					{
						CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, base.CharacterId, delegate(int offset, RawDataPool dataPool)
						{
							bool flag7 = base.CharacterId < 0;
							if (!flag7)
							{
								CharacterDisplayData characterDisplayData = null;
								Serializer.Deserialize(dataPool, offset, ref characterDisplayData);
								this.RefreshTip(characterDisplayData);
							}
						});
					}
				}
			}
		}

		// Token: 0x0600471B RID: 18203 RVA: 0x00214C64 File Offset: 0x00212E64
		public override void ResetToEmpty()
		{
			bool flag = this._organizationItem != null && !this._organizationItem.HasValidElement();
			if (flag)
			{
				bool flag2 = this.MonitorDataItem != null;
				if (flag2)
				{
					this.UnbindEvent();
					this.MonitorDataItem = null;
				}
			}
			else
			{
				InfoItem organizationItem = this._organizationItem;
				if (organizationItem != null)
				{
					organizationItem.SetInfoValue(string.Empty);
				}
				bool flag3 = this._identityItem != null && !this._identityItem.HasValidElement();
				if (flag3)
				{
					bool flag4 = this.MonitorDataItem != null;
					if (flag4)
					{
						this.UnbindEvent();
						this.MonitorDataItem = null;
					}
				}
				else
				{
					InfoItem identityItem = this._identityItem;
					if (identityItem != null)
					{
						identityItem.SetInfoValue(string.Empty);
					}
				}
			}
		}

		// Token: 0x0600471C RID: 18204 RVA: 0x00214D1C File Offset: 0x00212F1C
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			bool flag = this._basicMonitor == null || this._basicMonitor.CharacterId != charId;
			if (flag)
			{
				this._basicMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(charId, this.IsDead);
			}
			bool flag2 = this._ageHealthMonitor == null || this._ageHealthMonitor.CharacterId != charId;
			if (flag2)
			{
				this._ageHealthMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AgeHealthMonitor>(charId, this.IsDead);
			}
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x0600471D RID: 18205 RVA: 0x00214DB0 File Offset: 0x00212FB0
		private bool IsLeiKun()
		{
			return this._ageHealthMonitor.TemplateId >= 691 && this._ageHealthMonitor.TemplateId <= 695;
		}

		// Token: 0x0600471E RID: 18206 RVA: 0x00214DEC File Offset: 0x00212FEC
		private void RefreshTip(CharacterDisplayData characterDisplayData)
		{
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			bool flag = this._organizationItem != null;
			if (flag)
			{
				TooltipInvoker orgTip = this._organizationItem.GetMouseTip();
				bool flag2 = null != orgTip;
				if (flag2)
				{
					ArgumentBox args = EasyPool.Get<ArgumentBox>().Set<CharacterDisplayData>("CharacterDisplayData", characterDisplayData);
					orgTip.Type = TipType.Organization;
					orgTip.enabled = true;
					orgTip.RuntimeParam = args;
				}
			}
			bool flag3 = this._identityItem != null;
			if (flag3)
			{
				TooltipInvoker orgGradeTip = this._identityItem.GetMouseTip();
				bool flag4 = null != orgGradeTip;
				if (flag4)
				{
					string gradeInfoName = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Identity);
					stringBuilder.Clear();
					stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Identity_TipContent));
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_Treasury_Fixed_Contribution, string.Format("+{0}", characterDisplayData.ContributionPerMonth).SetColor("pinkyellow")));
					OrganizationItem orgCfg = Organization.Instance[characterDisplayData.OrgInfo.OrgTemplateId];
					bool flag5 = orgCfg != null;
					if (flag5)
					{
						bool isChild = CommonUtils.CheckCharIsChild(characterDisplayData.OrgInfo, characterDisplayData.PhysiologicalAge);
						bool flag6 = isChild;
						if (flag6)
						{
							OrganizationMemberItem memberCfg = OrganizationMember.Instance[orgCfg.Members[(int)characterDisplayData.OrgInfo.Grade]];
							stringBuilder.Append(LanguageKey.LK_Main_SummaryInfo_Identity_PrefixDot.Tr());
							stringBuilder.AppendFormat(LanguageKey.LK_Main_SummaryInfo_Identity_Child.Tr(), new object[]
							{
								memberCfg.IdentityActiveAge,
								orgCfg.Name,
								characterDisplayData.OrgInfo.Grade,
								memberCfg.GradeName
							});
						}
						bool flag7 = characterDisplayData.OrgInfo.Grade > 0;
						bool flag8 = flag7;
						if (flag8)
						{
							sbyte templateId = orgCfg.TemplateId;
							if (templateId >= 21)
							{
								if (templateId > 35 && templateId - 36 > 2)
								{
									goto IL_1F9;
								}
							}
							else
							{
								if (templateId < 1)
								{
									goto IL_1F9;
								}
								if (templateId > 15)
								{
									goto IL_1F9;
								}
							}
							bool flag9 = true;
							goto IL_1FC;
							IL_1F9:
							flag9 = false;
							IL_1FC:
							flag8 = flag9;
						}
						bool flag10 = flag8;
						if (flag10)
						{
							bool flag11 = isChild;
							if (flag11)
							{
								stringBuilder.AppendLine();
							}
							stringBuilder.Append(LanguageKey.LK_Main_SummaryInfo_Identity_PrefixDot.Tr());
							bool flag12 = orgCfg.TemplateId == 12 && characterDisplayData.OrgInfo.Grade > 6;
							if (flag12)
							{
								stringBuilder.AppendLine((characterDisplayData.OrgInfo.Grade == 8) ? LanguageKey.LK_Main_SummaryInfo_Identity_Wuxian8.Tr() : LanguageKey.LK_Main_SummaryInfo_Identity_Wuxian7.Tr());
							}
							else
							{
								stringBuilder.AppendFormat((orgCfg.Hereditary ? LanguageKey.LK_Main_SummaryInfo_Identity_Inherit : LanguageKey.LK_Main_SummaryInfo_Identity_Normal).Tr(), new object[]
								{
									orgCfg.Name,
									characterDisplayData.OrgInfo.Grade,
									OrganizationMember.Instance[orgCfg.Members[(int)characterDisplayData.OrgInfo.Grade]].GradeName,
									(int)(characterDisplayData.OrgInfo.Grade - 1),
									OrganizationMember.Instance[orgCfg.Members[(int)(characterDisplayData.OrgInfo.Grade - 1)]].GradeName
								});
							}
						}
					}
					CommonUtils.AppendOrganizationMemberPotentialSuccessorIdentityTip(stringBuilder, characterDisplayData);
					string gradeInfoDesc = stringBuilder.ToString().ColorReplace();
					orgGradeTip.Type = TipType.Simple;
					orgGradeTip.enabled = true;
					orgGradeTip.IsLanguageKey = false;
					orgGradeTip.PresetParam = new string[]
					{
						gradeInfoName,
						gradeInfoDesc
					};
				}
			}
			EasyPool.Free<StringBuilder>(stringBuilder);
		}

		// Token: 0x04003122 RID: 12578
		private AgeHealthMonitor _ageHealthMonitor;

		// Token: 0x04003123 RID: 12579
		private BasicInfoMonitor _basicMonitor;

		// Token: 0x04003124 RID: 12580
		private readonly InfoItem _organizationItem;

		// Token: 0x04003125 RID: 12581
		private readonly InfoItem _identityItem;
	}
}
