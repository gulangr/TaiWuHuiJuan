using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class MouseTipBuildingLevel : MouseTipBase
{
	// Token: 0x17000480 RID: 1152
	// (get) Token: 0x06002921 RID: 10529 RVA: 0x00132630 File Offset: 0x00130830
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x00132634 File Offset: 0x00130834
	protected override void Init(ArgumentBox argsBox)
	{
		string line1Text;
		argsBox.Get("Line1Text", out line1Text);
		short buildingBlockTemplateId;
		argsBox.Get("BuildingBlockTemplateId", out buildingBlockTemplateId);
		int ranking;
		argsBox.Get("ResourceBlockRanking", out ranking);
		sbyte buildingLevel;
		argsBox.Get("BuildingLevel", out buildingLevel);
		argsBox.Get("IsTaiwuVillageBuilding", out this._isTaiwuVillageBuilding);
		BuildingBlockItem config = BuildingBlock.Instance[buildingBlockTemplateId];
		TextMeshProUGUI levelText = base.CGet<TextMeshProUGUI>("LevelText");
		levelText.SetText(line1Text, true);
		levelText.GetComponent<TMPTextSpriteHelper>().Parse();
		TextMeshProUGUI resourceEffectDesc = base.CGet<TextMeshProUGUI>("ResourceEffectDesc");
		GameObject resourceEffectDescSpace = base.CGet<GameObject>("ResourceEffectDescSpace");
		resourceEffectDesc.gameObject.SetActive(config.Class == EBuildingBlockClass.BornResource);
		resourceEffectDescSpace.gameObject.SetActive(config.Class == EBuildingBlockClass.BornResource);
		bool flag = config.Class == EBuildingBlockClass.BornResource;
		if (flag)
		{
			resourceEffectDesc.SetText((ranking < 5) ? LocalStringManager.GetFormat(LanguageKey.LK_Building_LevelTip_Rank1, ranking + 1, GameData.Domains.Building.SharedMethods.GetResourceBlockEffectPercentage(ranking)) : LocalStringManager.Get(LanguageKey.LK_Building_LevelTip_Rank2), true);
		}
		Refers levelRefer = base.CGet<Refers>("Line3");
		bool flag2 = config.ExpandInfos != null;
		if (flag2)
		{
			bool flag3 = config.ExpandInfos.Contains(-1);
			if (flag3)
			{
				Debug.Log(string.Format("MouseTipBuildingLevel buildingBlockTemplateId:{0}", buildingBlockTemplateId));
			}
			base.CGet<GameObject>("Line2").SetActive(true);
			base.CGet<GameObject>("Space1").SetActive(true);
			int listCount = this._levelReferList.Count;
			int count = 0;
			for (int i = 0; i < config.ExpandInfos.Count; i++)
			{
				short buildingScaleTemplateId = config.ExpandInfos[i];
				BuildingScaleItem buildingScaleItem = BuildingScale.Instance[buildingScaleTemplateId];
				bool flag4 = !this._isTaiwuVillageBuilding;
				if (!flag4)
				{
					bool flag5 = buildingLevel - 1 < 0;
					if (!flag5)
					{
						bool flag6 = buildingScaleItem.DlcAppId > 0U && !SingletonObject.getInstance<DlcManager>().IsDlcInstalled(buildingScaleItem.DlcAppId);
						if (!flag6)
						{
							count++;
							Refers cloneRefers = null;
							bool flag7 = i < listCount;
							if (flag7)
							{
								cloneRefers = this._levelReferList[i];
							}
							else
							{
								cloneRefers = Object.Instantiate<Refers>(levelRefer, base.transform);
								this._levelReferList.Add(cloneRefers);
							}
							TextMeshProUGUI text = cloneRefers.CGet<TextMeshProUGUI>("Text");
							DisableStyleRoot disableStyleRoot = text.GetComponent<DisableStyleRoot>();
							disableStyleRoot.enabled = true;
							disableStyleRoot.SetStyleEffect(GameData.Domains.Building.SharedMethods.HaveResourceBlockEffect(buildingScaleItem.TemplateId) && ranking >= 5, false);
							string desc = string.Empty;
							bool flag8 = buildingScaleItem.Type == EBuildingScaleType.Maintaince;
							if (flag8)
							{
								desc = buildingScaleItem.Desc.SetColor("brightred").ColorReplace();
							}
							else
							{
								desc = buildingScaleItem.Desc.SetColor("brightblue").ColorReplace();
							}
							text.SetText(desc.ColorReplace(), true);
							text.GetComponent<TMPTextSpriteHelper>().Parse();
							bool flag9 = buildingScaleItem.TemplateId == 109;
							if (flag9)
							{
								ExtraDomainMethod.AsyncCall.GetIsJiaoPoolOpen(this, delegate(int offset, RawDataPool dataPool)
								{
									bool isOpen = false;
									Serializer.Deserialize(dataPool, offset, ref isOpen);
									cloneRefers.gameObject.SetActive(isOpen);
								});
							}
							else
							{
								cloneRefers.gameObject.SetActive(true);
							}
						}
					}
				}
			}
			bool flag10 = count == 0;
			if (flag10)
			{
				base.CGet<GameObject>("Line2").SetActive(false);
				base.CGet<GameObject>("Space1").SetActive(false);
			}
		}
		else
		{
			base.CGet<GameObject>("Line2").SetActive(false);
			base.CGet<GameObject>("Space1").SetActive(false);
		}
	}

	// Token: 0x06002923 RID: 10531 RVA: 0x00132A10 File Offset: 0x00130C10
	protected override void OnDisable()
	{
		base.OnDisable();
		foreach (Refers refers in this._levelReferList)
		{
			refers.gameObject.SetActive(false);
		}
	}

	// Token: 0x04001DF8 RID: 7672
	private List<Refers> _levelReferList = new List<Refers>();

	// Token: 0x04001DF9 RID: 7673
	private bool _isTaiwuVillageBuilding;
}
