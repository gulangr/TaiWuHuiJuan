using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F9C RID: 3996
	public class CommonHotkeyDisplayNew : MonoBehaviour
	{
		// Token: 0x0600B7A3 RID: 47011 RVA: 0x0053AC58 File Offset: 0x00538E58
		private void Awake()
		{
			this.layout.gameObject.SetActive(true);
			bool flag = this.type != EHotKeyDisplayType.Count;
			if (flag)
			{
				this.Refresh(this.type);
			}
		}

		// Token: 0x0600B7A4 RID: 47012 RVA: 0x0053AC98 File Offset: 0x00538E98
		private void OnEnable()
		{
			bool flag = this.type != EHotKeyDisplayType.Count;
			if (flag)
			{
				this.Refresh(this.type);
			}
		}

		// Token: 0x0600B7A5 RID: 47013 RVA: 0x0053ACC4 File Offset: 0x00538EC4
		public void Refresh(EHotKeyDisplayType displayType)
		{
			this.type = displayType;
			this.Refresh((short)displayType);
		}

		// Token: 0x0600B7A6 RID: 47014 RVA: 0x0053ACD8 File Offset: 0x00538ED8
		private void Refresh(short templateId)
		{
			HotKeyDisplayItem config = HotKeyDisplay.Instance[templateId];
			this.RefreshDisplay(config.DisplayText, config.Params);
		}

		// Token: 0x0600B7A7 RID: 47015 RVA: 0x0053AD08 File Offset: 0x00538F08
		private void RefreshDisplay(string displayText, List<HotkeyIndex> commands)
		{
			foreach (object obj in this.layout)
			{
				Transform child = (Transform)obj;
				Object.Destroy(child.gameObject);
			}
			this.desc.text = displayText;
			int totalIndex = 0;
			for (int i = 0; i < commands.Count; i++)
			{
				KeyCode[] keyCodeArray = this.GetCommandStringAndKeyCodeList(commands[i]).Item2;
				foreach (KeyCode item in keyCodeArray)
				{
					bool flag = totalIndex > 0;
					if (flag)
					{
						this.AddToLayout();
					}
					this.AddToLayout(item);
					totalIndex++;
				}
			}
		}

		// Token: 0x0600B7A8 RID: 47016 RVA: 0x0053ADF0 File Offset: 0x00538FF0
		private void AddToLayout(KeyCode code)
		{
			string icon;
			bool flag = CommonHotkeyDisplayNew.TryGetMouseCommandIcon(code, out icon);
			if (flag)
			{
				CImage mouseInstance = Object.Instantiate<CImage>(this.mouseTemplate, this.layout);
				mouseInstance.gameObject.SetActive(true);
				mouseInstance.SetSprite(icon, false, null);
			}
			else
			{
				string txt = this.GetCommandText(code);
				Refers textInstance = Object.Instantiate<Refers>(this.textTemplate, this.layout);
				textInstance.CGet<TextMeshProUGUI>("Text").text = txt;
				textInstance.gameObject.SetActive(true);
			}
		}

		// Token: 0x0600B7A9 RID: 47017 RVA: 0x0053AE74 File Offset: 0x00539074
		private void AddToLayout()
		{
			GameObject temp = Object.Instantiate<GameObject>(this.spliterTemplate, this.layout);
			temp.gameObject.SetActive(true);
		}

		// Token: 0x0600B7AA RID: 47018 RVA: 0x0053AEA4 File Offset: 0x005390A4
		private string GetCommandText(KeyCode code)
		{
			if (!true)
			{
			}
			string result;
			if (code <= KeyCode.Space)
			{
				if (code == KeyCode.Escape)
				{
					result = LocalStringManager.Get(LanguageKey.LK_ShortCuts_ESC);
					goto IL_70;
				}
				if (code == KeyCode.Space)
				{
					result = LocalStringManager.Get(LanguageKey.LK_ShortCuts_Space);
					goto IL_70;
				}
			}
			else
			{
				if (code == KeyCode.Mouse0)
				{
					result = LocalStringManager.Get(LanguageKey.LK_ShortCuts_LeftMouse);
					goto IL_70;
				}
				if (code == KeyCode.Mouse1)
				{
					result = LocalStringManager.Get(LanguageKey.LK_ShortCuts_RightMouse);
					goto IL_70;
				}
			}
			result = code.ToString();
			IL_70:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600B7AB RID: 47019 RVA: 0x0053AF2C File Offset: 0x0053912C
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

		// Token: 0x0600B7AC RID: 47020 RVA: 0x0053AFA0 File Offset: 0x005391A0
		private void RefreshNormalCommand(Refers hotkeyItem, string commandKey)
		{
			TextMeshProUGUI hotkeyLabel = hotkeyItem.CGet<TextMeshProUGUI>("HotkeyLabel");
			hotkeyLabel.text = commandKey;
		}

		// Token: 0x0600B7AD RID: 47021 RVA: 0x0053AFC4 File Offset: 0x005391C4
		private ValueTuple<List<string>, KeyCode[]> GetCommandStringAndKeyCodeList(HotkeyIndex command)
		{
			CommonHotkeyDisplayNew.<>c__DisplayClass18_0 CS$<>8__locals1 = new CommonHotkeyDisplayNew.<>c__DisplayClass18_0();
			CS$<>8__locals1.command = command;
			CommandKitBase commandKit = CommandKitBase.CommandKitArray.Find((CommandKitBase g) => g.Id == CS$<>8__locals1.command.GroupId);
			CommonHotkeyDisplayNew.<>c__DisplayClass18_0 CS$<>8__locals2 = CS$<>8__locals1;
			HotKeyCommand[] tipDisplayCommandArray = commandKit.TipDisplayCommandArray;
			HotKeyCommand commandItem;
			if ((commandItem = ((tipDisplayCommandArray != null) ? tipDisplayCommandArray.Find((HotKeyCommand c) => c.Id == CS$<>8__locals1.command.CommandId) : null)) == null)
			{
				HotKeyCommand[] groupCommand = commandKit.GroupCommand;
				commandItem = ((groupCommand != null) ? groupCommand.Find((HotKeyCommand c) => c.Id == CS$<>8__locals1.command.CommandId) : null);
			}
			CS$<>8__locals2.commandItem = commandItem;
			KeyCode[] keyCodes = CS$<>8__locals1.commandItem.GetKeyCode(false);
			List<string> result = (from k in keyCodes
			select CS$<>8__locals1.commandItem.GetKeyCodeString(k)).ToList<string>();
			return new ValueTuple<List<string>, KeyCode[]>(result, keyCodes);
		}

		// Token: 0x04008E9C RID: 36508
		[SerializeField]
		private RectTransform layout;

		// Token: 0x04008E9D RID: 36509
		[SerializeField]
		private Refers textTemplate;

		// Token: 0x04008E9E RID: 36510
		[SerializeField]
		private CImage mouseTemplate;

		// Token: 0x04008E9F RID: 36511
		[SerializeField]
		private GameObject spliterTemplate;

		// Token: 0x04008EA0 RID: 36512
		[SerializeField]
		private bool refreshOnEnable;

		// Token: 0x04008EA1 RID: 36513
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x04008EA2 RID: 36514
		private short _templateId;

		// Token: 0x04008EA3 RID: 36515
		[SerializeField]
		private EHotKeyDisplayType type = EHotKeyDisplayType.Count;
	}
}
