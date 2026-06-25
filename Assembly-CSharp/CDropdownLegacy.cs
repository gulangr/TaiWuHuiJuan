using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020000C8 RID: 200
[Obsolete]
public class CDropdownLegacy : TMP_Dropdown
{
	// Token: 0x060006EC RID: 1772 RVA: 0x000307A4 File Offset: 0x0002E9A4
	public override void OnPointerClick(PointerEventData eventData)
	{
		bool flag = eventData.button > PointerEventData.InputButton.Left;
		if (!flag)
		{
			base.OnPointerClick(eventData);
			bool flag2 = !string.IsNullOrEmpty(this.ClickAudioKey);
			if (flag2)
			{
				AudioManager.Instance.PlaySound(this.ClickAudioKey, false, false);
			}
		}
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x000307F0 File Offset: 0x0002E9F0
	protected override GameObject CreateBlocker(Canvas rootCanvas)
	{
		GameObject go = base.CreateBlocker(rootCanvas);
		UnityEvent<GameObject> unityEvent = this.onShow;
		if (unityEvent != null)
		{
			unityEvent.Invoke(go);
		}
		return go;
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00030820 File Offset: 0x0002EA20
	public void SetOnShow(Action<GameObject> action)
	{
		this.onShow.RemoveAllListeners();
		bool flag = action != null;
		if (flag)
		{
			this.onShow.AddListener(new UnityAction<GameObject>(action.Invoke));
		}
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x0003085A File Offset: 0x0002EA5A
	protected override void DestroyBlocker(GameObject blocker)
	{
		base.DestroyBlocker(blocker);
	}

	// Token: 0x0400076E RID: 1902
	public string ClickAudioKey;

	// Token: 0x0400076F RID: 1903
	[Header("在展示下拉菜单时执行这些命令")]
	[SerializeField]
	private UnityEvent<GameObject> onShow;
}
