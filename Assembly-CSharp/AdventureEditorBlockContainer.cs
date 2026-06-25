using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using Game.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000171 RID: 369
public class AdventureEditorBlockContainer : MonoBehaviour
{
	// Token: 0x17000247 RID: 583
	// (get) Token: 0x06001472 RID: 5234 RVA: 0x0007F594 File Offset: 0x0007D794
	// (set) Token: 0x06001473 RID: 5235 RVA: 0x0007F59C File Offset: 0x0007D79C
	public bool IsDecorate { get; set; }

	// Token: 0x06001474 RID: 5236 RVA: 0x0007F5A8 File Offset: 0x0007D7A8
	private void Awake()
	{
		this.LoadArtLibrary();
		bool flag = this.searchInputField;
		if (flag)
		{
			this.searchInputField.onValueChanged.RemoveAllListeners();
			this.searchInputField.onValueChanged.AddListener(delegate(string _)
			{
				this.Refresh();
			});
		}
		bool flag2 = this.pathInputField;
		if (flag2)
		{
			this.pathInputField.onSelect.RemoveAllListeners();
			this.pathInputField.onSelect.AddListener(new UnityAction<string>(AdventureEditorBlockContainer.OnClickPath));
		}
		this.switchDecorate.onValueChanged.AddListener(delegate(bool isOn)
		{
			this.IsDecorate = isOn;
			this.Redirect();
			this.Refresh();
		});
	}

	// Token: 0x06001475 RID: 5237 RVA: 0x0007F658 File Offset: 0x0007D858
	private void OnEnable()
	{
		this.LoadArtLibrary();
		bool flag = this.searchInputField;
		if (flag)
		{
			this.searchInputField.SetTextWithoutNotify(string.Empty);
		}
		this.Refresh();
	}

	// Token: 0x06001476 RID: 5238 RVA: 0x0007F694 File Offset: 0x0007D894
	public void Bind(IAdventureEditorBlockElementHandler handler)
	{
		this._handler = handler;
	}

	// Token: 0x06001477 RID: 5239 RVA: 0x0007F69D File Offset: 0x0007D89D
	public void Redirect()
	{
		this._currentVirtualPathSegments = null;
		this.Refresh();
	}

	// Token: 0x06001478 RID: 5240 RVA: 0x0007F6AE File Offset: 0x0007D8AE
	public void ChangeVirtualDirectory(IReadOnlyList<string> virtualPathSegments)
	{
		this._currentVirtualPathSegments = ((virtualPathSegments == null) ? null : new List<string>(virtualPathSegments));
		this.Refresh();
	}

	// Token: 0x06001479 RID: 5241 RVA: 0x0007F6CA File Offset: 0x0007D8CA
	public void ForceRefresh()
	{
		this.Refresh();
	}

	// Token: 0x0600147A RID: 5242 RVA: 0x0007F6D4 File Offset: 0x0007D8D4
	public void ParentDirectory()
	{
		bool flag = this._currentVirtualPathSegments == null || this._currentVirtualPathSegments.Count == 0;
		if (!flag)
		{
			List<string> parentPath = AdventureEditorBlockContainer.GetParentVirtualPath(this._currentVirtualPathSegments);
			this.ChangeVirtualDirectory(parentPath);
		}
	}

	// Token: 0x0600147B RID: 5243 RVA: 0x0007F718 File Offset: 0x0007D918
	private void Refresh()
	{
		int index = 0;
		TMP_InputField tmp_InputField = this.searchInputField;
		string searchText = ((tmp_InputField != null) ? tmp_InputField.text : null) ?? string.Empty;
		List<AdventureArtIndex> rootIndexes = this.GetRootIndexes();
		AdventureArtIndex currentIndex = AdventureEditorBlockContainer.FindIndexByVirtualPathSegments(rootIndexes, this._currentVirtualPathSegments);
		bool flag = this.pathInputField;
		if (flag)
		{
			this.pathInputField.text = ((this._currentVirtualPathSegments == null || this._currentVirtualPathSegments.Count == 0) ? string.Empty : string.Join("/", this._currentVirtualPathSegments));
		}
		bool flag2 = !string.IsNullOrEmpty(searchText);
		if (flag2)
		{
			this.RefreshByArtLibrarySearch(ref index, rootIndexes, searchText);
			this.Clean(index);
		}
		else
		{
			List<string> currentVirtualPathSegments = this._currentVirtualPathSegments;
			bool flag3 = currentVirtualPathSegments != null && currentVirtualPathSegments.Count > 0;
			if (flag3)
			{
				AdventureEditorBlockElement parentElement = this.GetNextElement(ref index);
				parentElement.SetVirtualDirectory("(..)", AdventureEditorBlockContainer.GetParentVirtualPath(this._currentVirtualPathSegments), "{0}");
				parentElement.gameObject.SetActive(true);
			}
			List<AdventureArtIndex> subIndexes = (currentIndex != null) ? currentIndex.subIndexes : null;
			bool flag4 = subIndexes != null;
			if (flag4)
			{
				foreach (AdventureArtIndex sub in subIndexes)
				{
					AdventureEditorBlockElement element = this.GetNextElement(ref index);
					List<string> virtualPathSegments = AdventureEditorBlockContainer.CombineVirtualPathSegments(this._currentVirtualPathSegments, sub.name);
					element.SetVirtualDirectory(sub.name, virtualPathSegments, "");
					element.gameObject.SetActive(true);
				}
			}
			List<string> textures = (currentIndex != null) ? currentIndex.textureNames : null;
			bool flag5 = textures != null;
			if (flag5)
			{
				foreach (string texName in textures)
				{
					bool flag6 = !AdventureEditorBlockElement.IsValidBlockIcon(texName, this.IsDecorate);
					if (!flag6)
					{
						AdventureEditorBlockElement element2 = this.GetNextElement(ref index);
						element2.SetVirtualFile(texName);
						element2.gameObject.SetActive(true);
					}
				}
			}
			this.Clean(index);
		}
	}

	// Token: 0x0600147C RID: 5244 RVA: 0x0007F95C File Offset: 0x0007DB5C
	private AdventureEditorBlockElement GetNextElement(ref int index)
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
		AdventureEditorBlockElement element = child.GetComponent<AdventureEditorBlockElement>();
		element.Bind(this._handler);
		return element;
	}

	// Token: 0x0600147D RID: 5245 RVA: 0x0007F9E0 File Offset: 0x0007DBE0
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

	// Token: 0x0600147E RID: 5246 RVA: 0x0007FA30 File Offset: 0x0007DC30
	private static void OnClickPath(string path)
	{
		TextEditor te = new TextEditor
		{
			text = path
		};
		te.SelectAll();
		te.Copy();
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x0007FA5C File Offset: 0x0007DC5C
	public List<string> GetAllBlockIconNames()
	{
		List<string> result = new List<string>();
		this.CollectBlockIconNamesByArtLibrary(this.GetRootIndexes(), result);
		return result;
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x0007FA84 File Offset: 0x0007DC84
	private void CollectBlockIconNamesByArtLibrary(List<AdventureArtIndex> roots, List<string> result)
	{
		bool flag = roots == null;
		if (!flag)
		{
			foreach (AdventureArtIndex root in roots)
			{
				this.CollectBlockIconNamesByArtLibrary(root, result);
			}
		}
	}

	// Token: 0x06001481 RID: 5249 RVA: 0x0007FAE4 File Offset: 0x0007DCE4
	private void CollectBlockIconNamesByArtLibrary(AdventureArtIndex cur, List<string> result)
	{
		bool flag = cur == null;
		if (!flag)
		{
			bool flag2 = cur.textureNames != null;
			if (flag2)
			{
				foreach (string texName in cur.textureNames)
				{
					bool flag3 = AdventureEditorBlockElement.IsValidBlockIcon(texName, this.IsDecorate);
					if (flag3)
					{
						result.Add(texName);
					}
				}
			}
			bool flag4 = cur.subIndexes != null;
			if (flag4)
			{
				foreach (AdventureArtIndex sub in cur.subIndexes)
				{
					this.CollectBlockIconNamesByArtLibrary(sub, result);
				}
			}
		}
	}

	// Token: 0x06001482 RID: 5250 RVA: 0x0007FBC8 File Offset: 0x0007DDC8
	private void RefreshByArtLibrarySearch(ref int index, List<AdventureArtIndex> roots, string searchText)
	{
		bool flag = roots == null;
		if (!flag)
		{
			bool flag2 = string.IsNullOrEmpty(searchText);
			if (!flag2)
			{
				foreach (AdventureArtIndex root in roots)
				{
					List<string> list;
					if (!string.IsNullOrEmpty((root != null) ? root.name : null))
					{
						(list = new List<string>()).Add(root.name);
					}
					else
					{
						list = new List<string>();
					}
					List<string> rootPathSegments = list;
					this.SearchRecursivelyByArtLibrary(ref index, root, rootPathSegments, searchText);
				}
			}
		}
	}

	// Token: 0x06001483 RID: 5251 RVA: 0x0007FC64 File Offset: 0x0007DE64
	private void SearchRecursivelyByArtLibrary(ref int index, AdventureArtIndex cur, List<string> virtualPathSegments, string searchText)
	{
		bool flag = cur == null;
		if (!flag)
		{
			bool flag2 = AdventureEditorBlockContainer.ContainsIgnoreCase(cur.name, searchText);
			if (flag2)
			{
				AdventureEditorBlockElement element = this.GetNextElement(ref index);
				element.SetVirtualDirectory(cur.name, virtualPathSegments, "");
				element.gameObject.SetActive(true);
			}
			bool flag3 = cur.textureNames != null;
			if (flag3)
			{
				foreach (string texName in cur.textureNames)
				{
					bool flag4 = !AdventureEditorBlockElement.IsValidBlockIcon(texName, this.IsDecorate);
					if (!flag4)
					{
						string prefix = this.IsDecorate ? "adventure_decorate_" : "adventure_block_";
						bool flag5 = AdventureEditorBlockContainer.ContainsIgnoreCase(texName, searchText) || (texName.StartsWith(prefix) && AdventureEditorBlockContainer.ContainsIgnoreCase(texName.Substring(prefix.Length), searchText));
						if (flag5)
						{
							AdventureEditorBlockElement element2 = this.GetNextElement(ref index);
							element2.SetVirtualFile(texName);
							element2.gameObject.SetActive(true);
						}
					}
				}
			}
			bool flag6 = cur.subIndexes != null;
			if (flag6)
			{
				foreach (AdventureArtIndex sub in cur.subIndexes)
				{
					List<string> subPathSegments = AdventureEditorBlockContainer.CombineVirtualPathSegments(virtualPathSegments, sub.name);
					this.SearchRecursivelyByArtLibrary(ref index, sub, subPathSegments, searchText);
				}
			}
		}
	}

	// Token: 0x06001484 RID: 5252 RVA: 0x0007FE10 File Offset: 0x0007E010
	private void LoadArtLibrary()
	{
		bool flag = this._artLibrary != null;
		if (!flag)
		{
			this._artLibrary = ResLoader.SyncLoad<AdventureArtLibrary>("GameAtlas/AdventureArtLibrary");
		}
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x0007FE40 File Offset: 0x0007E040
	private List<AdventureArtIndex> GetRootIndexes()
	{
		List<AdventureArtIndex> result;
		if (!this.IsDecorate)
		{
			AdventureArtLibrary artLibrary = this._artLibrary;
			result = ((artLibrary != null) ? artLibrary.blocks : null);
		}
		else
		{
			AdventureArtLibrary artLibrary2 = this._artLibrary;
			result = ((artLibrary2 != null) ? artLibrary2.decorates : null);
		}
		return result;
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x0007FE80 File Offset: 0x0007E080
	private static List<string> CombineVirtualPathSegments(List<string> parentSegments, string name)
	{
		bool flag = parentSegments == null || parentSegments.Count == 0;
		List<string> result;
		if (flag)
		{
			result = new List<string>();
		}
		else
		{
			result = new List<string>(parentSegments);
		}
		bool flag2 = !string.IsNullOrEmpty(name);
		if (flag2)
		{
			result.Add(name);
		}
		return result;
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x0007FECC File Offset: 0x0007E0CC
	private static List<string> GetParentVirtualPath(List<string> virtualPathSegments)
	{
		bool flag = virtualPathSegments == null || virtualPathSegments.Count == 0;
		List<string> result2;
		if (flag)
		{
			result2 = null;
		}
		else
		{
			bool flag2 = virtualPathSegments.Count == 1;
			if (flag2)
			{
				result2 = new List<string>();
			}
			else
			{
				List<string> result = new List<string>(virtualPathSegments);
				result.RemoveAt(result.Count - 1);
				result2 = result;
			}
		}
		return result2;
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x0007FF24 File Offset: 0x0007E124
	private static AdventureArtIndex FindIndexByVirtualPathSegments(List<AdventureArtIndex> roots, List<string> virtualPathSegments)
	{
		bool flag = roots == null;
		AdventureArtIndex result;
		if (flag)
		{
			result = null;
		}
		else
		{
			bool flag2 = virtualPathSegments == null || virtualPathSegments.Count == 0;
			if (flag2)
			{
				result = new AdventureArtIndex
				{
					name = string.Empty,
					subIndexes = roots
				};
			}
			else
			{
				List<AdventureArtIndex> curList = roots;
				AdventureArtIndex cur = null;
				using (List<string>.Enumerator enumerator = virtualPathSegments.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string part = enumerator.Current;
						cur = ((curList != null) ? curList.Find((AdventureArtIndex x) => x != null && x.name == part) : null);
						bool flag3 = cur == null;
						if (flag3)
						{
							return null;
						}
						curList = cur.subIndexes;
					}
				}
				result = cur;
			}
		}
		return result;
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x0007FFF4 File Offset: 0x0007E1F4
	private static bool ContainsIgnoreCase(string source, string value)
	{
		bool flag = string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value);
		return !flag && source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
	}

	// Token: 0x04001129 RID: 4393
	[SerializeField]
	private RectTransform content;

	// Token: 0x0400112A RID: 4394
	[SerializeField]
	private TMP_InputField searchInputField;

	// Token: 0x0400112B RID: 4395
	[SerializeField]
	private TMP_InputField pathInputField;

	// Token: 0x0400112C RID: 4396
	[SerializeField]
	private CToggle switchDecorate;

	// Token: 0x0400112D RID: 4397
	private AdventureArtLibrary _artLibrary;

	// Token: 0x0400112E RID: 4398
	private List<string> _currentVirtualPathSegments;

	// Token: 0x0400112F RID: 4399
	private IAdventureEditorBlockElementHandler _handler;
}
