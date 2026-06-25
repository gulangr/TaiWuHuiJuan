using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F9B RID: 3995
	public class CommonHotkeyDisplay : MonoBehaviour
	{
		// Token: 0x0600B797 RID: 46999 RVA: 0x0053A670 File Offset: 0x00538870
		private void Awake()
		{
			bool flag = this.type != EHotKeyDisplayType.Count;
			if (flag)
			{
				this.Refresh(this.type);
			}
		}

		// Token: 0x0600B798 RID: 47000 RVA: 0x0053A69C File Offset: 0x0053889C
		public void Refresh(EHotKeyDisplayType displayType)
		{
			this.Refresh((short)displayType);
		}

		// Token: 0x0600B799 RID: 47001 RVA: 0x0053A6A8 File Offset: 0x005388A8
		public void Refresh(short templateId)
		{
			HotKeyDisplayItem config = HotKeyDisplay.Instance[templateId];
			this.content.gameObject.SetActive(this.newStyle);
			this.layout.gameObject.SetActive(!this.newStyle);
			bool flag = this.type == EHotKeyDisplayType.LoadingLast || this.type == EHotKeyDisplayType.LoadingNext;
			if (flag)
			{
				this.RefreshLoading();
			}
			else
			{
				bool flag2 = this.newStyle;
				if (flag2)
				{
					this.RefreshDisplay(config.DisplayText, config.Params);
				}
				else
				{
					this.RefreshInner(config.DisplayText, config.Params);
				}
			}
		}

		// Token: 0x0600B79A RID: 47002 RVA: 0x0053A74A File Offset: 0x0053894A
		private void RefreshLoading()
		{
		}

		// Token: 0x0600B79B RID: 47003 RVA: 0x0053A750 File Offset: 0x00538950
		private void RefreshDisplay(string displayText, List<HotkeyIndex> commands)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			List<string> shortcutsLabel = EasyPool.Get<List<string>>();
			for (int i = 0; i < commands.Count; i++)
			{
				ValueTuple<List<string>, KeyCode[]> commandStringAndKeyCodeList = this.GetCommandStringAndKeyCodeList(commands[i]);
				List<string> commandStringList = commandStringAndKeyCodeList.Item1;
				KeyCode[] keyCodeArray = commandStringAndKeyCodeList.Item2;
				bool flag = commandStringList.Count <= 1;
				if (flag)
				{
					shortcutsLabel.Add(this.GetCommandText(commandStringList.FirstOrDefault<string>()));
				}
				else
				{
					for (int j = 0; j < commandStringList.Count; j++)
					{
						bool flag2 = j > 0;
						if (flag2)
						{
							sb.Append('+');
						}
						sb.Append(this.GetCommandText(commandStringList[j]));
					}
					shortcutsLabel.Add(sb.ToString());
				}
			}
			this.SetData(shortcutsLabel, displayText);
			EasyPool.Free<StringBuilder>(sb);
			EasyPool.Free<List<string>>(shortcutsLabel);
		}

		// Token: 0x0600B79C RID: 47004 RVA: 0x0053A83C File Offset: 0x00538A3C
		private string GetCommandText(string commandString)
		{
			if (!true)
			{
			}
			string result;
			if (!(commandString == "Mouse0"))
			{
				if (!(commandString == "Mouse1"))
				{
					if (!(commandString == "Esc"))
					{
						if (!(commandString == "Space"))
						{
							result = commandString;
						}
						else
						{
							result = LocalStringManager.Get(LanguageKey.LK_ShortCuts_Space);
						}
					}
					else
					{
						result = LocalStringManager.Get(LanguageKey.LK_ShortCuts_ESC);
					}
				}
				else
				{
					result = LocalStringManager.Get(LanguageKey.LK_ShortCuts_RightMouse);
				}
			}
			else
			{
				result = LocalStringManager.Get(LanguageKey.LK_ShortCuts_LeftMouse);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600B79D RID: 47005 RVA: 0x0053A8C8 File Offset: 0x00538AC8
		private void SetData(List<string> shortcutsLabel, string desc)
		{
			this.shortcutsHolder.Rebuild((shortcutsLabel != null) ? shortcutsLabel.Count : 0, delegate(Refers refer, int i)
			{
				refer.CGet<TextMeshProUGUI>("HotKeyLabel").text = shortcutsLabel[i];
			});
			this.desc.text = desc;
		}

		// Token: 0x0600B79E RID: 47006 RVA: 0x0053A920 File Offset: 0x00538B20
		private void RefreshInner(string displayText, List<HotkeyIndex> commands)
		{
			foreach (object obj in this.layout)
			{
				Transform child = (Transform)obj;
				Object.Destroy(child.gameObject);
			}
			string[] parts = displayText.Split(new char[]
			{
				'{',
				'}'
			});
			int commandIndex = 0;
			for (int i = 0; i < parts.Length; i++)
			{
				bool flag = i % 2 == 0;
				if (flag)
				{
					bool flag2 = !string.IsNullOrEmpty(parts[i]);
					if (flag2)
					{
						TextMeshProUGUI textInstance = Object.Instantiate<TextMeshProUGUI>(this.textTemplate, this.layout);
						textInstance.text = parts[i];
						textInstance.gameObject.SetActive(true);
					}
				}
				else
				{
					bool flag3 = commandIndex < commands.Count;
					if (flag3)
					{
						ValueTuple<List<string>, KeyCode[]> commandStringAndKeyCodeList = this.GetCommandStringAndKeyCodeList(commands[commandIndex]);
						List<string> commandStringList = commandStringAndKeyCodeList.Item1;
						KeyCode[] keyCodeArray = commandStringAndKeyCodeList.Item2;
						for (int j = 0; j < commandStringList.Count; j++)
						{
							bool flag4 = j > 0;
							if (flag4)
							{
								TextMeshProUGUI textInstance2 = Object.Instantiate<TextMeshProUGUI>(this.textTemplate, this.layout);
								textInstance2.text = "+";
								textInstance2.gameObject.SetActive(true);
							}
							string mouseIcon;
							bool flag5 = CommonHotkeyDisplay.TryGetMouseCommandIcon(keyCodeArray[j], out mouseIcon);
							if (flag5)
							{
								CImage mouseInstance = Object.Instantiate<CImage>(this.mouseTemplate, this.layout);
								mouseInstance.gameObject.SetActive(true);
								mouseInstance.SetSprite(mouseIcon, false, null);
							}
							else
							{
								Refers hotkeyInstance = Object.Instantiate<Refers>(this.hotkeyTemplate, this.layout);
								this.RefreshNormalCommand(hotkeyInstance, commandStringList[j]);
								hotkeyInstance.gameObject.SetActive(true);
							}
						}
						commandIndex++;
					}
				}
			}
		}

		// Token: 0x0600B79F RID: 47007 RVA: 0x0053AB24 File Offset: 0x00538D24
		public static bool TryGetMouseCommandIcon(KeyCode keyCode, out string icon)
		{
			bool result;
			switch (keyCode)
			{
			case KeyCode.Mouse0:
				icon = "ui9_back_mousetip_mousekey_0";
				result = true;
				break;
			case KeyCode.Mouse1:
				icon = "ui9_back_mousetip_mousekey_1";
				result = true;
				break;
			case KeyCode.Mouse2:
				icon = "sp_mousekey_2";
				result = true;
				break;
			case KeyCode.Mouse3:
				icon = "sp_mousekey_3";
				result = true;
				break;
			case KeyCode.Mouse4:
				icon = "sp_mousekey_4";
				result = true;
				break;
			default:
				icon = null;
				result = false;
				break;
			}
			return result;
		}

		// Token: 0x0600B7A0 RID: 47008 RVA: 0x0053AB98 File Offset: 0x00538D98
		private void RefreshNormalCommand(Refers hotkeyItem, string commandKey)
		{
			TextMeshProUGUI hotkeyLabel = hotkeyItem.CGet<TextMeshProUGUI>("HotkeyLabel");
			hotkeyLabel.text = commandKey;
		}

		// Token: 0x0600B7A1 RID: 47009 RVA: 0x0053ABBC File Offset: 0x00538DBC
		private ValueTuple<List<string>, KeyCode[]> GetCommandStringAndKeyCodeList(HotkeyIndex command)
		{
			CommandKitBase commandKit = CommandKitBase.CommandKitArray.Find((CommandKitBase g) => g.Id == command.GroupId);
			HotKeyCommand commandItem = commandKit.TipDisplayCommandArray.Find((HotKeyCommand c) => c.Id == command.CommandId);
			KeyCode[] keyCodes = commandItem.GetKeyCode(false);
			List<string> result = (from k in keyCodes
			select commandItem.GetKeyCodeString(k)).ToList<string>();
			return new ValueTuple<List<string>, KeyCode[]>(result, keyCodes);
		}

		// Token: 0x04008E92 RID: 36498
		[SerializeField]
		private RectTransform layout;

		// Token: 0x04008E93 RID: 36499
		[SerializeField]
		private TextMeshProUGUI textTemplate;

		// Token: 0x04008E94 RID: 36500
		[SerializeField]
		private Refers hotkeyTemplate;

		// Token: 0x04008E95 RID: 36501
		[SerializeField]
		private CImage mouseTemplate;

		// Token: 0x04008E96 RID: 36502
		[SerializeField]
		private RectTransform content;

		// Token: 0x04008E97 RID: 36503
		[SerializeField]
		private TemplatedContainerAssembly shortcutsHolder;

		// Token: 0x04008E98 RID: 36504
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x04008E99 RID: 36505
		private short _templateId;

		// Token: 0x04008E9A RID: 36506
		[SerializeField]
		private EHotKeyDisplayType type = EHotKeyDisplayType.Count;

		// Token: 0x04008E9B RID: 36507
		[SerializeField]
		private bool newStyle = true;
	}
}
