using System;
using System.Collections.Generic;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000372 RID: 882
public class UI_ButtonSheet : UIBase
{
	// Token: 0x060033AB RID: 13227 RVA: 0x001989D9 File Offset: 0x00196BD9
	private void Awake()
	{
		PoolManager.SetSrcObject("ButtonSheetButtonKey", base.CGet<GameObject>("BtnPrefab"));
		this._selfRectTransform = base.CGet<RectTransform>("SelfRectTrans");
		this._selfRectTransform.localScale = Vector3.one;
	}

	// Token: 0x060033AC RID: 13228 RVA: 0x00198A14 File Offset: 0x00196C14
	public void OnDisable()
	{
		this.ClearButtons();
		this._sheetInfo = null;
	}

	// Token: 0x060033AD RID: 13229 RVA: 0x00198A25 File Offset: 0x00196C25
	private void OnDestroy()
	{
		PoolManager.RemoveData("ButtonSheetButtonKey");
	}

	// Token: 0x060033AE RID: 13230 RVA: 0x00198A34 File Offset: 0x00196C34
	public override void OnInit(ArgumentBox argsBox)
	{
		this._sheetInfo = null;
		bool flag = !argsBox.Get<List<SheetButtonInfo>>("ButtonInfos", out this._sheetInfo);
		if (!flag)
		{
			bool flag2 = !argsBox.Get("QuickSheet", out this._quickOpenSheet);
			if (flag2)
			{
				this._quickOpenSheet = false;
			}
			argsBox.Get<Action<CButtonObsolete, string>>("ButtonTextHandler", out this._buttonTextHandler);
			this._isSheetReady = false;
			this._selfRectTransform = base.CGet<RectTransform>("SelfRectTrans");
			RectTransform rectTrans;
			argsBox.Get<RectTransform>("TargetRect", out rectTrans);
			bool flag3 = !argsBox.Get<Vector2>("ButtonSize", out this._buttonSize);
			if (flag3)
			{
				this._buttonSize = base.CGet<GameObject>("BtnPrefab").GetComponent<RectTransform>().rect.size;
			}
			UIElement element = this.Element;
			element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(delegate()
			{
				this._selfRectTransform.localScale = Vector3.one;
				this.PlaceBtnsRoot(rectTrans);
				bool flag4 = this._sheetInfo == null || this._sheetInfo.Count <= 0;
				if (!flag4)
				{
					this.RefreshButtonList();
				}
			}));
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(3U, delegate
			{
				this._isSheetReady = true;
			});
		}
	}

	// Token: 0x060033AF RID: 13231 RVA: 0x00198B4C File Offset: 0x00196D4C
	public override void PlayAudioIn()
	{
	}

	// Token: 0x060033B0 RID: 13232 RVA: 0x00198B4F File Offset: 0x00196D4F
	public override void PlayAudioOut()
	{
	}

	// Token: 0x060033B1 RID: 13233 RVA: 0x00198B54 File Offset: 0x00196D54
	public override void QuickHide()
	{
		bool isSheetReady = this._isSheetReady;
		if (isSheetReady)
		{
			base.QuickHide();
		}
		this._buttonTextHandler = null;
	}

	// Token: 0x060033B2 RID: 13234 RVA: 0x00198B7C File Offset: 0x00196D7C
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = "HideSheet" == btnName;
		if (flag)
		{
			UIManager.Instance.HideUI(this.Element);
		}
	}

	// Token: 0x060033B3 RID: 13235 RVA: 0x00198BB4 File Offset: 0x00196DB4
	private void PlaceBtnsRoot(RectTransform rectTrans)
	{
		RectTransform btnRoot = base.CGet<RectTransform>("SheetContainer");
		btnRoot.anchoredPosition = Vector2.up * 4000f;
		Vector2 pivot = Vector2.up;
		bool flag = null == rectTrans;
		if (flag)
		{
			Vector2 mousePos = Input.mousePosition;
			Vector2 mouseViewportPoint = UIManager.Instance.UiCamera.ScreenToViewportPoint(mousePos);
			bool flag2 = mouseViewportPoint.x > 0.8f;
			if (flag2)
			{
				pivot.x = 1f;
			}
			bool flag3 = mouseViewportPoint.y < 0.2f;
			if (flag3)
			{
				pivot.y = 0f;
			}
			btnRoot.pivot = pivot;
			btnRoot.anchorMin = pivot;
			btnRoot.anchorMax = pivot;
			Vector2 mouseWorldPos = UIManager.Instance.UiCamera.ScreenToWorldPoint(mousePos);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(3U, delegate
			{
				this._isSheetReady = true;
				btnRoot.localPosition = this._selfRectTransform.InverseTransformPoint(mouseWorldPos) + new Vector3(0f, 150f, 0f);
			});
		}
		else
		{
			Vector2 rectMinWorldPos = rectTrans.TransformPoint(rectTrans.rect.min);
			Vector2 rectMaxWorldPos = rectTrans.TransformPoint(rectTrans.rect.max);
			Vector2 rectMinViewportPos = UIManager.Instance.UiCamera.WorldToViewportPoint(rectMinWorldPos);
			Vector2 rectMaxViewportPos = UIManager.Instance.UiCamera.WorldToViewportPoint(rectMaxWorldPos);
			bool flag4 = rectMaxViewportPos.x > 0.8f;
			if (flag4)
			{
				pivot.x = 1f;
			}
			bool flag5 = rectMinViewportPos.y < 0.2f;
			if (flag5)
			{
				pivot.y = 0f;
			}
			btnRoot.pivot = pivot;
			btnRoot.anchorMin = pivot;
			btnRoot.anchorMax = pivot;
			Vector2 minPos = this._selfRectTransform.InverseTransformPoint(rectMinWorldPos);
			Vector2 maxPos = this._selfRectTransform.InverseTransformPoint(rectMaxWorldPos);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(3U, delegate
			{
				this._isSheetReady = true;
				bool flag6 = Math.Abs(pivot.x - pivot.y) < 0.1f;
				if (flag6)
				{
					bool flag7 = Math.Abs(pivot.x) < 0.1f;
					if (flag7)
					{
						btnRoot.localPosition = new Vector3(minPos.x, maxPos.y, 0f);
					}
					else
					{
						btnRoot.localPosition = new Vector3(maxPos.x, minPos.y, 0f);
					}
				}
				else
				{
					bool flag8 = Math.Abs(pivot.x) < 0.1f;
					if (flag8)
					{
						btnRoot.localPosition = new Vector3(minPos.x, minPos.y, 0f);
					}
					else
					{
						btnRoot.localPosition = new Vector3(maxPos.x, maxPos.y, 0f);
					}
				}
			});
		}
	}

	// Token: 0x060033B4 RID: 13236 RVA: 0x00198EC0 File Offset: 0x001970C0
	private void ClearButtons()
	{
		RectTransform btnRoot = base.CGet<RectTransform>("SheetContainer");
		foreach (object obj in btnRoot)
		{
			Transform child = (Transform)obj;
			PoolManager.Destroy("ButtonSheetButtonKey", child.gameObject);
		}
	}

	// Token: 0x060033B5 RID: 13237 RVA: 0x00198F30 File Offset: 0x00197130
	private void RefreshButtonList()
	{
		bool flag = this._sheetInfo == null;
		if (flag)
		{
			this.Element.Hide(false);
		}
		else
		{
			RectTransform btnRoot = base.CGet<RectTransform>("SheetContainer");
			for (int i = this._sheetInfo.Count - 1; i >= 0; i--)
			{
				SheetButtonInfo info = this._sheetInfo[i];
				CButtonObsolete button = PoolManager.GetObject<CButtonObsolete>("ButtonSheetButtonKey");
				Transform transform = button.transform;
				transform.SetParent(btnRoot, false);
				RectTransform selfRectTrans = button.GetComponent<RectTransform>();
				selfRectTrans.pivot = btnRoot.pivot;
				selfRectTrans.anchorMin = btnRoot.pivot;
				selfRectTrans.anchorMax = btnRoot.pivot;
				selfRectTrans.SetSize(this._buttonSize);
				transform.localPosition = Vector3.zero;
				bool flag2 = this._buttonTextHandler != null;
				if (flag2)
				{
					this._buttonTextHandler(button, info.ButtonShowText);
				}
				else
				{
					UI_ButtonSheet.HandleButtonLabelContent(button, info.ButtonShowText);
				}
				button.interactable = info.Interactable;
				TooltipInvoker mouseTipDisplayer = button.GetComponent<TooltipInvoker>();
				mouseTipDisplayer.enabled = !info.MouseTip.IsNullOrEmpty();
				bool enabled = mouseTipDisplayer.enabled;
				if (enabled)
				{
					mouseTipDisplayer.PresetParam = new string[]
					{
						info.MouseTip
					};
				}
				button.gameObject.SetActive(true);
				PointerTrigger trigger = button.GetComponent<PointerTrigger>();
				trigger.EnterEvent.RemoveAllListeners();
				trigger.ExitEvent.RemoveAllListeners();
				trigger.EnterEvent.AddListener(delegate()
				{
					Action onButtonEnter = info.OnButtonEnter;
					if (onButtonEnter != null)
					{
						onButtonEnter();
					}
				});
				trigger.ExitEvent.AddListener(delegate()
				{
					Action onButtonExit = info.OnButtonExit;
					if (onButtonExit != null)
					{
						onButtonExit();
					}
				});
				button.ClearAndAddListener(delegate
				{
					Action onButtonClick = info.OnButtonClick;
					if (onButtonClick != null)
					{
						onButtonClick();
					}
					trigger.ExitEvent.Invoke();
					UIManager.Instance.HideUI(this.Element);
				});
				bool flag3 = btnRoot.pivot.y > 0f && i > 0;
				if (flag3)
				{
					button.transform.localPosition = Vector3.down * selfRectTrans.rect.height * (float)i;
				}
				bool flag4 = btnRoot.pivot.y <= 0f && i < this._sheetInfo.Count - 1;
				if (flag4)
				{
					button.transform.localPosition = Vector3.up * selfRectTrans.rect.height * (float)(this._sheetInfo.Count - 1 - i);
				}
			}
		}
	}

	// Token: 0x04002599 RID: 9625
	private List<SheetButtonInfo> _sheetInfo;

	// Token: 0x0400259A RID: 9626
	private const string ButtonKey = "ButtonSheetButtonKey";

	// Token: 0x0400259B RID: 9627
	private RectTransform _selfRectTransform;

	// Token: 0x0400259C RID: 9628
	private bool _quickOpenSheet;

	// Token: 0x0400259D RID: 9629
	private bool _isSheetReady;

	// Token: 0x0400259E RID: 9630
	private Action<CButtonObsolete, string> _buttonTextHandler;

	// Token: 0x0400259F RID: 9631
	private Vector2 _buttonSize;

	// Token: 0x040025A0 RID: 9632
	public static readonly Action<CButtonObsolete, string> HandleButtonLabelContent = delegate(CButtonObsolete button, string showText)
	{
		bool flag = button == null;
		if (!flag)
		{
			TextMeshProUGUI[] labels = button.transform.Find("LabelRoot").GetComponentsInChildren<TextMeshProUGUI>(true);
			Array.ForEach<TextMeshProUGUI>(labels, delegate(TextMeshProUGUI e)
			{
				e.text = showText;
			});
		}
	};
}
