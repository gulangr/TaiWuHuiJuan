using System;
using TMPro;
using UnityEngine;

namespace Game.Views.TaskPanel
{
	// Token: 0x02000756 RID: 1878
	public class TaskGroup_TaskItem : MonoBehaviour
	{
		// Token: 0x06005AF3 RID: 23283 RVA: 0x002A33C4 File Offset: 0x002A15C4
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

		// Token: 0x06005AF4 RID: 23284 RVA: 0x002A3432 File Offset: 0x002A1632
		public void BlockedState()
		{
			this._titleText.GetComponent<TextStyle>().enabled = false;
			this._contentsText.GetComponent<TextStyle>().enabled = false;
		}

		// Token: 0x06005AF5 RID: 23285 RVA: 0x002A3459 File Offset: 0x002A1659
		public void DefaultState()
		{
			this._titleText.GetComponent<TextStyle>().enabled = true;
			this._contentsText.GetComponent<TextStyle>().enabled = true;
		}

		// Token: 0x04003EB5 RID: 16053
		[SerializeField]
		private TextMeshProUGUI _titleText;

		// Token: 0x04003EB6 RID: 16054
		[SerializeField]
		private GameObject _task_Title;

		// Token: 0x04003EB7 RID: 16055
		[SerializeField]
		private TextMeshProUGUI _contentsText;

		// Token: 0x04003EB8 RID: 16056
		[SerializeField]
		private RectTransform _thisRectTransform;
	}
}
