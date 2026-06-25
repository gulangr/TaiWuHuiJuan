using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000822 RID: 2082
	public abstract class CommonAtomBase : MonoBehaviour
	{
		// Token: 0x06006643 RID: 26179
		public abstract void SetMarginLeft(int marginLeft);

		// Token: 0x06006644 RID: 26180 RVA: 0x002EB49B File Offset: 0x002E969B
		protected static void SetLabelText(TextMeshProUGUI label, string content)
		{
			label.text = (((content != null) ? content.ColorReplace() : null) ?? string.Empty);
			TMPTextSpriteHelper component = label.GetComponent<TMPTextSpriteHelper>();
			if (component != null)
			{
				component.Parse();
			}
		}
	}
}
