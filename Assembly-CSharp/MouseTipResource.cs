using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Organization;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002CC RID: 716
public class MouseTipResource : MouseTipBase
{
	// Token: 0x170004B7 RID: 1207
	// (get) Token: 0x06002B17 RID: 11031 RVA: 0x0014E96A File Offset: 0x0014CB6A
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B18 RID: 11032 RVA: 0x0014E970 File Offset: 0x0014CB70
	public override bool CanShowWithArgumentBox(ArgumentBox argumentBox)
	{
		string text;
		return argumentBox.Get("CharName", out text) && base.CanShowWithArgumentBox(argumentBox);
	}

	// Token: 0x06002B19 RID: 11033 RVA: 0x0014E996 File Offset: 0x0014CB96
	protected override void Init(ArgumentBox argsBox)
	{
		this._version++;
		this.Refresh(argsBox);
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x06002B1A RID: 11034 RVA: 0x0014E9BC File Offset: 0x0014CBBC
	public unsafe override void Refresh(ArgumentBox argsBox)
	{
		argsBox.Get("ResourceType", out this._resourceType);
		bool flag = (int)this._resourceType >= Config.ResourceType.Instance.Count;
		if (flag)
		{
			this.RefreshAsExp(argsBox);
		}
		else
		{
			ResourceTypeItem configData = Config.ResourceType.Instance[this._resourceType];
			string charName;
			argsBox.Get("CharName", out charName);
			int count;
			argsBox.Get("ResourceCount", out count);
			Dictionary<ItemSourceType, ResourceInts> resourceDict;
			argsBox.Get<Dictionary<ItemSourceType, ResourceInts>>("ResourceDict", out resourceDict);
			int countMax;
			argsBox.Get("ResourceCountMax", out countMax);
			argsBox.Get("ShowDetailChange", out this._showDetailChange);
			argsBox.Get("ShowOfferUpChange", out this._showOfferUpChange);
			CRawImage resourceImg = base.CGet<CRawImage>("ResourceImg");
			base.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.GetFormat(LanguageKey.LK_Resource_Tips_Title, charName, configData.Name);
			base.CGet<CImage>("Icon").SetSprite(configData.Icon, false, null);
			bool flag2 = resourceDict != null;
			if (flag2)
			{
				count = 0;
				foreach (KeyValuePair<ItemSourceType, ResourceInts> keyValuePair in resourceDict)
				{
					ItemSourceType itemSourceType;
					ResourceInts resourceInts;
					keyValuePair.Deconstruct(out itemSourceType, out resourceInts);
					ItemSourceType sourceType = itemSourceType;
					ResourceInts resource = resourceInts;
					int curCount = resource.Get((int)this._resourceType);
					count += curCount;
					Refers refers = base.CGet<Refers>(sourceType.ToString());
					bool show = curCount > 0;
					refers.gameObject.SetActive(show);
					bool flag3 = show;
					if (flag3)
					{
						string name = UI_Warehouse.GetTitle(sourceType, false);
						refers.CGet<TextMeshProUGUI>("Name").text = (name ?? "");
						refers.CGet<TextMeshProUGUI>("Value").text = curCount.ToString();
					}
				}
			}
			else
			{
				foreach (ItemSourceType sourceType2 in SingletonObject.getInstance<BuildingModel>().NeedItemSourceTypeList)
				{
					base.CGet<Refers>(sourceType2.ToString()).gameObject.SetActive(false);
				}
			}
			string countTitle = LocalStringManager.Get(LanguageKey.LK_Resource_NowOwn);
			string countFinialStr = count.ToString().SetColor("pinkyellow");
			base.CGet<TextMeshProUGUI>("Count").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_Resource_NowOwn, configData.Name);
			base.CGet<TextMeshProUGUI>("Value").text = countFinialStr;
			resourceImg.gameObject.SetActive(count > 0);
			bool flag4 = count > 0;
			if (flag4)
			{
				CommonUtils.SetResourceImage(this._resourceType, count, resourceImg, 1f);
				Vector2 imgSize = resourceImg.rectTransform.sizeDelta;
				Vector2 backSize = new Vector2(440f, 200f);
				float scale = 1f;
				while (imgSize.x * scale > backSize.x - 10f || imgSize.y * scale > backSize.y - 10f)
				{
					scale -= 0.05f;
				}
				resourceImg.rectTransform.localScale = Vector3.one * scale;
			}
			bool showDetailChange = this._showDetailChange;
			if (showDetailChange)
			{
				MouseTipResource.<>c__DisplayClass9_0 CS$<>8__locals1 = new MouseTipResource.<>c__DisplayClass9_0();
				CS$<>8__locals1.<>4__this = this;
				MouseTipResource.<>c__DisplayClass9_0 CS$<>8__locals2 = CS$<>8__locals1;
				int version = this._version + 1;
				this._version = version;
				CS$<>8__locals2.version = version;
				CS$<>8__locals1.resourceType = this._resourceType;
				bool flag5 = CS$<>8__locals1.resourceType == 7;
				if (flag5)
				{
					OrganizationDomainMethod.AsyncCall.CalcApprovingRateEffectAuthorityGain(this, delegate(int offset, RawDataPool dataPool)
					{
						bool flag7 = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this._version;
						if (!flag7)
						{
							int changeValue = 0;
							Serializer.Deserialize(dataPool, offset, ref changeValue);
							CS$<>8__locals1.<>4__this.CGet<TextMeshProUGUI>("SectSupportChangeValue").SetText("+" + changeValue.ToString(), true);
							CS$<>8__locals1.<>4__this.CGet<GameObject>("SectSupportChange").SetActive(changeValue > 0);
							CS$<>8__locals1.<>4__this.UpdateShow();
						}
					});
					TaiwuDomainMethod.AsyncCall.GetVillagerRoleHeadTotalAuthorityCost(this, delegate(int offset, RawDataPool pool)
					{
						bool flag7 = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this._version;
						if (!flag7)
						{
							int changeValue = 0;
							Serializer.Deserialize(pool, offset, ref changeValue);
							CS$<>8__locals1.<>4__this.CGet<TextMeshProUGUI>("VillagerRoleChangeValue").SetText(changeValue.ToString(), true);
							CS$<>8__locals1.<>4__this.CGet<GameObject>("VillagerRoleChange").SetActive(changeValue < 0);
							CS$<>8__locals1.<>4__this.UpdateShow();
						}
					});
				}
				else
				{
					base.CGet<GameObject>("SectSupportChange").SetActive(false);
					base.CGet<GameObject>("VillagerRoleChange").SetActive(false);
				}
				TaiwuDomainMethod.AsyncCall.CalcResourceChangeByVillageWork(this, delegate(int offset, RawDataPool dataPool)
				{
					bool flag7 = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this._version;
					if (!flag7)
					{
						int[] gatherChange = new int[8];
						Serializer.Deserialize(dataPool, offset, ref gatherChange);
						CS$<>8__locals1.<>4__this.CGet<TextMeshProUGUI>("GatherChangeValue").SetText("+" + gatherChange[(int)CS$<>8__locals1.resourceType].ToString(), true);
						CS$<>8__locals1.<>4__this.CGet<GameObject>("GatherChange").SetActive(gatherChange[(int)CS$<>8__locals1.resourceType] != 0);
						CS$<>8__locals1.<>4__this.UpdateShow();
					}
				});
				TaiwuDomainMethod.AsyncCall.CalcResourceChangeByBuildingEarn(this, delegate(int offset, RawDataPool dataPool)
				{
					bool flag7 = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this._version;
					if (!flag7)
					{
						int[] buildingChange = new int[8];
						Serializer.Deserialize(dataPool, offset, ref buildingChange);
						CS$<>8__locals1.<>4__this.CGet<TextMeshProUGUI>("BuildingChangeValue").SetText("+" + buildingChange[(int)CS$<>8__locals1.resourceType].ToString(), true);
						CS$<>8__locals1.<>4__this.CGet<GameObject>("BuildingChange").SetActive(buildingChange[(int)CS$<>8__locals1.resourceType] != 0);
						CS$<>8__locals1.<>4__this.UpdateShow();
					}
				});
				TaiwuDomainMethod.AsyncCall.CalcResourceChangeByBuildingMaintain(this, delegate(int offset, RawDataPool dataPool)
				{
					bool flag7 = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this._version;
					if (!flag7)
					{
						int[] maintainChange = new int[8];
						Serializer.Deserialize(dataPool, offset, ref maintainChange);
						CS$<>8__locals1.<>4__this.CGet<TextMeshProUGUI>("MaintainChangeValue").SetText(maintainChange[(int)CS$<>8__locals1.resourceType].ToString(), true);
						CS$<>8__locals1.<>4__this.CGet<GameObject>("MaintainChange").SetActive(maintainChange[(int)CS$<>8__locals1.resourceType] != 0);
						CS$<>8__locals1.<>4__this.UpdateShow();
					}
				});
				ExtraDomainMethod.AsyncCall.CalcResourceChangeByJiaoPool(this, CS$<>8__locals1.resourceType, delegate(int offset, RawDataPool dataPool)
				{
					bool flag7 = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this._version;
					if (!flag7)
					{
						int change = -1;
						Serializer.Deserialize(dataPool, offset, ref change);
						CS$<>8__locals1.<>4__this.CGet<TextMeshProUGUI>("JiaoPoolChangeValue").SetText(((change > 0) ? '+' : '-').ToString() + Mathf.Abs(change).ToString(), true);
						CS$<>8__locals1.<>4__this.CGet<GameObject>("JiaoPoolChange").SetActive(change != 0);
						CS$<>8__locals1.<>4__this.UpdateShow();
					}
				});
				BuildingDomainMethod.AsyncCall.GetAuthorityGain(this, delegate(int offset, RawDataPool pool)
				{
					bool flag7 = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this._version;
					if (!flag7)
					{
						bool needShow = CS$<>8__locals1.resourceType == 7 && SingletonObject.getInstance<TimeManager>().GetMonthInCurrYear() == (sbyte)(GlobalConfig.Instance.CricketActiveStartMonth - 1);
						int change = -1;
						Serializer.Deserialize(pool, offset, ref change);
						string valueStr = (((needShow && change > 0) ? '+' : '-').ToString() + Mathf.Abs(change).ToString()).SetColor((change > 0) ? "brightblue" : "brightred");
						CS$<>8__locals1.<>4__this.CGet<TextMeshProUGUI>("CricketChangeValue").SetText(valueStr, true);
						CS$<>8__locals1.<>4__this.CGet<GameObject>("CricketChange").SetActive(needShow && change != 0);
						CS$<>8__locals1.<>4__this.UpdateShow();
					}
				});
				TaiwuDomainMethod.AsyncCall.GetTotalVillagerMaintenance(this, delegate(int offset, RawDataPool pool)
				{
					bool flag7 = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this._version;
					if (!flag7)
					{
						ResourceInts resourceInts2 = default(ResourceInts);
						Serializer.Deserialize(pool, offset, ref resourceInts2);
						int change = -(*resourceInts2[(int)CS$<>8__locals1.resourceType]);
						bool needShow = change != 0;
						string valueStr = (((needShow && change > 0) ? '+' : '-').ToString() + Mathf.Abs(change).ToString()).SetColor((change > 0) ? "brightblue" : "brightred");
						CS$<>8__locals1.<>4__this.CGet<TextMeshProUGUI>("CostResourceChangeValue").SetText(valueStr, true);
						CS$<>8__locals1.<>4__this.CGet<GameObject>("CostResource").SetActive(needShow && change != 0);
						CS$<>8__locals1.<>4__this.UpdateShow();
					}
				});
				bool flag6 = !this._showOfferUpChange;
				if (flag6)
				{
					this.UpdateShow();
				}
			}
			else
			{
				base.CGet<GameObject>("MonthChange").SetActive(false);
				base.CGet<GameObject>("MaintainChange").SetActive(false);
				base.CGet<GameObject>("BuildingChange").SetActive(false);
				base.CGet<GameObject>("GatherChange").SetActive(false);
				base.CGet<GameObject>("JiaoPoolChange").SetActive(false);
				base.CGet<GameObject>("VillagerRoleChange").SetActive(false);
				base.CGet<GameObject>("CricketChange").SetActive(false);
				base.CGet<GameObject>("SectSupportChange").SetActive(false);
				base.CGet<GameObject>("CostResource").SetActive(false);
				base.CGet<GameObject>("Empty_3").SetActive(false);
			}
		}
	}

	// Token: 0x06002B1B RID: 11035 RVA: 0x0014EF20 File Offset: 0x0014D120
	public void RefreshAsExp(ArgumentBox argsBox)
	{
		string charName;
		argsBox.Get("CharName", out charName);
		int count;
		argsBox.Get("ResourceCount", out count);
		argsBox.Get("ShowDetailChange", out this._showDetailChange);
		argsBox.Get("ShowOfferUpChange", out this._showOfferUpChange);
		CRawImage resourceImg = base.CGet<CRawImage>("ResourceImg");
		base.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.GetFormat(LanguageKey.LK_Resource_Tips_Title, charName, LanguageKey.LK_Exp.Tr());
		base.CGet<CImage>("Icon").SetSprite("ui9_icon_resource_big_8", false, null);
		foreach (ItemSourceType sourceType in SingletonObject.getInstance<BuildingModel>().NeedItemSourceTypeList)
		{
			base.CGet<Refers>(sourceType.ToString()).gameObject.SetActive(false);
		}
		string countTitle = LocalStringManager.Get(LanguageKey.LK_Resource_NowOwn);
		string countFinialStr = count.ToString().SetColor("pinkyellow");
		base.CGet<TextMeshProUGUI>("Count").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_Resource_NowOwn, LanguageKey.LK_Exp.Tr());
		base.CGet<TextMeshProUGUI>("Value").text = countFinialStr;
		resourceImg.gameObject.SetActive(count > 0);
		bool flag = count > 0;
		if (flag)
		{
			CommonUtils.SetResourceImage(this._resourceType, count, resourceImg, 1f);
			Vector2 imgSize = resourceImg.rectTransform.sizeDelta;
			Vector2 backSize = new Vector2(470f, 181f);
			float scale = 1f;
			while (imgSize.x * scale > backSize.x - 10f || imgSize.y * scale > backSize.y - 10f)
			{
				scale -= 0.05f;
			}
			resourceImg.rectTransform.localScale = Vector3.one * scale;
		}
		base.CGet<GameObject>("MonthChange").SetActive(false);
		base.CGet<GameObject>("MaintainChange").SetActive(false);
		base.CGet<GameObject>("BuildingChange").SetActive(false);
		base.CGet<GameObject>("GatherChange").SetActive(false);
		base.CGet<GameObject>("JiaoPoolChange").SetActive(false);
		base.CGet<GameObject>("VillagerRoleChange").SetActive(false);
		base.CGet<GameObject>("CricketChange").SetActive(false);
		base.CGet<GameObject>("SectSupportChange").SetActive(false);
		base.CGet<GameObject>("CostResource").SetActive(false);
		base.CGet<GameObject>("Empty_3").SetActive(false);
	}

	// Token: 0x06002B1C RID: 11036 RVA: 0x0014F1E0 File Offset: 0x0014D3E0
	private void UpdateShow()
	{
		bool flag = !base.CGet<GameObject>("MaintainChange").activeSelf && !base.CGet<GameObject>("BuildingChange").activeSelf && !base.CGet<GameObject>("GatherChange").activeSelf && !base.CGet<GameObject>("JiaoPoolChange").activeSelf && !base.CGet<GameObject>("VillagerRoleChange").activeSelf && !base.CGet<GameObject>("CricketChange").activeSelf && !base.CGet<GameObject>("SectSupportChange").activeSelf;
		if (flag)
		{
			base.CGet<GameObject>("MonthChange").SetActive(false);
			base.CGet<GameObject>("Empty_3").SetActive(false);
		}
		else
		{
			base.CGet<GameObject>("MonthChange").SetActive(true);
			base.CGet<GameObject>("Empty_3").SetActive(true);
		}
	}

	// Token: 0x04001F1E RID: 7966
	private PoolItem _supportChangeItemPool;

	// Token: 0x04001F1F RID: 7967
	private bool _showDetailChange;

	// Token: 0x04001F20 RID: 7968
	private bool _showOfferUpChange;

	// Token: 0x04001F21 RID: 7969
	private sbyte _resourceType;

	// Token: 0x04001F22 RID: 7970
	private int _version;
}
