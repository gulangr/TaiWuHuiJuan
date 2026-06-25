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

// Token: 0x02000214 RID: 532
public class HotkeyDisplay : Refers
{
	// Token: 0x06002231 RID: 8753 RVA: 0x000FD988 File Offset: 0x000FBB88
	private void Awake()
	{
		this.InitRefers();
		bool flag = this.type != EHotKeyDisplayType.Count;
		if (flag)
		{
			this.Refresh(this.type);
		}
	}

	// Token: 0x06002232 RID: 8754 RVA: 0x000FD9BC File Offset: 0x000FBBBC
	private void InitRefers()
	{
		this._layout = base.CGet<RectTransform>("Layout");
		this._textTemplate = base.CGet<TextMeshProUGUI>("TextTemplate");
		this._hotkeyTemplate = base.CGet<Refers>("HotkeyTemplate");
		this._mouseTemplate = base.CGet<CImage>("MouseTemplate");
		this._content = base.CGet<RectTransform>("Content");
		this._shortcutsHolder = base.CGet<TemplatedContainerAssembly>("ShortcutsHolder");
		this._desc = base.CGet<TextMeshProUGUI>("Desc");
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x000FDA41 File Offset: 0x000FBC41
	public void Refresh(EHotKeyDisplayType displayType)
	{
		this.Refresh((short)displayType);
	}

	// Token: 0x06002234 RID: 8756 RVA: 0x000FDA50 File Offset: 0x000FBC50
	public void Refresh(short templateId)
	{
		this.InitRefers();
		HotKeyDisplayItem config = HotKeyDisplay.Instance[templateId];
		this._content.gameObject.SetActive(this.newStyle);
		this._layout.gameObject.SetActive(!this.newStyle);
		bool flag = this.newStyle;
		if (flag)
		{
			this.RefreshDisplay(config.DisplayText, config.Params);
		}
		else
		{
			this.RefreshInner(config.DisplayText, config.Params);
		}
	}

	// Token: 0x06002235 RID: 8757 RVA: 0x000FDAD4 File Offset: 0x000FBCD4
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

	// Token: 0x06002236 RID: 8758 RVA: 0x000FDBC0 File Offset: 0x000FBDC0
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

	// Token: 0x06002237 RID: 8759 RVA: 0x000FDC4C File Offset: 0x000FBE4C
	private void SetData(List<string> shortcutsLabel, string desc)
	{
		this._shortcutsHolder.Rebuild((shortcutsLabel != null) ? shortcutsLabel.Count : 0, delegate(Refers refer, int i)
		{
			refer.CGet<TextMeshProUGUI>("HotKeyLabel").text = shortcutsLabel[i];
		});
		this._desc.text = desc;
	}

	// Token: 0x06002238 RID: 8760 RVA: 0x000FDCA4 File Offset: 0x000FBEA4
	private void RefreshInner(string displayText, List<HotkeyIndex> commands)
	{
		foreach (object obj in this._layout)
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
					TextMeshProUGUI textInstance = Object.Instantiate<TextMeshProUGUI>(this._textTemplate, this._layout);
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
							TextMeshProUGUI textInstance2 = Object.Instantiate<TextMeshProUGUI>(this._textTemplate, this._layout);
							textInstance2.text = "+";
							textInstance2.gameObject.SetActive(true);
						}
						string mouseIcon;
						bool flag5 = HotkeyDisplay.TryGetMouseCommandIcon(keyCodeArray[j], out mouseIcon);
						if (flag5)
						{
							CImage mouseInstance = Object.Instantiate<CImage>(this._mouseTemplate, this._layout);
							mouseInstance.gameObject.SetActive(true);
							mouseInstance.SetSprite(mouseIcon, false, null);
						}
						else
						{
							Refers hotkeyInstance = Object.Instantiate<Refers>(this._hotkeyTemplate, this._layout);
							this.RefreshNormalCommand(hotkeyInstance, commandStringList[j]);
							hotkeyInstance.gameObject.SetActive(true);
						}
					}
					commandIndex++;
				}
			}
		}
	}

	// Token: 0x06002239 RID: 8761 RVA: 0x000FDEA8 File Offset: 0x000FC0A8
	public static bool TryGetMouseCommandIcon(KeyCode keyCode, out string icon)
	{
		bool result;
		switch (keyCode)
		{
		case KeyCode.Mouse0:
			icon = "sp_mousekey_0";
			result = true;
			break;
		case KeyCode.Mouse1:
			icon = "sp_mousekey_1";
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

	// Token: 0x0600223A RID: 8762 RVA: 0x000FDF1C File Offset: 0x000FC11C
	private void RefreshNormalCommand(Refers hotkeyItem, string commandKey)
	{
		TextMeshProUGUI hotkeyLabel = hotkeyItem.CGet<TextMeshProUGUI>("HotkeyLabel");
		hotkeyLabel.text = commandKey;
	}

	// Token: 0x0600223B RID: 8763 RVA: 0x000FDF40 File Offset: 0x000FC140
	private ValueTuple<List<string>, KeyCode[]> GetCommandStringAndKeyCodeList(HotkeyIndex command)
	{
		CommandKitBase commandKit = CommandKitBase.CommandKitArray.Find((CommandKitBase g) => g.Id == command.GroupId);
		HotKeyCommand commandItem = commandKit.TipDisplayCommandArray.Find((HotKeyCommand c) => c.Id == command.CommandId);
		KeyCode[] keyCodes = commandItem.GetKeyCode(false);
		List<string> result = (from k in keyCodes
		select commandItem.GetKeyCodeString(k)).ToList<string>();
		return new ValueTuple<List<string>, KeyCode[]>(result, keyCodes);
	}

	// Token: 0x04001A55 RID: 6741
	private short _templateId;

	// Token: 0x04001A56 RID: 6742
	[SerializeField]
	private EHotKeyDisplayType type = EHotKeyDisplayType.Count;

	// Token: 0x04001A57 RID: 6743
	[SerializeField]
	private bool newStyle = true;

	// Token: 0x04001A58 RID: 6744
	private RectTransform _layout;

	// Token: 0x04001A59 RID: 6745
	private TextMeshProUGUI _textTemplate;

	// Token: 0x04001A5A RID: 6746
	private Refers _hotkeyTemplate;

	// Token: 0x04001A5B RID: 6747
	private CImage _mouseTemplate;

	// Token: 0x04001A5C RID: 6748
	private RectTransform _content;

	// Token: 0x04001A5D RID: 6749
	private TemplatedContainerAssembly _shortcutsHolder;

	// Token: 0x04001A5E RID: 6750
	private TextMeshProUGUI _desc;
}
