using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000211 RID: 529
public class GMSearchHelper : Refers
{
	// Token: 0x1700035E RID: 862
	// (get) Token: 0x060021A7 RID: 8615 RVA: 0x000F6362 File Offset: 0x000F4562
	private Transform CharacterEditorLineParent
	{
		get
		{
			return this._characterEditor.List;
		}
	}

	// Token: 0x1700035F RID: 863
	// (get) Token: 0x060021A8 RID: 8616 RVA: 0x000F636F File Offset: 0x000F456F
	private bool IsInCharacterEditor
	{
		get
		{
			return this._characterEditor.gameObject.activeSelf;
		}
	}

	// Token: 0x060021A9 RID: 8617 RVA: 0x000F6384 File Offset: 0x000F4584
	public void Init(CToggleGroupObsolete pageToggleRoot, GMCharacterEditor characterEditor)
	{
		this.InitRefers();
		this._pageToggleRoot = pageToggleRoot;
		this._characterEditor = characterEditor;
		this._searchButton.ClearAndAddListener(new Action(this.OnSearchButtonClick));
		this._cancelButton.ClearAndAddListener(new Action(this.OnCancelButtonClick));
		this._inputFieldEvent.onValueChanged.AddListener(delegate(string text)
		{
			this.OnSearchButtonClick();
		});
	}

	// Token: 0x060021AA RID: 8618 RVA: 0x000F63F4 File Offset: 0x000F45F4
	private void Update()
	{
		bool isInCharacterEditor = this.IsInCharacterEditor;
		bool flag = isInCharacterEditor != this._lastIsInCharacterEditor;
		if (flag)
		{
			this.OnSearchButtonClick();
		}
		this._lastIsInCharacterEditor = isInCharacterEditor;
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x000F6429 File Offset: 0x000F4629
	private void OnCancelButtonClick()
	{
		this._inputField.SetTextWithoutNotify("");
		this.OnSearchButtonClick();
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x000F6444 File Offset: 0x000F4644
	private void OnSearchButtonClick()
	{
		bool flag = this._inputField.text.IsNullOrEmpty();
		if (flag)
		{
			bool isInCharacterEditor = this.IsInCharacterEditor;
			if (isInCharacterEditor)
			{
				this.ResetCharacterEditorSearch();
			}
			else
			{
				this.ResetSearch();
				this.RefreshGroups();
			}
		}
		else
		{
			bool isInCharacterEditor2 = this.IsInCharacterEditor;
			if (isInCharacterEditor2)
			{
				this.SearchCharacterEditorGm(this._inputField.text);
			}
			else
			{
				this.SearchGm(this._inputField.text);
				this.RefreshGroups();
				this.SelectFirstVisiblePageToggle();
			}
		}
	}

	// Token: 0x060021AD RID: 8621 RVA: 0x000F64D0 File Offset: 0x000F46D0
	public void RegisterPageToggle(int id, GameObject toggle, string searchText)
	{
		this._searchPageToggles.Add(new GMSearchHelper.SearchPageToggle
		{
			Id = id,
			Object = toggle,
			SearchText = searchText.ToLower()
		});
	}

	// Token: 0x060021AE RID: 8622 RVA: 0x000F6510 File Offset: 0x000F4710
	public void RegisterLine(int pageId, GameObject line, string searchText)
	{
		this._searchLines.Add(new GMSearchHelper.SearchLine
		{
			PageId = pageId,
			Object = line,
			SearchText = searchText.ToLower()
		});
	}

	// Token: 0x060021AF RID: 8623 RVA: 0x000F6550 File Offset: 0x000F4750
	public void RegisterCharacterEditorLine(int pageId, GameObject line, string searchText)
	{
		this._characterEditorSearchLines.Add(new GMSearchHelper.SearchLine
		{
			PageId = pageId,
			Object = line,
			SearchText = searchText.ToLower()
		});
	}

	// Token: 0x060021B0 RID: 8624 RVA: 0x000F6590 File Offset: 0x000F4790
	public void RegisterGroup(Transform group)
	{
		this._groups.Add(group);
	}

	// Token: 0x060021B1 RID: 8625 RVA: 0x000F65A0 File Offset: 0x000F47A0
	private void SearchGm(string text)
	{
		foreach (GMSearchHelper.SearchPageToggle toggle in this._searchPageToggles)
		{
			toggle.Object.SetActive(false);
		}
		foreach (GMSearchHelper.SearchLine line in this._searchLines)
		{
			bool flag = line.SearchText.Contains(text.ToLower());
			if (flag)
			{
				line.Object.SetActive(true);
				foreach (GMSearchHelper.SearchPageToggle toggle2 in this._searchPageToggles)
				{
					bool flag2 = toggle2.Id == line.PageId;
					if (flag2)
					{
						toggle2.Object.SetActive(true);
						break;
					}
				}
			}
			else
			{
				line.Object.SetActive(false);
			}
		}
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x000F66E4 File Offset: 0x000F48E4
	private void SearchCharacterEditorGm(string text)
	{
		foreach (GMSearchHelper.SearchLine line in this._characterEditorSearchLines)
		{
			line.Object.SetActive(line.SearchText.Contains(text.ToLower()));
		}
		this._characterEditor.InfoDisplay.gameObject.SetActive(false);
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x000F676C File Offset: 0x000F496C
	private void ResetSearch()
	{
		foreach (GMSearchHelper.SearchPageToggle toggle in this._searchPageToggles)
		{
			toggle.Object.SetActive(true);
		}
		foreach (GMSearchHelper.SearchLine line in this._searchLines)
		{
			line.Object.SetActive(true);
		}
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x000F6818 File Offset: 0x000F4A18
	private void ResetCharacterEditorSearch()
	{
		foreach (GMSearchHelper.SearchLine line in this._characterEditorSearchLines)
		{
			line.Object.SetActive(true);
		}
		this._characterEditor.InfoDisplay.gameObject.SetActive(true);
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x000F6890 File Offset: 0x000F4A90
	private void RefreshGroups()
	{
		foreach (Transform group in this._groups)
		{
			bool hasVisibleChild = false;
			foreach (object obj in group)
			{
				Transform child = (Transform)obj;
				bool activeSelf = child.gameObject.activeSelf;
				if (activeSelf)
				{
					hasVisibleChild = true;
					break;
				}
			}
			group.gameObject.SetActive(hasVisibleChild);
		}
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x000F694C File Offset: 0x000F4B4C
	private void SelectFirstVisiblePageToggle()
	{
		bool activeSelf = this._pageToggleRoot.GetActive().gameObject.activeSelf;
		if (!activeSelf)
		{
			foreach (GMSearchHelper.SearchPageToggle toggle in this._searchPageToggles)
			{
				bool activeSelf2 = toggle.Object.activeSelf;
				if (activeSelf2)
				{
					this._pageToggleRoot.Set(toggle.Object.GetComponent<CToggleObsolete>().Key, true, false);
					break;
				}
			}
		}
	}

	// Token: 0x060021B7 RID: 8631 RVA: 0x000F69E8 File Offset: 0x000F4BE8
	private void InitRefers()
	{
		this._inputFieldEvent = base.CGet<DelayedInputFieldEvent>("InputField");
		this._searchButton = base.CGet<CButtonObsolete>("SearchButton");
		this._cancelButton = base.CGet<CButtonObsolete>("CancelButton");
		this._inputField = this._inputFieldEvent.GetComponent<TMP_InputField>();
	}

	// Token: 0x04001A08 RID: 6664
	private readonly List<GMSearchHelper.SearchPageToggle> _searchPageToggles = new List<GMSearchHelper.SearchPageToggle>();

	// Token: 0x04001A09 RID: 6665
	private readonly List<GMSearchHelper.SearchLine> _searchLines = new List<GMSearchHelper.SearchLine>();

	// Token: 0x04001A0A RID: 6666
	private readonly List<GMSearchHelper.SearchLine> _characterEditorSearchLines = new List<GMSearchHelper.SearchLine>();

	// Token: 0x04001A0B RID: 6667
	private readonly List<Transform> _groups = new List<Transform>();

	// Token: 0x04001A0C RID: 6668
	private CToggleGroupObsolete _pageToggleRoot;

	// Token: 0x04001A0D RID: 6669
	private GMCharacterEditor _characterEditor;

	// Token: 0x04001A0E RID: 6670
	private bool _lastIsInCharacterEditor = false;

	// Token: 0x04001A0F RID: 6671
	private DelayedInputFieldEvent _inputFieldEvent;

	// Token: 0x04001A10 RID: 6672
	private CButtonObsolete _searchButton;

	// Token: 0x04001A11 RID: 6673
	private CButtonObsolete _cancelButton;

	// Token: 0x04001A12 RID: 6674
	private TMP_InputField _inputField;

	// Token: 0x020014A4 RID: 5284
	private interface ISearchableItem
	{
	}

	// Token: 0x020014A5 RID: 5285
	private struct SearchPageToggle
	{
		// Token: 0x0400A1E8 RID: 41448
		public int Id;

		// Token: 0x0400A1E9 RID: 41449
		public GameObject Object;

		// Token: 0x0400A1EA RID: 41450
		public string SearchText;
	}

	// Token: 0x020014A6 RID: 5286
	private struct SearchLine
	{
		// Token: 0x0400A1EB RID: 41451
		public int PageId;

		// Token: 0x0400A1EC RID: 41452
		public GameObject Object;

		// Token: 0x0400A1ED RID: 41453
		public string SearchText;
	}
}
