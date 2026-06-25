using System;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using GameData.Domains.Taiwu.Debate;
using UnityEngine;

namespace Game.Views.Debate
{
	// Token: 0x02000AA5 RID: 2725
	public class DebateUnitStrategy : MonoBehaviour
	{
		// Token: 0x17000EBB RID: 3771
		// (get) Token: 0x0600859C RID: 34204 RVA: 0x003E0AED File Offset: 0x003DECED
		private LifeSkillCombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<LifeSkillCombatModel>();
			}
		}

		// Token: 0x0600859D RID: 34205 RVA: 0x003E0AF4 File Offset: 0x003DECF4
		public bool Refresh(ActivatedStrategy strategy, bool valid, bool isSlotSelected, bool isSlotPreviewed)
		{
			bool show = valid || isSlotSelected || isSlotPreviewed;
			this.selfAddedEffect.gameObject.SetActive(false);
			this.enemyAddedEffect.gameObject.SetActive(false);
			this.enemyRemovedEffect.gameObject.SetActive(false);
			this.enemyAddedEffect.gameObject.SetActive(false);
			this.selfRevealEffect.gameObject.SetActive(false);
			this.npcRevealEffect.gameObject.SetActive(false);
			base.gameObject.SetActive(show);
			bool flag = !show;
			bool result;
			if (flag)
			{
				this._revealState = false;
				result = false;
			}
			else
			{
				this._isCastedByTaiwu = (strategy != null && strategy.IsCastedByTaiwu);
				this.image.SetAlpha(1f);
				Sprite sprite = this.GetStrategyImage(isSlotPreviewed, isSlotSelected, this._isCastedByTaiwu);
				this.image.sprite = sprite;
				bool needPlayRevealSound = false;
				bool flag2 = isSlotSelected || isSlotPreviewed;
				if (flag2)
				{
					this.selfRevealed.SetActive(false);
					this.npcRevealed.SetActive(false);
					this.tip.enabled = false;
				}
				else
				{
					bool isRevealed = strategy.IsRevealed || this.Model.ShowHiddenInfo;
					this.npcRevealed.SetActive(this._isCastedByTaiwu && isRevealed);
					this.selfRevealed.SetActive(!this._isCastedByTaiwu && isRevealed);
					bool flag3 = !this._revealState && isRevealed;
					if (flag3)
					{
						this._revealState = true;
						UIParticle effect = this._isCastedByTaiwu ? this.npcRevealEffect : this.selfRevealEffect;
						effect.gameObject.SetActive(true);
						effect.Play();
						needPlayRevealSound = true;
					}
					this.tip.enabled = this._revealState;
					bool enabled = this.tip.enabled;
					if (enabled)
					{
						string[] presetParam = this.tip.PresetParam;
						bool flag4 = presetParam == null || presetParam.Length != 2;
						if (flag4)
						{
							this.tip.PresetParam = new string[2];
						}
						DebateStrategyItem config = strategy.GetConfig();
						this.tip.PresetParam[0] = config.Name;
						this.tip.PresetParam[1] = config.PawnEffectDesc;
					}
				}
				result = needPlayRevealSound;
			}
			return result;
		}

		// Token: 0x0600859E RID: 34206 RVA: 0x003E0D38 File Offset: 0x003DEF38
		public bool SetStrategyRevealed()
		{
			this.image.SetAlpha(1f);
			bool needPlayAnim = !this._revealState;
			bool flag = needPlayAnim;
			if (flag)
			{
				this._revealState = true;
				UIParticle effect = this._isCastedByTaiwu ? this.npcRevealEffect : this.selfRevealEffect;
				effect.gameObject.SetActive(true);
				effect.Play();
			}
			this.npcRevealed.SetActive(this._isCastedByTaiwu);
			this.selfRevealed.SetActive(!this._isCastedByTaiwu);
			return needPlayAnim;
		}

		// Token: 0x0600859F RID: 34207 RVA: 0x003E0DC8 File Offset: 0x003DEFC8
		public void PlayRemoveStrategyAnim(bool isCastedByTaiwu)
		{
			AudioManager.Instance.PlaySound(ViewDebate.SoundRemoveUnitStrategy, false, true);
			this.image.SetAlpha(0f);
			UIParticle removedEffect = isCastedByTaiwu ? this.selfRemovedEffect : this.enemyRemovedEffect;
			removedEffect.gameObject.SetActive(true);
			removedEffect.Play();
		}

		// Token: 0x060085A0 RID: 34208 RVA: 0x003E0E20 File Offset: 0x003DF020
		public void PlayAddStrategyAnim(bool revealed, bool isCastedByTaiwu)
		{
			AudioManager.Instance.PlaySound(ViewDebate.SoundAddUnitStrategy, false, true);
			base.gameObject.SetActive(true);
			Sprite sprite = this.GetStrategyImage(false, false, isCastedByTaiwu);
			this.image.sprite = sprite;
			UIParticle addedEffect = isCastedByTaiwu ? this.selfAddedEffect : this.enemyAddedEffect;
			addedEffect.gameObject.SetActive(true);
			addedEffect.Play();
			DOVirtual.DelayedCall(0.2f, delegate
			{
				this.image.SetAlpha(1f);
			}, false);
		}

		// Token: 0x060085A1 RID: 34209 RVA: 0x003E0EA4 File Offset: 0x003DF0A4
		private Sprite GetStrategyImage(bool isPreviewed, bool isSlotSelected, bool isCastedByTaiwu)
		{
			return isSlotSelected ? this.spriteStrategySelectedSlot : (isPreviewed ? this.spriteStrategySlot : (isCastedByTaiwu ? this.spriteStrategySelf : this.spriteStrategyEnemy));
		}

		// Token: 0x0400667B RID: 26235
		[SerializeField]
		private CImage image;

		// Token: 0x0400667C RID: 26236
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x0400667D RID: 26237
		[SerializeField]
		private GameObject selfRevealed;

		// Token: 0x0400667E RID: 26238
		[SerializeField]
		private UIParticle selfRevealEffect;

		// Token: 0x0400667F RID: 26239
		[SerializeField]
		private GameObject npcRevealed;

		// Token: 0x04006680 RID: 26240
		[SerializeField]
		private UIParticle npcRevealEffect;

		// Token: 0x04006681 RID: 26241
		[SerializeField]
		private UIParticle selfRemovedEffect;

		// Token: 0x04006682 RID: 26242
		[SerializeField]
		private UIParticle selfAddedEffect;

		// Token: 0x04006683 RID: 26243
		[SerializeField]
		private UIParticle enemyRemovedEffect;

		// Token: 0x04006684 RID: 26244
		[SerializeField]
		private UIParticle enemyAddedEffect;

		// Token: 0x04006685 RID: 26245
		[SerializeField]
		private Sprite spriteStrategySelf;

		// Token: 0x04006686 RID: 26246
		[SerializeField]
		private Sprite spriteStrategyEnemy;

		// Token: 0x04006687 RID: 26247
		[SerializeField]
		private Sprite spriteStrategySelectedSlot;

		// Token: 0x04006688 RID: 26248
		[SerializeField]
		private Sprite spriteStrategySlot;

		// Token: 0x04006689 RID: 26249
		private bool _revealState;

		// Token: 0x0400668A RID: 26250
		private bool _isCastedByTaiwu;
	}
}
