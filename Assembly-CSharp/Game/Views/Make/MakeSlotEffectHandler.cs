using System;
using GameData.Domains.Item;
using UnityEngine;

namespace Game.Views.Make
{
	// Token: 0x02000951 RID: 2385
	public class MakeSlotEffectHandler : MonoBehaviour
	{
		// Token: 0x17000CDE RID: 3294
		// (get) Token: 0x060070A1 RID: 28833 RVA: 0x0034263D File Offset: 0x0034083D
		// (set) Token: 0x060070A2 RID: 28834 RVA: 0x00342645 File Offset: 0x00340845
		public MakeSlotEffectHandler.State CurState { get; private set; } = MakeSlotEffectHandler.State.Disabled;

		// Token: 0x17000CDF RID: 3295
		// (get) Token: 0x060070A3 RID: 28835 RVA: 0x0034264E File Offset: 0x0034084E
		// (set) Token: 0x060070A4 RID: 28836 RVA: 0x00342658 File Offset: 0x00340858
		public bool IsAdvanceCraftEnabled
		{
			get
			{
				return this._isAdvanceCraftEnabled;
			}
			set
			{
				bool flag = this._isAdvanceCraftEnabled != value;
				if (flag)
				{
					this._isAdvanceCraftEnabled = value;
					this.UpdateAdvanceCraftEffect();
				}
			}
		}

		// Token: 0x17000CE0 RID: 3296
		// (get) Token: 0x060070A5 RID: 28837 RVA: 0x00342686 File Offset: 0x00340886
		// (set) Token: 0x060070A6 RID: 28838 RVA: 0x00342690 File Offset: 0x00340890
		public bool IsSlotEnable
		{
			get
			{
				return this._isSlotEnable;
			}
			set
			{
				bool flag = this._isSlotEnable != value;
				if (flag)
				{
					this._isSlotEnable = value;
					this.Refresh();
				}
			}
		}

		// Token: 0x060070A7 RID: 28839 RVA: 0x003426C0 File Offset: 0x003408C0
		public void SetStateParam(bool? isSlotEnable = null, bool? isSkillNotEnough = null)
		{
			bool flag = isSlotEnable != null;
			if (flag)
			{
				this._isSlotEnable = isSlotEnable.Value;
			}
			bool flag2 = isSkillNotEnough != null;
			if (flag2)
			{
				this._isSkillNotEnough = isSkillNotEnough.Value;
			}
			this.Refresh();
		}

		// Token: 0x060070A8 RID: 28840 RVA: 0x0034270C File Offset: 0x0034090C
		public void Refresh()
		{
			bool flag = this.targetSlot == null;
			if (!flag)
			{
				bool isValid = this.targetSlot.IsValid;
				if (isValid)
				{
					this.SetState(this._isSkillNotEnough ? MakeSlotEffectHandler.State.ItemValidButLowSkill : MakeSlotEffectHandler.State.ItemValid);
					ItemKey itemId = this.targetSlot.ItemData.RealKey;
					bool flag2 = itemId != this._lastValidItemKey;
					if (flag2)
					{
						this._lastValidItemKey = new ItemKey?(itemId);
						this.PlayItemRefreshEffect();
					}
				}
				else
				{
					bool flag3 = !this.IsSlotEnable;
					if (flag3)
					{
						this.SetState(MakeSlotEffectHandler.State.Disabled);
					}
					else
					{
						this.SetState(MakeSlotEffectHandler.State.Empty);
						this._lastValidItemKey = null;
					}
				}
			}
		}

		// Token: 0x060070A9 RID: 28841 RVA: 0x003427D8 File Offset: 0x003409D8
		private void SetState(MakeSlotEffectHandler.State state)
		{
			this.CurState = state;
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_empty, this.CurState == MakeSlotEffectHandler.State.Empty);
			bool isNotTargetSlot = !this.IsTargetSlot();
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_valid, this.CurState == MakeSlotEffectHandler.State.ItemValid && isNotTargetSlot);
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_invalid, this.CurState == MakeSlotEffectHandler.State.ItemValidButLowSkill && isNotTargetSlot);
			this.UpdateAdvanceCraftEffect();
		}

		// Token: 0x060070AA RID: 28842 RVA: 0x00342842 File Offset: 0x00340A42
		private void OnEnable()
		{
			this.SetState(this.CurState);
		}

		// Token: 0x060070AB RID: 28843 RVA: 0x00342852 File Offset: 0x00340A52
		private void OnDisable()
		{
			this.ResetEffectState();
		}

		// Token: 0x060070AC RID: 28844 RVA: 0x0034285C File Offset: 0x00340A5C
		private bool IsTargetSlot()
		{
			bool flag = this.targetSlot == null || this.isIgnoreSlotType;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				EMakeTargetSlotType slotType = this.targetSlot.SlotType;
				if (!true)
				{
				}
				bool flag2;
				switch (slotType)
				{
				case EMakeTargetSlotType.MakeTarget:
					flag2 = true;
					goto IL_63;
				case EMakeTargetSlotType.AddPoisonTarget:
					flag2 = true;
					goto IL_63;
				case EMakeTargetSlotType.RemovePoisonTarget:
					flag2 = true;
					goto IL_63;
				case EMakeTargetSlotType.CustomTarget:
					flag2 = true;
					goto IL_63;
				}
				flag2 = false;
				IL_63:
				if (!true)
				{
				}
				result = flag2;
			}
			return result;
		}

		// Token: 0x060070AD RID: 28845 RVA: 0x003428D8 File Offset: 0x00340AD8
		public void ResetEffectState()
		{
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_advanceCraft, false);
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_wordsAffected, false);
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_refresh, false);
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_startCraft, false);
			this.IsAdvanceCraftEnabled = false;
			this._lastValidItemKey = null;
		}

		// Token: 0x060070AE RID: 28846 RVA: 0x0034292E File Offset: 0x00340B2E
		public void PlayEffectWordsAffected()
		{
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_wordsAffected, false);
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_wordsAffected, true);
		}

		// Token: 0x060070AF RID: 28847 RVA: 0x0034294B File Offset: 0x00340B4B
		public void PlayEffectStartCraft()
		{
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_startCraft, false);
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_startCraft, true);
		}

		// Token: 0x060070B0 RID: 28848 RVA: 0x00342968 File Offset: 0x00340B68
		public void PlayItemRefreshEffect()
		{
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_refresh, false);
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_refresh, true);
		}

		// Token: 0x060070B1 RID: 28849 RVA: 0x00342985 File Offset: 0x00340B85
		private void UpdateAdvanceCraftEffect()
		{
			MakeSlotEffectHandler.SetObjectActiveSafe(this.eff_advanceCraft, this.IsAdvanceCraftEnabled && this.CurState > MakeSlotEffectHandler.State.Empty);
		}

		// Token: 0x060070B2 RID: 28850 RVA: 0x003429A8 File Offset: 0x00340BA8
		private static void SetObjectActiveSafe(GameObject obj, bool active)
		{
			bool flag = obj != null && obj.activeSelf != active;
			if (flag)
			{
				obj.SetActive(active);
			}
		}

		// Token: 0x040053A9 RID: 21417
		[SerializeField]
		private MakeTargetSlot targetSlot;

		// Token: 0x040053AA RID: 21418
		public bool isIgnoreSlotType = false;

		// Token: 0x040053AB RID: 21419
		[Header("特效")]
		[SerializeField]
		private GameObject eff_empty;

		// Token: 0x040053AC RID: 21420
		[SerializeField]
		private GameObject eff_valid;

		// Token: 0x040053AD RID: 21421
		[SerializeField]
		private GameObject eff_invalid;

		// Token: 0x040053AE RID: 21422
		[SerializeField]
		private GameObject eff_advanceCraft;

		// Token: 0x040053AF RID: 21423
		[SerializeField]
		private GameObject eff_wordsAffected;

		// Token: 0x040053B0 RID: 21424
		[SerializeField]
		private GameObject eff_refresh;

		// Token: 0x040053B1 RID: 21425
		[SerializeField]
		private GameObject eff_startCraft;

		// Token: 0x040053B3 RID: 21427
		private bool _isAdvanceCraftEnabled = false;

		// Token: 0x040053B4 RID: 21428
		private bool _isSlotEnable = false;

		// Token: 0x040053B5 RID: 21429
		private bool _isSkillNotEnough = false;

		// Token: 0x040053B6 RID: 21430
		private ItemKey? _lastValidItemKey = null;

		// Token: 0x02001E3F RID: 7743
		public enum State
		{
			// Token: 0x0400C91D RID: 51485
			Disabled,
			// Token: 0x0400C91E RID: 51486
			Empty,
			// Token: 0x0400C91F RID: 51487
			ItemValid,
			// Token: 0x0400C920 RID: 51488
			ItemValidButLowSkill
		}
	}
}
