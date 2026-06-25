using System;
using Config;
using FrameWork;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;

// Token: 0x020003C8 RID: 968
public class MapBlockCharCaravan : MapBlockCharAlive
{
	// Token: 0x06003A87 RID: 14983 RVA: 0x001DBC7C File Offset: 0x001D9E7C
	public void Init(bool canInteract, MapBlockData mapBlock, CaravanDisplayData caravanData)
	{
		base.Init(canInteract, mapBlock, null);
		this._caravanData = caravanData;
		this._merchantConfig = Merchant.Instance[(int)caravanData.MerchantTemplateId];
		this._merchantTypeConfig = Config.MerchantType.Instance[this._merchantConfig.MerchantType];
		this.Refresh();
	}

	// Token: 0x06003A88 RID: 14984 RVA: 0x001DBCD3 File Offset: 0x001D9ED3
	protected override void Refresh()
	{
		base.Refresh();
		this.RefreshCaravanPath();
		this.RefreshCaravanState();
		this.RefreshMerchantInfo();
	}

	// Token: 0x06003A89 RID: 14985 RVA: 0x001DBCF4 File Offset: 0x001D9EF4
	protected override void RefreshName()
	{
		string nameContent = LocalStringManager.Get(LanguageKey.LK_Caravan);
		this.nameText.text = nameContent;
	}

	// Token: 0x06003A8A RID: 14986 RVA: 0x001DBD1A File Offset: 0x001D9F1A
	protected override void RefreshOrganization()
	{
		this.organizationText.text = this._merchantTypeConfig.Name;
		this.organizationIcon.gameObject.SetActive(false);
	}

	// Token: 0x06003A8B RID: 14987 RVA: 0x001DBD46 File Offset: 0x001D9F46
	protected override void RefreshAvatar()
	{
		base.RefreshAvatar();
		ResLoader.LoadModOrGameResource<Texture2D>(MapBlockCharBase.NpcAvatarTexturePath + "/" + this._merchantTypeConfig.CaravanAvatar, new Action<Texture2D>(this.avatar.Refresh), null);
	}

	// Token: 0x06003A8C RID: 14988 RVA: 0x001DBD82 File Offset: 0x001D9F82
	protected override void RefreshIcon()
	{
		this.iconImage.SetSprite("sp_icon_shanghui", false, null);
	}

	// Token: 0x06003A8D RID: 14989 RVA: 0x001DBD98 File Offset: 0x001D9F98
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

	// Token: 0x06003A8E RID: 14990 RVA: 0x001DBE80 File Offset: 0x001DA080
	private void RefreshCaravanPath()
	{
		TextMeshProUGUI merchantFavor = this.merchantRefer.CGet<TextMeshProUGUI>("Favor");
		TextMeshProUGUI businessMoveTime = this.merchantRefer.CGet<TextMeshProUGUI>("MoveTime");
		PointerTrigger pointerTrigger = this.button.GetComponent<PointerTrigger>();
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
		businessMoveTime.text = this._caravanData.PathInArea.MoveWaitDays.ToString();
		int limitedLevel;
		int limitedFavor;
		UI_Shop.IsReachProgressLimit(this._caravanData.Favorability, out limitedLevel, out limitedFavor);
		int curFavor = Mathf.Min(this._caravanData.Favorability, limitedFavor);
		string favorStr = string.Format("{0}/{1}", curFavor, limitedFavor);
		merchantFavor.text = LocalStringManager.GetFormat(LanguageKey.LK_BlockChar_Merchant_Favor, favorStr);
	}

	// Token: 0x06003A8F RID: 14991 RVA: 0x001DBF74 File Offset: 0x001DA174
	private void RefreshCaravanState()
	{
		CaravanExtraData extraData = this._caravanData.ExtraData;
		bool isRobbed = ((extraData != null) ? new CaravanState?(extraData.StateEnum) : null) == CaravanState.Robbed;
		this.merchantRefer.CGet<GameObject>("RobbedEffect").SetActive(isRobbed);
	}

	// Token: 0x06003A90 RID: 14992 RVA: 0x001DBFD8 File Offset: 0x001DA1D8
	private void RefreshMerchantInfo()
	{
		this.merchantNameBg.gameObject.SetActive(true);
		this.merchantNameText.text = this._merchantTypeConfig.Name;
		this.merchantLevelImage.SetSprite(base.GetMerchantLevelImage(this._merchantConfig.Level), false, null);
	}

	// Token: 0x04002A2C RID: 10796
	[SerializeField]
	private Refers merchantRefer;

	// Token: 0x04002A2D RID: 10797
	[SerializeField]
	private GameObject merchantNameBg;

	// Token: 0x04002A2E RID: 10798
	[SerializeField]
	private TextMeshProUGUI merchantNameText;

	// Token: 0x04002A2F RID: 10799
	[SerializeField]
	private CImage merchantLevelImage;

	// Token: 0x04002A30 RID: 10800
	private CaravanDisplayData _caravanData;

	// Token: 0x04002A31 RID: 10801
	private MerchantItem _merchantConfig;

	// Token: 0x04002A32 RID: 10802
	private MerchantTypeItem _merchantTypeConfig;
}
