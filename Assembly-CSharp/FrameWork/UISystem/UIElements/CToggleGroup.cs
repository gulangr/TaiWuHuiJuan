using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x0200100B RID: 4107
	public class CToggleGroup : MonoBehaviour, ICToggleGroup
	{
		// Token: 0x14000092 RID: 146
		// (add) Token: 0x0600BBA2 RID: 48034 RVA: 0x00555F64 File Offset: 0x00554164
		// (remove) Token: 0x0600BBA3 RID: 48035 RVA: 0x00555F9C File Offset: 0x0055419C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, int> OnActiveIndexChange;

		// Token: 0x0600BBA4 RID: 48036 RVA: 0x00555FD4 File Offset: 0x005541D4
		public void Init(int targetIndex = -1)
		{
			bool initialized = this._initialized;
			if (!initialized)
			{
				int preOnIndex = -1;
				for (int i = 0; i < this.toggleList.Count; i++)
				{
					CToggle toggle = this.toggleList[i];
					toggle.Register(this);
					bool isOn = toggle.isOn;
					if (isOn)
					{
						bool flag = !this.allowSwitchOff && preOnIndex < 0;
						if (flag)
						{
							preOnIndex = i;
						}
						else
						{
							toggle.isOn = false;
						}
					}
				}
				bool flag2 = preOnIndex < 0 && !this.allowSwitchOff && this.toggleList.Count > 0;
				if (flag2)
				{
					preOnIndex = (this.toggleList.CheckIndex(targetIndex) ? targetIndex : 0);
				}
				bool flag3 = preOnIndex >= 0;
				if (flag3)
				{
					this.toggleList[preOnIndex].isOn = true;
					this._activeIndex = preOnIndex;
				}
				this._initialized = true;
			}
		}

		// Token: 0x0600BBA5 RID: 48037 RVA: 0x005560C0 File Offset: 0x005542C0
		public List<CToggle> GetAll()
		{
			return this.toggleList;
		}

		// Token: 0x0600BBA6 RID: 48038 RVA: 0x005560D8 File Offset: 0x005542D8
		public CToggle Get(int index)
		{
			return this.toggleList.CheckIndex(index) ? this.toggleList[index] : null;
		}

		// Token: 0x0600BBA7 RID: 48039 RVA: 0x00556108 File Offset: 0x00554308
		public void SetInteractable(bool interactable)
		{
			foreach (CToggle t in this.toggleList)
			{
				t.interactable = interactable;
			}
		}

		// Token: 0x0600BBA8 RID: 48040 RVA: 0x00556160 File Offset: 0x00554360
		public void SetInteractable(bool interactable, int index)
		{
			bool flag = this.toggleList.CheckIndex(index);
			if (flag)
			{
				this.toggleList[index].interactable = interactable;
			}
		}

		// Token: 0x0600BBA9 RID: 48041 RVA: 0x00556194 File Offset: 0x00554394
		[ContextMenu("Add All Child Toggles")]
		public void AddAllChildToggles()
		{
			this.Clear();
			foreach (CToggle toggle in base.transform.GetComponentsInTopChildren(false))
			{
				this.Add(toggle);
			}
		}

		// Token: 0x0600BBAA RID: 48042 RVA: 0x005561D4 File Offset: 0x005543D4
		public void DeSelect(bool forceRaiseEvent = false)
		{
			bool flag = this._activeIndex < 0;
			if (!flag)
			{
				CToggle toggle = this.toggleList[this._activeIndex];
				bool flag2 = !forceRaiseEvent && !toggle.isOn;
				if (!flag2)
				{
					toggle.isOn = false;
					int oldIndex = this._activeIndex;
					this._activeIndex = -1;
					Action<int, int> onActiveIndexChange = this.OnActiveIndexChange;
					if (onActiveIndexChange != null)
					{
						onActiveIndexChange(-1, oldIndex);
					}
				}
			}
		}

		// Token: 0x0600BBAB RID: 48043 RVA: 0x00556244 File Offset: 0x00554444
		public void DeSelectWithoutNotify()
		{
			bool flag = this._activeIndex < 0;
			if (!flag)
			{
				CToggle toggle = this.toggleList[this._activeIndex];
				bool flag2 = !toggle.isOn;
				if (!flag2)
				{
					toggle.isOn = false;
					this._activeIndex = -1;
				}
			}
		}

		// Token: 0x0600BBAC RID: 48044 RVA: 0x00556294 File Offset: 0x00554494
		public void Set(int index, bool forceRaiseEvent = false)
		{
			bool flag = !this.toggleList.CheckIndex(index);
			if (!flag)
			{
				CToggle toggle = this.toggleList[index];
				bool flag2 = !forceRaiseEvent && toggle.isOn;
				if (!flag2)
				{
					int oldIndex = this._activeIndex;
					bool flag3 = oldIndex >= 0 && oldIndex != index;
					if (flag3)
					{
						this.toggleList[oldIndex].isOn = false;
					}
					toggle.isOn = true;
					this._activeIndex = index;
					Action<int, int> onActiveIndexChange = this.OnActiveIndexChange;
					if (onActiveIndexChange != null)
					{
						onActiveIndexChange(index, oldIndex);
					}
				}
			}
		}

		// Token: 0x0600BBAD RID: 48045 RVA: 0x00556328 File Offset: 0x00554528
		public void SetWithoutNotify(int index)
		{
			bool flag = !this.toggleList.CheckIndex(index);
			if (!flag)
			{
				CToggle toggle = this.toggleList[index];
				bool isOn = toggle.isOn;
				if (!isOn)
				{
					bool flag2 = this._activeIndex >= 0 && this._activeIndex != index;
					if (flag2)
					{
						this.toggleList[this._activeIndex].isOn = false;
					}
					toggle.isOn = true;
					this._activeIndex = index;
				}
			}
		}

		// Token: 0x0600BBAE RID: 48046 RVA: 0x005563A8 File Offset: 0x005545A8
		public int GetActiveIndex()
		{
			return this._activeIndex;
		}

		// Token: 0x0600BBAF RID: 48047 RVA: 0x005563C0 File Offset: 0x005545C0
		public void SetToFirstInteractable(bool forceNotifyEvent = false)
		{
			bool needNotify = this._activeIndex < 0 || forceNotifyEvent;
			int oldIndex = this._activeIndex;
			bool flag = this._activeIndex >= 0;
			if (flag)
			{
				this.toggleList[this._activeIndex].isOn = false;
			}
			this._activeIndex = -1;
			for (int i = 0; i < this.toggleList.Count; i++)
			{
				CToggle toggle = this.toggleList[i];
				bool flag2 = toggle.interactable && toggle.gameObject.activeSelf;
				if (flag2)
				{
					toggle.isOn = true;
					this._activeIndex = i;
					break;
				}
			}
			bool flag3 = needNotify;
			if (flag3)
			{
				Action<int, int> onActiveIndexChange = this.OnActiveIndexChange;
				if (onActiveIndexChange != null)
				{
					onActiveIndexChange(this._activeIndex, oldIndex);
				}
			}
		}

		// Token: 0x0600BBB0 RID: 48048 RVA: 0x00556490 File Offset: 0x00554690
		public int Count()
		{
			return this.toggleList.Count;
		}

		// Token: 0x0600BBB1 RID: 48049 RVA: 0x005564B0 File Offset: 0x005546B0
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
					bool flag4 = this._activeIndex >= 0;
					if (flag4)
					{
						toggle.isOn = false;
					}
					else
					{
						this._activeIndex = this.toggleList.Count - 1;
					}
				}
			}
		}

		// Token: 0x0600BBB2 RID: 48050 RVA: 0x00556554 File Offset: 0x00554754
		public bool CanAddToggle(CToggle toggle)
		{
			bool flag = toggle == null;
			return !flag && !this.toggleList.Contains(toggle);
		}

		// Token: 0x0600BBB3 RID: 48051 RVA: 0x00556584 File Offset: 0x00554784
		public void Remove(int index)
		{
			bool flag = !this.toggleList.CheckIndex(index);
			if (!flag)
			{
				this.RemoveInternal(index);
			}
		}

		// Token: 0x0600BBB4 RID: 48052 RVA: 0x005565B0 File Offset: 0x005547B0
		private void RemoveInternal(int index)
		{
			CToggle toggle = this.toggleList[index];
			toggle.UnRegister();
			this.toggleList.RemoveAt(index);
			bool flag = this._activeIndex == index;
			if (flag)
			{
				this._activeIndex = -1;
				bool flag2 = !this.allowSwitchOff && this.toggleList.Count > 0;
				if (flag2)
				{
					this.toggleList[0].isOn = true;
					this._activeIndex = 0;
				}
			}
			else
			{
				bool flag3 = this._activeIndex > index;
				if (flag3)
				{
					this._activeIndex--;
				}
			}
		}

		// Token: 0x0600BBB5 RID: 48053 RVA: 0x0055664D File Offset: 0x0055484D
		[ContextMenu("Clear All Toggles")]
		public void Clear()
		{
			this.toggleList.Clear();
			this._activeIndex = -1;
			this._initialized = false;
		}

		// Token: 0x0600BBB6 RID: 48054 RVA: 0x0055666C File Offset: 0x0055486C
		public bool AnyTogglesOn()
		{
			return this._activeIndex >= 0;
		}

		// Token: 0x0600BBB7 RID: 48055 RVA: 0x0055668C File Offset: 0x0055488C
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
					bool flag3 = isOn && !this.allowSwitchOff && this._activeIndex == index;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = !this.allowUncheck && isOn;
						result = !flag4;
					}
				}
			}
			return result;
		}

		// Token: 0x0600BBB8 RID: 48056 RVA: 0x00556700 File Offset: 0x00554900
		public void NotifyToggle(CToggle toggle, bool value, bool raiseEvent = true)
		{
			int index = this.toggleList.IndexOf(toggle);
			bool flag = index < 0;
			if (flag)
			{
				throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", toggle, this));
			}
			int oldIndex = this._activeIndex;
			if (value)
			{
				bool flag2 = this._activeIndex >= 0 && this._activeIndex != index;
				if (flag2)
				{
					this.toggleList[this._activeIndex].isOn = false;
				}
				this._activeIndex = index;
				if (raiseEvent)
				{
					Action<int, int> onActiveIndexChange = this.OnActiveIndexChange;
					if (onActiveIndexChange != null)
					{
						onActiveIndexChange(index, oldIndex);
					}
				}
			}
			else
			{
				bool flag3 = this._activeIndex == index;
				if (flag3)
				{
					this._activeIndex = -1;
				}
				if (raiseEvent)
				{
					Action<int, int> onActiveIndexChange2 = this.OnActiveIndexChange;
					if (onActiveIndexChange2 != null)
					{
						onActiveIndexChange2(-1, oldIndex);
					}
				}
			}
		}

		// Token: 0x0600BBB9 RID: 48057 RVA: 0x005567D0 File Offset: 0x005549D0
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

		// Token: 0x0600BBBA RID: 48058 RVA: 0x00556838 File Offset: 0x00554A38
		public int GetFirstInteractable()
		{
			for (int index = 0; index < this.toggleList.Count; index++)
			{
				CToggle toggle = this.toggleList[index];
				bool flag = toggle.gameObject.activeSelf && toggle.interactable;
				if (flag)
				{
					return index;
				}
			}
			return -1;
		}

		// Token: 0x0600BBBB RID: 48059 RVA: 0x00556894 File Offset: 0x00554A94
		public bool SelectNext(int direction, bool forceRaiseEvent = false)
		{
			bool flag = this.toggleList.Count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				direction = ((direction > 0) ? 1 : -1);
				int startIndex = (this._activeIndex >= 0) ? this._activeIndex : ((direction > 0) ? -1 : this.toggleList.Count);
				int count = this.toggleList.Count;
				for (int i = 0; i < count; i++)
				{
					int nextIndex = (startIndex + direction + count) % count;
					CToggle toggle = this.toggleList[nextIndex];
					bool flag2 = toggle.interactable && toggle.gameObject.activeSelf;
					if (flag2)
					{
						this.Set(nextIndex, forceRaiseEvent);
						return true;
					}
					startIndex = nextIndex;
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600BBBC RID: 48060 RVA: 0x0055695A File Offset: 0x00554B5A
		public void ClearOnActiveIndexChange()
		{
			this.OnActiveIndexChange = null;
		}

		// Token: 0x040090AC RID: 37036
		[SerializeField]
		private List<CToggle> toggleList = new List<CToggle>();

		// Token: 0x040090AD RID: 37037
		[Tooltip("是否允许一个都不选中")]
		public bool allowSwitchOff;

		// Token: 0x040090AE RID: 37038
		[Tooltip("是否允许再次点击以取消选中")]
		public bool allowUncheck;

		// Token: 0x040090AF RID: 37039
		private int _activeIndex = -1;

		// Token: 0x040090B1 RID: 37041
		private bool _initialized;
	}
}
