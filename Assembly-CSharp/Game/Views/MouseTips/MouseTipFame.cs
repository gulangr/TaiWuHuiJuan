using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Components.Common;
using Game.Views.Encyclopedia;
using GameData.Domains.Character;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000853 RID: 2131
	public class MouseTipFame : MouseTipBase
	{
		// Token: 0x06006788 RID: 26504 RVA: 0x002F4A8C File Offset: 0x002F2C8C
		protected override void Init(ArgumentBox argBox)
		{
			this.Element.ForceListenCommand = true;
			IReadOnlyList<short> featureIds;
			argBox.Get<IReadOnlyList<short>>("FeatureIds", out featureIds);
			IReadOnlyList<FameActionRecord> currentFameRecords;
			argBox.Get<IReadOnlyList<FameActionRecord>>("FameRecords", out currentFameRecords);
			OrganizationInfo orgInfo;
			argBox.Get<OrganizationInfo>("OrgInfo", out orgInfo);
			bool isTaiwu;
			argBox.Get("IsTaiwu", out isTaiwu);
			bool hasContent = false;
			bool flag = featureIds != null;
			if (flag)
			{
				foreach (short featureId in featureIds)
				{
					int fameChange = SharedMethods.GetSectFeatureFameBonus(featureId, isTaiwu, orgInfo);
					bool flag2 = fameChange == 0;
					if (!flag2)
					{
						hasContent = true;
						this.featureRecord.SetText(new string[]
						{
							LanguageKey.LK_Fame_Tip_Record_New.TrFormat(CharacterFeature.Instance[featureId].Name, CommonUtils.GetColoredValue(fameChange, true, 0, true, false)),
							"-"
						});
					}
				}
			}
			this.featureRecord.gameObject.SetActive(hasContent);
			int currDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
			FameActionRecord[] records = (from record in currentFameRecords
			where record.Id >= 0 && record.Value != 0 && record.EndDate > currDate && FameAction.Instance[record.Id] != null
			select record).ToArray<FameActionRecord>();
			hasContent |= (records.Length != 0);
			this.baseValue.Rebuild<ImagesAndTexts>(records.Length, delegate(ImagesAndTexts texts, int i)
			{
				FameActionRecord record = records[i];
				FameActionItem config = FameAction.Instance[record.Id];
				texts.SetText(new string[]
				{
					LanguageKey.LK_Fame_Tip_Record_New.TrFormat(config.Name, CommonUtils.GetColoredValue((int)record.Value, true, 0, true, false)),
					(record.EndDate - currDate).ToString()
				});
			});
			ValueTuple<int, int, int, int, bool, bool> rawFame = SharedMethods.GetRawFame(featureIds, currentFameRecords, orgInfo, currDate, isTaiwu);
			this.positiveBuff.gameObject.SetActive(rawFame.Item5);
			bool item = rawFame.Item5;
			if (item)
			{
				hasContent = true;
				int coef = Math.Max(rawFame.Item3, 0) - 100;
				this.positiveTitle.text = LanguageKey.LK_Main_SummaryInfo_Fame_Positive_Summary.TrFormat(CommonUtils.GetColoredValue(rawFame.Item1 * coef / 100, true, 0, true, false), CommonUtils.GetColoredValue(coef, true, 0, true, true)).ColorReplace();
				FameActionRecord[] recordsGood = currentFameRecords.Where(delegate(FameActionRecord record)
				{
					bool result;
					if (record.Id >= 0 && record.EndDate > currDate)
					{
						FameActionItem fameActionItem = FameAction.Instance[record.Id];
						result = (fameActionItem != null && fameActionItem.PositiveFameBonus != 0);
					}
					else
					{
						result = false;
					}
					return result;
				}).ToArray<FameActionRecord>();
				this.positiveBuff.Rebuild<ImagesAndTexts>(recordsGood.Length, delegate(ImagesAndTexts texts, int i)
				{
					FameActionRecord record = recordsGood[i];
					FameActionItem config = FameAction.Instance[record.Id];
					texts.SetText(new string[]
					{
						LanguageKey.LK_Fame_Tip_Record_Second.TrFormat(config.Name, CommonUtils.GetColoredValue(config.PositiveFameBonus, true, 0, true, true)),
						(record.EndDate - currDate).ToString()
					});
				});
			}
			this.negativeBuff.gameObject.SetActive(rawFame.Item6);
			bool item2 = rawFame.Item6;
			if (item2)
			{
				hasContent = true;
				int coef2 = Math.Max(rawFame.Item4, 0) - 100;
				this.negativeTitle.text = LanguageKey.LK_Main_SummaryInfo_Fame_Negative_Summary.TrFormat(CommonUtils.GetColoredValue(rawFame.Item2 * coef2 / 100, true, 0, true, false), CommonUtils.GetColoredValue(coef2, false, 0, true, true)).ColorReplace();
				FameActionRecord[] recordsBad = currentFameRecords.Where(delegate(FameActionRecord record)
				{
					bool result;
					if (record.Id >= 0 && record.EndDate > currDate)
					{
						FameActionItem fameActionItem = FameAction.Instance[record.Id];
						result = (fameActionItem != null && fameActionItem.NegativeFameBonus != 0);
					}
					else
					{
						result = false;
					}
					return result;
				}).ToArray<FameActionRecord>();
				this.negativeBuff.Rebuild<ImagesAndTexts>(recordsBad.Length, delegate(ImagesAndTexts texts, int i)
				{
					FameActionRecord record = recordsBad[i];
					FameActionItem config = FameAction.Instance[record.Id];
					texts.SetText(new string[]
					{
						LanguageKey.LK_Fame_Tip_Record_Second.TrFormat(config.Name, CommonUtils.GetColoredValue(config.NegativeFameBonus, false, 0, true, true)),
						(record.EndDate - currDate).ToString()
					});
				});
			}
			this.details.gameObject.SetActive(hasContent);
			this.encyclopedia.Refresh(EHotKeyDisplayType.ViewEncyclopedia);
		}

		// Token: 0x06006789 RID: 26505 RVA: 0x002F4DD4 File Offset: 0x002F2FD4
		private void Update()
		{
			bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.Fame);
			}
		}

		// Token: 0x04004918 RID: 18712
		[SerializeField]
		private TemplatedContainerAssemblyNew baseValue;

		// Token: 0x04004919 RID: 18713
		[SerializeField]
		private TemplatedContainerAssemblyNew positiveBuff;

		// Token: 0x0400491A RID: 18714
		[SerializeField]
		private TemplatedContainerAssemblyNew negativeBuff;

		// Token: 0x0400491B RID: 18715
		[SerializeField]
		private TMP_Text positiveTitle;

		// Token: 0x0400491C RID: 18716
		[SerializeField]
		private TMP_Text negativeTitle;

		// Token: 0x0400491D RID: 18717
		[SerializeField]
		private ImagesAndTexts featureRecord;

		// Token: 0x0400491E RID: 18718
		[SerializeField]
		private GameObject details;

		// Token: 0x0400491F RID: 18719
		[SerializeField]
		private Game.Components.Common.HotkeyDisplay encyclopedia;
	}
}
