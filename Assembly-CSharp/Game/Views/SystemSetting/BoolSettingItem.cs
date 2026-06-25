using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200075D RID: 1885
	public class BoolSettingItem : SettingItemBase<bool>
	{
		// Token: 0x06005B36 RID: 23350 RVA: 0x002A5467 File Offset: 0x002A3667
		private void Awake()
		{
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
		}

		// Token: 0x06005B37 RID: 23351 RVA: 0x002A5488 File Offset: 0x002A3688
		public override void Initialize(ISettingItemInfo info)
		{
			base.Initialize(info);
			bool flag = this._typedInfo.Attribute.Key == ESettingKey.SectStory;
			if (flag)
			{
				bool isInGame = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
				bool flag2 = isInGame;
				if (flag2)
				{
					sbyte orgTemplateId = (sbyte)this._typedInfo.Attribute.Order;
					Action<int> callback = new Action<int>(this.SetSectStoryStatus);
					UIElement sectStoryPopUpToggle = UIElement.SectStoryPopUpToggle;
					SectMainSettings.GetSectMainStoryIsActive(orgTemplateId, callback, (sectStoryPopUpToggle != null) ? sectStoryPopUpToggle.UiBaseAs<ViewSystemSetting>() : null);
				}
				else
				{
					this._value = false;
					this.toggle.isOn = this._value;
					TooltipInvoker tip = this.toggle.GetComponent<TooltipInvoker>();
					tip.Type = TipType.SingleDesc;
					tip.PresetParam[0] = LanguageKey.LK_SystemSetting_BaseSettings_NeedGameStart.Tr();
				}
			}
			else
			{
				this._value = this._typedInfo.GetValue();
				this.toggle.isOn = this._value;
				this.SetMouseTip();
			}
		}

		// Token: 0x06005B38 RID: 23352 RVA: 0x002A5574 File Offset: 0x002A3774
		private void OnToggleChanged(bool value)
		{
			this._value = value;
			base.InvokeTypedValueChanged(value);
			this.SetMouseTip();
		}

		// Token: 0x06005B39 RID: 23353 RVA: 0x002A5590 File Offset: 0x002A3790
		private void SetMouseTip()
		{
			TooltipInvoker tip = this.toggle.GetComponent<TooltipInvoker>();
			BoolSettingAttribute boolSettingAttribute = this._typedInfo.Attribute as BoolSettingAttribute;
			bool flag = boolSettingAttribute != null && boolSettingAttribute.ExtraTipLanguageKeys != null;
			if (flag)
			{
				tip.enabled = true;
				LanguageKey[] languageKeys = boolSettingAttribute.ExtraTipLanguageKeys;
				tip.Type = boolSettingAttribute.TipType;
				bool flag2 = languageKeys.Length == 2;
				if (flag2)
				{
					bool flag3 = boolSettingAttribute.TipType == TipType.Simple;
					if (flag3)
					{
						tip.Type = TipType.Simple;
						tip.PresetParam[0] = languageKeys[0].Tr();
						tip.PresetParam[1] = languageKeys[1].Tr();
					}
					else
					{
						tip.Type = TipType.SingleDesc;
						tip.PresetParam[0] = languageKeys[this._value ? 0 : 1].Tr();
					}
				}
				else
				{
					tip.Type = TipType.Simple;
					tip.PresetParam[0] = languageKeys[this._value ? 0 : 2].Tr();
					tip.PresetParam[1] = languageKeys[this._value ? 1 : 3].Tr();
				}
				tip.Refresh(false, -1);
			}
			else
			{
				tip.enabled = false;
			}
		}

		// Token: 0x06005B3A RID: 23354 RVA: 0x002A56B0 File Offset: 0x002A38B0
		public override object GetValue()
		{
			return this._value;
		}

		// Token: 0x06005B3B RID: 23355 RVA: 0x002A56C0 File Offset: 0x002A38C0
		public override void SetValue(object value)
		{
			this._value = (bool)value;
			bool flag = this.toggle != null;
			if (flag)
			{
				this.toggle.isOn = this._value;
			}
		}

		// Token: 0x06005B3C RID: 23356 RVA: 0x002A56FC File Offset: 0x002A38FC
		public override void SetTypedValue(bool value)
		{
			this._value = value;
			bool flag = this.toggle != null;
			if (flag)
			{
				this.toggle.isOn = this._value;
			}
		}

		// Token: 0x06005B3D RID: 23357 RVA: 0x002A5734 File Offset: 0x002A3934
		public override void SetInteractable(bool interactable)
		{
			bool flag = this.toggle != null;
			if (flag)
			{
				bool flag2 = this._typedInfo.Attribute.Key == ESettingKey.SectStory;
				if (flag2)
				{
					bool isInGame = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
					this.toggle.interactable = (isInGame && interactable);
				}
				else
				{
					this.toggle.interactable = interactable;
				}
				(this.toggle.graphic as CImage).sprite = this.toggleCheckMarkSprites[this.toggle.interactable ? 0 : 1];
			}
		}

		// Token: 0x06005B3E RID: 23358 RVA: 0x002A57C8 File Offset: 0x002A39C8
		private void OnDestroy()
		{
			bool flag = this.toggle != null;
			if (flag)
			{
				this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleChanged));
			}
		}

		// Token: 0x06005B3F RID: 23359 RVA: 0x002A5804 File Offset: 0x002A3A04
		private void SetSectStoryStatus(int status)
		{
			bool isPaused = status == -1;
			this._value = !isPaused;
			this.toggle.isOn = !isPaused;
			this.SetMouseTip();
		}

		// Token: 0x04003EED RID: 16109
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04003EEE RID: 16110
		[SerializeField]
		private Sprite[] toggleCheckMarkSprites;

		// Token: 0x04003EEF RID: 16111
		private bool _value;
	}
}
