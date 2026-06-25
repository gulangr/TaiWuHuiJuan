using System;
using Config;
using FrameWork;
using FrameWork.CommandSystem;
using Game.CommandSystem;
using Game.Views;
using TMPro;
using UnityEngine;

// Token: 0x020003B4 RID: 948
public class UI_TutorialChaptersMenu : UIBase
{
	// Token: 0x060038F0 RID: 14576 RVA: 0x001CC3E4 File Offset: 0x001CA5E4
	public override void OnInit(ArgumentBox argsBox)
	{
		this._settingData = SingletonObject.getInstance<GlobalSettings>();
		this.NeedDataListenerId = true;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.Element.ShowAfterRefresh));
	}

	// Token: 0x060038F1 RID: 14577 RVA: 0x001CC434 File Offset: 0x001CA634
	private void Awake()
	{
		base.CGet<GameObject>("ChapterTemplate").SetActive(false);
		CToggleObsolete skipTutorialToggle = base.CGet<CToggleObsolete>("SkipTutorialToggle");
		skipTutorialToggle.onValueChanged.RemoveAllListeners();
		skipTutorialToggle.isOn = this._settingData.SkipTutorialChapters;
		skipTutorialToggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			this._settingData.SkipTutorialChapters = isOn;
			this.RefreshSkipTip();
		});
	}

	// Token: 0x060038F2 RID: 14578 RVA: 0x001CC498 File Offset: 0x001CA698
	private void OnEnable()
	{
		CToggleObsolete skipTutorialToggle = base.CGet<CToggleObsolete>("SkipTutorialToggle");
		skipTutorialToggle.isOn = this._settingData.SkipTutorialChapters;
		this.UpdateChaptersListView();
		this.RefreshSkipTip();
	}

	// Token: 0x060038F3 RID: 14579 RVA: 0x001CC4D4 File Offset: 0x001CA6D4
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "Close";
		if (flag)
		{
			base.QuickHide();
		}
	}

	// Token: 0x060038F4 RID: 14580 RVA: 0x001CC501 File Offset: 0x001CA701
	public override void QuickHide()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		base.QuickHide();
	}

	// Token: 0x060038F5 RID: 14581 RVA: 0x001CC51D File Offset: 0x001CA71D
	private void OnDisable()
	{
		this._settingData.SaveSettings();
		ViewMainMenu viewMainMenu = (ViewMainMenu)UIElement.MainMenu.UiBase;
		if (viewMainMenu != null)
		{
			viewMainMenu.UpdateRoleListButtonInteractable();
		}
	}

	// Token: 0x060038F6 RID: 14582 RVA: 0x001CC548 File Offset: 0x001CA748
	private void UpdateChaptersListView()
	{
		float height = -15f;
		RectTransform content = base.CGet<CScrollRectLegacy>("ScrollView").Content;
		Refers[] chapterRefersArray = content.GetComponentsInTopChildren(false);
		GameObject prefab = base.CGet<GameObject>("ChapterTemplate");
		for (int i = 0; i < TutorialChapters.Instance.Count; i++)
		{
			bool flag = i == 8;
			if (flag)
			{
				RectTransform splitLineTrans = base.CGet<RectTransform>("SplitLine");
				splitLineTrans.anchoredPosition = Vector2.up * height;
				height -= splitLineTrans.rect.height + 15f;
			}
			bool flag2 = chapterRefersArray.CheckIndex(i);
			Refers refers;
			if (flag2)
			{
				refers = chapterRefersArray[i];
			}
			else
			{
				GameObject refersObj = Object.Instantiate<GameObject>(prefab, content, false);
				refersObj.transform.localScale = Vector3.one;
				refers = refersObj.GetComponent<Refers>();
			}
			RectTransform refersTrans = refers.GetComponent<RectTransform>();
			refersTrans.anchoredPosition = Vector2.up * height;
			height -= refersTrans.rect.height + 15f;
			refers.gameObject.SetActive(true);
			this.RefreshChapter(i, refers);
		}
		content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -height + 15f);
		for (int j = TutorialChapters.Instance.Count; j < chapterRefersArray.Length; j++)
		{
			chapterRefersArray[j].gameObject.SetActive(false);
		}
	}

	// Token: 0x060038F7 RID: 14583 RVA: 0x001CC6C4 File Offset: 0x001CA8C4
	private void RefreshChapter(int index, Refers chapterRefers)
	{
		TutorialChaptersItem item = TutorialChapters.Instance.GetItem((short)index);
		CButtonObsolete button = chapterRefers.CGet<CButtonObsolete>("Button");
		TextMeshProUGUI nameText = chapterRefers.CGet<TextMeshProUGUI>("Name");
		TextMeshProUGUI descText = chapterRefers.CGet<TextMeshProUGUI>("Description");
		TextMeshProUGUI headText = chapterRefers.CGet<TextMeshProUGUI>("Head");
		TextMeshProUGUI tailText = chapterRefers.CGet<TextMeshProUGUI>("Tail");
		CImage headImg = chapterRefers.CGet<GameObject>("TutorialIndicator").GetComponent<CImage>();
		CImage tailImg = chapterRefers.CGet<GameObject>("CompletionIndicator").GetComponent<CImage>();
		bool visible = index <= this._settingData.CompletedChapters;
		bool flag = index >= 8;
		if (flag)
		{
			visible = (visible && SingletonObject.getInstance<DisplayTriggerModel>().IsReachGameEnd());
		}
		button.interactable = visible;
		bool flag2 = visible;
		if (flag2)
		{
			descText.gameObject.SetActive(true);
			nameText.SetText(item.Name, true);
			descText.SetText(item.Desc, true);
			headText.SetText(item.Head, true);
			tailText.SetText(item.Tail, true);
			headImg.SetSprite((index < this._settingData.CompletedChapters) ? "mainmenu_anniu_7_2" : "mainmenu_anniu_7_0", false, null);
			tailImg.SetSprite((index < this._settingData.CompletedChapters) ? "mainmenu_anniu_7_2" : "mainmenu_anniu_7_0", false, null);
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(delegate()
			{
				button.interactable = false;
				CommandManager.AddCommand(new CommandEnterTutorial
				{
					TutorialChapter = item
				}, EPriority.EnterLoading);
			});
			button.interactable = true;
		}
		else
		{
			nameText.text = "? ? ?";
			descText.gameObject.SetActive(false);
		}
		chapterRefers.CGet<GameObject>("TutorialIndicator").SetActive(visible);
		chapterRefers.CGet<GameObject>("CompletionIndicator").SetActive(index < this._settingData.CompletedChapters);
		MonoJoint monoJoint = button.transform.Find("Hover").GetComponent<MonoJoint>();
		bool flag3 = button.interactable && index < this._settingData.CompletedChapters;
		if (flag3)
		{
			foreach (MonoJoint.ControlInfo controlInfo in monoJoint.ControlList)
			{
				controlInfo.Target.gameObject.SetActive(false);
			}
			monoJoint.enabled = false;
		}
		else
		{
			monoJoint.enabled = true;
		}
	}

	// Token: 0x060038F8 RID: 14584 RVA: 0x001CC980 File Offset: 0x001CAB80
	private void RefreshSkipTip()
	{
		bool showSkipTip = !this._settingData.SkipTutorialChapters && this._settingData.CompletedChapters <= TutorialChapters.Instance.Count - 1;
		base.CGet<GameObject>("SkipTip").SetActive(showSkipTip);
	}

	// Token: 0x04002937 RID: 10551
	private GlobalSettings _settingData;

	// Token: 0x04002938 RID: 10552
	private const float ElementGap = 15f;
}
