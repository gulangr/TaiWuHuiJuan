using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F8F RID: 3983
	public class HotkeyDisplay : MonoBehaviour
	{
		// Token: 0x0600B72C RID: 46892 RVA: 0x00537AE8 File Offset: 0x00535CE8
		private void OnEnable()
		{
			bool flag = this.type != EHotKeyDisplayType.Count;
			if (flag)
			{
				this.Refresh(this.type);
			}
		}

		// Token: 0x0600B72D RID: 46893 RVA: 0x00537B14 File Offset: 0x00535D14
		public void Refresh(EHotKeyDisplayType displayType)
		{
			this.Refresh((short)displayType);
		}

		// Token: 0x0600B72E RID: 46894 RVA: 0x00537B20 File Offset: 0x00535D20
		public void Refresh(short templateId)
		{
			this.type = (EHotKeyDisplayType)templateId;
			HotKeyDisplayItem config = HotKeyDisplay.Instance[templateId];
			this.RefreshInner(config.DisplayText, config.Params);
		}

		// Token: 0x0600B72F RID: 46895 RVA: 0x00537B54 File Offset: 0x00535D54
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
						TMPTextSpriteHelper helper;
						bool flag3 = textInstance.TryGetComponent<TMPTextSpriteHelper>(out helper);
						if (flag3)
						{
							helper.Parse();
						}
					}
				}
				else
				{
					bool flag4 = commandIndex < commands.Count;
					if (flag4)
					{
						ValueTuple<List<string>, KeyCode[]> commandStringAndKeyCodeList = this.GetCommandStringAndKeyCodeList(commands[commandIndex]);
						List<string> commandStringList = commandStringAndKeyCodeList.Item1;
						KeyCode[] keyCodeArray = commandStringAndKeyCodeList.Item2;
						for (int j = 0; j < commandStringList.Count; j++)
						{
							bool flag5 = j > 0;
							if (flag5)
							{
								TextMeshProUGUI textInstance2 = Object.Instantiate<TextMeshProUGUI>(this.textTemplate, this.layout);
								textInstance2.text = "+";
								textInstance2.gameObject.SetActive(true);
								TMPTextSpriteHelper helper2;
								bool flag6 = textInstance2.TryGetComponent<TMPTextSpriteHelper>(out helper2);
								if (flag6)
								{
									helper2.Parse();
								}
							}
							string mouseIcon;
							bool flag7 = HotkeyDisplay.TryGetMouseCommandIcon(keyCodeArray[j], out mouseIcon);
							if (flag7)
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

		// Token: 0x0600B730 RID: 46896 RVA: 0x00537D88 File Offset: 0x00535F88
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

		// Token: 0x0600B731 RID: 46897 RVA: 0x00537DFC File Offset: 0x00535FFC
		private void RefreshNormalCommand(Refers hotkeyItem, string commandKey)
		{
			TextMeshProUGUI hotkeyLabel = hotkeyItem.CGet<TextMeshProUGUI>("HotkeyLabel");
			hotkeyLabel.text = commandKey;
		}

		// Token: 0x0600B732 RID: 46898 RVA: 0x00537E20 File Offset: 0x00536020
		private ValueTuple<List<string>, KeyCode[]> GetCommandStringAndKeyCodeList(HotkeyIndex command)
		{
			CommandKitBase commandKit = CommandKitBase.CommandKitArray.Find((CommandKitBase g) => g.Id == command.GroupId);
			HotKeyCommand commandItem = commandKit.TipDisplayCommandArray.Find((HotKeyCommand c) => c.Id == command.CommandId);
			KeyCode[] keyCodes = commandItem.GetKeyCode(false);
			List<string> result = (from k in keyCodes
			select commandItem.GetKeyCodeString(k)).ToList<string>();
			return new ValueTuple<List<string>, KeyCode[]>(result, keyCodes);
		}

		// Token: 0x04008E48 RID: 36424
		[SerializeField]
		private EHotKeyDisplayType type = EHotKeyDisplayType.Count;

		// Token: 0x04008E49 RID: 36425
		[SerializeField]
		private RectTransform layout;

		// Token: 0x04008E4A RID: 36426
		[SerializeField]
		private TextMeshProUGUI textTemplate;

		// Token: 0x04008E4B RID: 36427
		[SerializeField]
		private Refers hotkeyTemplate;

		// Token: 0x04008E4C RID: 36428
		[SerializeField]
		private CImage mouseTemplate;
	}
}
