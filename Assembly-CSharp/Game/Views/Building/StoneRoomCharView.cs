using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BE6 RID: 3046
	public class StoneRoomCharView : MonoBehaviour
	{
		// Token: 0x06009A7E RID: 39550 RVA: 0x004858A8 File Offset: 0x00483AA8
		private void Awake()
		{
			this.btnOpenCharPage.ClearAndAddListener(new Action(this.OnClickCharAvatar));
			this.btnMain.ClearAndAddListener(new Action(this.OnClick));
		}

		// Token: 0x06009A7F RID: 39551 RVA: 0x004858DB File Offset: 0x00483ADB
		private void OnClick()
		{
			Action actionOnClick = this._actionOnClick;
			if (actionOnClick != null)
			{
				actionOnClick();
			}
		}

		// Token: 0x06009A80 RID: 39552 RVA: 0x004858F0 File Offset: 0x00483AF0
		private void OnClickCharAvatar()
		{
			this.OpenCharacterMenu(this._data.CharacterId);
		}

		// Token: 0x06009A81 RID: 39553 RVA: 0x00485905 File Offset: 0x00483B05
		public void SetEmpty()
		{
			this.emptyObj.SetActive(true);
			this.mainObj.SetActive(false);
		}

		// Token: 0x06009A82 RID: 39554 RVA: 0x00485924 File Offset: 0x00483B24
		public void Set(CharacterDisplayDataForGeneralScrollList data, bool isSelected, Action actionOnClick)
		{
			this.emptyObj.SetActive(false);
			this.mainObj.SetActive(true);
			this._data = data;
			this._actionOnClick = actionOnClick;
			this.avatar.Refresh(data.AvatarRelatedData, data.CharacterTemplateId);
			this.txtName.text = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, false, false);
			this.txtAge.text = data.PhysiologicalAge.ToString();
			CharacterItem charConfig = Character.Instance.GetItem(data.CharacterTemplateId);
			CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(data.Gender, -1);
			this.genderComp.Set(CommonUtils.GetGenderIconBig(displayGender), LanguageKey.LK_Main_SummaryInfo_Gender.Tr(), CommonUtils.GetGenderString(displayGender));
			bool isFixedPresetType = CreatingType.IsFixedPresetType(charConfig.CreatingType);
			string charmIcon = CommonUtils.GetCharmLevelBigIcon(data.Charm, data.ActualAge, data.AvatarRelatedData.ClothingDisplayId, data.AvatarRelatedData.AvatarData.FaceVisible, isFixedPresetType);
			this.charmComp.Set(charmIcon, LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.ActualAge, data.AvatarRelatedData.ClothingDisplayId, isFixedPresetType, true));
			this.behaviorTypeComp.Set(CommonUtils.GetBehaviorTypeIcon(data.BehaviorType), LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), CommonUtils.GetBehaviorString(data.BehaviorType));
			this.fameComp.Set(CommonUtils.GetFameIconName(data.Fame), LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), CommonUtils.GetFameString(FameType.GetFameType(data.Fame)));
			EHealthType healthType = CommonUtils.GetHealthType(data.Health, data.MaxLeftHealth, data.CharacterId);
			this.healthComp.Set(CommonUtils.GetHealthIcon(healthType), LanguageKey.LK_Health.Tr(), CommonUtils.GetHealthString(healthType));
			this.favorComp.Set(CommonUtils.GetFavorabilityIconName(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu), LanguageKey.LK_Favorability.Tr(), CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu));
			int curLevel = Mathf.Clamp(Mathf.Max((int)(data.ConsummateLevel - 1), 0) / 2, 0, 8);
			this.consummateComp.Set(CommonUtils.GetConsummateIcon((sbyte)curLevel), LanguageKey.LK_Mousetip_Sort_Desc_ConsummateLevel.Tr(), LocalStringManager.Get(string.Format("LK_Consummate_Level_{0}", curLevel)));
			this.SetSelected(isSelected);
			TooltipInvoker tooltipInvoker = this.mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.mouseTip.RuntimeParam.Set("charId", data.CharacterId);
		}

		// Token: 0x06009A83 RID: 39555 RVA: 0x00485BBB File Offset: 0x00483DBB
		public void SetSelected(bool isSelected)
		{
			this.selectedObj.SetActive(isSelected);
		}

		// Token: 0x06009A84 RID: 39556 RVA: 0x00485BCC File Offset: 0x00483DCC
		private void OpenCharacterMenu(int charId)
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", charId).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x04007767 RID: 30567
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04007768 RID: 30568
		[SerializeField]
		private TextMeshProUGUI txtName;

		// Token: 0x04007769 RID: 30569
		[SerializeField]
		private TextMeshProUGUI txtAge;

		// Token: 0x0400776A RID: 30570
		[SerializeField]
		private ComponentIconTitleValue genderComp;

		// Token: 0x0400776B RID: 30571
		[SerializeField]
		private ComponentIconTitleValue charmComp;

		// Token: 0x0400776C RID: 30572
		[SerializeField]
		private ComponentIconTitleValue behaviorTypeComp;

		// Token: 0x0400776D RID: 30573
		[SerializeField]
		private ComponentIconTitleValue fameComp;

		// Token: 0x0400776E RID: 30574
		[SerializeField]
		private ComponentIconTitleValue healthComp;

		// Token: 0x0400776F RID: 30575
		[SerializeField]
		private ComponentIconTitleValue favorComp;

		// Token: 0x04007770 RID: 30576
		[SerializeField]
		private ComponentIconTitleValue consummateComp;

		// Token: 0x04007771 RID: 30577
		[SerializeField]
		private CButton btnMain;

		// Token: 0x04007772 RID: 30578
		[SerializeField]
		private GameObject selectedObj;

		// Token: 0x04007773 RID: 30579
		[SerializeField]
		private CButton btnOpenCharPage;

		// Token: 0x04007774 RID: 30580
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04007775 RID: 30581
		[SerializeField]
		private GameObject emptyObj;

		// Token: 0x04007776 RID: 30582
		[SerializeField]
		private GameObject mainObj;

		// Token: 0x04007777 RID: 30583
		private CharacterDisplayDataForGeneralScrollList _data;

		// Token: 0x04007778 RID: 30584
		private Action _actionOnClick;
	}
}
