using System;
using TMPro;

namespace UICommon.Character
{
	// Token: 0x020005DD RID: 1501
	public class CharacterMainAttributeAndQiRecovery : CharacterMainAttributeRecovery
	{
		// Token: 0x060046EC RID: 18156 RVA: 0x002135CC File Offset: 0x002117CC
		public CharacterMainAttributeAndQiRecovery(Refers[] refersArray, Refers qiRefer) : base(refersArray)
		{
			this._qiRefer = qiRefer;
			this._qiRefer.CGet<CImage>("Icon").SetSprite("mousetip_neixiwenluan", false, null);
			TooltipInvoker mouseTip = this._qiRefer.CGet<TooltipInvoker>("MouseTip");
			mouseTip.IsLanguageKey = false;
			mouseTip.enabled = true;
			mouseTip.Type = TipType.Simple;
			mouseTip.PresetParam = new string[]
			{
				LocalStringManager.Get(LanguageKey.LK_Qi_Disorder),
				string.Empty
			};
			this._qiRefer.CGet<TextMeshProUGUI>("Value").text = string.Empty;
		}

		// Token: 0x060046ED RID: 18157 RVA: 0x0021366C File Offset: 0x0021186C
		public override void FillElement()
		{
			base.FillElement();
			int changeValue = (int)(base.Item.ChangeOfQiDisorder / 10);
			string changeString = (base.Item.ChangeOfQiDisorder > 0) ? string.Format("+{0}", changeValue).SetColor("brightred") : changeValue.ToString().SetColor("brightblue");
			this._qiRefer.CGet<TextMeshProUGUI>("Value").text = changeString;
			TooltipInvoker mouseTip = this._qiRefer.CGet<TooltipInvoker>("MouseTip");
			string changeDesc = LocalStringManager.GetFormat(LanguageKey.LK_ChangeValuePerMonth, mouseTip.PresetParam[0]);
			string tipContent = changeDesc + changeString;
			mouseTip.PresetParam[1] = tipContent;
		}

		// Token: 0x060046EE RID: 18158 RVA: 0x0021371A File Offset: 0x0021191A
		public override void ResetToEmpty()
		{
			base.ResetToEmpty();
			this._qiRefer.CGet<TextMeshProUGUI>("Value").text = string.Empty;
		}

		// Token: 0x0400310E RID: 12558
		private readonly Refers _qiRefer;
	}
}
