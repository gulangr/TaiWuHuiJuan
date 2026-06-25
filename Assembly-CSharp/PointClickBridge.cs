using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000085 RID: 133
public class PointClickBridge : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x060004E2 RID: 1250 RVA: 0x00021FC8 File Offset: 0x000201C8
	public void OnPointerClick(PointerEventData eventData)
	{
		bool flag = eventData.button == PointerEventData.InputButton.Right;
		if (flag)
		{
			Action onRightClick = this.OnRightClick;
			if (onRightClick != null)
			{
				onRightClick();
			}
		}
		else
		{
			bool flag2 = eventData.button == PointerEventData.InputButton.Left;
			if (flag2)
			{
				bool flag3 = this.OnDoubleClick == null;
				if (flag3)
				{
					Action onLeftClick = this.OnLeftClick;
					if (onLeftClick != null)
					{
						onLeftClick();
					}
				}
				else
				{
					float tmClick = Time.realtimeSinceStartup;
					float gap = (Math.Abs(this.AdjustDoubleClickGap) <= 0f) ? PointClickBridge.DoubleClickGap : this.AdjustDoubleClickGap;
					bool flag4 = tmClick - this.tmLastClick < gap;
					if (flag4)
					{
						base.CancelInvoke();
						Action onDoubleClick = this.OnDoubleClick;
						if (onDoubleClick != null)
						{
							onDoubleClick();
						}
					}
					else
					{
						base.Invoke("InvokeLeftClickEvent", gap + 0.05f);
					}
					this.tmLastClick = tmClick;
				}
			}
		}
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x000220A2 File Offset: 0x000202A2
	private void InvokeLeftClickEvent()
	{
		Action onLeftClick = this.OnLeftClick;
		if (onLeftClick != null)
		{
			onLeftClick();
		}
	}

	// Token: 0x040003E8 RID: 1000
	public Action OnLeftClick;

	// Token: 0x040003E9 RID: 1001
	public Action OnRightClick;

	// Token: 0x040003EA RID: 1002
	public Action OnDoubleClick;

	// Token: 0x040003EB RID: 1003
	public float AdjustDoubleClickGap = 0f;

	// Token: 0x040003EC RID: 1004
	public static float DoubleClickGap = 0.3f;

	// Token: 0x040003ED RID: 1005
	private float tmLastClick = 0f;
}
