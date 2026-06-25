using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000A1 RID: 161
public class TriggerPanel : MonoBehaviour
{
	// Token: 0x0600059C RID: 1436 RVA: 0x0002571C File Offset: 0x0002391C
	public void Init(Func<bool> checkEnterFunc, Action enterAction)
	{
		bool isInited = this._isInited;
		if (!isInited)
		{
			this._checkEnterFunc = checkEnterFunc;
			this._enterAction = enterAction;
			this._isInited = true;
			base.gameObject.SetActive(false);
			PointerTrigger panelPointerTrigger = base.GetComponent<PointerTrigger>();
			RectTransform panelRectTrans = base.GetComponent<RectTransform>();
			bool flag = this.buttonPointerTrigger.EnterEvent == null;
			if (flag)
			{
				this.buttonPointerTrigger.EnterEvent = new UnityEvent();
			}
			UnityAction <>9__2;
			this.buttonPointerTrigger.EnterEvent.AddListener(delegate()
			{
				Func<bool> checkEnterFunc2 = this._checkEnterFunc;
				bool canEnter = checkEnterFunc2 == null || checkEnterFunc2();
				bool flag3 = !canEnter;
				if (!flag3)
				{
					Action enterAction2 = this._enterAction;
					if (enterAction2 != null)
					{
						enterAction2();
					}
					panelPointerTrigger.gameObject.SetActive(true);
					bool flag4 = this.buttonPointerTrigger.ExitEvent == null;
					if (flag4)
					{
						this.buttonPointerTrigger.ExitEvent = new UnityEvent();
					}
					UnityEvent exitEvent = this.buttonPointerTrigger.ExitEvent;
					UnityAction call;
					if ((call = <>9__2) == null)
					{
						call = (<>9__2 = delegate()
						{
							bool flag5 = !RectTransformUtility.RectangleContainsScreenPoint(panelRectTrans, Input.mousePosition, UIManager.Instance.UiCamera);
							if (flag5)
							{
								panelPointerTrigger.gameObject.SetActive(false);
							}
						});
					}
					exitEvent.AddListener(call);
				}
			});
			bool flag2 = panelPointerTrigger.ExitEvent == null;
			if (flag2)
			{
				panelPointerTrigger.ExitEvent = new UnityEvent();
			}
			panelPointerTrigger.ExitEvent.RemoveAllListeners();
			panelPointerTrigger.ExitEvent.AddListener(delegate()
			{
				panelPointerTrigger.gameObject.SetActive(false);
			});
		}
	}

	// Token: 0x04000492 RID: 1170
	public PointerTrigger buttonPointerTrigger;

	// Token: 0x04000493 RID: 1171
	private Func<bool> _checkEnterFunc;

	// Token: 0x04000494 RID: 1172
	private Action _enterAction;

	// Token: 0x04000495 RID: 1173
	private bool _isInited;
}
