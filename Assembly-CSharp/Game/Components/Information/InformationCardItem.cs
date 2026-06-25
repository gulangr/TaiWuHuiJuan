using System;
using System.Text;
using Config;
using FrameWork;
using Game.Components.SortAndFilter.Information;
using TMPro;
using UnityEngine;

namespace Game.Components.Information
{
	// Token: 0x02000EF6 RID: 3830
	public class InformationCardItem : MonoBehaviour
	{
		// Token: 0x0600B078 RID: 45176 RVA: 0x00506A10 File Offset: 0x00504C10
		public void Set(InformationSortAndFilterData data, bool showRemainCount = true)
		{
			InformationItem config = Information.Instance[data.TemplateId];
			InformationInfoItem infoConfig = InformationInfo.Instance[config.InfoIds[(int)data.Level]];
			sbyte usedCount = data.UsedCount;
			sbyte usedCountMax = data.UsedCountMax;
			int remainCount = (int)(usedCountMax - usedCount);
			string usedCountString = config.UsedCountWithMax ? string.Format("{0} / {1}", remainCount, usedCountMax) : string.Format("x{0}", remainCount);
			bool flag = (int)data.Level >= this.colors.Length;
			if (flag)
			{
				this.quality.sprite = this.colors[0];
				Debug.LogError(string.Format("Data.level is over colors length ! Level:{0} Len:{1}", data.Level, this.colors.Length));
			}
			else
			{
				this.quality.sprite = this.colors[(int)data.Level];
			}
			this.titleLabel.text = (config.IsNeedShowLevel ? (infoConfig.Name + "·" + LocalStringManager.Get(string.Format("LK_TraditionalChineseNumber_{0}", (int)(data.Level + 1)))) : infoConfig.Name);
			this.countBack.SetActive(infoConfig.Consume && showRemainCount);
			this.countLabel.text = usedCountString;
			this.desc.text = infoConfig.Desc;
			LanguageKey usedCountDescLanguageKey = LanguageKey.LK_Information_Tips_RemainUsedCount;
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			sb.AppendLine(infoConfig.Desc);
			switch (config.Type)
			{
			case 0:
			case 1:
				sb.AppendLine();
				sb.AppendLine(LanguageKey.LK_Information_Tips_Area.Tr());
				break;
			case 5:
			{
				bool flag2 = infoConfig.SwordInformationType == EInformationInfoSwordInformationType.SwordTombHuman || !infoConfig.Consume;
				if (flag2)
				{
					sb.AppendLine();
					sb.AppendLine(LanguageKey.LK_Information_Tips_SwordTombA.Tr());
				}
				else
				{
					EInformationInfoSwordInformationType swordInformationType = infoConfig.SwordInformationType;
					bool flag3 = swordInformationType == EInformationInfoSwordInformationType.SwordTombHeaven || swordInformationType == EInformationInfoSwordInformationType.SwordTombEarth;
					if (flag3)
					{
						usedCountDescLanguageKey = LanguageKey.LK_Information_Tips_RemainUsedCountForHelping;
						sb.AppendLine();
						sb.AppendLine(LanguageKey.LK_Information_Tips_SwordTombB.Tr());
					}
				}
				break;
			}
			case 6:
				sb.AppendLine();
				sb.AppendLine(LanguageKey.LK_Information_Tips_Profession.Tr());
				break;
			}
			bool consume = infoConfig.Consume;
			if (consume)
			{
				sb.AppendLine(usedCountDescLanguageKey.TrFormat(remainCount.ToString().SetColor("brightyellow")));
			}
			this.tips.PresetParam = new string[]
			{
				infoConfig.Name,
				sb.ToString()
			};
			this.tips.Refresh(false, -1);
			EasyPool.Free<StringBuilder>(sb);
		}

		// Token: 0x0400886B RID: 34923
		[SerializeField]
		protected CImage quality;

		// Token: 0x0400886C RID: 34924
		[SerializeField]
		protected CImage resultLevel;

		// Token: 0x0400886D RID: 34925
		[SerializeField]
		protected TextMeshProUGUI titleLabel;

		// Token: 0x0400886E RID: 34926
		[SerializeField]
		protected GameObject countBack;

		// Token: 0x0400886F RID: 34927
		[SerializeField]
		protected TextMeshProUGUI countLabel;

		// Token: 0x04008870 RID: 34928
		[SerializeField]
		protected TextMeshProUGUI desc;

		// Token: 0x04008871 RID: 34929
		[SerializeField]
		protected TooltipInvoker tips;

		// Token: 0x04008872 RID: 34930
		[Header("品质色")]
		[SerializeField]
		private Sprite[] colors;
	}
}
