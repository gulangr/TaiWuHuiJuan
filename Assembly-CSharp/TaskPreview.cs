using System;
using TMPro;
using UnityEngine;

// Token: 0x0200030D RID: 781
public class TaskPreview : MonoBehaviour
{
	// Token: 0x06002E15 RID: 11797 RVA: 0x0016CE28 File Offset: 0x0016B028
	public void SetData(string title, string content)
	{
		bool flag = title.IsNullOrEmpty();
		if (flag)
		{
			this._title.SetActive(false);
			this._taskContentText.text = content.ColorReplace();
		}
		else
		{
			this._title.SetActive(true);
			this._titleText.text = title.ColorReplace();
			this._taskContentText.text = content.ColorReplace();
		}
		base.gameObject.SetActive(true);
	}

	// Token: 0x06002E16 RID: 11798 RVA: 0x0016CEA4 File Offset: 0x0016B0A4
	public float GetHeight()
	{
		return base.GetComponent<RectTransform>().rect.height;
	}

	// Token: 0x04002167 RID: 8551
	[SerializeField]
	private TextMeshProUGUI _titleText;

	// Token: 0x04002168 RID: 8552
	[SerializeField]
	private GameObject _title;

	// Token: 0x04002169 RID: 8553
	[SerializeField]
	private TextMeshProUGUI _taskContentText;

	// Token: 0x0400216A RID: 8554
	[SerializeField]
	private GameObject _content;
}
