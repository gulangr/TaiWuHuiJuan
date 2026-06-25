using System;
using TMPro;
using UnityEngine;

// Token: 0x0200030A RID: 778
public class TaskGroup_TaskItem : MonoBehaviour
{
	// Token: 0x06002DE8 RID: 11752 RVA: 0x0016B8D4 File Offset: 0x00169AD4
	public void SetData(string titleText, string contentsText)
	{
		bool flag = titleText.IsNullOrEmpty();
		if (flag)
		{
			this._task_Title.SetActive(false);
			this._contentsText.text = contentsText.ColorReplace();
		}
		else
		{
			this._task_Title.SetActive(true);
			this._titleText.text = titleText.ColorReplace();
			this._contentsText.text = contentsText.ColorReplace();
		}
	}

	// Token: 0x06002DE9 RID: 11753 RVA: 0x0016B944 File Offset: 0x00169B44
	public void BlockedState()
	{
		this._titleText.GetComponent<TextStyle>().enabled = false;
		this._contentsText.GetComponent<TextStyle>().enabled = false;
		this._titleText.alpha = 0.33333334f;
		this._contentsText.alpha = 0.33333334f;
		this._title_Icon.SetAlpha(0.33333334f);
		this._description_Icon.SetAlpha(0.33333334f);
	}

	// Token: 0x06002DEA RID: 11754 RVA: 0x0016B9BC File Offset: 0x00169BBC
	public void DefaultState()
	{
		this._titleText.GetComponent<TextStyle>().enabled = true;
		this._contentsText.GetComponent<TextStyle>().enabled = true;
		this._title_Icon.SetAlpha(1f);
		this._description_Icon.SetAlpha(1f);
	}

	// Token: 0x04002147 RID: 8519
	[SerializeField]
	private TextMeshProUGUI _titleText;

	// Token: 0x04002148 RID: 8520
	[SerializeField]
	private GameObject _task_Title;

	// Token: 0x04002149 RID: 8521
	[SerializeField]
	private TextMeshProUGUI _contentsText;

	// Token: 0x0400214A RID: 8522
	[SerializeField]
	private RectTransform _thisRectTransform;

	// Token: 0x0400214B RID: 8523
	[SerializeField]
	private CImage _title_Icon;

	// Token: 0x0400214C RID: 8524
	[SerializeField]
	private CImage _description_Icon;
}
