using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x02000059 RID: 89
public class DimensionsChangeListener : UIBehaviour
{
	// Token: 0x060002F0 RID: 752 RVA: 0x00011B50 File Offset: 0x0000FD50
	protected override void OnRectTransformDimensionsChange()
	{
		bool flag = !base.gameObject.activeInHierarchy;
		if (!flag)
		{
			base.OnRectTransformDimensionsChange();
			bool flag2 = this.EventType > EEvents.Invalid;
			if (flag2)
			{
				GEvent.OnEvent(this.EventType, null);
			}
			base.StartCoroutine(this.CallChangeEvent());
		}
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x00011BA5 File Offset: 0x0000FDA5
	private IEnumerator CallChangeEvent()
	{
		yield return new WaitForEndOfFrame();
		UnityEvent onDimensionsChange = this.OnDimensionsChange;
		if (onDimensionsChange != null)
		{
			onDimensionsChange.Invoke();
		}
		yield break;
	}

	// Token: 0x0400019A RID: 410
	public EEvents EventType;

	// Token: 0x0400019B RID: 411
	public UnityEvent OnDimensionsChange;
}
