using System;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia.Event;
using Game.Views.Encyclopedia.Save;
using Game.Views.Encyclopedia.SyntaxTree;
using Game.Views.Encyclopedia.Utilities;
using Game.Views.Encyclopedia.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A92 RID: 2706
	public class TitleElement : Element, ISearch
	{
		// Token: 0x17000E7F RID: 3711
		// (get) Token: 0x0600846A RID: 33898 RVA: 0x003D9BD0 File Offset: 0x003D7DD0
		internal RectTransform ChildrenContainer
		{
			get
			{
				return this.childrenContainer;
			}
		}

		// Token: 0x17000E80 RID: 3712
		// (get) Token: 0x0600846B RID: 33899 RVA: 0x003D9BD8 File Offset: 0x003D7DD8
		// (set) Token: 0x0600846C RID: 33900 RVA: 0x003D9BE0 File Offset: 0x003D7DE0
		internal int Index { get; set; }

		// Token: 0x17000E81 RID: 3713
		// (get) Token: 0x0600846D RID: 33901 RVA: 0x003D9BE9 File Offset: 0x003D7DE9
		internal int LinkId
		{
			get
			{
				return base.NodeData.Id;
			}
		}

		// Token: 0x17000E82 RID: 3714
		// (get) Token: 0x0600846E RID: 33902 RVA: 0x003D9BF6 File Offset: 0x003D7DF6
		public EncyclopediaContentItem ConfigItem
		{
			get
			{
				return EncyclopediaContent.Instance[this.LinkId];
			}
		}

		// Token: 0x17000E83 RID: 3715
		// (get) Token: 0x0600846F RID: 33903 RVA: 0x003D9C08 File Offset: 0x003D7E08
		// (set) Token: 0x06008470 RID: 33904 RVA: 0x003D9C10 File Offset: 0x003D7E10
		internal string LinkIdName { get; private set; }

		// Token: 0x17000E84 RID: 3716
		// (get) Token: 0x06008471 RID: 33905 RVA: 0x003D9C19 File Offset: 0x003D7E19
		// (set) Token: 0x06008472 RID: 33906 RVA: 0x003D9C21 File Offset: 0x003D7E21
		internal string TitleName { get; private set; }

		// Token: 0x17000E85 RID: 3717
		// (get) Token: 0x06008473 RID: 33907 RVA: 0x003D9C2A File Offset: 0x003D7E2A
		internal NodeLayerType ElementType
		{
			get
			{
				return this.elementType;
			}
		}

		// Token: 0x06008474 RID: 33908 RVA: 0x003D9C34 File Offset: 0x003D7E34
		private void Awake()
		{
			bool flag = this.selfButton;
			if (flag)
			{
				this.selfButton.onClick.AddListener(new UnityAction(this.OnClickSelf));
			}
			bool flag2 = this.extendButton;
			if (flag2)
			{
				this.extendButton.onClick.AddListener(new UnityAction(this.OnClickExtend));
			}
			bool flag3 = this.favoriteIcon != null;
			if (flag3)
			{
				this.favoriteIcon.onValueChanged.AddListener(new UnityAction<int>(this.OnFavoriteTypeChanged));
				this._favoriteTip = this.favoriteIcon.GetComponent<TooltipInvoker>();
			}
			bool flag4 = base.NodeData != null;
			if (flag4)
			{
				base.Init(base.NodeData, null);
			}
		}

		// Token: 0x06008475 RID: 33909 RVA: 0x003D9CF8 File Offset: 0x003D7EF8
		protected override void OnInit()
		{
			this.LinkIdName = base.NodeData.Key;
			bool flag = base.NodeData.NodeLayerType == NodeLayerType.Two;
			if (flag)
			{
				MultiStateToggle multiStateToggle = this.favoriteIcon;
				if (multiStateToggle != null)
				{
					multiStateToggle.gameObject.SetActive(false);
				}
			}
			bool isSelecting = BasicInfoView.IsShowSearchResult && base.NodeData.Id == BasicInfoView.CurSearchResultId;
			this.title.text = (this.TitleName = Utility.GetHighlightText(base.NodeData.Title, base.NodeData, null, BasicInfoView.CurSearchResultIndex.SingleTextIndex, isSelecting));
			bool flag2 = this.childrenContainer != null;
			if (flag2)
			{
				this.Refresh(base.NodeData.DefaultDisplayChild ? TitleShowMode.Down : TitleShowMode.Up);
			}
			this.Refresh(TitleShowMode.Normal);
			this.RefreshFavoriteIcon();
			this.RefreshButtonExtend();
		}

		// Token: 0x06008476 RID: 33910 RVA: 0x003D9DDC File Offset: 0x003D7FDC
		internal void RefreshFavoriteIcon()
		{
			bool isDone = Save.SaveData.FavoritesInfos.Contains(base.NodeData.ConfigItem.Key);
			MultiStateToggle multiStateToggle = this.favoriteIcon;
			if (multiStateToggle != null)
			{
				multiStateToggle.UpdateState(isDone ? 0 : 1);
			}
			bool flag = this._favoriteTip;
			if (flag)
			{
				LanguageKey languageKey = isDone ? LanguageKey.LK_Encyclopedia_Favorite_Button_Done : LanguageKey.LK_Encyclopedia_Favorite_Button;
				this._favoriteTip.PresetParam = new string[]
				{
					languageKey.Tr()
				};
			}
		}

		// Token: 0x06008477 RID: 33911 RVA: 0x003D9E60 File Offset: 0x003D8060
		public void SetDropDown()
		{
			TitleShowMode mode = this.IsShowChildren() ? TitleShowMode.Up : TitleShowMode.Down;
			this.Refresh(mode);
		}

		// Token: 0x06008478 RID: 33912 RVA: 0x003D9E84 File Offset: 0x003D8084
		public void RefreshButtonExtend()
		{
			bool flag = !this.extendButton;
			if (!flag)
			{
				bool hasChild = false;
				bool flag2 = this.childrenContainer;
				if (flag2)
				{
					for (int i = 0; i < this.childrenContainer.childCount; i++)
					{
						Transform child = this.childrenContainer.GetChild(i);
						bool activeSelf = child.gameObject.activeSelf;
						if (activeSelf)
						{
							hasChild = true;
						}
					}
				}
				this.extendButton.transform.localScale = (hasChild ? Vector3.one : Vector3.zero);
			}
		}

		// Token: 0x06008479 RID: 33913 RVA: 0x003D9F1C File Offset: 0x003D811C
		public void Refresh(TitleShowMode titleShowMode)
		{
			switch (titleShowMode)
			{
			case TitleShowMode.Normal:
			{
				bool flag = this.highlight;
				if (flag)
				{
					this.highlight.gameObject.SetActive(false);
				}
				this.RefreshSelectState(false);
				break;
			}
			case TitleShowMode.Highlight:
			{
				bool flag2 = this.highlight;
				if (flag2)
				{
					this.highlight.gameObject.SetActive(true);
				}
				this.RefreshSelectState(true);
				break;
			}
			case TitleShowMode.Down:
			{
				bool flag3 = this.ChildrenContainer;
				if (flag3)
				{
					this.ChildrenContainer.gameObject.SetActive(true);
				}
				bool flag4 = this.icon;
				if (flag4)
				{
					this.icon.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
				}
				break;
			}
			case TitleShowMode.Up:
			{
				bool flag5 = this.ChildrenContainer;
				if (flag5)
				{
					this.ChildrenContainer.gameObject.SetActive(false);
				}
				bool flag6 = this.icon;
				if (flag6)
				{
					this.icon.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				}
				break;
			}
			}
		}

		// Token: 0x0600847A RID: 33914 RVA: 0x003DA05C File Offset: 0x003D825C
		public bool IsShowChildren()
		{
			return this.childrenContainer.gameObject.activeSelf;
		}

		// Token: 0x0600847B RID: 33915 RVA: 0x003DA07E File Offset: 0x003D827E
		protected override void OnRelease()
		{
			this.OnClickSelfEvent = null;
			this.ParentElement = null;
		}

		// Token: 0x0600847C RID: 33916 RVA: 0x003DA090 File Offset: 0x003D8290
		private void OnClickSelf()
		{
			bool flag = this.tip;
			if (flag)
			{
				this.tip.HideTips();
			}
			Action<TitleElement> onClickSelfEvent = this.OnClickSelfEvent;
			if (onClickSelfEvent != null)
			{
				onClickSelfEvent(this);
			}
		}

		// Token: 0x0600847D RID: 33917 RVA: 0x003DA0CC File Offset: 0x003D82CC
		private void OnClickExtend()
		{
			this.SetDropDown();
			bool flag = BasicInfoView.Instance.ViewState == BasicInfoView.BasicInfoViewState.Normal && base.NodeData.Layer == EEncyclopediaContentLayer.Three;
			if (flag)
			{
				BasicInfoView.Instance.ChangeLevelThreeNodeFoldedStatus(base.NodeData.ConfigItem.Key, false);
			}
		}

		// Token: 0x0600847E RID: 33918 RVA: 0x003DA120 File Offset: 0x003D8320
		private void OnFavoriteTypeChanged(int currentState)
		{
			bool isFavorite = currentState == 0;
			FavoriteTypeChangedEventArgs args = new FavoriteTypeChangedEventArgs
			{
				ToFavorite = isFavorite,
				DataId = base.NodeData.Id
			};
			IEventArgs arg = EventArgs<FavoriteTypeChangedEventArgs>.CreateEventArgs(args);
			EventManager.Instance.Dispatch(2, arg);
			this.RefreshFavoriteIcon();
			this._favoriteTip.Refresh(true, -1);
		}

		// Token: 0x0600847F RID: 33919 RVA: 0x003DA180 File Offset: 0x003D8380
		public void RefreshSearchCount(int count)
		{
			bool show = count > 0;
			this.searchCountBg.gameObject.SetActive(show);
			bool flag = show;
			if (flag)
			{
				this.searchCount.text = count.ToString();
			}
		}

		// Token: 0x06008480 RID: 33920 RVA: 0x003DA1C0 File Offset: 0x003D83C0
		public void RefreshSearchResultHighlight(OptimizedHtmlPatternMatcher value, bool onlyTitle = false)
		{
			bool flag = this._cachedSearchingValue != ((value != null) ? value.Pattern : null);
			if (flag)
			{
				this._cachedSearchingValue = ((value != null) ? value.Pattern : null);
				this._cachedHighlightText = Utility.GetHighlightText(base.NodeData.Title, base.NodeData, value, BasicInfoView.CurSearchResultIndex.SingleTextIndex, false);
			}
			bool isSelecting = BasicInfoView.IsShowSearchResult && base.NodeData.Id == BasicInfoView.CurSearchResultId;
			this.title.text = (isSelecting ? Utility.GetHighlightText(base.NodeData.Title, base.NodeData, value, BasicInfoView.CurSearchResultIndex.SingleTextIndex, true) : this._cachedHighlightText);
			this.title.SetAllDirty();
		}

		// Token: 0x06008481 RID: 33921 RVA: 0x003DA287 File Offset: 0x003D8487
		public override void RefreshShowStatus()
		{
			this.RefreshLockTip();
			base.RefreshShowStatus();
		}

		// Token: 0x06008482 RID: 33922 RVA: 0x003DA298 File Offset: 0x003D8498
		private void RefreshLockTip()
		{
			EEncyclopediaContentLayer layer = base.NodeData.Layer;
			bool hasLockTip = layer == EEncyclopediaContentLayer.Three || layer == EEncyclopediaContentLayer.Four;
			bool flag = hasLockTip && this.selfButton;
			if (flag)
			{
				bool isLocked = base.NodeData.TempTipLevel != EEncyclopediaContentLevel.None;
				this.tip.enabled = isLocked;
				bool flag2 = isLocked;
				if (flag2)
				{
					string tipContent = base.NodeData.TempTipLevel.HasFlag(EEncyclopediaContentLevel.High) ? LanguageKey.LK_Encyclopedia_TitleElement_LockTip_High.Tr() : LanguageKey.LK_Encyclopedia_TitleElement_LockTip_Mid.Tr();
					this.tip.PresetParam = new string[]
					{
						tipContent
					};
				}
			}
			else
			{
				this.HideLockTip();
			}
		}

		// Token: 0x06008483 RID: 33923 RVA: 0x003DA35C File Offset: 0x003D855C
		private void HideLockTip()
		{
			bool flag = this.tip;
			if (flag)
			{
				this.tip.enabled = false;
			}
		}

		// Token: 0x06008484 RID: 33924 RVA: 0x003DA388 File Offset: 0x003D8588
		public void SetIcon(string btnNormalName, string btnHighlightName, string btnDisableName, string btnSelectName)
		{
			CImage img = this.selfButton.GetComponent<CImage>();
			SpriteState state = this.selfButton.spriteState;
			img.SetSprite(btnHighlightName, false, null);
			state.highlightedSprite = img.sprite;
			this._btnHighlightSprite = img.sprite;
			img.SetSprite(btnDisableName, false, null);
			state.disabledSprite = img.sprite;
			this.selfButton.spriteState = state;
			img.SetSprite(btnNormalName, false, null);
			this.highlight.SetSprite(btnSelectName, false, null);
		}

		// Token: 0x06008485 RID: 33925 RVA: 0x003DA414 File Offset: 0x003D8614
		public void SetLine(bool isActive)
		{
			bool flag = this.line != null;
			if (flag)
			{
				this.line.SetActive(isActive);
			}
		}

		// Token: 0x06008486 RID: 33926 RVA: 0x003DA440 File Offset: 0x003D8640
		private void RefreshSelectState(bool isSelect)
		{
			bool flag = this.changeTitleColor;
			if (flag)
			{
				this.title.color = (isSelect ? this.selectTitleColor : this.normalTitleColor);
			}
			bool flag2 = this._btnHighlightSprite == null;
			if (!flag2)
			{
				SpriteState state = this.selfButton.spriteState;
				state.highlightedSprite = (isSelect ? null : this._btnHighlightSprite);
				this.selfButton.spriteState = state;
			}
		}

		// Token: 0x04006583 RID: 25987
		[SerializeField]
		private NodeLayerType elementType;

		// Token: 0x04006584 RID: 25988
		[SerializeField]
		private MultiStateToggle favoriteIcon;

		// Token: 0x04006585 RID: 25989
		[SerializeField]
		private GameObject searchCountBg;

		// Token: 0x04006586 RID: 25990
		[SerializeField]
		private TextMeshProUGUI searchCount;

		// Token: 0x04006587 RID: 25991
		[SerializeField]
		private CButton selfButton;

		// Token: 0x04006588 RID: 25992
		[SerializeField]
		private CButton extendButton;

		// Token: 0x04006589 RID: 25993
		[SerializeField]
		private CImage icon;

		// Token: 0x0400658A RID: 25994
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x0400658B RID: 25995
		[SerializeField]
		private CImage highlight;

		// Token: 0x0400658C RID: 25996
		[SerializeField]
		private RectTransform childrenContainer;

		// Token: 0x0400658D RID: 25997
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x0400658E RID: 25998
		[SerializeField]
		private GameObject line;

		// Token: 0x0400658F RID: 25999
		[SerializeField]
		private bool changeTitleColor;

		// Token: 0x04006590 RID: 26000
		[SerializeField]
		private Color normalTitleColor;

		// Token: 0x04006591 RID: 26001
		[SerializeField]
		private Color selectTitleColor;

		// Token: 0x04006592 RID: 26002
		private TooltipInvoker _favoriteTip;

		// Token: 0x04006593 RID: 26003
		private Sprite _btnHighlightSprite;

		// Token: 0x04006594 RID: 26004
		internal Action<TitleElement> OnClickSelfEvent;

		// Token: 0x04006595 RID: 26005
		internal TitleElement ParentElement;

		// Token: 0x04006599 RID: 26009
		private string _cachedHighlightText;
	}
}
