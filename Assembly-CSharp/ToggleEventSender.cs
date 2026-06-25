using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200009F RID: 159
[RequireComponent(typeof(Toggle))]
public class ToggleEventSender : MonoBehaviour
{
	// Token: 0x06000582 RID: 1410 RVA: 0x0002507F File Offset: 0x0002327F
	private void Awake()
	{
		this._toggle = base.GetComponent<Toggle>();
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x00025090 File Offset: 0x00023290
	private void OnEnable()
	{
		bool flag = this.isSendOnEnable;
		if (flag)
		{
			this.OnToggleValueChanged(this._toggle.isOn);
		}
		this._toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x000250DC File Offset: 0x000232DC
	private void OnDisable()
	{
		bool flag = this._toggle != null;
		if (flag)
		{
			this._toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleValueChanged));
		}
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x00025119 File Offset: 0x00023319
	private void OnDestroy()
	{
		this._toggle = null;
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x00025124 File Offset: 0x00023324
	private void OnToggleValueChanged(bool isOn)
	{
		if (isOn)
		{
			UnityEvent onToggleOn = this.OnToggleOn;
			if (onToggleOn != null)
			{
				onToggleOn.Invoke();
			}
		}
		else
		{
			UnityEvent onToggleOff = this.OnToggleOff;
			if (onToggleOff != null)
			{
				onToggleOff.Invoke();
			}
		}
	}

	// Token: 0x04000481 RID: 1153
	private Toggle _toggle;

	// Token: 0x04000482 RID: 1154
	public bool isSendOnEnable = true;

	// Token: 0x04000483 RID: 1155
	public UnityEvent OnToggleOn;

	// Token: 0x04000484 RID: 1156
	public UnityEvent OnToggleOff;
}
