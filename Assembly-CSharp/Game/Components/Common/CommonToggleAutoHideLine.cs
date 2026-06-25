using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.Common
{
	// Token: 0x02000FA1 RID: 4001
	public class CommonToggleAutoHideLine : MonoBehaviour
	{
		// Token: 0x0600B7C3 RID: 47043 RVA: 0x0053BE94 File Offset: 0x0053A094
		private void Start()
		{
			CToggle toggle = base.GetComponent<CToggle>();
			toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
			this.line.SetActive(!toggle.isOn);
		}

		// Token: 0x0600B7C4 RID: 47044 RVA: 0x0053BED6 File Offset: 0x0053A0D6
		private void OnValueChanged(bool value)
		{
			this.line.SetActive(!value);
		}

		// Token: 0x04008ECC RID: 36556
		[SerializeField]
		private GameObject line;
	}
}
