using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Coffee.UIExtensions;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Taiwu.Profession;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UILogic
{
	// Token: 0x020006AE RID: 1710
	public class ProfessionSkillItem : MonoBehaviour
	{
		// Token: 0x170009C0 RID: 2496
		// (get) Token: 0x06004FCC RID: 20428 RVA: 0x00254EC2 File Offset: 0x002530C2
		private ProfessionModel Model
		{
			get
			{
				return SingletonObject.getInstance<ProfessionModel>();
			}
		}

		// Token: 0x170009C1 RID: 2497
		// (get) Token: 0x06004FCD RID: 20429 RVA: 0x00254EC9 File Offset: 0x002530C9
		public int SkillId
		{
			get
			{
				return this._skillId;
			}
		}

		// Token: 0x06004FCE RID: 20430 RVA: 0x00254ED4 File Offset: 0x002530D4
		public void Refresh(ProfessionSkillItem.RefreshConfig config)
		{
			this._isEmpty = config.IsEmpty;
			this._style = config.Style;
			this.empty.SetActive(config.IsEmpty);
			this.content.SetActive(!config.IsEmpty);
			this.RefreshInteract(config.OnClicked, config.OnPointerEnter, config.OnPointerExit);
			this._selected = config.IsSelected;
			this._hovered = false;
			this.SetSelected(config.IsSelected);
			bool isEmpty = config.IsEmpty;
			if (isEmpty)
			{
				this._skillId = -1;
				this._hovered = false;
				this.SetSelected(false);
				this.RefreshEmptyHoverState();
				GameObject gameObject = this.pendingLearnGo;
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
			else
			{
				bool unlocked = config.ProfessionData.IsSkillUnlocked(config.SkillIndexOfProfession);
				int professionId = config.ProfessionData.TemplateId;
				ProfessionItem professionConfig = Profession.Instance[professionId];
				int skillId = (config.SkillIndexOfProfession == 3) ? professionConfig.ExtraProfessionSkill : professionConfig.ProfessionSkills[config.SkillIndexOfProfession];
				this._skillId = skillId;
				ProfessionSkillItem skillConfig = ProfessionSkill.Instance[skillId];
				this.nameLabel.text = skillConfig.Name;
				this.lockGo.SetActive(!unlocked);
				bool flag = this.pendingLearnGo != null;
				if (flag)
				{
					this.pendingLearnGo.SetActive(config.IsPendingLearn);
				}
				bool isCoolDown = config.ProfessionData.IsSkillCooldown(SingletonObject.getInstance<BasicGameData>().CurrDate, config.SkillIndexOfProfession);
				this.skillIcon.SetSprite(skillConfig.Icon, false, null);
				this.skillIcon.SetColor(Color.white);
				this.RefreshBackgroundSprites(skillConfig.Type);
				this._baseBrightness = (config.IsPendingLearn ? 0.6f : (unlocked ? 1f : 0.3f));
				this.ApplyItemBrightness(this._baseBrightness);
				this.RefreshCd(config.ProfessionData, config.SkillIndexOfProfession, isCoolDown);
			}
		}

		// Token: 0x06004FCF RID: 20431 RVA: 0x002550DC File Offset: 0x002532DC
		public void SetTemplate(int professionId, int skillIndex)
		{
			ProfessionItem professionConfig = Profession.Instance[professionId];
			int skillId = (skillIndex == 3) ? professionConfig.ExtraProfessionSkill : professionConfig.ProfessionSkills[skillIndex];
			ProfessionSkillItem config = ProfessionSkill.Instance[skillId];
			this.nameLabel.text = config.Name;
			this.skillIcon.SetSprite(config.Icon, false, null);
			this.skillIcon.SetColor(Color.white);
			this.lockGo.SetActive(false);
			this.empty.SetActive(false);
			this.content.SetActive(true);
			this.coolDown.SetActive(false);
			GameObject gameObject = this.pendingLearnGo;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			this.RefreshBackgroundSprites(config.Type);
			this._selected = false;
			this._hovered = false;
			this.interact.enabled = false;
			this.interact.GetComponent<PointerTrigger>().enabled = false;
			this.SetSelected(false);
			this.HideHover();
			ResourceMonitor monitor = SingletonObject.getInstance<WorldMapModel>().TaiwuResources;
			ResourceInts resource = new ResourceInts(monitor.Resources);
			int exp = monitor.Exp;
			this.RefreshSkillItemTips(true, professionId, this.Model.GetProfessionData(professionId), skillId, skillIndex, exp, resource);
		}

		// Token: 0x06004FD0 RID: 20432 RVA: 0x0025521C File Offset: 0x0025341C
		private void RefreshInteract(Action onClicked, UnityAction onPointerEnter, UnityAction onPointerExit)
		{
			bool flag = onClicked != null;
			if (flag)
			{
				this.interact.ClearAndAddListener(onClicked);
			}
			PointerTrigger pointerTrigger = this.interact.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			bool flag2 = onPointerEnter != null;
			if (flag2)
			{
				pointerTrigger.EnterEvent.AddListener(onPointerEnter);
			}
			pointerTrigger.ExitEvent.RemoveAllListeners();
			bool flag3 = onPointerExit != null;
			if (flag3)
			{
				pointerTrigger.ExitEvent.AddListener(onPointerExit);
			}
		}

		// Token: 0x06004FD1 RID: 20433 RVA: 0x0025528D File Offset: 0x0025348D
		public void SetSkillIconGray(bool gray)
		{
			this.ApplyItemBrightness(gray ? Mathf.Min(this._baseBrightness, 0.5f) : this._baseBrightness);
		}

		// Token: 0x06004FD2 RID: 20434 RVA: 0x002552B4 File Offset: 0x002534B4
		public void SetSelected(bool selected)
		{
			this._selected = selected;
			if (selected)
			{
				this._hovered = false;
			}
			bool isEmpty = this._isEmpty;
			if (isEmpty)
			{
				this.RefreshEmptyHoverState();
			}
			else
			{
				this.RefreshBackgroundState();
				this.RefreshHoverNode();
			}
		}

		// Token: 0x06004FD3 RID: 20435 RVA: 0x002552FC File Offset: 0x002534FC
		public void ShowHover()
		{
			bool selected = this._selected;
			if (selected)
			{
				this._hovered = true;
				bool isEmpty = this._isEmpty;
				if (isEmpty)
				{
					this.RefreshEmptyHoverState();
				}
				else
				{
					this.RefreshHoverNode();
				}
			}
			else
			{
				this._hovered = true;
				bool isEmpty2 = this._isEmpty;
				if (isEmpty2)
				{
					this.RefreshEmptyHoverState();
				}
				else
				{
					this.RefreshBackgroundState();
				}
			}
		}

		// Token: 0x06004FD4 RID: 20436 RVA: 0x0025535C File Offset: 0x0025355C
		public void HideHover()
		{
			bool selected = this._selected;
			if (selected)
			{
				this._hovered = false;
				bool isEmpty = this._isEmpty;
				if (isEmpty)
				{
					this.RefreshEmptyHoverState();
				}
				else
				{
					this.RefreshHoverNode();
				}
			}
			else
			{
				this._hovered = false;
				bool isEmpty2 = this._isEmpty;
				if (isEmpty2)
				{
					this.RefreshEmptyHoverState();
				}
				else
				{
					this.RefreshBackgroundState();
				}
			}
		}

		// Token: 0x06004FD5 RID: 20437 RVA: 0x002553BC File Offset: 0x002535BC
		private void RefreshEmptyHoverState()
		{
			bool flag = this.hover != null;
			if (flag)
			{
				this.hover.gameObject.SetActive(this._isEmpty && this._hovered && !this._selected);
			}
		}

		// Token: 0x06004FD6 RID: 20438 RVA: 0x0025540C File Offset: 0x0025360C
		private void RefreshHoverNode()
		{
			bool flag = this.hover != null;
			if (flag)
			{
				this.hover.gameObject.SetActive(this._selected && this._hovered);
			}
		}

		// Token: 0x06004FD7 RID: 20439 RVA: 0x00255450 File Offset: 0x00253650
		private void ApplyItemBrightness(float brightness)
		{
			if (this._colorMultiplyStyleRoot == null)
			{
				this._colorMultiplyStyleRoot = base.GetComponent<ColorMultiplyStyleRoot>();
			}
			bool flag = this._colorMultiplyStyleRoot != null;
			if (flag)
			{
				this._colorMultiplyStyleRoot.RestoreAllToWhite();
				bool flag2 = Mathf.Abs(brightness - 1f) < 0.0001f;
				if (!flag2)
				{
					this._colorMultiplyStyleRoot.MultiplyColor(new Vector4(brightness, brightness, brightness, 1f));
				}
			}
			else
			{
				this.ApplyWhitelistBrightness(brightness);
			}
		}

		// Token: 0x06004FD8 RID: 20440 RVA: 0x002554CC File Offset: 0x002536CC
		private void ApplyWhitelistBrightness(float brightness)
		{
			this.CacheBaseColors();
			foreach (KeyValuePair<Graphic, Color> pair in this._baseColors)
			{
				Graphic graphic = pair.Key;
				bool flag = graphic == null;
				if (!flag)
				{
					Color baseColor = pair.Value;
					graphic.color = new Color(baseColor.r * brightness, baseColor.g * brightness, baseColor.b * brightness, baseColor.a);
				}
			}
		}

		// Token: 0x06004FD9 RID: 20441 RVA: 0x00255570 File Offset: 0x00253770
		private void CacheBaseColors()
		{
			bool flag = this._baseColors.Count > 0;
			if (!flag)
			{
				this.CacheBaseColor(this.skillIcon);
				this.CacheBaseColor(this.nameLabel);
				this.CacheBaseColor(this.cdLabel);
				foreach (CImage typeBg in this.typeBgs)
				{
					this.CacheBaseColor(typeBg);
				}
			}
		}

		// Token: 0x06004FDA RID: 20442 RVA: 0x002555E0 File Offset: 0x002537E0
		private void CacheBaseColor(Graphic graphic)
		{
			bool flag = graphic == null || this._baseColors.ContainsKey(graphic);
			if (!flag)
			{
				this._baseColors.Add(graphic, graphic.color);
			}
		}

		// Token: 0x06004FDB RID: 20443 RVA: 0x00255620 File Offset: 0x00253820
		private void RefreshBackgroundSprites(EProfessionSkillType skillType)
		{
			this._isPassiveSkill = (skillType == EProfessionSkillType.Passive);
			bool flag = this.hover != null;
			if (flag)
			{
				this.hover.gameObject.SetActive(false);
			}
			this.RefreshBackgroundState();
		}

		// Token: 0x06004FDC RID: 20444 RVA: 0x00255664 File Offset: 0x00253864
		private void RefreshBackgroundState()
		{
			ProfessionSkillItem.BackgroundState state = this._selected ? ProfessionSkillItem.BackgroundState.Selected : (this._hovered ? ProfessionSkillItem.BackgroundState.Hover : ProfessionSkillItem.BackgroundState.Normal);
			Sprite backgroundSprite = this.GetBackgroundSprite(state, this._isPassiveSkill);
			foreach (CImage typeBg in this.typeBgs)
			{
				bool flag = typeBg != null;
				if (flag)
				{
					typeBg.sprite = backgroundSprite;
				}
			}
		}

		// Token: 0x06004FDD RID: 20445 RVA: 0x002556CC File Offset: 0x002538CC
		private Sprite GetBackgroundSprite(ProfessionSkillItem.BackgroundState state, bool isPassive)
		{
			Sprite result;
			if (isPassive)
			{
				if (state != ProfessionSkillItem.BackgroundState.Hover)
				{
					if (state != ProfessionSkillItem.BackgroundState.Selected)
					{
						result = this.passiveNormalBg;
					}
					else
					{
						result = this.passiveSelectedBg;
					}
				}
				else
				{
					result = this.passiveHoverBg;
				}
			}
			else if (state != ProfessionSkillItem.BackgroundState.Hover)
			{
				if (state != ProfessionSkillItem.BackgroundState.Selected)
				{
					result = this.activeNormalBg;
				}
				else
				{
					result = this.activeSelectedBg;
				}
			}
			else
			{
				result = this.activeHoverBg;
			}
			return result;
		}

		// Token: 0x06004FDE RID: 20446 RVA: 0x0025573C File Offset: 0x0025393C
		private void RefreshCd(ProfessionData professionData, int skillIndex, bool isCoolDown)
		{
			this.coolDown.SetActive(isCoolDown);
			int coolDownTime = professionData.SkillOffCooldownDates[skillIndex] - SingletonObject.getInstance<BasicGameData>().CurrDate;
			coolDownTime = Mathf.Max(0, coolDownTime);
			this.cdLabel.text = coolDownTime.ToString();
			this.RefreshCdProgressBar(professionData, skillIndex);
		}

		// Token: 0x06004FDF RID: 20447 RVA: 0x00255790 File Offset: 0x00253990
		private void RefreshCdProgressBar(ProfessionData professionData, int skillIndex)
		{
			int remainingMonths = professionData.SkillOffCooldownDates[skillIndex] - SingletonObject.getInstance<BasicGameData>().CurrDate;
			remainingMonths = Mathf.Max(0, remainingMonths);
			bool flag = remainingMonths <= 0;
			if (!flag)
			{
				int professionId = professionData.TemplateId;
				ProfessionItem professionConfig = Profession.Instance[professionId];
				int skillId = (skillIndex == 3) ? professionConfig.ExtraProfessionSkill : professionConfig.ProfessionSkills[skillIndex];
				ProfessionSkillItem skillConfig = ProfessionSkill.Instance[skillId];
				short totalCooldownMonths = skillConfig.SkillCoolDown;
				float fillAmount = (totalCooldownMonths > 0) ? ((float)remainingMonths / (float)totalCooldownMonths) : 0f;
				this.cdProgressBar.fillAmount = fillAmount;
			}
		}

		// Token: 0x06004FE0 RID: 20448 RVA: 0x0025582C File Offset: 0x00253A2C
		public void SetSkillItemTipsEnable(bool enabled)
		{
			TooltipInvoker tipDisplayer = this.interact.GetComponent<TooltipInvoker>();
			tipDisplayer.enabled = enabled;
		}

		// Token: 0x06004FE1 RID: 20449 RVA: 0x00255850 File Offset: 0x00253A50
		public void HideTips()
		{
			TooltipInvoker tipDisplayer = this.interact.GetComponent<TooltipInvoker>();
			tipDisplayer.HideTips();
		}

		// Token: 0x06004FE2 RID: 20450 RVA: 0x00255874 File Offset: 0x00253A74
		public void RefreshSkillItemTips(bool isUnlocked, int professionId, ProfessionData professionData, int skillId, int skillIndex, int taiwuExp, ResourceInts taiwuResources)
		{
			TooltipInvoker mouseTip = this.interact.GetComponent<TooltipInvoker>();
			mouseTip.Type = TipType.ProfessionSkill;
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("IsLocked", !isUnlocked);
			argBox.Set("ProfessionSkillId", skillId);
			argBox.SetObject("ProfessionData", professionData);
			argBox.Set("ProfessionId", professionId);
			argBox.Set("SkillIndex", skillIndex);
			argBox.Set("Exp", taiwuExp);
			argBox.Set("DisableAdditionalRedText", true);
			argBox.SetObject("Resources", taiwuResources);
			mouseTip.RuntimeParam = argBox;
		}

		// Token: 0x040036EE RID: 14062
		private bool _inited = false;

		// Token: 0x040036EF RID: 14063
		private bool _selected = false;

		// Token: 0x040036F0 RID: 14064
		private bool _hovered = false;

		// Token: 0x040036F1 RID: 14065
		private bool _isEmpty = false;

		// Token: 0x040036F2 RID: 14066
		private bool _isPassiveSkill = false;

		// Token: 0x040036F3 RID: 14067
		private ProfessionSkillItem.Style _style;

		// Token: 0x040036F4 RID: 14068
		private float _baseBrightness = 1f;

		// Token: 0x040036F5 RID: 14069
		private readonly Dictionary<Graphic, Color> _baseColors = new Dictionary<Graphic, Color>();

		// Token: 0x040036F6 RID: 14070
		private int _skillId;

		// Token: 0x040036F7 RID: 14071
		[SerializeField]
		private GameObject lockGo;

		// Token: 0x040036F8 RID: 14072
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x040036F9 RID: 14073
		[SerializeField]
		private GameObject coolDown;

		// Token: 0x040036FA RID: 14074
		[SerializeField]
		private TextMeshProUGUI cdLabel;

		// Token: 0x040036FB RID: 14075
		[SerializeField]
		private CImage skillIcon;

		// Token: 0x040036FC RID: 14076
		[SerializeField]
		private CButton interact;

		// Token: 0x040036FD RID: 14077
		[SerializeField]
		private CImage[] typeBgs;

		// Token: 0x040036FE RID: 14078
		[SerializeField]
		private CImage hover;

		// Token: 0x040036FF RID: 14079
		[SerializeField]
		private GameObject empty;

		// Token: 0x04003700 RID: 14080
		[SerializeField]
		private GameObject content;

		// Token: 0x04003701 RID: 14081
		[SerializeField]
		private UIParticle equipEffect;

		// Token: 0x04003702 RID: 14082
		[SerializeField]
		private Image cdProgressBar;

		// Token: 0x04003703 RID: 14083
		[SerializeField]
		private GameObject pendingLearnGo;

		// Token: 0x04003704 RID: 14084
		private ColorMultiplyStyleRoot _colorMultiplyStyleRoot;

		// Token: 0x04003705 RID: 14085
		[SerializeField]
		private Sprite activeNormalBg;

		// Token: 0x04003706 RID: 14086
		[SerializeField]
		private Sprite activeHoverBg;

		// Token: 0x04003707 RID: 14087
		[SerializeField]
		private Sprite activeSelectedBg;

		// Token: 0x04003708 RID: 14088
		[SerializeField]
		private Sprite passiveNormalBg;

		// Token: 0x04003709 RID: 14089
		[SerializeField]
		private Sprite passiveHoverBg;

		// Token: 0x0400370A RID: 14090
		[SerializeField]
		private Sprite passiveSelectedBg;

		// Token: 0x02001AF7 RID: 6903
		public enum Style
		{
			// Token: 0x0400B78A RID: 46986
			Large,
			// Token: 0x0400B78B RID: 46987
			Small
		}

		// Token: 0x02001AF8 RID: 6904
		public struct RefreshConfig
		{
			// Token: 0x0400B78C RID: 46988
			public ProfessionData ProfessionData;

			// Token: 0x0400B78D RID: 46989
			public int SkillIndexOfProfession;

			// Token: 0x0400B78E RID: 46990
			public ProfessionSkillItem.Style Style;

			// Token: 0x0400B78F RID: 46991
			public bool IsSelected;

			// Token: 0x0400B790 RID: 46992
			public bool IsEmpty;

			// Token: 0x0400B791 RID: 46993
			public bool IsPendingLearn;

			// Token: 0x0400B792 RID: 46994
			public Action OnClicked;

			// Token: 0x0400B793 RID: 46995
			public UnityAction OnPointerEnter;

			// Token: 0x0400B794 RID: 46996
			public UnityAction OnPointerExit;
		}

		// Token: 0x02001AF9 RID: 6905
		private enum BackgroundState
		{
			// Token: 0x0400B796 RID: 46998
			Normal,
			// Token: 0x0400B797 RID: 46999
			Hover,
			// Token: 0x0400B798 RID: 47000
			Selected
		}
	}
}
