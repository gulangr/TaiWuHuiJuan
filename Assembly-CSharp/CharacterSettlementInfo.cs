using System;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x0200031F RID: 799
public class CharacterSettlementInfo : MonoBehaviour
{
	// Token: 0x06002EBB RID: 11963 RVA: 0x00170C4D File Offset: 0x0016EE4D
	public void Refresh(int charId)
	{
		this._charId = charId;
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, charId, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._characterDisplayData);
			this.RefreshSettlementTreasuryGuard();
			this.RefreshSettlementBounty();
			this.RefreshJieqingSign();
		});
	}

	// Token: 0x06002EBC RID: 11964 RVA: 0x00170C6B File Offset: 0x0016EE6B
	private void OnEnable()
	{
		GEvent.Add(UiEvents.OnJieqingSignStateRefresh, new GEvent.Callback(this.OnJieqingSignStateRefresh));
	}

	// Token: 0x06002EBD RID: 11965 RVA: 0x00170C8A File Offset: 0x0016EE8A
	private void OnJieqingSignStateRefresh(ArgumentBox argBox)
	{
		this.RefreshJieqingSign();
	}

	// Token: 0x06002EBE RID: 11966 RVA: 0x00170C94 File Offset: 0x0016EE94
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnJieqingSignStateRefresh, new GEvent.Callback(this.OnJieqingSignStateRefresh));
	}

	// Token: 0x06002EBF RID: 11967 RVA: 0x00170CB3 File Offset: 0x0016EEB3
	public void Refresh(CharacterDisplayData data)
	{
		this._charId = data.CharacterId;
		this._characterDisplayData = data;
		this.RefreshSettlementTreasuryGuard();
		this.RefreshSettlementBounty();
		this.RefreshJieqingSign();
	}

	// Token: 0x06002EC0 RID: 11968 RVA: 0x00170CE0 File Offset: 0x0016EEE0
	public void RefreshSettlementTreasuryGuard()
	{
		bool show = this._characterDisplayData.IsSettlementTreasuryGuard && this._characterDisplayData.AliveState == 0;
		this.settlementTreasuryGuardObj.SetActive(show);
		bool flag = !show;
		if (!flag)
		{
			TooltipInvoker tip = this.settlementTreasuryGuardObj.GetComponent<TooltipInvoker>();
			tip.Type = TipType.Simple;
			bool flag2 = tip.PresetParam == null || tip.PresetParam.Length < 2;
			if (flag2)
			{
				tip.PresetParam = new string[2];
			}
			int spLevel = Math.Clamp((int)(this._characterDisplayData.SettlementTreasuryGuardLevel - 1), 0, 2);
			tip.PresetParam[0] = CharacterSettlementInfo.GuardTitles[spLevel].Tr();
			string orgName = Organization.Instance[this._characterDisplayData.OrgInfo.OrgTemplateId].Name;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(CharacterSettlementInfo.GuardContents[spLevel].TrFormat(orgName));
			bool flag3 = !this._characterDisplayData.SettlementTreasuryGuardWorking;
			if (flag3)
			{
				sb.AppendLine(LanguageKey.LK_MapBlockCharList_SettlementTreasury_Guard_Not_Valid.Tr());
			}
			tip.PresetParam[1] = sb.ToString();
			this.settlementTreasuryGuardIcon.sprite = this.bountyIcons[spLevel];
		}
	}

	// Token: 0x06002EC1 RID: 11969 RVA: 0x00170E14 File Offset: 0x0016F014
	public void RefreshSettlementBounty()
	{
		bool show = this._characterDisplayData.BountyPunishmentSeverity >= 0 && this._characterDisplayData.BountyOrgTemplate >= 0 && this._characterDisplayData.AliveState == 0;
		this.settlementBountyIcon.gameObject.SetActive(show);
		this.settlementBountyIcon.raycastTarget = show;
		bool flag = !show;
		if (!flag)
		{
			this.settlementBountyIcon.SetSprite(string.Format("ui_prison_icon_seize_{0}", this._characterDisplayData.BountyPunishmentSeverity), false, null);
			TooltipInvoker tip = this.settlementBountyIcon.GetComponent<TooltipInvoker>();
			tip.Type = TipType.Simple;
			bool flag2 = tip.PresetParam == null || tip.PresetParam.Length < 2;
			if (flag2)
			{
				tip.PresetParam = new string[2];
			}
			tip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Mousetipcharacter_Bounty);
			OrganizationItem orgConfig = Organization.Instance[this._characterDisplayData.BountyOrgTemplate];
			tip.PresetParam[1] = LocalStringManager.GetFormat(LanguageKey.LK_Mousetipcharacter_Bounty_Tip, orgConfig.Name);
		}
	}

	// Token: 0x06002EC2 RID: 11970 RVA: 0x00170F20 File Offset: 0x0016F120
	public void RefreshJieqingSign()
	{
		this.jieqingSign.gameObject.SetActive(false);
		bool flag = this._characterDisplayData == null;
		if (!flag)
		{
			CommonUtils.QueryJieqingSpecialInteractionUnlocked(delegate(bool unlocked)
			{
				bool flag2 = !unlocked || !base.isActiveAndEnabled;
				if (!flag2)
				{
					int sectSign = SingletonObject.getInstance<GlobalSettings>().JieQingMurderSignDisplay;
					bool flag3 = !CommonUtils.CheckSectFlag(sectSign, (int)this._characterDisplayData.OrgInfo.OrgTemplateId) && !this.ShowJieqingSignFixed;
					if (!flag3)
					{
						int charId = this._charId;
						ExtraDomainMethod.AsyncCall.GetCharacterExtraLegacyPointWorth(null, charId, delegate(int offset, RawDataPool dataPool)
						{
							bool flag4 = !this.isActiveAndEnabled || charId != this._charId;
							if (!flag4)
							{
								int worth = 0;
								Serializer.Deserialize(dataPool, offset, ref worth);
								int levelIndex = CommonUtils.GetJieqingSignLevelIndex(worth);
								bool show = levelIndex >= 0;
								this.jieqingSign.gameObject.SetActive(show);
								bool flag5 = !show;
								if (!flag5)
								{
									this.jieqingSignTips.Type = TipType.JieqingInteractCharTips;
									TooltipInvoker tooltipInvoker = this.jieqingSignTips;
									if (tooltipInvoker.RuntimeParam == null)
									{
										tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
									}
									this.jieqingSignTips.RuntimeParam.Set("charId", charId);
									this.jieqingSign.SetSprite(string.Format("{0}{1}", "sp_icon_jieqingmark_", levelIndex), false, null);
								}
							}
						});
					}
				}
			}, null);
		}
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x00170F79 File Offset: 0x0016F179
	// Note: this type is marked as 'beforefieldinit'.
	static CharacterSettlementInfo()
	{
		LanguageKey[] array = new LanguageKey[3];
		RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.033A8DFB0659136B8C54E0185707A5CE9804E22B859C837D834E8314C5CDAE97).FieldHandle);
		CharacterSettlementInfo.GuardTitles = array;
		LanguageKey[] array2 = new LanguageKey[3];
		RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.C1195564870137F520DDC9C9CAAAA35F652E888063551A4533FCE3EC5831E986).FieldHandle);
		CharacterSettlementInfo.GuardContents = array2;
	}

	// Token: 0x040021EA RID: 8682
	[SerializeField]
	private GameObject settlementTreasuryGuardObj;

	// Token: 0x040021EB RID: 8683
	[SerializeField]
	private CImage settlementTreasuryGuardIcon;

	// Token: 0x040021EC RID: 8684
	[SerializeField]
	private CImage settlementBountyIcon;

	// Token: 0x040021ED RID: 8685
	[SerializeField]
	private Sprite[] bountyIcons;

	// Token: 0x040021EE RID: 8686
	private static readonly LanguageKey[] GuardTitles;

	// Token: 0x040021EF RID: 8687
	private static readonly LanguageKey[] GuardContents;

	// Token: 0x040021F0 RID: 8688
	[SerializeField]
	private CImage jieqingSign;

	// Token: 0x040021F1 RID: 8689
	[SerializeField]
	private TooltipInvoker jieqingSignTips;

	// Token: 0x040021F2 RID: 8690
	private CharacterDisplayData _characterDisplayData;

	// Token: 0x040021F3 RID: 8691
	private const string _jieqingSignPrefix = "sp_icon_jieqingmark_";

	// Token: 0x040021F4 RID: 8692
	public bool ShowJieqingSignFixed = false;

	// Token: 0x040021F5 RID: 8693
	private int _charId = -1;
}
