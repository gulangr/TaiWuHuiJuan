using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000090 RID: 144
public class Renamer : MonoBehaviour
{
	// Token: 0x17000083 RID: 131
	// (get) Token: 0x0600051A RID: 1306 RVA: 0x0002326A File Offset: 0x0002146A
	public static bool ShouldCancel
	{
		get
		{
			return CommonCommandKit.Esc.Check(Renamer._emptyUI, true, false, false, true, false) || CommonCommandKit.RightMouse.Check(Renamer._emptyUI, true, false, false, true, false);
		}
	}

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x0600051B RID: 1307 RVA: 0x00023299 File Offset: 0x00021499
	// (set) Token: 0x0600051C RID: 1308 RVA: 0x000232A4 File Offset: 0x000214A4
	public bool CanRename
	{
		get
		{
			return this._canRename;
		}
		set
		{
			bool flag = value == this._canRename;
			if (!flag)
			{
				GameObject renameButton = this.RenameButton;
				this._canRename = value;
				renameButton.SetActive(value);
				this.Name.transform.SetParent(value ? this.RenameRect : this.CannotRenameRect, false);
			}
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x0600051D RID: 1309 RVA: 0x000232FA File Offset: 0x000214FA
	// (set) Token: 0x0600051E RID: 1310 RVA: 0x00023307 File Offset: 0x00021507
	public int CharacterLimit
	{
		get
		{
			return this.Input.characterLimit;
		}
		set
		{
			this.Input.characterLimit = value;
		}
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x00023318 File Offset: 0x00021518
	public void Refresh(string newName, int characterLimit = -1, bool canRename = true, string sensitiveFallback = null)
	{
		bool flag = characterLimit > 0;
		if (flag)
		{
			this.CharacterLimit = characterLimit;
		}
		this.Input.gameObject.SetActive(false);
		this.CanRename = canRename;
		if (canRename)
		{
			this.RenameButton.SetActive(true);
		}
		bool flag2 = sensitiveFallback != null && this.Input.SensitiveWordHandle(ref newName);
		if (flag2)
		{
			TMP_Text name = this.Name;
			this.Input.text = sensitiveFallback;
			name.text = sensitiveFallback;
		}
		else
		{
			bool flag3 = newName != null;
			if (flag3)
			{
				this.Name.text = (this.Input.text = newName);
			}
		}
		this.Name.gameObject.SetActive(true);
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x000233D7 File Offset: 0x000215D7
	public void TriggerStartRename()
	{
		this.StartRename.Invoke(this, "");
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x000233EC File Offset: 0x000215EC
	public void SubmitOrCancelRename(string newText)
	{
		bool trimResult = this.TrimResult;
		if (trimResult)
		{
			newText = newText.Trim();
		}
		bool sensitive = false;
		bool hasSensitiveCheck = this.HasSensitiveCheck;
		if (hasSensitiveCheck)
		{
			bool flag;
			sensitive = (flag = this.Input.SensitiveWordHandle(ref newText));
			if (flag)
			{
				this.CommonWarningCanvasGroup.alpha = 1f;
				bool flag2 = this._sensitiveWordTipCoroutine != null;
				if (flag2)
				{
					base.StopCoroutine(this._sensitiveWordTipCoroutine);
				}
				Tween sensitiveWordTipTween = this._sensitiveWordTipTween;
				if (sensitiveWordTipTween != null)
				{
					sensitiveWordTipTween.Kill(false);
				}
				this._sensitiveWordTipCoroutine = base.StartCoroutine(this.SensitiveWarning());
			}
		}
		bool flag3 = Renamer.ShouldCancel || sensitive;
		if (flag3)
		{
			this.CancelRename.Invoke(this, newText);
		}
		else
		{
			bool handleQuickHide = this.HandleQuickHide;
			if (handleQuickHide)
			{
				UIManager.Instance.SetEscHandler(null);
			}
			this.SubmitRename.Invoke(this, newText);
		}
		this.EndRename.Invoke(this, newText);
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x000234D4 File Offset: 0x000216D4
	public static void DefaultOnStartRename(Renamer self, string nonsense = "")
	{
		self.RenameButton.SetActive(false);
		self.Name.gameObject.SetActive(false);
		self.Input.gameObject.SetActive(true);
		self.Input.InputOnSelectBindMoveTextEnd();
		bool handleQuickHide = self.HandleQuickHide;
		if (handleQuickHide)
		{
			Renamer.SelectAndSetEscHandler(self.Input, false);
		}
		else
		{
			self.Input.Select();
		}
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x00023544 File Offset: 0x00021744
	public static void DefaultOnEndRename(Renamer self, string newText)
	{
		self.Refresh(null, -1, true, null);
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x00023554 File Offset: 0x00021754
	public static void SetEscHandler(GameObject inputField, bool delay = true)
	{
		Action action = delegate()
		{
			bool activeInHierarchy = inputField.activeInHierarchy;
			if (activeInHierarchy)
			{
				Renamer.SetEscHandler(inputField, delay);
			}
		};
		Action delayed = delay ? delegate()
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, action);
		} : action;
		UIManager.Instance.SetEscHandler(delayed);
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x000235B1 File Offset: 0x000217B1
	public static void SelectAndSetEscHandler(Selectable inputField, bool delay = true)
	{
		inputField.Select();
		Renamer.SetEscHandler(inputField.gameObject, delay);
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x000235C8 File Offset: 0x000217C8
	public IEnumerator SensitiveWarning()
	{
		yield return new WaitForSeconds(SensitiveWordsSystem.SensitiveWordAnimationStayTime);
		bool activeInHierarchy = this.CommonWarningCanvasGroup.gameObject.activeInHierarchy;
		if (activeInHierarchy)
		{
			this._sensitiveWordTipTween = this.CommonWarningCanvasGroup.DOFade(0f, SensitiveWordsSystem.SensitiveWordAnimationFadeTime);
		}
		yield break;
	}

	// Token: 0x04000420 RID: 1056
	private static readonly UIElement _emptyUI = new UIElement
	{
		ForceListenCommand = true
	};

	// Token: 0x04000421 RID: 1057
	public TextMeshProUGUI Name;

	// Token: 0x04000422 RID: 1058
	public TMP_InputField Input;

	// Token: 0x04000423 RID: 1059
	public GameObject RenameButton;

	// Token: 0x04000424 RID: 1060
	public RectTransform RenameRect;

	// Token: 0x04000425 RID: 1061
	public RectTransform CannotRenameRect;

	// Token: 0x04000426 RID: 1062
	public CanvasGroup CommonWarningCanvasGroup;

	// Token: 0x04000427 RID: 1063
	public UnityEvent<Renamer, string> EndRename;

	// Token: 0x04000428 RID: 1064
	public UnityEvent<Renamer, string> StartRename;

	// Token: 0x04000429 RID: 1065
	public UnityEvent<Renamer, string> CancelRename;

	// Token: 0x0400042A RID: 1066
	public UnityEvent<Renamer, string> SubmitRename;

	// Token: 0x0400042B RID: 1067
	public InputCharValidator Validator;

	// Token: 0x0400042C RID: 1068
	public bool HandleQuickHide = true;

	// Token: 0x0400042D RID: 1069
	public bool HasSensitiveCheck = true;

	// Token: 0x0400042E RID: 1070
	public bool TrimResult = true;

	// Token: 0x0400042F RID: 1071
	private bool _canRename = true;

	// Token: 0x04000430 RID: 1072
	private Coroutine _sensitiveWordTipCoroutine;

	// Token: 0x04000431 RID: 1073
	private Tween _sensitiveWordTipTween;
}
