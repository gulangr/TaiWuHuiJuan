using System;
using DG.Tweening;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F31 RID: 3889
	public class InjuryPartArea : MonoBehaviour
	{
		// Token: 0x1700143D RID: 5181
		// (get) Token: 0x0600B2EE RID: 45806 RVA: 0x0051797C File Offset: 0x00515B7C
		public GameObject HighlightRoot
		{
			get
			{
				return this.highlightRoot;
			}
		}

		// Token: 0x0600B2EF RID: 45807 RVA: 0x00517984 File Offset: 0x00515B84
		public void Set(CharacterInjuryDisplayData displayData)
		{
			this._characterInjuryDisplayData = displayData;
			sbyte bodyPartType = 0;
			while ((int)bodyPartType < this.injuryPartItems.Length)
			{
				InjuryPartItem injuryPartItem = this.injuryPartItems[(int)bodyPartType];
				this.SetInjuryItem(injuryPartItem, bodyPartType);
				bodyPartType += 1;
			}
			this.SetDamageItem(this.mindDamageItem, LanguageKey.LK_Combat_MarkName_Mind);
			this.SetDamageItem(this.fatalDamageItem, LanguageKey.LK_Combat_MarkType_Fatal);
		}

		// Token: 0x0600B2F0 RID: 45808 RVA: 0x005179EC File Offset: 0x00515BEC
		private void SetInjuryItem(InjuryPartItem injuryPartItem, sbyte bodyPartType)
		{
			ValueTuple<sbyte, sbyte> valueTuple = this._characterInjuryDisplayData.Injuries.Get(bodyPartType);
			sbyte outerValue = valueTuple.Item1;
			sbyte innerValue = valueTuple.Item2;
			bool showInner = innerValue > 0;
			bool showOuter = outerValue > 0;
			this.newlyInnerInjuryParts[(int)bodyPartType].SetActive(showInner);
			this.newlyOuterInjuryParts[(int)bodyPartType].SetActive(showOuter);
			injuryPartItem.Set(bodyPartType, this._characterInjuryDisplayData);
			DOTweenAnimation dotweenAnimation = this.bodyAnimation;
			bool flag = ((dotweenAnimation != null) ? dotweenAnimation.tween : null) != null;
			if (flag)
			{
				this.bodyAnimation.DORestart();
			}
		}

		// Token: 0x0600B2F1 RID: 45809 RVA: 0x00517A78 File Offset: 0x00515C78
		private void SetDamageItem(GameObject item, LanguageKey titleKey)
		{
			TooltipInvoker mouseTipDisPlayer = item.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = mouseTipDisPlayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			mouseTipDisPlayer.RuntimeParam.Clear();
			mouseTipDisPlayer.RuntimeParam.Set("Title", titleKey.Tr());
			mouseTipDisPlayer.RuntimeParam.Set("Type", item.name);
			mouseTipDisPlayer.RuntimeParam.SetObject("Injury", this._characterInjuryDisplayData.Injuries);
			mouseTipDisPlayer.RuntimeParam.SetObject("CompleteDamageStepDisplayData", this._characterInjuryDisplayData.CompleteDamageStepDisplayData);
			mouseTipDisPlayer.RuntimeParam.SetObject("AllBodyPartExists", this._characterInjuryDisplayData.AllBodyPartExists);
			mouseTipDisPlayer.RuntimeParam.Set("CharacterId", this._characterInjuryDisplayData.CharacterId);
		}

		// Token: 0x0600B2F2 RID: 45810 RVA: 0x00517B54 File Offset: 0x00515D54
		public InjuryPartItem GetInjuryPartItem(sbyte partType)
		{
			return this.injuryPartItems[(int)partType];
		}

		// Token: 0x0600B2F3 RID: 45811 RVA: 0x00517B60 File Offset: 0x00515D60
		public void ShowInfectNotice(Injuries previewInjuries)
		{
			for (sbyte i = 0; i < 7; i += 1)
			{
				InjuryPartItem injuryPart = this.GetInjuryPartItem(i);
				injuryPart.Preview(i, this._characterInjuryDisplayData.Injuries, previewInjuries);
			}
		}

		// Token: 0x0600B2F4 RID: 45812 RVA: 0x00517BA0 File Offset: 0x00515DA0
		public void PlayTakeEffect()
		{
			bool flag = this.eff_TakeEffect != null;
			if (flag)
			{
				this.eff_TakeEffect.Play();
			}
		}

		// Token: 0x04008ADC RID: 35548
		[SerializeField]
		private GameObject[] newlyInnerInjuryParts;

		// Token: 0x04008ADD RID: 35549
		[SerializeField]
		private GameObject[] newlyOuterInjuryParts;

		// Token: 0x04008ADE RID: 35550
		[SerializeField]
		private InjuryPartItem[] injuryPartItems;

		// Token: 0x04008ADF RID: 35551
		[SerializeField]
		private GameObject highlightRoot;

		// Token: 0x04008AE0 RID: 35552
		[SerializeField]
		private DOTweenAnimation bodyAnimation;

		// Token: 0x04008AE1 RID: 35553
		[SerializeField]
		private GameObject mindDamageItem;

		// Token: 0x04008AE2 RID: 35554
		[SerializeField]
		private GameObject fatalDamageItem;

		// Token: 0x04008AE3 RID: 35555
		[SerializeField]
		private ParticleSystem eff_TakeEffect;

		// Token: 0x04008AE4 RID: 35556
		private CharacterInjuryDisplayData _characterInjuryDisplayData;
	}
}
