using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000019 RID: 25
public class HotKeyCommand
{
	// Token: 0x17000029 RID: 41
	// (get) Token: 0x060000B0 RID: 176 RVA: 0x00004BD3 File Offset: 0x00002DD3
	public byte Id { get; }

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x060000B1 RID: 177 RVA: 0x00004BDB File Offset: 0x00002DDB
	public bool CanSet { get; }

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x060000B2 RID: 178 RVA: 0x00004BE3 File Offset: 0x00002DE3
	public bool MouseKeyCanSet { get; }

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x060000B3 RID: 179 RVA: 0x00004BEB File Offset: 0x00002DEB
	public HotKeyGroup KeyGroup
	{
		get
		{
			return this._customKeyGroup;
		}
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x00004BF4 File Offset: 0x00002DF4
	public HotKeyCommand(byte id, LanguageKey descId, KeyCode key, KeyCode fnKey = KeyCode.None, bool canSet = true, bool mouseKeyCanSet = true)
	{
		this.Id = id;
		this.DescLanguageId = descId;
		this._defaultKeyGroup.Key = key;
		this._defaultKeyGroup.FunctionKey = fnKey;
		this._defaultKeyGroup.MouseKey = KeyCode.None;
		this._defaultKeyGroup.FunctionMouseKey = KeyCode.None;
		this._customKeyGroup = this._defaultKeyGroup;
		this.CanSet = canSet;
		this.MouseKeyCanSet = mouseKeyCanSet;
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x00004C64 File Offset: 0x00002E64
	public void SetCustomKey(KeyCode key, KeyCode fnKey = KeyCode.None, bool isHandlingMouseKey = false)
	{
		bool flag = !isHandlingMouseKey;
		if (flag)
		{
			this._customKeyGroup.Key = key;
			switch (fnKey)
			{
			case KeyCode.RightShift:
				fnKey = KeyCode.LeftShift;
				break;
			case KeyCode.RightControl:
				fnKey = KeyCode.LeftControl;
				break;
			case KeyCode.RightAlt:
				fnKey = KeyCode.LeftAlt;
				break;
			case KeyCode.RightMeta:
				fnKey = KeyCode.LeftMeta;
				break;
			}
			this._customKeyGroup.FunctionKey = fnKey;
		}
		else
		{
			this._customKeyGroup.MouseKey = key;
			switch (fnKey)
			{
			case KeyCode.RightShift:
				fnKey = KeyCode.LeftShift;
				break;
			case KeyCode.RightControl:
				fnKey = KeyCode.LeftControl;
				break;
			case KeyCode.RightAlt:
				fnKey = KeyCode.LeftAlt;
				break;
			case KeyCode.RightMeta:
				fnKey = KeyCode.LeftMeta;
				break;
			}
			this._customKeyGroup.FunctionMouseKey = fnKey;
		}
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00004D56 File Offset: 0x00002F56
	public void Reset()
	{
		this._customKeyGroup = this._defaultKeyGroup;
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x00004D68 File Offset: 0x00002F68
	public ValueTuple<bool, List<KeyCode>> GetSaveInfo()
	{
		bool flag = this._customKeyGroup.Key == this._defaultKeyGroup.Key && this._customKeyGroup.FunctionKey == this._defaultKeyGroup.FunctionKey && this._customKeyGroup.MouseKey == this._defaultKeyGroup.MouseKey;
		ValueTuple<bool, List<KeyCode>> result;
		if (flag)
		{
			result = new ValueTuple<bool, List<KeyCode>>(false, null);
		}
		else
		{
			List<KeyCode> list = new List<KeyCode>
			{
				this._customKeyGroup.Key,
				this._customKeyGroup.FunctionKey,
				this._customKeyGroup.MouseKey,
				this._customKeyGroup.FunctionMouseKey
			};
			result = new ValueTuple<bool, List<KeyCode>>(true, list);
		}
		return result;
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x00004E28 File Offset: 0x00003028
	public bool Check(UIElement element, bool holdCheck = false, bool downCheck = false, bool isIgnoreBlockHotKey = false, bool fnKeyCheckNone = true, bool isIgnoreElement = false)
	{
		HotKeyCommand.<>c__DisplayClass19_0 CS$<>8__locals1;
		CS$<>8__locals1.holdCheck = holdCheck;
		CS$<>8__locals1.downCheck = downCheck;
		CS$<>8__locals1.fnKeyCheckNone = fnKeyCheckNone;
		bool flag = !isIgnoreElement && element == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = !isIgnoreBlockHotKey && CommandKitBase.GetDisable();
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = !isIgnoreBlockHotKey && UIManager.Instance.BlockHotKey;
				if (flag3)
				{
					result = false;
				}
				else
				{
					bool flag4 = !isIgnoreElement && !element.ForceListenCommand && !UIManager.Instance.IsFocusElement(element);
					if (flag4)
					{
						result = false;
					}
					else
					{
						bool flag5 = EventSystem.current.currentSelectedGameObject;
						result = (!flag5 && (HotKeyCommand.<Check>g__CheckKey|19_0(this._customKeyGroup.Key, this._customKeyGroup.FunctionKey, true, ref CS$<>8__locals1) || HotKeyCommand.<Check>g__CheckKey|19_0(this._customKeyGroup.MouseKey, this._customKeyGroup.FunctionMouseKey, false, ref CS$<>8__locals1)));
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00004F20 File Offset: 0x00003120
	[return: TupleElementNames(new string[]
	{
		"keyConflict",
		"keyCannotConfirm",
		"mouseKeyConflict"
	})]
	public ValueTuple<bool, bool, bool> IsConflictDetail(KeyCode key, KeyCode fnKey)
	{
		bool mouseKeyConflict = this._customKeyGroup.MouseKey == key && this._customKeyGroup.FunctionMouseKey == fnKey;
		bool keysMatch = this._customKeyGroup.Key == key && this._customKeyGroup.FunctionKey == fnKey;
		bool keyConflict = this.CanSet && keysMatch;
		bool keyCannotConfirm = !this.CanSet && keysMatch;
		return new ValueTuple<bool, bool, bool>(keyConflict, keyCannotConfirm, mouseKeyConflict);
	}

	// Token: 0x060000BA RID: 186 RVA: 0x00004F91 File Offset: 0x00003191
	public void ClearKeyBinding()
	{
		this._customKeyGroup.Key = KeyCode.None;
		this._customKeyGroup.FunctionKey = KeyCode.None;
	}

	// Token: 0x060000BB RID: 187 RVA: 0x00004FAC File Offset: 0x000031AC
	public void ClearMouseKeyBinding()
	{
		this._customKeyGroup.MouseKey = KeyCode.None;
		this._customKeyGroup.FunctionMouseKey = KeyCode.None;
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00004FC8 File Offset: 0x000031C8
	public override string ToString()
	{
		bool flag = this._customKeyGroup.FunctionKey == KeyCode.None;
		string result;
		if (flag)
		{
			result = this.GetKeyCodeString(this._customKeyGroup.Key);
		}
		else
		{
			result = this.GetKeyCodeString(this._customKeyGroup.FunctionKey) + " + " + this.GetKeyCodeString(this._customKeyGroup.Key);
		}
		return result;
	}

	// Token: 0x060000BD RID: 189 RVA: 0x0000502C File Offset: 0x0000322C
	public string GetKeyCodeString(KeyCode code)
	{
		bool flag = code == KeyCode.None;
		string result;
		if (flag)
		{
			result = string.Empty;
		}
		else
		{
			string codeString;
			bool flag2 = HotKeyCommand.KeyCodeStringMap.TryGetValue(code, out codeString);
			if (flag2)
			{
				result = codeString;
			}
			else
			{
				result = code.ToString();
			}
		}
		return result;
	}

	// Token: 0x060000BE RID: 190 RVA: 0x00005070 File Offset: 0x00003270
	public KeyCode[] GetKeyCode(bool isHandlingMouseKey = false)
	{
		bool flag = !isHandlingMouseKey;
		KeyCode[] result;
		if (flag)
		{
			bool flag2 = this._customKeyGroup.FunctionKey == KeyCode.None;
			if (flag2)
			{
				result = new KeyCode[]
				{
					this._customKeyGroup.Key
				};
			}
			else
			{
				result = new KeyCode[]
				{
					this._customKeyGroup.FunctionKey,
					this._customKeyGroup.Key
				};
			}
		}
		else
		{
			bool flag3 = this._customKeyGroup.FunctionMouseKey == KeyCode.None;
			if (flag3)
			{
				result = new KeyCode[]
				{
					this._customKeyGroup.MouseKey
				};
			}
			else
			{
				result = new KeyCode[]
				{
					this._customKeyGroup.FunctionMouseKey,
					this._customKeyGroup.MouseKey
				};
			}
		}
		return result;
	}

	// Token: 0x060000BF RID: 191 RVA: 0x00005128 File Offset: 0x00003328
	public static bool CheckAnyKeyDown()
	{
		bool flag = SingletonObject.getInstance<TooltipManager>().IsShowingUnfixedTip();
		return !flag && Input.anyKeyDown;
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x000053F0 File Offset: 0x000035F0
	[CompilerGenerated]
	internal static bool <Check>g__CheckKey|19_0(KeyCode key, KeyCode fnKey, bool checkIsFnKey, ref HotKeyCommand.<>c__DisplayClass19_0 A_3)
	{
		bool flag = key == KeyCode.None;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool baseKeyOk = false;
			Vector2 scrollDelta = Input.mouseScrollDelta;
			bool flag2 = key == (KeyCode)1000 || key == (KeyCode)1001;
			if (flag2)
			{
				bool isPointerOverAnyCScrollRect = CScrollRect.IsPointerOverAnyCScrollRect;
				if (isPointerOverAnyCScrollRect)
				{
					return false;
				}
				bool flag3 = scrollDelta.y > 0.5f && key == (KeyCode)1000;
				if (flag3)
				{
					baseKeyOk = true;
				}
				bool flag4 = scrollDelta.y < -0.5f && key == (KeyCode)1001;
				if (flag4)
				{
					baseKeyOk = true;
				}
			}
			else
			{
				baseKeyOk = (A_3.holdCheck ? Input.GetKey(key) : (A_3.downCheck ? Input.GetKeyDown(key) : Input.GetKeyUp(key)));
			}
			if (checkIsFnKey)
			{
				bool isFnKey = key == KeyCode.LeftControl || key == KeyCode.RightControl || key == KeyCode.LeftShift || key == KeyCode.RightShift || key == KeyCode.LeftAlt || key == KeyCode.RightAlt;
				bool flag5 = isFnKey;
				if (flag5)
				{
					return baseKeyOk;
				}
			}
			bool controlDowning = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
			bool shiftDowning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			bool altDowning = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			bool flag6 = fnKey == KeyCode.None;
			if (flag6)
			{
				bool flag7 = A_3.fnKeyCheckNone && (controlDowning || shiftDowning || altDowning);
				result = (!flag7 && baseKeyOk);
			}
			else
			{
				bool fnKeyOk = false;
				switch (fnKey)
				{
				case KeyCode.RightShift:
				case KeyCode.LeftShift:
					fnKeyOk = shiftDowning;
					break;
				case KeyCode.RightControl:
				case KeyCode.LeftControl:
					fnKeyOk = controlDowning;
					break;
				case KeyCode.RightAlt:
				case KeyCode.LeftAlt:
					fnKeyOk = altDowning;
					break;
				}
				result = (fnKeyOk && baseKeyOk);
			}
		}
		return result;
	}

	// Token: 0x0400006D RID: 109
	public LanguageKey DescLanguageId;

	// Token: 0x0400006E RID: 110
	private HotKeyGroup _customKeyGroup;

	// Token: 0x0400006F RID: 111
	private readonly HotKeyGroup _defaultKeyGroup;

	// Token: 0x04000070 RID: 112
	private static readonly Dictionary<KeyCode, string> KeyCodeStringMap = new Dictionary<KeyCode, string>
	{
		{
			KeyCode.LeftAlt,
			"Alt"
		},
		{
			KeyCode.RightAlt,
			"Alt"
		},
		{
			KeyCode.LeftShift,
			"Shift"
		},
		{
			KeyCode.RightShift,
			"Shift"
		},
		{
			KeyCode.LeftControl,
			"Ctrl"
		},
		{
			KeyCode.RightControl,
			"Ctrl"
		},
		{
			KeyCode.LeftMeta,
			"Command"
		},
		{
			KeyCode.RightMeta,
			"Command"
		},
		{
			KeyCode.Escape,
			"Esc"
		},
		{
			KeyCode.Backslash,
			"\\"
		},
		{
			KeyCode.Comma,
			"<voffset=5px>,</voffset>"
		},
		{
			KeyCode.Period,
			"<voffset=5px>.</voffset>"
		},
		{
			KeyCode.Quote,
			"<voffset=-5px>'</voffset>"
		},
		{
			KeyCode.DoubleQuote,
			"<voffset=-5px>\"</voffset>"
		},
		{
			KeyCode.Semicolon,
			";"
		},
		{
			KeyCode.Slash,
			"/"
		},
		{
			KeyCode.Return,
			"Enter"
		},
		{
			KeyCode.Alpha0,
			"0"
		},
		{
			KeyCode.Alpha1,
			"1"
		},
		{
			KeyCode.Alpha2,
			"2"
		},
		{
			KeyCode.Alpha3,
			"3"
		},
		{
			KeyCode.Alpha4,
			"4"
		},
		{
			KeyCode.Alpha5,
			"5"
		},
		{
			KeyCode.Alpha6,
			"6"
		},
		{
			KeyCode.Alpha7,
			"7"
		},
		{
			KeyCode.Alpha8,
			"8"
		},
		{
			KeyCode.Alpha9,
			"9"
		},
		{
			KeyCode.LeftArrow,
			"←"
		},
		{
			KeyCode.RightArrow,
			"→"
		},
		{
			KeyCode.UpArrow,
			"↑"
		},
		{
			KeyCode.DownArrow,
			"↓"
		},
		{
			KeyCode.KeypadDivide,
			"KeyPad /"
		},
		{
			KeyCode.KeypadMinus,
			"KeyPad -"
		},
		{
			KeyCode.KeypadPlus,
			"KeyPad +"
		},
		{
			KeyCode.KeypadMultiply,
			"KeyPad *"
		},
		{
			KeyCode.KeypadPeriod,
			"KeyPad ."
		},
		{
			KeyCode.Minus,
			"-"
		},
		{
			KeyCode.Equals,
			"="
		},
		{
			KeyCode.LeftBracket,
			"["
		},
		{
			KeyCode.RightBracket,
			"]"
		},
		{
			(KeyCode)1000,
			"ScrollUp"
		},
		{
			(KeyCode)1001,
			"ScrollDown"
		}
	};
}
