using System;
using Config;
using FrameWork;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;

// Token: 0x020003C9 RID: 969
public class MapBlockCharCaravan2 : MapBlockCharAlive2
{
	// Token: 0x06003A93 RID: 14995 RVA: 0x001DC070 File Offset: 0x001DA270
	public void Init(bool canInteract, MapBlockData mapBlock, CaravanDisplayData caravanData)
	{
		base.Init(canInteract, mapBlock, null);
		this._caravanData = caravanData;
		this._merchantConfig = Merchant.Instance[(int)caravanData.MerchantTemplateId];
		this._merchantTypeConfig = Config.MerchantType.Instance[this._merchantConfig.MerchantType];
		this.Refresh();
	}

	// Token: 0x06003A94 RID: 14996 RVA: 0x001DC0C7 File Offset: 0x001DA2C7
	protected override void Refresh()
	{
		base.Refresh();
		this.RefreshCaravanPath();
		this.RefreshCaravanState();
		this.RefreshMerchantInfo();
	}

	// Token: 0x06003A95 RID: 14997 RVA: 0x001DC0E8 File Offset: 0x001DA2E8
	protected override void RefreshName()
	{
		string nameContent = LocalStringManager.Get(LanguageKey.LK_Caravan);
		this.nameLabel.text = nameContent;
	}

	// Token: 0x06003A96 RID: 14998 RVA: 0x001DC10E File Offset: 0x001DA30E
	protected override void RefreshOrganization()
	{
		this.organizationLabel.text = this._merchantTypeConfig.Name;
	}

	// Token: 0x06003A97 RID: 14999 RVA: 0x001DC128 File Offset: 0x001DA328
	protected override void OnClickButton()
	{
		bool isMoving = base.IsMoving;
		if (!isMoving)
		{
			CaravanDisplayData caravanData = this._caravanData;
			short? num;
			if (caravanData == null)
			{
				num = null;
			}
			else
			{
				CaravanPath pathInArea = caravanData.PathInArea;
				num = ((pathInArea != null) ? new short?(pathInArea.GetCurrLocation().BlockId) : null);
			}
			short? num2 = num;
			int? num3 = (num2 != null) ? new int?((int)num2.GetValueOrDefault()) : null;
			int currentBlockId = base.CurrentBlockId;
			bool flag = !(num3.GetValueOrDefault() == currentBlockId & num3 != null);
			if (!flag)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 15, this._caravanData.CaravanId);
				GEvent.OnEvent(UiEvents.WorldMapShowPath, null);
				GEvent.OnEvent(UiEvents.WorldMapRefreshTradeCaravanPath, EasyPool.Get<ArgumentBox>().Set("merchantId", -1));
				base.OnClickButton();
			}
		}
	}

	// Token: 0x06003A98 RID: 15000 RVA: 0x001DC210 File Offset: 0x001DA410
	private void RefreshCaravanPath()
	{
		PointerTrigger pointerTrigger = base.Button.GetComponent<PointerTrigger>();
		pointerTrigger.EnterEvent.AddListener(delegate()
		{
			GEvent.OnEvent(UiEvents.WorldMapHidePath, null);
			GEvent.OnEvent(UiEvents.WorldMapRefreshTradeCaravanPath, EasyPool.Get<ArgumentBox>().Set("merchantId", this._caravanData.CaravanId));
		});
		pointerTrigger.ExitEvent.AddListener(delegate()
		{
			GEvent.OnEvent(UiEvents.WorldMapShowPath, null);
			GEvent.OnEvent(UiEvents.WorldMapRefreshTradeCaravanPath, EasyPool.Get<ArgumentBox>().Set("merchantId", -1));
		});
		this.moveTimeLabel.text = this._caravanData.PathInArea.MoveWaitDays.ToString();
		int limitedLevel;
		int limitedFavor;
		UI_Shop.IsReachProgressLimit(this._caravanData.Favorability, out limitedLevel, out limitedFavor);
		int curFavor = Mathf.Min(this._caravanData.Favorability, limitedFavor);
		string favorStr = string.Format("{0}/{1}", curFavor, limitedFavor);
	}

	// Token: 0x06003A99 RID: 15001 RVA: 0x001DC2D0 File Offset: 0x001DA4D0
	private void RefreshCaravanState()
	{
		CaravanExtraData extraData = this._caravanData.ExtraData;
		bool isRobbed = ((extraData != null) ? new CaravanState?(extraData.StateEnum) : null) == CaravanState.Robbed;
		this.robbedEffect.SetActive(isRobbed);
	}

	// Token: 0x06003A9A RID: 15002 RVA: 0x001DC327 File Offset: 0x001DA527
	private void RefreshMerchantInfo()
	{
		this.merchantIcon.gameObject.SetActive(true);
		this.merchantLevelLabel.text = this._merchantConfig.Level.ToString();
	}

	// Token: 0x06003A9B RID: 15003 RVA: 0x001DC358 File Offset: 0x001DA558
	protected override void RefreshAvatar()
	{
		base.RefreshAvatar();
		ResLoader.LoadModOrGameResource<Texture2D>(MapBlockCharBase2.NpcAvatarTexturePath + "/" + this._merchantTypeConfig.CaravanAvatar, new Action<Texture2D>(this.avatar.Refresh), null);
	}

	// Token: 0x06003A9C RID: 15004 RVA: 0x001DC394 File Offset: 0x001DA594
	protected override void RefreshSpetialInfo(TextMeshProUGUI infoLabel, MapBlockCharCustomInfoItem info, TooltipInvoker tip)
	{
		int limitedLevel;
		int limitedFavor;
		UI_Shop.IsReachProgressLimit(this._caravanData.Favorability, out limitedLevel, out limitedFavor);
		infoLabel.text = Mathf.Min(this._caravanData.Favorability, limitedFavor).ToString();
	}

	// Token: 0x04002A33 RID: 10803
	[SerializeField]
	private CImage merchantIcon;

	// Token: 0x04002A34 RID: 10804
	[SerializeField]
	private TextMeshProUGUI merchantLevelLabel;

	// Token: 0x04002A35 RID: 10805
	[SerializeField]
	private TextMeshProUGUI moveTimeLabel;

	// Token: 0x04002A36 RID: 10806
	[SerializeField]
	private GameObject robbedEffect;

	// Token: 0x04002A37 RID: 10807
	private CaravanDisplayData _caravanData;

	// Token: 0x04002A38 RID: 10808
	private MerchantItem _merchantConfig;

	// Token: 0x04002A39 RID: 10809
	private MerchantTypeItem _merchantTypeConfig;
}
