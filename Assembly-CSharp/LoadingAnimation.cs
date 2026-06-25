using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x0200034A RID: 842
public class LoadingAnimation : MonoBehaviour
{
	// Token: 0x0600314C RID: 12620 RVA: 0x001846F1 File Offset: 0x001828F1
	private void OnEnable()
	{
		base.StartCoroutine(this.Process());
	}

	// Token: 0x0600314D RID: 12621 RVA: 0x00184701 File Offset: 0x00182901
	private IEnumerator Process()
	{
		string text = LocalStringManager.Get(LanguageKey.LK_Loading);
		WaitForSeconds wait = new WaitForSeconds(0.33f);
		int count = 3;
		for (;;)
		{
			int num;
			for (int i = 0; i < count; i = num + 1)
			{
				int dot = i + 1;
				this.Title.text = text + '.'.Repeat(dot) + ' '.Repeat(count - dot);
				yield return wait;
				num = i;
			}
		}
		yield break;
	}

	// Token: 0x04002414 RID: 9236
	public CImage TitleBackground;

	// Token: 0x04002415 RID: 9237
	public TextMeshProUGUI Title;
}
