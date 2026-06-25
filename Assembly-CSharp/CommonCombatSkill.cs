using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.CombatSkill;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001EE RID: 494
[ExecuteInEditMode]
public class CommonCombatSkill : CommonToggleState
{
	// Token: 0x17000340 RID: 832
	// (get) Token: 0x06002056 RID: 8278 RVA: 0x000EB912 File Offset: 0x000E9B12
	// (set) Token: 0x06002057 RID: 8279 RVA: 0x000EB91A File Offset: 0x000E9B1A
	public int Slot
	{
		get
		{
			return this.slot;
		}
		set
		{
			this.slot = value;
			this.RefreshWidth();
		}
	}

	// Token: 0x06002058 RID: 8280 RVA: 0x000EB92C File Offset: 0x000E9B2C
	private void Update()
	{
		bool flag = !Application.isPlaying;
		if (flag)
		{
			this.RefreshWidth();
		}
	}

	// Token: 0x06002059 RID: 8281 RVA: 0x000EB950 File Offset: 0x000E9B50
	public void Refresh(CombatSkillDisplayData skillData)
	{
		bool flag = skillData == null;
		if (!flag)
		{
			this._data = skillData;
			CombatSkillItem config = CombatSkill.Instance[skillData.TemplateId];
			string equipType = config.EquipType.ToString();
			string fiveElementType = CommonCombatSkill.FiveElementTextureSuffix[(int)config.FiveElements].ToString();
			string sectId = config.SectId.ToString();
			string powerText = skillData.Power.ToString();
			this.UpdateMouseTip();
			this.RefreshBonusTip();
			this.UpdateActionMenuButton();
			this.SetMastered(skillData.Mastered, (int)config.Grade);
			this.SetRevoked(skillData.Revoked);
			sbyte gradeIndex = config.Grade;
			bool mastered = skillData.Mastered;
			if (mastered)
			{
				this.normalGradeIcon.sprite = this.normalMasterGradeSprites[(int)gradeIndex];
				this.hoverGradeIcon.sprite = this.hoverMasterGradeSprites[(int)gradeIndex];
			}
			else
			{
				this.normalGradeIcon.sprite = this.normalGradeSprites[(int)gradeIndex];
				this.hoverGradeIcon.sprite = this.hoverGradeSprites[(int)gradeIndex];
			}
			this.icon.SetSprite(config.Icon, false, null);
			this.frame.SetSprite("ui_sp_base_combatskill_icon_{0}_{1}".GetFormat(equipType, fiveElementType), false, null);
			this.sect.SetSprite("ui_sp_graphics_combatskill_sect_{0}".GetFormat(sectId), false, null);
			this.mask.sprite = this.maskSprites[(int)config.EquipType];
			this.skillName.text = config.Name.SetGradeColor((int)config.Grade);
			this.power.text = "{0}%".GetFormat(powerText).SetColor((skillData.RequirementsPower < skillData.MaxPower) ? "orange" : "lightblue");
			this.pageStates.Refresh(skillData);
			int bonusCount = config.SkillBreakPlate.BonusCount;
			int objCount = this.bonuses.childCount;
			List<sbyte> breakBonusGrades = skillData.BreakBonusGrades;
			int actualCount = (breakBonusGrades != null) ? breakBonusGrades.Count : 0;
			bool flag2 = this._cachedBonusImages == null || this._cachedBonusImages.Length != objCount;
			if (flag2)
			{
				this._cachedBonusImages = new CImage[objCount];
				for (int i = 0; i < objCount; i++)
				{
					this._cachedBonusImages[i] = this.bonuses.GetChild(i).GetComponent<CImage>();
				}
			}
			List<sbyte> breakBonusGrades2 = skillData.BreakBonusGrades;
			if (breakBonusGrades2 != null)
			{
				breakBonusGrades2.Sort((sbyte x, sbyte y) => y.CompareTo(x));
			}
			for (int j = 0; j < objCount; j++)
			{
				int reverseIndex = objCount - j - 1;
				bool flag3 = j >= bonusCount;
				if (flag3)
				{
					this._cachedBonusImages[reverseIndex].gameObject.SetActive(false);
				}
				else
				{
					int bonusGrade = (int)((actualCount > j && skillData.BreakBonusGrades != null) ? (skillData.BreakBonusGrades[j] + 1) : 0);
					this._cachedBonusImages[reverseIndex].sprite = this.bonusSprites[bonusGrade];
					this._cachedBonusImages[reverseIndex].gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x0600205A RID: 8282 RVA: 0x000EBC84 File Offset: 0x000E9E84
	public void RefreshWidth()
	{
		base.GetComponent<RectTransform>().SetWidth((float)(186 * this.slot));
	}

	// Token: 0x0600205B RID: 8283 RVA: 0x000EBCA0 File Offset: 0x000E9EA0
	public void SetMastered(bool mastered, int grade)
	{
		if (mastered)
		{
			this.normalBgIcon.sprite = this.normalMasterBgSprite;
			this.hoverBgIcon.sprite = this.hoverMasterBgSprite;
			this.normalGradeIcon.sprite = this.normalMasterGradeSprites[grade];
			this.hoverGradeIcon.sprite = this.hoverMasterGradeSprites[grade];
		}
		else
		{
			this.normalBgIcon.sprite = this.normalBgSprite;
			this.hoverBgIcon.sprite = this.hoverBgSprite;
			this.normalGradeIcon.sprite = this.normalGradeSprites[grade];
			this.hoverGradeIcon.sprite = this.hoverGradeSprites[grade];
		}
	}

	// Token: 0x0600205C RID: 8284 RVA: 0x000EBD54 File Offset: 0x000E9F54
	public void SetRevoked(bool revoked)
	{
		bool flag = null == this.revokeMark;
		if (!flag)
		{
			this.revokeMark.SetActive(revoked);
		}
	}

	// Token: 0x0600205D RID: 8285 RVA: 0x000EBD84 File Offset: 0x000E9F84
	public void UpdateMouseTip()
	{
		this.mouseTip.enabled = (this._data != null);
		bool flag = this._data == null;
		if (!flag)
		{
			this.mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CombatSkillId", this._data.TemplateId).Set("CharId", this._data.CharId);
			bool showing = this.mouseTip.Showing;
			if (showing)
			{
				this.mouseTip.Refresh(false, -1);
			}
		}
	}

	// Token: 0x0600205E RID: 8286 RVA: 0x000EBE10 File Offset: 0x000EA010
	public void UpdateCounter(sbyte referenceNeiliType)
	{
		bool flag = this._data == null;
		if (!flag)
		{
			bool flag2 = referenceNeiliType < 0;
			if (!flag2)
			{
				sbyte skillFiveElementType = CombatSkill.Instance[this._data.TemplateId].FiveElements;
				NeiliTypeItem neiliTypeCfg = NeiliType.Instance[referenceNeiliType];
				bool countered = neiliTypeCfg.InjuryOnUseType == skillFiveElementType || neiliTypeCfg.MaxPowerChange[(int)skillFiveElementType] < 0;
				this.counterObject.SetActive(countered);
			}
		}
	}

	// Token: 0x0600205F RID: 8287 RVA: 0x000EBE85 File Offset: 0x000EA085
	public void UpdateCounterVisible(bool visible)
	{
		this.counterObject.SetActive(visible);
	}

	// Token: 0x06002060 RID: 8288 RVA: 0x000EBE98 File Offset: 0x000EA098
	private void RefreshBonusTip()
	{
		Behaviour behaviour = this.bonusTip;
		List<sbyte> breakBonusGrades = this._data.BreakBonusGrades;
		bool enabled;
		if (breakBonusGrades == null)
		{
			enabled = false;
		}
		else
		{
			enabled = breakBonusGrades.Any((sbyte grade) => grade >= 0);
		}
		behaviour.enabled = enabled;
		this.bonusTip.Type = TipType.CombatSkillBonus;
		TooltipInvoker tooltipInvoker = this.bonusTip;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		this.bonusTip.RuntimeParam.Set("CharId", this._data.CharId).Set("SkillId", this._data.TemplateId);
	}

	// Token: 0x06002061 RID: 8289 RVA: 0x000EBF4B File Offset: 0x000EA14B
	private void UpdateActionMenuButton()
	{
		this.actionMenuButton.ClearAndAddListener(delegate
		{
		});
	}

	// Token: 0x06002062 RID: 8290 RVA: 0x000EBF79 File Offset: 0x000EA179
	public override void InitializeStateDisplay()
	{
		base.InitializeStateDisplay();
		this.UpdateMouseTip();
	}

	// Token: 0x06002063 RID: 8291 RVA: 0x000EBF8A File Offset: 0x000EA18A
	public override void OnStateChanged()
	{
		base.OnStateChanged();
		this.hover.SetActive(base.CurrState == CommonToggleState.ToggleStates.Highlight);
	}

	// Token: 0x06002064 RID: 8292 RVA: 0x000EBFAC File Offset: 0x000EA1AC
	public void SetToggleValueChanged(UnityAction<bool> value)
	{
		bool flag = this._actionWhenToggleValueChange != null;
		if (flag)
		{
			this.toggle.onValueChanged.RemoveListener(this._actionWhenToggleValueChange);
		}
		this._actionWhenToggleValueChange = value;
		bool flag2 = this._actionWhenToggleValueChange != null;
		if (flag2)
		{
			this.toggle.onValueChanged.AddListener(this._actionWhenToggleValueChange);
		}
	}

	// Token: 0x06002065 RID: 8293 RVA: 0x000EC009 File Offset: 0x000EA209
	public void SetSelected(bool selected)
	{
		this.toggle.isOn = selected;
	}

	// Token: 0x0400184F RID: 6223
	[SerializeField]
	[Range(1f, 3f)]
	[Header("功法格子数")]
	private int slot = 1;

	// Token: 0x04001850 RID: 6224
	public CImage normalBgIcon;

	// Token: 0x04001851 RID: 6225
	public CImage hoverBgIcon;

	// Token: 0x04001852 RID: 6226
	public CImage normalGradeIcon;

	// Token: 0x04001853 RID: 6227
	public CImage hoverGradeIcon;

	// Token: 0x04001854 RID: 6228
	public CImage icon;

	// Token: 0x04001855 RID: 6229
	public CImage frame;

	// Token: 0x04001856 RID: 6230
	public CImage sect;

	// Token: 0x04001857 RID: 6231
	public CImage mask;

	// Token: 0x04001858 RID: 6232
	public Sprite normalBgSprite;

	// Token: 0x04001859 RID: 6233
	public Sprite hoverBgSprite;

	// Token: 0x0400185A RID: 6234
	public Sprite normalMasterBgSprite;

	// Token: 0x0400185B RID: 6235
	public Sprite hoverMasterBgSprite;

	// Token: 0x0400185C RID: 6236
	public TextMeshProUGUI skillName;

	// Token: 0x0400185D RID: 6237
	public TextMeshProUGUI power;

	// Token: 0x0400185E RID: 6238
	public CommonPageStates pageStates;

	// Token: 0x0400185F RID: 6239
	public Transform bonuses;

	// Token: 0x04001860 RID: 6240
	public GameObject revokeMark;

	// Token: 0x04001861 RID: 6241
	public CButtonObsolete actionMenuButton;

	// Token: 0x04001862 RID: 6242
	public GameObject counterObject;

	// Token: 0x04001863 RID: 6243
	public TooltipInvoker bonusTip;

	// Token: 0x04001864 RID: 6244
	[Header("品级图标缓存")]
	[SerializeField]
	private Sprite[] normalGradeSprites;

	// Token: 0x04001865 RID: 6245
	[SerializeField]
	private Sprite[] hoverGradeSprites;

	// Token: 0x04001866 RID: 6246
	[SerializeField]
	private Sprite[] normalMasterGradeSprites;

	// Token: 0x04001867 RID: 6247
	[SerializeField]
	private Sprite[] hoverMasterGradeSprites;

	// Token: 0x04001868 RID: 6248
	[Header("遮罩缓存")]
	[SerializeField]
	private Sprite[] maskSprites;

	// Token: 0x04001869 RID: 6249
	[Header("玄机图标缓存")]
	[SerializeField]
	private Sprite[] bonusSprites;

	// Token: 0x0400186A RID: 6250
	public static readonly int[] FiveElementTextureSuffix = new int[]
	{
		1,
		5,
		3,
		4,
		2,
		0
	};

	// Token: 0x0400186B RID: 6251
	public const string FramePrefix = "ui_sp_base_combatskill_icon_{0}_{1}";

	// Token: 0x0400186C RID: 6252
	private const string SectPrefix = "ui_sp_graphics_combatskill_sect_{0}";

	// Token: 0x0400186D RID: 6253
	public const string MaskPrefix = "ui_sp_base_combatskill_icon_{0}_mask";

	// Token: 0x0400186E RID: 6254
	private const string PowerSuffix = "{0}%";

	// Token: 0x0400186F RID: 6255
	public const string BonusPrefix = "ui_sp_icon_combatskill_profoundtheory_{0}";

	// Token: 0x04001870 RID: 6256
	public const int StandardWidth = 186;

	// Token: 0x04001871 RID: 6257
	public const int StandardHeight = 200;

	// Token: 0x04001872 RID: 6258
	private CombatSkillDisplayData _data;

	// Token: 0x04001873 RID: 6259
	private CImage[] _cachedBonusImages;

	// Token: 0x04001874 RID: 6260
	private UnityAction<bool> _actionWhenToggleValueChange;
}
