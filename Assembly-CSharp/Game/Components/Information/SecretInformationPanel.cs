using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Character.LifeRecord;
using Game.Components.Common;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Secret;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Serializer;
using GameData.Utilities;
using GameData.Utilities.Information;
using TMPro;
using UnityEngine;

namespace Game.Components.Information
{
	// Token: 0x02000EFA RID: 3834
	public class SecretInformationPanel : MonoBehaviour
	{
		// Token: 0x0600B085 RID: 45189 RVA: 0x00507564 File Offset: 0x00505764
		private void OnEnable()
		{
			this.RequestData();
		}

		// Token: 0x0600B086 RID: 45190 RVA: 0x0050756E File Offset: 0x0050576E
		public void Init(Action<SecretInformationDisplayData> callback = null)
		{
			this._callback = callback;
			this._canSelect = (callback != null);
			this.InitOwnerToggleGroup();
			this.InitInputField();
			this.InitLevelSetting();
			this.InitDateSelector();
			this.InitScroll();
			this.InitDetail();
		}

		// Token: 0x0600B087 RID: 45191 RVA: 0x005075AC File Offset: 0x005057AC
		public void Clear()
		{
			this._occurenceMap.Clear();
			this._holderCountMap.Clear();
			this._characterDisplayData.Clear();
			this.levelSettingSwitchToggle.isOn = false;
			foreach (Dictionary<SecretOccurenceId, SecretSortAndFilterData> dic in this._dataMap)
			{
				dic.Clear();
			}
		}

		// Token: 0x0600B088 RID: 45192 RVA: 0x00507634 File Offset: 0x00505834
		public void SetController(bool isTaiwu)
		{
			this._controller.SetVisibleSortIds(isTaiwu ? InformationUtils.TaiwuSortId : InformationUtils.NonTaiwuSortId);
		}

		// Token: 0x0600B089 RID: 45193 RVA: 0x00507654 File Offset: 0x00505854
		public void Set(SecretInformationDisplayPackage package, bool displayByOccurenceId)
		{
			this._displayByOccurenceId = displayByOccurenceId;
			bool flag = package != null;
			if (flag)
			{
				foreach (KeyValuePair<int, CharacterDisplayData> keyValuePair in package.CharacterData)
				{
					int num;
					CharacterDisplayData characterDisplayData;
					keyValuePair.Deconstruct(out num, out characterDisplayData);
					int id = num;
					CharacterDisplayData data = characterDisplayData;
					this._characterDisplayData[id] = data;
				}
				foreach (SecretInformationDisplayData data2 in package.SecretInformationDisplayDataList)
				{
					this.AddData(data2, data2.IsInBroadcast ? 1 : 0);
				}
			}
			for (int index = 0; index < this._dataMap.Count; index++)
			{
				this.ownerToggleGroupLabels[index].text = ((index == 0) ? LanguageKey.LK_CharacterMenu_Secret_Owner_Self.TrFormat(this._dataMap[index].Count) : LanguageKey.LK_CharacterMenu_Secret_Owner_Public.TrFormat(this._dataMap[index].Count));
				bool settingInit = this._settingInit;
				if (settingInit)
				{
					this.RefreshData(index);
				}
			}
			this.Refresh();
		}

		// Token: 0x0600B08A RID: 45194 RVA: 0x005077C0 File Offset: 0x005059C0
		public void Set(SecretInformationDisplayPackage package, int index, bool displayByOccurenceId)
		{
			this._displayByOccurenceId = displayByOccurenceId;
			bool flag = package != null;
			if (flag)
			{
				foreach (KeyValuePair<int, CharacterDisplayData> keyValuePair in package.CharacterData)
				{
					int num;
					CharacterDisplayData characterDisplayData;
					keyValuePair.Deconstruct(out num, out characterDisplayData);
					int id = num;
					CharacterDisplayData data = characterDisplayData;
					this._characterDisplayData[id] = data;
				}
				foreach (SecretInformationDisplayData data2 in package.SecretInformationDisplayDataList)
				{
					this.AddData(data2, index);
				}
			}
			this.ownerToggleGroupLabels[index].text = ((index == 0) ? LanguageKey.LK_CharacterMenu_Secret_Owner_Self.TrFormat(this._dataMap[index].Count) : LanguageKey.LK_CharacterMenu_Secret_Owner_Public.TrFormat(this._dataMap[index].Count));
			bool settingInit = this._settingInit;
			if (settingInit)
			{
				this.RefreshData(index);
			}
			bool flag2 = index == this.ownerToggleGroup.GetActiveIndex();
			if (flag2)
			{
				this.Refresh();
			}
		}

		// Token: 0x0600B08B RID: 45195 RVA: 0x0050790C File Offset: 0x00505B0C
		private void AddData(SecretInformationDisplayData data, int index)
		{
			SecretSortAndFilterData sortData;
			bool flag = this._dataMap[index].TryGetValue(data.OccurenceId, out sortData);
			if (flag)
			{
				this._dataMap[index][data.OccurenceId].Data.HolderCount += data.HolderCount;
			}
			else
			{
				sortData = new SecretSortAndFilterData
				{
					Data = data,
					Characters = new Dictionary<int, CharacterDisplayData>(),
					LevelScore = -1,
					Relation = -1,
					Date = SingletonObject.getInstance<BasicGameData>().CurrDate
				};
				this._dataMap[index][data.OccurenceId] = sortData;
			}
			this._occurenceMap.TryAdd(data.OccurenceId, new List<SecretInformationDisplayData>());
			this._occurenceMap[data.OccurenceId].Add(data);
			bool flag2 = data.SourceCharacterId >= 0;
			if (flag2)
			{
				this._holderCountMap.TryAdd(data.SourceCharacterId, new Dictionary<SecretInformationId, int>());
				this._holderCountMap[data.SourceCharacterId][data.SecretInformationId] = data.HolderCount;
			}
		}

		// Token: 0x0600B08C RID: 45196 RVA: 0x00507A35 File Offset: 0x00505C35
		private void RequestData()
		{
			InformationDomainMethod.AsyncCall.GetSecretInformationLevelFactor(null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._secretInformationLevelFactors);
				for (int index = 0; index < this._secretInformationLevelFactors.Length; index++)
				{
					this.levelSettingToggleGroups[index].SetWithoutNotify(this._secretInformationLevelFactors[index] + 1);
				}
				this._settingInit = true;
				for (int index2 = 0; index2 < this._dataMap.Count; index2++)
				{
					this.RefreshData(index2);
				}
				this.RefreshTop();
			});
		}

		// Token: 0x0600B08D RID: 45197 RVA: 0x00507A4C File Offset: 0x00505C4C
		private void UpdateDate()
		{
			this._dateIndices.Clear();
			this._minDate = int.MaxValue;
			this._maxDate = int.MinValue;
			int currDate = -1;
			for (int index = 0; index < this._filteredSecrets.Count; index++)
			{
				SecretSortAndFilterData secret = this._filteredSecrets[index];
				int date = secret.Data.OccurenceDate;
				bool flag = date != currDate;
				if (flag)
				{
					currDate = date;
					bool flag2 = this._minDate > date;
					if (flag2)
					{
						this._minDate = date;
					}
					bool flag3 = this._maxDate < date;
					if (flag3)
					{
						this._maxDate = date;
					}
					this._dateIndices[date] = index;
				}
			}
		}

		// Token: 0x0600B08E RID: 45198 RVA: 0x00507B00 File Offset: 0x00505D00
		private void UpdateSortAndFilterData(SecretSortAndFilterData data)
		{
			SecretInformationItem config = SecretInformation.Instance[data.Data.SecretInformationTemplateId];
			int relationScore = 0;
			int charScore = 0;
			int itemScore = 0;
			short valueScore = config.SortValue;
			byte[] parametersPack = data.Data.ParametersPack;
			if (parametersPack != null)
			{
				parametersPack.ExtractSecretParameters(config, delegate(int _, int charId)
				{
					CharacterDisplayData characterDisplayData = this._characterDisplayData[charId];
					data.Characters[charId] = characterDisplayData;
					charScore += (Organization.Instance[characterDisplayData.OrgInfo.OrgTemplateId].IsSect ? GlobalConfig.Instance.SecretSectFactor[(int)characterDisplayData.OrgInfo.Grade] : GlobalConfig.Instance.SecretNonSectFactor[(int)characterDisplayData.OrgInfo.Grade]);
					bool flag = charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					if (flag)
					{
						int relationScore = relationScore;
						int[] secretRelationFactor = GlobalConfig.Instance.SecretRelationFactor;
						relationScore += secretRelationFactor[secretRelationFactor.Length - 1];
						data.Relation = short.MaxValue;
					}
					else
					{
						int highest = 0;
						for (short type = 0; type <= 9; type += 1)
						{
							bool flag2 = type == 4;
							if (flag2)
							{
								bool flag3 = characterDisplayData.OrgInfo.OrgTemplateId == 16;
								if (flag3)
								{
									bool flag4 = highest < GlobalConfig.Instance.SecretRelationFactor[(int)type];
									if (flag4)
									{
										highest = GlobalConfig.Instance.SecretRelationFactor[(int)type];
										data.Relation = type;
									}
								}
							}
							else
							{
								foreach (sbyte typeId in RelationDisplayType.Instance[type].RelationTypeIds)
								{
									ushort relationType = RelationType.GetRelationType(typeId);
									bool flag5 = (characterDisplayData.RelationToTaiwu != ushort.MaxValue && RelationType.HasRelation(characterDisplayData.RelationToTaiwu, relationType)) || (characterDisplayData.RelationFromTaiwu != ushort.MaxValue && RelationType.HasRelation(characterDisplayData.RelationFromTaiwu, relationType));
									if (flag5)
									{
										bool flag6 = highest < GlobalConfig.Instance.SecretRelationFactor[(int)type];
										if (flag6)
										{
											highest = GlobalConfig.Instance.SecretRelationFactor[(int)type];
											data.Relation = type;
										}
									}
								}
							}
						}
						int relationScore;
						relationScore += highest;
					}
				}, null, null, delegate(int _, ItemKey itemKey)
				{
					itemScore += (itemKey.IsValid() ? GlobalConfig.Instance.SecretItemFactor[(int)ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId)] : 0);
				}, null, null, null);
			}
			data.LevelScore = relationScore * GlobalConfig.Instance.SecretRelationScore[this.levelSettingToggleGroups[2].GetActiveIndex()] + charScore * GlobalConfig.Instance.SecretCharScore[this.levelSettingToggleGroups[1].GetActiveIndex()] + itemScore * GlobalConfig.Instance.SecretItemScore[this.levelSettingToggleGroups[3].GetActiveIndex()] + (int)valueScore * GlobalConfig.Instance.SecretSortValueScore[this.levelSettingToggleGroups[0].GetActiveIndex()];
		}

		// Token: 0x0600B08F RID: 45199 RVA: 0x00507C18 File Offset: 0x00505E18
		private void InitOwnerToggleGroup()
		{
			this.ownerToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(UIElement.CharacterMenuSecretInformation, this.ownerToggleGroup, 1, null);
			this.ownerToggleGroup.OnActiveIndexChange += this.OnOwnerToggleGroupChange;
		}

		// Token: 0x0600B090 RID: 45200 RVA: 0x00507C53 File Offset: 0x00505E53
		private void InitInputField()
		{
			this.searchField.onValueChanged.ResetListener(new Action<string>(this.OnSearch));
		}

		// Token: 0x0600B091 RID: 45201 RVA: 0x00507C74 File Offset: 0x00505E74
		private void InitLevelSetting()
		{
			this.levelSettingSwitchToggle.onValueChanged.ResetListener(new Action<bool>(this.OnLevelToggleChange));
			this.btnReset.ClearAndAddListener(new Action(this.OnClickReset));
			this.btnCancel.ClearAndAddListener(new Action(this.OnClickCancel));
			this.btnConfirm.ClearAndAddListener(new Action(this.OnClickConfirm));
			foreach (CToggleGroup toggleGroup in this.levelSettingToggleGroups)
			{
				toggleGroup.Init(-1);
			}
		}

		// Token: 0x0600B092 RID: 45202 RVA: 0x00507D09 File Offset: 0x00505F09
		private void InitDateSelector()
		{
			this.dateSelectorParent.Init(new Action<int, bool, bool>(this.OnTriggerScrollToMonth));
			this.dateSelector.DateHasData = new Func<int, bool>(this.DateHasData);
		}

		// Token: 0x0600B093 RID: 45203 RVA: 0x00507D3C File Offset: 0x00505F3C
		private void InitScroll()
		{
			this._controller = new SecretSortAndFilterController(this.sortAndFilter);
			this._controller.Init(new Action(this.Refresh), "ViewCharacterMenuSecret");
			this.scroll.InitPageCount();
			this.scroll.OnItemRender += this.OnRenderSecret;
		}

		// Token: 0x0600B094 RID: 45204 RVA: 0x00507D9C File Offset: 0x00505F9C
		private void InitDetail()
		{
			this.detailedToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(UIElement.CharacterMenuSecretInformation, this.detailedToggleGroup, 2, null);
			this.detailedToggleGroup.OnActiveIndexChange += this.OnDetailedToggleGroupChange;
			this.sourceScroll.InitPageCount();
			this.sourceScroll.OnItemRender += (this._canSelect ? new Action<int, GameObject>(this.OnRenderSourceWithSelect) : new Action<int, GameObject>(this.OnRenderSourceWithoutSelect));
			this.effectScroll.InitPageCount();
			this.effectScroll.OnItemRender += this.OnRenderEffect;
		}

		// Token: 0x0600B095 RID: 45205 RVA: 0x00507E40 File Offset: 0x00506040
		private void Refresh()
		{
			this._filteredSecrets.Clear();
			this.OnSelectSecret(-1);
			Func<SecretSortAndFilterData, bool> filter = this._controller.GenerateFilter();
			Comparison<SecretSortAndFilterData> comparer = this._controller.GenerateComparer(this._filteredSecrets);
			string searchText = this.searchField.text;
			bool flag = searchText.IsNullOrEmpty();
			if (flag)
			{
				foreach (SecretSortAndFilterData data in this._dataMap[this.ownerToggleGroup.GetActiveIndex()].Values)
				{
					bool flag2 = filter(data);
					if (flag2)
					{
						this._filteredSecrets.Add(data);
					}
				}
			}
			else
			{
				foreach (SecretSortAndFilterData data2 in this._dataMap[this.ownerToggleGroup.GetActiveIndex()].Values)
				{
					bool flag3 = filter(data2) && SecretInformation.Instance[data2.Data.SecretInformationTemplateId].Name.Contains(searchText);
					if (flag3)
					{
						this._filteredSecrets.Add(data2);
					}
				}
			}
			bool flag4 = comparer != null;
			if (flag4)
			{
				this._filteredSecrets.Sort(comparer);
			}
			this._controller.AfterFilter(this._dataMap[this.ownerToggleGroup.GetActiveIndex()].Values);
			this.UpdateDate();
			this.RefreshScrollAndDate();
			this.RefreshRight();
		}

		// Token: 0x0600B096 RID: 45206 RVA: 0x00507FFC File Offset: 0x005061FC
		private void RefreshTop()
		{
			int val = this.levelSettingToggleGroups[0].GetActiveIndex();
			for (int index = 1; index < this.levelSettingToggleGroups.Length; index++)
			{
				bool flag = val != this.levelSettingToggleGroups[index].GetActiveIndex();
				if (flag)
				{
					this.settingLabel.text = LanguageKey.LK_CharacterMenu_Secret_Level_Custom.Tr();
					return;
				}
			}
			TextMeshProUGUI textMeshProUGUI = this.settingLabel;
			if (!true)
			{
			}
			string text;
			switch (val)
			{
			case 0:
				text = LanguageKey.LK_Low.Tr();
				break;
			case 1:
				text = LanguageKey.LK_Mid.Tr();
				break;
			case 2:
				text = LanguageKey.LK_High.Tr();
				break;
			default:
				text = LanguageKey.LK_CharacterMenu_Secret_Level_Custom.Tr();
				break;
			}
			if (!true)
			{
			}
			textMeshProUGUI.text = text;
		}

		// Token: 0x0600B097 RID: 45207 RVA: 0x005080C8 File Offset: 0x005062C8
		private void RefreshScrollAndDate()
		{
			this.scroll.SetDataCount(this._filteredSecrets.Count);
			this.dateSelector.gameObject.SetActive(false);
			bool flag = this._filteredSecrets.Count > 0;
			if (flag)
			{
				foreach (SortItemState state in this._controller.SortAndFilterState.SortData.ItemStates)
				{
					bool flag2 = state.SortId == 159;
					if (flag2)
					{
						this.dateSelector.Set(this._minDate, this._maxDate, this.dateSelectorParent);
						this.dateSelector.gameObject.SetActive(true);
					}
				}
				this.leftNoContent.SetActive(false);
			}
			else
			{
				this.leftNoContent.SetActive(true);
			}
		}

		// Token: 0x0600B098 RID: 45208 RVA: 0x005081CC File Offset: 0x005063CC
		private void RefreshRight()
		{
			this.OnDetailedToggleGroupChange(-1, -1);
			bool flag = this._currIndex < 0;
			if (flag)
			{
				this.rightNoContent.SetActive(true);
				this.rightContent.SetActive(false);
				this.detailedToggleGroup.Set(0, false);
			}
			else
			{
				SecretSortAndFilterData data = this._filteredSecrets[this._currIndex];
				SecretInformationItem config = SecretInformation.Instance[data.Data.SecretInformationTemplateId];
				int maxCount = (data.Data.AuthorityCostWhenDisseminating == 0) ? GlobalConfig.Instance.SecretInformationInBroadcastMaxUseCount : GlobalConfig.Instance.SecretInformationInPrivateMaxUseCount;
				this.titleLabel.text = config.Name;
				bool flag2 = data.Relation >= 0;
				if (flag2)
				{
					this.relationLabel.text = ((data.Relation == short.MaxValue) ? LanguageKey.LK_Taiwu.Tr() : RelationDisplayType.Instance[data.Relation].Name);
					this.relationBack.SetActive(true);
				}
				else
				{
					this.relationBack.SetActive(false);
				}
				bool flag3 = data.Data.DisseminationRate >= 0;
				if (flag3)
				{
					this.dissemination.SetSprite(string.Format("ui9_icon_dissemination_{0}", data.DisseminationLevel), false, null);
					this.dissemination.gameObject.SetActive(true);
				}
				else
				{
					this.dissemination.gameObject.SetActive(false);
				}
				this.charCountLabel.text = data.Data.HolderCount.ToString();
				this.durationLabel.text = ((data.GetConfig.Duration >= 0) ? data.LifeTime.ToString() : "∞");
				this.costLabel.text = ((data.Data.AuthorityCostWhenDisseminating == 0) ? data.Data.AuthorityCostWhenDisseminatingForBroadcast : data.Data.AuthorityCostWhenDisseminating).ToString();
				this.useCountLabel.text = string.Format("{0}/{1}", data.CanUseCount, maxCount);
				this.descLabel.text = InformationUtils.MakeSecretInformationDescription(data);
				this._sourceCharacters.Clear();
				bool displayByOccurenceId = this._displayByOccurenceId;
				if (displayByOccurenceId)
				{
					bool flag4 = !config.AutoBroadCast;
					if (flag4)
					{
						foreach (SecretInformationDisplayData displayData in this._occurenceMap[data.Data.OccurenceId])
						{
							this._sourceCharacters.Add(displayData.SourceCharacterId);
						}
					}
				}
				else
				{
					bool flag5 = data.Data.SourceCharacterId >= 0;
					if (flag5)
					{
						this._sourceCharacters.Add(data.Data.SourceCharacterId);
					}
				}
				this.sourceScroll.SetDataCount(this._sourceCharacters.Count);
				InformationDomainMethod.AsyncCall.GetSecretInformationDetailedData(null, data.Data.SecretInformationId, data.Data.SourceCharacterId, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._effectCharacters);
					InfinityScroll infinityScroll = this.effectScroll;
					List<SecretInformationEffectData> effectCharacters = this._effectCharacters;
					infinityScroll.SetDataCount((effectCharacters != null) ? effectCharacters.Count : 0);
				});
				this.targetContent.Set(this._filteredSecrets[this._currIndex].Data, this._characterDisplayData);
				this.rightNoContent.SetActive(false);
				this.rightContent.SetActive(true);
			}
		}

		// Token: 0x0600B099 RID: 45209 RVA: 0x00508550 File Offset: 0x00506750
		private void RefreshData(int index)
		{
			foreach (SecretSortAndFilterData data in this._dataMap[index].Values)
			{
				this.UpdateSortAndFilterData(data);
			}
			this.Refresh();
		}

		// Token: 0x0600B09A RID: 45210 RVA: 0x005085BC File Offset: 0x005067BC
		private void OnOwnerToggleGroupChange(int __, int _)
		{
			this.detailedToggleGroup.Get(2).gameObject.SetActive(this.ownerToggleGroup.GetActiveIndex() == 0);
			this.Refresh();
		}

		// Token: 0x0600B09B RID: 45211 RVA: 0x005085EC File Offset: 0x005067EC
		private void OnSearch(string value)
		{
			bool flag = CommonUtils.FixToShowAbleString(ref value, this.searchField.textComponent.font);
			if (flag)
			{
				this.searchField.SetTextWithoutNotify(value);
			}
			this.Refresh();
		}

		// Token: 0x0600B09C RID: 45212 RVA: 0x00508629 File Offset: 0x00506829
		private void OnLevelToggleChange(bool value)
		{
			this.levelSettingPanel.SetActive(value);
		}

		// Token: 0x0600B09D RID: 45213 RVA: 0x0050863C File Offset: 0x0050683C
		private void OnClickReset()
		{
			this._secretInformationLevelFactors = new int[4];
			foreach (CToggleGroup group in this.levelSettingToggleGroups)
			{
				group.Set(1, false);
			}
		}

		// Token: 0x0600B09E RID: 45214 RVA: 0x0050867C File Offset: 0x0050687C
		private void OnClickConfirm()
		{
			for (int i = 0; i < this.levelSettingToggleGroups.Length; i++)
			{
				this._secretInformationLevelFactors[i] = this.levelSettingToggleGroups[i].GetActiveIndex() - 1;
			}
			this.RefreshData(0);
			this.RefreshData(1);
			this.Refresh();
			this.RefreshTop();
			this.levelSettingSwitchToggle.isOn = false;
		}

		// Token: 0x0600B09F RID: 45215 RVA: 0x005086E3 File Offset: 0x005068E3
		private void OnClickCancel()
		{
			this.levelSettingSwitchToggle.isOn = false;
		}

		// Token: 0x0600B0A0 RID: 45216 RVA: 0x005086F4 File Offset: 0x005068F4
		private void OnTriggerScrollToMonth(int month, bool _, bool __)
		{
			bool flag = !this.dateSelector.gameObject.activeSelf;
			if (!flag)
			{
				int index;
				bool flag2 = this._dateIndices.TryGetValue(month, out index);
				if (flag2)
				{
					this.scroll.ScrollTo(index, 0.3f);
				}
				else
				{
					index = 0;
					int closest = int.MaxValue;
					foreach (KeyValuePair<int, int> keyValuePair in this._dateIndices)
					{
						int num;
						int num2;
						keyValuePair.Deconstruct(out num, out num2);
						int date = num;
						int otherIndex = num2;
						int val = Math.Abs(date - month);
						bool flag3 = val < closest;
						if (flag3)
						{
							closest = val;
							index = otherIndex;
						}
					}
					this.scroll.ScrollTo(index, 0.3f);
				}
			}
		}

		// Token: 0x0600B0A1 RID: 45217 RVA: 0x005087DC File Offset: 0x005069DC
		private void OnRenderSecret(int index, GameObject obj)
		{
			obj.GetComponent<CToggle>().SetIsOnWithoutNotify(index == this._currIndex);
			obj.GetComponent<SecretInformationCardItem>().Set(index, new Action<int, bool>(this.OnClickSecret), this._filteredSecrets[index]);
		}

		// Token: 0x0600B0A2 RID: 45218 RVA: 0x0050881C File Offset: 0x00506A1C
		private void OnDetailedToggleGroupChange(int _, int __)
		{
			int index = this.detailedToggleGroup.GetActiveIndex();
			for (int i = 0; i < this.detailedTypes.Length; i++)
			{
				this.detailedTypes[i].SetActive(i == index);
			}
		}

		// Token: 0x0600B0A3 RID: 45219 RVA: 0x0050885F File Offset: 0x00506A5F
		private void OnRenderSourceWithoutSelect(int index, GameObject obj)
		{
			obj.GetComponent<SecretInformationSourceItem>().Set(this._characterDisplayData[this._sourceCharacters[index]], this.GetHolderCount(index));
		}

		// Token: 0x0600B0A4 RID: 45220 RVA: 0x0050888C File Offset: 0x00506A8C
		private void OnRenderSourceWithSelect(int index, GameObject obj)
		{
			SecretInformationSourceItem item = obj.GetComponent<SecretInformationSourceItem>();
			item.Init(index, new Action<int>(this.OnSelectSource));
			item.Set(this._characterDisplayData[this._sourceCharacters[index]], this.GetHolderCount(index));
			item.SetSelected(this._currSourceIndex == index);
		}

		// Token: 0x0600B0A5 RID: 45221 RVA: 0x005088EA File Offset: 0x00506AEA
		private void OnRenderEffect(int index, GameObject obj)
		{
			obj.GetComponent<SecretInformationBroadCastEffectItem>().Set(this._effectCharacters[index], this._characterDisplayData[this._effectCharacters[index].CharId]);
		}

		// Token: 0x0600B0A6 RID: 45222 RVA: 0x00508924 File Offset: 0x00506B24
		private void OnClickSecret(int index, bool value)
		{
			if (value)
			{
				bool flag = this._currIndex >= 0;
				if (flag)
				{
					GameObject obj = this.scroll.GetActiveCell(this._currIndex);
					bool flag2 = obj != null;
					if (flag2)
					{
						obj.GetComponent<CToggle>().SetIsOnWithoutNotify(false);
					}
				}
				this.OnSelectSecret(index);
			}
			else
			{
				this.OnSelectSecret(-1);
			}
			this.RefreshRight();
		}

		// Token: 0x0600B0A7 RID: 45223 RVA: 0x00508990 File Offset: 0x00506B90
		private void OnSelectSecret(int index)
		{
			this._currIndex = index;
			bool flag = !this._canSelect;
			if (!flag)
			{
				bool flag2 = this._filteredSecrets.CheckIndex(index);
				if (flag2)
				{
					SecretSortAndFilterData data = this._filteredSecrets[index];
					SecretInformationItem config = SecretInformation.Instance[data.Data.SecretInformationTemplateId];
					bool autoBroadCast = config.AutoBroadCast;
					if (autoBroadCast)
					{
						this._currSourceIndex = -1;
						this._callback(data.Data);
					}
					else
					{
						this.OnSelectSource(0);
					}
				}
				else
				{
					this._currSourceIndex = -1;
					this._callback(null);
				}
			}
		}

		// Token: 0x0600B0A8 RID: 45224 RVA: 0x00508A34 File Offset: 0x00506C34
		private void OnSelectSource(int index)
		{
			this._currSourceIndex = index;
			for (int i = 0; i < this._sourceCharacters.Count; i++)
			{
				GameObject obj = this.sourceScroll.GetActiveCell(i);
				bool flag = obj != null;
				if (flag)
				{
					obj.GetComponent<SecretInformationSourceItem>().SetSelected(i == index);
				}
			}
			this._callback(this._occurenceMap[this._filteredSecrets[this._currIndex].Data.OccurenceId][index]);
		}

		// Token: 0x0600B0A9 RID: 45225 RVA: 0x00508AC8 File Offset: 0x00506CC8
		private int GetHolderCount(int index)
		{
			Dictionary<SecretInformationId, int> holder;
			int count;
			return (this._holderCountMap.TryGetValue(this._sourceCharacters[index], out holder) && holder.TryGetValue(this._filteredSecrets[this._currIndex].Data.SecretInformationId, out count)) ? count : 0;
		}

		// Token: 0x0600B0AA RID: 45226 RVA: 0x00508B19 File Offset: 0x00506D19
		private bool DateHasData(int month)
		{
			return this._dateIndices.ContainsKey(month);
		}

		// Token: 0x04008885 RID: 34949
		[Header("Top")]
		public CToggleGroup ownerToggleGroup;

		// Token: 0x04008886 RID: 34950
		public TMP_InputField searchField;

		// Token: 0x04008887 RID: 34951
		public CToggle levelSettingSwitchToggle;

		// Token: 0x04008888 RID: 34952
		public GameObject levelSettingPanel;

		// Token: 0x04008889 RID: 34953
		public CToggleGroup[] levelSettingToggleGroups;

		// Token: 0x0400888A RID: 34954
		public CButton btnReset;

		// Token: 0x0400888B RID: 34955
		public CButton btnConfirm;

		// Token: 0x0400888C RID: 34956
		public CButton btnCancel;

		// Token: 0x0400888D RID: 34957
		public TextMeshProUGUI[] ownerToggleGroupLabels;

		// Token: 0x0400888E RID: 34958
		public TextMeshProUGUI settingLabel;

		// Token: 0x0400888F RID: 34959
		[Header("Left")]
		public SortAndFilter sortAndFilter;

		// Token: 0x04008890 RID: 34960
		public InfinityScroll scroll;

		// Token: 0x04008891 RID: 34961
		public RecordSimple dateSelectorParent;

		// Token: 0x04008892 RID: 34962
		public LifeRecordDateSelector dateSelector;

		// Token: 0x04008893 RID: 34963
		public GameObject leftNoContent;

		// Token: 0x04008894 RID: 34964
		[Header("Right")]
		public GameObject rightContent;

		// Token: 0x04008895 RID: 34965
		public GameObject rightNoContent;

		// Token: 0x04008896 RID: 34966
		public TextMeshProUGUI titleLabel;

		// Token: 0x04008897 RID: 34967
		public GameObject relationBack;

		// Token: 0x04008898 RID: 34968
		public TextMeshProUGUI relationLabel;

		// Token: 0x04008899 RID: 34969
		public CImage dissemination;

		// Token: 0x0400889A RID: 34970
		public TextMeshProUGUI charCountLabel;

		// Token: 0x0400889B RID: 34971
		public TextMeshProUGUI durationLabel;

		// Token: 0x0400889C RID: 34972
		public TextMeshProUGUI costLabel;

		// Token: 0x0400889D RID: 34973
		public TextMeshProUGUI useCountLabel;

		// Token: 0x0400889E RID: 34974
		public TextMeshProUGUI descLabel;

		// Token: 0x0400889F RID: 34975
		public CToggleGroup detailedToggleGroup;

		// Token: 0x040088A0 RID: 34976
		public GameObject[] detailedTypes;

		// Token: 0x040088A1 RID: 34977
		public InfinityScroll sourceScroll;

		// Token: 0x040088A2 RID: 34978
		public InfinityScroll effectScroll;

		// Token: 0x040088A3 RID: 34979
		public SecretInformationTargetContent targetContent;

		// Token: 0x040088A4 RID: 34980
		private SecretSortAndFilterController _controller;

		// Token: 0x040088A5 RID: 34981
		private Dictionary<int, CharacterDisplayData> _characterDisplayData = new Dictionary<int, CharacterDisplayData>();

		// Token: 0x040088A6 RID: 34982
		private Dictionary<SecretOccurenceId, List<SecretInformationDisplayData>> _occurenceMap = new Dictionary<SecretOccurenceId, List<SecretInformationDisplayData>>();

		// Token: 0x040088A7 RID: 34983
		private Dictionary<int, Dictionary<SecretInformationId, int>> _holderCountMap = new Dictionary<int, Dictionary<SecretInformationId, int>>();

		// Token: 0x040088A8 RID: 34984
		private bool _displayByOccurenceId = false;

		// Token: 0x040088A9 RID: 34985
		private bool _canSelect;

		// Token: 0x040088AA RID: 34986
		private List<Dictionary<SecretOccurenceId, SecretSortAndFilterData>> _dataMap = new List<Dictionary<SecretOccurenceId, SecretSortAndFilterData>>
		{
			new Dictionary<SecretOccurenceId, SecretSortAndFilterData>(),
			new Dictionary<SecretOccurenceId, SecretSortAndFilterData>()
		};

		// Token: 0x040088AB RID: 34987
		private List<SecretSortAndFilterData> _filteredSecrets = new List<SecretSortAndFilterData>();

		// Token: 0x040088AC RID: 34988
		private Dictionary<int, int> _dateIndices = new Dictionary<int, int>();

		// Token: 0x040088AD RID: 34989
		private List<int> _sourceCharacters = new List<int>();

		// Token: 0x040088AE RID: 34990
		private List<SecretInformationEffectData> _effectCharacters = new List<SecretInformationEffectData>();

		// Token: 0x040088AF RID: 34991
		private Action<SecretInformationDisplayData> _callback;

		// Token: 0x040088B0 RID: 34992
		private int _minDate;

		// Token: 0x040088B1 RID: 34993
		private int _maxDate;

		// Token: 0x040088B2 RID: 34994
		private int _currIndex;

		// Token: 0x040088B3 RID: 34995
		private int _currSourceIndex;

		// Token: 0x040088B4 RID: 34996
		private bool _settingInit;

		// Token: 0x040088B5 RID: 34997
		private int[] _secretInformationLevelFactors = new int[4];
	}
}
