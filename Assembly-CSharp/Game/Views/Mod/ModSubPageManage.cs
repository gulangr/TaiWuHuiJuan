using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using FrameWork.ModSystem;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Mod;
using GameData.Domains.Mod;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization;
using TMPro;
using UnityEngine;

namespace Game.Views.Mod
{
	// Token: 0x020008CE RID: 2254
	public class ModSubPageManage : ModSubPage
	{
		// Token: 0x17000CA7 RID: 3239
		// (get) Token: 0x06006B77 RID: 27511 RVA: 0x0031984F File Offset: 0x00317A4F
		private bool IsInGame
		{
			get
			{
				return GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			}
		}

		// Token: 0x06006B78 RID: 27512 RVA: 0x00319860 File Offset: 0x00317A60
		public override void Init(ViewMod parentView)
		{
			base.Init(parentView);
			this.btnCloseAll.ClearAndAddListener(new Action(this.OnClickCloseAll));
			this.btnOpenCompatible.ClearAndAddListener(new Action(this.OnClickOpenCompatible));
			this.btnRefresh.ClearAndAddListener(new Action(this.RequestData));
			this.btnSave.ClearAndAddListener(new Action(this.OnClickSave));
			this.btnImport.ClearAndAddListener(new Action(this.OnClickImport));
			this.btnExport.ClearAndAddListener(new Action(this.OnClickExport));
			this.btnClearFilter.ClearAndAddListener(new Action(this.OnClickClearFilter));
			this.searchField.onValueChanged.ResetListener(new Action<string>(this.OnSearch));
			this._sortAndFilterController = new ModSortAndFilterController(this.sortAndFilter);
			this._sortAndFilterController.Init(new Action(this.RefreshList), "ModSortAndFilter");
			this.scroll.SetRowTemplate(this.rowTemplate);
			this.scroll.Init<ModSortAndFilterController>(this.GetCurrentColumnDefinitions(), true, null, new Action<int, RowItem>(this.OnClickRow));
			this.scroll.SetSortController(this._sortAndFilterController);
		}

		// Token: 0x06006B79 RID: 27513 RVA: 0x003199B0 File Offset: 0x00317BB0
		public override bool QuickHide()
		{
			bool interactable = this.btnSave.interactable;
			bool result;
			if (interactable)
			{
				DialogCmd dialog = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_Mod_HideWithoutApply_Dialog_Title),
					Content = LocalStringManager.Get(LanguageKey.LK_Mod_HideWithoutApply_Dialog_Content),
					Type = 1,
					Yes = delegate()
					{
						this.btnSave.interactable = false;
						this.QuickHide();
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialog));
				UIManager.Instance.MaskUI(UIElement.Dialog);
				result = true;
			}
			else
			{
				bool activeSelf = this.modEnableDependenceDialog.gameObject.activeSelf;
				if (activeSelf)
				{
					this.modEnableDependenceDialog.Hide();
					result = true;
				}
				else
				{
					result = base.QuickHide();
				}
			}
			return result;
		}

		// Token: 0x06006B7A RID: 27514 RVA: 0x00319A6B File Offset: 0x00317C6B
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshAll();
		}

		// Token: 0x06006B7B RID: 27515 RVA: 0x00319A7C File Offset: 0x00317C7C
		private void RefreshAll()
		{
			this._tempModOrder.Clear();
			this._whiteListMods.Clear();
			this._tempEnabledModIdList.Clear();
			this._tempEnabledModIdList.AddRange(ModManager.EnabledMods);
			this.searchField.SetTextWithoutNotify(string.Empty);
			this.RequestData();
			this.RefreshButtons();
		}

		// Token: 0x06006B7C RID: 27516 RVA: 0x00319ADE File Offset: 0x00317CDE
		private void RequestData()
		{
			ModManager.UpdateModList(delegate
			{
				bool flag = !this.IsInGame;
				if (flag)
				{
					this._hasUpdateDetailInfo = false;
				}
				bool flag2 = this.IsInGame && this._hasUpdateDetailInfo;
				if (flag2)
				{
					this.RefreshCurModList(true);
				}
				else
				{
					bool flag3 = !this._hasUpdateDetailInfo;
					if (flag3)
					{
						this.ShowMask();
						ModManager.UpdateUploadedItems(delegate(Dictionary<ModId, bool> uploadedDependencyChangeStateDict)
						{
							Action<Dictionary<ModId, bool>> <>9__3;
							ModManager.UpdateSubscribedItems(delegate(Dictionary<ModId, bool> subscribedDependencyChangeStateDict)
							{
								bool flag4 = subscribedDependencyChangeStateDict != null && subscribedDependencyChangeStateDict.Count > 0;
								if (flag4)
								{
									foreach (KeyValuePair<ModId, bool> keyValuePair in subscribedDependencyChangeStateDict)
									{
										ModId modId;
										bool flag5;
										keyValuePair.Deconstruct(out modId, out flag5);
										ModId key = modId;
										bool value = flag5;
										uploadedDependencyChangeStateDict.TryAdd(key, value);
									}
								}
								Action<Dictionary<ModId, bool>> onFinished;
								if ((onFinished = <>9__3) == null)
								{
									ModId modId;
									onFinished = (<>9__3 = delegate(Dictionary<ModId, bool> _)
									{
										this._hasUpdateDetailInfo = true;
										this._dependenciesChangedList.Clear();
										bool flag6 = uploadedDependencyChangeStateDict != null && uploadedDependencyChangeStateDict.Count > 0;
										if (flag6)
										{
											foreach (KeyValuePair<ModId, bool> keyValuePair2 in uploadedDependencyChangeStateDict)
											{
												ModId modId2;
												bool flag7;
												keyValuePair2.Deconstruct(out modId2, out flag7);
												ModId modId = modId2;
												bool changed = flag7;
												bool flag8 = changed && ModManager.EnabledMods.Exists((ModId id) => id.FileId == modId.FileId);
												if (flag8)
												{
													this._dependenciesChangedList.Add(modId);
												}
											}
										}
										this._tempEnabledModIdList.Clear();
										this._tempEnabledModIdList.AddRange(ModManager.EnabledMods);
										bool flag9 = this._dependenciesChangedList.Count > 0;
										if (flag9)
										{
											ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("DependenciesChangedList", this._dependenciesChangedList);
											UIElement.ModDependenceChangeList.SetOnInitArgs(args);
											UIManager.Instance.ShowUI(UIElement.ModDependenceChangeList, true);
										}
										this.RefreshCurModList(true);
										this.HideMask();
									});
								}
								ModManager.UpdateUploadedItems(onFinished);
							});
						});
					}
				}
			});
		}

		// Token: 0x06006B7D RID: 27517 RVA: 0x00319AF4 File Offset: 0x00317CF4
		private void RefreshCurModList(bool refreshData = true)
		{
			if (refreshData)
			{
				this._mods.Clear();
				bool isInGame = this.IsInGame;
				if (isInGame)
				{
					this._mods.AddRange(ModManager.EnabledMods);
				}
				else
				{
					this._mods.AddRange(ModManager.ExternalMods);
					this._mods.AddRange(from id in ModManager.PlatformMods
					where ModManager.GetModInfo(id).IsSubscribed
					select id);
				}
			}
			this._allData.Clear();
			this._indexMap.Clear();
			foreach (ModId modId in this._mods)
			{
				this._indexMap[modId] = this._allData.Count;
				this._allData.Add(this.GetSortAndFilterDataById(modId));
			}
			this.RefreshList();
		}

		// Token: 0x06006B7E RID: 27518 RVA: 0x00319C0C File Offset: 0x00317E0C
		private void RefreshList()
		{
			this._filteredList.Clear();
			Func<ModSortAndFilterData, bool> filter = this._sortAndFilterController.GenerateFilter();
			Comparison<ModSortAndFilterData> comparer = this._sortAndFilterController.GenerateComparer(this._filteredList);
			bool flag = this._searchText.IsNullOrEmpty();
			if (flag)
			{
				foreach (ModSortAndFilterData data in this._allData)
				{
					bool flag2 = filter(data);
					if (flag2)
					{
						this._filteredList.Add(data);
					}
				}
			}
			else
			{
				foreach (ModSortAndFilterData data2 in this._allData)
				{
					bool flag3 = filter(data2) && data2.Name.Contains(this._searchText);
					if (flag3)
					{
						this._filteredList.Add(data2);
					}
				}
			}
			bool flag4 = comparer != null;
			if (flag4)
			{
				this._filteredList.Sort(comparer);
			}
			this.noContent.SetActive(this._filteredList.Count == 0);
			this._sortAndFilterController.AfterFilter(this._allData);
			this.scroll.SetData<ModSortAndFilterData>(this._filteredList, -1);
		}

		// Token: 0x06006B7F RID: 27519 RVA: 0x00319D80 File Offset: 0x00317F80
		private void RefreshButtons()
		{
			bool modified = this._tempModOrder.Count > 0 || this._tempEnabledModIdList.ContentIsDifferent(ModManager.EnabledMods) || this._whiteListMods.Count > 0;
			this.btnSave.interactable = modified;
			this.btnExport.interactable = !modified;
		}

		// Token: 0x06006B80 RID: 27520 RVA: 0x00319DE0 File Offset: 0x00317FE0
		private void Save()
		{
			foreach (ModId modId in this._tempEnabledModIdList)
			{
				bool flag = !ModManager.EnabledMods.Contains(modId);
				if (flag)
				{
					ModManager.SetModEnabled(modId, true);
				}
			}
			List<ModId> mods = new List<ModId>();
			mods.AddRange(ModManager.EnabledMods);
			foreach (ModId modId2 in mods)
			{
				bool flag2 = !this._tempEnabledModIdList.Contains(modId2);
				if (flag2)
				{
					ModManager.SetModEnabled(modId2, false);
				}
			}
			foreach (KeyValuePair<ModId, int> keyValuePair in this._tempModOrder)
			{
				ModId modId3;
				int num;
				keyValuePair.Deconstruct(out modId3, out num);
				ModId id = modId3;
				int order = num;
				ModManager.SetModOrder(id, order);
			}
			ModManager.SaveModSettings(false);
			this.RefreshAll();
			this.RefreshButtons();
			DialogCmd dialog = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_Apply_Restart_Title),
				Content = LocalStringManager.Get(LanguageKey.LK_Apply_Restart_Content),
				Type = 1,
				Yes = new Action(GameApp.Instance.ReStart)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialog));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06006B81 RID: 27521 RVA: 0x00319F98 File Offset: 0x00318198
		private void SaveBeforeCheck()
		{
			this._missing.Clear();
			List<ModInfoWithDisplayData> checkList = new List<ModInfoWithDisplayData>();
			foreach (ModId modId in this._tempEnabledModIdList)
			{
				bool flag = ModManager.EnabledMods.Contains(modId);
				if (!flag)
				{
					ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
					bool flag2 = modInfo.Dependencies.Count > 0;
					if (flag2)
					{
						checkList.Add(modInfo);
					}
				}
			}
			bool flag3 = checkList.Count > 0;
			if (flag3)
			{
				for (int index = 0; index < checkList.Count; index++)
				{
					this.CheckAndUpdateDependency(checkList[index], index == checkList.Count - 1);
				}
			}
			else
			{
				this.Save();
			}
		}

		// Token: 0x06006B82 RID: 27522 RVA: 0x0031A084 File Offset: 0x00318284
		private void CheckAndUpdateDependency(ModInfoWithDisplayData modInfo, bool isEnd)
		{
			List<ulong> missingList = new List<ulong>();
			ModManager.UpdateTargetItems(modInfo.Dependencies, missingList, delegate(Dictionary<ModId, bool> dependenciesChangeStateDict)
			{
				List<ModId> dependenceList = new List<ModId>();
				bool flag = dependenciesChangeStateDict != null;
				if (flag)
				{
					dependenceList.AddRange(dependenciesChangeStateDict.Keys);
				}
				IEnumerable<ModId> local = from modId in ModManager.ExternalMods
				where modInfo.Dependencies.Contains(modId.FileId) && (dependenciesChangeStateDict == null || dependenciesChangeStateDict.All((KeyValuePair<ModId, bool> pair) => pair.Key.FileId != modId.FileId))
				select modId;
				dependenceList.AddRange(local);
				using (List<ModId>.Enumerator enumerator = dependenceList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ModId modId = enumerator.Current;
						bool flag2 = this._missing.Contains(modId);
						if (!flag2)
						{
							bool flag3 = !this._tempEnabledModIdList.Exists((ModId matchId) => modId.FileId == matchId.FileId);
							if (flag3)
							{
								this._missing.Add(modId);
							}
						}
					}
				}
				bool isEnd2 = isEnd;
				if (isEnd2)
				{
					bool flag4 = missingList.Count > 0;
					if (flag4)
					{
						StringBuilder missingStr = new StringBuilder();
						foreach (ulong missing in missingList)
						{
							missingStr.AppendLine(missing.ToString());
						}
						DialogCmd dialog = new DialogCmd
						{
							Title = LanguageKey.LK_Mod_Enable_Dependency_Missing_Title.Tr(),
							Content = LanguageKey.LK_Mod_Enable_Dependency_Missing_Content.TrFormat(missingStr),
							Type = 2
						};
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialog));
						UIManager.Instance.MaskUI(UIElement.Dialog);
					}
					bool flag5 = this._missing.Count == 0;
					if (flag5)
					{
						this.Save();
					}
					else
					{
						this.modEnableDependenceDialog.Show(this._missing.ToList<ModId>(), new Action(this.SubscribeAndSave), new Action(this.Save));
					}
				}
			});
		}

		// Token: 0x06006B83 RID: 27523 RVA: 0x0031A0DC File Offset: 0x003182DC
		private void SubscribeAndSave()
		{
			this.Save();
			foreach (ModId modId in this._missing)
			{
				GEvent.OnEvent(UiEvents.OnModViewDownload, EasyPool.Get<ArgumentBox>().SetObject("ModId", modId));
			}
		}

		// Token: 0x06006B84 RID: 27524 RVA: 0x0031A158 File Offset: 0x00318358
		private void ShowMask()
		{
			this.loadingAnimation.gameObject.SetActive(true);
		}

		// Token: 0x06006B85 RID: 27525 RVA: 0x0031A16D File Offset: 0x0031836D
		private void HideMask()
		{
			this.loadingAnimation.gameObject.SetActive(false);
		}

		// Token: 0x06006B86 RID: 27526 RVA: 0x0031A182 File Offset: 0x00318382
		private void OnClickCloseAll()
		{
			this._tempEnabledModIdList.Clear();
			this.RefreshList();
			this.RefreshButtons();
		}

		// Token: 0x06006B87 RID: 27527 RVA: 0x0031A1A0 File Offset: 0x003183A0
		private void OnClickOpenCompatible()
		{
			this._tempEnabledModIdList.Clear();
			foreach (ModSortAndFilterData data in this._allData)
			{
				bool flag = !data.IsExpired;
				if (flag)
				{
					this._tempEnabledModIdList.Add(data.ModId);
				}
			}
			this.RefreshList();
			this.RefreshButtons();
		}

		// Token: 0x06006B88 RID: 27528 RVA: 0x0031A228 File Offset: 0x00318428
		private void OnClickSave()
		{
			CommonUtils.ShowConfirmDialog(LanguageKey.LK_Common_Attention.Tr(), LanguageKey.LK_Mod_Save_Confirm_Content.Tr(), new Action(this.SaveBeforeCheck), null, EDialogType.None);
		}

		// Token: 0x06006B89 RID: 27529 RVA: 0x0031A254 File Offset: 0x00318454
		private void OnClickImport()
		{
			string archiveDir = GameApp.GetArchiveDirPath();
			bool flag = !Directory.Exists(archiveDir);
			if (flag)
			{
				Directory.CreateDirectory(archiveDir);
			}
			string path = LocalDialog.GetUnitySelectFileName("Lua Files(*.lua)\0*.lua\0", archiveDir);
			bool flag2 = File.Exists(path);
			if (flag2)
			{
				this.Import(path);
			}
		}

		// Token: 0x06006B8A RID: 27530 RVA: 0x0031A29A File Offset: 0x0031849A
		private void OnClickExport()
		{
			this.Export();
		}

		// Token: 0x06006B8B RID: 27531 RVA: 0x0031A2A4 File Offset: 0x003184A4
		private void OnClickClearFilter()
		{
			this._sortAndFilterController.ClearAllFilter();
		}

		// Token: 0x06006B8C RID: 27532 RVA: 0x0031A2B4 File Offset: 0x003184B4
		private void OnSwitch(int index, bool value)
		{
			ModId modId = this._allData[index].ModId;
			if (value)
			{
				bool flag = !this._tempEnabledModIdList.Contains(modId);
				if (flag)
				{
					this._tempEnabledModIdList.Add(modId);
				}
			}
			else
			{
				this._tempEnabledModIdList.Remove(modId);
			}
			this.RefreshList();
			this.RefreshButtons();
		}

		// Token: 0x06006B8D RID: 27533 RVA: 0x0031A31A File Offset: 0x0031851A
		private void OnChangeOrder(int index, int value)
		{
			this._tempModOrder[this._allData[index].ModId] = value;
			this.RefreshButtons();
		}

		// Token: 0x06006B8E RID: 27534 RVA: 0x0031A344 File Offset: 0x00318544
		private void OnSearch(string value)
		{
			bool flag = CommonUtils.FixToShowAbleString(ref value, this.searchField.textComponent.font);
			if (flag)
			{
				this.searchField.SetTextWithoutNotify(value);
			}
			this._searchText = value;
			this.RefreshList();
		}

		// Token: 0x06006B8F RID: 27535 RVA: 0x0031A388 File Offset: 0x00318588
		private void OnClickRow(int index, RowItem item)
		{
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(this._filteredList[index].ModId);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("DisplayData", modInfo);
			argBox.SetObject("OriginName", modInfo.Title);
			argBox.SetObject("OriginId", modInfo.ModId);
			argBox.SetObject("RefreshFunc", new Action(this.OnRefresh));
			argBox.Set("IsShowSettings", true);
			UIElement.ModInfo.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.ModInfo);
		}

		// Token: 0x06006B90 RID: 27536 RVA: 0x0031A42A File Offset: 0x0031862A
		private void OnRefresh()
		{
			this._hasUpdateDetailInfo = false;
			this.RefreshAll();
		}

		// Token: 0x06006B91 RID: 27537 RVA: 0x0031A43B File Offset: 0x0031863B
		private IEnumerable<ColumnDefinition> GetCurrentColumnDefinitions()
		{
			ColumnDefinition<ModSortAndFilterData, SwitchCellData> columnDefinition = new ColumnDefinition<ModSortAndFilterData, SwitchCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 242f,
				FlexibleWidth = 1000f,
				PreferredWidth = 242f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Mod_Head_Status.Tr());
			columnDefinition.CellDataGenerator = ((ModSortAndFilterData data) => new SwitchCellData
			{
				Id = this._indexMap[data.ModId],
				GetAction = new Func<int, bool>(this.IsModEnabled),
				SetAction = new Action<int, bool>(this.OnSwitch)
			});
			columnDefinition.SortId = 170;
			yield return columnDefinition;
			ColumnDefinition<ModSortAndFilterData, InputIntegerCellData> columnDefinition2 = new ColumnDefinition<ModSortAndFilterData, InputIntegerCellData>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 220f,
				FlexibleWidth = 1000f,
				PreferredWidth = 220f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_Mod_Head_Order.Tr());
			columnDefinition2.CellDataGenerator = ((ModSortAndFilterData data) => new InputIntegerCellData
			{
				Id = this._indexMap[data.ModId],
				GetAction = new Func<int, int>(this.GetOrder),
				SetAction = new Action<int, int>(this.OnChangeOrder)
			});
			columnDefinition2.SortId = 171;
			yield return columnDefinition2;
			ColumnDefinition<ModSortAndFilterData, IconAndTextCellData> columnDefinition3 = new ColumnDefinition<ModSortAndFilterData, IconAndTextCellData>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 680f,
				FlexibleWidth = 1000f,
				PreferredWidth = 680f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_Mod_Head_Name.Tr());
			columnDefinition3.CellDataGenerator = ((ModSortAndFilterData data) => new IconAndTextCellData
			{
				IconName = this.GetIcon(data.ModId),
				Text = data.Name
			});
			columnDefinition3.SortId = 172;
			yield return columnDefinition3;
			ColumnDefinition<ModSortAndFilterData, List<string>> columnDefinition4 = new ColumnDefinition<ModSortAndFilterData, List<string>>();
			columnDefinition4.LayoutOption = new LayoutOption
			{
				MinWidth = 370f,
				FlexibleWidth = 1000f,
				PreferredWidth = 370f,
				Priority = 1
			};
			columnDefinition4.TableHeadLabel = (() => LanguageKey.LK_Mod_Head_Tag.Tr());
			columnDefinition4.CellDataGenerator = new Func<ModSortAndFilterData, List<string>>(this.GetTags);
			columnDefinition4.SortId = -1;
			yield return columnDefinition4;
			ColumnDefinition<ModSortAndFilterData, List<IconAndTextCellData>> columnDefinition5 = new ColumnDefinition<ModSortAndFilterData, List<IconAndTextCellData>>();
			columnDefinition5.LayoutOption = new LayoutOption
			{
				MinWidth = 240f,
				FlexibleWidth = 1000f,
				PreferredWidth = 240f,
				Priority = 1
			};
			columnDefinition5.TableHeadLabel = (() => LanguageKey.LK_Mod_Head_Version.Tr());
			columnDefinition5.CellDataGenerator = new Func<ModSortAndFilterData, List<IconAndTextCellData>>(this.GetVersion);
			columnDefinition5.SortId = -1;
			yield return columnDefinition5;
			ColumnDefinition<ModSortAndFilterData, IconAndTextCellData> columnDefinition6 = new ColumnDefinition<ModSortAndFilterData, IconAndTextCellData>();
			columnDefinition6.LayoutOption = new LayoutOption
			{
				MinWidth = 180f,
				FlexibleWidth = 1000f,
				PreferredWidth = 180f,
				Priority = 1
			};
			columnDefinition6.TableHeadLabel = (() => LanguageKey.LK_Mod_Head_Source.Tr());
			columnDefinition6.CellDataGenerator = new Func<ModSortAndFilterData, IconAndTextCellData>(this.GetSource);
			columnDefinition6.SortId = -1;
			yield return columnDefinition6;
			ColumnDefinition<ModSortAndFilterData, List<IconAndTextCellData>> columnDefinition7 = new ColumnDefinition<ModSortAndFilterData, List<IconAndTextCellData>>();
			columnDefinition7.LayoutOption = new LayoutOption
			{
				MinWidth = 190f,
				FlexibleWidth = 1000f,
				PreferredWidth = 190f,
				Priority = 1
			};
			columnDefinition7.TableHeadLabel = (() => LanguageKey.LK_Mod_Head_Rate.Tr());
			columnDefinition7.CellDataGenerator = new Func<ModSortAndFilterData, List<IconAndTextCellData>>(this.GetRate);
			columnDefinition7.SortId = 173;
			yield return columnDefinition7;
			ColumnDefinition<ModSortAndFilterData, List<IconAndTextCellData>> columnDefinition8 = new ColumnDefinition<ModSortAndFilterData, List<IconAndTextCellData>>();
			columnDefinition8.LayoutOption = new LayoutOption
			{
				MinWidth = 220f,
				FlexibleWidth = 1000f,
				PreferredWidth = 220f,
				Priority = 1
			};
			columnDefinition8.TableHeadLabel = (() => LanguageKey.LK_Mod_Head_UpdateTime.Tr());
			columnDefinition8.CellDataGenerator = new Func<ModSortAndFilterData, List<IconAndTextCellData>>(this.GetUpdateTime);
			columnDefinition8.SortId = 175;
			yield return columnDefinition8;
			ColumnDefinition<ModSortAndFilterData, string> columnDefinition9 = new ColumnDefinition<ModSortAndFilterData, string>();
			columnDefinition9.LayoutOption = new LayoutOption
			{
				MinWidth = 167f,
				FlexibleWidth = 1000f,
				PreferredWidth = 167f,
				Priority = 1
			};
			columnDefinition9.TableHeadLabel = (() => LanguageKey.LK_Mod_Head_Size.Tr());
			columnDefinition9.CellDataGenerator = new Func<ModSortAndFilterData, string>(this.GetSize);
			columnDefinition9.SortId = 176;
			yield return columnDefinition9;
			yield break;
		}

		// Token: 0x06006B92 RID: 27538 RVA: 0x0031A44C File Offset: 0x0031864C
		private ModSortAndFilterData GetSortAndFilterDataById(ModId modId)
		{
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
			return new ModSortAndFilterData
			{
				ModId = modId,
				IsLocal = (modId.Source != 1),
				IsExpired = ModManager.IsModOutdated(modInfo),
				Name = modInfo.Title,
				Rate = modInfo.OriginScore * 5f,
				UpdateTime = modInfo.UpdateData,
				UploadTime = modInfo.CreateData,
				Size = modInfo.FileSize,
				Tags = SteamManager.GetTagMask(modInfo.TagList),
				IsEnabled = new Func<ModId, bool>(this.IsModEnabled),
				GetOrder = new Func<ModId, int>(this.GetOrder)
			};
		}

		// Token: 0x06006B93 RID: 27539 RVA: 0x0031A508 File Offset: 0x00318708
		private string GetIcon(ModId modId)
		{
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
			return (modInfo != null) ? modInfo.Cover : null;
		}

		// Token: 0x06006B94 RID: 27540 RVA: 0x0031A52C File Offset: 0x0031872C
		private string GetTimeString(uint time)
		{
			return ViewModInfo.GetTimeString(time);
		}

		// Token: 0x06006B95 RID: 27541 RVA: 0x0031A534 File Offset: 0x00318734
		private string GetSize(ModSortAndFilterData data)
		{
			return string.Format("{0} MB", Math.Round((double)data.Size / 1048576.0, 3));
		}

		// Token: 0x06006B96 RID: 27542 RVA: 0x0031A55C File Offset: 0x0031875C
		private List<IconAndTextCellData> GetVersion(ModSortAndFilterData data)
		{
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(data.ModId);
			string text;
			if (modInfo == null)
			{
				text = null;
			}
			else
			{
				Version gameVersion = modInfo.GameVersion;
				text = ((gameVersion != null) ? gameVersion.ToString() : null);
			}
			string supportedVersion = text ?? LanguageKey.LK_None.Tr();
			string icon = data.IsExpired ? "ui9_modpanel_icon_compatible_3" : "ui9_modpanel_icon_compatible_0";
			string color = data.IsExpired ? "brightred" : "brightblue";
			return new List<IconAndTextCellData>
			{
				new IconAndTextCellData(null, "V " + ModManager.VersionUlongToString(data.ModId.Version), false, false, false, false),
				new IconAndTextCellData(icon, (LanguageKey.LK_Mod_Supported_Version.Tr() + supportedVersion).SetColor(color), true, false, false, false)
			};
		}

		// Token: 0x06006B97 RID: 27543 RVA: 0x0031A621 File Offset: 0x00318821
		private IconAndTextCellData GetSource(ModSortAndFilterData data)
		{
			return data.IsLocal ? new IconAndTextCellData("ui9_modpanel_icon_local_0", LanguageKey.LK_Mod_Local.Tr(), true, false, false, false) : new IconAndTextCellData("ui9_modpanel_icon_logo_0", LanguageKey.LK_Mod_Steam.Tr(), true, false, false, false);
		}

		// Token: 0x06006B98 RID: 27544 RVA: 0x0031A660 File Offset: 0x00318860
		private List<IconAndTextCellData> GetRate(ModSortAndFilterData data)
		{
			List<IconAndTextCellData> list = new List<IconAndTextCellData>();
			list.Add(new IconAndTextCellData("ui9_modpanel_icon_evaluate_0", string.Format("{0:f1}", data.Rate), true, false, false, false));
			string iconName = null;
			LanguageKey languageKey = LanguageKey.LK_Mod_Rate_Count;
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(data.ModId);
			list.Add(new IconAndTextCellData(iconName, languageKey.TrFormat(CommonUtils.GetDisplayStringForNum((long)((ulong)((modInfo != null) ? modInfo.VoteCount : 0U)))), false, false, false, false));
			return list;
		}

		// Token: 0x06006B99 RID: 27545 RVA: 0x0031A6D4 File Offset: 0x003188D4
		private List<IconAndTextCellData> GetUpdateTime(ModSortAndFilterData data)
		{
			return new List<IconAndTextCellData>
			{
				new IconAndTextCellData("ui9_modpanel_icon_date_1", LanguageKey.LK_Mod_UpdateTime.TrFormat(this.GetTimeString(data.UpdateTime)), true, false, false, false),
				new IconAndTextCellData("ui9_modpanel_icon_date_0", LanguageKey.LK_Mod_UploadTime.TrFormat(this.GetTimeString(data.UpdateTime)), true, false, false, false)
			};
		}

		// Token: 0x06006B9A RID: 27546 RVA: 0x0031A73C File Offset: 0x0031893C
		private List<string> GetTags(ModSortAndFilterData data)
		{
			List<string> res = new List<string>();
			foreach (object obj in Enum.GetValues(typeof(SteamManager.ESteamTag)))
			{
				SteamManager.ESteamTag val = (SteamManager.ESteamTag)obj;
				bool flag = (data.Tags & 1 << (int)val) != 0;
				if (flag)
				{
					res.Add(SteamManager.SteamTagLanguageKeyDic[val].Tr());
				}
			}
			return res;
		}

		// Token: 0x06006B9B RID: 27547 RVA: 0x0031A7D8 File Offset: 0x003189D8
		private bool IsModEnabled(ModId id)
		{
			return this._tempEnabledModIdList.Contains(id);
		}

		// Token: 0x06006B9C RID: 27548 RVA: 0x0031A7E6 File Offset: 0x003189E6
		private bool IsModEnabled(int index)
		{
			return this.IsModEnabled(this._allData[index].ModId);
		}

		// Token: 0x06006B9D RID: 27549 RVA: 0x0031A7FF File Offset: 0x003189FF
		private int GetOrder(ModId id)
		{
			return this._tempModOrder.GetValueOrDefault(id, ModManager.GetModOrder(id));
		}

		// Token: 0x06006B9E RID: 27550 RVA: 0x0031A813 File Offset: 0x00318A13
		private int GetOrder(int index)
		{
			return this.GetOrder(this._allData[index].ModId);
		}

		// Token: 0x06006B9F RID: 27551 RVA: 0x0031A82C File Offset: 0x00318A2C
		private void Export()
		{
			string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
			Table table = new Table(null);
			string archiveDir = GameApp.GetArchiveDirPath();
			bool flag = !Directory.Exists(archiveDir);
			if (flag)
			{
				Directory.CreateDirectory(archiveDir);
			}
			StreamWriter writer = File.CreateText(Path.Combine(archiveDir, string.Format("ModSettings{0}.Lua", timeStamp)));
			List<string> enabledMods = new List<string>();
			List<string> whiteListMods = new List<string>();
			Dictionary<string, int> modOrder = new Dictionary<string, int>();
			foreach (ModId id in ModManager.EnabledMods)
			{
				enabledMods.Add(id.ToString());
			}
			foreach (string id2 in ModManager.WhitelistMods)
			{
				whiteListMods.Add(id2);
			}
			foreach (KeyValuePair<string, int> keyValuePair in ModManager.ModOrder)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string id3 = text;
				int value = num;
				modOrder.Add(id3, value);
			}
			table.Save("EnabledMods", enabledMods);
			table.Save("WhitelistMods", whiteListMods);
			table.Save("ModOrder", modOrder);
			writer.Write(table.Serialize(true, 0));
			writer.Close();
			DialogCmd dialog = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_Mod_Export_Title),
				Content = LocalStringManager.Get(LanguageKey.LK_Mod_Export_Content),
				Type = 2
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialog));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06006BA0 RID: 27552 RVA: 0x0031AA30 File Offset: 0x00318C30
		private void Import(string path)
		{
			Table luaTable = null;
			string text = File.ReadAllText(path);
			try
			{
				luaTable = LuaGame.Instance.ReadMoonSharpTable(text);
			}
			catch (Exception exception)
			{
				Debug.LogWarning("Invalid LuaTable: \n" + text);
				PredefinedLog.Show(4, path, exception.Message);
				luaTable = null;
				File.Delete(path);
			}
			bool flag = luaTable == null;
			if (!flag)
			{
				HashSet<string> missingMods = new HashSet<string>();
				List<ModId> enabledMods = new List<ModId>();
				List<string> whiteListMods = new List<string>();
				Dictionary<ModId, int> modOrder = new Dictionary<ModId, int>();
				List<string> mods;
				luaTable.Load("EnabledMods", out mods);
				bool flag2 = mods != null;
				if (flag2)
				{
					foreach (string modIdStr in mods)
					{
						ModInfoWithDisplayData modInfo;
						bool flag3 = ModManager.LocalMods.TryGetValue(modIdStr, out modInfo);
						if (flag3)
						{
							enabledMods.Add(modInfo.ModId);
						}
						else
						{
							missingMods.Add(modIdStr);
						}
					}
				}
				List<string> mods2;
				luaTable.Load("WhitelistMods", out mods2);
				bool flag4 = mods2 != null;
				if (flag4)
				{
					foreach (string modIdStr2 in mods2)
					{
						ModInfoWithDisplayData modInfo2;
						bool flag5 = ModManager.LocalMods.TryGetValue(modIdStr2, out modInfo2);
						if (flag5)
						{
							whiteListMods.Add(modIdStr2);
						}
						else
						{
							missingMods.Add(modIdStr2);
						}
					}
				}
				Dictionary<string, int> mods3;
				luaTable.Load("ModOrder", out mods3);
				bool flag6 = mods3 != null;
				if (flag6)
				{
					foreach (KeyValuePair<string, int> keyValuePair in mods3)
					{
						string text2;
						int num;
						keyValuePair.Deconstruct(out text2, out num);
						string modIdStr3 = text2;
						int value = num;
						ModInfoWithDisplayData modInfo3;
						bool flag7 = ModManager.LocalMods.TryGetValue(modIdStr3, out modInfo3);
						if (flag7)
						{
							modOrder[modInfo3.ModId] = Math.Clamp(value, 0, 9999);
						}
						else
						{
							missingMods.Add(modIdStr3);
						}
					}
				}
				StringBuilder sb = new StringBuilder();
				bool flag8 = missingMods.Count > 0;
				string content;
				if (flag8)
				{
					int index = 0;
					foreach (string modStr in missingMods)
					{
						bool flag9 = index++ != 0;
						if (flag9)
						{
							sb.Append(LanguageKey.LK_Separator.Tr());
						}
						sb.Append(modStr);
					}
					content = LanguageKey.LK_Mod_Import_Missing_Content.TrFormat(sb.ToString()) + "\n" + LanguageKey.LK_Mod_Import_Confirm.Tr();
				}
				else
				{
					content = LanguageKey.LK_Mod_Import_Confirm.Tr();
				}
				CommonUtils.ShowConfirmDialog(LanguageKey.LK_Mod_Import.Tr(), content, delegate
				{
					this._tempEnabledModIdList.Clear();
					this._tempEnabledModIdList.AddRange(enabledMods);
					this._whiteListMods.Clear();
					this._whiteListMods.AddRange(whiteListMods);
					this._tempModOrder.Clear();
					foreach (KeyValuePair<ModId, int> keyValuePair2 in modOrder)
					{
						ModId modId;
						int num2;
						keyValuePair2.Deconstruct(out modId, out num2);
						ModId id = modId;
						int value2 = num2;
						this._tempModOrder[id] = value2;
					}
					this.Save();
				}, null, EDialogType.None);
			}
		}

		// Token: 0x04004DF7 RID: 19959
		[SerializeField]
		private CButton btnCloseAll;

		// Token: 0x04004DF8 RID: 19960
		[SerializeField]
		private CButton btnOpenCompatible;

		// Token: 0x04004DF9 RID: 19961
		[SerializeField]
		private CButton btnRefresh;

		// Token: 0x04004DFA RID: 19962
		[SerializeField]
		private CButton btnSave;

		// Token: 0x04004DFB RID: 19963
		[SerializeField]
		private CButton btnImport;

		// Token: 0x04004DFC RID: 19964
		[SerializeField]
		private CButton btnExport;

		// Token: 0x04004DFD RID: 19965
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x04004DFE RID: 19966
		[SerializeField]
		private TMP_InputField searchField;

		// Token: 0x04004DFF RID: 19967
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x04004E00 RID: 19968
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x04004E01 RID: 19969
		[SerializeField]
		private ModEnableDependenceDialog modEnableDependenceDialog;

		// Token: 0x04004E02 RID: 19970
		[SerializeField]
		private GameObject noContent;

		// Token: 0x04004E03 RID: 19971
		[SerializeField]
		private CButton btnClearFilter;

		// Token: 0x04004E04 RID: 19972
		[SerializeField]
		private LoadingAnimation loadingAnimation;

		// Token: 0x04004E05 RID: 19973
		private ModSortAndFilterController _sortAndFilterController;

		// Token: 0x04004E06 RID: 19974
		private readonly HashSet<ModId> _missing = new HashSet<ModId>();

		// Token: 0x04004E07 RID: 19975
		private readonly List<ModId> _tempEnabledModIdList = new List<ModId>();

		// Token: 0x04004E08 RID: 19976
		private readonly List<string> _whiteListMods = new List<string>();

		// Token: 0x04004E09 RID: 19977
		private readonly Dictionary<ModId, int> _tempModOrder = new Dictionary<ModId, int>();

		// Token: 0x04004E0A RID: 19978
		private readonly List<ModSortAndFilterData> _allData = new List<ModSortAndFilterData>();

		// Token: 0x04004E0B RID: 19979
		private readonly List<ModSortAndFilterData> _filteredList = new List<ModSortAndFilterData>();

		// Token: 0x04004E0C RID: 19980
		private readonly Dictionary<ModId, int> _indexMap = new Dictionary<ModId, int>();

		// Token: 0x04004E0D RID: 19981
		private string _searchText;

		// Token: 0x04004E0E RID: 19982
		private bool _hasUpdateDetailInfo;

		// Token: 0x04004E0F RID: 19983
		private readonly List<ModId> _dependenciesChangedList = new List<ModId>();

		// Token: 0x04004E10 RID: 19984
		private readonly List<ModId> _mods = new List<ModId>();

		// Token: 0x04004E11 RID: 19985
		private ModInfoWithDisplayData _selectedWorkshopModInfo;

		// Token: 0x04004E12 RID: 19986
		private const string ModExportFile = "ModSettings{0}.Lua";
	}
}
