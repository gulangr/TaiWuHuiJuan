using System;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using Game.Views.Combat.Migrate;
using GameData.Domains.Combat;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B04 RID: 2820
	public class CombatDamageCompare : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F4E RID: 3918
		// (get) Token: 0x06008ABB RID: 35515 RVA: 0x00403350 File Offset: 0x00401550
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000F4F RID: 3919
		// (get) Token: 0x06008ABC RID: 35516 RVA: 0x00403357 File Offset: 0x00401557
		private DamageCompareData Data
		{
			get
			{
				return this.Model.DamageCompareData;
			}
		}

		// Token: 0x06008ABD RID: 35517 RVA: 0x00403364 File Offset: 0x00401564
		public void Setup()
		{
			this.Model.AddEvent(ECombatEvents.CombatEnd, new OnCombatEvent(this.OnCombatEnd));
			this.Model.AddEvent(ECombatEvents.OnDamageCompareDataChanged, new OnCombatEvent(this.OnDamageCompareDataChanged));
			this.Model.AddEvent(ECombatEvents.OnSkillDamageDataChanged, new OnCombatEvent(this.OnSkillDamageDataChanged));
		}

		// Token: 0x06008ABE RID: 35518 RVA: 0x004033C0 File Offset: 0x004015C0
		public void Close()
		{
			this.Model.RemoveEvent(ECombatEvents.CombatEnd, new OnCombatEvent(this.OnCombatEnd));
			this.Model.RemoveEvent(ECombatEvents.OnDamageCompareDataChanged, new OnCombatEvent(this.OnDamageCompareDataChanged));
			this.Model.RemoveEvent(ECombatEvents.OnSkillDamageDataChanged, new OnCombatEvent(this.OnSkillDamageDataChanged));
		}

		// Token: 0x06008ABF RID: 35519 RVA: 0x0040341C File Offset: 0x0040161C
		public void Set(byte power)
		{
			CanvasGroup skillPowerAni = this.skillPowerTextAni;
			RectTransform skillPowerAniTransform = skillPowerAni.GetComponent<RectTransform>();
			string numSprite = string.Format("combat_bifen_chengshu_0_{0}", (int)(power / 10));
			this.skillPowerNum.SetSprite(numSprite, false, null);
			this.skillPowerNumAni.SetSprite(numSprite, false, null);
			skillPowerAni.DOKill(false);
			skillPowerAni.alpha = 1f;
			skillPowerAni.DOFade(0f, 0.4f);
			skillPowerAniTransform.DOKill(false);
			skillPowerAniTransform.localScale = Vector3.one;
			skillPowerAniTransform.DOScale(3f, 0.4f);
		}

		// Token: 0x06008AC0 RID: 35520 RVA: 0x004034B4 File Offset: 0x004016B4
		private void OnCombatEnd()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06008AC1 RID: 35521 RVA: 0x004034C4 File Offset: 0x004016C4
		private void OnDamageCompareDataChanged()
		{
			bool flag = this.Data.SkillId < 0;
			if (!flag)
			{
				bool hasValid = false;
				for (int i = 0; i < 3; i++)
				{
					bool flag2 = this.Data.HitValue[i] >= 0 && this.Data.HitType[i] >= 0;
					if (flag2)
					{
						hasValid = true;
						break;
					}
				}
				bool flag3 = hasValid;
				if (flag3)
				{
					this.UpdateDataCompare();
				}
				this.RefreshDetailButtonState();
			}
		}

		// Token: 0x06008AC2 RID: 35522 RVA: 0x00403540 File Offset: 0x00401740
		private void OnSkillDamageDataChanged()
		{
			this.RefreshDetailButtonState();
			this.RefreshHitBars();
			bool combatPureOpen = CombatUtils.CombatPureOpen;
			if (!combatPureOpen)
			{
				for (int i = 0; i < 3; i++)
				{
					bool flag = this._triggeredSectionEffects[i];
					if (!flag)
					{
						ESkillDamageSectionResult result = this.GetSectionResult(i);
						bool flag2 = result >= ESkillDamageSectionResult.Checked;
						if (flag2)
						{
							this._triggeredSectionEffects[i] = true;
							this.TriggerSectionEffects(i, result >= ESkillDamageSectionResult.Hit);
						}
					}
				}
			}
		}

		// Token: 0x06008AC3 RID: 35523 RVA: 0x004035B8 File Offset: 0x004017B8
		private void TriggerSectionEffects(int sectionIndex, bool isHit)
		{
			bool isMindHit = this.Data.HitValue[1] < 0 || this.Data.HitType[1] < 0;
			RectTransform hitTypeHolder = (this.Data.IsAlly == isHit) ? this.selfHitTypeHolder : this.enemyHitTypeHolder;
			sbyte hitType = (sectionIndex < 3) ? this.Data.HitType[sectionIndex] : -1;
			int hitValue = (sectionIndex < 3) ? this.Data.HitValue[sectionIndex] : -1;
			bool flag = hitType >= 0 && hitValue >= 0;
			if (flag)
			{
				hitTypeHolder.GetChild((int)(3 - hitType)).GetComponent<CombatHitType>().valueWin.DOFade(1f, 0.2f);
			}
			bool flag2 = sectionIndex < 3 && (sectionIndex == 0 || !isMindHit);
			if (flag2)
			{
				CombatUtils.PlayAndHideParticle(this.hitParticleHolder.GetChild(isMindHit ? 1 : sectionIndex).GetComponent<ParticleSystem>(), 2f);
			}
			if (isHit)
			{
				CombatUtils.PlayAndHideParticle(this.skillPowerParticleHolder.GetChild(sectionIndex).GetComponent<ParticleSystem>(), 2f);
			}
		}

		// Token: 0x06008AC4 RID: 35524 RVA: 0x004036C8 File Offset: 0x004018C8
		private void ResetTriggeredEffects()
		{
			for (int i = 0; i < 3; i++)
			{
				this._triggeredSectionEffects[i] = false;
			}
		}

		// Token: 0x06008AC5 RID: 35525 RVA: 0x004036F0 File Offset: 0x004018F0
		private ESkillDamageSectionResult GetSectionResult(int sectionIndex)
		{
			SkillDamageData damageData = this.Model.SkillDamageData;
			SkillDamageSectionData section;
			bool flag = ((damageData != null) ? damageData.Sections : null) != null && damageData.Sections.TryGetValue(sectionIndex, out section);
			ESkillDamageSectionResult result;
			if (flag)
			{
				result = section.Result;
			}
			else
			{
				result = ESkillDamageSectionResult.Uncheck;
			}
			return result;
		}

		// Token: 0x06008AC6 RID: 35526 RVA: 0x0040373C File Offset: 0x0040193C
		private void UpdateDataCompare()
		{
			bool reset = !base.gameObject.activeSelf;
			bool flag = reset;
			if (flag)
			{
				this.ResetTriggeredEffects();
			}
			bool isAlly = this.Data.IsAlly;
			long outerAttack = (long)Mathf.Max(this.Data.OuterAttackValue, 1);
			long innerAttack = (long)Mathf.Max(this.Data.InnerAttackValue, 1);
			long outerDefend = (long)Mathf.Max(this.Data.OuterDefendValue, 1);
			long innerDefend = (long)Mathf.Max(this.Data.InnerDefendValue, 1);
			GameObject outerObj = this.outer;
			GameObject innerObj = this.inner;
			this.selfAttackTag.SetActive(isAlly);
			this.selfDefendTag.SetActive(!isAlly);
			this.selfAttackDecoration.SetActive(isAlly);
			this.selfDefendDecoration.SetActive(!isAlly);
			this.enemyAttackTag.SetActive(!isAlly);
			this.enemyDefendTag.SetActive(isAlly);
			this.enemyAttackDecoration.SetActive(!isAlly);
			this.enemyDefendDecoration.SetActive(isAlly);
			outerObj.SetActive(this.Data.OuterAttackValue >= 0);
			bool activeSelf = outerObj.activeSelf;
			if (activeSelf)
			{
				bool valueWin = this.Data.IsAlly ? (this.Data.OuterAttackValue >= this.Data.OuterDefendValue) : (this.Data.OuterAttackValue < this.Data.OuterDefendValue);
				this.selfOuterValue.text = CommonUtils.GetDisplayStringForNum(isAlly ? outerAttack : outerDefend).SetColor(valueWin ? "brightblue" : "brightred");
				this.enemyOuterValue.text = CommonUtils.GetDisplayStringForNum(isAlly ? outerDefend : outerAttack);
				this.outerPercent.text = string.Format("{0}%", Mathf.Min((float)((int)(outerAttack * 100L / outerDefend)), 9999f));
			}
			innerObj.SetActive(this.Data.InnerAttackValue >= 0);
			bool activeSelf2 = innerObj.activeSelf;
			if (activeSelf2)
			{
				bool valueWin2 = this.Data.IsAlly ? (this.Data.InnerAttackValue >= this.Data.InnerDefendValue) : (this.Data.InnerAttackValue < this.Data.InnerDefendValue);
				this.selfInnerValue.text = CommonUtils.GetDisplayStringForNum(isAlly ? innerAttack : innerDefend).SetColor(valueWin2 ? "brightblue" : "brightred");
				this.enemyInnerValue.text = CommonUtils.GetDisplayStringForNum(isAlly ? innerDefend : innerAttack);
				this.innerPercent.text = string.Format("{0}%", Mathf.Min((float)((int)(innerAttack * 100L / innerDefend)), 9999f));
			}
			for (sbyte hitType = 0; hitType < 4; hitType += 1)
			{
				bool hitTypeExist = this.Data.HitType.Exist(hitType);
				CombatHitType selfHitRefers = this.selfHitTypeHolder.GetChild((int)(3 - hitType)).GetComponent<CombatHitType>();
				CombatHitType enemyHitRefers = this.enemyHitTypeHolder.GetChild((int)(3 - hitType)).GetComponent<CombatHitType>();
				selfHitRefers.gameObject.SetActive(hitTypeExist);
				enemyHitRefers.gameObject.SetActive(hitTypeExist);
				bool flag2 = hitTypeExist;
				if (flag2)
				{
					int index = this.Data.HitType.IndexOf(hitType);
					string hitValue = (this.Data.HitValue[index] >= 0) ? this.Data.HitValue[index].ToString() : "-";
					string avoidValue = (this.Data.AvoidValue[index] >= 0) ? this.Data.AvoidValue[index].ToString() : "-";
					selfHitRefers.value.text = (this.Data.IsAlly ? hitValue : avoidValue);
					selfHitRefers.attackIcon.SetActive(this.Data.IsAlly);
					selfHitRefers.defendIcon.SetActive(!this.Data.IsAlly);
					bool flag3 = reset;
					if (flag3)
					{
						selfHitRefers.valueWin.color = Color.white.SetAlpha(0f);
					}
					enemyHitRefers.value.text = (this.Data.IsAlly ? avoidValue : hitValue);
					enemyHitRefers.attackIcon.SetActive(!this.Data.IsAlly);
					enemyHitRefers.defendIcon.SetActive(this.Data.IsAlly);
					bool flag4 = reset;
					if (flag4)
					{
						enemyHitRefers.valueWin.color = Color.white.SetAlpha(0f);
					}
				}
			}
			this.SetupHitBars();
			base.gameObject.SetActive(true);
		}

		// Token: 0x06008AC7 RID: 35527 RVA: 0x00403C10 File Offset: 0x00401E10
		private void SetupHitBars()
		{
			this._validSectionCount = 0;
			for (int i = 0; i < 3; i++)
			{
				bool flag = this.Data.HitValue[i] >= 0 && this.Data.HitType[i] >= 0;
				if (flag)
				{
					int[] validSectionIndices = this._validSectionIndices;
					int validSectionCount = this._validSectionCount;
					this._validSectionCount = validSectionCount + 1;
					validSectionIndices[validSectionCount] = i;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				this._triggeredSectionEffects[j] = false;
			}
			int k = 0;
			while (k < this.hitBars.Length)
			{
				bool flag2 = k < 3;
				if (!flag2)
				{
					this.hitBars[k].gameObject.SetActive(true);
					goto IL_115;
				}
				bool visible = k < this._validSectionCount;
				this.hitBars[k].gameObject.SetActive(visible);
				bool flag3 = !visible;
				if (!flag3)
				{
					int si = this._validSectionIndices[k];
					sbyte hitType = this.Data.HitType[si];
					bool flag4 = hitType >= 0;
					if (flag4)
					{
						this.hitBars[k].SetHitType(hitType);
					}
					goto IL_115;
				}
				IL_126:
				k++;
				continue;
				IL_115:
				this.hitBars[k].SetState(CombatDamageBar.EState.NotConducted);
				goto IL_126;
			}
		}

		// Token: 0x06008AC8 RID: 35528 RVA: 0x00403D60 File Offset: 0x00401F60
		private void RefreshHitBars()
		{
			bool flag = this.Data == null;
			if (!flag)
			{
				int i = 0;
				while (i < this.hitBars.Length)
				{
					bool flag2 = i < 3;
					if (flag2)
					{
						bool flag3 = i >= this._validSectionCount;
						if (!flag3)
						{
							this.hitBars[i].SetState(this.DetermineBarState(this._validSectionIndices[i]));
						}
					}
					else
					{
						this.hitBars[i].SetState(this.DetermineBarState(i));
					}
					IL_67:
					i++;
					continue;
					goto IL_67;
				}
			}
		}

		// Token: 0x06008AC9 RID: 35529 RVA: 0x00403DEC File Offset: 0x00401FEC
		private void RefreshDetailButtonState()
		{
			bool flag = this.detailButton != null;
			if (flag)
			{
				this.detailButton.interactable = (this.Model.SkillDamageData != null);
			}
		}

		// Token: 0x06008ACA RID: 35530 RVA: 0x00403E24 File Offset: 0x00402024
		private CombatDamageBar.EState DetermineBarState(int sectionIndex)
		{
			ESkillDamageSectionResult result = this.GetSectionResult(sectionIndex);
			bool flag = result >= ESkillDamageSectionResult.Checked;
			CombatDamageBar.EState result2;
			if (flag)
			{
				result2 = ((result >= ESkillDamageSectionResult.Hit) ? CombatDamageBar.EState.Hit : CombatDamageBar.EState.Miss);
			}
			else
			{
				result2 = CombatDamageBar.EState.NotConducted;
			}
			return result2;
		}

		// Token: 0x04006A58 RID: 27224
		[SerializeField]
		private TextMeshProUGUI selfOuterValue;

		// Token: 0x04006A59 RID: 27225
		[SerializeField]
		private TextMeshProUGUI enemyOuterValue;

		// Token: 0x04006A5A RID: 27226
		[SerializeField]
		private TextMeshProUGUI outerPercent;

		// Token: 0x04006A5B RID: 27227
		[SerializeField]
		private TextMeshProUGUI selfInnerValue;

		// Token: 0x04006A5C RID: 27228
		[SerializeField]
		private TextMeshProUGUI enemyInnerValue;

		// Token: 0x04006A5D RID: 27229
		[SerializeField]
		private TextMeshProUGUI innerPercent;

		// Token: 0x04006A5E RID: 27230
		[SerializeField]
		private RectTransform selfHitTypeHolder;

		// Token: 0x04006A5F RID: 27231
		[SerializeField]
		private RectTransform enemyHitTypeHolder;

		// Token: 0x04006A60 RID: 27232
		[SerializeField]
		private GameObject selfAttackTag;

		// Token: 0x04006A61 RID: 27233
		[SerializeField]
		private GameObject selfDefendTag;

		// Token: 0x04006A62 RID: 27234
		[SerializeField]
		private GameObject enemyAttackTag;

		// Token: 0x04006A63 RID: 27235
		[SerializeField]
		private GameObject enemyDefendTag;

		// Token: 0x04006A64 RID: 27236
		[SerializeField]
		private GameObject outer;

		// Token: 0x04006A65 RID: 27237
		[SerializeField]
		private GameObject inner;

		// Token: 0x04006A66 RID: 27238
		[SerializeField]
		private GameObject selfAttackDecoration;

		// Token: 0x04006A67 RID: 27239
		[SerializeField]
		private GameObject selfDefendDecoration;

		// Token: 0x04006A68 RID: 27240
		[SerializeField]
		private GameObject enemyAttackDecoration;

		// Token: 0x04006A69 RID: 27241
		[SerializeField]
		private GameObject enemyDefendDecoration;

		// Token: 0x04006A6A RID: 27242
		[SerializeField]
		private CImage skillPowerNum;

		// Token: 0x04006A6B RID: 27243
		[SerializeField]
		private RectTransform skillPowerParticleHolder;

		// Token: 0x04006A6C RID: 27244
		[SerializeField]
		private RectTransform hitParticleHolder;

		// Token: 0x04006A6D RID: 27245
		[SerializeField]
		private CImage skillPowerNumAni;

		// Token: 0x04006A6E RID: 27246
		[SerializeField]
		private CanvasGroup skillPowerTextAni;

		// Token: 0x04006A6F RID: 27247
		[SerializeField]
		private CombatDamageBar[] hitBars;

		// Token: 0x04006A70 RID: 27248
		[SerializeField]
		private CButton detailButton;

		// Token: 0x04006A71 RID: 27249
		private readonly bool[] _triggeredSectionEffects = new bool[3];

		// Token: 0x04006A72 RID: 27250
		private readonly int[] _validSectionIndices = new int[3];

		// Token: 0x04006A73 RID: 27251
		private int _validSectionCount;
	}
}
