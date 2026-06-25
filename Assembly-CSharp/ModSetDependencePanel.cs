using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameWork;
using FrameWork.ModSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using GameData.Domains.Mod;
using TMPro;
using UnityEngine;

// Token: 0x0200025F RID: 607
public class ModSetDependencePanel : MonoBehaviour
{
	// Token: 0x060027D6 RID: 10198 RVA: 0x00125A88 File Offset: 0x00123C88
	public void Init()
	{
		this.searchInputField.onValueChanged.RemoveAllListeners();
		this.searchInputField.onValueChanged.AddListener(delegate(string value)
		{
			this.RefreshSearchedList();
		});
		this.searchInputField.SetTextWithoutNotify(string.Empty);
		this.searchedScroll.OnItemRender += this.OnSearchedItemRender;
		this.selectedScroll.OnItemRender += this.OnSelectedItemRender;
		this.btnYes.ClearAndAddListener(new Action(this.OnClickConfirm));
		this.btnNo.ClearAndAddListener(new Action(this.OnClickCancel));
	}

	// Token: 0x060027D7 RID: 10199 RVA: 0x00125B38 File Offset: 0x00123D38
	private void OnSearchedItemRender(int index, GameObject go)
	{
		ModDependenceSettingItem refers = go.GetComponent<ModDependenceSettingItem>();
		ModId modId = this._searchedModIdList[index];
		this.RefreshModInfo(index, refers, modId);
		refers.button.ClearAndAddListener(delegate
		{
			bool flag = this._selectedModIdList.Contains(modId);
			if (!flag)
			{
				this._selectedModIdList.Add(modId);
				this.RefreshSelectedList();
			}
		});
	}

	// Token: 0x060027D8 RID: 10200 RVA: 0x00125B94 File Offset: 0x00123D94
	private void OnSelectedItemRender(int index, GameObject go)
	{
		ModDependenceSettingItem refers = go.GetComponent<ModDependenceSettingItem>();
		ModId modId = this._selectedModIdList[index];
		this.RefreshModInfo(index, refers, modId);
		refers.button.ClearAndAddListener(delegate
		{
			this._selectedModIdList.RemoveAt(index);
			this.RefreshSelectedList();
		});
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x00125BF8 File Offset: 0x00123DF8
	private void RefreshModInfo(int index, ModDependenceSettingItem refers, ModId modId)
	{
		ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
		refers.order.text = (index + 1).ToString();
		string title = modInfo.Title.SetColor("pinkyellow");
		refers.title.text = title;
		string nameTitle = LocalStringManager.Get(LanguageKey.LK_Mod_Name).SetColor("lightgrey");
		string colonSymbol = LocalStringManager.Get(LanguageKey.LK_Colon_Symbol).SetColor("lightgrey");
		string authorTitle = LocalStringManager.Get(LanguageKey.LK_Author).SetColor("lightgrey");
		string author = modInfo.Author.SetColor("pinkyellow");
		refers.author.text = authorTitle + colonSymbol + author;
		StringBuilder sb = EasyPool.Get<StringBuilder>();
		sb.Append(nameTitle);
		sb.Append(colonSymbol);
		sb.Append(title);
		sb.AppendLine();
		sb.Append(authorTitle);
		sb.Append(colonSymbol);
		sb.Append(author);
		string tipContent = sb.ToString();
		EasyPool.Free<StringBuilder>(sb);
		TooltipInvoker tip = refers.tip;
		bool flag = tip.PresetParam == null || tip.PresetParam.Length < 1;
		if (flag)
		{
			tip.PresetParam = new string[1];
		}
		tip.PresetParam[0] = tipContent;
	}

	// Token: 0x060027DA RID: 10202 RVA: 0x00125D44 File Offset: 0x00123F44
	private void OnClickConfirm()
	{
		this._dependencyList.Clear();
		this._dependencyList.AddRange(from id in this._selectedModIdList
		select id.FileId);
		this.Hide();
	}

	// Token: 0x060027DB RID: 10203 RVA: 0x00125D9B File Offset: 0x00123F9B
	private void OnClickCancel()
	{
		this.Hide();
	}

	// Token: 0x060027DC RID: 10204 RVA: 0x00125DA8 File Offset: 0x00123FA8
	public void Show(Action onUpdateStart, Action onUpdateEnd, ulong curFileId, List<ulong> dependencyList)
	{
		this._dependencyList = dependencyList;
		this._selectedModIdList.Clear();
		this._originModIdList.Clear();
		onUpdateStart();
		Func<ModId, bool> <>9__4;
		Action<Dictionary<ModId, bool>> <>9__3;
		Action<Dictionary<ModId, bool>> <>9__2;
		ModManager.UpdateSubscribedItems(delegate(Dictionary<ModId, bool> _)
		{
			foreach (ModId modId in ModManager.SubscribedMods)
			{
				bool flag = !this._originModIdList.Contains(modId) && modId.FileId != curFileId;
				if (flag)
				{
					this._originModIdList.Add(modId);
				}
			}
			Action<Dictionary<ModId, bool>> onFinished;
			if ((onFinished = <>9__2) == null)
			{
				onFinished = (<>9__2 = delegate(Dictionary<ModId, bool> _)
				{
					foreach (ModId modId2 in ModManager.UploadedMods)
					{
						bool flag2 = !this._originModIdList.Contains(modId2) && modId2.FileId != curFileId;
						if (flag2)
						{
							this._originModIdList.Add(modId2);
						}
					}
					bool flag3 = dependencyList.Count > 0;
					if (flag3)
					{
						IReadOnlyList<ulong> dependencyList2 = dependencyList;
						Action<Dictionary<ModId, bool>> onFinished2;
						if ((onFinished2 = <>9__3) == null)
						{
							onFinished2 = (<>9__3 = delegate(Dictionary<ModId, bool> dependenciesChangeStateDict)
							{
								IEnumerable<ModId> keys = dependenciesChangeStateDict.Keys;
								Func<ModId, bool> predicate;
								if ((predicate = <>9__4) == null)
								{
									predicate = (<>9__4 = ((ModId id) => dependencyList.Contains(id.FileId)));
								}
								List<ModId> modIdList = keys.Where(predicate).ToList<ModId>();
								foreach (ModId modId3 in modIdList)
								{
									bool flag4 = !this._selectedModIdList.Contains(modId3);
									if (flag4)
									{
										this._selectedModIdList.Add(modId3);
									}
								}
								base.<Show>g__OnUpdateEnd|1();
							});
						}
						ModManager.UpdateTargetItems(dependencyList2, onFinished2);
					}
					else
					{
						base.<Show>g__OnUpdateEnd|1();
					}
				});
			}
			ModManager.UpdateUploadedItems(onFinished);
		});
	}

	// Token: 0x060027DD RID: 10205 RVA: 0x00125E17 File Offset: 0x00124017
	public void Hide()
	{
		this.searchInputField.SetTextWithoutNotify(string.Empty);
		base.gameObject.SetActive(false);
	}

	// Token: 0x060027DE RID: 10206 RVA: 0x00125E38 File Offset: 0x00124038
	private void RefreshSelectedList()
	{
		this.selectedScroll.SetDataCount(this._selectedModIdList.Count);
	}

	// Token: 0x060027DF RID: 10207 RVA: 0x00125E54 File Offset: 0x00124054
	private void RefreshSearchedList()
	{
		string value = this.searchInputField.text;
		this._searchedModIdList.Clear();
		bool flag = value.IsNullOrEmpty();
		if (flag)
		{
			this._searchedModIdList.AddRange(this._originModIdList);
		}
		else
		{
			foreach (ModId modId in this._originModIdList)
			{
				ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
				bool flag2 = modInfo.Title.Contains(value) || modInfo.Author.Contains(value);
				if (flag2)
				{
					this._searchedModIdList.Add(modId);
				}
			}
		}
		this.searchedScroll.SetDataCount(this._searchedModIdList.Count);
	}

	// Token: 0x04001D1C RID: 7452
	[SerializeField]
	private TMP_InputField searchInputField;

	// Token: 0x04001D1D RID: 7453
	[SerializeField]
	private InfinityScroll searchedScroll;

	// Token: 0x04001D1E RID: 7454
	[SerializeField]
	private InfinityScroll selectedScroll;

	// Token: 0x04001D1F RID: 7455
	[SerializeField]
	private CButton btnYes;

	// Token: 0x04001D20 RID: 7456
	[SerializeField]
	private CButton btnNo;

	// Token: 0x04001D21 RID: 7457
	private readonly List<ModId> _originModIdList = new List<ModId>();

	// Token: 0x04001D22 RID: 7458
	private readonly List<ModId> _searchedModIdList = new List<ModId>();

	// Token: 0x04001D23 RID: 7459
	private readonly List<ModId> _selectedModIdList = new List<ModId>();

	// Token: 0x04001D24 RID: 7460
	private List<ulong> _dependencyList;
}
