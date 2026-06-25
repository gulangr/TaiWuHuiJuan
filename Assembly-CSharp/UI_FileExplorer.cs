using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x0200037F RID: 895
public class UI_FileExplorer : UIBase
{
	// Token: 0x1700059E RID: 1438
	// (get) Token: 0x060034C5 RID: 13509 RVA: 0x001A592C File Offset: 0x001A3B2C
	private bool IsSave
	{
		get
		{
			return this._command.IsSaveAction;
		}
	}

	// Token: 0x060034C6 RID: 13510 RVA: 0x001A593C File Offset: 0x001A3B3C
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get<UI_FileExplorer.Command>("Cmd", out this._command);
		bool flag = this._drivers == null && this._command.FreePath;
		if (flag)
		{
			this.InitDriverGroups();
		}
		bool flag2 = this._command.DirectoryFilter == null;
		if (flag2)
		{
			this._command.DirectoryFilter = "*";
		}
		bool flag3 = this._command.FileFilter == null;
		if (flag3)
		{
			this._command.FileFilter = "*.*";
		}
		base.CGet<PopupWindow>("PopupWindowBase").SetTitle(this._command.Title);
		bool flag4 = string.IsNullOrEmpty(this._command.InitialPath);
		if (flag4)
		{
			this._command.InitialPath = Application.dataPath.PathFix();
		}
		bool flag5 = this._scrollDataList == null;
		if (flag5)
		{
			this._scrollDataList = new List<ValueTuple<string, sbyte>>();
		}
		else
		{
			this._scrollDataList.Clear();
		}
		bool flag6 = this._curPathList == null;
		if (flag6)
		{
			this._curPathList = new List<string>();
		}
		else
		{
			this._curPathList.Clear();
		}
	}

	// Token: 0x060034C7 RID: 13511 RVA: 0x001A5A58 File Offset: 0x001A3C58
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = "OpenInSystem" == btnName;
		if (flag)
		{
			this.OpenSystemFileExplorer();
		}
	}

	// Token: 0x060034C8 RID: 13512 RVA: 0x001A5A84 File Offset: 0x001A3C84
	private void Awake()
	{
		this._scroll = base.CGet<InfinityScrollLegacy>("ContentScroll");
		base.CGet<CToggleGroupObsolete>("DriverTogGroup").OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnDriverIndexChange);
		this._scroll.OnItemRender = new Action<int, Refers>(this.OnScrollItemRender);
		base.CGet<PopupWindow>("PopupWindowBase").OnConfirmClick = new Action(this.OnConfirm);
		base.CGet<PopupWindow>("PopupWindowBase").OnCancelClick = new Action(this.QuickHide);
	}

	// Token: 0x060034C9 RID: 13513 RVA: 0x001A5B10 File Offset: 0x001A3D10
	private void OnEnable()
	{
		base.CGet<GameObject>("HorizontalScrollView").SetActive(this._command.FreePath);
		base.CGet<TMP_InputField>("SelectingTarget").gameObject.SetActive(this.IsSave);
		this._scroll.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (this._command.FreePath ? this.SizeNormal : this.SizeNoDriver) + (this.IsSave ? this.SizeInput : 0f));
		this.UpdateCurrentPath();
		this.UpdateScrollData();
	}

	// Token: 0x060034CA RID: 13514 RVA: 0x001A5BA8 File Offset: 0x001A3DA8
	private void InitDriverGroups()
	{
		string[] dataArray = Environment.GetLogicalDrives();
		this._drivers = new string[dataArray.Length];
		CToggleGroupObsolete toggleGroup = base.CGet<CToggleGroupObsolete>("DriverTogGroup");
		CToggleObsolete[] allChildToggles = toggleGroup.transform.GetComponentsInTopChildren(true);
		CToggleObsolete prefabToggle = base.CGet<CToggleObsolete>("Tog");
		toggleGroup.Clear();
		for (int i = 0; i < dataArray.Length; i++)
		{
			this._drivers[i] = dataArray[i].Substring(0, dataArray[i].Length - 1);
			bool flag = allChildToggles.CheckIndex(i);
			CToggleObsolete toggle;
			if (flag)
			{
				toggle = allChildToggles[i];
			}
			else
			{
				toggle = Object.Instantiate<CToggleObsolete>(prefabToggle, toggleGroup.transform, false);
			}
			toggle.Key = i;
			toggle.gameObject.SetActive(true);
			toggleGroup.Add(toggle);
			TextMeshProUGUI[] labels = toggle.GetComponentsInChildren<TextMeshProUGUI>(true);
			foreach (TextMeshProUGUI label in labels)
			{
				label.text = this._drivers[i];
			}
		}
		for (int j = this._drivers.Length; j < allChildToggles.Length; j++)
		{
			Object.Destroy(allChildToggles[j].gameObject);
		}
	}

	// Token: 0x060034CB RID: 13515 RVA: 0x001A5CE8 File Offset: 0x001A3EE8
	private void UpdateCurrentPath()
	{
		string path = this._command.InitialPath;
		bool flag = this._curPathList.Count > 0;
		if (flag)
		{
			path = Path.Combine(path, string.Join("/", this._curPathList)).PathFix();
		}
		TextMeshProUGUI label = base.CGet<TextMeshProUGUI>("CurrentPath");
		label.text = path;
		label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(1000f, label.preferredWidth));
		label.rectTransform.anchoredPosition = Vector2.left * 20f;
	}

	// Token: 0x060034CC RID: 13516 RVA: 0x001A5D7C File Offset: 0x001A3F7C
	private void OnDriverIndexChange(CToggleObsolete newTog, CToggleObsolete preTog)
	{
		this._command.InitialPath = string.Empty;
		this._curPathList.Clear();
		this._curPathList.Add(this._drivers[newTog.Key]);
		this.UpdateCurrentPath();
		this.UpdateScrollData();
	}

	// Token: 0x060034CD RID: 13517 RVA: 0x001A5DCD File Offset: 0x001A3FCD
	private void EnterFolder(string folderName)
	{
		this._curPathList.Add(folderName);
		this._scroll.SelectedTogKey = -1;
		this.UpdateCurrentPath();
		this.UpdateScrollData();
	}

	// Token: 0x060034CE RID: 13518 RVA: 0x001A5DF8 File Offset: 0x001A3FF8
	private void ToPrevFolder()
	{
		this._curPathList.RemoveAt(this._curPathList.Count - 1);
		this._scroll.SelectedTogKey = -1;
		this.UpdateCurrentPath();
		this.UpdateScrollData();
	}

	// Token: 0x060034CF RID: 13519 RVA: 0x001A5E30 File Offset: 0x001A4030
	private void UpdateScrollData()
	{
		this._scrollDataList.Clear();
		bool hasPrevDir = (this._command.FreePath && this._curPathList.Count > 1) || (!this._command.InitialPath.IsNullOrEmpty() && this._curPathList.Count > 0);
		bool flag = hasPrevDir;
		if (flag)
		{
			this._scrollDataList.Add(new ValueTuple<string, sbyte>("...", 0));
		}
		string curDirectory = base.CGet<TextMeshProUGUI>("CurrentPath").text.FixPath();
		string[] topChildDirectories = Directory.GetDirectories(curDirectory, this._command.DirectoryFilter, SearchOption.TopDirectoryOnly);
		for (int i = 0; i < topChildDirectories.Length; i++)
		{
			string folderName = Path.GetFileName(topChildDirectories[i]);
			bool flag2 = folderName.StartsWith("$");
			if (!flag2)
			{
				this._scrollDataList.Add(new ValueTuple<string, sbyte>(folderName, 1));
			}
		}
		string[] files = Directory.GetFiles(curDirectory, this._command.FileFilter, SearchOption.TopDirectoryOnly);
		for (int j = 0; j < files.Length; j++)
		{
			string fileName = Path.GetFileName(files[j]);
			bool flag3 = fileName.StartsWith("$");
			if (!flag3)
			{
				this._scrollDataList.Add(new ValueTuple<string, sbyte>(fileName, 2));
			}
		}
		this._scroll.SetDataCount(this._scrollDataList.Count);
	}

	// Token: 0x060034D0 RID: 13520 RVA: 0x001A5FA0 File Offset: 0x001A41A0
	private void OnScrollItemRender(int index, Refers refers)
	{
		ValueTuple<string, sbyte> valueTuple = this._scrollDataList[index];
		string displayName = valueTuple.Item1;
		sbyte type = valueTuple.Item2;
		refers.CGet<TextMeshProUGUI>("Name").text = displayName;
		refers.CGet<GameObject>("FolderIcon").SetActive(type != 2);
		bool select = index == this._scroll.SelectedTogKey;
		refers.CGet<CImage>("BackSprite").color = (select ? this.ColorSelected : this.ColorNormal);
		PointClickBridge clickBridge = refers.CGet<PointClickBridge>("ClickBridge");
		clickBridge.OnLeftClick = delegate()
		{
			int prevIndex = this._scroll.SelectedTogKey;
			this._scroll.SelectedTogKey = index;
			bool flag = prevIndex > -1;
			if (flag)
			{
				this._scroll.RefreshCell(prevIndex);
			}
			refers.CGet<CImage>("BackSprite").color = this.ColorSelected;
			bool isSave = this.IsSave;
			if (isSave)
			{
				this.CGet<TMP_InputField>("SelectingTarget").text = displayName;
			}
		};
		clickBridge.OnDoubleClick = delegate()
		{
			bool flag = type == 1;
			if (flag)
			{
				this.EnterFolder(displayName);
			}
			else
			{
				bool flag2 = type == 2;
				if (flag2)
				{
					bool flag3 = !this._command.IsSaveAction;
					if (flag3)
					{
						bool flag4 = this._command.OnExploreComplete != null && this._command.OnExploreComplete(Path.Combine(this.CGet<TextMeshProUGUI>("CurrentPath").text, displayName).PathFix());
						if (flag4)
						{
							this.QuickHide();
						}
					}
				}
				else
				{
					bool flag5 = type == 0;
					if (flag5)
					{
						this.ToPrevFolder();
					}
				}
			}
		};
	}

	// Token: 0x060034D1 RID: 13521 RVA: 0x001A60A4 File Offset: 0x001A42A4
	private void OpenSystemFileExplorer()
	{
		string path = base.CGet<TextMeshProUGUI>("CurrentPath").text;
		Process.Start("explorer.exe", "\"" + path.FixPath() + "\"");
	}

	// Token: 0x060034D2 RID: 13522 RVA: 0x001A60E4 File Offset: 0x001A42E4
	private void OnConfirm()
	{
		int curIndex = this._scroll.SelectedTogKey;
		bool flag = -1 == curIndex;
		if (!flag)
		{
			string path = base.CGet<TextMeshProUGUI>("CurrentPath").text;
			string selectName = string.Empty;
			sbyte type = 2;
			bool flag2 = this._scrollDataList.CheckIndex(curIndex);
			if (flag2)
			{
				ValueTuple<string, sbyte> valueTuple = this._scrollDataList[curIndex];
				selectName = valueTuple.Item1;
				type = valueTuple.Item2;
			}
			bool flag3 = type == 0;
			if (!flag3)
			{
				bool flag4 = type == 1 && !this._command.CanSelectFolder;
				if (!flag4)
				{
					bool isSave = this.IsSave;
					if (isSave)
					{
						selectName = base.CGet<TMP_InputField>("SelectingTarget").text;
					}
					bool flag5 = string.IsNullOrEmpty(selectName);
					if (!flag5)
					{
						string selectPath = Path.Combine(path, selectName).PathFix();
						bool flag6 = this._command.OnExploreComplete != null && this._command.OnExploreComplete(selectPath);
						if (flag6)
						{
							this.QuickHide();
						}
					}
				}
			}
		}
	}

	// Token: 0x04002652 RID: 9810
	public const sbyte PrevDirectory = 0;

	// Token: 0x04002653 RID: 9811
	public const sbyte Folder = 1;

	// Token: 0x04002654 RID: 9812
	public const sbyte File = 2;

	// Token: 0x04002655 RID: 9813
	public float SizeNormal;

	// Token: 0x04002656 RID: 9814
	public float SizeNoDriver;

	// Token: 0x04002657 RID: 9815
	public Color ColorNormal;

	// Token: 0x04002658 RID: 9816
	public Color ColorSelected;

	// Token: 0x04002659 RID: 9817
	public float SizeInput;

	// Token: 0x0400265A RID: 9818
	private UI_FileExplorer.Command _command;

	// Token: 0x0400265B RID: 9819
	private string[] _drivers;

	// Token: 0x0400265C RID: 9820
	private List<string> _curPathList;

	// Token: 0x0400265D RID: 9821
	private List<ValueTuple<string, sbyte>> _scrollDataList;

	// Token: 0x0400265E RID: 9822
	private InfinityScrollLegacy _scroll;

	// Token: 0x02001781 RID: 6017
	public class Command
	{
		// Token: 0x0400ABD1 RID: 43985
		public string Title;

		// Token: 0x0400ABD2 RID: 43986
		public string InitialPath;

		// Token: 0x0400ABD3 RID: 43987
		public bool FreePath;

		// Token: 0x0400ABD4 RID: 43988
		public bool CanSelectFolder;

		// Token: 0x0400ABD5 RID: 43989
		public string FileFilter;

		// Token: 0x0400ABD6 RID: 43990
		public string DirectoryFilter;

		// Token: 0x0400ABD7 RID: 43991
		public Func<string, bool> OnExploreComplete;

		// Token: 0x0400ABD8 RID: 43992
		public bool IsSaveAction;
	}
}
