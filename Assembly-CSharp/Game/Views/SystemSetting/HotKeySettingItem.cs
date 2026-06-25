using System;
using UnityEngine;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000762 RID: 1890
	public class HotKeySettingItem : SettingItemBase<HotKeyCommand>
	{
		// Token: 0x06005B6A RID: 23402 RVA: 0x002A7330 File Offset: 0x002A5530
		public override object GetValue()
		{
			return this._command;
		}

		// Token: 0x06005B6B RID: 23403 RVA: 0x002A7348 File Offset: 0x002A5548
		public override void SetTypedValue(HotKeyCommand value)
		{
			this._command = value;
			this.RefreshDisplay();
		}

		// Token: 0x06005B6C RID: 23404 RVA: 0x002A735C File Offset: 0x002A555C
		public override void SetValue(object value)
		{
			HotKeyCommand cmd = value as HotKeyCommand;
			bool flag = cmd != null;
			if (flag)
			{
				this.SetTypedValue(cmd);
			}
		}

		// Token: 0x06005B6D RID: 23405 RVA: 0x002A7384 File Offset: 0x002A5584
		public override void Initialize(ISettingItemInfo info)
		{
			base.Initialize(info);
			HotKeySettingItemInfo hotKeyInfo = info as HotKeySettingItemInfo;
			bool flag = hotKeyInfo != null;
			if (flag)
			{
				this._command = hotKeyInfo.Command;
				this._kitId = hotKeyInfo.KitId;
				this._subCategory = hotKeyInfo.SubCategory;
				this.RefreshDisplay();
			}
		}

		// Token: 0x06005B6E RID: 23406 RVA: 0x002A73D8 File Offset: 0x002A55D8
		public void RefreshDisplay()
		{
			bool flag = this._command == null;
			if (!flag)
			{
				bool keyConflict = HotKeyService.IsCommandKeyConflict(this._command);
				bool mouseKeyConflict = HotKeyService.IsCommandMouseKeyConflict(this._command);
				this.SetConflictState(keyConflict || mouseKeyConflict);
				bool flag2 = this.keyboardKeyButton != null;
				if (flag2)
				{
					this.keyboardKeyButton.Set(this._command, false, this._command.CanSet, delegate
					{
						this.StartEdit(true);
					}, keyConflict);
				}
				bool flag3 = this.mouseKeyButton != null;
				if (flag3)
				{
					this.mouseKeyButton.Set(this._command, true, this._command.MouseKeyCanSet, delegate
					{
						this.StartEdit(false);
					}, mouseKeyConflict);
				}
			}
		}

		// Token: 0x06005B6F RID: 23407 RVA: 0x002A7498 File Offset: 0x002A5698
		private void StartEdit(bool isKeyboard)
		{
			this._isEditingKeyboard = isKeyboard;
			HotKeySettingButton triggerButton = isKeyboard ? this.keyboardKeyButton : this.mouseKeyButton;
			bool flag = this._viewSystemSetting != null;
			if (flag)
			{
				this._viewSystemSetting.ShowHotKeyEditPanel(this._command, this._kitId, this._subCategory, !isKeyboard, new Action(this.EndEdit), ((triggerButton != null) ? triggerButton.transform : null) as RectTransform);
			}
		}

		// Token: 0x06005B70 RID: 23408 RVA: 0x002A7510 File Offset: 0x002A5710
		public void EndEdit()
		{
			bool flag = this._command == null;
			if (!flag)
			{
				this.RefreshDisplay();
				base.NotifyChanged();
			}
		}

		// Token: 0x06005B71 RID: 23409 RVA: 0x002A753C File Offset: 0x002A573C
		public void SetConflictState(bool isConflict)
		{
			bool flag = this.conflictBg != null;
			if (flag)
			{
				this.conflictBg.gameObject.SetActive(isConflict);
			}
		}

		// Token: 0x06005B72 RID: 23410 RVA: 0x002A7570 File Offset: 0x002A5770
		public override void SetInteractable(bool interactable)
		{
			this._interactable = interactable;
			HotKeyCommand command = this._command;
			bool canSet = command != null && command.CanSet;
			bool actualInteractable = this._interactable && canSet;
			bool flag = this.keyboardKeyButton != null;
			if (flag)
			{
				this.keyboardKeyButton.SetInteractable(actualInteractable);
			}
			bool flag2 = this.mouseKeyButton != null;
			if (flag2)
			{
				HotKeySettingButton hotKeySettingButton = this.mouseKeyButton;
				bool interactable2;
				if (this._interactable)
				{
					HotKeyCommand command2 = this._command;
					interactable2 = (command2 != null && command2.MouseKeyCanSet);
				}
				else
				{
					interactable2 = false;
				}
				hotKeySettingButton.SetInteractable(interactable2);
			}
		}

		// Token: 0x04003F10 RID: 16144
		[SerializeField]
		private HotKeySettingButton keyboardKeyButton;

		// Token: 0x04003F11 RID: 16145
		[SerializeField]
		private HotKeySettingButton mouseKeyButton;

		// Token: 0x04003F12 RID: 16146
		[SerializeField]
		private CImage conflictBg;

		// Token: 0x04003F13 RID: 16147
		private HotKeyCommand _command;

		// Token: 0x04003F14 RID: 16148
		private byte _kitId;

		// Token: 0x04003F15 RID: 16149
		private ESettingSubCategory _subCategory;

		// Token: 0x04003F16 RID: 16150
		private bool _isEditingKeyboard = true;

		// Token: 0x04003F17 RID: 16151
		private ViewSystemSetting _viewSystemSetting = UIElement.SystemSetting.UiBaseAs<ViewSystemSetting>();

		// Token: 0x04003F18 RID: 16152
		private bool _interactable = true;
	}
}
