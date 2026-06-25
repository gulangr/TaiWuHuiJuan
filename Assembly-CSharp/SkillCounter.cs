using System;
using TMPro;
using UnityEngine;

// Token: 0x020003FD RID: 1021
public class SkillCounter : MonoBehaviour
{
	// Token: 0x06003D1F RID: 15647 RVA: 0x001EC038 File Offset: 0x001EA238
	public void Set(short[] counter)
	{
		for (int i = 0; i < 9; i++)
		{
			int j = (int)(counter.CheckIndex(i) ? counter[i] : 0);
			this.grades[i].sprite = this.gradeSprites[i];
			this.gradeNames[i].text = LocalStringManager.Get(string.Format("LK_Mousetip_Grade_Short_{0}", i)).SetGradeColor(i);
			this.counts[i].text = j.ToString();
		}
	}

	// Token: 0x04002BD9 RID: 11225
	[SerializeField]
	private Sprite[] gradeSprites;

	// Token: 0x04002BDA RID: 11226
	[SerializeField]
	private CImage[] grades;

	// Token: 0x04002BDB RID: 11227
	[SerializeField]
	private TMP_Text[] gradeNames;

	// Token: 0x04002BDC RID: 11228
	[SerializeField]
	private TMP_Text[] counts;
}
