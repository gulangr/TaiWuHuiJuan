using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips
{
	// Token: 0x02000891 RID: 2193
	public class ToolTipSoulPiece : MouseTipBase
	{
		// Token: 0x06006931 RID: 26929 RVA: 0x00304F78 File Offset: 0x00303178
		protected override void Init(ArgumentBox argsBox)
		{
			Func<ArgumentBox> getArgsBoxFunc;
			argsBox.Get<Func<ArgumentBox>>("GetArgsBoxFunc", out getArgsBoxFunc);
			argsBox = getArgsBoxFunc();
			bool flag = argsBox == null;
			if (!flag)
			{
				int type;
				argsBox.Get("Type", out type);
				this._type = (ToolTipSoulPieceType)type;
				for (int i = 0; i < this.contentHolder.childCount; i++)
				{
					this.contentHolder.GetChild(i).gameObject.SetActive(false);
				}
				switch (this._type)
				{
				case ToolTipSoulPieceType.Samsara:
					this.samsaraHolder.gameObject.SetActive(true);
					this.titleLabel.text = LanguageKey.LK_MouseTip_Title_ReincarnationInfo.Tr();
					this.SetSamsara(argsBox);
					break;
				case ToolTipSoulPieceType.Feature:
					this.featureHolder.gameObject.SetActive(true);
					this.titleLabel.text = LanguageKey.LK_CharacterFeature.Tr();
					this.SetFeature(argsBox);
					break;
				case ToolTipSoulPieceType.MainAttribute:
					this.mainAttributeHolder.gameObject.SetActive(true);
					this.titleLabel.text = LanguageKey.LK_MouseTip_Title_Attribute.Tr();
					this.SerMainAttribute(argsBox);
					break;
				case ToolTipSoulPieceType.Qualification:
					this.qualificationHolder.gameObject.SetActive(true);
					this.titleLabel.text = LanguageKey.LK_LifeAndCombatQualification.Tr();
					this.SetQualification(argsBox);
					break;
				case ToolTipSoulPieceType.BehaviorAndHappiness:
					this.behaviorAndHappinessHolder.gameObject.SetActive(true);
					this.titleLabel.text = LanguageKey.LK_MouseTip_Title_BehaviorAndHappiness.Tr();
					this.SetBehaviorAndHappiness(argsBox);
					break;
				}
				LayoutElement layoutElement = this.layoutElement;
				ToolTipSoulPieceType type2 = this._type;
				if (!true)
				{
				}
				int num;
				if (type2 != ToolTipSoulPieceType.Samsara)
				{
					if (type2 != ToolTipSoulPieceType.Feature)
					{
						num = 608;
					}
					else
					{
						num = 674;
					}
				}
				else
				{
					num = 626;
				}
				if (!true)
				{
				}
				layoutElement.preferredWidth = (float)num;
			}
		}

		// Token: 0x06006932 RID: 26930 RVA: 0x0030516C File Offset: 0x0030336C
		private unsafe void SetSamsara(ArgumentBox argsBox)
		{
			CharacterSamsaraData characterSamsaraData;
			argsBox.Get<CharacterSamsaraData>("CharacterSamsaraData", out characterSamsaraData);
			bool flag = characterSamsaraData == null;
			if (flag)
			{
				Debug.LogWarning("characterSamsaraData is null");
			}
			else
			{
				List<SamsaraCharacterToggle> characterToggleList = this.samsaraHolder.GetComponentsInChildren<SamsaraCharacterToggle>().ToList<SamsaraCharacterToggle>();
				for (int i = 0; i < 9; i++)
				{
					List<DeadCharacter> deadCharacters = characterSamsaraData.DeadCharacters;
					DeadCharacter deadCharacter = (deadCharacters != null) ? deadCharacters.GetOrDefault(i) : null;
					int pos = PreexistenceCharIds.Positions[i];
					SamsaraCharacterToggle characterToggle = characterToggleList[pos];
					characterToggle.SetIndex(i);
					bool hasPreLife = deadCharacter != null;
					bool flag2 = !hasPreLife;
					if (flag2)
					{
						characterToggle.GetComponent<CToggle>().isOn = false;
						characterToggle.SetEmpty();
					}
					else
					{
						int index = characterSamsaraData.PreexistenceCharIds.GetIndexByPos(pos);
						int charId = *(ref characterSamsaraData.PreexistenceCharIds.CharIds.FixedElementField + (IntPtr)index * 4);
						SingletonObject.getInstance<CharacterMonitorModel>().AddDeadCharacterCache(charId, deadCharacter);
						List<NameRelatedData> deadCharacterNames = characterSamsaraData.DeadCharacterNames;
						NameRelatedData? nameRelatedData = (deadCharacterNames != null) ? new NameRelatedData?(deadCharacterNames.GetOrDefault(i)) : null;
						NameRelatedData name;
						bool flag3;
						if (nameRelatedData != null)
						{
							name = nameRelatedData.GetValueOrDefault();
							flag3 = true;
						}
						else
						{
							flag3 = false;
						}
						bool flag4 = flag3;
						if (flag4)
						{
							characterToggle.Set(deadCharacter, ref name);
						}
						characterToggle.GetComponent<CToggle>().isOn = (index == 0);
					}
				}
			}
		}

		// Token: 0x06006933 RID: 26931 RVA: 0x003052C0 File Offset: 0x003034C0
		private void SetFeature(ArgumentBox argsBox)
		{
			List<short> displayFeatureIds;
			argsBox.Get<List<short>>("DisplayFeatureIds", out displayFeatureIds);
			bool flag = displayFeatureIds == null;
			if (flag)
			{
				Debug.LogWarning("displayFeatureIds is null");
			}
			else
			{
				List<Feature> featureItemList = this.featureHolder.GetComponentsInChildren<Feature>().ToList<Feature>();
				for (int i = 0; i < featureItemList.Count; i++)
				{
					bool flag2 = displayFeatureIds.CheckIndex(i) && displayFeatureIds[i] >= 0;
					if (flag2)
					{
						featureItemList[i].gameObject.SetActive(true);
						featureItemList[i].Set(displayFeatureIds[i], -1, false, -1);
					}
					else
					{
						featureItemList[i].gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06006934 RID: 26932 RVA: 0x00305380 File Offset: 0x00303580
		private void SerMainAttribute(ArgumentBox argsBox)
		{
			MainAttributes baseMainAttributes;
			argsBox.Get<MainAttributes>("BaseMainAttributes", out baseMainAttributes);
			List<TooltipItemProperty> mainAttributesItemList = this.mainAttributeHolder.GetComponentsInChildren<TooltipItemProperty>().ToList<TooltipItemProperty>();
			for (int i = 0; i < mainAttributesItemList.Count; i++)
			{
				CharacterPropertyDisplayItem characterPropertyItem = CharacterPropertyDisplay.Instance[6 + i];
				mainAttributesItemList[i].Set(characterPropertyItem.TipsIcon, characterPropertyItem.Name, (ref baseMainAttributes.Items.FixedElementField + (IntPtr)i * 2).ToString(), true);
			}
		}

		// Token: 0x06006935 RID: 26933 RVA: 0x00305404 File Offset: 0x00303604
		private unsafe void SetQualification(ArgumentBox argsBox)
		{
			LifeSkillShorts lifeSkillShorts;
			argsBox.Get<LifeSkillShorts>("LifeSkillShorts", out lifeSkillShorts);
			List<TooltipItemProperty> lifeSkillShortsItemList = this.lifeSkillHolder.GetComponentsInChildren<TooltipItemProperty>().ToList<TooltipItemProperty>();
			for (int i = 0; i < lifeSkillShortsItemList.Count; i++)
			{
				CharacterPropertyDisplayItem characterPropertyItem = CharacterPropertyDisplay.Instance[50 + i];
				lifeSkillShortsItemList[i].Set(characterPropertyItem.TipsIcon, characterPropertyItem.Name, (ref lifeSkillShorts.Items.FixedElementField + (IntPtr)i * 2).ToString().SetColor(CommonUtils.GetCharacterSkillColorByValue(*(ref lifeSkillShorts.Items.FixedElementField + (IntPtr)i * 2))), true);
			}
			CombatSkillShorts combatSkillShorts;
			argsBox.Get<CombatSkillShorts>("CombatSkillShorts", out combatSkillShorts);
			List<TooltipItemProperty> combatSkillShortsItemList = this.combatSkillHolder.GetComponentsInChildren<TooltipItemProperty>().ToList<TooltipItemProperty>();
			for (int j = 0; j < combatSkillShortsItemList.Count; j++)
			{
				CharacterPropertyDisplayItem characterPropertyItem2 = CharacterPropertyDisplay.Instance[80 + j];
				combatSkillShortsItemList[j].Set(characterPropertyItem2.TipsIcon, characterPropertyItem2.Name, (ref combatSkillShorts.Items.FixedElementField + (IntPtr)j * 2).ToString().SetColor(CommonUtils.GetCharacterSkillColorByValue(*(ref combatSkillShorts.Items.FixedElementField + (IntPtr)j * 2))), true);
			}
		}

		// Token: 0x06006936 RID: 26934 RVA: 0x00305550 File Offset: 0x00303750
		private void SetBehaviorAndHappiness(ArgumentBox argsBox)
		{
			sbyte behaviorType;
			argsBox.Get("BehaviorType", out behaviorType);
			sbyte happinessLevel;
			argsBox.Get("HappinessLevel", out happinessLevel);
			TooltipItemProperty component = this.behaviorAndHappinessHolder.GetChild(0).GetComponent<TooltipItemProperty>();
			string behaviorTypeIcon = CommonUtils.GetBehaviorTypeIcon(behaviorType);
			string title = LocalStringManager.Get(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_1);
			string str = "LK_Goodness_";
			int num = (int)behaviorType;
			component.Set(behaviorTypeIcon, title, LocalStringManager.Get(str + num.ToString()).ColorReplace(), true);
			TooltipItemProperty component2 = this.behaviorAndHappinessHolder.GetChild(1).GetComponent<TooltipItemProperty>();
			string happinessIconName = CommonUtils.GetHappinessIconName(happinessLevel);
			string title2 = LocalStringManager.Get(LanguageKey.LK_VillagerInfo_Happiness);
			string str2 = "LK_HappinessLevel_";
			num = (int)happinessLevel;
			component2.Set(happinessIconName, title2, LocalStringManager.Get(str2 + num.ToString()).ColorReplace(), true);
		}

		// Token: 0x04004B5B RID: 19291
		[SerializeField]
		private LayoutElement layoutElement;

		// Token: 0x04004B5C RID: 19292
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x04004B5D RID: 19293
		[SerializeField]
		private RectTransform contentHolder;

		// Token: 0x04004B5E RID: 19294
		[SerializeField]
		private RectTransform samsaraHolder;

		// Token: 0x04004B5F RID: 19295
		[SerializeField]
		private RectTransform featureHolder;

		// Token: 0x04004B60 RID: 19296
		[SerializeField]
		private RectTransform mainAttributeHolder;

		// Token: 0x04004B61 RID: 19297
		[SerializeField]
		private RectTransform lifeSkillHolder;

		// Token: 0x04004B62 RID: 19298
		[SerializeField]
		private RectTransform combatSkillHolder;

		// Token: 0x04004B63 RID: 19299
		[SerializeField]
		private RectTransform qualificationHolder;

		// Token: 0x04004B64 RID: 19300
		[SerializeField]
		private RectTransform behaviorAndHappinessHolder;

		// Token: 0x04004B65 RID: 19301
		private ToolTipSoulPieceType _type;
	}
}
