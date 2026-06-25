using System;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000361 RID: 865
public class SwitchButton : Refers
{
	// Token: 0x0600323A RID: 12858 RVA: 0x0018C7AC File Offset: 0x0018A9AC
	public void InitSwitchButton(string leftContent, string rightContent, UnityAction<bool> onSwitch)
	{
		this.InitRefers();
		CToggleObsolete toggle = base.GetComponent<CToggleObsolete>();
		toggle.onValueChanged.RemoveAllListeners();
		bool flag = onSwitch != null;
		if (flag)
		{
			toggle.onValueChanged.AddListener(onSwitch);
		}
		bool isOn = toggle.isOn;
		for (int i = 0; i < this._leftArr.Length; i++)
		{
			this._leftArr[i].text = leftContent;
			this._leftArr[i].transform.parent.GetComponent<CImage>().enabled = isOn;
			this._rightArr[i].text = rightContent;
			this._rightArr[i].transform.parent.GetComponent<CImage>().enabled = !isOn;
		}
		this._normalLabelLeft.gameObject.SetActive(isOn);
		this._normalLabelLeftWhite.gameObject.SetActive(!isOn);
		this._normalLabelRight.gameObject.SetActive(!isOn);
		this._normalLabelRightWhite.gameObject.SetActive(isOn);
	}

	// Token: 0x0600323B RID: 12859 RVA: 0x0018C8B8 File Offset: 0x0018AAB8
	public void SetStatus(bool isOn)
	{
		for (int i = 0; i < this._leftArr.Length; i++)
		{
			this._leftArr[i].transform.parent.GetComponent<CImage>().enabled = isOn;
			this._rightArr[i].transform.parent.GetComponent<CImage>().enabled = !isOn;
		}
		this._normalLabelLeft.gameObject.SetActive(isOn);
		this._normalLabelLeftWhite.gameObject.SetActive(!isOn);
		this._normalLabelRight.gameObject.SetActive(!isOn);
		this._normalLabelRightWhite.gameObject.SetActive(isOn);
	}

	// Token: 0x0600323C RID: 12860 RVA: 0x0018C96C File Offset: 0x0018AB6C
	public void SetHovor(bool isHovor)
	{
		bool isOn = base.GetComponent<CToggleObsolete>().isOn;
		this._hoverLabelLeft.transform.parent.gameObject.SetActive(isOn && isHovor);
		this._hoverLabelRight.transform.parent.gameObject.SetActive(!isOn && isHovor);
	}

	// Token: 0x0600323D RID: 12861 RVA: 0x0018C9C5 File Offset: 0x0018ABC5
	private void OnEnable()
	{
		base.StartCoroutine(this.UpdateWidth());
	}

	// Token: 0x0600323E RID: 12862 RVA: 0x0018C9D5 File Offset: 0x0018ABD5
	private IEnumerator UpdateWidth()
	{
		yield return null;
		float maxWidth = Math.Max(this._normalLabelLeft.preferredWidth, this._normalLabelRight.preferredWidth);
		int num;
		for (int i = 0; i < this._leftArr.Length; i = num + 1)
		{
			this._leftArr[i].GetComponent<LayoutElement>().preferredWidth = maxWidth;
			this._rightArr[i].GetComponent<LayoutElement>().preferredWidth = maxWidth;
			num = i;
		}
		yield break;
	}

	// Token: 0x0600323F RID: 12863 RVA: 0x0018C9E4 File Offset: 0x0018ABE4
	private void InitRefers()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._disableLabelLeft = base.CGet<TextMeshProUGUI>("DisableLabelLeft");
			this._disableLabelRight = base.CGet<TextMeshProUGUI>("DisableLabelRight");
			this._hoverLabelLeft = base.CGet<TextMeshProUGUI>("HoverLabelLeft");
			this._hoverLabelRight = base.CGet<TextMeshProUGUI>("HoverLabelRight");
			this._normalLabelLeft = base.CGet<TextMeshProUGUI>("NormalLabelLeft");
			this._normalLabelLeftWhite = base.CGet<TextMeshProUGUI>("NormalLabelLeftWhite");
			this._normalLabelRight = base.CGet<TextMeshProUGUI>("NormalLabelRight");
			this._normalLabelRightWhite = base.CGet<TextMeshProUGUI>("NormalLabelRightWhite");
			this._leftArr[0] = this._normalLabelLeft;
			this._leftArr[1] = this._normalLabelLeftWhite;
			this._leftArr[2] = this._disableLabelLeft;
			this._leftArr[3] = this._hoverLabelLeft;
			this._rightArr[0] = this._normalLabelRight;
			this._rightArr[1] = this._normalLabelRightWhite;
			this._rightArr[2] = this._disableLabelRight;
			this._rightArr[3] = this._hoverLabelRight;
			this._inited = true;
		}
	}

	// Token: 0x040024C2 RID: 9410
	private bool _inited = false;

	// Token: 0x040024C3 RID: 9411
	private readonly TextMeshProUGUI[] _leftArr = new TextMeshProUGUI[4];

	// Token: 0x040024C4 RID: 9412
	private readonly TextMeshProUGUI[] _rightArr = new TextMeshProUGUI[4];

	// Token: 0x040024C5 RID: 9413
	private TextMeshProUGUI _disableLabelLeft;

	// Token: 0x040024C6 RID: 9414
	private TextMeshProUGUI _disableLabelRight;

	// Token: 0x040024C7 RID: 9415
	private TextMeshProUGUI _hoverLabelLeft;

	// Token: 0x040024C8 RID: 9416
	private TextMeshProUGUI _hoverLabelRight;

	// Token: 0x040024C9 RID: 9417
	private TextMeshProUGUI _normalLabelLeft;

	// Token: 0x040024CA RID: 9418
	private TextMeshProUGUI _normalLabelLeftWhite;

	// Token: 0x040024CB RID: 9419
	private TextMeshProUGUI _normalLabelRight;

	// Token: 0x040024CC RID: 9420
	private TextMeshProUGUI _normalLabelRightWhite;
}
