using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020000D3 RID: 211
[Obsolete]
public class CToggleGroupObsolete : MonoBehaviour
{
	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06000776 RID: 1910 RVA: 0x000348A0 File Offset: 0x00032AA0
	public int AllOnNum
	{
		get
		{
			return this._allowOnNum;
		}
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x000348A8 File Offset: 0x00032AA8
	public void InitPreOnToggle(int targetKey = -1)
	{
		bool preOnToggleInited = this._preOnToggleInited;
		if (!preOnToggleInited)
		{
			List<CToggleObsolete> preOnToggle = new List<CToggleObsolete>();
			this._keyList.Clear();
			foreach (CToggleObsolete t2 in this._toggleList)
			{
				bool flag = this._keyList.Contains(t2.Key);
				if (flag)
				{
					throw new Exception("Duplicated key " + t2.Key.ToString() + " in the same toggle group.");
				}
				this._keyList.Add(t2.Key);
				t2.Register(this);
				bool isOn = t2.isOn;
				if (isOn)
				{
					bool flag2 = !this.AllowSwitchOff && preOnToggle.Count < this.AllOnNum;
					if (flag2)
					{
						preOnToggle.Add(t2);
					}
					else
					{
						t2.isOn = false;
					}
				}
			}
			bool flag3 = preOnToggle.Count <= 0 && !this.AllowSwitchOff && this._toggleList.Count > 0;
			if (flag3)
			{
				bool flag4 = targetKey != -1;
				if (flag4)
				{
					int targetIndex = this._toggleList.FindIndex((CToggleObsolete t) => t.Key == targetKey);
					bool flag5 = this._toggleList.CheckIndex(targetIndex);
					if (flag5)
					{
						preOnToggle.Add(this._toggleList[targetIndex]);
					}
				}
				bool flag6 = preOnToggle.Count <= 0;
				if (flag6)
				{
					preOnToggle.Add(this._toggleList[0]);
				}
			}
			preOnToggle.ForEach(delegate(CToggleObsolete e)
			{
				e.isOn = true;
				this.NotifyToggle(e, true, true);
			});
			this._preOnToggleInited = true;
		}
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x00034A8C File Offset: 0x00032C8C
	public List<CToggleObsolete> GetAll()
	{
		return this._toggleList;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00034AA4 File Offset: 0x00032CA4
	public CToggleObsolete Get(int key)
	{
		foreach (CToggleObsolete toggle in this._toggleList)
		{
			bool flag = toggle.Key == key;
			if (flag)
			{
				return toggle;
			}
		}
		return null;
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x00034B0C File Offset: 0x00032D0C
	public void SetInteractable(bool interactable, CToggleObsolete toggle = null)
	{
		foreach (CToggleObsolete t in this._toggleList)
		{
			bool flag = null == toggle || t == toggle;
			if (flag)
			{
				t.interactable = interactable;
			}
		}
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x00034B80 File Offset: 0x00032D80
	public void SetInteractable(bool interactable, int key)
	{
		this.SetInteractable(interactable, this.Get(key));
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00034B94 File Offset: 0x00032D94
	[ContextMenu("Add All Child Toggles")]
	public void AddAllChildToggles()
	{
		this.Clear();
		foreach (CToggleObsolete t in base.transform.GetComponentsInTopChildren(false))
		{
			t.Key = t.transform.GetSiblingIndex();
			this.Add(t);
		}
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x00034BE4 File Offset: 0x00032DE4
	public void DeSelectAll(bool forceRaiseEvent = false)
	{
		for (int i = this._onList.Count - 1; i >= 0; i--)
		{
			bool flag = !forceRaiseEvent && !this._onList[i].isOn;
			if (flag)
			{
				break;
			}
			this._onList[i].isOn = false;
			this.NotifyToggle(this._onList[i], false, true);
		}
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00034C5C File Offset: 0x00032E5C
	public void Set(int key, bool value = true, bool forceRaiseEvent = false)
	{
		foreach (CToggleObsolete toggle in this._toggleList)
		{
			bool flag = toggle.Key == key;
			if (flag)
			{
				bool flag2 = !forceRaiseEvent && toggle.isOn == value;
				if (flag2)
				{
					break;
				}
				toggle.isOn = value;
				this.NotifyToggle(toggle, value, true);
				bool flag3 = value && toggle.graphic && toggle.graphic.canvasRenderer;
				if (flag3)
				{
					toggle.graphic.canvasRenderer.SetAlpha(1f);
				}
			}
			else
			{
				bool flag4 = toggle.graphic && toggle.graphic.canvasRenderer;
				if (flag4)
				{
					toggle.graphic.canvasRenderer.SetAlpha(0f);
				}
			}
		}
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x00034D6C File Offset: 0x00032F6C
	public void Set(CToggleObsolete toggle, bool value = true)
	{
		bool flag = toggle.isOn == value;
		if (!flag)
		{
			toggle.isOn = value;
			this.NotifyToggle(toggle, value, true);
		}
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x00034D9C File Offset: 0x00032F9C
	public void SetWithoutNotify(int key, bool value = true)
	{
		foreach (CToggleObsolete toggle in this._toggleList)
		{
			bool flag = toggle.Key == key;
			if (flag)
			{
				bool flag2 = toggle.isOn == value;
				if (flag2)
				{
					break;
				}
				toggle.isOn = value;
				this.NotifyToggle(toggle, value, false);
				break;
			}
		}
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x00034E20 File Offset: 0x00033020
	public void SetAllowOnNum(int num)
	{
		this._allowOnNum = num;
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x00034E2C File Offset: 0x0003302C
	public CToggleObsolete GetActive()
	{
		bool flag = this._onList.Count >= 1;
		CToggleObsolete result;
		if (flag)
		{
			result = this._onList[0];
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x00034E64 File Offset: 0x00033064
	public List<CToggleObsolete> GetAllActive()
	{
		return this._onList;
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x00034E7C File Offset: 0x0003307C
	public void SetToFirstInteractable(bool forceNotifyEvent = false)
	{
		bool needNotify = this._onList.Count == 0 || forceNotifyEvent;
		for (int i = 0; i < this._onList.Count; i++)
		{
			this._onList[i].isOn = false;
		}
		this._onList.Clear();
		for (int j = 0; j < this._toggleList.Count; j++)
		{
			CToggleObsolete tog = this._toggleList[j];
			bool flag = tog.interactable && tog.gameObject.activeSelf;
			if (flag)
			{
				tog.isOn = true;
				this._onList.Add(tog);
				break;
			}
		}
		bool flag2 = needNotify;
		if (flag2)
		{
			Action<CToggleObsolete, CToggleObsolete> onActiveToggleChange = this.OnActiveToggleChange;
			if (onActiveToggleChange != null)
			{
				onActiveToggleChange(null, null);
			}
		}
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x00034F58 File Offset: 0x00033158
	public int Count()
	{
		return this._toggleList.Count;
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x00034F78 File Offset: 0x00033178
	public void Add(CToggleObsolete toggle)
	{
		bool flag = this._keyList.Contains(toggle.Key);
		if (flag)
		{
			throw new Exception("Duplicated key " + toggle.Key.ToString() + " in the same toggle group.");
		}
		bool flag2 = this._toggleList.Contains(toggle);
		if (flag2)
		{
			throw new Exception(toggle.name + " is already member of " + base.name + ".");
		}
		this._toggleList.Add(toggle);
		this._keyList.Add(toggle.Key);
		toggle.Register(this);
		bool isOn = toggle.isOn;
		if (isOn)
		{
			bool flag3 = this._onList.Count >= this._allowOnNum && this._allowOnNum > 0;
			if (flag3)
			{
				toggle.isOn = false;
			}
			else
			{
				this._onList.Add(toggle);
			}
		}
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x00035060 File Offset: 0x00033260
	public void Remove(int index)
	{
		bool flag = this._toggleList == null || this._toggleList.Count <= 0;
		if (!flag)
		{
			for (int i = this._toggleList.Count - 1; i >= 0; i--)
			{
				CToggleObsolete toggle = this._toggleList[i];
				bool flag2 = toggle.Key == index;
				if (flag2)
				{
					this.Remove(toggle);
					break;
				}
			}
		}
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x000350D8 File Offset: 0x000332D8
	public void Remove(CToggleObsolete toggle)
	{
		toggle.UnRegister();
		this._toggleList.Remove(toggle);
		this._keyList.Remove(toggle.Key);
		bool flag = this._onList.Contains(toggle);
		if (flag)
		{
			this._onList.Remove(toggle);
			bool flag2 = !this.AllowSwitchOff && this._onList.Count <= 0;
			if (flag2)
			{
				this._toggleList[0].isOn = true;
				this._onList.Add(this._toggleList[0]);
			}
		}
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x00035178 File Offset: 0x00033378
	[ContextMenu("Clear All Toggles")]
	public void Clear()
	{
		this._toggleList.Clear();
		this._keyList.Clear();
		this._onList.Clear();
		this._preOnToggleInited = false;
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x000351A8 File Offset: 0x000333A8
	public bool AnyTogglesOn()
	{
		return this._onList.Count > 0;
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x000351C8 File Offset: 0x000333C8
	private void ValidateToggleIsInGroup(CToggleObsolete toggle)
	{
		bool flag = toggle == null || !this._toggleList.Contains(toggle);
		if (flag)
		{
			throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[]
			{
				toggle,
				this
			}));
		}
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x00035214 File Offset: 0x00033414
	public bool ValidateStateChange(CToggleObsolete toggle, bool isOn)
	{
		bool flag = isOn && !this.AllowSwitchOff && this._onList.Count == 1 && this._onList[0] == toggle;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = !this.AllowUncheck && isOn;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = this._onList.Count >= this._allowOnNum && this._allowOnNum > 0 && !isOn;
				result = (!flag3 || this._autoCancelFront);
			}
		}
		return result;
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x000352A0 File Offset: 0x000334A0
	public void NotifyToggle(CToggleObsolete toggle, bool value, bool raiseEvent = true)
	{
		this.ValidateToggleIsInGroup(toggle);
		if (value)
		{
			bool flag = !this._onList.Contains(toggle);
			if (flag)
			{
				this._onList.Add(toggle);
				bool flag2 = this._onList.Count > this._allowOnNum && this._allowOnNum > 0;
				if (flag2)
				{
					CToggleObsolete tog = this._onList[0];
					tog.isOn = false;
					this._onList.RemoveAt(0);
					if (raiseEvent)
					{
						Action<CToggleObsolete, CToggleObsolete> onActiveToggleChange = this.OnActiveToggleChange;
						if (onActiveToggleChange != null)
						{
							onActiveToggleChange(toggle, tog);
						}
					}
				}
				else if (raiseEvent)
				{
					Action<CToggleObsolete, CToggleObsolete> onActiveToggleChange2 = this.OnActiveToggleChange;
					if (onActiveToggleChange2 != null)
					{
						onActiveToggleChange2(toggle, null);
					}
				}
			}
			else if (raiseEvent)
			{
				Action<CToggleObsolete, CToggleObsolete> onActiveToggleChange3 = this.OnActiveToggleChange;
				if (onActiveToggleChange3 != null)
				{
					onActiveToggleChange3(toggle, null);
				}
			}
		}
		else
		{
			this._onList.Remove(toggle);
			if (raiseEvent)
			{
				Action<CToggleObsolete, CToggleObsolete> onActiveToggleChange4 = this.OnActiveToggleChange;
				if (onActiveToggleChange4 != null)
				{
					onActiveToggleChange4(null, toggle);
				}
			}
		}
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x000353B4 File Offset: 0x000335B4
	[return: TupleElementNames(new string[]
	{
		"onlyOne",
		"activeIndex"
	})]
	public ValueTuple<bool, int> IsOnlyTogActive()
	{
		int count = 0;
		int index = -1;
		foreach (CToggleObsolete toggle in this._toggleList)
		{
			bool activeSelf = toggle.gameObject.activeSelf;
			if (activeSelf)
			{
				count++;
				index = toggle.Key;
			}
		}
		bool flag = count == 1;
		ValueTuple<bool, int> result;
		if (flag)
		{
			result = new ValueTuple<bool, int>(true, index);
		}
		else
		{
			result = new ValueTuple<bool, int>(false, -1);
		}
		return result;
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x0003544C File Offset: 0x0003364C
	public int GetActivatedCount()
	{
		int count = 0;
		foreach (CToggleObsolete toggle in this._toggleList)
		{
			bool activeSelf = toggle.gameObject.activeSelf;
			if (activeSelf)
			{
				count++;
			}
		}
		return count;
	}

	// Token: 0x040007AF RID: 1967
	[SerializeField]
	[HideInInspector]
	private List<CToggleObsolete> _toggleList = new List<CToggleObsolete>();

	// Token: 0x040007B0 RID: 1968
	private List<int> _keyList = new List<int>();

	// Token: 0x040007B1 RID: 1969
	[SerializeField]
	[Tooltip("是否允许一个都不选中")]
	public bool AllowSwitchOff = false;

	// Token: 0x040007B2 RID: 1970
	[Tooltip("是否允许再次点击以取消选中")]
	public bool AllowUncheck = false;

	// Token: 0x040007B3 RID: 1971
	[SerializeField]
	private int _allowOnNum = 1;

	// Token: 0x040007B4 RID: 1972
	[SerializeField]
	private bool _autoCancelFront = true;

	// Token: 0x040007B5 RID: 1973
	private List<CToggleObsolete> _onList = new List<CToggleObsolete>();

	// Token: 0x040007B6 RID: 1974
	public Action<CToggleObsolete, CToggleObsolete> OnActiveToggleChange;

	// Token: 0x040007B7 RID: 1975
	private bool _preOnToggleInited;
}
