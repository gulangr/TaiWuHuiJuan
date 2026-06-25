using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.CombatSkill;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001EB RID: 491
public class CombatSkillView : Refers
{
	// Token: 0x1700033D RID: 829
	// (get) Token: 0x06002029 RID: 8233 RVA: 0x000EA505 File Offset: 0x000E8705
	// (set) Token: 0x0600202A RID: 8234 RVA: 0x000EA512 File Offset: 0x000E8712
	public Action<CombatSkillDisplayData> OnMasteredStatusChanged
	{
		get
		{
			return this._masteredHelper.OnMasteredStatusChanged;
		}
		set
		{
			this._masteredHelper.OnMasteredStatusChanged = value;
		}
	}

	// Token: 0x0600202B RID: 8235 RVA: 0x000EA520 File Offset: 0x000E8720
	private void Awake()
	{
		PointerTrigger trigger = base.CGet<PointerTrigger>("CombatSkillView_PointerTrigger");
		trigger.EnterEvent.AddListener(new UnityAction(this.OnMouseEnter));
		trigger.ExitEvent.AddListener(new UnityAction(this.OnMouseExit));
	}

	// Token: 0x0600202C RID: 8236 RVA: 0x000EA56C File Offset: 0x000E876C
	public void SetData(CombatSkillDisplayData skillData, bool interactable = true, bool locked = false, bool resetCheckState = true, bool isShowNeiLiFinish = false)
	{
		this._masteredHelper.SetData(skillData);
		bool isEmptyItem = skillData == null;
		this.UpdateMouseTip();
		bool flag = this.Names.Contains("Empty") && this.Names.Contains("ElementsRoot");
		if (flag)
		{
			base.CGet<GameObject>("Empty").SetActive(isEmptyItem);
			base.CGet<GameObject>("ElementsRoot").SetActive(!isEmptyItem);
			bool flag2 = this.Names.Contains("NameBack");
			if (flag2)
			{
				base.CGet<RectTransform>("NameBack").gameObject.SetActive(!isEmptyItem);
			}
			base.CGet<CImage>("GradeBack").gameObject.SetActive(!isEmptyItem);
			base.CGet<CImage>("CombatSkillView_CImage").SetSprite(this.BackImgNormal, false, null);
			bool flag3 = isEmptyItem;
			if (flag3)
			{
				return;
			}
		}
		bool flag4 = this.Names.Contains("FinishIcon");
		if (flag4)
		{
			base.CGet<GameObject>("FinishIcon").SetActive(skillData != null && skillData.ObtainedNeili >= skillData.MaxObtainableNeili && isShowNeiLiFinish);
		}
		CombatSkillItem configData = CombatSkill.Instance[skillData.TemplateId];
		base.CGet<TextMeshProUGUI>("Name").text = configData.Name.SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
		base.CGet<CImage>("FiveElementsType").enabled = false;
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("Grade").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		base.CGet<CImage>("SectType").SetSprite(CombatSkillView.EquipTypeImg[configData.EquipType], true, null);
		base.CGet<CImage>("SkillType").SetSprite(configData.Icon, true, null);
		base.CGet<CImage>("SkillType").SetColor(Colors.Instance.FiveElementsColors[(int)configData.FiveElements]);
		base.CGet<GameObject>("Revoked").SetActive(skillData.Revoked);
		ushort readingState = skillData.ReadingState;
		ushort activeState = skillData.ActivationState;
		int outlineCounter = 0;
		int directCounter = 0;
		int reverseCounter = 0;
		for (sbyte type = 0; type < 5; type += 1)
		{
			bool flag5 = CombatSkillStateHelper.IsPageRead(readingState, CombatSkillStateHelper.GetOutlinePageInternalIndex(type));
			if (flag5)
			{
				outlineCounter++;
			}
		}
		for (byte page = 1; page <= 5; page += 1)
		{
			bool flag6 = CombatSkillStateHelper.IsPageRead(readingState, CombatSkillStateHelper.GetNormalPageInternalIndex(0, page));
			if (flag6)
			{
				directCounter++;
			}
			bool flag7 = CombatSkillStateHelper.IsPageRead(readingState, CombatSkillStateHelper.GetNormalPageInternalIndex(1, page));
			if (flag7)
			{
				reverseCounter++;
			}
		}
		bool brokenOut = CombatSkillStateHelper.IsBrokenOut(activeState);
		RectTransform breakPageHolder = base.CGet<RectTransform>("BreakPageHolder");
		breakPageHolder.gameObject.SetActive(brokenOut);
		this.SetReadStateHolder(!brokenOut);
		bool flag8 = brokenOut;
		if (flag8)
		{
			for (byte page2 = 1; page2 <= 5; page2 += 1)
			{
				sbyte pageActiveDirection = CombatSkillStateHelper.GetPageActiveDirection(activeState, page2);
				breakPageHolder.GetChild((int)page2).GetComponent<CImage>().SetSprite(CombatSkillView.BrokenPageIcon.CheckIndex((int)pageActiveDirection) ? CombatSkillView.BrokenPageIcon[(int)pageActiveDirection] : "charactermenu3_20_zhuzi_0", false, null);
			}
			RectTransform breakBonusHolder = base.CGet<RectTransform>("BreakBonusHolder");
			breakBonusHolder.gameObject.SetActive(true);
			SkillBreakPlateItem skillBreakPlate = configData.SkillBreakPlate;
			int totalBonus = skillBreakPlate.BonusCount;
			List<sbyte> breakBonusGrades = skillData.BreakBonusGrades;
			if (breakBonusGrades != null)
			{
				breakBonusGrades.Sort((sbyte x, sbyte y) => y.CompareTo(x));
			}
			for (int i = 0; i < totalBonus; i++)
			{
				bool flag9 = this._instantiatedBonusRefersList.Count <= i;
				if (flag9)
				{
					this.AddBonusRefers();
				}
				this.ShowBonusSlot(this._instantiatedBonusRefersList[i], (int)((skillData.BreakBonusGrades == null || skillData.BreakBonusGrades.Count <= i) ? 0 : (skillData.BreakBonusGrades[i] + 1)));
			}
			for (int j = totalBonus; j < this._instantiatedBonusRefersList.Count; j++)
			{
				this._instantiatedBonusRefersList[j].gameObject.SetActive(false);
			}
		}
		else
		{
			RectTransform readStateHolder = base.CGet<RectTransform>("ReadStateHolder");
			bool page0Read = false;
			for (sbyte behaviorType = 0; behaviorType < 5; behaviorType += 1)
			{
				bool flag10 = CombatSkillStateHelper.IsPageRead(readingState, CombatSkillStateHelper.GetOutlinePageInternalIndex(behaviorType));
				if (flag10)
				{
					page0Read = true;
					break;
				}
			}
			readStateHolder.GetChild(0).GetComponent<CImage>().SetSprite(page0Read ? "charactermenu3_19_page_0" : "charactermenu3_19_page_1", false, null);
			for (byte page3 = 1; page3 <= 5; page3 += 1)
			{
				bool isDirectRead = CombatSkillStateHelper.IsPageRead(readingState, CombatSkillStateHelper.GetNormalPageInternalIndex(0, page3));
				bool isReversRead = CombatSkillStateHelper.IsPageRead(readingState, CombatSkillStateHelper.GetNormalPageInternalIndex(1, page3));
				bool isRead = isDirectRead || isReversRead;
				readStateHolder.GetChild((int)page3).GetComponent<CImage>().SetSprite(isRead ? "charactermenu3_19_page_0" : "charactermenu3_19_page_1", false, null);
			}
			RectTransform breakBonusHolder2 = base.CGet<RectTransform>("BreakBonusHolder");
			breakBonusHolder2.gameObject.SetActive(false);
		}
		this._interactable = interactable;
		CButtonObsolete btn = base.CGet<CButtonObsolete>("CombatSkillView_CButton");
		bool flag11 = btn != null;
		if (flag11)
		{
			btn.interactable = interactable;
		}
		this._locked = locked;
		this.SetLock(locked);
		if (resetCheckState)
		{
			this.SetChecked(false);
		}
		base.CGet<GameObject>("MouseOver").gameObject.SetActive(false);
		this.SetDisableStyle(locked || skillData.Revoked);
		bool flag12 = this.autoUpdateMastered;
		if (flag12)
		{
			this.UpdateMasteredStatus(false);
		}
	}

	// Token: 0x0600202D RID: 8237 RVA: 0x000EAB70 File Offset: 0x000E8D70
	private void ShowBonusSlot(Refers instantiatedBonusRefers, int skillDataBreakBonusGrade)
	{
		Texture2D slotIcon = instantiatedBonusRefers.CGet<Texture2D>(string.Format("bonusSlot{0}", skillDataBreakBonusGrade));
		instantiatedBonusRefers.CGet<CRawImage>("bonusSlotImgComponent").texture = slotIcon;
		instantiatedBonusRefers.gameObject.SetActive(true);
	}

	// Token: 0x0600202E RID: 8238 RVA: 0x000EABB4 File Offset: 0x000E8DB4
	private void AddBonusRefers()
	{
		Refers bonusPrefab = base.CGet<Refers>("breakBonusPrefab");
		Refers bonusInstance = Object.Instantiate<Refers>(bonusPrefab, base.CGet<RectTransform>("BreakBonusHolder").transform);
		this._instantiatedBonusRefersList.Add(bonusInstance);
	}

	// Token: 0x0600202F RID: 8239 RVA: 0x000EABF4 File Offset: 0x000E8DF4
	public void UpdateMouseTip()
	{
		CombatSkillDisplayData skillData = this._masteredHelper.DisplayData;
		TooltipInvoker mouseTip = base.CGet<TooltipInvoker>("CombatSkillView_MouseTipDisplayer");
		mouseTip.enabled = (skillData != null);
		bool flag = skillData == null;
		if (!flag)
		{
			mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CombatSkillId", skillData.TemplateId).Set("CharId", skillData.CharId);
			bool flag2 = this.isCombatSkillPracticeTip;
			if (flag2)
			{
				mouseTip.RuntimeParam.Set("NeedExp", skillData.NewUnderstandingNeedExp).SetObject("CannotPracticeReasons", this.cannotPracticeReasons);
			}
			bool showing = mouseTip.Showing;
			if (showing)
			{
				mouseTip.Refresh(false, -1);
			}
		}
	}

	// Token: 0x06002030 RID: 8240 RVA: 0x000EACA4 File Offset: 0x000E8EA4
	public void RefreshMouseTip()
	{
		TooltipInvoker mouseTip = base.CGet<TooltipInvoker>("CombatSkillView_MouseTipDisplayer");
		mouseTip.Refresh(false, -1);
	}

	// Token: 0x06002031 RID: 8241 RVA: 0x000EACC8 File Offset: 0x000E8EC8
	public void SetClickEvent(UnityAction onClick)
	{
		CButtonObsolete btn = base.CGet<CButtonObsolete>("CombatSkillView_CButton");
		btn.onClick.RemoveAllListeners();
		bool flag = onClick != null;
		if (flag)
		{
			btn.onClick.AddListener(onClick);
		}
	}

	// Token: 0x06002032 RID: 8242 RVA: 0x000EAD04 File Offset: 0x000E8F04
	public void SetEnterEvent(UnityAction onEnter)
	{
		PointerTrigger trigger = base.CGet<PointerTrigger>("CombatSkillView_PointerTrigger");
		trigger.EnterEvent.RemoveAllListeners();
		trigger.EnterEvent.AddListener(new UnityAction(this.OnMouseEnter));
		trigger.EnterEvent.AddListener(onEnter);
	}

	// Token: 0x06002033 RID: 8243 RVA: 0x000EAD50 File Offset: 0x000E8F50
	public void SetExitEvent(UnityAction onExit)
	{
		PointerTrigger trigger = base.CGet<PointerTrigger>("CombatSkillView_PointerTrigger");
		trigger.ExitEvent.RemoveAllListeners();
		trigger.ExitEvent.AddListener(new UnityAction(this.OnMouseExit));
		trigger.ExitEvent.AddListener(onExit);
	}

	// Token: 0x06002034 RID: 8244 RVA: 0x000EAD9C File Offset: 0x000E8F9C
	public void SetInactiveMasterEnterEvent(UnityAction onEnter)
	{
		GameObject master = base.CGet<GameObject>("Master");
		Transform masterInactive = master.transform.GetChild(0);
		bool activeSelf = masterInactive.gameObject.activeSelf;
		if (activeSelf)
		{
			PointerTrigger masterInactivePointerTrigger = masterInactive.GetComponent<PointerTrigger>();
			masterInactivePointerTrigger.EnterEvent.RemoveAllListeners();
			masterInactivePointerTrigger.EnterEvent.AddListener(onEnter);
		}
	}

	// Token: 0x06002035 RID: 8245 RVA: 0x000EADF4 File Offset: 0x000E8FF4
	public void SetActiveMasterEnterEvent(UnityAction onEnter)
	{
		GameObject master = base.CGet<GameObject>("Master");
		Transform masterActive = master.transform.GetChild(1);
		bool activeSelf = masterActive.gameObject.activeSelf;
		if (activeSelf)
		{
			PointerTrigger masterActivePointerTrigger = masterActive.GetComponent<PointerTrigger>();
			masterActivePointerTrigger.EnterEvent.RemoveAllListeners();
			masterActivePointerTrigger.EnterEvent.AddListener(onEnter);
		}
	}

	// Token: 0x06002036 RID: 8246 RVA: 0x000EAE4C File Offset: 0x000E904C
	public void SetInactiveMasterExitEvent(UnityAction onExit)
	{
		GameObject master = base.CGet<GameObject>("Master");
		Transform masterInactive = master.transform.GetChild(0);
		bool activeSelf = masterInactive.gameObject.activeSelf;
		if (activeSelf)
		{
			PointerTrigger masterInactivePointerTrigger = masterInactive.GetComponent<PointerTrigger>();
			masterInactivePointerTrigger.ExitEvent.RemoveAllListeners();
			masterInactivePointerTrigger.ExitEvent.AddListener(onExit);
		}
	}

	// Token: 0x06002037 RID: 8247 RVA: 0x000EAEA4 File Offset: 0x000E90A4
	public void SetActiveMasterExitEvent(UnityAction onExit)
	{
		GameObject master = base.CGet<GameObject>("Master");
		Transform masterActive = master.transform.GetChild(1);
		bool activeSelf = masterActive.gameObject.activeSelf;
		if (activeSelf)
		{
			PointerTrigger masterActivePointerTrigger = masterActive.GetComponent<PointerTrigger>();
			masterActivePointerTrigger.ExitEvent.RemoveAllListeners();
			masterActivePointerTrigger.ExitEvent.AddListener(onExit);
		}
	}

	// Token: 0x06002038 RID: 8248 RVA: 0x000EAEFC File Offset: 0x000E90FC
	public void SetLock(bool locked)
	{
		base.CGet<GameObject>("Lock").SetActive(locked);
	}

	// Token: 0x06002039 RID: 8249 RVA: 0x000EAF14 File Offset: 0x000E9114
	public void SetDisableMask(bool disabled)
	{
		GameObject disableMask;
		bool flag = this.CTryGet<GameObject>("DisableMask", out disableMask);
		if (flag)
		{
			disableMask.SetActive(disabled);
		}
	}

	// Token: 0x0600203A RID: 8250 RVA: 0x000EAF3B File Offset: 0x000E913B
	public void SetChecked(bool check)
	{
		base.CGet<GameObject>("CheckMark").SetActive(check);
	}

	// Token: 0x0600203B RID: 8251 RVA: 0x000EAF50 File Offset: 0x000E9150
	public void SetInteractable(bool interactable)
	{
		CButtonObsolete btn = base.CGet<CButtonObsolete>("CombatSkillView_CButton");
		this._interactable = interactable;
		bool flag = btn != null;
		if (flag)
		{
			btn.interactable = interactable;
		}
		bool flag2 = !interactable;
		if (flag2)
		{
			base.CGet<GameObject>("MouseOver").gameObject.SetActive(false);
		}
	}

	// Token: 0x0600203C RID: 8252 RVA: 0x000EAFA4 File Offset: 0x000E91A4
	public void FitWidth(int gridCount)
	{
		CImage skillBack = base.CGet<CImage>("CombatSkillView_CImage");
		skillBack.SetAlpha(CombatSkillView.SkillBackAlpha[gridCount - 1]);
		Vector2 anchoredPos = new Vector2(-6.5f, -7.5f);
		bool flag = gridCount == 2;
		if (flag)
		{
			anchoredPos = new Vector2(-13.5f, -7.5f);
		}
		else
		{
			bool flag2 = gridCount == 3;
			if (flag2)
			{
				anchoredPos = new Vector2(-21.5f, -7.5f);
			}
		}
		base.CGet<RectTransform>("BreakBonusHolder").anchoredPosition = anchoredPos;
	}

	// Token: 0x0600203D RID: 8253 RVA: 0x000EB029 File Offset: 0x000E9229
	public void SetDisableStyle(bool useDisableStyle)
	{
		base.CGet<DisableStyleRoot>("GrayRoot").SetStyleEffect(useDisableStyle, false);
	}

	// Token: 0x0600203E RID: 8254 RVA: 0x000EB040 File Offset: 0x000E9240
	public void OnMouseEnter()
	{
		bool flag = this._interactable && !base.CGet<GameObject>("CheckMark").activeSelf;
		if (flag)
		{
			base.CGet<GameObject>("MouseOver").gameObject.SetActive(true);
		}
	}

	// Token: 0x0600203F RID: 8255 RVA: 0x000EB088 File Offset: 0x000E9288
	public void OnMouseExit()
	{
		bool interactable = this._interactable;
		if (interactable)
		{
			base.CGet<GameObject>("MouseOver").gameObject.SetActive(false);
		}
	}

	// Token: 0x06002040 RID: 8256 RVA: 0x000EB0B8 File Offset: 0x000E92B8
	public void UpdateMasteredStatus(bool alsoUpdateStatus = false)
	{
		bool flag = this._masteredHelper.DisplayData == null;
		if (!flag)
		{
			base.CGet<TextMeshProUGUI>("GridCount").text = this._masteredHelper.DisplayData.GridCount.ToString();
			base.CGet<CImage>("CombatSkillView_CImage").SetSprite(this._masteredHelper.DisplayData.Mastered ? this.BackImgMastered : this.BackImgNormal, false, null);
			bool flag2 = !alsoUpdateStatus;
			if (!flag2)
			{
				CButtonObsolete activeStatus = base.CGet<CButtonObsolete>("MasterActiveStatus");
				bool flag3 = activeStatus.gameObject.activeSelf != this._masteredHelper.DisplayData.Mastered;
				if (flag3)
				{
					activeStatus.gameObject.SetActive(this._masteredHelper.DisplayData.Mastered);
				}
				CButtonObsolete inactiveStatus = base.CGet<CButtonObsolete>("MasterInactiveStatus");
				bool flag4 = inactiveStatus.gameObject.activeSelf == this._masteredHelper.DisplayData.Mastered;
				if (flag4)
				{
					inactiveStatus.gameObject.SetActive(!this._masteredHelper.DisplayData.Mastered);
				}
			}
		}
	}

	// Token: 0x06002041 RID: 8257 RVA: 0x000EB1E4 File Offset: 0x000E93E4
	public void ChangeMasteredInteractable(bool interactable)
	{
		bool flag = this._masteredHelper.DisplayData == null;
		if (flag)
		{
			AdaptableLog.Warning("Cannot call CombatSkillView.ChangeMasteredInteractable until SetData", false);
		}
		else
		{
			bool active = interactable && this._masteredHelper.CanOperate;
			GameObject master = base.CGet<GameObject>("Master");
			bool flag2 = master.activeSelf != active;
			if (flag2)
			{
				master.SetActive(active);
			}
			bool flag3 = !active;
			if (!flag3)
			{
				bool flag4 = !this._masteredHelper.DisplayData.PreviewMastered;
				if (flag4)
				{
					this.UpdateMasteredStatus(true);
				}
				CButtonObsolete btnActiveStatus = base.CGet<CButtonObsolete>("MasterActiveStatus");
				CButtonObsolete btnInactiveStatus = base.CGet<CButtonObsolete>("MasterInactiveStatus");
				btnActiveStatus.ClearAndAddListener(delegate
				{
					UI_CharacterMenuEquipCombatSkill.IsChangingMasterState = true;
					this._masteredHelper.OnClickChangeMastered();
				});
				btnInactiveStatus.ClearAndAddListener(delegate
				{
					UI_CharacterMenuEquipCombatSkill.IsChangingMasterState = true;
					this._masteredHelper.OnClickChangeMastered();
				});
			}
		}
	}

	// Token: 0x06002042 RID: 8258 RVA: 0x000EB2BC File Offset: 0x000E94BC
	public CButtonObsolete GetButton()
	{
		return base.CGet<CButtonObsolete>("CombatSkillView_CButton");
	}

	// Token: 0x06002043 RID: 8259 RVA: 0x000EB2DC File Offset: 0x000E94DC
	public TooltipInvoker GetMouseTipDisplay()
	{
		return base.CGet<TooltipInvoker>("CombatSkillView_MouseTipDisplayer");
	}

	// Token: 0x06002044 RID: 8260 RVA: 0x000EB2F9 File Offset: 0x000E94F9
	public void SetReadStateHolder(bool isShow)
	{
		base.CGet<RectTransform>("ReadStateHolder").gameObject.SetActive(isShow);
	}

	// Token: 0x06002046 RID: 8262 RVA: 0x000EB364 File Offset: 0x000E9564
	// Note: this type is marked as 'beforefieldinit'.
	static CombatSkillView()
	{
		float[] array = new float[3];
		array[0] = 1f;
		CombatSkillView.SkillBackAlpha = array;
	}

	// Token: 0x0400183E RID: 6206
	public static readonly string[] FiveElementImg = new string[]
	{
		"charactermenu3_19_neili_2",
		"charactermenu3_19_neili_3",
		"charactermenu3_19_neili_1",
		"charactermenu3_19_neili_0",
		"charactermenu3_19_neili_4",
		"charactermenu3_19_neili_5"
	};

	// Token: 0x0400183F RID: 6207
	public static readonly string[] SectImg = new string[]
	{
		"charactermenu3_19_menpai_0",
		"charactermenu3_19_menpai_1",
		"charactermenu3_19_menpai_2",
		"charactermenu3_19_menpai_3",
		"charactermenu3_19_menpai_4",
		"charactermenu3_19_menpai_5",
		"charactermenu3_19_menpai_6",
		"charactermenu3_19_menpai_7",
		"charactermenu3_19_menpai_8",
		"charactermenu3_19_menpai_9",
		"charactermenu3_19_menpai_10",
		"charactermenu3_19_menpai_11",
		"charactermenu3_19_menpai_12",
		"charactermenu3_19_menpai_13",
		"charactermenu3_19_menpai_14",
		"charactermenu3_19_menpai_15"
	};

	// Token: 0x04001840 RID: 6208
	public static readonly Dictionary<sbyte, string> EquipTypeImg = new Dictionary<sbyte, string>
	{
		{
			2,
			"sp_combatskillback_agile"
		},
		{
			1,
			"sp_combatskillback_attack"
		},
		{
			3,
			"sp_combatskillback_defense"
		},
		{
			4,
			"sp_combatskillback_others"
		},
		{
			0,
			"sp_combatskillback_others"
		}
	};

	// Token: 0x04001841 RID: 6209
	public static readonly string[] BrokenPageIcon = new string[]
	{
		"charactermenu3_20_zhuzi_1",
		"charactermenu3_20_zhuzi_2"
	};

	// Token: 0x04001842 RID: 6210
	private static readonly float[] SkillBackAlpha;

	// Token: 0x04001843 RID: 6211
	private readonly string BackImgMastered = "charactermenu3_23_jingtongge_0";

	// Token: 0x04001844 RID: 6212
	private readonly string BackImgNormal = "charactermenu3_19_shuben";

	// Token: 0x04001845 RID: 6213
	public bool autoUpdateMastered = true;

	// Token: 0x04001846 RID: 6214
	public bool isCombatSkillPracticeTip = false;

	// Token: 0x04001847 RID: 6215
	public List<bool> cannotPracticeReasons;

	// Token: 0x04001848 RID: 6216
	private readonly CombatSkillViewMasteredHelper _masteredHelper = new CombatSkillViewMasteredHelper();

	// Token: 0x04001849 RID: 6217
	private bool _interactable;

	// Token: 0x0400184A RID: 6218
	private bool _locked;

	// Token: 0x0400184B RID: 6219
	private List<Refers> _instantiatedBonusRefersList = new List<Refers>();
}
