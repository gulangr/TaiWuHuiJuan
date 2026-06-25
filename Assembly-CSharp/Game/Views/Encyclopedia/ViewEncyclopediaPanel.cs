using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia.Event;
using Game.Views.Encyclopedia.Save;
using Game.Views.Encyclopedia.Views;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A66 RID: 2662
	public class ViewEncyclopediaPanel : UIBase, IEventListener
	{
		// Token: 0x060082C5 RID: 33477 RVA: 0x003CE9F8 File Offset: 0x003CCBF8
		public static void OpenLink(EncyclopediaTipLinkItem link)
		{
			bool flag = link.Mode == EEncyclopediaTipLinkMode.Default;
			if (flag)
			{
				UIElement.Encyclopedia.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("link", link));
				UIManager.Instance.ShowUI(UIElement.Encyclopedia, true);
			}
			else
			{
				Debug.LogError(string.Format("Mode {0} not supported", link.Mode));
			}
		}

		// Token: 0x060082C6 RID: 33478 RVA: 0x003CEA5D File Offset: 0x003CCC5D
		private static void JumpToPrevSearch()
		{
			ViewEncyclopediaPanel._view.JumpToLastSearchResult();
		}

		// Token: 0x060082C7 RID: 33479 RVA: 0x003CEA6A File Offset: 0x003CCC6A
		private static void JumpToNextSearch()
		{
			ViewEncyclopediaPanel._view.JumpToNextSearchResult();
		}

		// Token: 0x060082C8 RID: 33480 RVA: 0x003CEA78 File Offset: 0x003CCC78
		public override void OnInit(ArgumentBox argsBox)
		{
			ViewEncyclopediaPanel._view = this.view;
			Save.LoadInfo();
			bool isFirstTipActive = !Save.SaveData.Inited;
			bool flag = isFirstTipActive;
			if (flag)
			{
				UIManager.Instance.MaskComponent(this.firstTips.GetComponent<RectTransform>());
			}
			else
			{
				UIManager.Instance.UnMaskComponent(this.firstTips.GetComponent<RectTransform>());
			}
			string key = string.Empty;
			if (argsBox != null)
			{
				argsBox.Get("key", out key);
			}
			if (argsBox != null)
			{
				argsBox.Get<EncyclopediaTipLinkItem>("link", out this._linkItem);
			}
			this.view.JumpKey = key;
		}

		// Token: 0x060082C9 RID: 33481 RVA: 0x003CEB14 File Offset: 0x003CCD14
		private void Awake()
		{
			this._languageType = LocalStringManager.CurLanguageType;
			EncyclopediaDataManager.Instance.ReInitialize();
			this.view.NeedInit = true;
			this.InitButtonEvents();
		}

		// Token: 0x060082CA RID: 33482 RVA: 0x003CEB40 File Offset: 0x003CCD40
		private void OnEnable()
		{
			bool flag = this._languageType != LocalStringManager.CurLanguageType;
			if (flag)
			{
				this._languageType = LocalStringManager.CurLanguageType;
				EncyclopediaDataManager.Instance.ReInitialize();
				this.view.NeedInit = true;
			}
			bool flag2 = this._linkItem != null;
			if (flag2)
			{
				BasicInfoView basicInfoView = this.view;
				EncyclopediaReferenceItem encyclopediaReferenceItem = EncyclopediaReference.Instance[this._linkItem.RefName];
				basicInfoView.JumpKey = (((encyclopediaReferenceItem != null) ? encyclopediaReferenceItem.Param : null) ?? string.Empty);
				this._linkItem = null;
			}
			this.view.Show();
		}

		// Token: 0x060082CB RID: 33483 RVA: 0x003CEBE0 File Offset: 0x003CCDE0
		private void Update()
		{
			foreach (Action action in this._hotKey2Action.CheckSeries(this.Element))
			{
				action();
			}
		}

		// Token: 0x060082CC RID: 33484 RVA: 0x003CEC3C File Offset: 0x003CCE3C
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "ButtonClearLabel")
			{
				this.view.ClearTempLabel();
			}
		}

		// Token: 0x060082CD RID: 33485 RVA: 0x003CEC70 File Offset: 0x003CCE70
		private void JumpTo(string key)
		{
			BasicInfoView basicInfoView = this.view;
			basicInfoView.JumpTo(key);
		}

		// Token: 0x060082CE RID: 33486 RVA: 0x003CEC90 File Offset: 0x003CCE90
		public override void QuickHide()
		{
			bool activeSelf = this.firstTips.activeSelf;
			if (activeSelf)
			{
				UIManager.Instance.UnMaskComponent(this.firstTips.GetComponent<RectTransform>());
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			}
			else
			{
				base.QuickHide();
			}
		}

		// Token: 0x060082CF RID: 33487 RVA: 0x003CECE0 File Offset: 0x003CCEE0
		private void InitButtonEvents()
		{
			this.closeButton.onClick.AddListener(new UnityAction(this.OnCloseButtonClick));
			this.view.InitButtonEvents();
			this.firstTipConfirmButton.ClearAndAddListener(new Action(this.OnClickFirstTipConfirmButton));
			this.firstTipCancelButton.ClearAndAddListener(new Action(this.OnClickFirstTipCancelButton));
		}

		// Token: 0x060082D0 RID: 33488 RVA: 0x003CED47 File Offset: 0x003CCF47
		private void OnClickFirstTipConfirmButton()
		{
			this.showLevelsNew.SetInit();
			UIManager.Instance.UnMaskComponent(this.firstTips.GetComponent<RectTransform>());
		}

		// Token: 0x060082D1 RID: 33489 RVA: 0x003CED6C File Offset: 0x003CCF6C
		private void OnClickFirstTipCancelButton()
		{
			UIManager.Instance.UnMaskComponent(this.firstTips.GetComponent<RectTransform>());
		}

		// Token: 0x060082D2 RID: 33490 RVA: 0x003CED85 File Offset: 0x003CCF85
		public void HandleEvent(int eventId, IEventArgs args)
		{
		}

		// Token: 0x060082D3 RID: 33491 RVA: 0x003CED88 File Offset: 0x003CCF88
		private void OnCloseButtonClick()
		{
			UIManager.Instance.HideUI(UIElement.Encyclopedia);
		}

		// Token: 0x04006450 RID: 25680
		[SerializeField]
		private CButton closeButton;

		// Token: 0x04006451 RID: 25681
		[SerializeField]
		private BasicInfoView view;

		// Token: 0x04006452 RID: 25682
		[SerializeField]
		private GameObject firstTips;

		// Token: 0x04006453 RID: 25683
		[SerializeField]
		private CButton firstTipConfirmButton;

		// Token: 0x04006454 RID: 25684
		[SerializeField]
		private CButton firstTipCancelButton;

		// Token: 0x04006455 RID: 25685
		[SerializeField]
		private ShowLevelsNew showLevelsNew;

		// Token: 0x04006456 RID: 25686
		private static BasicInfoView _view;

		// Token: 0x04006457 RID: 25687
		private LocalStringManager.LanguageType _languageType;

		// Token: 0x04006458 RID: 25688
		private EncyclopediaTipLinkItem _linkItem;

		// Token: 0x04006459 RID: 25689
		private readonly Dictionary<HotKeyCommand, Action> _hotKey2Action = new Dictionary<HotKeyCommand, Action>
		{
			{
				EncyclopediaKit.PrevSearch,
				new Action(ViewEncyclopediaPanel.JumpToPrevSearch)
			},
			{
				EncyclopediaKit.NextSearch,
				new Action(ViewEncyclopediaPanel.JumpToNextSearch)
			}
		};
	}
}
