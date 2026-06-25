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

namespace Game.Views.Mod.Upload
{
	// Token: 0x020008D4 RID: 2260
	public class ModSetDependencePanel : MonoBehaviour
	{
		// Token: 0x06006C43 RID: 27715 RVA: 0x0031E7BC File Offset: 0x0031C9BC
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

		// Token: 0x06006C44 RID: 27716 RVA: 0x0031E86C File Offset: 0x0031CA6C
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

		// Token: 0x06006C45 RID: 27717 RVA: 0x0031E8C8 File Offset: 0x0031CAC8
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

		// Token: 0x06006C46 RID: 27718 RVA: 0x0031E92C File Offset: 0x0031CB2C
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

		// Token: 0x06006C47 RID: 27719 RVA: 0x0031EA78 File Offset: 0x0031CC78
		private void OnClickConfirm()
		{
			this._dependencyList.Clear();
			this._dependencyList.AddRange(from id in this._selectedModIdList
			select id.FileId);
			this.Hide();
		}

		// Token: 0x06006C48 RID: 27720 RVA: 0x0031EACF File Offset: 0x0031CCCF
		private void OnClickCancel()
		{
			this.Hide();
		}

		// Token: 0x06006C49 RID: 27721 RVA: 0x0031EADC File Offset: 0x0031CCDC
		public void Show(ulong curFileId, List<ulong> dependencyList)
		{
			this._dependencyList = dependencyList;
			this._selectedModIdList.Clear();
			this._originModIdList.Clear();
			GEvent.OnEvent(UiEvents.OnModViewShowMask, null);
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

		// Token: 0x06006C4A RID: 27722 RVA: 0x0031EB4D File Offset: 0x0031CD4D
		public void Hide()
		{
			this.searchInputField.SetTextWithoutNotify(string.Empty);
			base.gameObject.SetActive(false);
		}

		// Token: 0x06006C4B RID: 27723 RVA: 0x0031EB6E File Offset: 0x0031CD6E
		private void RefreshSelectedList()
		{
			this.selectedScroll.SetDataCount(this._selectedModIdList.Count);
		}

		// Token: 0x06006C4C RID: 27724 RVA: 0x0031EB88 File Offset: 0x0031CD88
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

		// Token: 0x04004E81 RID: 20097
		[SerializeField]
		private TMP_InputField searchInputField;

		// Token: 0x04004E82 RID: 20098
		[SerializeField]
		private InfinityScroll searchedScroll;

		// Token: 0x04004E83 RID: 20099
		[SerializeField]
		private InfinityScroll selectedScroll;

		// Token: 0x04004E84 RID: 20100
		[SerializeField]
		private CButton btnYes;

		// Token: 0x04004E85 RID: 20101
		[SerializeField]
		private CButton btnNo;

		// Token: 0x04004E86 RID: 20102
		private readonly List<ModId> _originModIdList = new List<ModId>();

		// Token: 0x04004E87 RID: 20103
		private readonly List<ModId> _searchedModIdList = new List<ModId>();

		// Token: 0x04004E88 RID: 20104
		private readonly List<ModId> _selectedModIdList = new List<ModId>();

		// Token: 0x04004E89 RID: 20105
		private List<ulong> _dependencyList;
	}
}
