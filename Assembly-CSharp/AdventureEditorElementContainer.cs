using System;
using System.IO;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000178 RID: 376
public class AdventureEditorElementContainer : MonoBehaviour
{
	// Token: 0x17000254 RID: 596
	// (get) Token: 0x060014D3 RID: 5331 RVA: 0x0008112C File Offset: 0x0007F32C
	private bool CurrentIsValid
	{
		get
		{
			DirectoryInfo currentDirectory = this.CurrentDirectory;
			return currentDirectory != null && currentDirectory.Exists;
		}
	}

	// Token: 0x17000255 RID: 597
	// (get) Token: 0x060014D4 RID: 5332 RVA: 0x0008114C File Offset: 0x0007F34C
	// (set) Token: 0x060014D5 RID: 5333 RVA: 0x00081154 File Offset: 0x0007F354
	public DirectoryInfo CurrentDirectory { get; private set; }

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x060014D6 RID: 5334 RVA: 0x0008115D File Offset: 0x0007F35D
	// (set) Token: 0x060014D7 RID: 5335 RVA: 0x00081165 File Offset: 0x0007F365
	public bool NotDisplayDirectory
	{
		get
		{
			return this._notDisplayDirectory;
		}
		set
		{
			this._notDisplayDirectory = value;
			this.Refresh(-1);
		}
	}

	// Token: 0x060014D8 RID: 5336 RVA: 0x00081178 File Offset: 0x0007F378
	private void Awake()
	{
		this.searchInputField.onValueChanged.RemoveAllListeners();
		this.searchInputField.onValueChanged.AddListener(delegate(string _)
		{
			this.Refresh(-1);
		});
		this.pathInputField.onSelect.RemoveAllListeners();
		this.pathInputField.onSelect.AddListener(new UnityAction<string>(AdventureEditorElementContainer.OnClickPath));
	}

	// Token: 0x060014D9 RID: 5337 RVA: 0x000811E2 File Offset: 0x0007F3E2
	private void OnEnable()
	{
		this.searchInputField.SetTextWithoutNotify(string.Empty);
		this.Refresh(-1);
	}

	// Token: 0x060014DA RID: 5338 RVA: 0x000811FE File Offset: 0x0007F3FE
	public void Bind(IAdventureEditorElementHandler handler)
	{
		this._handler = handler;
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x00081207 File Offset: 0x0007F407
	public void Redirect()
	{
		this.CurrentDirectory = null;
		this.Refresh(-1);
	}

	// Token: 0x060014DC RID: 5340 RVA: 0x0008121A File Offset: 0x0007F41A
	public void ChangeDirectory(DirectoryInfo directory)
	{
		this.CurrentDirectory = directory;
		this.Refresh(-1);
	}

	// Token: 0x060014DD RID: 5341 RVA: 0x0008122D File Offset: 0x0007F42D
	public void ForceRefresh(int id = -1)
	{
		this.Refresh(id);
	}

	// Token: 0x060014DE RID: 5342 RVA: 0x00081238 File Offset: 0x0007F438
	public void CreateFile()
	{
		bool flag = !this.CurrentIsValid;
		if (!flag)
		{
			string path = AdventureEditorKit.GetNewElementFilePath(this.CurrentDirectory.FullName);
			int id = AdventureEditorKit.CreateNewElement(path);
			this.Refresh(id);
		}
	}

	// Token: 0x060014DF RID: 5343 RVA: 0x00081278 File Offset: 0x0007F478
	public void CreateDirectory()
	{
		bool flag = !this.CurrentIsValid;
		if (!flag)
		{
			string path = AdventureEditorKit.GetNewElementDirectoryPath(this.CurrentDirectory.FullName);
			Directory.CreateDirectory(path);
			this.Refresh(-1);
		}
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x000812B8 File Offset: 0x0007F4B8
	public void ParentDirectory()
	{
		bool flag = !this.CurrentIsValid;
		if (!flag)
		{
			bool flag2 = this.CurrentDirectory.FullName == AdventureEditorKit.ElementDirectory;
			if (!flag2)
			{
				this.CurrentDirectory = this.CurrentDirectory.Parent;
				this.Refresh(-1);
			}
		}
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x0008130C File Offset: 0x0007F50C
	private void Refresh(int selectingElementId = -1)
	{
		bool flag = this.CurrentDirectory == null && !string.IsNullOrEmpty(AdventureEditorKit.ElementDirectory);
		if (flag)
		{
			this.CurrentDirectory = new DirectoryInfo(AdventureEditorKit.ElementDirectory);
		}
		DirectoryInfo currentDirectory = this.CurrentDirectory;
		bool flag2 = currentDirectory == null || !currentDirectory.Exists;
		if (flag2)
		{
			this.Clean(0);
		}
		else
		{
			bool flag3 = this.searchInputField.text.IsNullOrEmpty();
			if (flag3)
			{
				int index = 0;
				bool flag4 = this.CurrentDirectory.FullName != AdventureEditorKit.ElementDirectory;
				if (flag4)
				{
					DirectoryInfo directory = this.CurrentDirectory.Parent;
					AdventureEditorElement element = this.GetNextElement(ref index);
					element.Set(directory, "(..)");
					element.gameObject.SetActive(!this._notDisplayDirectory);
				}
				foreach (DirectoryInfo directory2 in this.CurrentDirectory.GetDirectories())
				{
					AdventureEditorElement element2 = this.GetNextElement(ref index);
					element2.Set(directory2, "");
					element2.gameObject.SetActive(!this._notDisplayDirectory);
				}
				foreach (FileInfo file in this.CurrentDirectory.GetFiles())
				{
					AdventureElementSnapshot data;
					bool flag5 = AdventureElementSnapshot.TryLoadFromFile(file.FullName, out data);
					if (flag5)
					{
						AdventureEditorElement element3 = this.GetNextElement(ref index);
						element3.Set(file, data);
						bool flag6 = selectingElementId == data.Id;
						if (flag6)
						{
							element3.OnClick();
						}
					}
				}
				this.Clean(index);
			}
			else
			{
				int index2 = 0;
				this.GetNextElementOnSearch(ref index2, this.CurrentDirectory, this.searchInputField.text);
				this.Clean(index2);
			}
		}
		TMP_InputField tmp_InputField = this.pathInputField;
		string text;
		if (!AdventureEditorKit.ElementDirectory.IsNullOrEmpty())
		{
			DirectoryInfo currentDirectory2 = this.CurrentDirectory;
			text = ((currentDirectory2 != null) ? currentDirectory2.FullName.Replace(AdventureEditorKit.ElementDirectory, string.Empty) : null);
		}
		else
		{
			text = string.Empty;
		}
		tmp_InputField.text = text;
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x00081520 File Offset: 0x0007F720
	private void GetNextElementOnSearch(ref int index, DirectoryInfo directoryInfo, string searchText)
	{
		foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
		{
			bool flag = directory.Name.Contains(searchText);
			if (flag)
			{
				this.GetNextElement(ref index).Set(directory, "");
			}
			this.GetNextElementOnSearch(ref index, directory, searchText);
		}
		foreach (FileInfo file in directoryInfo.GetFiles())
		{
			AdventureElementSnapshot data;
			bool flag2 = !AdventureElementSnapshot.TryLoadFromFile(file.FullName, out data);
			if (!flag2)
			{
				string value = data.Name.Value;
				bool nameSearched = value != null && value.Contains(searchText);
				string value2 = data.Desc.Value;
				bool descSearched = value2 != null && value2.Contains(searchText);
				bool flag3 = nameSearched || descSearched;
				if (flag3)
				{
					this.GetNextElement(ref index).Set(file, data);
				}
			}
		}
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x0008160C File Offset: 0x0007F80C
	private AdventureEditorElement GetNextElement(ref int index)
	{
		bool flag = index == this.content.childCount;
		if (flag)
		{
			Object.Instantiate<Transform>(this.content.GetChild(0), this.content);
		}
		GameObject child = this.content.GetChild(index).gameObject;
		index++;
		bool flag2 = !child.activeSelf;
		if (flag2)
		{
			child.SetActive(true);
		}
		AdventureEditorElement element = child.GetComponent<AdventureEditorElement>();
		element.Bind(this._handler);
		return element;
	}

	// Token: 0x060014E4 RID: 5348 RVA: 0x00081690 File Offset: 0x0007F890
	private void Clean(int begin = 0)
	{
		for (int i = begin; i < this.content.childCount; i++)
		{
			GameObject child = this.content.GetChild(i).gameObject;
			bool activeSelf = child.activeSelf;
			if (activeSelf)
			{
				child.SetActive(false);
			}
		}
	}

	// Token: 0x060014E5 RID: 5349 RVA: 0x000816E0 File Offset: 0x0007F8E0
	private static void OnClickPath(string path)
	{
		TextEditor te = new TextEditor
		{
			text = path
		};
		te.SelectAll();
		te.Copy();
	}

	// Token: 0x0400115B RID: 4443
	[SerializeField]
	private RectTransform content;

	// Token: 0x0400115C RID: 4444
	[SerializeField]
	private TMP_InputField searchInputField;

	// Token: 0x0400115D RID: 4445
	[SerializeField]
	private TMP_InputField pathInputField;

	// Token: 0x0400115F RID: 4447
	private bool _notDisplayDirectory;

	// Token: 0x04001160 RID: 4448
	private IAdventureEditorElementHandler _handler;
}
