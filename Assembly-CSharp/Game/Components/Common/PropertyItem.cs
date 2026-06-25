using System;
using System.Text;
using Config;
using FrameWork.UI.LanguageRule;
using TMPro;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F96 RID: 3990
	public class PropertyItem : MonoBehaviour
	{
		// Token: 0x170014B2 RID: 5298
		// (get) Token: 0x0600B774 RID: 46964 RVA: 0x00539986 File Offset: 0x00537B86
		public TooltipInvoker Tip
		{
			get
			{
				return this.tipDisplayer;
			}
		}

		// Token: 0x0600B775 RID: 46965 RVA: 0x0053998E File Offset: 0x00537B8E
		private void Awake()
		{
			this.InitRuleTip();
		}

		// Token: 0x0600B776 RID: 46966 RVA: 0x00539998 File Offset: 0x00537B98
		public void SetDelta(int value, LanguageKey typeLangId, bool showDelta = true)
		{
			bool flag = value == 0 || !this.imageDelta;
			if (flag)
			{
				bool flag2 = this.imageDelta;
				if (flag2)
				{
					this.imageDelta.gameObject.SetActive(false);
				}
			}
			else
			{
				string typeName = typeLangId.Tr();
				this.deltaDisplayer.gameObject.SetActive(true);
				bool flag3 = this.imageDelta;
				if (flag3)
				{
					this.imageDelta.gameObject.SetActive(showDelta);
				}
				this.imageDelta.sprite = ((value > 0) ? this.propertyInc : this.propertyDec);
				string[] tipsData = new string[2];
				tipsData[0] = LocalStringManager.Get(LanguageKey.LK_CombatDifficultyInfluence);
				bool flag4 = value > 0;
				if (flag4)
				{
					tipsData[1] = LocalStringManager.GetFormat(LanguageKey.LK_CombatDifficultyInfluence_MergedEffect, typeName, "lightblue", "+" + value.ToString());
				}
				else
				{
					tipsData[1] = LocalStringManager.GetFormat(LanguageKey.LK_CombatDifficultyInfluence_MergedEffect, typeName, "brightred", value);
				}
				this.deltaDisplayer.PresetParam = tipsData;
			}
		}

		// Token: 0x0600B777 RID: 46967 RVA: 0x00539AA8 File Offset: 0x00537CA8
		public void SetDelta(ECharacterPropertyDisplayType property, int value, bool showDelta = true)
		{
			bool flag = value == 0 || !this.imageDelta;
			if (flag)
			{
				bool flag2 = this.imageDelta;
				if (flag2)
				{
					this.imageDelta.gameObject.SetActive(false);
				}
			}
			else
			{
				this.imageDelta.gameObject.SetActive(true);
				bool flag3 = this.imageDelta;
				if (flag3)
				{
					this.imageDelta.gameObject.SetActive(showDelta);
				}
				this.imageDelta.sprite = ((value > 0) ? this.propertyInc : this.propertyDec);
				CharacterPropertyDisplayItem propertyConfig = CharacterPropertyDisplay.Instance.GetItem((short)property);
				bool flag4 = propertyConfig == null;
				if (!flag4)
				{
					this.deltaDisplayer.PresetParam[0] = LanguageKey.LK_CombatDifficultyInfluence.Tr();
					this.deltaDisplayer.PresetParam[1] = string.Format("{0}\n\n·<SpName={1}>{2}: <color=#{3}>{4}{5}%</color>", new object[]
					{
						LanguageKey.LK_CombatDifficultyInfluenceEffect.Tr(),
						propertyConfig.TipsIcon,
						propertyConfig.Name,
						(value > 0) ? "brightblue" : "brightred",
						(value > 0) ? "+" : "",
						value
					}).SetColor("pinkyellow");
				}
			}
		}

		// Token: 0x0600B778 RID: 46968 RVA: 0x00539BF0 File Offset: 0x00537DF0
		public void SetDelta(ECharacterPropertyDisplayType[] propertys, int[] values, bool showDelta = true)
		{
			bool flag = !this.imageDelta;
			if (flag)
			{
				bool flag2 = this.imageDelta;
				if (flag2)
				{
					this.imageDelta.gameObject.SetActive(false);
				}
			}
			else
			{
				this.imageDelta.gameObject.SetActive(true);
				int value = 0;
				foreach (int v in values)
				{
					value += v;
				}
				this.imageDelta.sprite = ((value > 0) ? this.propertyInc : this.propertyDec);
				bool flag3 = this.imageDelta;
				if (flag3)
				{
					this.imageDelta.gameObject.SetActive(showDelta && value != 0);
				}
				StringBuilder sb = new StringBuilder();
				int index = 0;
				foreach (ECharacterPropertyDisplayType property in propertys)
				{
					CharacterPropertyDisplayItem propertyConfig = CharacterPropertyDisplay.Instance.GetItem((short)property);
					bool flag4 = propertyConfig != null;
					if (flag4)
					{
						sb.AppendLine(string.Format("·<SpName={0}>{1}: <color=#{2}>{3}{4}%</color>", new object[]
						{
							propertyConfig.TipsIcon,
							propertyConfig.Name,
							(values[index] > 0) ? "brightblue" : "brightred",
							(values[index] > 0) ? "+" : "",
							values[index]
						}).SetColor("pinkyellow"));
					}
					index++;
				}
				bool flag5 = sb.Length == 0;
				if (!flag5)
				{
					sb.Insert(0, LanguageKey.LK_CombatDifficultyInfluenceEffect.Tr() + "\n\n");
					this.deltaDisplayer.PresetParam[0] = LanguageKey.LK_CombatDifficultyInfluence.Tr();
					this.deltaDisplayer.PresetParam[1] = sb.ToString();
				}
			}
		}

		// Token: 0x0600B779 RID: 46969 RVA: 0x00539DCC File Offset: 0x00537FCC
		public void Set(string icon, string title, string value, bool? showBack = null, bool nativeSize = false)
		{
			bool flag = showBack != null;
			if (flag)
			{
				this.imageBack.enabled = showBack.Value;
			}
			CImage cimage = this.imageIcon;
			if (cimage != null)
			{
				cimage.SetSprite(icon, false, null);
			}
			if (nativeSize)
			{
				CImage cimage2 = this.imageIcon;
				if (cimage2 != null)
				{
					cimage2.SetNativeSize();
				}
			}
			this.textTitle.text = title;
			this.SetValue(value);
			this.RefreshRuleTip();
		}

		// Token: 0x0600B77A RID: 46970 RVA: 0x00539E44 File Offset: 0x00538044
		public void Set(Sprite iconSprite, string title, string value, bool? showBack = null, bool nativeSize = false)
		{
			bool flag = showBack != null;
			if (flag)
			{
				this.imageBack.enabled = showBack.Value;
			}
			bool flag2 = this.imageIcon;
			if (flag2)
			{
				this.imageIcon.sprite = iconSprite;
				if (nativeSize)
				{
					this.imageIcon.SetNativeSize();
				}
			}
			this.textTitle.text = title;
			this.SetValue(value);
			this.RefreshRuleTip();
		}

		// Token: 0x0600B77B RID: 46971 RVA: 0x00539EBC File Offset: 0x005380BC
		public void Set(string title, string value, bool? showBack = null)
		{
			bool flag = showBack != null;
			if (flag)
			{
				this.imageBack.enabled = showBack.Value;
			}
			this.textTitle.text = title;
			this.SetValue(value);
			this.RefreshRuleTip();
		}

		// Token: 0x0600B77C RID: 46972 RVA: 0x00539F04 File Offset: 0x00538104
		private void RefreshRuleTip()
		{
			this.InitRuleTip();
			bool flag = this._languageRuleTipsArray != null;
			if (flag)
			{
				foreach (LanguageRuleTips ruleTips in this._languageRuleTipsArray)
				{
					ruleTips.Refresh();
				}
			}
		}

		// Token: 0x0600B77D RID: 46973 RVA: 0x00539F4A File Offset: 0x0053814A
		private void InitRuleTip()
		{
			if (this._languageRuleTipsArray == null)
			{
				this._languageRuleTipsArray = base.GetComponentsInChildren<LanguageRuleTips>(true);
			}
		}

		// Token: 0x0600B77E RID: 46974 RVA: 0x00539F62 File Offset: 0x00538162
		public void SetValue(string value)
		{
			this.textValue.text = value;
		}

		// Token: 0x0600B77F RID: 46975 RVA: 0x00539F74 File Offset: 0x00538174
		public void SetLastValue(string value)
		{
			bool flag = this.textLastValue == null;
			if (!flag)
			{
				bool show = !string.IsNullOrEmpty(value);
				this.textLastValue.gameObject.SetActive(show);
				bool flag2 = show;
				if (flag2)
				{
					this.textLastValue.text = value;
				}
			}
		}

		// Token: 0x0600B780 RID: 46976 RVA: 0x00539FC4 File Offset: 0x005381C4
		public void SetIncreaseArrowVisible(bool visible)
		{
			bool flag = this.increaceArrow != null;
			if (flag)
			{
				this.increaceArrow.SetActive(visible);
			}
		}

		// Token: 0x0600B781 RID: 46977 RVA: 0x00539FEF File Offset: 0x005381EF
		public void SetSingleValue(string value)
		{
			this.SetValue(value);
			this.ClearLastValue();
			this.SetIncreaseArrowVisible(false);
		}

		// Token: 0x0600B782 RID: 46978 RVA: 0x0053A00C File Offset: 0x0053820C
		public void SetValueChange(int lastValue, int newValue, bool colorNewValue = true)
		{
			bool flag = newValue > lastValue;
			if (flag)
			{
				this.SetLastValue(lastValue.ToString().SetColor("PersonalityType_Calm"));
				string newValueStr = newValue.ToString();
				if (colorNewValue)
				{
					newValueStr = newValueStr.SetColor("FiveElementType_Xuanyin");
				}
				this.SetValue(newValueStr);
				this.SetIncreaseArrowVisible(true);
			}
			else
			{
				this.SetSingleValue(newValue.ToString());
			}
		}

		// Token: 0x0600B783 RID: 46979 RVA: 0x0053A074 File Offset: 0x00538274
		public void ClearLastValue()
		{
			this.SetLastValue(string.Empty);
			this.SetIncreaseArrowVisible(false);
		}

		// Token: 0x0600B784 RID: 46980 RVA: 0x0053A08B File Offset: 0x0053828B
		public void SetShowBack(bool showBack)
		{
			this.imageBack.enabled = showBack;
		}

		// Token: 0x0600B785 RID: 46981 RVA: 0x0053A09C File Offset: 0x0053829C
		public void SetTip(int templateId)
		{
			bool flag = this.tipDisplayer == null;
			if (!flag)
			{
				CharacterPropertyDisplayItem config = CharacterPropertyDisplay.Instance[templateId];
				bool flag2 = config == null;
				if (!flag2)
				{
					this.tipDisplayer.enabled = true;
					this.tipDisplayer.IsLanguageKey = false;
					this.tipDisplayer.Type = TipType.Simple;
					this.tipDisplayer.PresetParam = new string[]
					{
						config.Name,
						config.Desc
					};
				}
			}
		}

		// Token: 0x0600B786 RID: 46982 RVA: 0x0053A11C File Offset: 0x0053831C
		public void SetTip(string tipName, string tipDesc)
		{
			bool flag = this.tipDisplayer == null;
			if (!flag)
			{
				this.tipDisplayer.enabled = true;
				this.tipDisplayer.IsLanguageKey = false;
				this.tipDisplayer.Type = TipType.Simple;
				this.tipDisplayer.PresetParam = new string[]
				{
					tipName,
					tipDesc
				};
			}
		}

		// Token: 0x0600B787 RID: 46983 RVA: 0x0053A17C File Offset: 0x0053837C
		public void PlayParticle()
		{
			bool flag = this.particle == null;
			if (!flag)
			{
				this.particle.gameObject.SetActive(true);
				bool flag2 = !this.particle.isPlaying;
				if (flag2)
				{
					this.particle.Play();
				}
			}
		}

		// Token: 0x0600B788 RID: 46984 RVA: 0x0053A1CC File Offset: 0x005383CC
		public void SetIconEnable(bool active)
		{
			this.imageIcon.enabled = active;
		}

		// Token: 0x04008E78 RID: 36472
		[SerializeField]
		private CImage imageBack;

		// Token: 0x04008E79 RID: 36473
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04008E7A RID: 36474
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04008E7B RID: 36475
		[SerializeField]
		private TextMeshProUGUI textValue;

		// Token: 0x04008E7C RID: 36476
		[SerializeField]
		private TextMeshProUGUI textLastValue;

		// Token: 0x04008E7D RID: 36477
		[SerializeField]
		private GameObject increaceArrow;

		// Token: 0x04008E7E RID: 36478
		[SerializeField]
		private ParticleSystem particle;

		// Token: 0x04008E7F RID: 36479
		[SerializeField]
		private TooltipInvoker tipDisplayer;

		// Token: 0x04008E80 RID: 36480
		[SerializeField]
		private CImage imageDelta;

		// Token: 0x04008E81 RID: 36481
		[SerializeField]
		private Sprite propertyInc;

		// Token: 0x04008E82 RID: 36482
		[SerializeField]
		private Sprite propertyDec;

		// Token: 0x04008E83 RID: 36483
		[SerializeField]
		private TooltipInvoker deltaDisplayer;

		// Token: 0x04008E84 RID: 36484
		private LanguageRuleTips[] _languageRuleTipsArray;
	}
}
