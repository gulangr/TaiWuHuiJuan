using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class ConchShipCursor : Refers
{
	// Token: 0x1700004F RID: 79
	// (get) Token: 0x06000298 RID: 664 RVA: 0x0000FEE6 File Offset: 0x0000E0E6
	// (set) Token: 0x06000299 RID: 665 RVA: 0x0000FEED File Offset: 0x0000E0ED
	public static ConchShipCursor Instance { get; private set; }

	// Token: 0x0600029A RID: 666 RVA: 0x0000FEF8 File Offset: 0x0000E0F8
	private void Awake()
	{
		ConchShipCursor.Instance = this;
		this._rectTransform = base.GetComponent<RectTransform>();
		Cursor.visible = true;
		this._usingCursorIcon = false;
		this._loadCursorStarted = false;
		ConchShipCursor._onWheelProgressFullEventList = new List<ValueTuple<string, Action>>();
		this._cursorImg = new GameObject("HiddenCursorImage", new Type[]
		{
			typeof(RectTransform),
			typeof(CImage)
		}).GetComponent<CImage>();
		Transform transform;
		(transform = this._cursorImg.transform).SetParent(base.transform, false);
		transform.localScale = Vector3.one * 0.1f;
		transform.localPosition = Vector3.down * 5000f;
	}

	// Token: 0x0600029B RID: 667 RVA: 0x0000FFB4 File Offset: 0x0000E1B4
	private void LateUpdate()
	{
		bool flag = null == this._cursorIcon && null != AtlasInfo.Instance && !this._loadCursorStarted;
		if (flag)
		{
			this._loadCursorStarted = true;
			this.LoadCursor();
		}
		this.UpdateCursor();
		bool flag2 = this._leftKeepProgressTime > 0f;
		if (flag2)
		{
			this._leftKeepProgressTime -= Time.deltaTime;
			bool flag3 = this._leftKeepProgressTime <= 0f && null != this._progressImg;
			if (flag3)
			{
				this._progress = 0f;
				this._progressImg.DOKill(false);
				this._progressImg.DOFillAmount(0f, 0.1f);
			}
		}
	}

	// Token: 0x0600029C RID: 668 RVA: 0x00010079 File Offset: 0x0000E279
	private void OnApplicationFocus(bool focus)
	{
		Cursor.visible = !this._usingCursorIcon;
	}

	// Token: 0x0600029D RID: 669 RVA: 0x0001008C File Offset: 0x0000E28C
	private void LoadCursor()
	{
		string resPath = "RemakeResources/Prefab/Legacy/Core/Cursor";
		ResLoader.Load<GameObject>(resPath, delegate(GameObject obj)
		{
			obj = Object.Instantiate<GameObject>(obj, base.transform, false);
			bool flag = null == obj;
			if (!flag)
			{
				this.Objects.Clear();
				this.Names.Clear();
				Refers refers = obj.GetComponent<Refers>();
				this.Objects.AddRange(refers.Objects);
				this.Names.AddRange(refers.Names);
				this._cursorIcon = base.CGet<CImage>("Cursor");
				this._progressImg = base.CGet<CImage>("OpenMapProgress");
				this._progressImg.fillAmount = 0f;
				this._progress = 0f;
				this._leftKeepProgressTime = 0f;
				this._progressImg.fillAmount = 0f;
				this._cursorIcon.enabled = false;
				this.SetDefaultCursor();
				this._selectCount = base.CGet<Refers>("SelectCount");
				this._selectCount.gameObject.SetActive(false);
				this._selectApprovedTaiwu = base.CGet<Refers>("SelectApprovedTaiwu");
				this._selectApprovedTaiwu.gameObject.SetActive(false);
				this._lifeSkillCombatRoot = base.CGet<Refers>("LifeSkillCombatRoot");
				this._lifeSkillCombatRoot.gameObject.SetActive(false);
				this._usingMedicineRoot = base.CGet<Refers>("UsingMedicineRoot");
				this._usingMedicineRoot.gameObject.SetActive(false);
			}
		}, null, false);
	}

	// Token: 0x0600029E RID: 670 RVA: 0x000100B8 File Offset: 0x0000E2B8
	private void UpdateCursor()
	{
		bool flag = null == this._cursorIcon;
		if (!flag)
		{
			this._cursorIcon.rectTransform.localPosition = UIManager.Instance.MousePosToLocalPos(this._rectTransform);
		}
	}

	// Token: 0x0600029F RID: 671 RVA: 0x000100FE File Offset: 0x0000E2FE
	public void SetCursor(int num = -1)
	{
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x00010101 File Offset: 0x0000E301
	public void SetCursorNumColor(Color color)
	{
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x00010104 File Offset: 0x0000E304
	public void SetCursorImageWithKey(string key, string cursorName, float pivotX = -1f, float pivotY = -1f)
	{
		bool flag = key == "";
		if (flag)
		{
			Debug.LogWarning("Cannot use empty string for key");
		}
		else
		{
			bool flag2 = !this._currentSpecialImageKey.IsNullOrEmpty() && key != this._currentSpecialImageKey;
			if (!flag2)
			{
				this._currentSpecialImageKey = key;
				this.SetCursorImage(cursorName, pivotX, pivotY);
			}
		}
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x00010164 File Offset: 0x0000E364
	public void SetDefaultCursorAndReleaseKey(string key)
	{
		bool flag = key == "";
		if (flag)
		{
			Debug.LogWarning("Cannot use empty string for key");
		}
		else
		{
			bool flag2 = !this._currentSpecialImageKey.IsNullOrEmpty() && key != this._currentSpecialImageKey;
			if (!flag2)
			{
				this._currentSpecialImageKey = null;
				this.SetDefaultCursor();
			}
		}
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x000101C0 File Offset: 0x0000E3C0
	public void TrySetDefaultCursor()
	{
		bool flag = !this._currentSpecialImageKey.IsNullOrEmpty();
		if (!flag)
		{
			this.SetDefaultCursor();
		}
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x000101EC File Offset: 0x0000E3EC
	public void TrySetClickableCursor()
	{
		bool flag = !this.CanChange;
		if (!flag)
		{
			bool flag2 = !this._currentSpecialImageKey.IsNullOrEmpty();
			if (!flag2)
			{
				this.SetCursorImage(string.Empty, -1f, -1f);
				Cursor.visible = true;
				Cursor.SetCursor(this.ClickableCursor, Vector2.zero, CursorMode.Auto);
			}
		}
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x0001024C File Offset: 0x0000E44C
	public void SetCursorImage(string cursorName, float pivotX = -1f, float pivotY = -1f)
	{
		bool flag = !this.CanChange || !this._cursorIcon;
		if (!flag)
		{
			this._cursorIcon.enabled = true;
			this._cursorIcon.SetSprite(cursorName, false, null);
			bool flag2 = string.IsNullOrEmpty(cursorName);
			if (flag2)
			{
				this._cursorIcon.rectTransform.localPosition = Vector3.down * 5000f;
				this._usingCursorIcon = false;
			}
			else
			{
				Cursor.visible = false;
				this._usingCursorIcon = true;
				this._cursorIcon.SetNativeSize();
				this._cursorIcon.rectTransform.pivot = ((pivotX >= 0f && pivotY >= 0f) ? new Vector2(pivotX, pivotY) : Vector2.up);
			}
		}
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x00010318 File Offset: 0x0000E518
	public void SetDefaultCursor()
	{
		bool flag = !this.CanChange || !this._cursorIcon;
		if (!flag)
		{
			this.SetCursorImage(string.Empty, -1f, -1f);
			Cursor.visible = true;
			this._usingCursorIcon = false;
			Cursor.SetCursor(this.DefaultCursor, Vector2.zero, CursorMode.Auto);
			RectTransform rectTransform = this._cursorIcon.GetComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.up;
			rectTransform.anchorMax = Vector2.up;
			rectTransform.sizeDelta = new Vector2(45f, 45f);
			this._cursorIcon.rectTransform.pivot = Vector2.up;
		}
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x000103D0 File Offset: 0x0000E5D0
	[Obsolete]
	public void SetCursorColor(Color color)
	{
		bool flag = null != this._cursorIcon;
		if (flag)
		{
			this._cursorIcon.color = color;
		}
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x000103FB File Offset: 0x0000E5FB
	public void SetCursorVisible(bool visible)
	{
		Cursor.visible = visible;
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x00010408 File Offset: 0x0000E608
	public RectTransform GetCursorRectTransform()
	{
		return this._cursorIcon.rectTransform;
	}

	// Token: 0x060002AA RID: 682 RVA: 0x00010428 File Offset: 0x0000E628
	[Obsolete]
	public Vector2 GetCursorSize()
	{
		return this._cursorIcon.rectTransform.sizeDelta;
	}

	// Token: 0x060002AB RID: 683 RVA: 0x0001044C File Offset: 0x0000E64C
	public void AddMouseWheelProgressFullHandler(string pageName, Action handler)
	{
		for (int i = 0; i < ConchShipCursor._onWheelProgressFullEventList.Count; i++)
		{
			ValueTuple<string, Action> tuple = ConchShipCursor._onWheelProgressFullEventList[i];
			bool flag = tuple.Item1 == pageName && tuple.Item2 == handler;
			if (flag)
			{
				return;
			}
		}
		ConchShipCursor._onWheelProgressFullEventList.Add(new ValueTuple<string, Action>(pageName, handler));
	}

	// Token: 0x060002AC RID: 684 RVA: 0x000104B8 File Offset: 0x0000E6B8
	public void RemoveMouseWheelProgressFullHandler(Action handler)
	{
		for (int i = 0; i < ConchShipCursor._onWheelProgressFullEventList.Count; i++)
		{
			ValueTuple<string, Action> tuple = ConchShipCursor._onWheelProgressFullEventList[i];
			bool flag = tuple.Item2 == handler;
			if (flag)
			{
				ConchShipCursor._onWheelProgressFullEventList.RemoveAt(i);
				break;
			}
		}
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0001050C File Offset: 0x0000E70C
	public void SetWheelProgress(float progress)
	{
		this._progressImg.fillAmount = progress;
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0001051C File Offset: 0x0000E71C
	public void AddWheelProgress(float addProgress)
	{
		bool flag = !this._progressFull;
		if (flag)
		{
			float progress = Mathf.Max(this._progressImg.fillAmount + addProgress, 0f);
			bool flag2 = Math.Abs(progress - this._progress) < 0.0001f;
			if (!flag2)
			{
				this._progressImg.DOKill(false);
				this._progress = progress;
				bool flag3 = progress >= 1f;
				if (flag3)
				{
					this._leftKeepProgressTime = 0f;
					this._progressFull = true;
					this._progressImg.DOFillAmount(1f, 0.1f).OnComplete(delegate
					{
						bool flag4 = ConchShipCursor._onWheelProgressFullEventList.Count > 0;
						if (flag4)
						{
							Action item = ConchShipCursor._onWheelProgressFullEventList[ConchShipCursor._onWheelProgressFullEventList.Count - 1].Item2;
							if (item != null)
							{
								item();
							}
						}
						this._progressImg.fillAmount = 0f;
						this._progressFull = false;
					});
				}
				else
				{
					this._leftKeepProgressTime = 1f;
					this._progressImg.DOFillAmount(progress, 0.1f);
				}
			}
		}
	}

	// Token: 0x060002AF RID: 687 RVA: 0x000105F0 File Offset: 0x0000E7F0
	public void SetSelectCountActive(bool active)
	{
		Refers selectCount = this._selectCount;
		if (selectCount != null)
		{
			selectCount.gameObject.SetActive(active);
		}
		Refers selectApprovedTaiwu = this._selectApprovedTaiwu;
		if (selectApprovedTaiwu != null)
		{
			selectApprovedTaiwu.gameObject.SetActive(false);
		}
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00010623 File Offset: 0x0000E823
	public void SetSelectCount(int cur, int max)
	{
		this.SetSelectCountCur(cur, null);
		this.SetSelectCountMax(max, null);
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x00010638 File Offset: 0x0000E838
	public void SetSelectCountCur(int count, string color = null)
	{
		string text = color.IsNullOrEmpty() ? count.ToString() : count.ToString().SetColor(color);
		Refers selectCount = this._selectCount;
		if (selectCount != null)
		{
			TextMeshProUGUI textMeshProUGUI = selectCount.CGet<TextMeshProUGUI>("Cur");
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.SetText(text, true);
			}
		}
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x0001068C File Offset: 0x0000E88C
	public void SetSelectCountMax(int count, string color = null)
	{
		string text = color.IsNullOrEmpty() ? count.ToString() : count.ToString().SetColor(color);
		Refers selectCount = this._selectCount;
		if (selectCount != null)
		{
			TextMeshProUGUI textMeshProUGUI = selectCount.CGet<TextMeshProUGUI>("Max");
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.SetText(text, true);
			}
		}
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x000106DD File Offset: 0x0000E8DD
	public void SetSelectCountDescOn(bool isOn)
	{
		Refers selectCount = this._selectCount;
		if (selectCount != null)
		{
			GameObject gameObject = selectCount.CGet<GameObject>("Desc");
			if (gameObject != null)
			{
				gameObject.SetActive(isOn);
			}
		}
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x00010704 File Offset: 0x0000E904
	public void SetSelectCountDescText(string text)
	{
		Refers selectCount = this._selectCount;
		if (selectCount != null)
		{
			GameObject gameObject = selectCount.CGet<GameObject>("Desc");
			if (gameObject != null)
			{
				Refers component = gameObject.GetComponent<Refers>();
				if (component != null)
				{
					TextMeshProUGUI textMeshProUGUI = component.CGet<TextMeshProUGUI>("ConfirmLabel");
					if (textMeshProUGUI != null)
					{
						textMeshProUGUI.SetText(text, true);
					}
				}
			}
		}
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00010751 File Offset: 0x0000E951
	public void SetSelectApprovedTaiwuActive(bool active)
	{
		Refers selectApprovedTaiwu = this._selectApprovedTaiwu;
		if (selectApprovedTaiwu != null)
		{
			selectApprovedTaiwu.gameObject.SetActive(active);
		}
		Refers selectCount = this._selectCount;
		if (selectCount != null)
		{
			selectCount.gameObject.SetActive(false);
		}
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00010784 File Offset: 0x0000E984
	public void SetSelectApprovedTaiwuCur(string count)
	{
		Refers selectApprovedTaiwu = this._selectApprovedTaiwu;
		if (selectApprovedTaiwu != null)
		{
			TextMeshProUGUI textMeshProUGUI = selectApprovedTaiwu.CGet<TextMeshProUGUI>("Cur");
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.SetText(count, true);
			}
		}
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x000107AB File Offset: 0x0000E9AB
	public void SetSelectApprovedTaiwuMax(string count)
	{
		Refers selectApprovedTaiwu = this._selectApprovedTaiwu;
		if (selectApprovedTaiwu != null)
		{
			TextMeshProUGUI textMeshProUGUI = selectApprovedTaiwu.CGet<TextMeshProUGUI>("Max");
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.SetText(count, true);
			}
		}
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x000107D4 File Offset: 0x0000E9D4
	public void ShowLifeSkillCombatUseStrategyTip(string content)
	{
		this._lifeSkillCombatRoot.gameObject.SetActive(true);
		this._lifeSkillCombatRoot.CGet<TextMeshProUGUI>("ConfirmLabel").text = content;
		this._lifeSkillCombatRoot.CGet<TextMeshProUGUI>("CancelLabel").text = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_TargetCancel);
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x0001082B File Offset: 0x0000EA2B
	public void HideLifeSkillCombatUseStrategyTip()
	{
		this._lifeSkillCombatRoot.gameObject.SetActive(false);
	}

	// Token: 0x060002BA RID: 698 RVA: 0x00010840 File Offset: 0x0000EA40
	public float GetLifeSkillCombatUseStrategyTipHeight()
	{
		return this._lifeSkillCombatRoot.GetComponent<RectTransform>().rect.height;
	}

	// Token: 0x0400015D RID: 349
	private bool _loadCursorStarted;

	// Token: 0x0400015E RID: 350
	public const string DefaultCursorSpriteName = "sp_cursor_base";

	// Token: 0x0400015F RID: 351
	public const string ClickableCourseSpriteName = "sp_cursor_clickable";

	// Token: 0x04000160 RID: 352
	public Texture2D DefaultCursor;

	// Token: 0x04000161 RID: 353
	public Texture2D ClickableCursor;

	// Token: 0x04000162 RID: 354
	private const float KeepProgressTime = 1f;

	// Token: 0x04000163 RID: 355
	private RectTransform _rectTransform;

	// Token: 0x04000164 RID: 356
	private CImage _cursorImg;

	// Token: 0x04000165 RID: 357
	private CImage _cursorIcon;

	// Token: 0x04000166 RID: 358
	private CImage _progressImg;

	// Token: 0x04000167 RID: 359
	private float _progress;

	// Token: 0x04000168 RID: 360
	private float _leftKeepProgressTime;

	// Token: 0x04000169 RID: 361
	private bool _progressFull = false;

	// Token: 0x0400016A RID: 362
	private static List<ValueTuple<string, Action>> _onWheelProgressFullEventList;

	// Token: 0x0400016B RID: 363
	public bool CanChange = true;

	// Token: 0x0400016C RID: 364
	private bool _usingCursorIcon = false;

	// Token: 0x0400016D RID: 365
	private string _currentSpecialImageKey = string.Empty;

	// Token: 0x0400016E RID: 366
	private Refers _selectCount;

	// Token: 0x0400016F RID: 367
	private Refers _selectApprovedTaiwu;

	// Token: 0x04000170 RID: 368
	private Refers _lifeSkillCombatRoot;

	// Token: 0x04000171 RID: 369
	private Refers _usingMedicineRoot;
}
