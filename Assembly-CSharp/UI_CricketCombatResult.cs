using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Views.Combat;
using Game.Views.Migrate;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x020001F9 RID: 505
[Obsolete("请看ViewCricketCombatResult，此界面暂时留作参考几天")]
public class UI_CricketCombatResult : UIBase
{
	// Token: 0x060020C8 RID: 8392 RVA: 0x000EF0BC File Offset: 0x000ED2BC
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get("IsWin", out this._isWin);
		argsBox.Get<Wager>("Wager", out this._wager);
		CharacterDisplayData charData;
		argsBox.Get<CharacterDisplayData>("WagerChar", out charData);
		this._wagerCharList.Clear();
		bool flag = charData != null;
		if (flag)
		{
			this._wagerCharList.Add(charData);
		}
		argsBox.Get<CharacterDisplayData>("TaiwuChar", out this._taiwuChar);
		argsBox.Get<CharacterDisplayData>("EnemyChar", out this._enemyChar);
		bool flag2 = this._wager.Type == 1;
		if (flag2)
		{
			int charId = this._isWin ? this._taiwuChar.CharacterId : this._enemyChar.CharacterId;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				ItemDomainMethod.Call.GetItemDisplayData(this.Element.GameDataListenerId, this._wager.ItemKey, charId);
			}));
		}
		CanvasGroup btnCanvas = this.btnClose.GetComponent<CanvasGroup>();
		btnCanvas.alpha = 0f;
		this.cgMainWindow.alpha = 0f;
	}

	// Token: 0x060020C9 RID: 8393 RVA: 0x000EF1DC File Offset: 0x000ED3DC
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)SingletonObject.getInstance<BasicGameData>().TaiwuCharId, new uint[]
		{
			34U
		}));
	}

	// Token: 0x060020CA RID: 8394 RVA: 0x000EF208 File Offset: 0x000ED408
	public unsafe override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = notification.MethodId == 7;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._itemDisplayData);
						this.Refresh();
						this.Element.ShowAfterRefresh();
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag2 = uid.DomainId == 4 && uid.DataId == 0 && uid.SubId1 == 34U;
				if (flag2)
				{
					ResourceInts resources = default(ResourceInts);
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref resources);
					bool flag3 = this._wager.Type == 0;
					if (flag3)
					{
						int resourceCount = *(ref resources.Items.FixedElementField + (IntPtr)this._wager.WagerResourceType * 4);
						bool isWin = this._isWin;
						if (isWin)
						{
							resourceCount -= this._wager.Count;
						}
						else
						{
							resourceCount += this._wager.Count;
						}
						this.txtMeshPrevResource.text = (CommonUtils.GetDisplayStringForNum(resourceCount, 100000) ?? "");
					}
					bool flag4 = this._wager.Type != 1;
					if (flag4)
					{
						this.Refresh();
						this.Element.ShowAfterRefresh();
					}
				}
			}
		}
	}

	// Token: 0x060020CB RID: 8395 RVA: 0x000EF3C8 File Offset: 0x000ED5C8
	private void Refresh()
	{
		this.txtMeshLootTitle.transform.parent.gameObject.SetActive(this._wager.Type != -1);
		this.txtMeshLootTitle.text = LocalStringManager.Get(this._isWin ? LanguageKey.LK_OtherCombat_Win : LanguageKey.LK_OtherCombat_Lose);
		GameObject gameObject = this.rectTsResourceHolder.gameObject;
		sbyte type = this._wager.Type;
		gameObject.SetActive(type == 0 || type == 3);
		CricketCombatResultResourceViewInfo resourceViewInfo = this.goResourceView.GetComponent<CricketCombatResultResourceViewInfo>();
		CImage resourceImage = resourceViewInfo.imgResourceImage;
		bool flag = this._wager.Type == 0;
		if (flag)
		{
			resourceViewInfo.imgResourceIcon.SetSprite(Config.ResourceType.Instance[this._wager.WagerResourceType].Icon, false, null);
			resourceViewInfo.txtMeshResourceName.SetText(Config.ResourceType.Instance[this._wager.WagerResourceType].Name, true);
			resourceViewInfo.txtMeshResourceCount.text = (this._isWin ? ("+" + CommonUtils.GetDisplayStringForNum(this._wager.Count, 100000)).SetColor("brightblue") : ("-" + CommonUtils.GetDisplayStringForNum(this._wager.Count, 100000)).SetColor("brightred"));
			string iconName = CombatDrops.GetIconName(this._wager.Count, this._wager.WagerResourceType, false);
			resourceImage.SetSprite(iconName, false, null);
			resourceImage.SetNativeSize();
		}
		bool flag2 = this._wager.Type == 3;
		if (flag2)
		{
			resourceViewInfo.imgResourceIcon.SetSprite("sp_icon_lilian", false, null);
			resourceViewInfo.txtMeshResourceName.SetText(LocalStringManager.Get(LanguageKey.LK_Exp), true);
			resourceViewInfo.txtMeshResourceCount.text = (this._isWin ? ("+" + CommonUtils.GetDisplayStringForNum(this._wager.Count, 100000)).SetColor("brightblue") : ("-" + CommonUtils.GetDisplayStringForNum(this._wager.Count, 100000)).SetColor("brightred"));
			resourceImage.SetSprite("sp_tuan_lilian", false, null);
			resourceImage.SetNativeSize();
		}
		TextMeshProUGUI evaluationLabel = this.txtMeshEvaluation;
		bool taiwuHappinessIsUp = (this._isWin ? GlobalConfig.Instance.OtherCombatWinHappiness[(int)this._taiwuChar.BehaviorType] : GlobalConfig.Instance.OtherCombatLoseHappiness[(int)this._taiwuChar.BehaviorType]) > 0;
		bool favorabilityToTaiwuIsUp = (this._isWin ? GlobalConfig.Instance.OtherCombatLoseFavorability[(int)this._enemyChar.BehaviorType] : GlobalConfig.Instance.OtherCombatWinFavorability[(int)this._enemyChar.BehaviorType]) > 0;
		string enemyName = NameCenter.GetCharMonasticTitleOrNameByDisplayData(this._enemyChar, false, false);
		string taiwuName = NameCenter.GetCharMonasticTitleOrNameByDisplayData(this._taiwuChar, true, false);
		string evaluationLabelText = LocalStringManager.GetFormat(taiwuHappinessIsUp ? LanguageKey.LK_OtherCombat_Happiness_Up : LanguageKey.LK_OtherCombat_Happiness_Down, taiwuName) + LocalStringManager.GetFormat(favorabilityToTaiwuIsUp ? LanguageKey.LK_OtherCombat_Favorability_Up : LanguageKey.LK_OtherCombat_Favorability_Down, enemyName, taiwuName);
		evaluationLabel.text = evaluationLabelText;
		RectTransform itemHolder = this.rectTsItemHolder;
		itemHolder.gameObject.SetActive(this._wager.Type == 1);
		bool flag3 = this._wager.Type == 1;
		if (flag3)
		{
			GameObject itemPrefab = itemHolder.GetChild(0).gameObject;
			CardItem cardItem = itemPrefab.GetComponent<CardItem>();
			RowItemMain row = cardItem.GetComponent<RowItemMain>();
			row.SetData(this._itemDisplayData);
			cardItem.Set(row, false);
			cardItem.SetInteractable(true, true);
			cardItem.gameObject.SetActive(true);
		}
		RectTransform charHolder = this.rectTsCharHolder;
		charHolder.gameObject.SetActive(this._wager.Type == 2);
		this.goCharBack.SetActive(this._wager.Type == 2);
		bool flag4 = this._wager.Type == 2;
		if (flag4)
		{
			GameObject charPrefab = charHolder.GetChild(0).gameObject;
			List<CharacterDisplayData> wagerCharList = this._wagerCharList;
			int charCount = (wagerCharList != null) ? wagerCharList.Count : 0;
			for (int i = 0; i < charCount; i++)
			{
				bool flag5 = i >= charHolder.childCount;
				if (flag5)
				{
					Object.Instantiate<GameObject>(charPrefab, charHolder);
				}
				CharacterDisplayData charDisplayData = this._wagerCharList[i];
				CricketCombatResultCharPrefabInfo charInfo = charHolder.GetChild(i).GetComponent<CricketCombatResultCharPrefabInfo>();
				charInfo.avatar.Refresh(charDisplayData, true);
				charInfo.txtMeshName.text = NameCenter.GetCharMonasticTitleOrNameByDisplayData(charDisplayData, false, false);
				charInfo.gameObject.SetActive(true);
			}
			for (int j = charCount; j < charHolder.childCount; j++)
			{
				charHolder.GetChild(j).gameObject.SetActive(false);
			}
		}
		bool win = this._isWin;
		float aniTime = win ? 2f : 2f;
		CanvasGroup btnCanvas = this.btnClose.GetComponent<CanvasGroup>();
		this.resultAni.AnimationState.SetAnimation(0, win ? "combat_win" : "combat_lose", false);
		this.goPointerMask.SetActive(true);
		this.cgMainWindow.alpha = 0f;
		this.cgMainWindow.DOFade(1f, 0.5f).SetDelay(aniTime).OnComplete(delegate
		{
			this.goPointerMask.SetActive(false);
		});
		this.btnClose.interactable = false;
		btnCanvas.alpha = 0f;
		btnCanvas.DOFade(1f, 0.5f).SetDelay(aniTime).OnComplete(delegate
		{
			this.btnClose.interactable = true;
		});
	}

	// Token: 0x060020CC RID: 8396 RVA: 0x000EF9A4 File Offset: 0x000EDBA4
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "Close";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x060020CD RID: 8397 RVA: 0x000EF9D4 File Offset: 0x000EDBD4
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x060020CE RID: 8398 RVA: 0x000EFA04 File Offset: 0x000EDC04
	public override void QuickHide()
	{
		bool flag = !this.btnClose.interactable;
		if (!flag)
		{
			UIManager.Instance.HideUI(UIElement.CricketCombat);
			UIManager.Instance.HideUI(this.Element);
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("CricketCombatOver", "WinState", this._isWin);
			TaiwuEventDomainMethod.Call.TriggerListener("CricketCombatOver", true);
			SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
		}
	}

	// Token: 0x04001920 RID: 6432
	[SerializeField]
	private TextMeshProUGUI txtMeshPrevResource;

	// Token: 0x04001921 RID: 6433
	[SerializeField]
	private TextMeshProUGUI txtMeshAddResource;

	// Token: 0x04001922 RID: 6434
	[SerializeField]
	private RectTransform rectTsItemHolder;

	// Token: 0x04001923 RID: 6435
	[SerializeField]
	private RectTransform rectTsCharHolder;

	// Token: 0x04001924 RID: 6436
	[SerializeField]
	private CButton btnClose;

	// Token: 0x04001925 RID: 6437
	[SerializeField]
	private GameObject goPointerMask;

	// Token: 0x04001926 RID: 6438
	[SerializeField]
	private TextMeshProUGUI txtMeshEvaluation;

	// Token: 0x04001927 RID: 6439
	[SerializeField]
	private CImage imgResourceIcon;

	// Token: 0x04001928 RID: 6440
	[SerializeField]
	private SkeletonGraphic resultAni;

	// Token: 0x04001929 RID: 6441
	[SerializeField]
	private CanvasGroup cgMainWindow;

	// Token: 0x0400192A RID: 6442
	[SerializeField]
	private RectTransform rectTsResourceHolder;

	// Token: 0x0400192B RID: 6443
	[SerializeField]
	private TextMeshProUGUI txtMeshResourceName;

	// Token: 0x0400192C RID: 6444
	[SerializeField]
	private GameObject goCharBack;

	// Token: 0x0400192D RID: 6445
	[SerializeField]
	private TextMeshProUGUI txtMeshLootTitle;

	// Token: 0x0400192E RID: 6446
	[SerializeField]
	private GameObject goResourceView;

	// Token: 0x0400192F RID: 6447
	private const float WinAniTime = 2f;

	// Token: 0x04001930 RID: 6448
	private const float LoseAniTime = 2f;

	// Token: 0x04001931 RID: 6449
	private const float FadeTime = 0.5f;

	// Token: 0x04001932 RID: 6450
	private bool _isWin;

	// Token: 0x04001933 RID: 6451
	private Wager _wager;

	// Token: 0x04001934 RID: 6452
	private List<CharacterDisplayData> _wagerCharList = new List<CharacterDisplayData>();

	// Token: 0x04001935 RID: 6453
	private CharacterDisplayData _taiwuChar;

	// Token: 0x04001936 RID: 6454
	private CharacterDisplayData _enemyChar;

	// Token: 0x04001937 RID: 6455
	private ItemDisplayData _itemDisplayData;
}
