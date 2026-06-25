using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000173 RID: 371
public class AdventureEditorBlockElement : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x1700024C RID: 588
	// (get) Token: 0x06001494 RID: 5268 RVA: 0x000800A6 File Offset: 0x0007E2A6
	// (set) Token: 0x06001495 RID: 5269 RVA: 0x000800AE File Offset: 0x0007E2AE
	public string VirtualDirectoryPath { get; private set; }

	// Token: 0x1700024D RID: 589
	// (get) Token: 0x06001496 RID: 5270 RVA: 0x000800B7 File Offset: 0x0007E2B7
	// (set) Token: 0x06001497 RID: 5271 RVA: 0x000800BF File Offset: 0x0007E2BF
	public IReadOnlyList<string> VirtualDirectoryPathSegments { get; private set; }

	// Token: 0x1700024E RID: 590
	// (get) Token: 0x06001498 RID: 5272 RVA: 0x000800C8 File Offset: 0x0007E2C8
	// (set) Token: 0x06001499 RID: 5273 RVA: 0x000800D0 File Offset: 0x0007E2D0
	public string IconName { get; private set; }

	// Token: 0x1700024F RID: 591
	// (get) Token: 0x0600149A RID: 5274 RVA: 0x000800D9 File Offset: 0x0007E2D9
	// (set) Token: 0x0600149B RID: 5275 RVA: 0x000800E1 File Offset: 0x0007E2E1
	public string FullIconName { get; private set; }

	// Token: 0x17000250 RID: 592
	// (get) Token: 0x0600149C RID: 5276 RVA: 0x000800EA File Offset: 0x0007E2EA
	public RectTransform RectTransform
	{
		get
		{
			return (RectTransform)base.transform;
		}
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x000800F8 File Offset: 0x0007E2F8
	private void Awake()
	{
		CButton btn = base.GetComponent<CButton>();
		btn.ClearAndAddListener(new Action(this.OnClick));
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x00080120 File Offset: 0x0007E320
	private void LateUpdate()
	{
		bool flag = this.selectedSign && this.selectedSign.activeSelf;
		if (flag)
		{
			this.selectedSign.SetActive(false);
		}
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x0008015C File Offset: 0x0007E35C
	public void OnClick()
	{
		bool flag = this._lastClickTime < 0f || this._lastClickTime + this.doubleClickDuration < Time.unscaledTime;
		if (flag)
		{
			this._lastClickTime = Time.unscaledTime;
			IAdventureEditorBlockElementHandler handler = this._handler;
			if (handler != null)
			{
				handler.OnClick(this._payload, this.IsSwitchToDecorate());
			}
		}
		else
		{
			this._lastClickTime = -1f;
			IAdventureEditorBlockElementHandler handler2 = this._handler;
			if (handler2 != null)
			{
				handler2.OnDoubleClick(this._payload, this.IsSwitchToDecorate());
			}
		}
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x000801EC File Offset: 0x0007E3EC
	public void OnPointerClick(PointerEventData eventData)
	{
		bool flag = eventData.button == PointerEventData.InputButton.Right;
		if (flag)
		{
			IAdventureEditorBlockElementHandler handler = this._handler;
			if (handler != null)
			{
				handler.OnRightClick(this._payload, this.IsSwitchToDecorate());
			}
		}
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x00080227 File Offset: 0x0007E427
	public void Bind(IAdventureEditorBlockElementHandler handler)
	{
		this._handler = handler;
	}

	// Token: 0x060014A2 RID: 5282 RVA: 0x00080230 File Offset: 0x0007E430
	public void SetVirtualFile(string fullIconName)
	{
		this.VirtualDirectoryPath = null;
		this.VirtualDirectoryPathSegments = null;
		this.FullIconName = fullIconName;
		this._payload = AdventureEditorBlockElementPayload.CreateIcon(fullIconName);
		this.IconName = (fullIconName.StartsWith("adventure_block_") ? fullIconName.Substring("adventure_block_".Length) : fullIconName);
		this.text.text = this.IconName;
		bool flag = this.textNode;
		if (flag)
		{
			this.textNode.SetActive(false);
		}
		this.icon.SetSprite(this.FullIconName, true, null);
		this.blockPiece.SetActive(AdventureEditorBlockElement.IsValidBlockIcon(this.FullIconName, this.IsSwitchToDecorate()));
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x000802E8 File Offset: 0x0007E4E8
	public void SetVirtualDirectory(string directoryName, IReadOnlyList<string> virtualPathSegments, string overrideNameFormat = "")
	{
		this.VirtualDirectoryPathSegments = virtualPathSegments;
		this.VirtualDirectoryPath = ((virtualPathSegments == null || virtualPathSegments.Count == 0) ? string.Empty : string.Join("/", virtualPathSegments));
		this.IconName = string.Empty;
		this.FullIconName = string.Empty;
		this._payload = AdventureEditorBlockElementPayload.CreateDirectory(virtualPathSegments);
		this.icon.sprite = this.dirIcon;
		this.icon.SetNativeSize();
		this.text.text = (string.IsNullOrEmpty(overrideNameFormat) ? directoryName : string.Format(overrideNameFormat, directoryName));
		bool flag = this.textNode;
		if (flag)
		{
			this.textNode.SetActive(true);
		}
		this.blockPiece.SetActive(false);
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x000803AC File Offset: 0x0007E5AC
	public static bool IsValidBlockIcon(string fileName, bool isDecorate)
	{
		return fileName.StartsWith(isDecorate ? "adventure_decorate_" : "adventure_block_");
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x000803D4 File Offset: 0x0007E5D4
	private bool IsSwitchToDecorate()
	{
		AdventureEditorBlockContainer blockContainer = base.GetComponentInParent<AdventureEditorBlockContainer>();
		return blockContainer.IsDecorate;
	}

	// Token: 0x04001134 RID: 4404
	public const string BlockIconPrefix = "adventure_block_";

	// Token: 0x04001135 RID: 4405
	public const string BlockDecorateIconPrefix = "adventure_decorate_";

	// Token: 0x04001136 RID: 4406
	[SerializeField]
	private GameObject selectedSign;

	// Token: 0x04001137 RID: 4407
	[SerializeField]
	private CImage icon;

	// Token: 0x04001138 RID: 4408
	[SerializeField]
	private TextMeshProUGUI text;

	// Token: 0x04001139 RID: 4409
	[SerializeField]
	private GameObject textNode;

	// Token: 0x0400113A RID: 4410
	[SerializeField]
	private Sprite dirIcon;

	// Token: 0x0400113B RID: 4411
	[SerializeField]
	private GameObject blockPiece;

	// Token: 0x0400113C RID: 4412
	[SerializeField]
	private float doubleClickDuration = 0.5f;

	// Token: 0x0400113D RID: 4413
	private float _lastClickTime = -1f;

	// Token: 0x0400113E RID: 4414
	private IAdventureEditorBlockElementHandler _handler;

	// Token: 0x0400113F RID: 4415
	private AdventureEditorBlockElementPayload _payload = AdventureEditorBlockElementPayload.None;

	// Token: 0x04001140 RID: 4416
	internal static string CurrentSelectedIconName;
}
