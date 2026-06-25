using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CommonSortAndFilterLegacy;
using EasyButtons;
using FrameWork.UI.LanguageRule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000334 RID: 820
public class CommonTableHead : Refers, ILanguage
{
	// Token: 0x17000538 RID: 1336
	// (get) Token: 0x06002F6C RID: 12140 RVA: 0x00173BDF File Offset: 0x00171DDF
	public Sprite[] LastSprites
	{
		get
		{
			return this.lastSprites;
		}
	}

	// Token: 0x17000539 RID: 1337
	// (get) Token: 0x06002F6D RID: 12141 RVA: 0x00173BE7 File Offset: 0x00171DE7
	public Sprite[] NormalSprites
	{
		get
		{
			return this.normalSprites;
		}
	}

	// Token: 0x06002F6E RID: 12142 RVA: 0x00173BF0 File Offset: 0x00171DF0
	public float[] RefreshWidth()
	{
		bool flag = this.columnWidthArray == null || this.isColumnDisplaying == null;
		float[] result;
		if (flag)
		{
			result = null;
		}
		else
		{
			this.adaptableWidth = (this.totalWidth - this.columnWidthArray.Where((float x, int i) => this.isColumnDisplaying[i] && x > 0f).Sum()) / Math.Min(this.columnWidthArray.Where((float x, int i) => this.isColumnDisplaying[i] && x < 0f).Sum(), -0.01f);
			result = this.columnWidthArray.Select(delegate(float f, int i)
			{
				Transform child = this.GetTableHeadChild(i, false);
				child.gameObject.SetActive(this.isColumnDisplaying[i]);
				float width = this.isColumnDisplaying[i] ? this.GetWidth(i) : 0f;
				RectTransform component = child.GetComponent<RectTransform>();
				if (component != null)
				{
					component.SetWidth(width);
				}
				return width;
			}).ToArray<float>();
		}
		return result;
	}

	// Token: 0x06002F6F RID: 12143 RVA: 0x00173C8C File Offset: 0x00171E8C
	public void RefreshTitle(List<LanguageKey> languageKeys)
	{
		int count = this.GetContentChildCount(false);
		List<Refers> referList = new List<Refers>();
		for (int i = 0; i < count; i++)
		{
			Transform head = this.GetTableHeadChild(i, false);
			Refers refers = head.GetComponent<Refers>();
			bool flag = !refers;
			if (!flag)
			{
				referList.Add(refers);
			}
		}
		for (int index = 0; index < referList.Count; index++)
		{
			Refers refers2 = referList[index];
			TextMeshProUGUI text;
			bool flag2 = refers2.CTryGet<TextMeshProUGUI>("Label", out text) && languageKeys.CheckIndex(index);
			if (flag2)
			{
				text.text = languageKeys[index].Tr();
			}
		}
	}

	// Token: 0x06002F70 RID: 12144 RVA: 0x00173D48 File Offset: 0x00171F48
	private bool ValidateItemTemplate()
	{
		Debug.Assert(this.itemTemplate != null, "itemTemplate 未赋值");
		return this.itemTemplate != null;
	}

	// Token: 0x06002F71 RID: 12145 RVA: 0x00173D80 File Offset: 0x00171F80
	private void InitializeItemLanguageRules()
	{
		bool flag = this._isPointerTriggersInitialized || !Application.isPlaying;
		if (!flag)
		{
			HorizontalLayoutGroup horizontalLayoutGroup = base.GetComponent<HorizontalLayoutGroup>();
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				LanguageRuleExpandOnHover rule = child.GetComponent<LanguageRuleExpandOnHover>();
				bool flag2 = rule == null;
				if (!flag2)
				{
					GameObject childGameObject = child.gameObject;
					rule.EnterEvent.AddListener(delegate()
					{
						this.OnItemHoverEnter(childGameObject);
					});
					rule.ExitEvent.AddListener(delegate()
					{
						this.OnItemHoverExit(childGameObject);
					});
					rule.AddLayoutToDisable(horizontalLayoutGroup);
				}
			}
			this._isPointerTriggersInitialized = true;
		}
	}

	// Token: 0x06002F72 RID: 12146 RVA: 0x00173E58 File Offset: 0x00172058
	private void Start()
	{
		bool isPlaying = Application.isPlaying;
		if (isPlaying)
		{
			this.InitializeItemLanguageRules();
		}
	}

	// Token: 0x06002F73 RID: 12147 RVA: 0x00173E78 File Offset: 0x00172078
	private void OnEnable()
	{
		bool isPlaying = Application.isPlaying;
		if (isPlaying)
		{
			this.InitializeItemLanguageRules();
		}
	}

	// Token: 0x06002F74 RID: 12148 RVA: 0x00173E98 File Offset: 0x00172098
	private void CreateAndConfigureChild(int index)
	{
		GameObject elem = this.InstantiateChild(index);
		bool flag = elem == null;
		if (!flag)
		{
			RectTransform rt = elem.GetComponent<RectTransform>();
			bool flag2 = rt != null;
			if (flag2)
			{
				rt.SetWidth(this.GetWidth(index));
			}
		}
	}

	// Token: 0x06002F75 RID: 12149 RVA: 0x00173EE0 File Offset: 0x001720E0
	[Button("清空", LabelColor = "#FF0000")]
	private void ClearObject()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Transform child = base.transform.GetChild(i);
			Object.DestroyImmediate(child.gameObject);
		}
	}

	// Token: 0x06002F76 RID: 12150 RVA: 0x00173F28 File Offset: 0x00172128
	[Button("根据列宽生成（将先删除所有子物体，再生成）")]
	private void CreateObject()
	{
		bool flag = this.totalWidth > 0f;
		if (flag)
		{
			base.RectTransform.SetWidth(this.totalWidth);
		}
		bool flag2 = !this.ValidateItemTemplate();
		if (!flag2)
		{
			this.ClearObject();
			bool flag3 = this.columnWidthArray == null;
			if (!flag3)
			{
				for (int i = 0; i < this.columnWidthArray.Length; i++)
				{
					this.CreateAndConfigureChild(i);
				}
				bool isPlaying = Application.isPlaying;
				if (isPlaying)
				{
					this._isPointerTriggersInitialized = false;
					this.InitializeItemLanguageRules();
				}
			}
		}
	}

	// Token: 0x06002F77 RID: 12151 RVA: 0x00173FBC File Offset: 0x001721BC
	private GameObject InstantiateChild(int index)
	{
		GameObject elem = Object.Instantiate<GameObject>(this.itemTemplate, base.transform);
		bool flag = elem == null;
		GameObject result;
		if (flag)
		{
			Debug.LogError("Failed to instantiate itemTemplate");
			result = null;
		}
		else
		{
			elem.name = string.Format("Head_{0}", index);
			elem.SetActive(true);
			result = elem;
		}
		return result;
	}

	// Token: 0x06002F78 RID: 12152 RVA: 0x0017401C File Offset: 0x0017221C
	[Button("根据列宽刷新（将刷新现有的元素的宽度，并且只会增加不会删除）")]
	private void RefreshObject()
	{
		bool flag = this.totalWidth > 0f;
		if (flag)
		{
			base.RectTransform.SetWidth(this.totalWidth);
		}
		bool flag2 = !this.ValidateItemTemplate();
		if (!flag2)
		{
			bool flag3 = this.columnWidthArray == null;
			if (!flag3)
			{
				int currentCount = this.GetContentChildCount(false);
				int targetCount = this.columnWidthArray.Length;
				for (int i = currentCount; i < targetCount; i++)
				{
					this.CreateAndConfigureChild(i);
				}
				for (int j = 0; j < targetCount; j++)
				{
					Transform child = this.GetTableHeadChild(j, false);
					bool flag4 = child == null;
					if (!flag4)
					{
						child.name = string.Format("Head_{0}", j);
						RectTransform rt = child.GetComponent<RectTransform>();
						bool flag5 = rt != null;
						if (flag5)
						{
							rt.SetWidth(this.GetWidth(j));
						}
					}
				}
				bool isPlaying = Application.isPlaying;
				if (isPlaying)
				{
					this.InitializeItemLanguageRules();
				}
			}
		}
	}

	// Token: 0x06002F79 RID: 12153 RVA: 0x00174130 File Offset: 0x00172330
	public void HideAllArrowAndSequence()
	{
		for (int i = 0; i < this.GetContentChildCount(false); i++)
		{
			this.SetArrow(i, false, false);
			this.SetSequence(i, false, -1);
		}
	}

	// Token: 0x06002F7A RID: 12154 RVA: 0x0017416C File Offset: 0x0017236C
	public void SetArrow(int index, bool active, bool isUp)
	{
		Transform item = this.GetTableHeadChild(index, true);
		bool flag = item == null;
		if (!flag)
		{
			Refers childRefers = item.GetComponent<Refers>();
			this.SetArrow(childRefers, active, isUp);
		}
	}

	// Token: 0x06002F7B RID: 12155 RVA: 0x001741A4 File Offset: 0x001723A4
	public void SetArrow(Refers childRefers, bool active, bool isUp)
	{
		bool flag = !this.IsValidChildRefers(childRefers);
		if (!flag)
		{
			GameObject arrow = childRefers.CGet<GameObject>("Arrow");
			bool flag2 = arrow == null;
			if (!flag2)
			{
				arrow.SetActive(active);
				if (active)
				{
					RectTransform rt = arrow.GetComponent<RectTransform>();
					bool flag3 = rt != null;
					if (flag3)
					{
						rt.localEulerAngles = new Vector3(0f, 0f, isUp ? 0f : 180f);
					}
				}
			}
		}
	}

	// Token: 0x06002F7C RID: 12156 RVA: 0x00174228 File Offset: 0x00172428
	public void SetSequence(int index, bool active, int number)
	{
		Transform item = this.GetTableHeadChild(index, true);
		bool flag = item == null;
		if (!flag)
		{
			Refers childRefers = item.GetComponent<Refers>();
			this.SetSequence(childRefers, active, number);
		}
	}

	// Token: 0x06002F7D RID: 12157 RVA: 0x00174260 File Offset: 0x00172460
	public void SetSequence(Refers childRefers, bool active, int number)
	{
		bool flag = !this.IsValidChildRefers(childRefers);
		if (!flag)
		{
			GameObject numberIcon = childRefers.CGet<GameObject>("NumberIcon");
			bool flag2 = numberIcon == null;
			if (!flag2)
			{
				numberIcon.gameObject.SetActive(active);
				bool flag3 = !active;
				if (!flag3)
				{
					TextMeshProUGUI tmp = childRefers.CGet<TextMeshProUGUI>("NumberLabel");
					bool flag4 = tmp != null;
					if (flag4)
					{
						tmp.text = number.ToString();
					}
				}
			}
		}
	}

	// Token: 0x06002F7E RID: 12158 RVA: 0x001742D9 File Offset: 0x001724D9
	public float GetWidth(int index)
	{
		return this.isColumnDisplaying[index] ? ((this.columnWidthArray[index] > 0f) ? this.columnWidthArray[index] : (this.adaptableWidth * this.columnWidthArray[index])) : 0f;
	}

	// Token: 0x06002F7F RID: 12159 RVA: 0x00174314 File Offset: 0x00172514
	public void SetDisplayStatus(IEnumerable<int> show = null, IEnumerable<int> hide = null)
	{
		bool flag = show != null;
		if (flag)
		{
			foreach (int idx in show)
			{
				bool flag2 = this.isColumnDisplaying.CheckIndex(idx);
				if (flag2)
				{
					this.isColumnDisplaying[idx] = true;
				}
				else
				{
					string format = "Show index {0} > {1} out of range.";
					object arg = idx;
					bool[] array = this.isColumnDisplaying;
					Debug.LogError(string.Format(format, arg, (array != null) ? array.Length : -1));
				}
			}
		}
		bool flag3 = hide != null;
		if (flag3)
		{
			foreach (int idx2 in hide)
			{
				bool flag4 = this.isColumnDisplaying.CheckIndex(idx2);
				if (flag4)
				{
					this.isColumnDisplaying[idx2] = false;
				}
				else
				{
					string format2 = "Hide index {0} > {1} out of range.";
					object arg2 = idx2;
					bool[] array2 = this.isColumnDisplaying;
					Debug.LogError(string.Format(format2, arg2, (array2 != null) ? array2.Length : -1));
				}
			}
		}
		this.RefreshWidth();
	}

	// Token: 0x06002F80 RID: 12160 RVA: 0x00174438 File Offset: 0x00172638
	public void SetDisplayStatus(int index, bool show)
	{
		bool flag = this.isColumnDisplaying.CheckIndex(index);
		if (flag)
		{
			this.isColumnDisplaying[index] = show;
		}
		else
		{
			string format = "Show index {0} > {1} out of range.";
			object arg = index;
			bool[] array = this.isColumnDisplaying;
			Debug.LogError(string.Format(format, arg, (array != null) ? array.Length : -1));
		}
	}

	// Token: 0x06002F81 RID: 12161 RVA: 0x0017448C File Offset: 0x0017268C
	[return: TupleElementNames(new string[]
	{
		"content",
		"hover",
		"fakeBg"
	})]
	private ValueTuple<RectTransform, RectTransform, RectTransform> GetHoverComponents(GameObject itemRoot)
	{
		Refers refers = itemRoot.GetComponent<Refers>();
		bool flag = refers == null;
		ValueTuple<RectTransform, RectTransform, RectTransform> result;
		if (flag)
		{
			result = new ValueTuple<RectTransform, RectTransform, RectTransform>(null, null, null);
		}
		else
		{
			GameObject gameObject = refers.CGet<GameObject>("ContentLayout");
			RectTransform content = (gameObject != null) ? gameObject.GetComponent<RectTransform>() : null;
			GameObject gameObject2 = refers.CGet<GameObject>("Hover");
			RectTransform hover = (gameObject2 != null) ? gameObject2.GetComponent<RectTransform>() : null;
			GameObject gameObject3 = refers.CGet<GameObject>("FakeBg");
			RectTransform fakeBg = (gameObject3 != null) ? gameObject3.GetComponent<RectTransform>() : null;
			result = new ValueTuple<RectTransform, RectTransform, RectTransform>(content, hover, fakeBg);
		}
		return result;
	}

	// Token: 0x06002F82 RID: 12162 RVA: 0x00174510 File Offset: 0x00172710
	private void SetHoverComponentsActive(RectTransform hover, RectTransform fakeBg, bool active)
	{
		if (hover != null)
		{
			hover.gameObject.SetActive(active);
		}
		if (fakeBg != null)
		{
			fakeBg.gameObject.SetActive(active);
		}
	}

	// Token: 0x06002F83 RID: 12163 RVA: 0x00174538 File Offset: 0x00172738
	public void OnItemHoverEnter(GameObject itemRoot)
	{
		bool flag = itemRoot == null;
		if (!flag)
		{
			Transform itemTf = itemRoot.transform;
			bool flag2 = itemTf.parent != base.transform;
			if (!flag2)
			{
				ValueTuple<RectTransform, RectTransform, RectTransform> hoverComponents = this.GetHoverComponents(itemRoot);
				RectTransform content = hoverComponents.Item1;
				RectTransform hover = hoverComponents.Item2;
				RectTransform fakeBg = hoverComponents.Item3;
				bool flag3 = content == null || hover == null || fakeBg == null;
				if (!flag3)
				{
					this.SetHoverComponentsActive(hover, fakeBg, true);
				}
			}
		}
	}

	// Token: 0x06002F84 RID: 12164 RVA: 0x001745C0 File Offset: 0x001727C0
	public void OnItemHoverExit(GameObject itemRoot)
	{
		bool flag = itemRoot == null;
		if (!flag)
		{
			Transform itemTf = itemRoot.transform;
			bool flag2 = itemTf.parent != base.transform;
			if (!flag2)
			{
				ValueTuple<RectTransform, RectTransform, RectTransform> hoverComponents = this.GetHoverComponents(itemRoot);
				RectTransform content = hoverComponents.Item1;
				RectTransform hover = hoverComponents.Item2;
				RectTransform fakeBg = hoverComponents.Item3;
				bool flag3 = content == null || hover == null || fakeBg == null;
				if (!flag3)
				{
					this.SetHoverComponentsActive(hover, fakeBg, false);
				}
			}
		}
	}

	// Token: 0x06002F85 RID: 12165 RVA: 0x00174648 File Offset: 0x00172848
	private bool IsValidChildRefers(Refers childRefers)
	{
		return childRefers != null && childRefers.transform.parent == base.transform;
	}

	// Token: 0x06002F86 RID: 12166 RVA: 0x0017467C File Offset: 0x0017287C
	public Transform GetTableHeadChild(int contentIndex, bool onlyActive = false)
	{
		int childCount = this.GetContentChildCount(onlyActive);
		bool flag = contentIndex < 0 || contentIndex >= childCount;
		Transform result;
		if (flag)
		{
			result = null;
		}
		else
		{
			if (onlyActive)
			{
				int count = 0;
				for (int i = 0; i < base.transform.childCount; i++)
				{
					Transform child = base.transform.GetChild(i);
					bool activeSelf = child.gameObject.activeSelf;
					if (activeSelf)
					{
						bool flag2 = count == contentIndex;
						if (flag2)
						{
							return child;
						}
						count++;
					}
				}
			}
			result = base.transform.GetChild(contentIndex);
		}
		return result;
	}

	// Token: 0x06002F87 RID: 12167 RVA: 0x00174720 File Offset: 0x00172920
	public int GetContentChildCount(bool onlyActive = false)
	{
		int result;
		if (onlyActive)
		{
			int count = 0;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				bool activeSelf = base.transform.GetChild(i).gameObject.activeSelf;
				if (activeSelf)
				{
					count++;
				}
			}
			result = count;
		}
		else
		{
			result = base.transform.childCount;
		}
		return result;
	}

	// Token: 0x06002F88 RID: 12168 RVA: 0x00174788 File Offset: 0x00172988
	internal void BindSortController(ICommonSortAndFilterController sortController, short[] columnSortIds)
	{
		this._sortController = sortController;
		this._columnSortIds = columnSortIds;
		bool flag = this._sortController != null && !this.disableButtonListen;
		if (flag)
		{
			CButtonObsolete[] sortButtons = base.GetComponentsInChildren<CButtonObsolete>();
			bool flag2 = sortButtons != null;
			if (flag2)
			{
				CButtonObsolete[] array = sortButtons;
				for (int i = 0; i < array.Length; i++)
				{
					CButtonObsolete sortButton = array[i];
					sortButton.ClearAndAddListener(delegate
					{
						this.OnColumnClick(sortButton);
					});
				}
			}
		}
	}

	// Token: 0x06002F89 RID: 12169 RVA: 0x0017481A File Offset: 0x00172A1A
	internal void UnbindSortController()
	{
		this._sortController = null;
		this._columnSortIds = null;
	}

	// Token: 0x06002F8A RID: 12170 RVA: 0x0017482C File Offset: 0x00172A2C
	private void OnColumnClick(CButtonObsolete btn)
	{
		bool flag = this._sortController == null;
		if (!flag)
		{
			Refers refers = btn.GetComponent<Refers>();
			bool flag2 = refers == null;
			if (!flag2)
			{
				int columnIndex = -1;
				int childCount = this.GetContentChildCount(true);
				for (int i = 0; i < childCount; i++)
				{
					Transform child = this.GetTableHeadChild(i, true);
					bool flag3 = child == refers.transform;
					if (flag3)
					{
						columnIndex = i;
						break;
					}
				}
				bool flag4 = columnIndex < 0 || this._columnSortIds == null || columnIndex >= this._columnSortIds.Length;
				if (!flag4)
				{
					short sortId = this._columnSortIds[columnIndex];
					bool flag5 = sortId < 0;
					if (!flag5)
					{
						SortStateData currentSortData = this._sortController.SortAndFilterState.SortData;
						List<SortItemState> itemStates = currentSortData.ItemStates;
						int currentItemStateIndex = (itemStates != null) ? itemStates.FindIndex((SortItemState x) => x.SortId == sortId) : -1;
						List<SortItemState> newStateList = new List<SortItemState>();
						bool flag6 = currentSortData.ItemStates != null;
						if (flag6)
						{
							newStateList.AddRange(currentSortData.ItemStates);
						}
						bool flag7 = currentItemStateIndex >= 0;
						if (flag7)
						{
							SortItemState currentState = newStateList[currentItemStateIndex];
							bool flag8 = currentState.SortDirection == ESortDirection.Descending;
							if (flag8)
							{
								currentState.SortDirection = ESortDirection.Ascending;
								newStateList[currentItemStateIndex] = currentState;
							}
							else
							{
								newStateList.RemoveAt(currentItemStateIndex);
							}
						}
						else
						{
							newStateList.Add(new SortItemState
							{
								SortId = sortId,
								SortDirection = ESortDirection.Descending
							});
						}
						SortStateData newSortState = new SortStateData
						{
							ItemStates = newStateList
						};
						this._sortController.SetSortState(newSortState);
					}
				}
			}
		}
	}

	// Token: 0x06002F8B RID: 12171 RVA: 0x001749F0 File Offset: 0x00172BF0
	public void SyncSortStateFromController()
	{
		bool flag = this._sortController == null || this._isSyncingFromController;
		if (!flag)
		{
			this._isSyncingFromController = true;
			try
			{
				this.HideAllArrowAndSequence();
				SortStateData sortData = this._sortController.SortAndFilterState.SortData;
				bool flag2 = sortData == null || sortData.ItemStates == null || sortData.ItemStates.Count == 0;
				if (!flag2)
				{
					for (int i = 0; i < sortData.ItemStates.Count; i++)
					{
						SortItemState sortItem = sortData.ItemStates[i];
						short sortId = sortItem.SortId;
						bool isDescending = sortItem.SortDirection == ESortDirection.Descending;
						int columnIndex = -1;
						bool flag3 = this._columnSortIds != null;
						if (flag3)
						{
							for (int j = 0; j < this._columnSortIds.Length; j++)
							{
								bool flag4 = this._columnSortIds[j] == sortId;
								if (flag4)
								{
									columnIndex = j;
									break;
								}
							}
						}
						bool flag5 = columnIndex >= 0;
						if (flag5)
						{
							this.SetArrow(columnIndex, true, isDescending);
							this.SetSequence(columnIndex, true, i + 1);
						}
					}
				}
			}
			finally
			{
				this._isSyncingFromController = false;
			}
		}
	}

	// Token: 0x06002F8C RID: 12172 RVA: 0x00174B38 File Offset: 0x00172D38
	public void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		for (int i = 0; i < this.GetContentChildCount(false); i++)
		{
			Transform child = this.GetTableHeadChild(i, false);
			bool flag = child == null;
			if (!flag)
			{
				ILanguage[] components = child.GetComponents<ILanguage>();
				foreach (ILanguage component in components)
				{
					component.OnLanguageChange(languageType);
				}
			}
		}
	}

	// Token: 0x0400227F RID: 8831
	[Header("模板：CommonTableHeadItem (Prefab)")]
	[SerializeField]
	private GameObject itemTemplate;

	// Token: 0x04002280 RID: 8832
	[Header("模板物品列的背景的最后一项的Sprites，需与normalSprites一一对应")]
	[SerializeField]
	private Sprite[] lastSprites;

	// Token: 0x04002281 RID: 8833
	[Header("模板物品列的背景的正常Sprites，需与lastSprites一一对应")]
	[SerializeField]
	private Sprite[] normalSprites;

	// Token: 0x04002282 RID: 8834
	[SerializeField]
	private bool disableButtonListen = false;

	// Token: 0x04002283 RID: 8835
	[Tooltip("大于等于0意为像素列宽\n小于0意为自适应列宽\n\n自适应列宽按比例划分剩余宽度\n该值的如果为负数，其绝对值应大于0.01f以防止除零异常")]
	[Header("列宽数组")]
	[SerializeField]
	private float[] columnWidthArray;

	// Token: 0x04002284 RID: 8836
	[Tooltip("为防止意料之外的状态修改，UnityEditor中默认展示每一列，如果需要隐藏某列，应在其他脚本中设置isColumnDisplaying状态之后执行RefreshWidth")]
	[Header("列展示状态")]
	[ReadOnly]
	[SerializeField]
	private bool[] isColumnDisplaying;

	// Token: 0x04002285 RID: 8837
	[Header("全表宽")]
	[SerializeField]
	private float totalWidth;

	// Token: 0x04002286 RID: 8838
	[Header("自适应宽度，默认每一列都显示，由Unity自行计算")]
	[ReadOnly]
	[SerializeField]
	private float adaptableWidth;

	// Token: 0x04002287 RID: 8839
	private ICommonSortAndFilterController _sortController;

	// Token: 0x04002288 RID: 8840
	private short[] _columnSortIds;

	// Token: 0x04002289 RID: 8841
	private bool _isSyncingFromController;

	// Token: 0x0400228A RID: 8842
	private bool _isPointerTriggersInitialized;
}
