using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using GameData.Domains.CombatSkill;
using GameData.Domains.Taiwu.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000251 RID: 593
public class LoopingInformation : Refers
{
	// Token: 0x1700044A RID: 1098
	// (get) Token: 0x06002740 RID: 10048 RVA: 0x00121803 File Offset: 0x0011FA03
	private CImage _sourceFiveElementIcon
	{
		get
		{
			return base.CGet<CImage>("FEIcon1");
		}
	}

	// Token: 0x1700044B RID: 1099
	// (get) Token: 0x06002741 RID: 10049 RVA: 0x00121810 File Offset: 0x0011FA10
	private CImage _destinationFiveElementIcon
	{
		get
		{
			return base.CGet<CImage>("FEIcon2");
		}
	}

	// Token: 0x1700044C RID: 1100
	// (get) Token: 0x06002742 RID: 10050 RVA: 0x0012181D File Offset: 0x0011FA1D
	private TextMeshProUGUI _sourceFiveElementLabel
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("SourceFiveElement");
		}
	}

	// Token: 0x1700044D RID: 1101
	// (get) Token: 0x06002743 RID: 10051 RVA: 0x0012182A File Offset: 0x0011FA2A
	private TextMeshProUGUI _destinationFiveElementLabel
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("DestFiveElement");
		}
	}

	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x06002744 RID: 10052 RVA: 0x00121837 File Offset: 0x0011FA37
	private TextMeshProUGUI _transferAmountLabel
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("FiveElementTransferAmount");
		}
	}

	// Token: 0x1700044F RID: 1103
	// (get) Token: 0x06002745 RID: 10053 RVA: 0x00121844 File Offset: 0x0011FA44
	private TextMeshProUGUI _neiliLabel
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("Neili");
		}
	}

	// Token: 0x17000450 RID: 1104
	// (get) Token: 0x06002746 RID: 10054 RVA: 0x00121851 File Offset: 0x0011FA51
	private TextMeshProUGUI _eventRateLabel
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("EventRate");
		}
	}

	// Token: 0x17000451 RID: 1105
	// (get) Token: 0x06002747 RID: 10055 RVA: 0x0012185E File Offset: 0x0011FA5E
	private List<Refers> _neiliAllocationItemList
	{
		get
		{
			return base.CGetList<Refers>("NeiliAllocationItem_");
		}
	}

	// Token: 0x17000452 RID: 1106
	// (get) Token: 0x06002748 RID: 10056 RVA: 0x0012186B File Offset: 0x0011FA6B
	private List<Refers> _qiArtStrategySlotList
	{
		get
		{
			return base.CGetList<Refers>("LoopingStrategySlot");
		}
	}

	// Token: 0x17000453 RID: 1107
	// (get) Token: 0x06002749 RID: 10057 RVA: 0x00121878 File Offset: 0x0011FA78
	private GameObject _fiveElementTransferHolder
	{
		get
		{
			return base.CGet<GameObject>("FiveElementTransferHolder");
		}
	}

	// Token: 0x17000454 RID: 1108
	// (get) Token: 0x0600274A RID: 10058 RVA: 0x00121885 File Offset: 0x0011FA85
	private GameObject _fiveElementTransferAmountHolder
	{
		get
		{
			return base.CGet<GameObject>("FiveElementTransferAmountHolder");
		}
	}

	// Token: 0x17000455 RID: 1109
	// (get) Token: 0x0600274B RID: 10059 RVA: 0x00121892 File Offset: 0x0011FA92
	private CButtonObsolete _removeButton
	{
		get
		{
			return base.CGet<CButtonObsolete>("RemoveCurNeigongBtn");
		}
	}

	// Token: 0x17000456 RID: 1110
	// (get) Token: 0x0600274C RID: 10060 RVA: 0x0012189F File Offset: 0x0011FA9F
	private ToggleGroup _strategiesToggleGroup
	{
		get
		{
			return base.CGet<ToggleGroup>("Strategies");
		}
	}

	// Token: 0x0600274D RID: 10061 RVA: 0x001218AC File Offset: 0x0011FAAC
	public void SetData(CombatSkillDisplayData skillDisplayData, List<short> referenceSkillList, List<int> extraNeiliAllocationProgress, List<QiArtStrategyDisplayData> qiArtStrategies, ValueTuple<int, int> extraDeltaNeiliPerloop, IntList extraDeltaNeiliAllocationPerloop, Action onClickRemoveButton = null, bool enableRemoveButton = true, bool enableStrategyToggle = false, Action<int> onSelectStrategyToggle = null)
	{
		CombatSkillItem skillConfig = CombatSkill.Instance[skillDisplayData.TemplateId];
		this.RefreshFiveElement(skillDisplayData, skillConfig, qiArtStrategies);
		this.RefreshNeili(skillDisplayData, skillConfig, extraDeltaNeiliPerloop);
		this.RefreshEventRate(skillConfig, referenceSkillList);
		this.RefreshNeiliAllocationItems(skillConfig, extraNeiliAllocationProgress, extraDeltaNeiliAllocationPerloop);
		this.RefreshQiArtStrategies(qiArtStrategies, enableStrategyToggle, onSelectStrategyToggle);
		this._removeButton.gameObject.SetActive(enableRemoveButton);
		if (enableRemoveButton)
		{
			this._removeButton.ClearAndAddListener(onClickRemoveButton);
		}
	}

	// Token: 0x0600274E RID: 10062 RVA: 0x0012192C File Offset: 0x0011FB2C
	public void SelectStrategy(int index)
	{
		Refers refers = this._qiArtStrategySlotList[index];
		CToggleObsolete toggle = refers.CGet<CToggleObsolete>("Toggle");
		toggle.isOn = true;
	}

	// Token: 0x0600274F RID: 10063 RVA: 0x0012195C File Offset: 0x0011FB5C
	private void RefreshQiArtStrategies(List<QiArtStrategyDisplayData> qiArtStrategies, bool enableStrategyToggle, Action<int> onSelectStrategyToggle)
	{
		int currentDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
		for (int i = 0; i < this._qiArtStrategySlotList.Count; i++)
		{
			Refers refers = this._qiArtStrategySlotList[i];
			sbyte strategyId = qiArtStrategies[i].TemplateId;
			GameObject item = refers.CGet<GameObject>("Item");
			LoopingStrategyTipHelper itemTipHelper = refers.CGet<LoopingStrategyTipHelper>("ItemBack");
			GameObject emptyBack = refers.CGet<GameObject>("EmptyBack");
			CToggleObsolete toggle = refers.CGet<CToggleObsolete>("Toggle");
			CButtonObsolete button = refers.CGet<CButtonObsolete>("ToggleButton");
			itemTipHelper.GetComponent<LoopingStrategyTipHelper>().Refresh(strategyId);
			GameObject selected = refers.CGet<GameObject>("Selected");
			toggle.interactable = false;
			button.interactable = false;
			button.gameObject.SetActive(enableStrategyToggle);
			emptyBack.SetActive(true);
			if (enableStrategyToggle)
			{
				toggle.group = this._strategiesToggleGroup;
			}
			toggle.isOn = false;
			bool flag = onSelectStrategyToggle != null;
			if (flag)
			{
				button.ClearAndAddListener(delegate
				{
					toggle.isOn = true;
				});
				toggle.onValueChanged.RemoveAllListeners();
				int ii = i;
				toggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					selected.SetActive(isOn);
					if (isOn)
					{
						onSelectStrategyToggle(ii);
					}
				});
			}
			bool flag2 = strategyId < 0;
			if (flag2)
			{
				item.SetActive(false);
			}
			else
			{
				item.SetActive(true);
				QiArtStrategyItem config = QiArtStrategy.Instance[strategyId];
				TextMeshProUGUI strategyName = refers.CGet<TextMeshProUGUI>("StrategyName");
				strategyName.text = config.Name;
				GameObject strategyBack = refers.CGet<GameObject>("StrategyBack");
				TextMeshProUGUI duration = refers.CGet<TextMeshProUGUI>("Duration");
				duration.text = (qiArtStrategies[i].ExpireTime - currentDate).ToString();
			}
		}
	}

	// Token: 0x06002750 RID: 10064 RVA: 0x00121B98 File Offset: 0x0011FD98
	private void RefreshNeiliAllocationItems(CombatSkillItem skillConfig, List<int> extraNeiliAllocationProgress, IntList extraDeltaNeiliAllocationPerloop)
	{
		for (int i = 0; i < this._neiliAllocationItemList.Count; i++)
		{
			Refers refers = this._neiliAllocationItemList[i];
			this.RefreshNeiliAllocationItem((byte)i, refers, extraNeiliAllocationProgress, skillConfig, extraDeltaNeiliAllocationPerloop.Items[i], extraDeltaNeiliAllocationPerloop.Items[i + 4]);
		}
	}

	// Token: 0x06002751 RID: 10065 RVA: 0x00121BF8 File Offset: 0x0011FDF8
	private void RefreshNeiliAllocationItem(byte neiliAllocationType, Refers refers, List<int> extraNeiliAllocationProgress, CombatSkillItem skillConfig, int minDeltaNeiliAllocation, int maxDeltaNeiliAllocation)
	{
		CImage allocationIcon = refers.CGet<CImage>("AllocationIcon");
		allocationIcon.SetSprite("sp_23_logo_" + ((int)(neiliAllocationType + 2)).ToString(), false, null);
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

	// Token: 0x06002752 RID: 10066 RVA: 0x00121E7C File Offset: 0x0012007C
	private void RefreshEventRate(CombatSkillItem skillConfig, List<short> referenceSkillList)
	{
		int rate = Math.Min(100, LoopingCommonUtils.CalcLoopingEventRate(skillConfig, referenceSkillList));
		this._eventRateLabel.text = string.Format("{0}%", rate);
	}

	// Token: 0x06002753 RID: 10067 RVA: 0x00121EB8 File Offset: 0x001200B8
	private void RefreshNeili(CombatSkillDisplayData skillDisplayData, CombatSkillItem skillConfig, [TupleElementNames(new string[]
	{
		"min",
		"max"
	})] ValueTuple<int, int> extraDeltaNeiliPerloop)
	{
		short obtainedNeili = skillDisplayData.ObtainedNeili;
		short basicNeiliPerLoop = skillConfig.ObtainedNeiliPerLoop;
		short maxNeili = skillDisplayData.MaxObtainableNeili;
		int realMin = Math.Min((int)basicNeiliPerLoop + extraDeltaNeiliPerloop.Item1, (int)(maxNeili - obtainedNeili));
		int realMax = Math.Min((int)basicNeiliPerLoop + extraDeltaNeiliPerloop.Item2, (int)(maxNeili - obtainedNeili));
		string extraNeiliString = (realMax > realMin) ? string.Format("{0}~{1}", realMin, realMax) : realMin.ToString();
		string coloredExtraNeiliString = (realMax > 0) ? ("<color=#brightblue>+" + extraNeiliString + "</color>") : "";
		this._neiliLabel.text = string.Format("{0}{1}/{2}", obtainedNeili, coloredExtraNeiliString, maxNeili).ColorReplace();
	}

	// Token: 0x06002754 RID: 10068 RVA: 0x00121F70 File Offset: 0x00120170
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
			this.SetFiveElementIcon(this._sourceFiveElementIcon, srcType);
			this.SetFiveElementIcon(this._destinationFiveElementIcon, destType);
			this._sourceFiveElementLabel.text = LocalStringManager.Get("LK_FiveElements_Type_" + srcType.ToString());
			this._destinationFiveElementLabel.text = LocalStringManager.Get("LK_FiveElements_Type_" + destType.ToString());
			sbyte baseTransferAmount = skillConfig.FiveElementChangePerLoop;
			int amountBonusMin = 0;
			int amountBonusMax = 0;
			foreach (QiArtStrategyDisplayData strategy in qiArtStrategies)
			{
				bool flag2 = strategy.TemplateId == -1;
				if (!flag2)
				{
					QiArtStrategyItem strategyConfig = QiArtStrategy.Instance[strategy.TemplateId];
					short minBonus = strategyConfig.MinExtraFiveElements;
					short maxBonus = strategyConfig.MaxExtraFiveElements;
					amountBonusMin += (int)minBonus;
					amountBonusMax += (int)maxBonus;
				}
			}
			int amountMin = (int)baseTransferAmount * (100 + amountBonusMin) / 100;
			int amountMax = (int)baseTransferAmount * (100 + amountBonusMax) / 100;
			bool flag3 = amountMax > amountMin;
			if (flag3)
			{
				this._transferAmountLabel.text = string.Format("{0}~{1}", amountMin, amountMax);
			}
			else
			{
				this._transferAmountLabel.text = amountMin.ToString();
			}
		}
	}

	// Token: 0x06002755 RID: 10069 RVA: 0x00122158 File Offset: 0x00120358
	private void SetFiveElementIcon(CImage image, sbyte type)
	{
		image.SetSprite("sp_icon_fiveelements_" + type.ToString(), false, null);
	}
}
