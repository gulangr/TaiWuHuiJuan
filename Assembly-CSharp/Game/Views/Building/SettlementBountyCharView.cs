using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using Game.Views.SettlementPrison;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BE5 RID: 3045
	public class SettlementBountyCharView : MonoBehaviour
	{
		// Token: 0x06009A79 RID: 39545 RVA: 0x004855C3 File Offset: 0x004837C3
		private void Awake()
		{
			this.btnOpenCharPage.ClearAndAddListener(new Action(this.OnClickCharAvatar));
		}

		// Token: 0x06009A7A RID: 39546 RVA: 0x004855DE File Offset: 0x004837DE
		private void OnClickCharAvatar()
		{
			this.OpenCharacterMenu(this._data.CharacterDisplayDataForGeneralScrollList.CharacterId);
		}

		// Token: 0x06009A7B RID: 39547 RVA: 0x004855F8 File Offset: 0x004837F8
		public void Set(CharacterDisplayDataForSettlementBounty data, int settlementOrgTemplateId)
		{
			this._data = data;
			this.avatar.Refresh(data.AvatarRelatedData, data.CharacterDisplayDataForGeneralScrollList.CharacterTemplateId);
			this.txtName.text = NameCenter.GetMonasticTitleOrDisplayName(ref data.CharacterDisplayDataForGeneralScrollList.NameData, false, false);
			this.txtAge.text = data.PhysiologicalAge.ToString();
			int remainTime = data.SettlementBounty.ExpireDate - SingletonObject.getInstance<BasicGameData>().CurrDate;
			this.txtRemainMonth.text = remainTime.ToString();
			CharacterItem charConfig = Character.Instance.GetItem(data.CharacterDisplayDataForGeneralScrollList.CharacterTemplateId);
			CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(data.Gender, -1);
			sbyte severity = data.SettlementBounty.PunishmentSeverity;
			this.crimeSeverityName.Set(string.Format("{0}{1}", "ui9_back_bounty_level_big_", severity), LanguageKey.LK_Law_CrimeLevel.Tr(), CommonUtils.GetCrimeSeverityName(data));
			this.crimeName.Set("ui9_back_bounty_name_big_0", LanguageKey.LK_Sect_Wanted_Reason.Tr(), CommonUtils.GetCrimeNameString(data));
			this.bountyAmount.Set(CommonUtils.GetSettlementBountyIcon(settlementOrgTemplateId), LanguageKey.LK_BountyAmount.Tr(), data.SettlementBounty.BountyAmount.ToString());
			string orgName = ViewSettlementPrison.GetOrgName((short)data.OrgInfo.OrgTemplateId, data.RandomNameId);
			string identityName = CommonUtils.GetIdentityString(data.OrgInfo, data.Gender, data.PhysiologicalAge, false);
			this.roleName.Set(string.Format("{0}{1}", "ui9_back_bounty_identity_big_", data.OrgInfo.Grade), LanguageKey.LK_Main_SummaryInfo_Identity.Tr(), orgName + " - " + identityName);
			string bountyStateStr = CommonUtils.GetHunterStateText(data.HunterState);
			this.bountyState.Set("ui9_back_bounty_state_big_0", LanguageKey.LK_HunterState_Short.Tr(), bountyStateStr);
			this.location.SetValue(SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(data.FullBlockName, true, true, false, false));
			this.mouseTips.Type = TipType.Character;
			TooltipInvoker tooltipInvoker = this.mouseTips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.mouseTips.RuntimeParam.Set("charId", data.CharacterDisplayDataForGeneralScrollList.CharacterId);
		}

		// Token: 0x06009A7C RID: 39548 RVA: 0x00485844 File Offset: 0x00483A44
		private void OpenCharacterMenu(int charId)
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", charId).Set("PreviousView", 6).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x0400775A RID: 30554
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x0400775B RID: 30555
		[SerializeField]
		private TextMeshProUGUI txtName;

		// Token: 0x0400775C RID: 30556
		[SerializeField]
		private TextMeshProUGUI txtAge;

		// Token: 0x0400775D RID: 30557
		[SerializeField]
		private TextMeshProUGUI txtRemainMonth;

		// Token: 0x0400775E RID: 30558
		[SerializeField]
		private ComponentIconTitleValue crimeSeverityName;

		// Token: 0x0400775F RID: 30559
		[SerializeField]
		private ComponentIconTitleValue crimeName;

		// Token: 0x04007760 RID: 30560
		[SerializeField]
		private ComponentIconTitleValue bountyAmount;

		// Token: 0x04007761 RID: 30561
		[SerializeField]
		private ComponentIconTitleValue roleName;

		// Token: 0x04007762 RID: 30562
		[SerializeField]
		private ComponentIconTitleValue bountyState;

		// Token: 0x04007763 RID: 30563
		[SerializeField]
		private ComponentIconTitleValue location;

		// Token: 0x04007764 RID: 30564
		[SerializeField]
		private CButton btnOpenCharPage;

		// Token: 0x04007765 RID: 30565
		[SerializeField]
		private TooltipInvoker mouseTips;

		// Token: 0x04007766 RID: 30566
		private CharacterDisplayDataForSettlementBounty _data;
	}
}
