using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using EventEditor;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000177 RID: 375
public class AdventureEditorElementArea : MonoBehaviour, IAdventureEditorElementHandler
{
	// Token: 0x060014C2 RID: 5314 RVA: 0x00080A96 File Offset: 0x0007EC96
	private void Start()
	{
		this.info.gameObject.SetActive(true);
	}

	// Token: 0x060014C3 RID: 5315 RVA: 0x00080AAC File Offset: 0x0007ECAC
	private void OnApplicationFocus(bool hasFocus)
	{
		bool flag = !hasFocus;
		if (!flag)
		{
			this.ValidateAndRedirectDirectoryIfInvalid();
			bool flag2 = this.container != null;
			if (flag2)
			{
				this.container.ForceRefresh(-1);
			}
		}
	}

	// Token: 0x060014C4 RID: 5316 RVA: 0x00080AE8 File Offset: 0x0007ECE8
	private void ValidateAndRedirectDirectoryIfInvalid()
	{
		bool flag = this.container == null;
		if (!flag)
		{
			DirectoryInfo currentDir = this.container.CurrentDirectory;
			bool flag2 = currentDir == null || new DirectoryInfo(currentDir.FullName).Exists;
			if (!flag2)
			{
				string invalidPath = currentDir.FullName;
				string ancestorPath = invalidPath;
				while (!string.IsNullOrEmpty(ancestorPath))
				{
					bool flag3 = Directory.Exists(ancestorPath);
					if (flag3)
					{
						break;
					}
					ancestorPath = Path.GetDirectoryName(ancestorPath);
				}
				bool flag4 = string.IsNullOrEmpty(ancestorPath);
				if (flag4)
				{
					ancestorPath = AdventureEditorKit.ElementDirectory;
					bool flag5 = !Directory.Exists(ancestorPath);
					if (flag5)
					{
						AdaptableLog.Error("[AdventureEditor] Root directory " + ancestorPath + " not found!");
						return;
					}
				}
				DirectoryInfo finalDir = new DirectoryInfo(ancestorPath);
				DialogCmd cmd = new DialogCmd
				{
					Title = LanguageKey.LK_AdventureEditor_Dialog_Title_Tip.Tr(),
					Content = LanguageKey.LK_AdventureEditor_Dialog_AutoGotoPath.TrFormat(invalidPath, ancestorPath),
					Yes = delegate()
					{
						this.container.ChangeDirectory(finalDir);
					},
					Type = 2
				};
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.SetObject("Cmd", cmd);
				UIElement.Dialog.SetOnInitArgs(box);
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x00080C3C File Offset: 0x0007EE3C
	private void Awake()
	{
		EventEditorScript.Init(this.editorScript);
		this.container.Bind(this);
		this.container.ForceRefresh(-1);
		this.info.Clear();
		AdventureEditorElementInfo adventureEditorElementInfo = this.info;
		if (adventureEditorElementInfo.onElementContainerRefresh == null)
		{
			adventureEditorElementInfo.onElementContainerRefresh = new UnityEvent<int>();
		}
		this.info.onElementContainerRefresh.AddListener(delegate(int id)
		{
			this.container.ForceRefresh(id);
		});
		this.adventureContextMenuBack.ClearAndAddListener(new Action(this.HideContextMenu));
	}

	// Token: 0x060014C6 RID: 5318 RVA: 0x00080CCB File Offset: 0x0007EECB
	private void OnEnable()
	{
		this.HideContextMenu();
	}

	// Token: 0x060014C7 RID: 5319 RVA: 0x00080CD5 File Offset: 0x0007EED5
	public void RedirectAndClear()
	{
		this.container.Redirect();
		this.info.Clear();
	}

	// Token: 0x060014C8 RID: 5320 RVA: 0x00080CF0 File Offset: 0x0007EEF0
	public void OnClick(FileInfo file, DirectoryInfo directory)
	{
		this.HideContextMenu();
		bool flag = file != null && file.Exists;
		if (flag)
		{
			this.SwitchElementWithConfirm(file, null, delegate
			{
				DirectoryInfo currentDirectory = this.container.CurrentDirectory;
				string a = (currentDirectory != null) ? currentDirectory.FullName : null;
				DirectoryInfo directory2 = file.Directory;
				bool flag3 = a != ((directory2 != null) ? directory2.FullName : null);
				if (flag3)
				{
					this.container.ChangeDirectory(file.Directory);
				}
				this.info.Set(file);
			});
		}
		bool flag2 = directory != null && directory.Exists;
		if (flag2)
		{
			this.SwitchElementWithConfirm(null, directory, delegate
			{
				this.info.Set(directory);
			});
		}
	}

	// Token: 0x060014C9 RID: 5321 RVA: 0x00080D8C File Offset: 0x0007EF8C
	public void OnDoubleClick(FileInfo file, DirectoryInfo directory)
	{
		this.HideContextMenu();
		bool flag = directory == null;
		if (!flag)
		{
			this.container.ChangeDirectory(directory);
			this.info.Clear();
		}
	}

	// Token: 0x060014CA RID: 5322 RVA: 0x00080DC4 File Offset: 0x0007EFC4
	void IAdventureEditorElementHandler.OnRightClick(FileInfo file, DirectoryInfo directory)
	{
		AdventureEditorElementArea.RightClickMenuProcess(file, directory, this.adventureContextMenu, this.adventureContextMenuBack.gameObject, null, null);
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x00080DE2 File Offset: 0x0007EFE2
	void IAdventureEditorElementHandler.OnSelectEmpty()
	{
		this.HideContextMenu();
	}

	// Token: 0x060014CC RID: 5324 RVA: 0x00080DEC File Offset: 0x0007EFEC
	public IAdventureEditorElementHandler.DragPostProcess OnDragValidate(GameObject dropTarget, FileInfo file, DirectoryInfo directory)
	{
		this.HideContextMenu();
		bool flag = ((dropTarget != null) ? dropTarget.GetComponent<CScrollRectLegacy>() : null) != null;
		IAdventureEditorElementHandler.DragPostProcess result;
		if (flag)
		{
			result = IAdventureEditorElementHandler.DragPostProcess.Failed;
		}
		else
		{
			AdventureEditorElement targetElement = (dropTarget != null) ? dropTarget.GetComponentInChildren<AdventureEditorElement>() : null;
			bool flag2;
			if (targetElement != null)
			{
				DirectoryInfo directory2 = targetElement.Directory;
				if (directory2 != null)
				{
					flag2 = directory2.Exists;
					goto IL_4B;
				}
			}
			flag2 = false;
			IL_4B:
			bool flag3 = flag2;
			if (flag3)
			{
				bool flag4 = file != null && file.Exists && file.DirectoryName != targetElement.Directory.FullName;
				if (flag4)
				{
					try
					{
						File.Move(file.FullName, Path.Combine(targetElement.Directory.FullName, file.Name));
						return IAdventureEditorElementHandler.DragPostProcess.Delete;
					}
					catch (Exception e)
					{
						AdaptableLog.Warning(e.Message, false);
					}
				}
			}
			result = IAdventureEditorElementHandler.DragPostProcess.Failed;
		}
		return result;
	}

	// Token: 0x060014CD RID: 5325 RVA: 0x00080EC8 File Offset: 0x0007F0C8
	private void HideContextMenu()
	{
		bool activeSelf = this.adventureContextMenu.gameObject.activeSelf;
		if (activeSelf)
		{
			this.adventureContextMenu.gameObject.SetActive(false);
		}
		this.adventureContextMenuBack.gameObject.SetActive(false);
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x00080F10 File Offset: 0x0007F110
	internal static void RightClickMenuProcess(FileInfo file, DirectoryInfo directory, AdventureContextMenu contextMenu, GameObject contextMenuBack, AdventureEditorElementInfo elementInfoEditor, GameObject elementInfoPanel)
	{
		AdventureEditorElementArea.<>c__DisplayClass18_0 CS$<>8__locals1 = new AdventureEditorElementArea.<>c__DisplayClass18_0();
		CS$<>8__locals1.elementInfoEditor = elementInfoEditor;
		CS$<>8__locals1.file = file;
		CS$<>8__locals1.elementInfoPanel = elementInfoPanel;
		AdventureEditorElementArea.<>c__DisplayClass18_0 CS$<>8__locals2 = CS$<>8__locals1;
		string fullPath;
		if ((fullPath = ((directory != null) ? directory.FullName : null)) == null)
		{
			FileInfo file2 = CS$<>8__locals1.file;
			string text;
			if (file2 == null)
			{
				text = null;
			}
			else
			{
				DirectoryInfo directory2 = file2.Directory;
				text = ((directory2 != null) ? directory2.FullName : null);
			}
			fullPath = (text ?? string.Empty);
		}
		CS$<>8__locals2.fullPath = fullPath;
		bool flag = string.IsNullOrEmpty(CS$<>8__locals1.fullPath);
		if (!flag)
		{
			List<AdventureContextMenuItemData> items = new List<AdventureContextMenuItemData>();
			AdventureContextMenuItemData openExplorer = new AdventureContextMenuItemData
			{
				ItemName = LanguageKey.LK_Adventure_CtxMenu_Inspect_Via_Explorer.Tr(),
				ItemAction = delegate()
				{
					AdventureEditorElementArea.OpenExplorer(CS$<>8__locals1.fullPath);
				},
				GroupIndex = 0
			};
			items.Add(openExplorer);
			bool flag2 = CS$<>8__locals1.file != null && CS$<>8__locals1.file.Exists && CS$<>8__locals1.elementInfoEditor && CS$<>8__locals1.elementInfoPanel;
			if (flag2)
			{
				items.Add(new AdventureContextMenuItemData
				{
					ItemName = LanguageKey.LK_Adventure_CtxMenu_Properties.Tr(),
					ItemAction = delegate()
					{
						CS$<>8__locals1.elementInfoEditor.Set(CS$<>8__locals1.file);
						CS$<>8__locals1.elementInfoPanel.gameObject.SetActive(true);
					},
					GroupIndex = 1
				});
			}
			contextMenu.RefreshFirstLevelContextMenu(items);
			if (contextMenuBack != null)
			{
				contextMenuBack.SetActive(true);
			}
		}
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x0008104B File Offset: 0x0007F24B
	private static void OpenExplorer(string path)
	{
		Process.Start("Explorer.exe", path);
	}

	// Token: 0x060014D0 RID: 5328 RVA: 0x0008105C File Offset: 0x0007F25C
	private void SwitchElementWithConfirm(FileInfo targetFile, DirectoryInfo targetDirectory, Action switchAction)
	{
		bool flag = switchAction == null;
		if (!flag)
		{
			bool flag2 = !this.info.HasEditingTarget() || !this.info.CheckModified() || this.info.IsEditingTarget(targetFile, targetDirectory);
			if (flag2)
			{
				switchAction();
			}
			else
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LanguageKey.LK_AdventureEditor_Dialog_SwitchElement_Title.Tr(),
					Content = LanguageKey.LK_AdventureEditor_Dialog_SwitchElement_Content.Tr(),
					Yes = switchAction,
					No = null
				};
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.SetObject("Cmd", cmd);
				UIElement.Dialog.SetOnInitArgs(box);
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}
	}

	// Token: 0x04001155 RID: 4437
	[SerializeField]
	private AdventureEditorElementContainer container;

	// Token: 0x04001156 RID: 4438
	[SerializeField]
	private AdventureEditorElementInfo info;

	// Token: 0x04001157 RID: 4439
	[SerializeField]
	private EventEditorScript editorScript;

	// Token: 0x04001158 RID: 4440
	[SerializeField]
	private Toggle toggleNoDisplayDirectory;

	// Token: 0x04001159 RID: 4441
	[SerializeField]
	private CButton adventureContextMenuBack;

	// Token: 0x0400115A RID: 4442
	[SerializeField]
	private AdventureContextMenu adventureContextMenu;
}
