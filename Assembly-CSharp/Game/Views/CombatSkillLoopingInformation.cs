using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using GameData.Domains.CombatSkill;
using GameData.Domains.Taiwu.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x020006E8 RID: 1768
	public class CombatSkillLoopingInformation : MonoBehaviour
	{
		// Token: 0x060053D4 RID: 21460 RVA: 0x0026D718 File Offset: 0x0026B918
		public void SetData(CombatSkillDisplayData skillDisplayData, List<short> referenceSkillList, List<int> extraNeiliAllocationProgress, List<QiArtStrategyDisplayData> qiArtStrategies, ValueTuple<int, int> extraDeltaNeiliPerloop, IntList extraDeltaNeiliAllocationPerloop, Action onClickRemoveButton = null, bool enableStrategyToggle = false, Action<int> onSelectStrategyToggle = null, int amount = 1)
		{
			CombatSkillItem skillConfig = CombatSkill.Instance[skillDisplayData.TemplateId];
			this.RefreshFiveElement(skillDisplayData, skillConfig, qiArtStrategies);
			this.RefreshNeili(skillDisplayData, skillConfig, extraDeltaNeiliPerloop, amount);
			this.RefreshEventRate(skillConfig, referenceSkillList);
			this.RefreshNeiliAllocationItems(skillConfig, extraNeiliAllocationProgress, extraDeltaNeiliAllocationPerloop);
		}

		// Token: 0x060053D5 RID: 21461 RVA: 0x0026D764 File Offset: 0x0026B964
		private void RefreshNeiliAllocationItems(CombatSkillItem skillConfig, List<int> extraNeiliAllocationProgress, IntList extraDeltaNeiliAllocationPerloop)
		{
			for (int i = 0; i < this.neiliAllocationItemList.Count; i++)
			{
				Refers refers = this.neiliAllocationItemList[i];
				this.RefreshNeiliAllocationItem((byte)i, refers, extraNeiliAllocationProgress, skillConfig, extraDeltaNeiliAllocationPerloop.Items[i], extraDeltaNeiliAllocationPerloop.Items[i + 4]);
			}
		}

		// Token: 0x060053D6 RID: 21462 RVA: 0x0026D7C4 File Offset: 0x0026B9C4
		private void RefreshNeiliAllocationItem(byte neiliAllocationType, Refers refers, List<int> extraNeiliAllocationProgress, CombatSkillItem skillConfig, int minDeltaNeiliAllocation, int maxDeltaNeiliAllocation)
		{
			CImage allocationIcon = refers.CGet<CImage>("AllocationIcon");
			allocationIcon.SetSprite("ui9_icon_mousetip_kungfu_" + ((int)(neiliAllocationType + 1)).ToString(), false, null);
			TextMeshProUGUI allocationType = refers.CGet<TextMeshProUGUI>("AllocationType");
			string typeText = LocalStringManager.Get("LK_Neili_Allocation_Type_" + neiliAllocationType.ToString());
			if (!true)
			{
			}
			string text;
			switch (neiliAllocationType)
			{
			case 0:
				text = "attack";
				break;
			case 1:
				text = "agile";
				break;
			case 2:
				text = "defense";
				break;
			case 3:
				text = "assist";
				break;
			default:
				text = "attack";
				break;
			}
			if (!true)
			{
			}
			string typeColor = text;
			allocationType.text = string.Concat(new string[]
			{
				"<color=#",
				typeColor,
				">",
				typeText,
				"</color>"
			}).ColorReplace();
			TextMeshProUGUI extraNeiliAllocationLabel = refers.CGet<TextMeshProUGUI>("ExtraNeiliAllocation");
			CImage progressBar = refers.CGet<CImage>("ProgressBar");
			TextMeshProUGUI extraNeiliAllocationGrow = refers.CGet<TextMeshProUGUI>("ExtraNeiliAllocationGrow");
			int currentProgress = extraNeiliAllocationProgress[(int)neiliAllocationType];
			List<int> minestones = LoopingCommonUtils.GenerateNeiliAllocationProgressMinestones(0, currentProgress);
			int num;
			if (minestones.Count <= 0)
			{
				num = currentProgress;
			}
			else
			{
				int num2 = currentProgress;
				List<int> list = minestones;
				num = num2 - list[list.Count - 1];
			}
			int progressInStage = num;
			int extraNeiliAllocation = LoopingCommonUtils.CalcExtraNeiliAllocationFromProgress(extraNeiliAllocationProgress[(int)neiliAllocationType]);
			bool flag = extraNeiliAllocation >= (int)GlobalConfig.Instance.MaxExtraNeiliAllocation;
			if (flag)
			{
				progressInStage = 0;
			}
			int progressStageLength = 100 * ((int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio + extraNeiliAllocation * (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth);
			extraNeiliAllocationLabel.text = string.Format("+{0}/{1}", extraNeiliAllocation, GlobalConfig.Instance.MaxExtraNeiliAllocation);
			progressBar.fillAmount = (float)progressInStage / (float)progressStageLength;
			sbyte basicDelta = skillConfig.ExtraNeiliAllocationProgress[(int)neiliAllocationType];
			int progressMax = LoopingCommonUtils.GetNeiliAllocationMaxProgress();
			int realMin = Math.Min((int)(basicDelta * 100) + minDeltaNeiliAllocation, progressMax - extraNeiliAllocationProgress[(int)neiliAllocationType]);
			int realMax = Math.Min((int)(basicDelta * 100) + maxDeltaNeiliAllocation, progressMax - extraNeiliAllocationProgress[(int)neiliAllocationType]);
			string growString = (realMax > realMin) ? string.Format("{0}~{1}", realMin / 100, realMax / 100) : (realMin / 100).ToString();
			string coloredGrowString = (realMax > 0) ? ("+<color=#brightblue>" + growString + "</color>") : "";
			extraNeiliAllocationGrow.text = string.Format("{0}{1}/{2}", progressInStage / 100, coloredGrowString, progressStageLength / 100).ColorReplace();
		}

		// Token: 0x060053D7 RID: 21463 RVA: 0x0026DA48 File Offset: 0x0026BC48
		private void RefreshEventRate(CombatSkillItem skillConfig, List<short> referenceSkillList)
		{
			int rate = (referenceSkillList == null) ? 100 : Math.Min(100, LoopingCommonUtils.CalcLoopingEventRate(skillConfig, referenceSkillList));
			this.eventRateLabel.text = string.Format("{0}%", rate);
		}

		// Token: 0x060053D8 RID: 21464 RVA: 0x0026DA88 File Offset: 0x0026BC88
		private void RefreshNeili(CombatSkillDisplayData skillDisplayData, CombatSkillItem skillConfig, [TupleElementNames(new string[]
		{
			"min",
			"max"
		})] ValueTuple<int, int> extraDeltaNeiliPerloop, int amount = 1)
		{
			short obtainedNeili = skillDisplayData.ObtainedNeili;
			int basicNeiliPerLoop = (int)skillConfig.ObtainedNeiliPerLoop * amount;
			short maxNeili = skillDisplayData.MaxObtainableNeili;
			int realMin = Math.Min(basicNeiliPerLoop + extraDeltaNeiliPerloop.Item1, (int)(maxNeili - obtainedNeili));
			int realMax = Math.Min(basicNeiliPerLoop + extraDeltaNeiliPerloop.Item2, (int)(maxNeili - obtainedNeili));
			string extraNeiliString = (realMax > realMin) ? string.Format("{0}~{1}", realMin, realMax) : realMin.ToString();
			string coloredExtraNeiliString = (realMax > 0) ? ("<color=#brightblue>+" + extraNeiliString + "</color>") : "";
			this.neiliLabel.text = string.Format("{0}{1}/{2}", obtainedNeili, coloredExtraNeiliString, maxNeili).ColorReplace();
		}

		// Token: 0x060053D9 RID: 21465 RVA: 0x0026DB40 File Offset: 0x0026BD40
		private void RefreshFiveElement(CombatSkillDisplayData skillDisplayData, CombatSkillItem skillConfig, List<QiArtStrategyDisplayData> qiArtStrategies)
		{
			sbyte destType = skillDisplayData.FiveElementDestTypeWhileLooping;
			sbyte transferType = skillDisplayData.FiveElementTransferTypeWhileLooping;
			bool haveTransfer = destType >= 0;
			this._fiveElementTransferHolder.SetActive(haveTransfer);
			this._fiveElementTransferAmountHolder.SetActive(haveTransfer);
			bool flag = haveTransfer;
			if (flag)
			{
				if (!true)
				{
				}
				sbyte b;
				switch (transferType)
				{
				case 0:
					b = FiveElementsType.Countered[(int)destType];
					break;
				case 1:
					b = FiveElementsType.Countering[(int)destType];
					break;
				case 2:
					b = FiveElementsType.Produced[(int)destType];
					break;
				default:
					b = FiveElementsType.Producing[(int)destType];
					break;
				}
				if (!true)
				{
				}
				sbyte srcType = b;
				this.SetFiveElementIcon(this.sourceFiveElementIcon, srcType);
				this.SetFiveElementIcon(this.destinationFiveElementIcon, destType);
				this.sourceFiveElementLabel.text = LocalStringManager.Get("LK_FiveElements_Type_" + srcType.ToString());
				this.destinationFiveElementLabel.text = LocalStringManager.Get("LK_FiveElements_Type_" + destType.ToString());
				sbyte baseTransferAmount = skillConfig.FiveElementChangePerLoop;
				int amountBonusMin = 0;
				int amountBonusMax = 0;
				bool flag2 = qiArtStrategies != null;
				if (flag2)
				{
					foreach (QiArtStrategyDisplayData strategy in qiArtStrategies)
					{
						bool flag3 = strategy.TemplateId == -1;
						if (!flag3)
						{
							QiArtStrategyItem strategyConfig = QiArtStrategy.Instance[strategy.TemplateId];
							short minBonus = strategyConfig.MinExtraFiveElements;
							short maxBonus = strategyConfig.MaxExtraFiveElements;
							amountBonusMin += (int)minBonus;
							amountBonusMax += (int)maxBonus;
						}
					}
				}
				int amountMin = (int)baseTransferAmount * (100 + amountBonusMin) / 100;
				int amountMax = (int)baseTransferAmount * (100 + amountBonusMax) / 100;
				bool flag4 = amountMax > amountMin;
				if (flag4)
				{
					this.transferAmountLabel.text = string.Format("{0}~{1}", amountMin, amountMax);
				}
				else
				{
					this.transferAmountLabel.text = amountMin.ToString();
				}
			}
		}

		// Token: 0x060053DA RID: 21466 RVA: 0x0026DD34 File Offset: 0x0026BF34
		private void SetFiveElementIcon(CImage image, sbyte type)
		{
			image.SetSprite("ui9_icon_elements_big_" + type.ToString(), false, null);
		}

		// Token: 0x040038B9 RID: 14521
		[SerializeField]
		private CImage sourceFiveElementIcon;

		// Token: 0x040038BA RID: 14522
		[SerializeField]
		private CImage destinationFiveElementIcon;

		// Token: 0x040038BB RID: 14523
		[SerializeField]
		private TextMeshProUGUI sourceFiveElementLabel;

		// Token: 0x040038BC RID: 14524
		[SerializeField]
		private TextMeshProUGUI destinationFiveElementLabel;

		// Token: 0x040038BD RID: 14525
		[SerializeField]
		private TextMeshProUGUI transferAmountLabel;

		// Token: 0x040038BE RID: 14526
		[SerializeField]
		private TextMeshProUGUI neiliLabel;

		// Token: 0x040038BF RID: 14527
		[SerializeField]
		private TextMeshProUGUI eventRateLabel;

		// Token: 0x040038C0 RID: 14528
		[SerializeField]
		private List<Refers> neiliAllocationItemList;

		// Token: 0x040038C1 RID: 14529
		[SerializeField]
		private GameObject _fiveElementTransferHolder;

		// Token: 0x040038C2 RID: 14530
		[SerializeField]
		private GameObject _fiveElementTransferAmountHolder;
	}
}
