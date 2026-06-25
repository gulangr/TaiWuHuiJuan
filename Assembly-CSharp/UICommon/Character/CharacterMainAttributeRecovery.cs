using System;
using CharacterDataMonitor;
using Config;
using TMPro;

namespace UICommon.Character
{
	// Token: 0x020005DE RID: 1502
	public class CharacterMainAttributeRecovery : CharacterUIElement
	{
		// Token: 0x170008F2 RID: 2290
		// (get) Token: 0x060046EF RID: 18159 RVA: 0x0021373F File Offset: 0x0021193F
		protected DisorderOfQiMonitor Item
		{
			get
			{
				return this.MonitorDataItem as DisorderOfQiMonitor;
			}
		}

		// Token: 0x060046F0 RID: 18160 RVA: 0x0021374C File Offset: 0x0021194C
		public CharacterMainAttributeRecovery(Refers[] refersArray)
		{
			bool flag = refersArray == null || refersArray.Length == 0;
			if (flag)
			{
				throw new Exception("CharacterMainAttributeRecovery must handle at least one attribute Refers");
			}
			this._mainAttrRefers = refersArray;
			sbyte i = 0;
			while ((int)i < CharacterMainAttributeRecovery.MainAttributeTemplateIdArray.Length)
			{
				Refers refers = this._mainAttrRefers[(int)i];
				bool flag2 = null == refers;
				if (!flag2)
				{
					CharacterPropertyDisplayItem config = CharacterPropertyDisplay.Instance[CharacterMainAttributeRecovery.MainAttributeTemplateIdArray[(int)i]];
					refers.CGet<CImage>("Icon").SetSprite(config.Icon, false, null);
					TooltipInvoker mouseTip = refers.CGet<TooltipInvoker>("MouseTip");
					mouseTip.IsLanguageKey = false;
					mouseTip.enabled = true;
					mouseTip.Type = TipType.Simple;
					mouseTip.PresetParam = new string[]
					{
						config.Name,
						string.Empty
					};
					refers.CGet<TextMeshProUGUI>("Value").text = string.Empty;
				}
				i += 1;
			}
		}

		// Token: 0x060046F1 RID: 18161 RVA: 0x00213844 File Offset: 0x00211A44
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DisorderOfQiMonitor>(charId, this.IsDead);
		}

		// Token: 0x060046F2 RID: 18162 RVA: 0x00213867 File Offset: 0x00211A67
		internal override void BindEvent()
		{
			this.Item.AddChangeOfMainAttributeListener(new Action(this.FillElement));
		}

		// Token: 0x060046F3 RID: 18163 RVA: 0x00213883 File Offset: 0x00211A83
		public override void UnbindEvent()
		{
			this.Item.RemoveChangeOfMainAttributeListener(new Action(this.FillElement));
		}

		// Token: 0x060046F4 RID: 18164 RVA: 0x002138A0 File Offset: 0x00211AA0
		public override void FillElement()
		{
			sbyte i = 0;
			while ((int)i < CharacterMainAttributeRecovery.MainAttributeTemplateIdArray.Length)
			{
				Refers refers = this._mainAttrRefers[(int)i];
				bool flag = null == refers;
				if (!flag)
				{
					refers.CGet<TextMeshProUGUI>("Value").text = this.GetShowStringFromValue(this.Item.MainAttribute[(int)i]);
					TooltipInvoker mouseTip = refers.CGet<TooltipInvoker>("MouseTip");
					string changeDesc = LocalStringManager.GetFormat(LanguageKey.LK_ChangeValuePerMonth, mouseTip.PresetParam[0]);
					string tipContent = changeDesc + this.Item.MainAttribute[(int)i].ToString().SetColor("brightblue");
					mouseTip.PresetParam[1] = tipContent;
				}
				i += 1;
			}
		}

		// Token: 0x060046F5 RID: 18165 RVA: 0x0021395C File Offset: 0x00211B5C
		public override void ResetToEmpty()
		{
			sbyte i = 0;
			while ((int)i < CharacterMainAttributeRecovery.MainAttributeTemplateIdArray.Length)
			{
				Refers refers = this._mainAttrRefers[(int)i];
				bool flag = null == refers;
				if (!flag)
				{
					refers.CGet<TextMeshProUGUI>("Value").text = string.Empty;
				}
				i += 1;
			}
		}

		// Token: 0x060046F6 RID: 18166 RVA: 0x002139B0 File Offset: 0x00211BB0
		private string GetShowStringFromValue(short value)
		{
			return (value >= 0) ? string.Format("+{0}", value) : value.ToString();
		}

		// Token: 0x0400310F RID: 12559
		private static readonly short[] MainAttributeTemplateIdArray = new short[]
		{
			0,
			1,
			2,
			3,
			4,
			5
		};

		// Token: 0x04003110 RID: 12560
		private readonly Refers[] _mainAttrRefers;
	}
}
