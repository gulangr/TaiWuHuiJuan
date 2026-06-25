using System;
using System.Collections.Generic;
using DisplayConfig;
using FrameWork;
using Game.Views.VillagerRoleView;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003A1 RID: 929
public class UI_SelectChicken : UIBase
{
	// Token: 0x170005BF RID: 1471
	// (get) Token: 0x060037E0 RID: 14304 RVA: 0x001C1A5D File Offset: 0x001BFC5D
	private bool IsCountMax
	{
		get
		{
			HashSet<int> selectedChickenIds = this._selectedChickenIds;
			return ((selectedChickenIds != null) ? selectedChickenIds.Count : 0) >= this._selectCount;
		}
	}

	// Token: 0x060037E1 RID: 14305 RVA: 0x001C1A7C File Offset: 0x001BFC7C
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get<Action<HashSet<int>>>("Callback", out this._onSelected);
		argsBox.Get<Action<HashSet<int>>>("PreviewCallback", out this._onPreviewSelected);
		argsBox.Get<Action>("CancelCallback", out this._onCancel);
		argsBox.Get<List<Chicken>>("ChickenList", out this._chickenList);
		List<int> selectedChickenIdList;
		argsBox.Get<List<int>>("SelectedChickenIdList", out selectedChickenIdList);
		bool flag = selectedChickenIdList != null;
		if (flag)
		{
			this._selectedChickenIds = new HashSet<int>(selectedChickenIdList);
		}
		else
		{
			this._selectedChickenIds = new HashSet<int>();
		}
		argsBox.Get("MaxSelectCount", out this._selectCount);
		argsBox.Get<List<sbyte>>("PersonalitySortTypes", out this._personalitySortTypes);
		argsBox.Get<UI_SelectChicken.ComparisonMaker>("ComparisonMaker", out this._comparisonMaker);
		argsBox.Get<Dictionary<int, short>>("ChickenIdToRoleDict", out this._chickenIdToRoleDict);
		bool flag2 = !this._inited;
		if (flag2)
		{
			this.InitRefers();
			this._chickenScroll.OnItemRender = new Action<int, Refers>(this.OnItemRender);
		}
		this.RefreshCountLabel();
		this.InitPersonalitySortItems();
		this.SortChickens();
		this.RefreshChickenScroll();
		this.Element.ShowAfterRefresh();
		this._inited = true;
	}

	// Token: 0x060037E2 RID: 14306 RVA: 0x001C1BA8 File Offset: 0x001BFDA8
	private void RefreshCountLabel()
	{
		int selectedCount = this._selectedChickenIds.Count;
		int maxCount = this._selectCount;
		string selectedCountText = selectedCount.ToString().SetColor((selectedCount >= maxCount) ? "brightred" : "pinkyellow");
		this._selectedChickenLabel.text = string.Format("{0}/{1}", selectedCountText, maxCount);
	}

	// Token: 0x060037E3 RID: 14307 RVA: 0x001C1C04 File Offset: 0x001BFE04
	private void InitPersonalitySortItems()
	{
		CButtonObsolete template = this._personalitySortItemTemplate;
		template.gameObject.SetActive(false);
		int childCount = this._personalitySortItemTemplate.transform.parent.childCount;
		for (int i = 0; i < this._personalitySortTypes.Count; i++)
		{
			bool flag = i < childCount - 1;
			CButtonObsolete item;
			if (flag)
			{
				item = this._personalitySortItemTemplate.transform.parent.GetChild(i + 1).GetComponent<CButtonObsolete>();
			}
			else
			{
				item = Object.Instantiate<CButtonObsolete>(template, template.transform.parent);
			}
			item.gameObject.SetActive(true);
			item.name = string.Format("PersonalitySortItem_{0}", i + 1);
			Refers refers = item.GetComponent<Refers>();
			CImage checkMark = refers.CGet<CImage>("CheckMark");
			CImage icon = refers.CGet<CImage>("Icon");
			Toggle toggle = refers.CGet<Toggle>("Toggle");
			PersonalityItem personalityConfig = Personality.Instance[(int)this._personalitySortTypes[i]];
			icon.SetSprite(personalityConfig.Icon, false, null);
			int ii = i;
			toggle.onValueChanged.RemoveAllListeners();
			toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				checkMark.gameObject.SetActive(isOn);
			});
			item.ClearAndAddListener(delegate
			{
				toggle.isOn = true;
				this._selectedPersonalitySortIndex = ii;
				this.SortChickens();
				this.RefreshChickenScroll();
			});
			toggle.isOn = (i == 0);
			bool flag2 = i == 0;
			if (flag2)
			{
				item.onClick.Invoke();
			}
		}
	}

	// Token: 0x060037E4 RID: 14308 RVA: 0x001C1DAF File Offset: 0x001BFFAF
	private void RefreshChickenScroll()
	{
		this._chickenScroll.SetDataCount(this._chickenList.Count);
	}

	// Token: 0x060037E5 RID: 14309 RVA: 0x001C1DCC File Offset: 0x001BFFCC
	private void OnItemRender(int index, Refers outRefers)
	{
		SelectableChickenItem chickenItem = outRefers.CGet<SelectableChickenItem>("SelectableChickenItem");
		bool isSelected = this._selectedChickenIds.Contains(this._chickenList[index].Id);
		chickenItem.SetDisable(!isSelected && this.IsCountMax);
		chickenItem.Refresh(index, this._chickenList[index], null, isSelected, false, new Action<int>(this.OnClickChickenItem), null, null, false);
		short roleId;
		bool flag = this._chickenIdToRoleDict.TryGetValue(this._chickenList[index].Id, out roleId);
		if (flag)
		{
			chickenItem.RefreshRoleIcon(roleId);
			chickenItem.SetRoleIconVisible(true);
		}
		else
		{
			chickenItem.SetRoleIconVisible(false);
		}
	}

	// Token: 0x060037E6 RID: 14310 RVA: 0x001C1E7C File Offset: 0x001C007C
	private void OnClickChickenItem(int index)
	{
		int chickenId = this._chickenList[index].Id;
		bool flag = this._selectedChickenIds.Contains(chickenId);
		if (flag)
		{
			this._modified = true;
			this._selectedChickenIds.Remove(chickenId);
			Action<HashSet<int>> onPreviewSelected = this._onPreviewSelected;
			if (onPreviewSelected != null)
			{
				onPreviewSelected(this._selectedChickenIds);
			}
		}
		else
		{
			bool flag2 = this._selectedChickenIds.Count >= this._selectCount;
			if (flag2)
			{
				return;
			}
			this._modified = true;
			this._selectedChickenIds.Add(chickenId);
			Action<HashSet<int>> onPreviewSelected2 = this._onPreviewSelected;
			if (onPreviewSelected2 != null)
			{
				onPreviewSelected2(this._selectedChickenIds);
			}
		}
		this.RefreshCountLabel();
		this._chickenScroll.ReRender();
	}

	// Token: 0x060037E7 RID: 14311 RVA: 0x001C1F3C File Offset: 0x001C013C
	private void SortChickens()
	{
		UI_SelectChicken.ComparisonMaker comparisonMaker = this._comparisonMaker;
		Comparison<Chicken> comparison = (comparisonMaker != null) ? comparisonMaker(this._personalitySortTypes[this._selectedPersonalitySortIndex], this._selectedChickenIds) : null;
		bool flag = comparison != null;
		if (flag)
		{
			this._chickenList.Sort(comparison);
		}
	}

	// Token: 0x060037E8 RID: 14312 RVA: 0x001C1F8C File Offset: 0x001C018C
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "Confirm";
		if (flag)
		{
			Action<HashSet<int>> onSelected = this._onSelected;
			if (onSelected != null)
			{
				onSelected(this._selectedChickenIds);
			}
			this._modified = false;
			base.QuickHide();
		}
		else
		{
			bool flag2 = btn.name == "Cancel";
			if (flag2)
			{
				this.QuickHide();
			}
		}
	}

	// Token: 0x060037E9 RID: 14313 RVA: 0x001C1FF8 File Offset: 0x001C01F8
	public override void QuickHide()
	{
		bool modified = this._modified;
		if (modified)
		{
			DialogCmd dialogCmd = new DialogCmd
			{
				Type = 1,
				Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
				Content = LocalStringManager.Get(LanguageKey.LK_AssignChicken_Cancel_Alert).ColorReplace(),
				Yes = new Action(this.CancelQuit)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
		else
		{
			this.CancelQuit();
		}
	}

	// Token: 0x060037EA RID: 14314 RVA: 0x001C208C File Offset: 0x001C028C
	private void CancelQuit()
	{
		Action onCancel = this._onCancel;
		if (onCancel != null)
		{
			onCancel();
		}
		base.QuickHide();
	}

	// Token: 0x060037EB RID: 14315 RVA: 0x001C20A8 File Offset: 0x001C02A8
	private void InitRefers()
	{
		this._chickenScroll = base.CGet<InfinityScrollLegacy>("ChickenScroll");
		this._confirm = base.CGet<CButtonObsolete>("Confirm");
		this._cancel = base.CGet<CButtonObsolete>("Cancel");
		this._personalitySortItemTemplate = base.CGet<CButtonObsolete>("PersonalitySortItemTemplate");
		this._selectedChickenLabel = base.CGet<TextMeshProUGUI>("SelectedChickenLabel");
	}

	// Token: 0x04002868 RID: 10344
	private Action<HashSet<int>> _onSelected;

	// Token: 0x04002869 RID: 10345
	private Action<HashSet<int>> _onPreviewSelected;

	// Token: 0x0400286A RID: 10346
	private Action _onCancel;

	// Token: 0x0400286B RID: 10347
	private UI_SelectChicken.ComparisonMaker _comparisonMaker;

	// Token: 0x0400286C RID: 10348
	private List<Chicken> _chickenList;

	// Token: 0x0400286D RID: 10349
	private int _selectCount;

	// Token: 0x0400286E RID: 10350
	private HashSet<int> _selectedChickenIds;

	// Token: 0x0400286F RID: 10351
	private List<sbyte> _personalitySortTypes = new List<sbyte>();

	// Token: 0x04002870 RID: 10352
	private int _selectedPersonalitySortIndex = 0;

	// Token: 0x04002871 RID: 10353
	private bool _modified = false;

	// Token: 0x04002872 RID: 10354
	private Dictionary<int, short> _chickenIdToRoleDict;

	// Token: 0x04002873 RID: 10355
	private bool _inited;

	// Token: 0x04002874 RID: 10356
	private InfinityScrollLegacy _chickenScroll;

	// Token: 0x04002875 RID: 10357
	private CButtonObsolete _confirm;

	// Token: 0x04002876 RID: 10358
	private CButtonObsolete _cancel;

	// Token: 0x04002877 RID: 10359
	private CButtonObsolete _personalitySortItemTemplate;

	// Token: 0x04002878 RID: 10360
	private TextMeshProUGUI _selectedChickenLabel;

	// Token: 0x020017F2 RID: 6130
	// (Invoke) Token: 0x0600D57D RID: 54653
	public delegate Comparison<Chicken> ComparisonMaker(sbyte selectedPersonalityType, HashSet<int> selectedChickenIds);
}
