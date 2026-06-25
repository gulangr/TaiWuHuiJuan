using System;
using DG.Tweening;
using FrameWork;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002CD RID: 717
public class MouseTipResourceHolder : MouseTipBase
{
	// Token: 0x170004B8 RID: 1208
	// (get) Token: 0x06002B1E RID: 11038 RVA: 0x0014F2CB File Offset: 0x0014D4CB
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B1F RID: 11039 RVA: 0x0014F2D0 File Offset: 0x0014D4D0
	public override bool CanShowWithArgumentBox(ArgumentBox argumentBox)
	{
		MapBlockData blockData;
		return argumentBox.Get<MapBlockData>("MapBlockData", out blockData) && blockData != null;
	}

	// Token: 0x06002B20 RID: 11040 RVA: 0x0014F2F8 File Offset: 0x0014D4F8
	protected override void Init(ArgumentBox argsBox)
	{
		CanvasGroup canvasGroup = base.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		MapBlockData blockData;
		argsBox.Get<MapBlockData>("MapBlockData", out blockData);
		bool isCityTown = blockData.IsCityTown();
		RectTransform resourcesHolder = base.CGet<RectTransform>("ResourceHolder");
		Refers population = resourcesHolder.GetChild(6).GetComponent<Refers>();
		Refers culture = resourcesHolder.GetChild(7).GetComponent<Refers>();
		Refers safety = resourcesHolder.GetChild(8).GetComponent<Refers>();
		this.NeedWaitData = isCityTown;
		Location location = new Location(blockData.AreaId, blockData.BlockId);
		MapDomainMethod.AsyncCall.GetBlockFullName(null, location, delegate(int offsetData, RawDataPool poolData)
		{
			FullBlockName fullBlockName = default(FullBlockName);
			Serializer.Deserialize(poolData, offsetData, ref fullBlockName);
			string blockName = SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(fullBlockName, true, true, true, true);
			this.CGet<TextMeshProUGUI>("Title").text = blockName;
			canvasGroup.DOFade(1f, 0.2f);
		});
		for (sbyte type = 0; type < 6; type += 1)
		{
			resourcesHolder.GetChild((int)type).gameObject.SetActive(!isCityTown);
		}
		population.gameObject.SetActive(isCityTown);
		culture.gameObject.SetActive(isCityTown);
		safety.gameObject.SetActive(isCityTown);
		bool flag = isCityTown;
		if (flag)
		{
			MapAreaData areaData = SingletonObject.getInstance<WorldMapModel>().Areas[(int)blockData.AreaId];
			AsyncMethodCallbackDelegate <>9__1;
			for (int i = 0; i < areaData.SettlementInfos.Length; i++)
			{
				bool flag2 = areaData.SettlementInfos[i].BlockId != blockData.GetRootBlock().BlockId;
				if (!flag2)
				{
					IAsyncMethodRequestHandler requestHandler = null;
					short settlementId = areaData.SettlementInfos[i].SettlementId;
					AsyncMethodCallbackDelegate callback;
					if ((callback = <>9__1) == null)
					{
						callback = (<>9__1 = delegate(int offset, RawDataPool pool)
						{
							SettlementDisplayData displayData = default(SettlementDisplayData);
							Serializer.Deserialize(pool, offset, ref displayData);
							population.CGet<TextMeshProUGUI>("Current").text = displayData.Population.ToString();
							culture.CGet<TextMeshProUGUI>("Current").text = displayData.Culture.ToString();
							safety.CGet<TextMeshProUGUI>("Current").text = displayData.Safety.ToString();
							UIElement element = this.Element;
							if (element != null)
							{
								element.ShowAfterRefresh();
							}
						});
					}
					OrganizationDomainMethod.AsyncCall.GetDisplayData(requestHandler, settlementId, callback);
					break;
				}
			}
		}
		else
		{
			for (sbyte type2 = 0; type2 < 6; type2 += 1)
			{
				resourcesHolder.GetChild((int)type2).GetComponent<Refers>().CGet<TextMeshProUGUI>("Current").text = (ref blockData.CurrResources.Items.FixedElementField + (IntPtr)type2 * 2).ToString();
			}
		}
	}
}
