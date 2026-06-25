using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F32 RID: 3890
	public class InjuryPartItem : MonoBehaviour
	{
		// Token: 0x1700143E RID: 5182
		// (get) Token: 0x0600B2F6 RID: 45814 RVA: 0x00517BD3 File Offset: 0x00515DD3
		public TooltipInvoker ButtonTip
		{
			get
			{
				return this.buttonTip;
			}
		}

		// Token: 0x1700143F RID: 5183
		// (get) Token: 0x0600B2F7 RID: 45815 RVA: 0x00517BDB File Offset: 0x00515DDB
		public TooltipInvoker BackTip
		{
			get
			{
				return this.backTip;
			}
		}

		// Token: 0x17001440 RID: 5184
		// (get) Token: 0x0600B2F8 RID: 45816 RVA: 0x00517BE3 File Offset: 0x00515DE3
		public CButton Button
		{
			get
			{
				return this.button;
			}
		}

		// Token: 0x17001441 RID: 5185
		// (get) Token: 0x0600B2F9 RID: 45817 RVA: 0x00517BEB File Offset: 0x00515DEB
		public PointerTrigger PointerTrigger
		{
			get
			{
				return this.pointerTrigger;
			}
		}

		// Token: 0x17001442 RID: 5186
		// (get) Token: 0x0600B2FA RID: 45818 RVA: 0x00517BF3 File Offset: 0x00515DF3
		public GameObject Selected
		{
			get
			{
				return this.selected;
			}
		}

		// Token: 0x17001443 RID: 5187
		// (get) Token: 0x0600B2FB RID: 45819 RVA: 0x00517BFB File Offset: 0x00515DFB
		public GameObject Hover
		{
			get
			{
				return this.hover.gameObject;
			}
		}

		// Token: 0x17001444 RID: 5188
		// (get) Token: 0x0600B2FC RID: 45820 RVA: 0x00517C08 File Offset: 0x00515E08
		public TextMeshProUGUI TextInnerValue
		{
			get
			{
				return this.textInnerValue;
			}
		}

		// Token: 0x17001445 RID: 5189
		// (get) Token: 0x0600B2FD RID: 45821 RVA: 0x00517C10 File Offset: 0x00515E10
		public TextMeshProUGUI TextOuterValue
		{
			get
			{
				return this.textOuterValue;
			}
		}

		// Token: 0x17001446 RID: 5190
		// (get) Token: 0x0600B2FE RID: 45822 RVA: 0x00517C18 File Offset: 0x00515E18
		public HSVStyleRoot HSVStyleRoot
		{
			get
			{
				return this.hsvStyleRoot;
			}
		}

		// Token: 0x0600B2FF RID: 45823 RVA: 0x00517C20 File Offset: 0x00515E20
		public void Set(sbyte bodyPartType, CharacterInjuryDisplayData displayData)
		{
			this.icon.SetSprite("ui9_icon_bodyparts_big_0_" + bodyPartType.ToString(), false, null);
			this.textName.text = BodyPart.Instance[bodyPartType].Name;
			ValueTuple<sbyte, sbyte> valueTuple = displayData.Injuries.Get(bodyPartType);
			sbyte outerValue = valueTuple.Item1;
			sbyte innerValue = valueTuple.Item2;
			this.textInnerValue.text = innerValue.ToString();
			this.textOuterValue.text = outerValue.ToString();
			this.SetTipParam(this.backTip, displayData);
			this.SetTipParam(this.buttonTip, displayData);
			this.selected.SetActive(false);
			this.Hover.SetActive(false);
			this.pointerTrigger.enabled = false;
		}

		// Token: 0x0600B300 RID: 45824 RVA: 0x00517CEC File Offset: 0x00515EEC
		public void Preview(sbyte bodyPartType, Injuries originInjuries, Injuries previewInjuries)
		{
			ValueTuple<sbyte, sbyte> valueTuple = previewInjuries.Get(bodyPartType);
			sbyte previewOuter = valueTuple.Item1;
			sbyte previewInner = valueTuple.Item2;
			ValueTuple<sbyte, sbyte> valueTuple2 = originInjuries.Get(bodyPartType);
			sbyte curOuter = valueTuple2.Item1;
			sbyte curInner = valueTuple2.Item2;
			this.textInnerValue.text = previewInner.ToString().SetColor(this.GetValueColor((int)curInner, (int)previewInner));
			this.textOuterValue.text = previewOuter.ToString().SetColor(this.GetValueColor((int)curOuter, (int)previewOuter));
			bool isGood = previewInner < curInner || previewOuter < curOuter;
			this.RefreshHover(isGood);
			this.Hover.SetActive(previewInner != curInner || previewOuter != curOuter);
		}

		// Token: 0x0600B301 RID: 45825 RVA: 0x00517D94 File Offset: 0x00515F94
		private string GetValueColor(int origin, int preview)
		{
			return (preview < origin) ? "brightblue" : ((preview > origin) ? "brightred" : string.Empty);
		}

		// Token: 0x0600B302 RID: 45826 RVA: 0x00517DB1 File Offset: 0x00515FB1
		public void RefreshHover(bool isGood)
		{
			this.hover.sprite = (isGood ? this.spriteHoverGood : this.spriteHoverBad);
		}

		// Token: 0x0600B303 RID: 45827 RVA: 0x00517DD4 File Offset: 0x00515FD4
		private void SetTipParam(TooltipInvoker tip, CharacterInjuryDisplayData displayData)
		{
			bool flag = tip == null;
			if (!flag)
			{
				if (tip.RuntimeParam == null)
				{
					tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				tip.Type = TipType.BodyPart;
				tip.RuntimeParam.Clear();
				tip.RuntimeParam.Set("Title", this.textName.text);
				tip.RuntimeParam.Set("Type", base.gameObject.name);
				tip.RuntimeParam.SetObject("Injury", displayData.Injuries);
				tip.RuntimeParam.SetObject("CompleteDamageStepDisplayData", displayData.CompleteDamageStepDisplayData);
				tip.RuntimeParam.SetObject("AllBodyPartExists", displayData.AllBodyPartExists);
				tip.RuntimeParam.Set("CharacterId", displayData.CharacterId);
				tip.enabled = true;
			}
		}

		// Token: 0x0600B304 RID: 45828 RVA: 0x00517EC0 File Offset: 0x005160C0
		public void PlayTakeEffect()
		{
			bool flag = this.eff_TakeEffect != null;
			if (flag)
			{
				this.eff_TakeEffect.Play(true);
			}
		}

		// Token: 0x04008AE5 RID: 35557
		[SerializeField]
		private CImage icon;

		// Token: 0x04008AE6 RID: 35558
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04008AE7 RID: 35559
		[SerializeField]
		private TextMeshProUGUI textInnerValue;

		// Token: 0x04008AE8 RID: 35560
		[SerializeField]
		private TextMeshProUGUI textOuterValue;

		// Token: 0x04008AE9 RID: 35561
		[SerializeField]
		private TooltipInvoker backTip;

		// Token: 0x04008AEA RID: 35562
		[SerializeField]
		private TooltipInvoker buttonTip;

		// Token: 0x04008AEB RID: 35563
		[SerializeField]
		private CButton button;

		// Token: 0x04008AEC RID: 35564
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04008AED RID: 35565
		[SerializeField]
		private GameObject selected;

		// Token: 0x04008AEE RID: 35566
		[SerializeField]
		private CImage hover;

		// Token: 0x04008AEF RID: 35567
		[SerializeField]
		private HSVStyleRoot hsvStyleRoot;

		// Token: 0x04008AF0 RID: 35568
		[SerializeField]
		private Sprite spriteHoverGood;

		// Token: 0x04008AF1 RID: 35569
		[SerializeField]
		private Sprite spriteHoverBad;

		// Token: 0x04008AF2 RID: 35570
		[SerializeField]
		private ParticleSystem eff_TakeEffect;
	}
}
