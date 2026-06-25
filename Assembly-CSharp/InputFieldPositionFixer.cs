using System;
using TMPro;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class InputFieldPositionFixer : MonoBehaviour
{
	// Token: 0x06000425 RID: 1061 RVA: 0x00019CFC File Offset: 0x00017EFC
	private void Update()
	{
		bool flag = !this.inputFieldText.text.StartsWith("<u") && this.inputFieldText.text.Length - 1 <= this.inputField.characterLimit;
		if (flag)
		{
			this.inputFieldText.rectTransform.anchoredPosition = this.referenceRect.anchoredPosition;
			Transform caret = this.inputFieldText.transform.parent.Find("Caret");
			bool flag2 = caret != null;
			if (flag2)
			{
				caret.GetComponent<RectTransform>().anchoredPosition = this.referenceRect.anchoredPosition;
			}
		}
	}

	// Token: 0x04000290 RID: 656
	public TMP_InputField inputField;

	// Token: 0x04000291 RID: 657
	public TextMeshProUGUI inputFieldText;

	// Token: 0x04000292 RID: 658
	public RectTransform referenceRect;
}
