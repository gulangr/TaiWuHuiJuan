using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000057 RID: 87
[RequireComponent(typeof(TMP_InputField))]
public class DelayedInputFieldEvent : MonoBehaviour
{
	// Token: 0x060002E4 RID: 740 RVA: 0x000118E0 File Offset: 0x0000FAE0
	private void Awake()
	{
		this.inputField = base.GetComponent<TMP_InputField>();
		bool flag = this.inputField == null;
		if (flag)
		{
			Debug.LogError("No TMP_InputField found on the GameObject.");
			base.enabled = false;
		}
		else
		{
			this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.OnInputValueChanged));
		}
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x00011940 File Offset: 0x0000FB40
	private void OnInputValueChanged(string text)
	{
		bool flag = this.delayCoroutine != null;
		if (flag)
		{
			base.StopCoroutine(this.delayCoroutine);
		}
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
		if (activeInHierarchy)
		{
			this.delayCoroutine = base.StartCoroutine(this.TriggerEventAfterDelay(text));
		}
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x0001198E File Offset: 0x0000FB8E
	private IEnumerator TriggerEventAfterDelay(string text)
	{
		yield return new WaitForSeconds(this.delayTime);
		this.onValueChanged.Invoke(text);
		yield break;
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x000119A4 File Offset: 0x0000FBA4
	private void OnDestroy()
	{
		bool flag = this.inputField != null;
		if (flag)
		{
			this.inputField.onValueChanged.RemoveListener(new UnityAction<string>(this.OnInputValueChanged));
		}
	}

	// Token: 0x04000191 RID: 401
	public UnityEvent<string> onValueChanged = new UnityEvent<string>();

	// Token: 0x04000192 RID: 402
	private TMP_InputField inputField;

	// Token: 0x04000193 RID: 403
	public float delayTime = 0.3f;

	// Token: 0x04000194 RID: 404
	private Coroutine delayCoroutine;
}
