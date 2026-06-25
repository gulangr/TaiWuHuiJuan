using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Bottom
{
	// Token: 0x02000C55 RID: 3157
	public class ViewFindMapBlock : UIBase
	{
		// Token: 0x170010E8 RID: 4328
		// (get) Token: 0x0600A09E RID: 41118 RVA: 0x004AFB94 File Offset: 0x004ADD94
		public MapBlockFindData CurrentFilterData
		{
			get
			{
				return this._currentFilterData;
			}
		}

		// Token: 0x0600A09F RID: 41119 RVA: 0x004AFB9C File Offset: 0x004ADD9C
		public override void OnInit(ArgumentBox argsBox)
		{
			this.NeedDataListenerId = true;
			this.NeedWaitData = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
			this.switchToggle.SetIsOnWithoutNotify(true);
			this._isOn = true;
			this.DispatchEvent();
		}

		// Token: 0x0600A0A0 RID: 41120 RVA: 0x004AFBFA File Offset: 0x004ADDFA
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.UpdateMapBlockData, new GEvent.Callback(this.OnMapBlockDataUpdated));
		}

		// Token: 0x0600A0A1 RID: 41121 RVA: 0x004AFC1C File Offset: 0x004ADE1C
		private void OnMapBlockDataUpdated(ArgumentBox _)
		{
			bool isOn = this._isOn;
			if (isOn)
			{
				this.DispatchEvent();
				MapDomainMethod.AsyncCall.SetMapBlockFindDataPreset(null, this._currentPresetIndex, this._currentFilterData, delegate(int offset, RawDataPool pool)
				{
					List<Location> res = new List<Location>();
					Serializer.Deserialize(pool, offset, ref res);
					SingletonObject.getInstance<WorldMapModel>().ClearAllTemporaryMarkList();
					SingletonObject.getInstance<WorldMapModel>().AddLocationsToTemporaryMarkList(res);
				});
			}
		}

		// Token: 0x0600A0A2 RID: 41122 RVA: 0x004AFC6F File Offset: 0x004ADE6F
		private void RequestData()
		{
			MapDomainMethod.AsyncCall.GetMapBlockFindDataPreset(this, this._currentPresetIndex, delegate(int offset, RawDataPool pool)
			{
				bool flag = !this;
				if (!flag)
				{
					Serializer.Deserialize(pool, offset, ref this._currentFilterData);
					if (this._currentFilterData == null)
					{
						this._currentFilterData = new MapBlockFindData();
					}
					this.UpdateTertiaryFilterUI();
					this.SetFilterItems();
					this.Element.ShowAfterRefresh();
					this.OnFilterDataChanged();
				}
			});
		}

		// Token: 0x0600A0A3 RID: 41123 RVA: 0x004AFC8B File Offset: 0x004ADE8B
		private void OnFilterDataChanged()
		{
			this.DispatchEvent();
			MapDomainMethod.AsyncCall.SetMapBlockFindDataPreset(this, this._currentPresetIndex, this._currentFilterData, delegate(int offset, RawDataPool pool)
			{
				bool flag = !this;
				if (!flag)
				{
					List<Location> res = new List<Location>();
					Serializer.Deserialize(pool, offset, ref res);
					this.ClearAllTemporaryMarkList();
					this.AddLocationsToTemporaryMarkList(res);
				}
			});
		}

		// Token: 0x0600A0A4 RID: 41124 RVA: 0x004AFCB4 File Offset: 0x004ADEB4
		private void Awake()
		{
			this._currentFilterData = new MapBlockFindData();
			this.InitializeUIConfig();
			this.InitFilterItems();
			this.switchToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnSwitchToggleChanged));
			this.toggleGroupPreset.Init(-1);
			this.toggleGroupPreset.OnActiveIndexChange += delegate(int newIndex, int oldIndex)
			{
				this._currentPresetIndex = newIndex;
				this.RequestData();
			};
			this.primaryFilterToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.primaryFilterToggleGroup, 0, null);
			this.primaryFilterToggleGroup.OnActiveIndexChange += delegate(int newIndex, int oldIndex)
			{
				this._currentPrimaryType = (EPrimaryFilterType)newIndex;
				this.UpdateTertiaryFilterUI();
			};
			this.secondaryFilterToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.secondaryFilterToggleGroup, 1, null);
			this.secondaryFilterToggleGroup.OnActiveIndexChange += delegate(int newIndex, int oldIndex)
			{
				this._currentSecondaryType = new ESecondaryFilterType?((ESecondaryFilterType)newIndex);
				this.UpdateTertiaryFilterUI();
			};
			this.primaryResetBtn.ClearAndAddListener(new Action(this.ClearCurrentFilter));
			this.secondaryResetBtn.ClearAndAddListener(new Action(this.ClearCharacterFilter));
			GEvent.Add(UiEvents.UpdateMapBlockData, new GEvent.Callback(this.OnMapBlockDataUpdated));
		}

		// Token: 0x0600A0A5 RID: 41125 RVA: 0x004AFDDC File Offset: 0x004ADFDC
		private void ClearCharacterFilter()
		{
			foreach (KeyValuePair<FilterLevelKey, EFilterItemKey[]> keyValuePair in this._uiConfig.TertiaryConfigs)
			{
				FilterLevelKey filterLevelKey;
				EFilterItemKey[] array;
				keyValuePair.Deconstruct(out filterLevelKey, out array);
				FilterLevelKey i = filterLevelKey;
				EFilterItemKey[] v = array;
				bool flag = i.Primary > EPrimaryFilterType.Character;
				if (!flag)
				{
					foreach (EFilterItemKey key in v)
					{
						FilterItemConfig itemConfig = this._uiConfig.ItemConfigs[key];
						switch (itemConfig.Type)
						{
						case EFilterElementType.SingleSelect:
							this._currentFilterData.SingleSelectData.Remove(key);
							break;
						case EFilterElementType.MultiSelect:
							this._currentFilterData.MultiSelectData.Remove(key);
							break;
						case EFilterElementType.SingleSlider:
							this._currentFilterData.SingleSliderData.Remove(key);
							break;
						case EFilterElementType.RangeSlider:
							this._currentFilterData.RangeSliderData.Remove(key);
							break;
						case EFilterElementType.ToggleSlider:
							this._currentFilterData.ToggleSliderData.Remove(key);
							break;
						}
					}
				}
			}
			this.OnFilterDataChanged();
			this.Refresh();
		}

		// Token: 0x0600A0A6 RID: 41126 RVA: 0x004AFF48 File Offset: 0x004AE148
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "ButtonCloseView" == btnName;
			if (flag)
			{
				this.Element.Hide(false);
			}
		}

		// Token: 0x0600A0A7 RID: 41127 RVA: 0x004AFF7C File Offset: 0x004AE17C
		private void InitFilterItems()
		{
			foreach (KeyValuePair<FilterLevelKey, List<MapBlockFindItemBase>> keyValuePair in ViewFindMapBlock._filterItemDict)
			{
				FilterLevelKey filterLevelKey;
				List<MapBlockFindItemBase> list;
				keyValuePair.Deconstruct(out filterLevelKey, out list);
				List<MapBlockFindItemBase> v = list;
				v.RemoveAll((MapBlockFindItemBase item) => item == null);
			}
			foreach (KeyValuePair<FilterLevelKey, EFilterItemKey[]> keyValuePair2 in this._uiConfig.TertiaryConfigs)
			{
				FilterLevelKey filterLevelKey;
				EFilterItemKey[] array;
				keyValuePair2.Deconstruct(out filterLevelKey, out array);
				FilterLevelKey i = filterLevelKey;
				EFilterItemKey[] v2 = array;
				int holderIndex = FilterLevelKey.FilterLevelKeyDict[i];
				foreach (EFilterItemKey eFilterItemKey in v2)
				{
					FilterItemConfig itemConfig = this._uiConfig.ItemConfigs[eFilterItemKey];
					MapBlockFindItemBase filterItem = Object.Instantiate<MapBlockFindItemBase>(this.GetTemplateItem(itemConfig.Type), this.itemHolderRoot.GetChild(holderIndex));
					filterItem.gameObject.SetActive(true);
					filterItem.Set(this, itemConfig.Key, itemConfig);
					ViewFindMapBlock._filterItemDict[i].Add(filterItem);
				}
			}
		}

		// Token: 0x0600A0A8 RID: 41128 RVA: 0x004B0100 File Offset: 0x004AE300
		private MapBlockFindItemBase GetTemplateItem(EFilterElementType type)
		{
			MapBlockFindItemBase result;
			switch (type)
			{
			case EFilterElementType.SingleSelect:
				result = this.singleSelectItem;
				break;
			case EFilterElementType.MultiSelect:
				result = this.multiSelectItem;
				break;
			case EFilterElementType.SingleSlider:
				result = this.singleSliderItem;
				break;
			case EFilterElementType.RangeSlider:
				result = this.rangeSliderItem;
				break;
			case EFilterElementType.ToggleSlider:
				result = this.toggleSliderItem;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		// Token: 0x0600A0A9 RID: 41129 RVA: 0x004B0160 File Offset: 0x004AE360
		public void SetFilterItems()
		{
			foreach (KeyValuePair<FilterLevelKey, List<MapBlockFindItemBase>> keyValuePair in ViewFindMapBlock._filterItemDict)
			{
				FilterLevelKey filterLevelKey;
				List<MapBlockFindItemBase> list;
				keyValuePair.Deconstruct(out filterLevelKey, out list);
				List<MapBlockFindItemBase> v = list;
				foreach (MapBlockFindItemBase item in v)
				{
					bool flag = item == null;
					if (!flag)
					{
						item.SetWithoutNotify();
					}
				}
			}
		}

		// Token: 0x0600A0AA RID: 41130 RVA: 0x004B0218 File Offset: 0x004AE418
		public void Refresh()
		{
			this.UpdateTertiaryFilterUI();
			this.SetFilterItems();
		}

		// Token: 0x0600A0AB RID: 41131 RVA: 0x004B022C File Offset: 0x004AE42C
		public void SwitchPrimaryFilter(EPrimaryFilterType primaryType)
		{
			this._currentPrimaryType = primaryType;
			bool flag = primaryType == EPrimaryFilterType.Character;
			if (!flag)
			{
				this.UpdateTertiaryFilterUI();
			}
		}

		// Token: 0x0600A0AC RID: 41132 RVA: 0x004B0256 File Offset: 0x004AE456
		public void SwitchSecondaryFilter(ESecondaryFilterType secondaryType)
		{
			this._currentSecondaryType = new ESecondaryFilterType?(secondaryType);
			this.UpdateTertiaryFilterUI();
		}

		// Token: 0x0600A0AD RID: 41133 RVA: 0x004B026C File Offset: 0x004AE46C
		private void UpdateTertiaryFilterUI()
		{
			FilterLevelKey key = new FilterLevelKey(this._currentPrimaryType, this._currentSecondaryType);
			int index = FilterLevelKey.FilterLevelKeyDict[key];
			this.secondaryFilterToggleGroup.transform.parent.gameObject.SetActive(this._currentPrimaryType == EPrimaryFilterType.Character);
			for (int i = 0; i < this.itemHolderRoot.childCount; i++)
			{
				this.itemHolderRoot.GetChild(i).gameObject.SetActive(i == index);
			}
		}

		// Token: 0x0600A0AE RID: 41134 RVA: 0x004B02F4 File Offset: 0x004AE4F4
		public void UpdateMultiSelectData(EFilterItemKey key, int[] selectedIndices)
		{
			bool flag = selectedIndices == null || selectedIndices.Length == 0;
			if (flag)
			{
				this._currentFilterData.MultiSelectData.Remove(key);
				this.GetFindRes();
			}
			else
			{
				IntList data;
				bool flag2 = !this._currentFilterData.MultiSelectData.TryGetValue(key, out data);
				if (flag2)
				{
					this._currentFilterData.MultiSelectData.Add(key, IntList.Create());
					data = this._currentFilterData.MultiSelectData[key];
				}
				bool flag3 = data.Items == null;
				if (flag3)
				{
					data = IntList.Create();
				}
				data.Items.Clear();
				data.Items.AddRange(selectedIndices);
				this.GetFindRes();
			}
		}

		// Token: 0x0600A0AF RID: 41135 RVA: 0x004B03A8 File Offset: 0x004AE5A8
		public void UpdateSingleSelectData(EFilterItemKey key, int selectedIndex)
		{
			bool flag = selectedIndex < 0;
			if (flag)
			{
				this._currentFilterData.SingleSelectData.Remove(key);
			}
			else
			{
				this._currentFilterData.SingleSelectData[key] = selectedIndex;
			}
			bool flag2 = key == EFilterItemKey.CharacterAttributeType;
			if (flag2)
			{
				this.UpdateSkillTypeVisibility();
			}
			this.GetFindRes();
		}

		// Token: 0x0600A0B0 RID: 41136 RVA: 0x004B0404 File Offset: 0x004AE604
		public void UpdateSkillTypeVisibility()
		{
			int characterAttributeType = this._currentFilterData.SingleSelectData.GetValueOrDefault(EFilterItemKey.CharacterAttributeType, -1);
			foreach (MapBlockFindItemBase item in ViewFindMapBlock._filterItemDict[FilterLevelKey.CharacterAttribute])
			{
				bool flag = item == null;
				if (!flag)
				{
					EFilterItemKey efilterItemKey = item.EFilterItemKey;
					bool flag2 = efilterItemKey == EFilterItemKey.CharacterCombatSkillType || efilterItemKey == EFilterItemKey.CharacterLifeSkillType;
					if (flag2)
					{
						item.gameObject.SetActive(characterAttributeType == (int)(item.EFilterItemKey - EFilterItemKey.CharacterCombatSkillType));
					}
				}
			}
		}

		// Token: 0x0600A0B1 RID: 41137 RVA: 0x004B04C0 File Offset: 0x004AE6C0
		public void UpdateSingleSliderData(EFilterItemKey key, int value)
		{
			this._currentFilterData.SingleSliderData[key] = value;
			this.GetFindRes();
		}

		// Token: 0x0600A0B2 RID: 41138 RVA: 0x004B04DD File Offset: 0x004AE6DD
		public void UpdateRangeSliderData(EFilterItemKey key, IntPair range)
		{
			this._currentFilterData.RangeSliderData[key] = range;
			this.GetFindRes();
		}

		// Token: 0x0600A0B3 RID: 41139 RVA: 0x004B04FA File Offset: 0x004AE6FA
		public void UpdateToggleSliderData(EFilterItemKey key, ToggleSliderValue value)
		{
			this._currentFilterData.ToggleSliderData[key] = value;
			this.GetFindRes();
		}

		// Token: 0x0600A0B4 RID: 41140 RVA: 0x004B0517 File Offset: 0x004AE717
		public void ClearCurrentFilter()
		{
			this._currentFilterData.Clear();
			this.OnFilterDataChanged();
			this.Refresh();
		}

		// Token: 0x0600A0B5 RID: 41141 RVA: 0x004B0534 File Offset: 0x004AE734
		public void AddLocationsToTemporaryMarkList(List<Location> locations)
		{
			bool flag = !this._isOn;
			if (!flag)
			{
				SingletonObject.getInstance<WorldMapModel>().AddLocationsToTemporaryMarkList(locations);
				this.resLabel.gameObject.SetActive(true);
				bool flag2 = locations != null && locations.Count > 0;
				if (flag2)
				{
					this.resLabel.text = LanguageKey.LK_FindMapBlock_Res_Find.TrFormat(locations.Count);
				}
				else
				{
					this.resLabel.text = LanguageKey.LK_FindMapBlock_Res_Null.Tr();
				}
			}
		}

		// Token: 0x0600A0B6 RID: 41142 RVA: 0x004B05C0 File Offset: 0x004AE7C0
		private void OnSwitchToggleChanged(bool isOn)
		{
			this._isOn = isOn;
			bool flag = !this._isOn;
			if (flag)
			{
				this.ClearAllTemporaryMarkList();
			}
			else
			{
				this.GetFindRes();
			}
			this.DispatchEvent();
		}

		// Token: 0x0600A0B7 RID: 41143 RVA: 0x004B05FD File Offset: 0x004AE7FD
		public void ClearAllTemporaryMarkList()
		{
			this.resLabel.gameObject.SetActive(false);
			SingletonObject.getInstance<WorldMapModel>().ClearAllTemporaryMarkList();
		}

		// Token: 0x0600A0B8 RID: 41144 RVA: 0x004B0620 File Offset: 0x004AE820
		internal void ResetFilterItem(EFilterItemKey eFilterItemKey)
		{
			FilterItemConfig itemConfig = this._uiConfig.ItemConfigs[eFilterItemKey];
			switch (itemConfig.Type)
			{
			case EFilterElementType.SingleSelect:
			{
				this._currentFilterData.SingleSelectData.Remove(eFilterItemKey);
				bool flag = eFilterItemKey == EFilterItemKey.CharacterAttributeType;
				if (flag)
				{
					this.UpdateSkillTypeVisibility();
				}
				break;
			}
			case EFilterElementType.MultiSelect:
				this._currentFilterData.MultiSelectData.Remove(eFilterItemKey);
				break;
			case EFilterElementType.SingleSlider:
				this._currentFilterData.SingleSliderData.Remove(eFilterItemKey);
				break;
			case EFilterElementType.RangeSlider:
				this._currentFilterData.RangeSliderData.Remove(eFilterItemKey);
				break;
			case EFilterElementType.ToggleSlider:
				this._currentFilterData.ToggleSliderData.Remove(eFilterItemKey);
				break;
			}
			this.OnFilterDataChanged();
		}

		// Token: 0x0600A0B9 RID: 41145 RVA: 0x004B06E2 File Offset: 0x004AE8E2
		private void GetFindRes()
		{
			this.OnFilterDataChanged();
		}

		// Token: 0x0600A0BA RID: 41146 RVA: 0x004B06EC File Offset: 0x004AE8EC
		private void DispatchEvent()
		{
			ArgumentBox arg = EasyPool.Get<ArgumentBox>();
			arg.Set("PresetIndex", this._currentPresetIndex);
			arg.Set<MapBlockFindData>("FilterData", this._currentFilterData);
			arg.Set("ToggleState", this._isOn);
			GEvent.OnEvent(UiEvents.MapBlockChange, arg);
		}

		// Token: 0x0600A0BB RID: 41147 RVA: 0x004B0748 File Offset: 0x004AE948
		private void InitializeUIConfig()
		{
			string[] gradeOptions = new string[9];
			for (int i = 0; i < 9; i++)
			{
				gradeOptions[i] = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", i)).SetGradeColor(i);
			}
			string[] relationOptions = new string[10];
			for (int j = 0; j < 10; j++)
			{
				relationOptions[j] = LocalStringManager.Get(string.Format("LK_FindMapBlock_Relation_{0}", j));
			}
			this.AddItemConfig(EFilterItemKey.CharacterRelation, LanguageKey.LK_FindMapBlock_Chracter_Relation.Tr(), EFilterElementType.MultiSelect, relationOptions, null);
			this.AddItemConfig(EFilterItemKey.CharacterIdentity, LanguageKey.LK_FindMapBlock_Chracter_Identity.Tr(), EFilterElementType.MultiSelect, new string[]
			{
				LanguageKey.LK_FindMapBlock_Chracter_Identity_Sect.Tr(),
				LanguageKey.LK_FindMapBlock_Chracter_Identity_Civilian.Tr(),
				LanguageKey.LK_FindMapBlock_Chracter_Identity_Villager.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.CharacterRank, LanguageKey.LK_FindMapBlock_Chracter_Grade.Tr(), EFilterElementType.MultiSelect, gradeOptions, null);
			this.AddItemConfig(EFilterItemKey.CharacterFollow, LanguageKey.LK_FindMapBlock_Chracter_Focus.Tr(), EFilterElementType.SingleSelect, new string[]
			{
				LanguageKey.LK_FindMapBlock_Chracter_Yes.Tr(),
				LanguageKey.LK_FindMapBlock_Chracter_No.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.CharacterAge, LanguageKey.LK_FindMapBlock_Chracter_Age.Tr(), EFilterElementType.RangeSlider, null, new IntPair?(new IntPair(0, 200)));
			this.AddItemConfig(EFilterItemKey.CharacterGender, LanguageKey.LK_FindMapBlock_Chracter_Gender.Tr(), EFilterElementType.MultiSelect, new string[]
			{
				LanguageKey.LK_FindMapBlock_Chracter_Male.Tr(),
				LanguageKey.LK_FindMapBlock_Chracter_Female.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.CharacterSpouse, LanguageKey.LK_FindMapBlock_Chracter_Married.Tr(), EFilterElementType.SingleSelect, new string[]
			{
				LanguageKey.LK_FindMapBlock_Chracter_Married_Exsit.Tr(),
				LanguageKey.LK_FindMapBlock_Chracter_Married_None.Tr(),
				LanguageKey.LK_FindMapBlock_Chracter_Married_Dead.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.CharacterReincarnation, LanguageKey.LK_FindMapBlock_Chracter_Samsara.Tr(), EFilterElementType.ToggleSlider, null, new IntPair?(new IntPair(0, 9)));
			this.AddItemConfig(EFilterItemKey.CharacterFavorability, LanguageKey.LK_Favorability.Tr(), EFilterElementType.RangeSlider, null, new IntPair?(new IntPair(-6, 6)));
			string[] moodOptions = new string[]
			{
				LanguageKey.LK_FindMapBlock_Chracter_Mood_0.Tr(),
				LanguageKey.LK_FindMapBlock_Chracter_Mood_1.Tr(),
				LanguageKey.LK_FindMapBlock_Chracter_Mood_2.Tr(),
				LanguageKey.LK_FindMapBlock_Chracter_Mood_3.Tr(),
				LanguageKey.LK_FindMapBlock_Chracter_Mood_4.Tr(),
				LanguageKey.LK_FindMapBlock_Chracter_Mood_5.Tr(),
				LanguageKey.LK_FindMapBlock_Chracter_Mood_6.Tr()
			};
			this.AddItemConfig(EFilterItemKey.CharacterMood, LanguageKey.LK_VillagerInfo_Happiness.Tr(), EFilterElementType.MultiSelect, moodOptions, null);
			this.AddItemConfig(EFilterItemKey.CharacterHealth, LanguageKey.LK_Health.Tr(), EFilterElementType.ToggleSlider, null, new IntPair?(new IntPair(0, 100)));
			this.AddItemConfig(EFilterItemKey.CharacterBreath, LanguageKey.LK_FindMapBlock_Chracter_Breath.Tr(), EFilterElementType.ToggleSlider, null, new IntPair?(new IntPair(0, 800)));
			this.AddItemConfig(EFilterItemKey.CharacterInjury, LanguageKey.LK_Injury.Tr(), EFilterElementType.ToggleSlider, null, new IntPair?(new IntPair(0, 30)));
			this.AddItemConfig(EFilterItemKey.CharacterPoison, LanguageKey.LK_Poison.Tr(), EFilterElementType.ToggleSlider, null, new IntPair?(new IntPair(0, 18)));
			this.AddItemConfig(EFilterItemKey.CharacterTrait, LanguageKey.LK_FindMapBlock_Chracter_Trait.Tr(), EFilterElementType.SingleSelect, new string[]
			{
				LanguageKey.LK_FindMapBlock_Chracter_Trait_Pregnant.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.CharacterAttributeType, LanguageKey.LK_FindMapBlock_Chracter_Tab_Property.Tr(), EFilterElementType.SingleSelect, new string[]
			{
				LanguageKey.LK_CombatSkill_2.Tr(),
				LanguageKey.LK_LifeSkill.Tr()
			}, null);
			string[] combatSkillTypeOptions = new string[14];
			for (int k = 0; k < 14; k++)
			{
				combatSkillTypeOptions[k] = LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", k));
			}
			this.AddItemConfig(EFilterItemKey.CharacterCombatSkillType, LanguageKey.LK_CombatSkill_2.Tr(), EFilterElementType.MultiSelect, combatSkillTypeOptions, null);
			string[] lifeSkillTypeOptions = new string[16];
			for (int l = 0; l < 16; l++)
			{
				lifeSkillTypeOptions[l] = LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", l));
			}
			this.AddItemConfig(EFilterItemKey.CharacterLifeSkillType, LanguageKey.LK_LifeSkill.Tr(), EFilterElementType.MultiSelect, lifeSkillTypeOptions, null);
			this.AddItemConfig(EFilterItemKey.CharacterAptitude, LanguageKey.LK_Qualification.Tr(), EFilterElementType.SingleSlider, null, new IntPair?(new IntPair(0, 999)));
			this.AddItemConfig(EFilterItemKey.CharacterAchievement, LanguageKey.LK_Attainment.Tr(), EFilterElementType.SingleSlider, null, new IntPair?(new IntPair(0, 999)));
			this.AddItemConfig(EFilterItemKey.CharacterGrowth, LanguageKey.LK_FindMapBlock_Chracter_Growth.Tr(), EFilterElementType.MultiSelect, new string[]
			{
				LanguageKey.LK_Qualification_Growth_Precocious.Tr(),
				LanguageKey.LK_Qualification_Growth_Average.Tr(),
				LanguageKey.LK_Qualification_Growth_LateBlooming.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.CharacterStrength, LanguageKey.LK_Main_Attribute_Strength.Tr(), EFilterElementType.SingleSlider, null, new IntPair?(new IntPair(0, 100)));
			this.AddItemConfig(EFilterItemKey.CharacterDexterity, LanguageKey.LK_Main_Attribute_Dexterity.Tr(), EFilterElementType.SingleSlider, null, new IntPair?(new IntPair(0, 100)));
			this.AddItemConfig(EFilterItemKey.CharacterConcentration, LanguageKey.LK_Main_Attribute_Concentration.Tr(), EFilterElementType.SingleSlider, null, new IntPair?(new IntPair(0, 100)));
			this.AddItemConfig(EFilterItemKey.CharacterVitality, LanguageKey.LK_Main_Attribute_Vitality.Tr(), EFilterElementType.SingleSlider, null, new IntPair?(new IntPair(0, 100)));
			this.AddItemConfig(EFilterItemKey.CharacterEnergy, LanguageKey.LK_Main_Attribute_Energy.Tr(), EFilterElementType.SingleSlider, null, new IntPair?(new IntPair(0, 100)));
			this.AddItemConfig(EFilterItemKey.CharacterIntelligence, LanguageKey.LK_Main_Attribute_Intelligence.Tr(), EFilterElementType.SingleSlider, null, new IntPair?(new IntPair(0, 100)));
			this.AddItemConfig(EFilterItemKey.MerchantType, LanguageKey.LK_FindMapBlock_Merchant_Type.Tr(), EFilterElementType.MultiSelect, new string[]
			{
				LanguageKey.LK_MerchantInfo_CurrentTog_Merchant.Tr(),
				LanguageKey.LK_Caravan.Tr(),
				LanguageKey.LK_Merchant.Tr()
			}, null);
			string[] merchantGuildOptions = new string[]
			{
				LanguageKey.LK_FindMapBlock_Merchant_Name_0.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Name_1.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Name_2.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Name_3.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Name_4.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Name_5.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Name_6.Tr()
			};
			this.AddItemConfig(EFilterItemKey.MerchantGuildType, LanguageKey.LK_FindMapBlock_Merchant_GuildType.Tr(), EFilterElementType.MultiSelect, merchantGuildOptions, null);
			string[] merchantLevelOptions = new string[]
			{
				LanguageKey.LK_FindMapBlock_Merchant_Level_0.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Level_1.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Level_2.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Level_3.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Level_4.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Level_5.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Level_6.Tr()
			};
			this.AddItemConfig(EFilterItemKey.MerchantGuildRank, LanguageKey.LK_FindMapBlock_Merchant_GuildRank.Tr(), EFilterElementType.MultiSelect, merchantLevelOptions, null);
			this.AddItemConfig(EFilterItemKey.MerchantCaravanStatus, LanguageKey.LK_FindMapBlock_Merchant_CaravanStatus.Tr(), EFilterElementType.SingleSelect, new string[]
			{
				LanguageKey.LK_FindMapBlock_Merchant_Caravan_Normal.Tr(),
				LanguageKey.LK_FindMapBlock_Merchant_Caravan_Robbed.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.GraveRank, LanguageKey.LK_FindMapBlock_Grade.Tr(), EFilterElementType.MultiSelect, gradeOptions, null);
			this.AddItemConfig(EFilterItemKey.GraveInteraction, LanguageKey.LK_FindMapBlock_Grave_Interaction.Tr(), EFilterElementType.SingleSelect, new string[]
			{
				LanguageKey.LK_FindMapBlock_Grave_Interaction_Worship.Tr(),
				LanguageKey.LK_FindMapBlock_Grave_Interaction_Repair.Tr(),
				LanguageKey.LK_FindMapBlock_Grave_Interaction_Rob.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.GraveDurability, LanguageKey.LK_FindMapBlock_Durability.Tr(), EFilterElementType.SingleSlider, null, new IntPair?(new IntPair(1, 100)));
			this.AddItemConfig(EFilterItemKey.GraveHasRelation, LanguageKey.LK_FindMapBlock_Grave_HasRelation.Tr(), EFilterElementType.SingleSelect, new string[]
			{
				LanguageKey.LK_FindMapBlock_Grave_HasRelation_Yes.Tr(),
				LanguageKey.LK_FindMapBlock_Grave_HasRelation_No.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.GraveRelation, LanguageKey.LK_FindMapBlock_Grave_RelationType.Tr(), EFilterElementType.MultiSelect, relationOptions, null);
			this.AddItemConfig(EFilterItemKey.BeastType, LanguageKey.LK_FindMapBlock_Beast_Type.Tr(), EFilterElementType.MultiSelect, new string[]
			{
				LanguageKey.LK_FindMapBlock_Type_Animal.Tr(),
				LanguageKey.LK_Jiao.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.BeastRank, LanguageKey.LK_FindMapBlock_Chracter_Grade.Tr(), EFilterElementType.MultiSelect, gradeOptions, null);
			this.AddItemConfig(EFilterItemKey.BeastStatus, LanguageKey.LK_FindMapBlock_Status.Tr(), EFilterElementType.SingleSelect, new string[]
			{
				LanguageKey.LK_FindMapBlock_Beast_Wild.Tr(),
				LanguageKey.LK_FindMapBlock_Beast_Escaped.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.TerrainResourceType, LanguageKey.LK_FindMapBlock_Terrain_ResourceType.Tr(), EFilterElementType.SingleSelect, new string[]
			{
				LanguageKey.LK_ItemSubType_500.Tr(),
				LanguageKey.LK_ItemSubType_501.Tr(),
				LanguageKey.LK_ItemSubType_502.Tr(),
				LanguageKey.LK_ItemSubType_503.Tr(),
				LanguageKey.LK_ItemSubType_504.Tr(),
				LanguageKey.LK_ItemSubType_505.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.TerrainResourceAmount, LanguageKey.LK_FindMapBlock_Terrain_ResourceAmount.Tr(), EFilterElementType.RangeSlider, null, new IntPair?(new IntPair(0, 300)));
			this.AddItemConfig(EFilterItemKey.TerrainMigration, LanguageKey.LK_FindMapBlock_Terrain_Migration.Tr(), EFilterElementType.SingleSelect, new string[]
			{
				LanguageKey.LK_FindMapBlock_Terrain_Migration_Can.Tr(),
				LanguageKey.LK_FindMapBlock_Terrain_Migration_Cannot.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.TerrainStatus, LanguageKey.LK_FindMapBlock_Status.Tr(), EFilterElementType.SingleSelect, new string[]
			{
				LanguageKey.LK_FindMapBlock_Terrain_Normal.Tr(),
				LanguageKey.LK_FindMapBlock_Terrain_Destroyed.Tr()
			}, null);
			this.AddItemConfig(EFilterItemKey.TerrainExcavation, LanguageKey.LK_FindMapBlock_Terrain_Excavation.Tr(), EFilterElementType.ToggleSlider, null, new IntPair?(new IntPair(1, 9)));
			Dictionary<FilterLevelKey, EFilterItemKey[]> tertiaryConfigs = this._uiConfig.TertiaryConfigs;
			FilterLevelKey key = new FilterLevelKey(EPrimaryFilterType.Character, new ESecondaryFilterType?(ESecondaryFilterType.Identity));
			EFilterItemKey[] array = new EFilterItemKey[8];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.8A851FF82EE7048AD09EC3847F1DDF44944104D2CBD17EF4E3DB22C6785A0D45).FieldHandle);
			tertiaryConfigs[key] = array;
			Dictionary<FilterLevelKey, EFilterItemKey[]> tertiaryConfigs2 = this._uiConfig.TertiaryConfigs;
			FilterLevelKey key2 = new FilterLevelKey(EPrimaryFilterType.Character, new ESecondaryFilterType?(ESecondaryFilterType.Status));
			EFilterItemKey[] array2 = new EFilterItemKey[7];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.E804D61857F4288C66232F7FF5789C0D7E5D2DBBD854AB2FA98636AB77BDC050).FieldHandle);
			tertiaryConfigs2[key2] = array2;
			Dictionary<FilterLevelKey, EFilterItemKey[]> tertiaryConfigs3 = this._uiConfig.TertiaryConfigs;
			FilterLevelKey key3 = new FilterLevelKey(EPrimaryFilterType.Character, new ESecondaryFilterType?(ESecondaryFilterType.Attribute));
			EFilterItemKey[] array3 = new EFilterItemKey[12];
			RuntimeHelpers.InitializeArray(array3, fieldof(<PrivateImplementationDetails>.89F5C22094913B503FA92958E6251C66B3FCE52095523E9F21B3E0CC03F63B7C).FieldHandle);
			tertiaryConfigs3[key3] = array3;
			Dictionary<FilterLevelKey, EFilterItemKey[]> tertiaryConfigs4 = this._uiConfig.TertiaryConfigs;
			FilterLevelKey key4 = new FilterLevelKey(EPrimaryFilterType.Merchant, null);
			EFilterItemKey[] array4 = new EFilterItemKey[4];
			RuntimeHelpers.InitializeArray(array4, fieldof(<PrivateImplementationDetails>.DC80166374A48022BC6FEC7300D4CEE8375653E971AC55B55E07A21C7B07F1D2).FieldHandle);
			tertiaryConfigs4[key4] = array4;
			Dictionary<FilterLevelKey, EFilterItemKey[]> tertiaryConfigs5 = this._uiConfig.TertiaryConfigs;
			FilterLevelKey key5 = new FilterLevelKey(EPrimaryFilterType.Terrain, null);
			EFilterItemKey[] array5 = new EFilterItemKey[5];
			RuntimeHelpers.InitializeArray(array5, fieldof(<PrivateImplementationDetails>.FAFA8E00235BA0A0C7867A908FD5774AEBBB9E791379488CA073A03AB9908E99).FieldHandle);
			tertiaryConfigs5[key5] = array5;
			Dictionary<FilterLevelKey, EFilterItemKey[]> tertiaryConfigs6 = this._uiConfig.TertiaryConfigs;
			FilterLevelKey key6 = new FilterLevelKey(EPrimaryFilterType.Beast, null);
			EFilterItemKey[] array6 = new EFilterItemKey[3];
			RuntimeHelpers.InitializeArray(array6, fieldof(<PrivateImplementationDetails>.B7191AFFF323DFFDAFBA79CAA52DB0D9494B146FE063D83EE3C0F6763C5392DA).FieldHandle);
			tertiaryConfigs6[key6] = array6;
			Dictionary<FilterLevelKey, EFilterItemKey[]> tertiaryConfigs7 = this._uiConfig.TertiaryConfigs;
			FilterLevelKey key7 = new FilterLevelKey(EPrimaryFilterType.Grave, null);
			EFilterItemKey[] array7 = new EFilterItemKey[5];
			RuntimeHelpers.InitializeArray(array7, fieldof(<PrivateImplementationDetails>.1B8215A63D8F1328B6745CA6B9125C9F64C1609437747220DBAC3BFC03EA8C0B).FieldHandle);
			tertiaryConfigs7[key7] = array7;
		}

		// Token: 0x0600A0BC RID: 41148 RVA: 0x004B12AC File Offset: 0x004AF4AC
		private void AddItemConfig(EFilterItemKey key, string displayName, EFilterElementType type, string[] options = null, IntPair? sliderRange = null)
		{
			FilterItemConfig config = new FilterItemConfig(key, displayName, type)
			{
				Options = options,
				SliderRange = (sliderRange ?? ViewFindMapBlock.defaultSliderRange)
			};
			this._uiConfig.ItemConfigs[key] = config;
		}

		// Token: 0x04007C93 RID: 31891
		[SerializeField]
		private TextMeshProUGUI resLabel;

		// Token: 0x04007C94 RID: 31892
		[SerializeField]
		private CToggle switchToggle;

		// Token: 0x04007C95 RID: 31893
		[SerializeField]
		private CToggleGroup toggleGroupPreset;

		// Token: 0x04007C96 RID: 31894
		[SerializeField]
		private CToggleGroup primaryFilterToggleGroup;

		// Token: 0x04007C97 RID: 31895
		[SerializeField]
		private CButton primaryResetBtn;

		// Token: 0x04007C98 RID: 31896
		[SerializeField]
		private CToggleGroup secondaryFilterToggleGroup;

		// Token: 0x04007C99 RID: 31897
		[SerializeField]
		private CButton secondaryResetBtn;

		// Token: 0x04007C9A RID: 31898
		[SerializeField]
		private RectTransform itemHolderRoot;

		// Token: 0x04007C9B RID: 31899
		[SerializeField]
		private MapBlockFindItem_SingleSelect singleSelectItem;

		// Token: 0x04007C9C RID: 31900
		[SerializeField]
		private MapBlockFindItem_MultiSelect multiSelectItem;

		// Token: 0x04007C9D RID: 31901
		[SerializeField]
		private MapBlockFindItem_SingleSlider singleSliderItem;

		// Token: 0x04007C9E RID: 31902
		[SerializeField]
		private MapBlockFindItem_ToggleSlider toggleSliderItem;

		// Token: 0x04007C9F RID: 31903
		[SerializeField]
		private MapBlockFindItem_RangeSlider rangeSliderItem;

		// Token: 0x04007CA0 RID: 31904
		private static readonly IntPair defaultSliderRange = new IntPair(0, 0);

		// Token: 0x04007CA1 RID: 31905
		private static readonly Dictionary<FilterLevelKey, List<MapBlockFindItemBase>> _filterItemDict = new Dictionary<FilterLevelKey, List<MapBlockFindItemBase>>
		{
			{
				FilterLevelKey.CharacterAttribute,
				new List<MapBlockFindItemBase>()
			},
			{
				FilterLevelKey.CharacterIdentity,
				new List<MapBlockFindItemBase>()
			},
			{
				FilterLevelKey.CharacterStatus,
				new List<MapBlockFindItemBase>()
			},
			{
				FilterLevelKey.Merchant,
				new List<MapBlockFindItemBase>()
			},
			{
				FilterLevelKey.Grave,
				new List<MapBlockFindItemBase>()
			},
			{
				FilterLevelKey.Beast,
				new List<MapBlockFindItemBase>()
			},
			{
				FilterLevelKey.Terrain,
				new List<MapBlockFindItemBase>()
			}
		};

		// Token: 0x04007CA2 RID: 31906
		private MapBlockFindData _currentFilterData;

		// Token: 0x04007CA3 RID: 31907
		private readonly FilterUIConfig _uiConfig = new FilterUIConfig();

		// Token: 0x04007CA4 RID: 31908
		private EPrimaryFilterType _currentPrimaryType = EPrimaryFilterType.Character;

		// Token: 0x04007CA5 RID: 31909
		private ESecondaryFilterType? _currentSecondaryType = new ESecondaryFilterType?(ESecondaryFilterType.Identity);

		// Token: 0x04007CA6 RID: 31910
		private int _currentPresetIndex = 0;

		// Token: 0x04007CA7 RID: 31911
		private bool _isOn;
	}
}
