using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.World;
using TMPro;
using UnityEngine;

// Token: 0x02000239 RID: 569
public class UI_SelectLegacyRewardGroup : UIBase
{
	// Token: 0x170003C1 RID: 961
	// (get) Token: 0x06002507 RID: 9479 RVA: 0x001108E8 File Offset: 0x0010EAE8
	public TooltipInvoker disableMouseTip
	{
		get
		{
			return base.CGet<TooltipInvoker>("DisableMouseTip");
		}
	}

	// Token: 0x170003C2 RID: 962
	// (get) Token: 0x06002508 RID: 9480 RVA: 0x001108F5 File Offset: 0x0010EAF5
	private bool HasEnoughLegacyPoint
	{
		get
		{
			return this._legacyPoint >= this._cost;
		}
	}

	// Token: 0x06002509 RID: 9481 RVA: 0x00110908 File Offset: 0x0010EB08
	private void Awake()
	{
		this.titleBackTips1.Type = TipType.LegacyLevel;
		this.titleBackTips2.Type = TipType.LegacyLevel;
		this.titleBackTips3.Type = TipType.LegacyLevel;
	}

	// Token: 0x0600250A RID: 9482 RVA: 0x0011093C File Offset: 0x0010EB3C
	public override void OnInit(ArgumentBox argsBox)
	{
		this.Init();
		argsBox.Get<WorldCreationInfo>("WorldCreationInfo", out this._worldCreationInfo);
		argsBox.Get("Cost", out this._cost);
		argsBox.Get("LegacyPoint", out this._legacyPoint);
		argsBox.Get<Action<short>>("OnSelectLegacy", out this._onSelectReward);
		argsBox.Get<Action>("OnCreateRandomLegacy", out this._onCreateRandomLegacy);
		this._window = base.CGet<PopupWindow>("Window");
		this._window.OnConfirmClick = new Action(this.OnConfirmSelect);
		this._window.OnCancelClick = new Action(this.QuickHide);
		this._costValue = base.CGet<TextMeshProUGUI>("Value");
		this._costValueNeed = base.CGet<TextMeshProUGUI>("ValueNeed");
		this._toggleGroup = base.CGet<CToggleGroupObsolete>("Groups");
		this._toggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChanged);
		this._toggleGroup.InitPreOnToggle(-1);
		bool flag = this._toggleGroup.AnyTogglesOn();
		if (flag)
		{
			this._toggleGroup.Set(this._toggleGroup.GetActive(), false);
		}
		List<CToggleObsolete> toggles = this._toggleGroup.GetAll();
		string dot = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
		sbyte i = 0;
		while ((int)i < toggles.Count)
		{
			sbyte templateId = i;
			WorldCreationGroupItem groupCfg = WorldCreationGroup.Instance[templateId];
			int level = this._worldCreationInfo.GetGroupLevel(templateId);
			string levelStr = LocalStringManager.Get(string.Format("LK_WorldCreation_GroupLevel_{0}", level));
			Refers refers = toggles[(int)i].GetComponent<Refers>();
			Color color = WorldDetailSettingGroup.GetLevelColor(level);
			string titleText = (groupCfg.Name + dot + levelStr).SetColor(color);
			refers.CGet<CImage>("Image").SetSprite(groupCfg.Image, false, null);
			refers.CGet<TextMeshProUGUI>("TitleText").SetText(titleText, true);
			refers.CGet<CImage>("TitleBack").sprite = this._titleBackSprites[level];
			refers.CGet<CButtonObsolete>("CheckLegacyButton").ClearAndAddListener(delegate
			{
				this.CheckGroupLegacies(templateId);
			});
			i += 1;
		}
		this.UpdateCost();
		this.UpdateConfirmButton();
	}

	// Token: 0x0600250B RID: 9483 RVA: 0x00110BA0 File Offset: 0x0010EDA0
	private void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			base.CGet<CButtonObsolete>("ClosePanel").ClearAndAddListener(new Action(this.QuickHide));
			this.disableMouseTip.PresetParam = new string[1];
			this.disableMouseTip.IsLanguageKey = true;
			this._inited = true;
		}
	}

	// Token: 0x0600250C RID: 9484 RVA: 0x00110BFC File Offset: 0x0010EDFC
	private void UpdateCost()
	{
		string value = this._legacyPoint.ToString().SetColor(this.HasEnoughLegacyPoint ? "brightblue" : "brightred");
		this._costValue.SetText(value ?? "", true);
		this._costValueNeed.SetText(string.Format("/{0}", this._cost), true);
	}

	// Token: 0x0600250D RID: 9485 RVA: 0x00110C68 File Offset: 0x0010EE68
	private void UpdateConfirmButton()
	{
		this._window.ConfirmButton.interactable = (this._toggleGroup.AnyTogglesOn() && this.HasEnoughLegacyPoint);
		bool flag = !this.HasEnoughLegacyPoint;
		if (flag)
		{
			this.disableMouseTip.PresetParam[0] = "LK_Legacy_NotEnoughLegacyPoints";
		}
		else
		{
			bool flag2 = !this._toggleGroup.AnyTogglesOn();
			if (flag2)
			{
				this.disableMouseTip.PresetParam[0] = "LK_Legacy_NeedSelectRandomBonus";
			}
		}
	}

	// Token: 0x0600250E RID: 9486 RVA: 0x00110CE8 File Offset: 0x0010EEE8
	private void OnConfirmSelect()
	{
		this._onCreateRandomLegacy();
		this.QuickHide();
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("GroupId", this._toggleGroup.GetActive().Key).SetObject("OnSelectLegacy", this._onSelectReward);
		UIElement.SelectRandomLegacyReward.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.SelectRandomLegacyReward, true);
	}

	// Token: 0x0600250F RID: 9487 RVA: 0x00110D56 File Offset: 0x0010EF56
	private void OnActiveToggleChanged(CToggleObsolete curr, CToggleObsolete prev)
	{
		this.UpdateCost();
		this.UpdateConfirmButton();
	}

	// Token: 0x06002510 RID: 9488 RVA: 0x00110D68 File Offset: 0x0010EF68
	private void CheckGroupLegacies(sbyte templateId)
	{
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("GroupId", templateId).Set("Level", this._worldCreationInfo.GetGroupLevel(templateId));
		UIElement.DisplayConfigLegacy.SetOnInitArgs(argumentBox);
		UIManager.Instance.ShowUI(UIElement.DisplayConfigLegacy, true);
	}

	// Token: 0x04001B9A RID: 7066
	[SerializeField]
	private TooltipInvoker titleBackTips1;

	// Token: 0x04001B9B RID: 7067
	[SerializeField]
	private TooltipInvoker titleBackTips2;

	// Token: 0x04001B9C RID: 7068
	[SerializeField]
	private TooltipInvoker titleBackTips3;

	// Token: 0x04001B9D RID: 7069
	[SerializeField]
	private Sprite[] _titleBackSprites;

	// Token: 0x04001B9E RID: 7070
	private PopupWindow _window;

	// Token: 0x04001B9F RID: 7071
	private CToggleGroupObsolete _toggleGroup;

	// Token: 0x04001BA0 RID: 7072
	private TextMeshProUGUI _costValue;

	// Token: 0x04001BA1 RID: 7073
	private TextMeshProUGUI _costValueNeed;

	// Token: 0x04001BA2 RID: 7074
	private WorldCreationInfo _worldCreationInfo;

	// Token: 0x04001BA3 RID: 7075
	private int _legacyPoint;

	// Token: 0x04001BA4 RID: 7076
	private Action<short> _onSelectReward;

	// Token: 0x04001BA5 RID: 7077
	private Action _onCreateRandomLegacy;

	// Token: 0x04001BA6 RID: 7078
	private int _cost;

	// Token: 0x04001BA7 RID: 7079
	private bool _inited = false;
}
