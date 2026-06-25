using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x02000058 RID: 88
public class DelayPointerTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x060002E9 RID: 745 RVA: 0x00011A00 File Offset: 0x0000FC00
	public void OnPointerEnter(PointerEventData eventData)
	{
		bool flag = this._enterCoroutine != null;
		if (flag)
		{
			base.StopCoroutine(this._enterCoroutine);
		}
		this._enterCoroutine = base.StartCoroutine(this.DelayedEnter());
	}

	// Token: 0x060002EA RID: 746 RVA: 0x00011A3C File Offset: 0x0000FC3C
	public void OnPointerEnterNotDelay(PointerEventData eventData)
	{
		bool flag = this._enterCoroutine != null;
		if (flag)
		{
			base.StopCoroutine(this._enterCoroutine);
		}
		this._isInside = true;
		this._enterCoroutine = null;
		this.enterEvent.Invoke();
	}

	// Token: 0x060002EB RID: 747 RVA: 0x00011A80 File Offset: 0x0000FC80
	public void OnPointerExit(PointerEventData eventData)
	{
		bool flag = this._enterCoroutine != null;
		if (flag)
		{
			base.StopCoroutine(this._enterCoroutine);
			this._enterCoroutine = null;
		}
		bool isInside = this._isInside;
		if (isInside)
		{
			this._isInside = false;
			this.exitEvent.Invoke();
		}
	}

	// Token: 0x060002EC RID: 748 RVA: 0x00011AD0 File Offset: 0x0000FCD0
	private IEnumerator DelayedEnter()
	{
		yield return new WaitForSeconds(this.delay);
		this._isInside = true;
		this._enterCoroutine = null;
		this.enterEvent.Invoke();
		yield break;
	}

	// Token: 0x060002ED RID: 749 RVA: 0x00011AE0 File Offset: 0x0000FCE0
	private void OnDestroy()
	{
		bool flag = this._enterCoroutine != null;
		if (flag)
		{
			base.StopCoroutine(this._enterCoroutine);
		}
	}

	// Token: 0x060002EE RID: 750 RVA: 0x00011B08 File Offset: 0x0000FD08
	private void OnDisable()
	{
		bool isInside = this._isInside;
		if (isInside)
		{
			this._isInside = false;
			this.exitEvent.Invoke();
		}
	}

	// Token: 0x04000195 RID: 405
	[Tooltip("停留多少秒后才视为有效进入")]
	public float delay = 0.3f;

	// Token: 0x04000196 RID: 406
	private Coroutine _enterCoroutine;

	// Token: 0x04000197 RID: 407
	private bool _isInside = false;

	// Token: 0x04000198 RID: 408
	public UnityEvent enterEvent;

	// Token: 0x04000199 RID: 409
	public UnityEvent exitEvent;
}
