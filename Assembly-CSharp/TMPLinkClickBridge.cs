using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200009D RID: 157
[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPLinkClickBridge : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	// Token: 0x0600056A RID: 1386 RVA: 0x0002479C File Offset: 0x0002299C
	private void Awake()
	{
		bool flag = null == this._tmpText;
		if (flag)
		{
			this._tmpText = base.GetComponent<TextMeshProUGUI>();
		}
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x000247C6 File Offset: 0x000229C6
	public void OnPointerEnter(PointerEventData eventData)
	{
		this._needCheckId = true;
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x000247D0 File Offset: 0x000229D0
	public void OnPointerExit(PointerEventData eventData)
	{
		this._needCheckId = false;
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x000247DC File Offset: 0x000229DC
	public void OnPointerClick(PointerEventData eventData)
	{
		bool flag = eventData.button > PointerEventData.InputButton.Left;
		if (!flag)
		{
			bool flag2 = this._needCheckId && this._tmpText;
			if (flag2)
			{
				int linkIndex = TMP_TextUtilities.FindIntersectingLink(this._tmpText, Input.mousePosition, UIManager.Instance.UiCamera);
				bool flag3 = linkIndex != -1;
				if (flag3)
				{
					TMP_LinkInfo linkInfo = this._tmpText.textInfo.linkInfo[linkIndex];
					string linkId = linkInfo.GetLinkID();
					bool flag4 = !string.IsNullOrEmpty(linkId);
					if (flag4)
					{
						Action<string> onLinkClickEvent = this.OnLinkClickEvent;
						if (onLinkClickEvent != null)
						{
							onLinkClickEvent(linkId);
						}
					}
				}
			}
		}
	}

	// Token: 0x04000469 RID: 1129
	private bool _needCheckId;

	// Token: 0x0400046A RID: 1130
	private TextMeshProUGUI _tmpText;

	// Token: 0x0400046B RID: 1131
	public Action<string> OnLinkClickEvent;
}
