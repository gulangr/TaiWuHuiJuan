using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Common
{
	// Token: 0x02000F99 RID: 3993
	[RequireComponent(typeof(CToggleGroup))]
	public class ToggleGroupHotkeyController : MonoBehaviour
	{
		// Token: 0x0600B790 RID: 46992 RVA: 0x0053A330 File Offset: 0x00538530
		public static void Set(UIElement element, CToggleGroup toggleGroup, sbyte level = 0, Func<bool> checkFunc = null)
		{
			bool flag = element == null;
			if (flag)
			{
				Debug.LogWarning("element is null");
			}
			ToggleGroupHotkeyController controller;
			bool flag2 = !toggleGroup.TryGetComponent<ToggleGroupHotkeyController>(out controller);
			if (flag2)
			{
				controller = toggleGroup.gameObject.AddComponent<ToggleGroupHotkeyController>();
			}
			controller._toggleGroup = toggleGroup;
			controller._element = element;
			controller._checkFunc = checkFunc;
			controller._level = level;
			controller._isDualButtonMode = false;
		}

		// Token: 0x0600B791 RID: 46993 RVA: 0x0053A394 File Offset: 0x00538594
		public static void Set(UIElement element, Button prevButton, Button nextButton, sbyte level = 0, Func<bool> checkFunc = null)
		{
			GameObject go = (prevButton != null) ? prevButton.gameObject : ((nextButton != null) ? nextButton.gameObject : null);
			bool flag = go == null;
			if (!flag)
			{
				ToggleGroupHotkeyController controller;
				bool flag2 = !go.TryGetComponent<ToggleGroupHotkeyController>(out controller);
				if (flag2)
				{
					controller = go.AddComponent<ToggleGroupHotkeyController>();
				}
				controller._prevButton = prevButton;
				controller._nextButton = nextButton;
				controller._element = element;
				controller._checkFunc = checkFunc;
				controller._level = level;
				controller._isDualButtonMode = true;
			}
		}

		// Token: 0x0600B792 RID: 46994 RVA: 0x0053A418 File Offset: 0x00538618
		private void Update()
		{
			bool flag = this._element == null;
			if (!flag)
			{
				bool isDualButtonMode = this._isDualButtonMode;
				if (isDualButtonMode)
				{
					Func<bool> checkFunc = this._checkFunc;
					bool flag2 = checkFunc == null || checkFunc();
					if (flag2)
					{
						HotKeyCommand tabSwitchCommand = this.GetTabSwitchCommand(true);
						bool flag3 = tabSwitchCommand != null && tabSwitchCommand.Check(this._element, false, false, false, true, false) && this._prevButton.interactable && this._prevButton.gameObject.activeInHierarchy;
						if (flag3)
						{
							Button prevButton = this._prevButton;
							if (prevButton != null)
							{
								Button.ButtonClickedEvent onClick = prevButton.onClick;
								if (onClick != null)
								{
									onClick.Invoke();
								}
							}
						}
						HotKeyCommand tabSwitchCommand2 = this.GetTabSwitchCommand(false);
						bool flag4 = tabSwitchCommand2 != null && tabSwitchCommand2.Check(this._element, false, false, false, true, false) && this._nextButton.interactable && this._nextButton.gameObject.activeInHierarchy;
						if (flag4)
						{
							Button nextButton = this._nextButton;
							if (nextButton != null)
							{
								Button.ButtonClickedEvent onClick2 = nextButton.onClick;
								if (onClick2 != null)
								{
									onClick2.Invoke();
								}
							}
						}
					}
				}
				else
				{
					Func<bool> checkFunc2 = this._checkFunc;
					bool flag5 = (checkFunc2 != null) ? checkFunc2() : this._toggleGroup.gameObject.activeInHierarchy;
					if (flag5)
					{
						HotKeyCommand tabSwitchCommand3 = this.GetTabSwitchCommand(true);
						bool flag6 = tabSwitchCommand3 != null && tabSwitchCommand3.Check(this._element, false, false, false, true, false);
						if (flag6)
						{
							CToggleGroup toggleGroup = this._toggleGroup;
							if (toggleGroup != null)
							{
								toggleGroup.SelectNext(-1, false);
							}
						}
						HotKeyCommand tabSwitchCommand4 = this.GetTabSwitchCommand(false);
						bool flag7 = tabSwitchCommand4 != null && tabSwitchCommand4.Check(this._element, false, false, false, true, false);
						if (flag7)
						{
							CToggleGroup toggleGroup2 = this._toggleGroup;
							if (toggleGroup2 != null)
							{
								toggleGroup2.SelectNext(1, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600B793 RID: 46995 RVA: 0x0053A5CC File Offset: 0x005387CC
		private HotKeyCommand GetTabSwitchCommand(bool isPrev)
		{
			sbyte level = this._level;
			if (!true)
			{
			}
			HotKeyCommand result;
			switch (level)
			{
			case 0:
				result = (isPrev ? TabSwitchCommandKit.PrevTabLevel1 : TabSwitchCommandKit.NextTabLevel1);
				break;
			case 1:
				result = (isPrev ? TabSwitchCommandKit.PrevTabLevel2 : TabSwitchCommandKit.NextTabLevel2);
				break;
			case 2:
				result = (isPrev ? TabSwitchCommandKit.PrevTabLevel3 : TabSwitchCommandKit.NextTabLevel3);
				break;
			default:
				result = null;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04008E8A RID: 36490
		private CToggleGroup _toggleGroup;

		// Token: 0x04008E8B RID: 36491
		private Func<bool> _checkFunc;

		// Token: 0x04008E8C RID: 36492
		private UIElement _element;

		// Token: 0x04008E8D RID: 36493
		private sbyte _level;

		// Token: 0x04008E8E RID: 36494
		private Button _prevButton;

		// Token: 0x04008E8F RID: 36495
		private Button _nextButton;

		// Token: 0x04008E90 RID: 36496
		private bool _isDualButtonMode;
	}
}
