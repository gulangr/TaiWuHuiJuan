using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x0200100C RID: 4108
	public class CToggleGroupMultiSelect : MonoBehaviour, ICToggleGroup
	{
		// Token: 0x14000093 RID: 147
		// (add) Token: 0x0600BBBE RID: 48062 RVA: 0x00556980 File Offset: 0x00554B80
		// (remove) Token: 0x0600BBBF RID: 48063 RVA: 0x005569B8 File Offset: 0x00554BB8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, int> OnActiveIndexChange;

		// Token: 0x1700152E RID: 5422
		// (get) Token: 0x0600BBC0 RID: 48064 RVA: 0x005569ED File Offset: 0x00554BED
		public int MaxSelectCount
		{
			get
			{
				return this.maxSelectCount;
			}
		}

		// Token: 0x0600BBC1 RID: 48065 RVA: 0x005569F8 File Offset: 0x00554BF8
		public void Init()
		{
			bool initialized = this._initialized;
			if (!initialized)
			{
				this._activeIndices.Clear();
				for (int i = 0; i < this.toggleList.Count; i++)
				{
					CToggle toggle = this.toggleList[i];
					toggle.Register(this);
					bool isOn = toggle.isOn;
					if (isOn)
					{
						bool flag = this.maxSelectCount <= 0 || this._activeIndices.Count < this.maxSelectCount;
						if (flag)
						{
							this._activeIndices.Add(i);
						}
						else
						{
							toggle.isOn = false;
						}
					}
				}
				bool flag2 = this._activeIndices.Count == 0 && !this.allowSwitchOff && this.toggleList.Count > 0;
				if (flag2)
				{
					this.toggleList[0].isOn = true;
					this._activeIndices.Add(0);
				}
				this._initialized = true;
			}
		}

		// Token: 0x0600BBC2 RID: 48066 RVA: 0x00556AF8 File Offset: 0x00554CF8
		public List<CToggle> GetAll()
		{
			return this.toggleList;
		}

		// Token: 0x0600BBC3 RID: 48067 RVA: 0x00556B10 File Offset: 0x00554D10
		public CToggle Get(int index)
		{
			return this.toggleList.CheckIndex(index) ? this.toggleList[index] : null;
		}

		// Token: 0x0600BBC4 RID: 48068 RVA: 0x00556B40 File Offset: 0x00554D40
		public void SetInteractable(bool interactable)
		{
			foreach (CToggle t in this.toggleList)
			{
				t.interactable = interactable;
			}
		}

		// Token: 0x0600BBC5 RID: 48069 RVA: 0x00556B98 File Offset: 0x00554D98
		public void SetInteractable(bool interactable, int index)
		{
			bool flag = this.toggleList.CheckIndex(index);
			if (flag)
			{
				this.toggleList[index].interactable = interactable;
			}
		}

		// Token: 0x0600BBC6 RID: 48070 RVA: 0x00556BCC File Offset: 0x00554DCC
		[ContextMenu("Add All Child Toggles")]
		public void AddAllChildToggles()
		{
			this.Clear();
			foreach (CToggle toggle in base.transform.GetComponentsInTopChildren(false))
			{
				this.Add(toggle);
			}
		}

		// Token: 0x0600BBC7 RID: 48071 RVA: 0x00556C0C File Offset: 0x00554E0C
		public void SelectAll(bool forceRaiseEvent = false)
		{
			for (int i = 0; i < this.toggleList.Count; i++)
			{
				CToggle toggle = this.toggleList[i];
				bool flag = !forceRaiseEvent && toggle.isOn;
				if (!flag)
				{
					toggle.isOn = true;
					this._activeIndices.Add(i);
					Action<int, int> onActiveIndexChange = this.OnActiveIndexChange;
					if (onActiveIndexChange != null)
					{
						onActiveIndexChange(i, -1);
					}
				}
			}
		}

		// Token: 0x0600BBC8 RID: 48072 RVA: 0x00556C80 File Offset: 0x00554E80
		public void DeSelectAll(bool forceRaiseEvent = false)
		{
			for (int i = this._activeIndices.Count - 1; i >= 0; i--)
			{
				int idx = this._activeIndices[i];
				CToggle toggle = this.toggleList[idx];
				bool flag = !forceRaiseEvent && !toggle.isOn;
				if (!flag)
				{
					toggle.isOn = false;
					this._activeIndices.RemoveAt(i);
					Action<int, int> onActiveIndexChange = this.OnActiveIndexChange;
					if (onActiveIndexChange != null)
					{
						onActiveIndexChange(-1, idx);
					}
				}
			}
		}

		// Token: 0x0600BBC9 RID: 48073 RVA: 0x00556D0C File Offset: 0x00554F0C
		public void Select(int index, bool forceRaiseEvent = false)
		{
			bool flag = !this.toggleList.CheckIndex(index);
			if (!flag)
			{
				bool flag2 = this._activeIndices.Contains(index);
				if (flag2)
				{
					if (forceRaiseEvent)
					{
						Action<int, int> onActiveIndexChange = this.OnActiveIndexChange;
						if (onActiveIndexChange != null)
						{
							onActiveIndexChange(index, -1);
						}
					}
				}
				else
				{
					int cancelledIndex = -1;
					bool flag3 = this.maxSelectCount > 0 && this._activeIndices.Count >= this.maxSelectCount;
					if (flag3)
					{
						bool flag4 = this.autoCancelFront;
						if (!flag4)
						{
							return;
						}
						cancelledIndex = this._activeIndices[0];
						this.toggleList[cancelledIndex].isOn = false;
						this._activeIndices.RemoveAt(0);
					}
					this.toggleList[index].isOn = true;
					this._activeIndices.Add(index);
					Action<int, int> onActiveIndexChange2 = this.OnActiveIndexChange;
					if (onActiveIndexChange2 != null)
					{
						onActiveIndexChange2(index, cancelledIndex);
					}
				}
			}
		}

		// Token: 0x0600BBCA RID: 48074 RVA: 0x00556E04 File Offset: 0x00555004
		public void DeSelect(int index, bool forceRaiseEvent = false)
		{
			bool flag = !this.toggleList.CheckIndex(index);
			if (!flag)
			{
				bool flag2 = !this._activeIndices.Contains(index);
				if (flag2)
				{
					if (forceRaiseEvent)
					{
						Action<int, int> onActiveIndexChange = this.OnActiveIndexChange;
						if (onActiveIndexChange != null)
						{
							onActiveIndexChange(-1, index);
						}
					}
				}
				else
				{
					bool flag3 = !this.allowSwitchOff && this._activeIndices.Count == 1;
					if (!flag3)
					{
						this.toggleList[index].isOn = false;
						this._activeIndices.Remove(index);
						Action<int, int> onActiveIndexChange2 = this.OnActiveIndexChange;
						if (onActiveIndexChange2 != null)
						{
							onActiveIndexChange2(-1, index);
						}
					}
				}
			}
		}

		// Token: 0x0600BBCB RID: 48075 RVA: 0x00556EAC File Offset: 0x005550AC
		public void SelectWithoutNotify(int index)
		{
			bool flag = !this.toggleList.CheckIndex(index);
			if (!flag)
			{
				bool flag2 = this._activeIndices.Contains(index);
				if (!flag2)
				{
					bool flag3 = this.maxSelectCount > 0 && this._activeIndices.Count >= this.maxSelectCount;
					if (flag3)
					{
						bool flag4 = this.autoCancelFront;
						if (!flag4)
						{
							return;
						}
						int cancelledIndex = this._activeIndices[0];
						this.toggleList[cancelledIndex].isOn = false;
						this._activeIndices.RemoveAt(0);
					}
					this.toggleList[index].isOn = true;
					this._activeIndices.Add(index);
				}
			}
		}

		// Token: 0x0600BBCC RID: 48076 RVA: 0x00556F70 File Offset: 0x00555170
		public void DeSelectWithoutNotify(int index)
		{
			bool flag = !this.toggleList.CheckIndex(index);
			if (!flag)
			{
				bool flag2 = !this._activeIndices.Contains(index);
				if (!flag2)
				{
					bool flag3 = !this.allowSwitchOff && this._activeIndices.Count == 1;
					if (!flag3)
					{
						this.toggleList[index].isOn = false;
						this._activeIndices.Remove(index);
					}
				}
			}
		}

		// Token: 0x0600BBCD RID: 48077 RVA: 0x00556FE8 File Offset: 0x005551E8
		public List<int> GetActiveIndices()
		{
			return this._activeIndices;
		}

		// Token: 0x0600BBCE RID: 48078 RVA: 0x00557000 File Offset: 0x00555200
		public int GetFirstActiveIndex()
		{
			return (this._activeIndices.Count > 0) ? this._activeIndices[0] : -1;
		}

		// Token: 0x0600BBCF RID: 48079 RVA: 0x0055702F File Offset: 0x0055522F
		public void SetMaxSelectCount(int count)
		{
			this.maxSelectCount = count;
		}

		// Token: 0x0600BBD0 RID: 48080 RVA: 0x0055703C File Offset: 0x0055523C
		public int Count()
		{
			return this.toggleList.Count;
		}

		// Token: 0x0600BBD1 RID: 48081 RVA: 0x0055705C File Offset: 0x0055525C
		public void Add(CToggle toggle)
		{
			bool flag = toggle == null;
			if (!flag)
			{
				bool flag2 = this.toggleList.Contains(toggle);
				if (flag2)
				{
					throw new InvalidOperationException(toggle.name + " is already member of " + base.name + ".");
				}
				this.toggleList.Add(toggle);
				toggle.Register(this);
				bool flag3 = !toggle.isOn;
				if (!flag3)
				{
					bool flag4 = this.maxSelectCount > 0 && this._activeIndices.Count >= this.maxSelectCount;
					if (flag4)
					{
						toggle.isOn = false;
					}
					else
					{
						this._activeIndices.Add(this.toggleList.Count - 1);
					}
				}
			}
		}

		// Token: 0x0600BBD2 RID: 48082 RVA: 0x0055711C File Offset: 0x0055531C
		public void Remove(int index)
		{
			bool flag = !this.toggleList.CheckIndex(index);
			if (!flag)
			{
				this.RemoveInternal(index);
			}
		}

		// Token: 0x0600BBD3 RID: 48083 RVA: 0x00557148 File Offset: 0x00555348
		private void RemoveInternal(int index)
		{
			CToggle toggle = this.toggleList[index];
			toggle.UnRegister();
			this.toggleList.RemoveAt(index);
			bool wasActive = this._activeIndices.Remove(index);
			for (int i = 0; i < this._activeIndices.Count; i++)
			{
				bool flag = this._activeIndices[i] > index;
				if (flag)
				{
					List<int> activeIndices = this._activeIndices;
					int index2 = i;
					int num = activeIndices[index2];
					activeIndices[index2] = num - 1;
				}
			}
			bool flag2 = wasActive && !this.allowSwitchOff && this._activeIndices.Count == 0 && this.toggleList.Count > 0;
			if (flag2)
			{
				this.toggleList[0].isOn = true;
				this._activeIndices.Add(0);
			}
		}

		// Token: 0x0600BBD4 RID: 48084 RVA: 0x00557225 File Offset: 0x00555425
		[ContextMenu("Clear All Toggles")]
		public void Clear()
		{
			this.toggleList.Clear();
			this._activeIndices.Clear();
			this._initialized = false;
		}

		// Token: 0x0600BBD5 RID: 48085 RVA: 0x00557248 File Offset: 0x00555448
		public bool AnyTogglesOn()
		{
			return this._activeIndices.Count > 0;
		}

		// Token: 0x0600BBD6 RID: 48086 RVA: 0x00557268 File Offset: 0x00555468
		public int SelectedCount()
		{
			return this._activeIndices.Count;
		}

		// Token: 0x0600BBD7 RID: 48087 RVA: 0x00557288 File Offset: 0x00555488
		public bool ValidateStateChange(CToggle toggle, bool isOn)
		{
			bool flag = toggle == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int index = this.toggleList.IndexOf(toggle);
				bool flag2 = index < 0;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = isOn && !this.allowSwitchOff && this._activeIndices.Count == 1 && this._activeIndices[0] == index;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = !this.allowUncheck && isOn;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = !isOn && this.maxSelectCount > 0 && this._activeIndices.Count >= this.maxSelectCount && !this.autoCancelFront;
							result = !flag5;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600BBD8 RID: 48088 RVA: 0x00557348 File Offset: 0x00555548
		public void NotifyToggle(CToggle toggle, bool value, bool raiseEvent = true)
		{
			int index = this.toggleList.IndexOf(toggle);
			bool flag = index < 0;
			if (flag)
			{
				throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", toggle, this));
			}
			if (value)
			{
				bool flag2 = this._activeIndices.Contains(index);
				if (!flag2)
				{
					int cancelledIndex = -1;
					bool flag3 = this.maxSelectCount > 0 && this._activeIndices.Count >= this.maxSelectCount;
					if (flag3)
					{
						cancelledIndex = this._activeIndices[0];
						this.toggleList[cancelledIndex].isOn = false;
						this._activeIndices.RemoveAt(0);
					}
					this._activeIndices.Add(index);
					if (raiseEvent)
					{
						Action<int, int> onActiveIndexChange = this.OnActiveIndexChange;
						if (onActiveIndexChange != null)
						{
							onActiveIndexChange(index, cancelledIndex);
						}
					}
				}
			}
			else
			{
				bool flag4 = !this._activeIndices.Contains(index);
				if (!flag4)
				{
					this._activeIndices.Remove(index);
					if (raiseEvent)
					{
						Action<int, int> onActiveIndexChange2 = this.OnActiveIndexChange;
						if (onActiveIndexChange2 != null)
						{
							onActiveIndexChange2(-1, index);
						}
					}
				}
			}
		}

		// Token: 0x0600BBD9 RID: 48089 RVA: 0x00557464 File Offset: 0x00555664
		public int GetIsOnCount()
		{
			int count = 0;
			foreach (CToggle toggle in this.toggleList)
			{
				bool isOn = toggle.isOn;
				if (isOn)
				{
					count++;
				}
			}
			return count;
		}

		// Token: 0x040090B2 RID: 37042
		[SerializeField]
		private List<CToggle> toggleList = new List<CToggle>();

		// Token: 0x040090B3 RID: 37043
		[Tooltip("是否允许一个都不选中")]
		public bool allowSwitchOff;

		// Token: 0x040090B4 RID: 37044
		[Tooltip("是否允许再次点击以取消选中")]
		public bool allowUncheck = true;

		// Token: 0x040090B5 RID: 37045
		[SerializeField]
		[Tooltip("多选最大数量，0表示不限数量")]
		private int maxSelectCount;

		// Token: 0x040090B6 RID: 37046
		[SerializeField]
		[Tooltip("当超过多选数量时，是否自动取消最前面的选择")]
		private bool autoCancelFront = true;

		// Token: 0x040090B7 RID: 37047
		private readonly List<int> _activeIndices = new List<int>();

		// Token: 0x040090B9 RID: 37049
		private bool _initialized;
	}
}
