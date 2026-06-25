using System;
using TMPro;

namespace UICommon.Character.Elements
{
	// Token: 0x020005F5 RID: 1525
	public class InfoItem
	{
		// Token: 0x060047EA RID: 18410 RVA: 0x0021B41C File Offset: 0x0021961C
		public InfoItem(Refers refers)
		{
			bool flag = null == refers;
			if (flag)
			{
				throw new Exception("Can not create InfoItem from null Refers!");
			}
			bool flag2 = refers.Names.Contains("Icon");
			if (flag2)
			{
				this._icon = refers.CGet<CImage>("Icon");
			}
			bool flag3 = refers.Names.Contains("InfoName");
			if (flag3)
			{
				this._nameLabel = refers.CGet<TextMeshProUGUI>("InfoName");
				this._nameLabel.text = string.Empty;
			}
			bool flag4 = refers.Names.Contains("InfoValue");
			if (flag4)
			{
				this._valueLabel = refers.CGet<TextMeshProUGUI>("InfoValue");
				this._valueLabel.text = string.Empty;
			}
			bool flag5 = refers.Names.Contains("MouseTip");
			if (flag5)
			{
				this._mouseTip = refers.CGet<TooltipInvoker>("MouseTip");
				this._mouseTip.enabled = false;
			}
			bool flag6 = refers.Names.Contains("Animation");
			if (flag6)
			{
				this._animation = refers.CGet<TMPTextFadeAnimation>("Animation");
				this._animation.Duration = 0.2f;
			}
		}

		// Token: 0x060047EB RID: 18411 RVA: 0x0021B54D File Offset: 0x0021974D
		public InfoItem(CImage icon, TextMeshProUGUI nameLabel, TextMeshProUGUI valueLabel, TMPTextFadeAnimation animation = null, TooltipInvoker mouseTip = null)
		{
			this._icon = icon;
			this._nameLabel = nameLabel;
			this._valueLabel = valueLabel;
			this._mouseTip = mouseTip;
			this._animation = animation;
		}

		// Token: 0x060047EC RID: 18412 RVA: 0x0021B57C File Offset: 0x0021977C
		public bool HasValidElement()
		{
			return null != this._icon || null != this._nameLabel || null != this._valueLabel;
		}

		// Token: 0x060047ED RID: 18413 RVA: 0x0021B5B9 File Offset: 0x002197B9
		public void SetIcon(string iconSpriteName)
		{
			CImage icon = this._icon;
			if (icon != null)
			{
				icon.SetSprite(iconSpriteName, false, null);
			}
		}

		// Token: 0x060047EE RID: 18414 RVA: 0x0021B5D4 File Offset: 0x002197D4
		public void SetInfoName(string infoName)
		{
			bool flag = null != this._nameLabel;
			if (flag)
			{
				this._nameLabel.text = infoName;
			}
		}

		// Token: 0x060047EF RID: 18415 RVA: 0x0021B600 File Offset: 0x00219800
		public void SetInfoValue(string value)
		{
			bool flag = null != this._valueLabel;
			if (flag)
			{
				this._valueLabel.text = value;
			}
		}

		// Token: 0x060047F0 RID: 18416 RVA: 0x0021B630 File Offset: 0x00219830
		public TooltipInvoker GetMouseTip()
		{
			return this._mouseTip;
		}

		// Token: 0x060047F1 RID: 18417 RVA: 0x0021B648 File Offset: 0x00219848
		public static void SetInfoItemValue(Refers infoItemRefers, string iconName, string valueText)
		{
			bool flag = !string.IsNullOrEmpty(iconName);
			if (flag)
			{
				infoItemRefers.CGet<CImage>("Icon").SetSprite(iconName, false, null);
			}
			TMPTextFadeAnimation animation = infoItemRefers.CGet<TMPTextFadeAnimation>("Animation");
			TextMeshProUGUI valueLabel = infoItemRefers.CGet<TextMeshProUGUI>("InfoValue");
			bool flag2 = animation == null || string.IsNullOrEmpty(valueText);
			if (flag2)
			{
				valueLabel.SetText(valueText, true);
			}
			else
			{
				animation.Duration = 0.2f;
				animation.Rewind(delegate
				{
					valueLabel.SetText(valueText.ColorReplace(), true);
					animation.Play();
				});
			}
		}

		// Token: 0x040031AF RID: 12719
		private readonly CImage _icon;

		// Token: 0x040031B0 RID: 12720
		private readonly TextMeshProUGUI _nameLabel;

		// Token: 0x040031B1 RID: 12721
		private readonly TextMeshProUGUI _valueLabel;

		// Token: 0x040031B2 RID: 12722
		private readonly TooltipInvoker _mouseTip;

		// Token: 0x040031B3 RID: 12723
		private readonly TMPTextFadeAnimation _animation;
	}
}
