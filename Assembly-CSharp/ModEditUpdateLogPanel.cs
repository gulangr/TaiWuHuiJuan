using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using TMPro;
using UnityEngine;

// Token: 0x0200025D RID: 605
public class ModEditUpdateLogPanel : MonoBehaviour
{
	// Token: 0x060027CB RID: 10187 RVA: 0x0012552C File Offset: 0x0012372C
	public void Init()
	{
		this.btnYes.ClearAndAddListener(new Action(this.OnClickConfirm));
		this.btnNo.ClearAndAddListener(new Action(this.OnClickCancel));
		this.buttonMore.ClearAndAddListener(delegate
		{
			this._logList.Add(string.Empty);
			this.Refresh();
		});
	}

	// Token: 0x060027CC RID: 10188 RVA: 0x00125582 File Offset: 0x00123782
	private void OnClickConfirm()
	{
		this.Hide();
		Action onConfirm = this._onConfirm;
		if (onConfirm != null)
		{
			onConfirm();
		}
	}

	// Token: 0x060027CD RID: 10189 RVA: 0x0012559E File Offset: 0x0012379E
	private void OnClickCancel()
	{
		this.Hide();
	}

	// Token: 0x060027CE RID: 10190 RVA: 0x001255A8 File Offset: 0x001237A8
	public void Show(List<string> logList, Action onConfirm)
	{
		this._logList = logList;
		bool flag = this._logList.Count == 0;
		if (flag)
		{
			this._logList.Add(string.Empty);
		}
		this._onConfirm = onConfirm;
		this.Refresh();
		base.gameObject.SetActive(true);
	}

	// Token: 0x060027CF RID: 10191 RVA: 0x001255FC File Offset: 0x001237FC
	private void Refresh()
	{
		for (int i = 0; i < this._logList.Count; i++)
		{
			Transform obj = (i < this.layout.childCount) ? this.layout.GetChild(i) : Object.Instantiate<Transform>(this.layout.GetChild(0), this.layout);
			obj.gameObject.SetActive(true);
			ModUpdateLogItem refer = obj.GetComponent<ModUpdateLogItem>();
			int index = i;
			TMP_InputField inputField = refer.descriptionInputField;
			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(delegate(string value)
			{
				this._logList[index] = value;
			});
			inputField.SetTextWithoutNotify(this._logList[index]);
			CButton buttonLess = refer.buttonLess;
			buttonLess.ClearAndAddListener(delegate
			{
				this._logList.RemoveAt(index);
				this.Refresh();
			});
			buttonLess.interactable = (index != 0);
			refer.text.text = string.Format("{0}.", index + 1);
		}
		for (int j = this._logList.Count; j < this.layout.childCount; j++)
		{
			this.layout.GetChild(j).gameObject.SetActive(false);
		}
	}

	// Token: 0x060027D0 RID: 10192 RVA: 0x00125764 File Offset: 0x00123964
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001D0F RID: 7439
	[SerializeField]
	private RectTransform layout;

	// Token: 0x04001D10 RID: 7440
	[SerializeField]
	private CButton buttonMore;

	// Token: 0x04001D11 RID: 7441
	[SerializeField]
	private CButton btnYes;

	// Token: 0x04001D12 RID: 7442
	[SerializeField]
	private CButton btnNo;

	// Token: 0x04001D13 RID: 7443
	private List<string> _logList;

	// Token: 0x04001D14 RID: 7444
	private Action _onConfirm;
}
