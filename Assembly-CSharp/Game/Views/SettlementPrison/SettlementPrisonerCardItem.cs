using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Common;
using Game.Components.Item;
using Game.Views.CharacterMenu;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.Enum;
using TMPro;
using UnityEngine;

namespace Game.Views.SettlementPrison
{
	// Token: 0x02000788 RID: 1928
	public class SettlementPrisonerCardItem : MonoBehaviour
	{
		// Token: 0x06005D52 RID: 23890 RVA: 0x002AEA48 File Offset: 0x002ACC48
		public void Set(CharacterDisplayDataForSettlementPrisoner data, bool isStoneRoomFull, bool isBreaking, Action forceQuickHide)
		{
			this._data = data;
			this._isBreaking = isBreaking;
			this._forceQuickHide = forceQuickHide;
			this.avatar.Refresh(data.KidnapCharDisplayData.AvatarRelatedData, data.KidnapCharDisplayData.CharacterTemplateId);
			this.buttonAvatar.ClearAndAddListener(delegate
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", data.SettlementPrisoner.CharId);
				argBox.Set("PreviousView", 11);
				argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.Character));
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			});
			ITradeableContent tradeableContent = data.KidnapCharDisplayData;
			NameRelatedData nameRelatedData = tradeableContent.NameRelatedData;
			string nameText = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, false, false);
			this.textName.text = nameText;
			string age = LocalStringManager.GetFormat(LanguageKey.LK_Age, data.KidnapCharDisplayData.PhysiologicalAge);
			this.propertyAge.Set("ui9_icon_age_big", LanguageKey.LK_Char_Age.Tr(), age, null, false);
			string title = LanguageKey.LK_Main_SummaryInfo_Gender.Tr();
			CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(data.KidnapCharDisplayData.Gender, data.KidnapCharDisplayData.CharacterTemplateId);
			string icon = CommonUtils.GetGenderIconBig(displayGender);
			string value = CommonUtils.GetGenderString(displayGender);
			this.propertyGender.Set(icon, title, value, null, false);
			string title2 = LanguageKey.LK_Main_SummaryInfo_Organization.Tr();
			string icon2 = CommonUtils.GetOrganizationIcon((short)data.KidnapCharDisplayData.OrganizationInfo.OrgTemplateId);
			string value2 = ViewSettlementPrison.GetOrgName((short)data.KidnapCharDisplayData.OrganizationInfo.OrgTemplateId, data.RandomNameId);
			this.propertyOrg.Set(icon2, title2, value2, null, false);
			string title3 = LanguageKey.LK_Main_SummaryInfo_Identity.Tr();
			string icon3 = CommonUtils.GetIdentityIconName(data.KidnapCharDisplayData.OrganizationInfo.Grade);
			string value3 = CommonUtils.GetIdentityString(data.KidnapCharDisplayData.OrganizationInfo, data.KidnapCharDisplayData.Gender, data.KidnapCharDisplayData.PhysiologicalAge, false);
			this.propertyIdentity.Set(icon3, title3, value3, null, false);
			ItemKey ropeItemKey = data.SettlementPrisoner.RopeItemKey;
			ItemDisplayData ropeItemData = new ItemDisplayData(ropeItemKey.ItemType, ropeItemKey.TemplateId);
			this.ropeItem.Set(ropeItemData, false);
			this.textResistance.text = data.Resistance.ToString().SetColor((data.Resistance > 100) ? "brightred" : "brightblue");
			PunishmentTypeItem punishmentTypeItem = PunishmentType.Instance[data.SettlementPrisoner.PunishmentType];
			PunishmentSeverityItem punishmentSeverityItem = PunishmentSeverity.Instance[data.SettlementPrisoner.PunishmentSeverity];
			this.textPunishmentType.text = punishmentTypeItem.ShortName.SetColor(punishmentSeverityItem.NameColor);
			this.textPunishmentDuration.text = ViewSettlementPrison.GetPrisonDurationDisplayStr(data);
			int ropeEffect = data.SettlementPrisoner.GetRopeReduceEscapeRate();
			this.tipResistance.Type = TipType.PrisonerResistance;
			TooltipInvoker tooltipInvoker = this.tipResistance;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.tipResistance.RuntimeParam.Set("IsPrivate", false).Set("Resistance", data.Resistance).Set("EscapeRate", data.EscapeRate).Set("RopeEffect", ropeEffect).Set("CompletelyInfected", data.CompletelyInfected).Set("OwningBook", data.OwningBook);
			this.buttonPlead.gameObject.SetActive(!isBreaking && !data.CompletelyInfected);
			this.buttonRescue.gameObject.SetActive(!data.CompletelyInfected);
			this.buttonKidnap.interactable = (!data.CompletelyInfected || (data.CompletelyInfected && !isStoneRoomFull));
			this.tipButtonKidnap.enabled = (data.CompletelyInfected && isStoneRoomFull);
			string buttonKidnapTipContent = LocalStringManager.Get(LanguageKey.LK_SettlementPrison_Tip_StoneRoomFull).SetColor("brightred");
			this.tipButtonKidnap.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("arg0", buttonKidnapTipContent);
			this.buttonPlead.ClearAndAddListener(delegate
			{
				this.InteractPrisoner(InteractPrisonerType.Plead);
			});
			this.buttonKidnap.ClearAndAddListener(delegate
			{
				this.InteractPrisoner(InteractPrisonerType.Kidnap);
			});
			this.buttonRescue.ClearAndAddListener(delegate
			{
				this.InteractPrisoner(InteractPrisonerType.Rescue);
			});
		}

		// Token: 0x06005D53 RID: 23891 RVA: 0x002AEF30 File Offset: 0x002AD130
		private void InteractPrisoner(InteractPrisonerType type)
		{
			bool flag = !this._isBreaking;
			if (flag)
			{
				TaiwuEventDomainMethod.Call.InteractPrisoner(this._data.SettlementPrisoner.CharId, type.ToInt());
			}
			else
			{
				TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("SettlementPrisonComplete", "SelectCharacterId", this._data.SettlementPrisoner.CharId);
				TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("SettlementPrisonComplete", "InteractPrisonerType", type.ToInt());
				this._forceQuickHide();
			}
		}

		// Token: 0x04004023 RID: 16419
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04004024 RID: 16420
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04004025 RID: 16421
		[SerializeField]
		private PropertyItem propertyAge;

		// Token: 0x04004026 RID: 16422
		[SerializeField]
		private PropertyItem propertyGender;

		// Token: 0x04004027 RID: 16423
		[SerializeField]
		private PropertyItem propertyOrg;

		// Token: 0x04004028 RID: 16424
		[SerializeField]
		private PropertyItem propertyIdentity;

		// Token: 0x04004029 RID: 16425
		[SerializeField]
		private ItemBack ropeItem;

		// Token: 0x0400402A RID: 16426
		[SerializeField]
		private TextMeshProUGUI textResistance;

		// Token: 0x0400402B RID: 16427
		[SerializeField]
		private TooltipInvoker tipResistance;

		// Token: 0x0400402C RID: 16428
		[SerializeField]
		private TextMeshProUGUI textPunishmentType;

		// Token: 0x0400402D RID: 16429
		[SerializeField]
		private TextMeshProUGUI textPunishmentDuration;

		// Token: 0x0400402E RID: 16430
		[SerializeField]
		private CButton buttonAvatar;

		// Token: 0x0400402F RID: 16431
		[SerializeField]
		private CButton buttonPlead;

		// Token: 0x04004030 RID: 16432
		[SerializeField]
		private CButton buttonKidnap;

		// Token: 0x04004031 RID: 16433
		[SerializeField]
		private TooltipInvoker tipButtonKidnap;

		// Token: 0x04004032 RID: 16434
		[SerializeField]
		private CButton buttonRescue;

		// Token: 0x04004033 RID: 16435
		private Action _forceQuickHide;

		// Token: 0x04004034 RID: 16436
		private bool _isBreaking;

		// Token: 0x04004035 RID: 16437
		private CharacterDisplayDataForSettlementPrisoner _data;
	}
}
