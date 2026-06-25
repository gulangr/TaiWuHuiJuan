using System;
using System.Collections.Generic;
using CommonSortAndFilterLegacy;
using UnityEngine;

namespace FrameWork.UISystem.Components
{
	// Token: 0x0200101D RID: 4125
	public abstract class BaseMultiSelectDropdown<TBarConfig, TItemConfig> : CommonFakeHidePanel
	{
		// Token: 0x0600BCF0 RID: 48368
		protected abstract void SetupMenuBarInternal();

		// Token: 0x0600BCF1 RID: 48369
		protected abstract void SetupMenu();

		// Token: 0x0600BCF2 RID: 48370 RVA: 0x0055E484 File Offset: 0x0055C684
		public IReadOnlyCollection<int> GetSelectedIndices()
		{
			return this.SelectedIndices;
		}

		// Token: 0x1700154F RID: 5455
		// (get) Token: 0x0600BCF3 RID: 48371 RVA: 0x0055E49C File Offset: 0x0055C69C
		// (set) Token: 0x0600BCF4 RID: 48372 RVA: 0x0055E4A4 File Offset: 0x0055C6A4
		public MultiSelectDropdownConfig<TBarConfig, TItemConfig> Config { get; protected set; }

		// Token: 0x0600BCF5 RID: 48373 RVA: 0x0055E4B0 File Offset: 0x0055C6B0
		public virtual void Setup(MultiSelectDropdownConfig<TBarConfig, TItemConfig> config)
		{
			this.Config = config;
			this.SelectedIndices.Clear();
			bool flag = config.DefaultSelectedIndices != null;
			if (flag)
			{
				this.SelectedIndices.UnionWith(config.DefaultSelectedIndices);
			}
			this.SetupMenuBar();
			this.SetupMenu();
			this.HideMenu();
		}

		// Token: 0x0600BCF6 RID: 48374 RVA: 0x0055E508 File Offset: 0x0055C708
		private void SetupMenuBar()
		{
			bool flag = !this.MenuBarInstance;
			if (flag)
			{
				this.MenuBarInstance = this.InstantiateMenuBar();
			}
			this.MenuBarInstance.gameObject.SetActive(true);
			this.SetupMenuBarInternal();
		}

		// Token: 0x0600BCF7 RID: 48375 RVA: 0x0055E550 File Offset: 0x0055C750
		protected virtual GameObject InstantiateMenuBar()
		{
			return Object.Instantiate<GameObject>(this.menuBarTemplate, this.menuBarSlot);
		}

		// Token: 0x0600BCF8 RID: 48376 RVA: 0x0055E574 File Offset: 0x0055C774
		protected void ToggleSelected(int index)
		{
			bool flag = !this.isMultiSelect && !this.SelectedIndices.Contains(index);
			if (flag)
			{
				this.SelectedIndices.Clear();
			}
			bool flag2 = !this.SelectedIndices.Add(index);
			if (flag2)
			{
				this.SelectedIndices.Remove(index);
			}
		}

		// Token: 0x04009159 RID: 37209
		public bool isMultiSelect = true;

		// Token: 0x0400915A RID: 37210
		protected GameObject MenuBarInstance;

		// Token: 0x0400915B RID: 37211
		protected readonly HashSet<int> SelectedIndices = new HashSet<int>();

		// Token: 0x0400915D RID: 37213
		[SerializeField]
		protected GameObject menuBarTemplate;

		// Token: 0x0400915E RID: 37214
		[SerializeField]
		protected GameObject itemTemplate;

		// Token: 0x0400915F RID: 37215
		[SerializeField]
		protected DisableStyleRoot disableStyleRoot;

		// Token: 0x04009160 RID: 37216
		[SerializeField]
		protected RectTransform menuBarSlot;
	}
}
