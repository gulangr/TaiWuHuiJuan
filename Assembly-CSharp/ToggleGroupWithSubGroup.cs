using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000A0 RID: 160
public class ToggleGroupWithSubGroup : MonoBehaviour
{
	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000588 RID: 1416 RVA: 0x00025171 File Offset: 0x00023371
	public CToggleObsolete CurrentParentToggle
	{
		get
		{
			return this._parentGroup.GetActive();
		}
	}

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000589 RID: 1417 RVA: 0x0002517E File Offset: 0x0002337E
	public CToggleObsolete CurrentChildToggle
	{
		get
		{
			return this._childGroup.GetActive();
		}
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x0002518B File Offset: 0x0002338B
	public CToggleObsolete GetParentToggle(int key)
	{
		return this._parentGroup.Get(key);
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x00025199 File Offset: 0x00023399
	public CToggleObsolete GetChildToggle(int key)
	{
		return this._childGroup.Get(key);
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x000251A7 File Offset: 0x000233A7
	private void Awake()
	{
		this.InitInternal();
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x000251B1 File Offset: 0x000233B1
	private void OnEnable()
	{
		this.RefreshLayoutGroup();
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x000251BC File Offset: 0x000233BC
	private void InitInternal()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._childToggleHolder.gameObject.SetActive(false);
			CToggleObsolete parentTogglePrefab = this.ParentTogglePrefab;
			if (parentTogglePrefab != null)
			{
				parentTogglePrefab.gameObject.SetActive(false);
			}
			CToggleObsolete childTogglePrefab = this.ChildTogglePrefab;
			if (childTogglePrefab != null)
			{
				childTogglePrefab.gameObject.SetActive(false);
			}
			this._parentGroup.SetAllowOnNum(1);
			this._childGroup.SetAllowOnNum(1);
			bool flag = this._subToggleAmount == null;
			if (flag)
			{
				this._subToggleAmount = new List<int>(this._parentGroup.Count());
				bool flag2 = this._parentGroup.GetAll() != null;
				if (flag2)
				{
					foreach (CToggleObsolete item in this._parentGroup.GetAll())
					{
						this._subToggleAmount.Add(0);
					}
				}
			}
			CToggleGroupObsolete childGroup = this._childGroup;
			childGroup.OnActiveToggleChange = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(childGroup.OnActiveToggleChange, new Action<CToggleObsolete, CToggleObsolete>(this.OnChildToggleChange));
			CToggleGroupObsolete parentGroup = this._parentGroup;
			parentGroup.OnActiveToggleChange = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(parentGroup.OnActiveToggleChange, new Action<CToggleObsolete, CToggleObsolete>(this.OnParentToggleChange));
			this._inited = true;
		}
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x0002531C File Offset: 0x0002351C
	private void RefreshLayoutGroup()
	{
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
		if (activeInHierarchy)
		{
			base.StartCoroutine(this.Coroutine_GenerateSubToggles());
		}
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x00025346 File Offset: 0x00023546
	private IEnumerator Coroutine_GenerateSubToggles()
	{
		yield return new WaitForEndOfFrame();
		LayoutRebuilder.ForceRebuildLayoutImmediate(this._parentToggleHolder);
		bool flag = this._childGroup.gameObject.activeSelf && this.CurrentParentToggle != null;
		if (flag)
		{
			this._childGroup.transform.position = this.CurrentParentToggle.transform.position;
			this._childGroup.GetComponent<RectTransform>().anchoredPosition += this.ChildOffset;
			LayoutRebuilder.ForceRebuildLayoutImmediate(this._childToggleHolder);
		}
		yield break;
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x00025358 File Offset: 0x00023558
	private void OnParentToggleChange(CToggleObsolete newTog, CToggleObsolete preTog)
	{
		Action<CToggleObsolete, CToggleObsolete> onToggleChange = this.OnToggleChange;
		if (onToggleChange != null)
		{
			onToggleChange(null, newTog);
		}
		this.GenerateSubToggles(newTog.Key);
		bool flag = this.AutoSelectFirstChild && this._childGroup.Count() > 0;
		if (flag)
		{
			this._childGroup.Set(0, true, false);
		}
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x000253B8 File Offset: 0x000235B8
	private void GenerateSubToggles(int key)
	{
		this._childGroup.Clear();
		CToggleObsolete parentToggle = this._parentGroup.Get(key);
		CommonUtils.PrepareEnoughChildren(this._childToggleHolder, this.ChildTogglePrefab.gameObject, this._subToggleAmount[key], null);
		this._childToggleHolder.gameObject.SetActive(true);
		this._childGroup.transform.position = parentToggle.transform.position;
		this._childGroup.GetComponent<RectTransform>().anchoredPosition += this.ChildOffset;
		for (int i = 0; i < this._subToggleAmount[key]; i++)
		{
			CToggleObsolete childToggle = this._childToggleHolder.GetChild(i).GetComponent<CToggleObsolete>();
			childToggle.Key = i;
			this._childGroup.Add(childToggle);
			Action<CToggleObsolete, CToggleObsolete> onChildToggleRender = this.OnChildToggleRender;
			if (onChildToggleRender != null)
			{
				onChildToggleRender(childToggle, parentToggle);
			}
		}
		this.RefreshLayoutGroup();
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x000254BD File Offset: 0x000236BD
	private void OnChildToggleChange(CToggleObsolete newTog, CToggleObsolete preTog)
	{
		Action<CToggleObsolete, CToggleObsolete> onToggleChange = this.OnToggleChange;
		if (onToggleChange != null)
		{
			onToggleChange(newTog, this.CurrentParentToggle);
		}
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x000254DC File Offset: 0x000236DC
	private void SetParentToggleAmount(int amount)
	{
		this._parentGroup.Clear();
		CommonUtils.PrepareEnoughChildren(this._parentToggleHolder, this.ParentTogglePrefab.gameObject, amount, null);
		this._parentToggleHolder.gameObject.SetActive(true);
		for (int i = 0; i < amount; i++)
		{
			CToggleObsolete parentToggle = this._parentToggleHolder.GetChild(i).GetComponent<CToggleObsolete>();
			parentToggle.Key = i;
			this._parentGroup.Add(parentToggle);
			Action<CToggleObsolete> onParentToggleRender = this.OnParentToggleRender;
			if (onParentToggleRender != null)
			{
				onParentToggleRender(parentToggle);
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(this._parentToggleHolder);
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x00025584 File Offset: 0x00023784
	private void TriggerRenderParents(int amount)
	{
		this._parentToggleHolder.gameObject.SetActive(true);
		for (int i = 0; i < amount; i++)
		{
			CToggleObsolete parentToggle = this._parentToggleHolder.GetChild(i).GetComponent<CToggleObsolete>();
			Action<CToggleObsolete> onParentToggleRender = this.OnParentToggleRender;
			if (onParentToggleRender != null)
			{
				onParentToggleRender(parentToggle);
			}
		}
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x000255E0 File Offset: 0x000237E0
	public void Init(List<int> subToggleAmount, bool regenerateParent = true, int defaultSelectKey = -1)
	{
		this.InitInternal();
		bool flag = subToggleAmount != null;
		if (flag)
		{
			this._subToggleAmount = new List<int>(subToggleAmount);
			if (regenerateParent)
			{
				this.SetParentToggleAmount(subToggleAmount.Count);
			}
			else
			{
				this.TriggerRenderParents(this._subToggleAmount.Count);
			}
		}
		this._parentGroup.InitPreOnToggle(defaultSelectKey);
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x00025642 File Offset: 0x00023842
	public void Init(int defaultSelectKey = -1)
	{
		this.InitInternal();
		this.TriggerRenderParents(this._subToggleAmount.Count);
		this._parentGroup.InitPreOnToggle(defaultSelectKey);
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x0002566C File Offset: 0x0002386C
	public void ChangeSubToggleAmount(int parentKey, int newAmount, int defaultSelect, bool notifySelect = false)
	{
		this._subToggleAmount[parentKey] = newAmount;
		bool flag = this.CurrentParentToggle.Key == parentKey;
		if (flag)
		{
			this.GenerateSubToggles(parentKey);
			bool flag2 = this._childGroup.Count() > 0 && defaultSelect >= 0;
			if (flag2)
			{
				if (notifySelect)
				{
					this._childGroup.Set(defaultSelect, true, false);
				}
				else
				{
					this._childGroup.SetWithoutNotify(defaultSelect, true);
				}
			}
		}
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x000256E7 File Offset: 0x000238E7
	public void SetParentToggle(int parentTogleKey)
	{
		this._parentGroup.Set(parentTogleKey, true, false);
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x000256F9 File Offset: 0x000238F9
	public void SetParentToggleWithoutNotify(int parentTogleKey)
	{
		this._parentGroup.SetWithoutNotify(parentTogleKey, true);
	}

	// Token: 0x04000485 RID: 1157
	[SerializeField]
	private CToggleGroupObsolete _parentGroup;

	// Token: 0x04000486 RID: 1158
	[SerializeField]
	private CToggleGroupObsolete _childGroup;

	// Token: 0x04000487 RID: 1159
	[SerializeField]
	private List<int> _subToggleAmount;

	// Token: 0x04000488 RID: 1160
	[SerializeField]
	private RectTransform _parentToggleHolder;

	// Token: 0x04000489 RID: 1161
	[SerializeField]
	private RectTransform _childToggleHolder;

	// Token: 0x0400048A RID: 1162
	[SerializeField]
	public CToggleObsolete ParentTogglePrefab;

	// Token: 0x0400048B RID: 1163
	[SerializeField]
	public CToggleObsolete ChildTogglePrefab;

	// Token: 0x0400048C RID: 1164
	[SerializeField]
	public bool AutoSelectFirstChild;

	// Token: 0x0400048D RID: 1165
	[SerializeField]
	public Vector2 ChildOffset;

	// Token: 0x0400048E RID: 1166
	public Action<CToggleObsolete> OnParentToggleRender;

	// Token: 0x0400048F RID: 1167
	public Action<CToggleObsolete, CToggleObsolete> OnChildToggleRender;

	// Token: 0x04000490 RID: 1168
	public Action<CToggleObsolete, CToggleObsolete> OnToggleChange;

	// Token: 0x04000491 RID: 1169
	private bool _inited = false;
}
