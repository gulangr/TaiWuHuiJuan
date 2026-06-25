using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.TaiwuLifeSummary;
using GameData.DLC;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.GameLineScroll
{
	// Token: 0x02000A1D RID: 2589
	public class TaiwuScrollItem : MonoBehaviour
	{
		// Token: 0x06007EFE RID: 32510 RVA: 0x003B2BC1 File Offset: 0x003B0DC1
		private void Awake()
		{
			this.taiwuAvatar.gameObject.SetActive(false);
		}

		// Token: 0x06007EFF RID: 32511 RVA: 0x003B2BD8 File Offset: 0x003B0DD8
		private void ApplySummaryItems()
		{
			foreach (ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]> valueTuple in this._summaryItems)
			{
				TaiwuSummaryHolderItem holder = valueTuple.Item1;
				TaiwuLifeSummaryTypeItem[] types = valueTuple.Item2;
				holder.Set(types, this._taiwuLifeSummary, "");
			}
		}

		// Token: 0x17000DC1 RID: 3521
		// (get) Token: 0x06007F00 RID: 32512 RVA: 0x003B2C48 File Offset: 0x003B0E48
		private TextMeshProUGUI[] TaiwuIndexLabels
		{
			get
			{
				return new TextMeshProUGUI[]
				{
					this.relationTaiwuIndexLabel,
					this.combatTaiwuIndexLabel,
					this.lifeSkillTaiwuIndexLabel,
					this.combatSkillTaiwuIndexLabel,
					this.buildingTaiwuIndexLabel,
					this.professionTaiwuIndexLabel,
					this.travelTaiwuIndexLabel,
					this.invasionTaiwuIndexLabel,
					this.swordGraveTaiwuIndexLabel
				};
			}
		}

		// Token: 0x06007F01 RID: 32513 RVA: 0x003B2CB0 File Offset: 0x003B0EB0
		public void Set(int index, CharacterDisplayData characterDisplayData, TaiwuLifeSummary taiwuLifeSummary)
		{
			this._taiwuLifeSummary = taiwuLifeSummary;
			this.SetTaiwuIndexLabels(index);
			this.SetCombatData();
			this.SetLifeSkillData();
			this.SetCombatSkillData();
			this.SetBuildingData();
			this.SetProfessionData();
			this.SetTravelData();
			this.SetInvasionData();
			this.SetSwordGraveData();
			this.SetRelationData(characterDisplayData, taiwuLifeSummary);
		}

		// Token: 0x06007F02 RID: 32514 RVA: 0x003B2D10 File Offset: 0x003B0F10
		public void Set(int index, TaiwuLifeSummaryInfo taiwuLifeSummaryInfo, int currSaveSlot)
		{
			this._taiwuLifeSummary = taiwuLifeSummaryInfo.TaiwuLifeSummaryData;
			this.SetTaiwuIndexLabels(index);
			this.SetCombatData();
			this.SetLifeSkillData();
			this.SetCombatSkillData();
			this.SetBuildingData();
			this.SetProfessionData();
			this.SetTravelData();
			this.SetInvasionData();
			this.SetSwordGraveData();
			this.SetRelationData(taiwuLifeSummaryInfo, currSaveSlot);
		}

		// Token: 0x06007F03 RID: 32515 RVA: 0x003B2D74 File Offset: 0x003B0F74
		private bool CheckRemovedSpecialDlc(WorldInfo worldInfo, List<DlcId> warningDlcIdList, out string names)
		{
			names = string.Empty;
			warningDlcIdList.Clear();
			this._removedDlcList.Clear();
			bool flag = ((worldInfo != null) ? worldInfo.DlcIds : null) == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				List<DlcId> dlcIdList = SingletonObject.getInstance<DlcManager>().GetDlcIdList();
				this._removedDlcList.AddRange(from dlc in worldInfo.DlcIds
				where dlcIdList.All((DlcId d) => d.AppId != dlc.AppId)
				select dlc);
				warningDlcIdList.AddRange(this._removedDlcList);
				bool needWarning = this._removedDlcList.Count > 0;
				bool flag2 = needWarning;
				if (flag2)
				{
					names = this.GetDlcNames(warningDlcIdList);
				}
				result = needWarning;
			}
			return result;
		}

		// Token: 0x06007F04 RID: 32516 RVA: 0x003B2E20 File Offset: 0x003B1020
		private string GetDlcNames(List<DlcId> warningDlcIdList)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			for (int i = 0; i < warningDlcIdList.Count; i++)
			{
				DlcId dlcId = warningDlcIdList[i];
				string dlcName = SingletonObject.getInstance<DlcManager>().GetDlcName(dlcId).SetColor("orange");
				sb.Append(dlcName);
				bool flag = i < warningDlcIdList.Count - 1;
				if (flag)
				{
					sb.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
				}
			}
			string result = sb.ToString();
			EasyPool.Free<StringBuilder>(sb);
			return result;
		}

		// Token: 0x06007F05 RID: 32517 RVA: 0x003B2EAC File Offset: 0x003B10AC
		public void SetTaiwuIndexLabels(int index)
		{
			string ordinalText = TaiwuScrollItem.GetTaiwuOrdinalDisplayText(index + 1);
			foreach (TextMeshProUGUI label in this.TaiwuIndexLabels)
			{
				label.text = LanguageKey.LK_TaiwuSummary_Scroll_Index.TrFormat(ordinalText);
			}
		}

		// Token: 0x06007F06 RID: 32518 RVA: 0x003B2EF0 File Offset: 0x003B10F0
		public static string GetTaiwuOrdinalDisplayText(int oneBasedNumber)
		{
			bool flag = oneBasedNumber <= 0;
			string result;
			if (flag)
			{
				result = LocalStringManager.Get(CommonUtils.DigitLanguageKeys[0]);
			}
			else
			{
				List<string> parts = new List<string>();
				for (int i = oneBasedNumber; i > 0; i /= 10)
				{
					int d = i % 10;
					parts.Add(LocalStringManager.Get(CommonUtils.DigitLanguageKeys[d]));
				}
				parts.Reverse();
				result = string.Concat(parts);
			}
			return result;
		}

		// Token: 0x06007F07 RID: 32519 RVA: 0x003B2F60 File Offset: 0x003B1160
		public void SetRelationData(CharacterDisplayData characterDisplayData, TaiwuLifeSummary taiwuLifeSummaryData)
		{
			bool flag = characterDisplayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			if (flag)
			{
				this.taiwuAvatar.Refresh(characterDisplayData, false);
			}
			else
			{
				Game.Components.Avatar.Avatar avatar = this.taiwuAvatar;
				AbridgedCharacter abridgedCharacter = taiwuLifeSummaryData.AbridgedCharacter;
				avatar.Refresh(((abridgedCharacter != null) ? abridgedCharacter.GenerateAvatarRelatedData() : null) ?? characterDisplayData.AvatarRelatedData);
			}
			this.taiwuAvatar.gameObject.SetActive(true);
			this.nameLabel.text = NameCenter.GetNameByDisplayData(characterDisplayData, true, false);
			CharacterDomainMethod.AsyncCall.GetTitles(null, characterDisplayData.CharacterId, delegate(int offset, RawDataPool pool)
			{
				List<short> titles = new List<short>();
				Serializer.Deserialize(pool, offset, ref titles);
				this.taiwuTitleLabel.text = ((titles != null && titles.Count > 0) ? CharacterTitle.Instance[titles[0]].Name : string.Empty);
			});
			int index = 0;
			int count = 0;
			foreach (TaiwuLifeSummaryTypeItem item in ((IEnumerable<TaiwuLifeSummaryTypeItem>)TaiwuLifeSummaryType.Instance))
			{
				bool flag2 = item.Type != 0;
				if (!flag2)
				{
					int value = this._taiwuLifeSummary.Get(item.TemplateId);
					this.relationCountLabels[index].text = LanguageKey.LK_TaiwuSummary_Format.TrFormat(TaiwuScrollItem.RelationCountLabelsKeys[index].Tr(), value.ToString());
					index++;
					count += value;
				}
			}
			this.relationTotalCountLabel.text = LanguageKey.LK_TaiwuSummary_Scroll_Relation.Tr();
		}

		// Token: 0x06007F08 RID: 32520 RVA: 0x003B30AC File Offset: 0x003B12AC
		private ArchiveInfo GetArchiveInfo(int index)
		{
			return GlobalOperations.ArchivesInfo.CheckIndex(index) ? GlobalOperations.ArchivesInfo[index] : null;
		}

		// Token: 0x06007F09 RID: 32521 RVA: 0x003B30D8 File Offset: 0x003B12D8
		public void SetRelationData(TaiwuLifeSummaryInfo taiwuLifeSummaryInfo, int currSaveSlot)
		{
			List<DlcId> warningDlcIdList = new List<DlcId>();
			ArchiveInfo archiveInfo = this.GetArchiveInfo(currSaveSlot);
			string names;
			bool removedSpecialDlc = this.CheckRemovedSpecialDlc((archiveInfo != null) ? archiveInfo.WorldInfo : null, warningDlcIdList, out names);
			this.taiwuAvatar.gameObject.SetActive(!removedSpecialDlc);
			bool flag = !removedSpecialDlc;
			if (flag)
			{
				this.taiwuAvatar.Refresh(taiwuLifeSummaryInfo.GetAvatarRelatedData());
			}
			this.nameLabel.text = NameCenter.FormatName(taiwuLifeSummaryInfo.Surname, taiwuLifeSummaryInfo.GivenName);
			this.taiwuTitleLabel.text = ((taiwuLifeSummaryInfo.TitleId >= 0) ? CharacterTitle.Instance[taiwuLifeSummaryInfo.TitleId].Name : string.Empty);
			int index = 0;
			int count = 0;
			foreach (TaiwuLifeSummaryTypeItem item in ((IEnumerable<TaiwuLifeSummaryTypeItem>)TaiwuLifeSummaryType.Instance))
			{
				bool flag2 = item.Type != 0;
				if (!flag2)
				{
					int value = this._taiwuLifeSummary.Get(item.TemplateId);
					this.relationCountLabels[index].text = LanguageKey.LK_TaiwuSummary_Format.TrFormat(TaiwuScrollItem.RelationCountLabelsKeys[index].Tr(), value.ToString());
					index++;
					count += value;
				}
			}
			this.relationTotalCountLabel.text = LanguageKey.LK_TaiwuSummary_Scroll_Relation.Tr();
		}

		// Token: 0x06007F0A RID: 32522 RVA: 0x003B3244 File Offset: 0x003B1444
		public void SetCombatData()
		{
			this._summaryItems.Clear();
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.combatCountSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[10],
				TaiwuLifeSummaryType.Instance[9],
				TaiwuLifeSummaryType.Instance[11]
			}));
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.combatTotalCountSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[8]
			}));
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.savedInfectedCountSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[12]
			}));
			this.ApplySummaryItems();
		}

		// Token: 0x06007F0B RID: 32523 RVA: 0x003B3308 File Offset: 0x003B1508
		public void SetLifeSkillData()
		{
			this._summaryItems.Clear();
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.newLifeSkillSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[41]
			}));
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.debateWinCountSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[39]
			}));
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.readStrategySummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[45]
			}));
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.interpretLifeSkillSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[42],
				TaiwuLifeSummaryType.Instance[43]
			}));
			this.ApplySummaryItems();
		}

		// Token: 0x06007F0C RID: 32524 RVA: 0x003B33E8 File Offset: 0x003B15E8
		public void SetCombatSkillData()
		{
			this._summaryItems.Clear();
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.interpretCombatSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[47],
				TaiwuLifeSummaryType.Instance[48]
			}));
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.newCombatSkillSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[46]
			}));
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.breakSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[50],
				TaiwuLifeSummaryType.Instance[51]
			}));
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.loopingStrategySummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[53]
			}));
			this.ApplySummaryItems();
		}

		// Token: 0x06007F0D RID: 32525 RVA: 0x003B34D8 File Offset: 0x003B16D8
		public void SetBuildingData()
		{
			this._summaryItems.Clear();
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.buildingSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[54],
				TaiwuLifeSummaryType.Instance[55],
				TaiwuLifeSummaryType.Instance[56],
				TaiwuLifeSummaryType.Instance[66],
				TaiwuLifeSummaryType.Instance[67]
			}));
			this.ApplySummaryItems();
		}

		// Token: 0x06007F0E RID: 32526 RVA: 0x003B3564 File Offset: 0x003B1764
		public void SetProfessionData()
		{
			this._summaryItems.Clear();
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.professionSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[69],
				TaiwuLifeSummaryType.Instance[70],
				TaiwuLifeSummaryType.Instance[71]
			}));
			this.ApplySummaryItems();
		}

		// Token: 0x06007F0F RID: 32527 RVA: 0x003B35D0 File Offset: 0x003B17D0
		public void SetTravelData()
		{
			this._summaryItems.Clear();
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.cricketSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[72],
				TaiwuLifeSummaryType.Instance[74]
			}));
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.informationSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[75]
			}));
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.adventureSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[78]
			}));
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.travelSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[76],
				TaiwuLifeSummaryType.Instance[77]
			}));
			this.ApplySummaryItems();
		}

		// Token: 0x06007F10 RID: 32528 RVA: 0x003B36C0 File Offset: 0x003B18C0
		public void SetInvasionData()
		{
			this.invasionTitleLabel.text = LanguageKey.LK_TaiwuSummary_Format.TrFormat(LanguageKey.LK_TaiwuSummary_Type_8.Tr(), this._taiwuLifeSummary.Get(85).ToString());
		}

		// Token: 0x06007F11 RID: 32529 RVA: 0x003B3704 File Offset: 0x003B1904
		public void SetSwordGraveData()
		{
			int count = 0;
			for (int i = 86; i < 95; i++)
			{
				bool flag = this._taiwuLifeSummary.Contains(i);
				if (flag)
				{
					count++;
				}
			}
			this.swordGraveTitleLabel.text = LanguageKey.LK_TaiwuSummary_Format.TrFormat(LanguageKey.LK_TaiwuSummary_Type_9.Tr(), count.ToString());
			this._summaryItems.Clear();
			this._summaryItems.Add(new ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>(this.swordGraveSummaryHolder, new TaiwuLifeSummaryTypeItem[]
			{
				TaiwuLifeSummaryType.Instance[86],
				TaiwuLifeSummaryType.Instance[87],
				TaiwuLifeSummaryType.Instance[88],
				TaiwuLifeSummaryType.Instance[89],
				TaiwuLifeSummaryType.Instance[90],
				TaiwuLifeSummaryType.Instance[91],
				TaiwuLifeSummaryType.Instance[92],
				TaiwuLifeSummaryType.Instance[93],
				TaiwuLifeSummaryType.Instance[94]
			}));
			this.ApplySummaryItems();
		}

		// Token: 0x06007F13 RID: 32531 RVA: 0x003B383A File Offset: 0x003B1A3A
		// Note: this type is marked as 'beforefieldinit'.
		static TaiwuScrollItem()
		{
			LanguageKey[] array = new LanguageKey[8];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.752F009D6BBECE7D11AA8618CBE5FCEBE60382A6C79CBC4534CBA86DA32FC59D).FieldHandle);
			TaiwuScrollItem.RelationCountLabelsKeys = array;
		}

		// Token: 0x04006107 RID: 24839
		[Header("结缘数据")]
		[SerializeField]
		private TextMeshProUGUI relationTaiwuIndexLabel;

		// Token: 0x04006108 RID: 24840
		[SerializeField]
		private TextMeshProUGUI relationTitleLabel;

		// Token: 0x04006109 RID: 24841
		[SerializeField]
		private Game.Components.Avatar.Avatar taiwuAvatar;

		// Token: 0x0400610A RID: 24842
		[SerializeField]
		private CImage taiwuStatusImage;

		// Token: 0x0400610B RID: 24843
		[SerializeField]
		private Sprite[] statusSprites;

		// Token: 0x0400610C RID: 24844
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x0400610D RID: 24845
		[SerializeField]
		private TextMeshProUGUI taiwuTitleLabel;

		// Token: 0x0400610E RID: 24846
		[SerializeField]
		private TextMeshProUGUI relationTotalCountLabel;

		// Token: 0x0400610F RID: 24847
		[SerializeField]
		private TextMeshProUGUI[] relationCountLabels;

		// Token: 0x04006110 RID: 24848
		[Header("战斗数据")]
		[SerializeField]
		private TextMeshProUGUI combatTaiwuIndexLabel;

		// Token: 0x04006111 RID: 24849
		[SerializeField]
		private TextMeshProUGUI combatTitleLabel;

		// Token: 0x04006112 RID: 24850
		[SerializeField]
		private TaiwuSummaryHolderItem combatCountSummaryHolder;

		// Token: 0x04006113 RID: 24851
		[SerializeField]
		private TaiwuSummaryHolderItem combatTotalCountSummaryHolder;

		// Token: 0x04006114 RID: 24852
		[SerializeField]
		private TaiwuSummaryHolderItem savedInfectedCountSummaryHolder;

		// Token: 0x04006115 RID: 24853
		[Header("技艺数据")]
		[SerializeField]
		private TextMeshProUGUI lifeSkillTaiwuIndexLabel;

		// Token: 0x04006116 RID: 24854
		[SerializeField]
		private TextMeshProUGUI lifeSkillTitleLabel;

		// Token: 0x04006117 RID: 24855
		[SerializeField]
		private TaiwuSummaryHolderItem newLifeSkillSummaryHolder;

		// Token: 0x04006118 RID: 24856
		[SerializeField]
		private TaiwuSummaryHolderItem debateWinCountSummaryHolder;

		// Token: 0x04006119 RID: 24857
		[SerializeField]
		private TaiwuSummaryHolderItem readStrategySummaryHolder;

		// Token: 0x0400611A RID: 24858
		[SerializeField]
		private TaiwuSummaryHolderItem interpretLifeSkillSummaryHolder;

		// Token: 0x0400611B RID: 24859
		[Header("武学数据")]
		[SerializeField]
		private TextMeshProUGUI combatSkillTaiwuIndexLabel;

		// Token: 0x0400611C RID: 24860
		[SerializeField]
		private TextMeshProUGUI combatSkillTitleLabel;

		// Token: 0x0400611D RID: 24861
		[SerializeField]
		private TaiwuSummaryHolderItem interpretCombatSummaryHolder;

		// Token: 0x0400611E RID: 24862
		[SerializeField]
		private TaiwuSummaryHolderItem newCombatSkillSummaryHolder;

		// Token: 0x0400611F RID: 24863
		[SerializeField]
		private TaiwuSummaryHolderItem breakSummaryHolder;

		// Token: 0x04006120 RID: 24864
		[SerializeField]
		private TaiwuSummaryHolderItem loopingStrategySummaryHolder;

		// Token: 0x04006121 RID: 24865
		[Header("产业数据")]
		[SerializeField]
		private TextMeshProUGUI buildingTaiwuIndexLabel;

		// Token: 0x04006122 RID: 24866
		[SerializeField]
		private TextMeshProUGUI buildingTitleLabel;

		// Token: 0x04006123 RID: 24867
		[SerializeField]
		private TaiwuSummaryHolderItem buildingSummaryHolder;

		// Token: 0x04006124 RID: 24868
		[Header("志向数据")]
		[SerializeField]
		private TextMeshProUGUI professionTaiwuIndexLabel;

		// Token: 0x04006125 RID: 24869
		[SerializeField]
		private TextMeshProUGUI professionTitleLabel;

		// Token: 0x04006126 RID: 24870
		[SerializeField]
		private TaiwuSummaryHolderItem professionSummaryHolder;

		// Token: 0x04006127 RID: 24871
		[Header("游历数据")]
		[SerializeField]
		private TextMeshProUGUI travelTaiwuIndexLabel;

		// Token: 0x04006128 RID: 24872
		[SerializeField]
		private TextMeshProUGUI travelTitleLabel;

		// Token: 0x04006129 RID: 24873
		[SerializeField]
		private TaiwuSummaryHolderItem cricketSummaryHolder;

		// Token: 0x0400612A RID: 24874
		[SerializeField]
		private TaiwuSummaryHolderItem informationSummaryHolder;

		// Token: 0x0400612B RID: 24875
		[SerializeField]
		private TaiwuSummaryHolderItem adventureSummaryHolder;

		// Token: 0x0400612C RID: 24876
		[SerializeField]
		private TaiwuSummaryHolderItem travelSummaryHolder;

		// Token: 0x0400612D RID: 24877
		[Header("抵御侵袭")]
		[SerializeField]
		private TextMeshProUGUI invasionTaiwuIndexLabel;

		// Token: 0x0400612E RID: 24878
		[SerializeField]
		private TextMeshProUGUI invasionTitleLabel;

		// Token: 0x0400612F RID: 24879
		[Header("击破剑冢")]
		[SerializeField]
		private TextMeshProUGUI swordGraveTaiwuIndexLabel;

		// Token: 0x04006130 RID: 24880
		[SerializeField]
		private TextMeshProUGUI swordGraveTitleLabel;

		// Token: 0x04006131 RID: 24881
		[SerializeField]
		private TaiwuSummaryHolderItem swordGraveSummaryHolder;

		// Token: 0x04006132 RID: 24882
		private TaiwuLifeSummary _taiwuLifeSummary;

		// Token: 0x04006133 RID: 24883
		[TupleElementNames(new string[]
		{
			"holder",
			"types"
		})]
		private readonly List<ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>> _summaryItems = new List<ValueTuple<TaiwuSummaryHolderItem, TaiwuLifeSummaryTypeItem[]>>();

		// Token: 0x04006134 RID: 24884
		private readonly List<DlcId> _removedDlcList = new List<DlcId>();

		// Token: 0x04006135 RID: 24885
		private static readonly LanguageKey[] RelationCountLabelsKeys;
	}
}
