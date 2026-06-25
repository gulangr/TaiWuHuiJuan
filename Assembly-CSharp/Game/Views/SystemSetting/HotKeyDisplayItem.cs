using System;
using TMPro;
using UnityEngine;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000760 RID: 1888
	public class HotKeyDisplayItem : MonoBehaviour
	{
		// Token: 0x06005B61 RID: 23393 RVA: 0x002A6ED8 File Offset: 0x002A50D8
		public void Set(HotKeyCommand command, bool isMouseKey, bool isInEdit = false)
		{
			KeyCode[] keyCodes = command.GetKeyCode(isMouseKey);
			bool hasValidKey = keyCodes != null && keyCodes.Length >= 1 && keyCodes[0] > KeyCode.None;
			bool flag = !hasValidKey;
			if (flag)
			{
				this.key.transform.parent.gameObject.SetActive(isInEdit);
				this.key.text = LanguageKey.LK_SystemSetting_HotKey_Edit_UnboundText.Tr();
				this.add.gameObject.SetActive(false);
				this.functionKey.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				bool hasFunctionKey = keyCodes.Length >= 2;
				bool flag2 = hasFunctionKey;
				if (flag2)
				{
					this.key.transform.parent.gameObject.SetActive(true);
					this.key.text = command.GetKeyCodeString(keyCodes[1]);
					this.add.gameObject.SetActive(true);
					this.functionKey.transform.parent.gameObject.SetActive(true);
					this.functionKey.text = command.GetKeyCodeString(keyCodes[0]);
				}
				else
				{
					this.key.transform.parent.gameObject.SetActive(true);
					this.key.text = command.GetKeyCodeString(keyCodes[0]);
					this.add.gameObject.SetActive(false);
					this.functionKey.transform.parent.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06005B62 RID: 23394 RVA: 0x002A705C File Offset: 0x002A525C
		public void SetWithTransition(HotKeyCommand command, KeyCode newKey, KeyCode newFnKey, bool isMouseKey)
		{
			this.key.transform.parent.gameObject.SetActive(true);
			bool flag = newKey > KeyCode.None;
			if (flag)
			{
				bool flag2 = newFnKey > KeyCode.None;
				if (flag2)
				{
					this.key.text = command.GetKeyCodeString(newKey);
					this.add.gameObject.SetActive(true);
					this.functionKey.transform.parent.gameObject.SetActive(true);
					this.functionKey.text = command.GetKeyCodeString(newFnKey);
				}
				else
				{
					this.key.text = command.GetKeyCodeString(newKey);
					this.add.gameObject.SetActive(false);
					this.functionKey.transform.parent.gameObject.SetActive(false);
				}
			}
			else
			{
				this.key.text = LanguageKey.LK_SystemSetting_HotKey_Edit_UnboundText.Tr();
				this.add.gameObject.SetActive(false);
				this.functionKey.transform.parent.gameObject.SetActive(false);
			}
		}

		// Token: 0x04003F04 RID: 16132
		[SerializeField]
		private TextMeshProUGUI key;

		// Token: 0x04003F05 RID: 16133
		[SerializeField]
		private TextMeshProUGUI add;

		// Token: 0x04003F06 RID: 16134
		[SerializeField]
		private TextMeshProUGUI functionKey;
	}
}
