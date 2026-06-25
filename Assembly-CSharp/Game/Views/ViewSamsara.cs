using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
	// Token: 0x0200070D RID: 1805
	public class ViewSamsara : UIBase
	{
		// Token: 0x0600556D RID: 21869 RVA: 0x0027973C File Offset: 0x0027793C
		public override void OnInit(ArgumentBox argsBox)
		{
			int charId;
			argsBox.Get("charId", out charId);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, charId);
				CharacterDomainMethod.Call.GetCharacterSamsaraData(this.Element.GameDataListenerId, charId);
			}));
		}

		// Token: 0x0600556E RID: 21870 RVA: 0x00279798 File Offset: 0x00277998
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					this.FetchMethodReturnValue(notification.DomainId, notification.MethodId, notification.ValueOffset, wrapper.DataPool);
				}
			}
		}

		// Token: 0x0600556F RID: 21871 RVA: 0x00279820 File Offset: 0x00277A20
		private void FetchMethodReturnValue(ushort domainId, ushort methodId, int returnValueOffset, RawDataPool dataPool)
		{
			if (domainId == 4)
			{
				bool flag = methodId == 131;
				if (flag)
				{
					Serializer.Deserialize(dataPool, returnValueOffset, ref this._characterDisplayData);
					CharacterDisplayData characterDisplayData = this._characterDisplayData;
					short? num = (characterDisplayData != null) ? new short?(characterDisplayData.TemplateId) : null;
					int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
					int num3 = 916;
					this._isRanchenzi = (num2.GetValueOrDefault() == num3 & num2 != null);
				}
				else
				{
					bool flag2 = methodId == 55;
					if (flag2)
					{
						Serializer.Deserialize(dataPool, returnValueOffset, ref this._characterSamsaraData);
						this.Element.ShowAfterRefresh();
						this.Refresh();
					}
				}
			}
		}

		// Token: 0x06005570 RID: 21872 RVA: 0x002798F0 File Offset: 0x00277AF0
		private void Awake()
		{
			this.toggleGroupChar.OnActiveIndexChange += this.ToggleGroupCharOnOnActiveIndexChange;
			this.toggleGroupChar.Init(-1);
			this._characterToggleList = this.toggleGroupChar.GetComponentsInChildren<SamsaraCharacterToggle>().ToList<SamsaraCharacterToggle>();
			this._characterToggleList.ForEach(delegate(SamsaraCharacterToggle e)
			{
				e.SetEmpty();
			});
			this.scrollCombatQualification.OnItemRender += this.ScrollCombatQualificationOnItemRender;
			this.scrollLifeQualification.OnItemRender += this.ScrollLifeQualificationOnItemRender;
		}

		// Token: 0x06005571 RID: 21873 RVA: 0x00279994 File Offset: 0x00277B94
		private void ToggleGroupCharOnOnActiveIndexChange(int newIndex, int oldIndex)
		{
			bool isRanchenzi = this._isRanchenzi;
			if (isRanchenzi)
			{
				this.RefreshCharInfoRanchenzi(newIndex);
			}
			else
			{
				this.RefreshCharInfoNormal(newIndex);
			}
		}

		// Token: 0x06005572 RID: 21874 RVA: 0x002799C0 File Offset: 0x00277BC0
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "ButtonCloseView")
			{
				this.QuickHide();
			}
		}

		// Token: 0x06005573 RID: 21875 RVA: 0x002799F0 File Offset: 0x00277BF0
		private void Refresh()
		{
			bool isRanchenzi = this._isRanchenzi;
			if (isRanchenzi)
			{
				this.RefreshCharToggleGroupRanchenzi();
			}
			else
			{
				this.RefreshCharToggleGroupNormal();
			}
			GlobalDomainMethod.Call.InvokeGuidingTrigger(97);
		}

		// Token: 0x06005574 RID: 21876 RVA: 0x00279A20 File Offset: 0x00277C20
		private bool IsRanchenziSamsaraVisible(int index)
		{
			return SingletonObject.getInstance<BasicGameData>().XiangshuAvatarTaskStatusArray[index].JuniorXiangshuTaskStatus != 0;
		}

		// Token: 0x06005575 RID: 21877 RVA: 0x00279A4C File Offset: 0x00277C4C
		private unsafe void RefreshCharToggleGroupNormal()
		{
			int needAutoOnIndex = -1;
			PreexistenceCharIds preexistenceCharIds = this._characterSamsaraData.PreexistenceCharIds;
			for (int i = 0; i < 9; i++)
			{
				DeadCharacter deadCharacter = this._characterSamsaraData.DeadCharacters.GetOrDefault(i);
				int pos = PreexistenceCharIds.Positions[i];
				SamsaraCharacterToggle characterToggle = this._characterToggleList[pos];
				characterToggle.SetIndex(i);
				bool hasPreLife = deadCharacter != null;
				bool flag = !hasPreLife;
				if (flag)
				{
					characterToggle.SetEmpty();
					this.toggleGroupChar.SetInteractable(false, pos);
				}
				else
				{
					int index = preexistenceCharIds.GetIndexByPos(pos);
					int charId = *(ref preexistenceCharIds.CharIds.FixedElementField + (IntPtr)index * 4);
					SingletonObject.getInstance<CharacterMonitorModel>().AddDeadCharacterCache(charId, deadCharacter);
					this.toggleGroupChar.SetInteractable(true, pos);
					NameRelatedData nameRelatedData = this._characterSamsaraData.DeadCharacterNames[i];
					characterToggle.Set(deadCharacter, ref nameRelatedData);
					bool flag2 = index == 0;
					if (flag2)
					{
						needAutoOnIndex = pos;
					}
				}
			}
			bool flag3 = needAutoOnIndex >= 0;
			if (flag3)
			{
				this.toggleGroupChar.Set(needAutoOnIndex, true);
			}
		}

		// Token: 0x06005576 RID: 21878 RVA: 0x00279B6C File Offset: 0x00277D6C
		private void RefreshCharToggleGroupRanchenzi()
		{
			int setToIndex = -1;
			for (int i = 0; i < this._characterToggleList.Count; i++)
			{
				SamsaraCharacterToggle controller = this._characterToggleList[i];
				this.toggleGroupChar.SetInteractable(true, i);
				controller.Set(this._ranChenziSamsaraCharacterTemplateIdList[i]);
				bool flag = this.IsRanchenziSamsaraVisible(i);
				if (flag)
				{
					bool flag2 = setToIndex < 0;
					if (flag2)
					{
						setToIndex = i;
					}
					controller.SetAvatarAnchoredPosition(Vector2.down * 30f);
				}
				else
				{
					controller.SetInvisible();
					controller.SetAvatarAnchoredPosition(Vector2.down * 10f);
				}
			}
			setToIndex = Mathf.Max(0, setToIndex);
			this.toggleGroupChar.Set(setToIndex, true);
		}

		// Token: 0x06005577 RID: 21879 RVA: 0x00279C40 File Offset: 0x00277E40
		private string GetBirthYearString(int date)
		{
			int birthYear = SingletonObject.getInstance<TimeManager>().GetYearByDate(date);
			return (birthYear < 0) ? LanguageKey.LK_Genealogy_BirthAndDeath_LessThanZero.TrFormat(-birthYear) : birthYear.ToString();
		}

		// Token: 0x06005578 RID: 21880 RVA: 0x00279C80 File Offset: 0x00277E80
		private unsafe void RefreshCharInfoNormal(int pos)
		{
			int index = this._characterSamsaraData.PreexistenceCharIds.GetIndexByPos(pos);
			DeadCharacter deadCharacter = this._characterSamsaraData.DeadCharacters[index];
			CharacterItem charConfig = Character.Instance.GetItem(deadCharacter.TemplateId);
			OrganizationItem orgConfig = Config.Organization.Instance[deadCharacter.OrganizationInfo.OrgTemplateId];
			this.avatar.Refresh(deadCharacter.GenerateAvatarRelatedData(), deadCharacter.TemplateId);
			NameRelatedData nameRelatedData = this._characterSamsaraData.DeadCharacterNames[index];
			this.textCharName.text = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, false, true);
			string birthText = this.GetBirthYearString(deadCharacter.BirthDate);
			string deathText = this.GetBirthYearString(deadCharacter.DeathDate);
			this.textCharLife.text = LanguageKey.LK_Genealogy_BirthAndDeath_B.TrFormat(birthText, deathText);
			string title = LanguageKey.LK_Main_SummaryInfo_Behavior.Tr();
			sbyte behaviorType = GameData.Domains.Character.BehaviorType.GetBehaviorType(deadCharacter.Morality);
			string icon = CommonUtils.GetBehaviorTypeBigIcon(behaviorType);
			string value = CommonUtils.GetBehaviorString(behaviorType);
			this.RefreshCharInfoProperty(this.charInfoContainer, ViewSamsara.ECharInfoType.Behavior, icon, title, value);
			string title2 = LanguageKey.LK_Main_SummaryInfo_Organization.Tr();
			string icon2 = CommonUtils.GetOrganizationIcon((short)deadCharacter.OrganizationInfo.OrgTemplateId);
			string value2 = SingletonObject.getInstance<WorldMapModel>().GetSettlementName(deadCharacter.OrganizationInfo);
			this.RefreshCharInfoProperty(this.charInfoContainer, ViewSamsara.ECharInfoType.Organization, icon2, title2, value2);
			string title3 = LanguageKey.LK_Main_SummaryInfo_Identity.Tr();
			string icon3 = CommonUtils.GetIdentityIconName(deadCharacter.OrganizationInfo.Grade);
			string value3 = CommonUtils.GetIdentityString(deadCharacter.OrganizationInfo, deadCharacter.Gender, deadCharacter.CurrAge, false);
			this.RefreshCharInfoProperty(this.charInfoContainer, ViewSamsara.ECharInfoType.Identify, icon3, title3, value3);
			string title4 = LanguageKey.LK_Main_SummaryInfo_Charm.Tr();
			AvatarRelatedData avatarRelatedData = deadCharacter.GenerateAvatarRelatedData();
			bool isFixedPresetType = CreatingType.IsFixedPresetType(charConfig.CreatingType);
			string icon4 = CommonUtils.GetCharmLevelBigIcon(deadCharacter.Attraction, deadCharacter.GetActualAge(), avatarRelatedData.AvatarData.ClothDisplayId, avatarRelatedData.AvatarData.FaceVisible, isFixedPresetType);
			string value4 = CommonUtils.GetCharmLevelText(deadCharacter.Attraction, deadCharacter.Gender, deadCharacter.GetActualAge(), avatarRelatedData.ClothingDisplayId, isFixedPresetType, true);
			this.RefreshCharInfoProperty(this.charInfoContainer, ViewSamsara.ECharInfoType.Charm, icon4, title4, value4);
			string title5 = LanguageKey.LK_Samsara.Tr();
			string icon5 = "ui9_icon_samsara_big";
			string value5 = LocalStringManager.GetFormat(LanguageKey.LK_SamsaraCount, index + 1);
			this.RefreshCharInfoProperty(this.charInfoContainer, ViewSamsara.ECharInfoType.Samsara, icon5, title5, value5);
			string title6 = LanguageKey.LK_Main_SummaryInfo_Fame.Tr();
			string icon6 = "ui9_icon_fame_big_" + CommonUtils.GetFameIconIndex(deadCharacter.FameType).ToString();
			string value6 = CommonUtils.GetFameString(deadCharacter.FameType);
			this.RefreshCharInfoProperty(this.charInfoContainer, ViewSamsara.ECharInfoType.Fame, icon6, title6, value6);
			this._isVisible = true;
			this.RefreshCharCombatSkillQualifications(index);
			this.RefreshCharLifeSkillQualifications(index);
			PreexistenceCharIds preexistenceCharIds = this._characterSamsaraData.PreexistenceCharIds;
			this.featureScroll.Set(deadCharacter.FeatureIds, false, false, null, *(ref preexistenceCharIds.CharIds.FixedElementField + (IntPtr)index * 4));
		}

		// Token: 0x06005579 RID: 21881 RVA: 0x00279F84 File Offset: 0x00278184
		private void RefreshCharInfoRanchenzi(int index)
		{
			CharacterItem charConfig = Character.Instance[this._ranChenziSamsaraCharacterTemplateIdList[index]];
			string unknown = LanguageKey.LK_UnknownCharName.Tr();
			bool isVisible = this.IsRanchenziSamsaraVisible(index);
			bool flag = !isVisible;
			if (flag)
			{
				string resPath = "RemakeResources/Textures/NpcFace/NormalFace/NpcFace_ranchenzi";
				ResLoader.Load<Sprite>(resPath, new Action<Sprite>(this.avatar.Refresh), null, false);
				this.textCharName.text = unknown;
			}
			else
			{
				this.avatar.Refresh(null, charConfig.TemplateId);
				this.textCharName.text = charConfig.Surname + charConfig.GivenName;
			}
			this.textCharLife.text = LanguageKey.LK_Genealogy_BirthAndDeath_B.TrFormat(unknown, unknown);
			OrganizationItem orgConfig = Config.Organization.Instance[charConfig.OrganizationInfo.OrgTemplateId];
			string title = LanguageKey.LK_Main_SummaryInfo_Behavior.Tr();
			sbyte behaviorType = GameData.Domains.Character.BehaviorType.GetBehaviorType(charConfig.BaseMorality);
			string value = isVisible ? CommonUtils.GetBehaviorString(behaviorType) : unknown;
			this.RefreshCharInfoProperty(this.charInfoContainer, ViewSamsara.ECharInfoType.Behavior, null, title, value);
			string title2 = LanguageKey.LK_Main_SummaryInfo_Organization.Tr();
			string value2 = isVisible ? orgConfig.Name : unknown;
			this.RefreshCharInfoProperty(this.charInfoContainer, ViewSamsara.ECharInfoType.Organization, null, title2, value2);
			string title3 = LanguageKey.LK_Main_SummaryInfo_Identity.Tr();
			short memberId = orgConfig.Members[(int)charConfig.OrganizationInfo.Grade];
			string value3 = isVisible ? OrganizationMember.Instance[memberId].GradeName : unknown;
			this.RefreshCharInfoProperty(this.charInfoContainer, ViewSamsara.ECharInfoType.Identify, null, title3, value3);
			string title4 = LanguageKey.LK_Main_SummaryInfo_Charm.Tr();
			bool isFixedPresetType = CreatingType.IsFixedPresetType(charConfig.CreatingType);
			string value4 = isVisible ? CommonUtils.GetCharmLevelText(charConfig.BaseAttraction, charConfig.Gender, charConfig.ActualAge, 0, isFixedPresetType, true) : unknown;
			this.RefreshCharInfoProperty(this.charInfoContainer, ViewSamsara.ECharInfoType.Charm, null, title4, value4);
			this._isVisible = this.IsRanchenziSamsaraVisible(index);
			this.RefreshCharCombatSkillQualifications(index);
			this.RefreshCharLifeSkillQualifications(index);
			this.featureScroll.Set(isVisible ? charConfig.FeatureIds : null, false, false, null, -1);
		}

		// Token: 0x0600557A RID: 21882 RVA: 0x0027A1A0 File Offset: 0x002783A0
		private bool GetShowBack(int childIndex, GridLayoutGroup gridLayoutGroup)
		{
			return childIndex / gridLayoutGroup.constraintCount % 2 == 1;
		}

		// Token: 0x0600557B RID: 21883 RVA: 0x0027A1C0 File Offset: 0x002783C0
		private void RefreshCharInfoProperty(GridLayoutGroup gridLayoutGroup, ViewSamsara.ECharInfoType eCharInfoType, string icon, string title, string value)
		{
			int propertyIndex = eCharInfoType.ToInt();
			PropertyItem propertyItem = gridLayoutGroup.transform.GetChild(propertyIndex).GetComponent<PropertyItem>();
			bool showBack = this.GetShowBack(propertyIndex, gridLayoutGroup);
			propertyItem.Set(icon, title, value, new bool?(showBack), true);
		}

		// Token: 0x0600557C RID: 21884 RVA: 0x0027A208 File Offset: 0x00278408
		private void RefreshCharCombatSkillQualifications(int index)
		{
			bool isRanchenzi = this._isRanchenzi;
			if (isRanchenzi)
			{
				CharacterItem charConfig = Character.Instance[this._ranChenziSamsaraCharacterTemplateIdList[index]];
				this._combatSkillShorts = charConfig.BaseCombatSkillQualifications;
			}
			else
			{
				DeadCharacter deadCharacter = this._characterSamsaraData.DeadCharacters[index];
				this._combatSkillShorts = deadCharacter.BaseCombatSkillQualifications;
			}
			this.scrollCombatQualification.SetDataCount(14);
		}

		// Token: 0x0600557D RID: 21885 RVA: 0x0027A278 File Offset: 0x00278478
		private void ScrollCombatQualificationOnItemRender(int index, GameObject obj)
		{
			PropertyItem propertyItem = obj.GetComponent<PropertyItem>();
			CombatSkillTypeItem config = CombatSkillType.Instance[index];
			string value = this._isVisible ? this._combatSkillShorts[index].ToString() : LanguageKey.LK_UnknownCharName.Tr();
			propertyItem.Set(config.DisplayIconOutLine, config.Name, value, null, false);
		}

		// Token: 0x0600557E RID: 21886 RVA: 0x0027A2E0 File Offset: 0x002784E0
		private void RefreshCharLifeSkillQualifications(int index)
		{
			bool isRanchenzi = this._isRanchenzi;
			if (isRanchenzi)
			{
				CharacterItem charConfig = Character.Instance[this._ranChenziSamsaraCharacterTemplateIdList[index]];
				this._lifeSkillShorts = charConfig.BaseLifeSkillQualifications;
			}
			else
			{
				DeadCharacter deadCharacter = this._characterSamsaraData.DeadCharacters[index];
				this._lifeSkillShorts = deadCharacter.BaseLifeSkillQualifications;
			}
			this.scrollLifeQualification.SetDataCount(16);
		}

		// Token: 0x0600557F RID: 21887 RVA: 0x0027A350 File Offset: 0x00278550
		private void ScrollLifeQualificationOnItemRender(int index, GameObject obj)
		{
			PropertyItem propertyItem = obj.GetComponent<PropertyItem>();
			LifeSkillTypeItem config = Config.LifeSkillType.Instance[index];
			string value = this._isVisible ? this._lifeSkillShorts[index].ToString() : LanguageKey.LK_UnknownCharName.Tr();
			propertyItem.Set(config.DisplayIconOutLine, config.Name, value, null, false);
		}

		// Token: 0x04003A3F RID: 14911
		[SerializeField]
		private CToggleGroup toggleGroupChar;

		// Token: 0x04003A40 RID: 14912
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04003A41 RID: 14913
		[SerializeField]
		private TextMeshProUGUI textCharName;

		// Token: 0x04003A42 RID: 14914
		[SerializeField]
		private TextMeshProUGUI textCharLife;

		// Token: 0x04003A43 RID: 14915
		[SerializeField]
		private GridLayoutGroup charInfoContainer;

		// Token: 0x04003A44 RID: 14916
		[SerializeField]
		private InfinityScroll scrollCombatQualification;

		// Token: 0x04003A45 RID: 14917
		[SerializeField]
		private InfinityScroll scrollLifeQualification;

		// Token: 0x04003A46 RID: 14918
		[SerializeField]
		private FeatureScroll featureScroll;

		// Token: 0x04003A47 RID: 14919
		private CharacterDisplayData _characterDisplayData;

		// Token: 0x04003A48 RID: 14920
		private CharacterSamsaraData _characterSamsaraData;

		// Token: 0x04003A49 RID: 14921
		private bool _isRanchenzi;

		// Token: 0x04003A4A RID: 14922
		private CombatSkillShorts _combatSkillShorts;

		// Token: 0x04003A4B RID: 14923
		private LifeSkillShorts _lifeSkillShorts;

		// Token: 0x04003A4C RID: 14924
		private bool _isVisible;

		// Token: 0x04003A4D RID: 14925
		private List<SamsaraCharacterToggle> _characterToggleList;

		// Token: 0x04003A4E RID: 14926
		private readonly List<short> _ranChenziSamsaraCharacterTemplateIdList = new List<short>
		{
			47,
			56,
			65,
			74,
			83,
			92,
			101,
			110,
			119
		};

		// Token: 0x02001B5F RID: 7007
		private enum ECharInfoType
		{
			// Token: 0x0400BA7D RID: 47741
			Behavior,
			// Token: 0x0400BA7E RID: 47742
			Organization,
			// Token: 0x0400BA7F RID: 47743
			Identify,
			// Token: 0x0400BA80 RID: 47744
			Charm,
			// Token: 0x0400BA81 RID: 47745
			Samsara,
			// Token: 0x0400BA82 RID: 47746
			Fame
		}
	}
}
