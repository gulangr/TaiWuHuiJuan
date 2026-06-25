using System;
using Coffee.UIExtensions;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000310 RID: 784
public class TeammateCommandLongItem : Refers
{
	// Token: 0x06002E41 RID: 11841 RVA: 0x0016DD54 File Offset: 0x0016BF54
	private void Awake()
	{
		bool flag = !this._inited;
		if (flag)
		{
			this.Init();
		}
	}

	// Token: 0x06002E42 RID: 11842 RVA: 0x0016DD78 File Offset: 0x0016BF78
	public void Init()
	{
		this.InitRefers();
		this._button.ClearAndAddListener(new Action(this.OnButtonClick));
		this._deleteButton.ClearAndAddListener(new Action(this.OnDeletButtonClick));
		this._downgradeButton.ClearAndAddListener(new Action(this.OnDowngradeButtonClick));
		bool flag = !this._inited;
		if (flag)
		{
			this._advanceEffect.gameObject.SetActive(true);
			this._advanceEffect.Play();
			this._advanceEffect.GetComponent<CanvasGroup>().alpha = 0f;
		}
		this._inited = true;
	}

	// Token: 0x06002E43 RID: 11843 RVA: 0x0016DE1F File Offset: 0x0016C01F
	private void OnEnable()
	{
		this._advanceEffect.Play();
	}

	// Token: 0x06002E44 RID: 11844 RVA: 0x0016DE30 File Offset: 0x0016C030
	public void Refresh(int index, int cmdType, bool isSelected = false, Action<int> onClick = null, bool showDeleteButton = false, Action<int> onDeleteClick = null, bool isEmpty = false, bool isOwned = false, bool isDisabled = false, Action<int> onDowngradeClick = null, bool showMedal = false)
	{
		this._index = index;
		this._onClick = onClick;
		this.SetSelect(isSelected);
		if (isEmpty)
		{
			this._content.SetActive(false);
			this._deleteButton.interactable = false;
			this._emptyImage.SetSprite(isSelected ? "ui_charactermenu_order_base_1" : "ui_charactermenu_order_base_0", false, null);
		}
		else
		{
			this._content.SetActive(true);
			this._config = TeammateCommand.Instance[cmdType];
			this._onDeleteClick = onDeleteClick;
			this._deleteButton.interactable = (onDeleteClick != null);
			this._onDowngradeClick = onDowngradeClick;
			this._downgradeButton.gameObject.SetActive(onDowngradeClick != null);
			this._desc.text = this._config.Description.ColorReplace();
			this.RefreshBg(isSelected);
			this.RefreshCommandItem();
			this.SetOwned(isOwned);
			this.SetDisable(isDisabled);
			this.SetMedal(showMedal);
		}
	}

	// Token: 0x06002E45 RID: 11845 RVA: 0x0016DF33 File Offset: 0x0016C133
	public void SetSelect(bool isSelect)
	{
		this._selected.SetActive(isSelect);
	}

	// Token: 0x06002E46 RID: 11846 RVA: 0x0016DF43 File Offset: 0x0016C143
	public void SetOwned(bool isOwned)
	{
		this._ownedMark.SetActive(isOwned);
	}

	// Token: 0x06002E47 RID: 11847 RVA: 0x0016DF54 File Offset: 0x0016C154
	private void RefreshCommandItem()
	{
		TextMeshProUGUI nameLabel3 = this._characterTeammateCommand.CGet<TextMeshProUGUI>("NameLabel3");
		TeammateCommandItem config = TeammateCommand.Instance[this._config.TemplateId];
		nameLabel3.text = config.Name;
		this._medalName.text = config.Name;
		TooltipInvoker mouseTip = base.CGet<TooltipInvoker>("MouseTip");
		bool flag = mouseTip == null;
		if (!flag)
		{
			TooltipInvoker tooltipInvoker = mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			mouseTip.enabled = MouseTipTeammateCommand.CanUse(this._config.TemplateId);
			mouseTip.RuntimeParam.Set("CommandId", this._config.TemplateId);
		}
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x0016E010 File Offset: 0x0016C210
	public void SetDisable(bool isDisabled)
	{
		DisableStyleRoot disableRoot = this._bg.GetComponent<DisableStyleRoot>();
		disableRoot.EffectTextColor = Colors.Instance["lightgrey"];
		disableRoot.SetStyleEffect(isDisabled, false);
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x0016E048 File Offset: 0x0016C248
	public void SetPointerTriggerDisable(bool isDisabled)
	{
		PointerTrigger pointerTrigger = this._button.GetComponent<PointerTrigger>();
		pointerTrigger.enabled = !isDisabled;
	}

	// Token: 0x06002E4A RID: 11850 RVA: 0x0016E06D File Offset: 0x0016C26D
	public void SetButtonEnable(bool isEnable)
	{
		this._button.enabled = isEnable;
		this._selected.SetActive(false);
	}

	// Token: 0x06002E4B RID: 11851 RVA: 0x0016E08C File Offset: 0x0016C28C
	public void SetMedal(bool isShowMedal)
	{
		this._medal.SetActive(isShowMedal);
		if (isShowMedal)
		{
			this._medalIcon.SetSprite(CommonUtils.GetFeatureMedalIcon((int)this._config.MedalType, (int)this._config.MedalCount), false, null);
			this._medalCount.text = string.Format("x {0}", this._config.MedalCount);
		}
	}

	// Token: 0x06002E4C RID: 11852 RVA: 0x0016E100 File Offset: 0x0016C300
	public void SetTipsText(string tipsText = null)
	{
		TooltipInvoker tipDisplayer = this._button.GetComponent<TooltipInvoker>();
		bool tipDisplayerEnabled = tipsText != null;
		tipDisplayer.enabled = tipDisplayerEnabled;
		bool flag = !tipDisplayerEnabled;
		if (!flag)
		{
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tipDisplayer.RuntimeParam.Set("arg0", tipsText);
		}
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x0016E15C File Offset: 0x0016C35C
	public void SetCanUpgradeEffect(bool isShow)
	{
		this._advanceEffect.GetComponent<CanvasGroup>().alpha = (float)(isShow ? 1 : 0);
	}

	// Token: 0x06002E4E RID: 11854 RVA: 0x0016E178 File Offset: 0x0016C378
	public void PlayUpgradeEffect()
	{
		this._upgradeEffect.gameObject.SetActive(true);
		this._upgradeEffect.Play();
		bool flag = this._upgradeEffectCoroutine != null;
		if (flag)
		{
			base.StopCoroutine(this._upgradeEffectCoroutine);
		}
		this._upgradeEffectCoroutine = SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.3f, delegate
		{
			UIParticle upgradeEffect = this._upgradeEffect;
			if (upgradeEffect != null)
			{
				GameObject gameObject = upgradeEffect.gameObject;
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
			UIParticle upgradeEffect2 = this._upgradeEffect;
			if (upgradeEffect2 != null)
			{
				upgradeEffect2.Stop();
			}
		});
	}

	// Token: 0x06002E4F RID: 11855 RVA: 0x0016E1E1 File Offset: 0x0016C3E1
	private void RefreshBg(bool selected)
	{
		this._bg.SetSprite((this._config.Type == ETeammateCommandType.Advance) ? "popup_upgradeteammatecommand_progress_1" : (selected ? "ui_charactermenu_order_base_1" : "ui_charactermenu_order_base_0"), false, null);
	}

	// Token: 0x06002E50 RID: 11856 RVA: 0x0016E216 File Offset: 0x0016C416
	private void OnButtonClick()
	{
		Action<int> onClick = this._onClick;
		if (onClick != null)
		{
			onClick(this._index);
		}
	}

	// Token: 0x06002E51 RID: 11857 RVA: 0x0016E231 File Offset: 0x0016C431
	private void OnDeletButtonClick()
	{
		Action<int> onDeleteClick = this._onDeleteClick;
		if (onDeleteClick != null)
		{
			onDeleteClick(this._index);
		}
	}

	// Token: 0x06002E52 RID: 11858 RVA: 0x0016E24C File Offset: 0x0016C44C
	private void OnDowngradeButtonClick()
	{
		Action<int> onDowngradeClick = this._onDowngradeClick;
		if (onDowngradeClick != null)
		{
			onDowngradeClick(this._index);
		}
	}

	// Token: 0x06002E53 RID: 11859 RVA: 0x0016E268 File Offset: 0x0016C468
	private void InitRefers()
	{
		this._bg = base.CGet<CImage>("Bg");
		this._emptyImage = base.CGet<CImage>("EmptyImage");
		this._ownedMark = base.CGet<GameObject>("OwnedMark");
		this._characterTeammateCommand = base.CGet<Refers>("CharacterTeammateCommand");
		this._desc = base.CGet<TextMeshProUGUI>("Desc");
		this._button = base.CGet<CButtonObsolete>("Button");
		this._selected = base.CGet<GameObject>("Selected");
		this._deleteButton = base.CGet<CButtonObsolete>("DeleteButton");
		this._empty = base.CGet<GameObject>("Empty");
		this._content = base.CGet<GameObject>("Content");
		this._downgradeButton = base.CGet<CButtonObsolete>("DowngradeButton");
		this._medal = base.CGet<GameObject>("Medal");
		this._medalIcon = base.CGet<CImage>("MedalIcon");
		this._medalCount = base.CGet<TextMeshProUGUI>("MedalCount");
		this._medalName = base.CGet<TextMeshProUGUI>("MedalName");
		this._advanceEffect = base.CGet<UIParticle>("AdvanceEffect");
		this._upgradeEffect = base.CGet<UIParticle>("UpgradeEffect");
	}

	// Token: 0x04002179 RID: 8569
	private bool _inited = false;

	// Token: 0x0400217A RID: 8570
	private TeammateCommandItem _config;

	// Token: 0x0400217B RID: 8571
	private int _index = -1;

	// Token: 0x0400217C RID: 8572
	private Action<int> _onClick;

	// Token: 0x0400217D RID: 8573
	private Action<int> _onDeleteClick;

	// Token: 0x0400217E RID: 8574
	private Action<int> _onDowngradeClick;

	// Token: 0x0400217F RID: 8575
	private Coroutine _upgradeEffectCoroutine;

	// Token: 0x04002180 RID: 8576
	private CImage _bg;

	// Token: 0x04002181 RID: 8577
	private CImage _emptyImage;

	// Token: 0x04002182 RID: 8578
	private GameObject _ownedMark;

	// Token: 0x04002183 RID: 8579
	private Refers _characterTeammateCommand;

	// Token: 0x04002184 RID: 8580
	private TextMeshProUGUI _desc;

	// Token: 0x04002185 RID: 8581
	private CButtonObsolete _button;

	// Token: 0x04002186 RID: 8582
	private GameObject _selected;

	// Token: 0x04002187 RID: 8583
	private CButtonObsolete _deleteButton;

	// Token: 0x04002188 RID: 8584
	private GameObject _empty;

	// Token: 0x04002189 RID: 8585
	private GameObject _content;

	// Token: 0x0400218A RID: 8586
	private CButtonObsolete _downgradeButton;

	// Token: 0x0400218B RID: 8587
	private GameObject _medal;

	// Token: 0x0400218C RID: 8588
	private CImage _medalIcon;

	// Token: 0x0400218D RID: 8589
	private TextMeshProUGUI _medalCount;

	// Token: 0x0400218E RID: 8590
	private TextMeshProUGUI _medalName;

	// Token: 0x0400218F RID: 8591
	private UIParticle _advanceEffect;

	// Token: 0x04002190 RID: 8592
	private UIParticle _upgradeEffect;
}
