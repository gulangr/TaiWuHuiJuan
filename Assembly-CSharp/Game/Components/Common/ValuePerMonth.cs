using System;
using TMPro;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F9A RID: 3994
	public class ValuePerMonth : MonoBehaviour
	{
		// Token: 0x0600B795 RID: 46997 RVA: 0x0053A645 File Offset: 0x00538845
		public void Set(int value)
		{
			this.textValue.text = string.Format("+{0}/", value);
		}

		// Token: 0x04008E91 RID: 36497
		[SerializeField]
		private TextMeshProUGUI textValue;
	}
}
