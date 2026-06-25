using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

// Token: 0x0200033E RID: 830
public class CombatSkillBreakGridView : MonoBehaviour
{
	// Token: 0x17000557 RID: 1367
	// (get) Token: 0x060030BD RID: 12477 RVA: 0x0017EB6C File Offset: 0x0017CD6C
	// (set) Token: 0x060030BE RID: 12478 RVA: 0x0017EB74 File Offset: 0x0017CD74
	public SkillBreakPlateGrid GridData { get; private set; }

	// Token: 0x17000558 RID: 1368
	// (get) Token: 0x060030BF RID: 12479 RVA: 0x0017EB7D File Offset: 0x0017CD7D
	// (set) Token: 0x060030C0 RID: 12480 RVA: 0x0017EB85 File Offset: 0x0017CD85
	public CombatSkillItem CombatSkillConfig { get; private set; }

	// Token: 0x17000559 RID: 1369
	// (get) Token: 0x060030C1 RID: 12481 RVA: 0x0017EB8E File Offset: 0x0017CD8E
	// (set) Token: 0x060030C2 RID: 12482 RVA: 0x0017EB96 File Offset: 0x0017CD96
	public byte Row { get; private set; }

	// Token: 0x1700055A RID: 1370
	// (get) Token: 0x060030C3 RID: 12483 RVA: 0x0017EB9F File Offset: 0x0017CD9F
	// (set) Token: 0x060030C4 RID: 12484 RVA: 0x0017EBA7 File Offset: 0x0017CDA7
	public byte Column { get; private set; }

	// Token: 0x060030C5 RID: 12485 RVA: 0x0017EBB0 File Offset: 0x0017CDB0
	public void DestroySelf()
	{
		this.GridData = null;
		this.CombatSkillConfig = null;
		bool flag = null != this._showingImage;
		if (flag)
		{
			Object.Destroy(this._showingImage.gameObject);
		}
		bool flag2 = null != this._showingNameText;
		if (flag2)
		{
			Object.Destroy(this._showingNameText.gameObject);
		}
		bool flag3 = null != this._showingDescText;
		if (flag3)
		{
			Object.Destroy(this._showingDescText.gameObject);
		}
		bool flag4 = null != this._successRateBackImage;
		if (flag4)
		{
			Object.Destroy(this._successRateBackImage.gameObject);
		}
		bool flag5 = null != this._successRateText;
		if (flag5)
		{
			Object.Destroy(this._successRateText.gameObject);
		}
		bool flag6 = null != this._canSelectObject;
		if (flag6)
		{
			CombatSkillBreakGridView.RemoveSelectTweenImage(this._canSelectObject.GetComponentInChildren<CImage>());
			Object.Destroy(this._canSelectObject);
		}
		bool flag7 = null != this._toHighLightImage;
		if (flag7)
		{
			this._toHighLightImage.transform.localPosition = Vector3.zero;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060030C6 RID: 12486 RVA: 0x0017ECDC File Offset: 0x0017CEDC
	public void OnPointerEnter()
	{
		bool flag = null == this._toHighLightImage;
		if (!flag)
		{
			bool flag2 = null != this._showingImage;
			if (flag2)
			{
				this._showingImage.gameObject.SetActive(false);
			}
			this._toHighLightImage.transform.position = base.transform.position;
		}
	}

	// Token: 0x060030C7 RID: 12487 RVA: 0x0017ED3C File Offset: 0x0017CF3C
	public void OnPointerExit()
	{
		bool flag = null == this._toHighLightImage;
		if (!flag)
		{
			bool flag2 = null != this._showingImage;
			if (flag2)
			{
				this._showingImage.gameObject.SetActive(true);
			}
			this._toHighLightImage.transform.position = CombatSkillBreakGridView.SrcRefers.transform.position;
		}
	}

	// Token: 0x060030C8 RID: 12488 RVA: 0x0017EDA0 File Offset: 0x0017CFA0
	public void OnSelectThisGrid()
	{
		bool flag = this.GridData.State == ESkillBreakGridState.CanSelect;
		if (flag)
		{
			Action<byte, byte> onSelectNode = CombatSkillBreakGridView.OnSelectNode;
			if (onSelectNode != null)
			{
				onSelectNode(this.Row, this.Column);
			}
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060030C9 RID: 12489 RVA: 0x0017EDEC File Offset: 0x0017CFEC
	private static void StartSelectTween()
	{
		CombatSkillBreakGridView._canSelectImageTweener = DOVirtual.Float(0f, 1f, 1f, delegate(float stepValue)
		{
			bool removeFlag = false;
			for (int i = CombatSkillBreakGridView._tweenTargetList.Count - 1; i >= 0; i--)
			{
				CImage image = CombatSkillBreakGridView._tweenTargetList[i];
				bool flag = null == image;
				if (flag)
				{
					CombatSkillBreakGridView._tweenTargetList.RemoveAt(i);
					removeFlag = true;
				}
				else
				{
					image.SetAlpha(stepValue);
				}
			}
			bool flag2 = removeFlag;
			if (flag2)
			{
				CombatSkillBreakGridView.CheckTween();
			}
		}).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
	}

	// Token: 0x060030CA RID: 12490 RVA: 0x0017EE40 File Offset: 0x0017D040
	private static void AddSelectTweenImage(CImage image)
	{
		bool flag = null == image;
		if (!flag)
		{
			bool flag2 = CombatSkillBreakGridView._tweenTargetList == null;
			if (flag2)
			{
				CombatSkillBreakGridView._tweenTargetList = new List<CImage>();
			}
			bool flag3 = !CombatSkillBreakGridView._tweenTargetList.Contains(image);
			if (flag3)
			{
				CombatSkillBreakGridView._tweenTargetList.Add(image);
			}
			bool flag4 = CombatSkillBreakGridView._canSelectImageTweener == null;
			if (flag4)
			{
				CombatSkillBreakGridView.StartSelectTween();
			}
		}
	}

	// Token: 0x060030CB RID: 12491 RVA: 0x0017EEA4 File Offset: 0x0017D0A4
	private static void RemoveSelectTweenImage(CImage image)
	{
		bool flag = CombatSkillBreakGridView._tweenTargetList == null;
		if (!flag)
		{
			bool flag2 = null == image;
			if (!flag2)
			{
				CombatSkillBreakGridView._tweenTargetList.Remove(image);
				CombatSkillBreakGridView.CheckTween();
			}
		}
	}

	// Token: 0x060030CC RID: 12492 RVA: 0x0017EEE0 File Offset: 0x0017D0E0
	private static void CheckTween()
	{
		bool flag = CombatSkillBreakGridView._tweenTargetList.Count <= 0;
		if (flag)
		{
			CombatSkillBreakGridView._canSelectImageTweener.Kill(false);
			CombatSkillBreakGridView._canSelectImageTweener = null;
		}
	}

	// Token: 0x060030CD RID: 12493 RVA: 0x0017EF16 File Offset: 0x0017D116
	public void SetInteractable(bool interactable)
	{
		base.GetComponent<CEmptyGraphic>().enabled = interactable;
		base.GetComponent<CButtonObsolete>().interactable = interactable;
		base.gameObject.SetActive(interactable);
	}

	// Token: 0x060030CE RID: 12494 RVA: 0x0017EF40 File Offset: 0x0017D140
	public void Refresh(SkillBreakPlateGrid gridData, CombatSkillItem config, byte row, byte col)
	{
		this.GridData = gridData;
		this.CombatSkillConfig = config;
		this.Row = row;
		this.Column = col;
		RectTransform selfTrans = base.GetComponent<RectTransform>();
		selfTrans.SetParent(CombatSkillBreakGridView.InteractRoot, false);
		selfTrans.anchoredPosition = CombatSkillBreakGridView.LocationConvertFunc((int)this.Row, (int)this.Column);
		selfTrans.name = string.Format("{0}_{1}", this.Row, this.Column);
		SkillBreakGridTypeItem gridTypeConfig = SkillBreakGridType.Instance[this.GridData.TemplateId];
		string gridName = gridTypeConfig.Name;
		string gridDesc = gridTypeConfig.Desc;
		this.TipDisplayer.PresetParam[0] = gridName;
		this.TipDisplayer.PresetParam[1] = gridDesc;
		string toHighLightImageKey = string.Empty;
		string nodeDesc = string.Empty;
		bool flag = this.GridData.TemplateId == 0;
		string toShowImageKey;
		string toShowNameTextKey;
		if (flag)
		{
			toShowImageKey = this.StartNodeStateNames[(int)this.GridData.State];
			nodeDesc = config.BreakStart;
			toShowNameTextKey = ((this.GridData.State == ESkillBreakGridState.Failed) ? "Name_Gray" : "Name_Blue");
		}
		else
		{
			bool flag2 = this.GridData.TemplateId == 1;
			if (flag2)
			{
				toShowImageKey = this.FinishNodeStateNames[(int)this.GridData.State];
				nodeDesc = config.BreakEnd;
				toShowNameTextKey = ((this.GridData.State == ESkillBreakGridState.CanSelect || this.GridData.State == ESkillBreakGridState.Selected) ? "Name_Red" : "Name_Gray");
			}
			else
			{
				bool flag3 = this.GridData.TemplateId == 2;
				if (flag3)
				{
					toShowImageKey = this.GoldSpecialNodeStateNames[(int)this.GridData.State];
					toShowNameTextKey = ((this.GridData.State == ESkillBreakGridState.Failed) ? "Name_Red" : "Name_Gold");
					toHighLightImageKey = ((this.GridData.State == ESkillBreakGridState.CanSelect) ? this.GoldSpecialNodeStateNames[2] : string.Empty);
				}
				else
				{
					bool flag4 = this.GridData.TemplateId == 3;
					if (flag4)
					{
						toShowImageKey = this.NormalNodeStateNames[(int)this.GridData.State];
						toShowNameTextKey = ((this.GridData.State == ESkillBreakGridState.Showed) ? "Name_Gray" : "Name_Yellow");
						toHighLightImageKey = ((this.GridData.State == ESkillBreakGridState.CanSelect) ? this.NormalNodeStateNames[2] : string.Empty);
					}
					else
					{
						toShowImageKey = this.PurpleSpecialNodeStateNames[(int)this.GridData.State];
						toShowNameTextKey = ((this.GridData.State == ESkillBreakGridState.Failed) ? "Name_Red" : "Name_Purple");
						toHighLightImageKey = ((this.GridData.State == ESkillBreakGridState.CanSelect) ? this.PurpleSpecialNodeStateNames[2] : string.Empty);
					}
				}
			}
		}
		bool flag5 = null != this._showingNameText;
		if (flag5)
		{
			Object.Destroy(this._showingNameText.gameObject);
		}
		this._showingNameText = Object.Instantiate<TextMeshProUGUI>(CombatSkillBreakGridView.SrcRefers.CGet<TextMeshProUGUI>(toShowNameTextKey), CombatSkillBreakGridView.TextRoot);
		this._showingNameText.transform.SetParent(CombatSkillBreakGridView.TextRoot, false);
		this._showingNameText.transform.position = base.transform.position;
		bool flag6 = this.GridData.TemplateId == 0 || this.GridData.TemplateId == 1;
		if (flag6)
		{
			Vector3 pos = this._showingNameText.transform.localPosition + this.StartOrFinishNodeNameOffset;
			this._showingNameText.transform.localPosition = pos;
		}
		this._showingNameText.text = gridName;
		bool flag7 = null != this._showingImage;
		if (flag7)
		{
			Object.Destroy(this._showingImage.gameObject);
		}
		this._showingImage = Object.Instantiate<CImage>(CombatSkillBreakGridView.SrcRefers.CGet<CImage>(toShowImageKey), CombatSkillBreakGridView.ImageRoot);
		this._showingImage.transform.SetParent(CombatSkillBreakGridView.ImageRoot, false);
		this._showingImage.transform.position = base.transform.position;
		this._toHighLightImage = (toHighLightImageKey.IsNullOrEmpty() ? null : CombatSkillBreakGridView.SrcRefers.CGet<CImage>(toHighLightImageKey));
		bool flag8 = string.IsNullOrEmpty(nodeDesc);
		if (flag8)
		{
			bool flag9 = null != this._showingDescText;
			if (flag9)
			{
				Object.Destroy(this._showingDescText.gameObject);
			}
		}
		else
		{
			bool flag10 = null == this._showingDescText;
			if (flag10)
			{
				this._showingDescText = Object.Instantiate<TextMeshProUGUI>(CombatSkillBreakGridView.SrcRefers.CGet<TextMeshProUGUI>("NodeDesc"), CombatSkillBreakGridView.TextRoot);
			}
			this._showingDescText.transform.SetParent(CombatSkillBreakGridView.TextRoot, false);
			this._showingDescText.transform.position = base.transform.position;
			Vector3 pos2 = this._showingDescText.transform.localPosition + this.StartOrFinishNodeDescOffset;
			this._showingDescText.transform.localPosition = pos2;
			this._showingDescText.text = nodeDesc;
		}
		bool flag11 = this.GridData.TemplateId == 0 || this.GridData.TemplateId == 1;
		if (flag11)
		{
			bool flag12 = null != this._successRateBackImage;
			if (flag12)
			{
				Object.Destroy(this._successRateBackImage.gameObject);
			}
			bool flag13 = null != this._successRateText;
			if (flag13)
			{
				Object.Destroy(this._successRateText.gameObject);
			}
		}
		else
		{
			bool flag14 = null == this._successRateBackImage;
			if (flag14)
			{
				this._successRateBackImage = Object.Instantiate<CImage>(CombatSkillBreakGridView.SrcRefers.CGet<CImage>("SuccessRateImage"), CombatSkillBreakGridView.ImageRoot);
			}
			this._successRateBackImage.transform.SetParent(CombatSkillBreakGridView.ImageRoot, false);
			this._successRateBackImage.transform.SetAsLastSibling();
			this._successRateBackImage.transform.position = base.transform.position;
			Vector3 pos3 = this._successRateBackImage.transform.localPosition + this.SuccessRateOffset;
			this._successRateBackImage.transform.localPosition = pos3;
			bool flag15 = null == this._successRateText;
			if (flag15)
			{
				this._successRateText = Object.Instantiate<TextMeshProUGUI>(CombatSkillBreakGridView.SrcRefers.CGet<TextMeshProUGUI>("SuccessRate"), CombatSkillBreakGridView.TextRoot);
			}
			this._successRateText.transform.SetParent(CombatSkillBreakGridView.TextRoot, false);
			this._successRateText.transform.position = base.transform.position;
			pos3 = this._successRateText.transform.localPosition + this.SuccessRateOffset;
			this._successRateText.transform.localPosition = pos3;
			this._successRateText.text = ((this.GridData.TemplateId == 0 || this.GridData.TemplateId == 1) ? "" : string.Format("{0}%", CombatSkillBreakGridView.GetFinaleSuccessRateFunc(this.Row, this.Column)));
		}
		string canSelectObjKey = (!CombatSkillBreakGridView.BreakHasResult && this.GridData.State == ESkillBreakGridState.CanSelect) ? "CanSelect" : string.Empty;
		bool flag16 = !string.IsNullOrEmpty(canSelectObjKey);
		if (flag16)
		{
			bool flag17 = null == this._canSelectObject;
			if (flag17)
			{
				this._canSelectObject = Object.Instantiate<GameObject>(CombatSkillBreakGridView.SrcRefers.CGet<GameObject>(canSelectObjKey), CombatSkillBreakGridView.ImageRoot);
			}
			CombatSkillBreakGridView.AddSelectTweenImage(this._canSelectObject.GetComponentInChildren<CImage>());
			this._canSelectObject.transform.SetParent(CombatSkillBreakGridView.ImageRoot, false);
			this._canSelectObject.transform.position = base.transform.position;
			this._canSelectObject.transform.SetAsLastSibling();
		}
		else
		{
			bool flag18 = null != this._canSelectObject;
			if (flag18)
			{
				CombatSkillBreakGridView.RemoveSelectTweenImage(this._canSelectObject.GetComponentInChildren<CImage>());
				Object.Destroy(this._canSelectObject);
			}
		}
	}

	// Token: 0x04002382 RID: 9090
	public static Refers SrcRefers;

	// Token: 0x04002383 RID: 9091
	public static RectTransform InteractRoot;

	// Token: 0x04002384 RID: 9092
	public static Func<int, int, Vector2> LocationConvertFunc;

	// Token: 0x04002385 RID: 9093
	public static Action<byte, byte> OnSelectNode;

	// Token: 0x04002386 RID: 9094
	public static Func<byte, byte, int> GetFinaleSuccessRateFunc;

	// Token: 0x04002387 RID: 9095
	public static RectTransform ImageRoot;

	// Token: 0x04002388 RID: 9096
	public static RectTransform TextRoot;

	// Token: 0x04002389 RID: 9097
	public static bool BreakHasResult;

	// Token: 0x0400238E RID: 9102
	public TooltipInvoker TipDisplayer;

	// Token: 0x0400238F RID: 9103
	public string[] StartNodeStateNames;

	// Token: 0x04002390 RID: 9104
	public string[] FinishNodeStateNames;

	// Token: 0x04002391 RID: 9105
	public string[] NormalNodeStateNames;

	// Token: 0x04002392 RID: 9106
	public string[] GoldSpecialNodeStateNames;

	// Token: 0x04002393 RID: 9107
	public string[] PurpleSpecialNodeStateNames;

	// Token: 0x04002394 RID: 9108
	public Vector3 SuccessRateOffset;

	// Token: 0x04002395 RID: 9109
	public Vector3 StartOrFinishNodeNameOffset;

	// Token: 0x04002396 RID: 9110
	public Vector3 StartOrFinishNodeDescOffset;

	// Token: 0x04002397 RID: 9111
	private CImage _showingImage;

	// Token: 0x04002398 RID: 9112
	private CImage _toHighLightImage;

	// Token: 0x04002399 RID: 9113
	private TextMeshProUGUI _showingNameText;

	// Token: 0x0400239A RID: 9114
	private TextMeshProUGUI _showingDescText;

	// Token: 0x0400239B RID: 9115
	private CImage _successRateBackImage;

	// Token: 0x0400239C RID: 9116
	private TextMeshProUGUI _successRateText;

	// Token: 0x0400239D RID: 9117
	private GameObject _canSelectObject;

	// Token: 0x0400239E RID: 9118
	private static Tweener _canSelectImageTweener;

	// Token: 0x0400239F RID: 9119
	private static List<CImage> _tweenTargetList;
}
