using System;
using System.Collections.Generic;
using CommonSortAndFilterLegacy;
using CommonSortAndFilterLegacy.Item;
using FrameWork;
using FrameWork.UI.LanguageRule;
using FrameWork.UISystem.Components;
using TMPro;
using UnityEngine;

// Token: 0x020003AF RID: 943
public class UI_TestNewUI : UIBase
{
	// Token: 0x170005CB RID: 1483
	// (get) Token: 0x06003899 RID: 14489 RVA: 0x001C9337 File Offset: 0x001C7537
	private GameObject domainZyh
	{
		get
		{
			return base.CGet<GameObject>("DomainZyh");
		}
	}

	// Token: 0x0600389A RID: 14490 RVA: 0x001C9344 File Offset: 0x001C7544
	public override void OnInit(ArgumentBox argsBox)
	{
		base.CGet<CButtonObsolete>("CommonStripButton_Disbale").interactable = false;
		base.CGet<CButtonObsolete>("CommonButtonCloseDisable").interactable = false;
		base.CGet<CButtonObsolete>("DisableCommonStripInteractButton").interactable = false;
		this.domainZyh.SetActive(false);
		base.CGet<CButtonObsolete>("BtnZyhDomain").ClearAndAddListener(new Action(this.SwitchZyhDomain));
		base.CGet<SwitchButton>("SwitchButton").InitSwitchButton("一段短文本", "一段长长的长长的长长的长长的长长的文本", null);
	}

	// Token: 0x0600389B RID: 14491 RVA: 0x001C93CE File Offset: 0x001C75CE
	public void SwitchPageActiveStatus(GameObject go)
	{
		go.SetActive(!go.activeSelf);
	}

	// Token: 0x0600389C RID: 14492 RVA: 0x001C93E0 File Offset: 0x001C75E0
	private void SwitchZyhDomain()
	{
		GameObject domainZyh = base.CGet<GameObject>("DomainZyh");
		domainZyh.gameObject.SetActive(!domainZyh.activeSelf);
	}

	// Token: 0x0600389D RID: 14493 RVA: 0x001C940F File Offset: 0x001C760F
	private void Awake()
	{
		this.TestSubGroupUI();
		this.TestConfigurablePropGrid();
		this.TestCharacter();
		this.TestHSV();
		this.TestCommonHorizontalLayoutGrid();
		this.TestSortAndFilter();
		this.TestCommonSwitch();
		this.TestHotkeyButton();
	}

	// Token: 0x0600389E RID: 14494 RVA: 0x001C944C File Offset: 0x001C764C
	private void OnEnable()
	{
		InfinityScroll verticalScrollView = base.CGet<InfinityScroll>("VerticalScrollView");
		verticalScrollView.UpdateData(20);
		base.CGet<CommonChooseAmount>("CommonChooseAmount").Init(999999, 1, 1, true);
	}

	// Token: 0x0600389F RID: 14495 RVA: 0x001C9488 File Offset: 0x001C7688
	private void TestCommonHorizontalLayoutGrid()
	{
		InfinityScroll verticalScrollView = base.CGet<InfinityScroll>("VerticalScrollView");
		verticalScrollView.OnItemRender += this.OnItemRender;
	}

	// Token: 0x060038A0 RID: 14496 RVA: 0x001C94B8 File Offset: 0x001C76B8
	private void OnItemRender(int index, GameObject charObj)
	{
		CommonHorizontalLayoutGrid grid = charObj.GetComponent<CommonHorizontalLayoutGrid>();
		grid.SetRowBg(index);
		bool flag = index == 0;
		if (flag)
		{
			grid.SetSpecialBg(1, true);
		}
	}

	// Token: 0x060038A1 RID: 14497 RVA: 0x001C94E8 File Offset: 0x001C76E8
	private void TestHSV()
	{
		bool flag = base.CGet<CToggleObsolete>("CommonToggleDisable").interactable = false;
	}

	// Token: 0x060038A2 RID: 14498 RVA: 0x001C950C File Offset: 0x001C770C
	private void TestCharacter()
	{
		CommonCharacterListItem c = base.CGet<CommonCharacterListItem>("CommonCharacterListItem_Selected");
		c.interactable = false;
	}

	// Token: 0x060038A3 RID: 14499 RVA: 0x001C9530 File Offset: 0x001C7730
	private void TestConfigurablePropGrid()
	{
		CommonConfigurableParameterGrid gridUI = base.CGet<CommonConfigurableParameterGrid>("ConfigurablePropGrid");
		gridUI.OnLineRender = delegate(Refers lineRefers)
		{
			Debug.Log(string.Format("LineRender:{0}", lineRefers.UserInt));
		};
		gridUI.OnCellRender = delegate(Refers cellRefers)
		{
			Debug.Log(string.Format("CellRender:{0}", cellRefers.UserInt));
		};
		ParameterGridLayoutData layoutData = new ParameterGridLayoutData
		{
			LineItems = new ParameterGridLineItem[6]
		};
		layoutData.LineItems[0].Height = 100f;
		layoutData.LineItems[0].GridItems = new ParameterGridGridItem[]
		{
			new ParameterGridGridItem
			{
				Weight = 2f
			},
			new ParameterGridGridItem
			{
				Weight = 1f
			},
			new ParameterGridGridItem
			{
				Weight = 1f
			}
		};
		ParameterGridLineItem commonLineLayout = new ParameterGridLineItem
		{
			Height = 67f,
			GridItems = new ParameterGridGridItem[]
			{
				new ParameterGridGridItem
				{
					Weight = 1f
				},
				new ParameterGridGridItem
				{
					Weight = 1f
				},
				new ParameterGridGridItem
				{
					Weight = 1f
				}
			}
		};
		for (int i = 1; i < layoutData.LineItems.Length; i++)
		{
			layoutData.LineItems[i] = commonLineLayout;
		}
		gridUI.Init(layoutData);
		CommonConfigurableParameterGrid gridUI2 = base.CGet<CommonConfigurableParameterGrid>("ConfigurablePropGrid2");
		gridUI2.OnLineRender = delegate(Refers lineRefers)
		{
			Debug.Log(string.Format("LineRender2:{0}", lineRefers.UserInt));
		};
		gridUI2.OnCellRender = delegate(Refers cellRefers)
		{
			Debug.Log(string.Format("CellRender2:{0}", cellRefers.UserInt));
		};
		gridUI2.Init();
	}

	// Token: 0x060038A4 RID: 14500 RVA: 0x001C973C File Offset: 0x001C793C
	private void TestSubGroupUI()
	{
		ToggleGroupWithSubGroup toggleGroupWithSubGroup = base.CGet<ToggleGroupWithSubGroup>("ToggleGroupWithSubGroup");
		ToggleGroupWithSubGroup toggleGroupWithSubGroup2 = toggleGroupWithSubGroup;
		toggleGroupWithSubGroup2.OnParentToggleRender = (Action<CToggleObsolete>)Delegate.Combine(toggleGroupWithSubGroup2.OnParentToggleRender, new Action<CToggleObsolete>(this.OnParentRender));
		ToggleGroupWithSubGroup toggleGroupWithSubGroup3 = toggleGroupWithSubGroup;
		toggleGroupWithSubGroup3.OnChildToggleRender = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(toggleGroupWithSubGroup3.OnChildToggleRender, new Action<CToggleObsolete, CToggleObsolete>(this.OnChildRender));
		ToggleGroupWithSubGroup toggleGroupWithSubGroup4 = toggleGroupWithSubGroup;
		toggleGroupWithSubGroup4.OnToggleChange = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(toggleGroupWithSubGroup4.OnToggleChange, new Action<CToggleObsolete, CToggleObsolete>(this.OnToggleChange));
		List<int> testData = new List<int>
		{
			2,
			2,
			3,
			4,
			0,
			0
		};
		toggleGroupWithSubGroup.Init(testData, true, -1);
		ToggleGroupWithSubGroup ToggleGroupWithSubGroupManual = base.CGet<ToggleGroupWithSubGroup>("ToggleGroupWithSubGroupManual");
		ToggleGroupWithSubGroup toggleGroupWithSubGroup5 = ToggleGroupWithSubGroupManual;
		toggleGroupWithSubGroup5.OnChildToggleRender = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(toggleGroupWithSubGroup5.OnChildToggleRender, new Action<CToggleObsolete, CToggleObsolete>(this.OnChildRender));
		ToggleGroupWithSubGroup toggleGroupWithSubGroup6 = ToggleGroupWithSubGroupManual;
		toggleGroupWithSubGroup6.OnToggleChange = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(toggleGroupWithSubGroup6.OnToggleChange, new Action<CToggleObsolete, CToggleObsolete>(this.OnToggleChange));
		ToggleGroupWithSubGroupManual.Init(-1);
	}

	// Token: 0x060038A5 RID: 14501 RVA: 0x001C9854 File Offset: 0x001C7A54
	private void OnToggleChange(CToggleObsolete child, CToggleObsolete parent)
	{
		Debug.Log(string.Format("选中了：{0} ~ {1}", (parent != null) ? parent.Key : -1, (child != null) ? child.Key : -1));
	}

	// Token: 0x060038A6 RID: 14502 RVA: 0x001C98A0 File Offset: 0x001C7AA0
	private void OnChildRender(CToggleObsolete child, CToggleObsolete parent)
	{
		string childName = string.Format("子签{0}", child.Key);
		child.SetLabelContent(childName);
	}

	// Token: 0x060038A7 RID: 14503 RVA: 0x001C98CC File Offset: 0x001C7ACC
	private void OnParentRender(CToggleObsolete toggle)
	{
		string childName = string.Format("父签{0}", toggle.Key);
		toggle.SetLabelContent(childName);
	}

	// Token: 0x060038A8 RID: 14504 RVA: 0x001C98F8 File Offset: 0x001C7AF8
	protected override void OnClick(Transform btn)
	{
		base.OnClick(btn);
		bool flag = btn.name.Contains("Close");
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x060038A9 RID: 14505 RVA: 0x001C992C File Offset: 0x001C7B2C
	private void TestSortAndFilter()
	{
		CommonSortAndFilter commonSortAndFilter = base.CGet<CommonSortAndFilter>("CommonSortAndFilter");
		ItemSortAndFilterController sortAndFilterController = new ItemSortAndFilterController(commonSortAndFilter);
		sortAndFilterController.Init(new Action(this.OnItemListChanged), "TestNewUI");
	}

	// Token: 0x060038AA RID: 14506 RVA: 0x001C9968 File Offset: 0x001C7B68
	private void TestCommonSwitch()
	{
		CToggleGroupObsolete toggleGroup = base.CGet<CToggleGroupObsolete>("CommonSwitchToggleGroup");
		toggleGroup.InitPreOnToggle(-1);
	}

	// Token: 0x060038AB RID: 14507 RVA: 0x001C998C File Offset: 0x001C7B8C
	private void TestHotkeyButton()
	{
		List<CommonHotkeyButton> bs = base.CGetList<CommonHotkeyButton>("CommonHotkeyButton_");
		foreach (CommonHotkeyButton b in bs)
		{
			b.UpdateHotKeyByCommand(CombatCommandKit.ClearDefend);
			b.UpdateMainLabelByLanguageKey(LanguageKey.GM_Name);
		}
	}

	// Token: 0x060038AC RID: 14508 RVA: 0x001C99FC File Offset: 0x001C7BFC
	private void OnItemListChanged()
	{
		Debug.Log("道具列表发生了变化");
	}

	// Token: 0x060038AD RID: 14509 RVA: 0x001C9A0C File Offset: 0x001C7C0C
	public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		base.OnLanguageChange(languageType);
		LanguageRuleTips debugRule = base.CGet<LanguageRuleTips>("DebugRule1");
		debugRule.GetComponent<TextMeshProUGUI>().text = ((languageType == LocalStringManager.LanguageType.CN) ? "规则一的中文" : "Rule One English. but very long line to test overflow");
		LanguageRuleChangeSizeOnSwitch debugRule2 = base.CGet<LanguageRuleChangeSizeOnSwitch>("DebugRule2");
		debugRule2.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = ((languageType == LocalStringManager.LanguageType.CN) ? "规则二的中文" : "Rule Two English. but a long long line.");
		LanguageRuleExpandOnHover debugRule3 = base.CGet<LanguageRuleExpandOnHover>("DebugRule5");
		LanguageRuleActiveOne debugRule4 = base.CGet<LanguageRuleActiveOne>("DebugRule46");
		TextMeshProUGUI debugRule3Label = base.CGet<TextMeshProUGUI>("DebugRule3Label");
		debugRule3Label.text = ((languageType == LocalStringManager.LanguageType.CN) ? "规则三的中文" : "Rule Three English\n with a line break");
		debugRule.OnLanguageChange(languageType);
		debugRule2.OnLanguageChange(languageType);
		debugRule3.OnLanguageChange(languageType);
		debugRule4.OnLanguageChange(languageType);
		List<CommonHotkeyButton> bs = base.CGetList<CommonHotkeyButton>("CommonHotkeyButton_");
		foreach (CommonHotkeyButton b in bs)
		{
			b.OnLanguageChange(languageType);
			CommonHotkeyButton commonHotkeyButton = b;
			if (!true)
			{
			}
			string labelString;
			if (languageType != LocalStringManager.LanguageType.CN)
			{
				if (languageType != LocalStringManager.LanguageType.EN)
				{
					labelString = "Test Button";
				}
				else
				{
					labelString = "English";
				}
			}
			else
			{
				labelString = "中文";
			}
			if (!true)
			{
			}
			commonHotkeyButton.UpdateMainLabelRaw(labelString);
		}
	}
}
