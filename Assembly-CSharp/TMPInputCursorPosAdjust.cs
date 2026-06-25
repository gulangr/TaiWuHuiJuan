using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x0200009C RID: 156
[RequireComponent(typeof(TMP_InputField))]
public class TMPInputCursorPosAdjust : MonoBehaviour
{
	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000560 RID: 1376 RVA: 0x000245AC File Offset: 0x000227AC
	private BaseInput InputSystem
	{
		get
		{
			bool flag = EventSystem.current && EventSystem.current.currentInputModule;
			BaseInput result;
			if (flag)
			{
				result = EventSystem.current.currentInputModule.input;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x000245F4 File Offset: 0x000227F4
	private void Start()
	{
		bool flag = null == this._input;
		if (flag)
		{
			this._input = base.GetComponent<TMP_InputField>();
		}
		this._input.onSelect.AddListener(delegate(string str)
		{
			this.Working = true;
		});
		this._input.onValueChanged.AddListener(new UnityAction<string>(this.CalculateCompositeCursorPos));
		this._input.onDeselect.AddListener(delegate(string str)
		{
			this.Working = false;
		});
		this._input.onEndEdit.AddListener(delegate(string str)
		{
			this.Working = false;
		});
		this._input.onSubmit.AddListener(delegate(string str)
		{
			this.Working = false;
		});
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x000246AF File Offset: 0x000228AF
	private void OnDisable()
	{
		this.Working = false;
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x000246BC File Offset: 0x000228BC
	private void Update()
	{
		bool working = this.Working;
		if (working)
		{
			this.CalculateCompositeCursorPos("");
		}
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x000246E4 File Offset: 0x000228E4
	private void CalculateCompositeCursorPos(string str)
	{
		Vector2 leftBottom = this._input.textComponent.rectTransform.rect.min;
		leftBottom = this._input.textComponent.rectTransform.TransformPoint(leftBottom);
		Vector2 pos = UIManager.Instance.UiCamera.WorldToScreenPoint(leftBottom);
		pos.y = (float)Screen.height - pos.y;
		this.InputSystem.compositionCursorPos = pos;
	}

	// Token: 0x04000467 RID: 1127
	private TMP_InputField _input;

	// Token: 0x04000468 RID: 1128
	public bool Working;
}
