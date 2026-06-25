using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using TMPro;

// Token: 0x02000238 RID: 568
public class UI_DisplayConifgLegacy : UIBase
{
	// Token: 0x06002500 RID: 9472 RVA: 0x00110688 File Offset: 0x0010E888
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get("GroupId", out this._templateId);
		argsBox.Get("Level", out this._level);
		bool flag = !this._inited;
		if (flag)
		{
			this._inited = true;
			this.InitRefers();
			this._verticalScrollView.OnItemRender = new Action<int, Refers>(this.OnRenderLegacyItem);
			this._buttonClose.ClearAndAddListener(delegate
			{
				this.QuickHide();
			});
		}
		this.InitTitle();
		this.InitScroll();
	}

	// Token: 0x06002501 RID: 9473 RVA: 0x00110714 File Offset: 0x0010E914
	private void InitTitle()
	{
		string worldCreationName = WorldCreationGroup.Instance[this._templateId].Name;
		string dotSymbol = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
		string groupLevelKey = string.Format("LK_WorldCreation_GroupLevel_{0}", this._level);
		string groupLevel = LocalStringManager.Get(groupLevelKey);
		this._title.text = worldCreationName + dotSymbol + groupLevel;
	}

	// Token: 0x06002502 RID: 9474 RVA: 0x00110774 File Offset: 0x0010E974
	private void InitScroll()
	{
		this._legacyItems.Clear();
		foreach (LegacyItem legacyItem in ((IEnumerable<LegacyItem>)Legacy.Instance))
		{
			bool flag = (int)legacyItem.Level == this._level && legacyItem.WorldCreationGroup == this._templateId && legacyItem.Weight > 0;
			if (flag)
			{
				this._legacyItems.Add(legacyItem);
			}
		}
		this._verticalScrollView.SetDataCount(this._legacyItems.Count);
	}

	// Token: 0x06002503 RID: 9475 RVA: 0x0011081C File Offset: 0x0010EA1C
	private void OnRenderLegacyItem(int index, Refers refers)
	{
		LegacyView legacyView = refers as LegacyView;
		LegacyItem configData = this._legacyItems[index];
		legacyView.RefreshBasicInfo(configData);
		legacyView.RefreshCostInfo(configData, false, false, true, true, false);
		legacyView.RefreshMouseTip(configData, configData.Desc);
		legacyView.RefreshHighlight(false, true, false);
		legacyView.RefreshInteraction(false, false, false);
	}

	// Token: 0x06002504 RID: 9476 RVA: 0x00110878 File Offset: 0x0010EA78
	private void InitRefers()
	{
		this._verticalScrollView = base.CGet<InfinityScrollLegacy>("VerticalScrollView");
		this._buttonClose = base.CGet<CButtonObsolete>("ButtonClose");
		this._title = base.CGet<TextMeshProUGUI>("Title");
		this._imgTitle36 = base.CGet<TooltipInvoker>("ImgTitle36");
	}

	// Token: 0x04001B92 RID: 7058
	private bool _inited;

	// Token: 0x04001B93 RID: 7059
	private sbyte _templateId;

	// Token: 0x04001B94 RID: 7060
	private int _level;

	// Token: 0x04001B95 RID: 7061
	private readonly List<LegacyItem> _legacyItems = new List<LegacyItem>();

	// Token: 0x04001B96 RID: 7062
	private InfinityScrollLegacy _verticalScrollView;

	// Token: 0x04001B97 RID: 7063
	private CButtonObsolete _buttonClose;

	// Token: 0x04001B98 RID: 7064
	private TextMeshProUGUI _title;

	// Token: 0x04001B99 RID: 7065
	private TooltipInvoker _imgTitle36;
}
