using System;
using Config;
using FrameWork;
using TMPro;

namespace UICommon.Character.Elements
{
	// Token: 0x020005F0 RID: 1520
	public class AttributeItemWithRecovery : AttributeItem
	{
		// Token: 0x060047AC RID: 18348 RVA: 0x00219684 File Offset: 0x00217884
		public AttributeItemWithRecovery(Refers refers, Refers recoveryRefers, short propertyId, bool bgActive) : base(refers, propertyId, bgActive)
		{
			bool flag = !recoveryRefers;
			if (!flag)
			{
				this._recoveryValueText = recoveryRefers.CGet<TextMeshProUGUI>("RecoveryValue");
				this._mouseTip = recoveryRefers.CGet<TooltipInvoker>("MouseTip");
			}
		}

		// Token: 0x060047AD RID: 18349 RVA: 0x002196CE File Offset: 0x002178CE
		public void UpdateRecoveryValue(short recoveryValue)
		{
			this._recoveryValue = recoveryValue;
			this.RefreshLabel();
			this.RefreshTips();
		}

		// Token: 0x060047AE RID: 18350 RVA: 0x002196E8 File Offset: 0x002178E8
		private void RefreshLabel()
		{
			bool flag = !this._recoveryValueText;
			if (!flag)
			{
				TextMeshProUGUI recoveryValueText = this._recoveryValueText;
				short recoveryValue = this._recoveryValue;
				if (!true)
				{
				}
				string text;
				if (recoveryValue <= 0)
				{
					if (recoveryValue >= 0)
					{
						text = string.Empty;
					}
					else
					{
						text = string.Format("{0}<SpName=ui_mousetip_month>", this._recoveryValue);
					}
				}
				else
				{
					text = string.Format("+{0}<SpName=ui_mousetip_month>", this._recoveryValue);
				}
				if (!true)
				{
				}
				recoveryValueText.text = text;
				this._recoveryValueText.GetComponent<TMPTextSpriteHelper>().Parse();
			}
		}

		// Token: 0x060047AF RID: 18351 RVA: 0x0021977C File Offset: 0x0021797C
		private void RefreshTips()
		{
			bool flag = !this._mouseTip;
			if (!flag)
			{
				TooltipInvoker mouseTip = this._mouseTip;
				if (mouseTip.RuntimeParam == null)
				{
					mouseTip.RuntimeParam = new ArgumentBox();
				}
				string attributeName = CharacterPropertyDisplay.Instance[this.PropertyTemplateId].Name;
				this._mouseTip.RuntimeParam.Set("arg0", attributeName);
				string changeDesc = LocalStringManager.GetFormat(LanguageKey.LK_ChangeValuePerMonth, attributeName);
				string tipContent = changeDesc + this._recoveryValue.ToString().SetColor("brightblue");
				this._mouseTip.RuntimeParam.Set("arg1", tipContent);
			}
		}

		// Token: 0x04003170 RID: 12656
		private readonly TextMeshProUGUI _recoveryValueText;

		// Token: 0x04003171 RID: 12657
		private readonly TooltipInvoker _mouseTip;

		// Token: 0x04003172 RID: 12658
		private short _recoveryValue;
	}
}
