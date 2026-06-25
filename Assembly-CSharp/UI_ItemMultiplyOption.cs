using System;
using System.Collections.Generic;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000388 RID: 904
public class UI_ItemMultiplyOption : UIBase
{
	// Token: 0x170005A5 RID: 1445
	// (get) Token: 0x060035BA RID: 13754 RVA: 0x001B0100 File Offset: 0x001AE300
	private static List<string> GradeNameList
	{
		get
		{
			bool flag = UI_ItemMultiplyOption._gradeNameList.Count == 0;
			if (flag)
			{
				for (int i = 9; i > 0; i--)
				{
					string gradeName = CommonUtils.GetItemGradeShortNameWithMoreThan(i);
					UI_ItemMultiplyOption._gradeNameList.Add(gradeName);
				}
			}
			return UI_ItemMultiplyOption._gradeNameList;
		}
	}

	// Token: 0x060035BB RID: 13755 RVA: 0x001B0154 File Offset: 0x001AE354
	public override void OnInit(ArgumentBox argsBox)
	{
		RectTransform anchorItem;
		bool flag = argsBox.Get<RectTransform>("AnchorItem", out anchorItem);
		if (flag)
		{
			this._anchorItem = anchorItem;
			this._anchorOriginParent = anchorItem.parent;
			anchorItem.SetParent(base.transform);
		}
		RectTransform root = base.CGet<RectTransform>("Root");
		Vector3 pos;
		bool flag2 = argsBox.Get<Vector3>("Pos", out pos);
		if (flag2)
		{
			root.position = pos;
		}
		this._dropDown = base.CGet<CDropdownLegacy>("Dropdown");
		this._dropDown.onValueChanged.RemoveAllListeners();
		this._dropDown.ClearOptions();
		this._dropDown.AddOptions(UI_ItemMultiplyOption.GradeNameList);
		Enum type;
		argsBox.Get("Type", out type);
		ItemGradeFilterSetting.ItemGradeFilterSourceType itemGradeFilterSourceType = (ItemGradeFilterSetting.ItemGradeFilterSourceType)type;
		this._setting = SingletonObject.getInstance<GameSort>().GetItemGradeFilterSetting();
		sbyte grade = this._setting.GetGrade(itemGradeFilterSourceType);
		this.SetCurGrade(grade);
		this._dropDown.value = this._setting.GetIndex(itemGradeFilterSourceType);
		this._dropDown.onValueChanged.AddListener(delegate(int index)
		{
			sbyte curGrade = this._setting.GetGradeByIndex(index);
			this.SetCurGrade(curGrade);
			this._setting.SetGrade(itemGradeFilterSourceType, curGrade);
			SingletonObject.getInstance<GameSort>().SetItemGradeFilterSetting(this._setting);
			UIManager.Instance.HideUI(UIElement.ItemMultiplyOptionOld);
			GEvent.OnEvent(UiEvents.ItemGradeFilterSettingChange, null);
		});
		Action _onSetGrade;
		bool flag3 = argsBox.Get<Action>("OnSetGrade", out _onSetGrade);
		if (flag3)
		{
			UIElement element = this.Element;
			element.OnHide = (Action)Delegate.Combine(element.OnHide, _onSetGrade);
		}
		TooltipInvoker tip = base.CGet<TooltipInvoker>("TextEmptyImage");
		bool isGearMate;
		bool flag4 = argsBox.Get("IsGearMate", out isGearMate);
		if (flag4)
		{
			tip.PresetParam[1] = LocalStringManager.Get(LanguageKey.LK_GearMate_ItemMultiplyOption_Tip);
		}
		else
		{
			tip.PresetParam[1] = LocalStringManager.Get(LanguageKey.LK_ItemMultipyOption_Tip);
		}
		UIElement element2 = this.Element;
		element2.OnHide = (Action)Delegate.Combine(element2.OnHide, new Action(delegate()
		{
			bool flag5 = this._anchorItem;
			if (flag5)
			{
				this._anchorItem.SetParent(this._anchorOriginParent);
			}
		}));
	}

	// Token: 0x060035BC RID: 13756 RVA: 0x001B032C File Offset: 0x001AE52C
	private void SetCurGrade(sbyte grade)
	{
		CImage gradeImage = base.CGet<CImage>("GradeBack");
		bool flag = grade > 0;
		if (flag)
		{
			gradeImage.gameObject.SetActive(true);
			gradeImage.SetSprite(ItemView.GetGradeIcon(grade), false, null);
			TextMeshProUGUI componentInChildren = gradeImage.GetComponentInChildren<TextMeshProUGUI>();
			if (componentInChildren != null)
			{
				componentInChildren.SetText(ItemView.GetGradeText(grade), true);
			}
		}
		else
		{
			gradeImage.gameObject.SetActive(false);
		}
	}

	// Token: 0x060035BD RID: 13757 RVA: 0x001B0398 File Offset: 0x001AE598
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "UIMask";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x060035BE RID: 13758 RVA: 0x001B03C4 File Offset: 0x001AE5C4
	private void OnGUI()
	{
		bool flag = this._dropDown && this._dropDown.IsExpanded && this._setting != null;
		if (flag)
		{
			Transform trans = this._dropDown.transform.Find("Dropdown List");
			bool flag2 = !trans;
			if (!flag2)
			{
				CToggleObsolete[] toggles = this._dropDown.GetComponentsInChildren<CToggleObsolete>();
				PositionFollower positionFollower = this._dropDown.GetComponentInChildren<PositionFollower>();
				foreach (CToggleObsolete togCell in toggles)
				{
					bool flag3 = !togCell.gameObject.activeSelf;
					if (!flag3)
					{
						togCell.transform.Find("Disable").gameObject.SetActive(togCell.isOn);
						bool flag4 = togCell.isOn && positionFollower;
						if (flag4)
						{
							positionFollower.Target = togCell.transform;
						}
					}
				}
				RectTransform content = trans.GetComponentInChildren<CScrollRectLegacy>().Content;
				int childCount = content.childCount;
				for (int i = 1; i < childCount; i++)
				{
					Transform item = content.GetChild(i);
					Transform gradeBack = item.Find("Layout/GradeBack");
					bool flag5 = i == 1;
					if (flag5)
					{
						bool activeSelf = gradeBack.gameObject.activeSelf;
						if (activeSelf)
						{
							gradeBack.gameObject.SetActive(false);
						}
					}
					else
					{
						bool flag6 = !gradeBack.gameObject.activeSelf;
						if (flag6)
						{
							gradeBack.gameObject.SetActive(true);
						}
						sbyte grade = this._setting.GetGradeByIndex(i - 1);
						CImage component = gradeBack.GetComponent<CImage>();
						if (component != null)
						{
							component.SetSprite(ItemView.GetGradeIcon(grade), false, null);
						}
						TextMeshProUGUI componentInChildren = gradeBack.GetComponentInChildren<TextMeshProUGUI>();
						if (componentInChildren != null)
						{
							componentInChildren.SetText(ItemView.GetGradeText(grade), true);
						}
					}
				}
			}
		}
	}

	// Token: 0x040026FB RID: 9979
	private static readonly List<string> _gradeNameList = new List<string>(9);

	// Token: 0x040026FC RID: 9980
	private CDropdownLegacy _dropDown;

	// Token: 0x040026FD RID: 9981
	private ItemGradeFilterSetting _setting;

	// Token: 0x040026FE RID: 9982
	private Transform _anchorOriginParent;

	// Token: 0x040026FF RID: 9983
	private Transform _anchorItem;
}
