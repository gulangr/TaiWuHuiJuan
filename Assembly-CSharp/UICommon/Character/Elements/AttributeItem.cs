using System;
using Config;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UICommon.Character.Elements
{
	// Token: 0x020005EF RID: 1519
	public class AttributeItem
	{
		// Token: 0x060047A8 RID: 18344 RVA: 0x00219398 File Offset: 0x00217598
		public AttributeItem(Refers refers, short propertyId, bool bgActive)
		{
			CharacterPropertyDisplayItem propertyItem = CharacterPropertyDisplay.Instance.GetItem(propertyId);
			bool flag = propertyItem == null;
			if (!flag)
			{
				this.PropertyTemplateId = propertyId;
				string icon = propertyItem.Icon;
				icon = "ui_sp_icon_attribute_0";
				refers.CGet<CImage>("Icon").SetSprite(icon, false, null);
				refers.CGet<TextMeshProUGUI>("AttrName").text = propertyItem.Name;
				this._propertyValueText = refers.CGet<TextMeshProUGUI>("AttrValue");
				this._propertyValueText.text = string.Empty;
				TooltipInvoker mouseTip = refers.CGet<TooltipInvoker>("MouseTip");
				bool flag2 = null != mouseTip;
				if (flag2)
				{
					mouseTip.enabled = true;
					mouseTip.IsLanguageKey = false;
					mouseTip.Type = TipType.Simple;
					mouseTip.PresetParam = new string[]
					{
						propertyItem.Name,
						propertyItem.Desc
					};
				}
				this.SetAttributeValueText();
			}
		}

		// Token: 0x060047A9 RID: 18345 RVA: 0x00219488 File Offset: 0x00217688
		public void UpdateValue(int value, int bonus = 0)
		{
			CommonUtils.TryKillTween(this._tweener, true);
			bool flag = this._bonusValue != 0;
			if (!flag)
			{
				int startValue = this._value;
				int startBonus = this._bonusValue;
				bool flag2 = bonus == 0;
				if (flag2)
				{
					startBonus = 0;
					this._bonusValue = 0;
				}
				this._tweener = DOVirtual.Float(0f, 1f, this.TweenTime, delegate(float stepValue)
				{
					this._value = (int)Mathf.Lerp((float)startValue, (float)value, stepValue);
					this._bonusValue = (int)Mathf.Lerp((float)startBonus, (float)bonus, stepValue);
					this.SetAttributeValueText();
				}).SetAutoKill(true);
			}
		}

		// Token: 0x060047AA RID: 18346 RVA: 0x00219530 File Offset: 0x00217730
		public void UpdateValueImmediately(int value, int bonus)
		{
			CommonUtils.TryKillTween(this._tweener, false);
			this._bonusValue = bonus;
			bool flag = bonus == 0;
			if (flag)
			{
				this.SetAttributeValueText();
			}
			else
			{
				bool flag2 = bonus > 0;
				if (flag2)
				{
					this._propertyValueText.text = string.Format("{0} <color=#{1}>+{2}</color>", value, "lightblue", bonus).ColorReplace();
				}
				else
				{
					this._propertyValueText.text = string.Format("{0} <color=#{1}>{2}</color>", value, "red", bonus).ColorReplace();
				}
			}
		}

		// Token: 0x060047AB RID: 18347 RVA: 0x002195C8 File Offset: 0x002177C8
		private void SetAttributeValueText()
		{
			string valueString = this._value.ToString();
			bool flag = this.GetShowValueString != null;
			if (flag)
			{
				valueString = this.GetShowValueString(this.PropertyTemplateId, this._value);
			}
			bool flag2 = this._bonusValue > 0;
			if (flag2)
			{
				valueString = string.Format("{0} <color=#{1}>+{2}</color>", this._value, "lightblue", this._bonusValue);
			}
			else
			{
				bool flag3 = this._bonusValue < 0;
				if (flag3)
				{
					valueString = string.Format("{0} <color=#{1}>{2}</color>", this._value, "red", this._bonusValue);
				}
			}
			this._propertyValueText.text = valueString.ColorReplace();
		}

		// Token: 0x04003168 RID: 12648
		public const float DefaultTweenTime = 0.3f;

		// Token: 0x04003169 RID: 12649
		public float TweenTime = 0.3f;

		// Token: 0x0400316A RID: 12650
		private readonly TextMeshProUGUI _propertyValueText;

		// Token: 0x0400316B RID: 12651
		public Func<short, int, string> GetShowValueString;

		// Token: 0x0400316C RID: 12652
		protected readonly short PropertyTemplateId;

		// Token: 0x0400316D RID: 12653
		private int _value;

		// Token: 0x0400316E RID: 12654
		private int _bonusValue;

		// Token: 0x0400316F RID: 12655
		private Tweener _tweener;
	}
}
