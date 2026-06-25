using System;
using System.Collections.Generic;
using FrameWork.ModSystem;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Mod
{
	// Token: 0x020008C8 RID: 2248
	public class DropdownSettingItem : SettingItemBase
	{
		// Token: 0x06006B42 RID: 27458 RVA: 0x00318F41 File Offset: 0x00317141
		private void Awake()
		{
			this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnDropdownChanged));
		}

		// Token: 0x06006B43 RID: 27459 RVA: 0x00318F64 File Offset: 0x00317164
		public override void Initialize(SettingEntry entry)
		{
			base.Initialize(entry);
			this._typedEntry = (DropdownSetting)entry;
			this.dropdown.ClearOptions();
			bool flag = this._typedEntry.Options != null && this._typedEntry.Options.Count > 0;
			if (flag)
			{
				List<CDropdown.OptionData> optionDataList = new List<CDropdown.OptionData>();
				foreach (string optionText in this._typedEntry.Options)
				{
					optionDataList.Add(new CDropdown.OptionData(optionText));
				}
				this.dropdown.AddOptions(optionDataList);
			}
			this._tempValue = this._typedEntry.Value;
			this.dropdown.SetValueWithoutNotify(this._tempValue);
			this._isInitialized = true;
		}

		// Token: 0x06006B44 RID: 27460 RVA: 0x00319050 File Offset: 0x00317250
		public override void SetWithoutNotify()
		{
			this.dropdown.SetValueWithoutNotify(this._tempValue);
		}

		// Token: 0x06006B45 RID: 27461 RVA: 0x00319068 File Offset: 0x00317268
		private void OnDropdownChanged(int index)
		{
			bool flag = !this._isInitialized;
			if (!flag)
			{
				bool changed = this._tempValue != index;
				this._tempValue = index;
				base.NotifyValueChanged(changed);
			}
		}

		// Token: 0x06006B46 RID: 27462 RVA: 0x003190A4 File Offset: 0x003172A4
		public override void SetInteractable(bool interactable)
		{
			bool flag = this.dropdown != null;
			if (flag)
			{
				this.dropdown.interactable = interactable;
			}
		}

		// Token: 0x06006B47 RID: 27463 RVA: 0x003190D0 File Offset: 0x003172D0
		public override void ApplyValue()
		{
			bool flag = this._typedEntry != null;
			if (flag)
			{
				this._typedEntry.Value = this._tempValue;
			}
		}

		// Token: 0x06006B48 RID: 27464 RVA: 0x00319100 File Offset: 0x00317300
		public override void ResetValue()
		{
			bool flag = this._typedEntry != null;
			if (flag)
			{
				this._tempValue = this._typedEntry.Value;
				this.SetWithoutNotify();
			}
		}

		// Token: 0x06006B49 RID: 27465 RVA: 0x00319138 File Offset: 0x00317338
		private void OnDestroy()
		{
			bool flag = this.dropdown != null;
			if (flag)
			{
				this.dropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnDropdownChanged));
			}
		}

		// Token: 0x04004DE0 RID: 19936
		[SerializeField]
		private CDropdown dropdown;

		// Token: 0x04004DE1 RID: 19937
		private DropdownSetting _typedEntry;

		// Token: 0x04004DE2 RID: 19938
		private bool _isInitialized;

		// Token: 0x04004DE3 RID: 19939
		private int _tempValue;
	}
}
