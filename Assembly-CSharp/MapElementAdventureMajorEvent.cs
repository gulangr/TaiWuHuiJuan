using System;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using GameData.Domains.Adventure;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020003DF RID: 991
public class MapElementAdventureMajorEvent : MapElementBase
{
	// Token: 0x17000607 RID: 1543
	// (get) Token: 0x06003B9D RID: 15261 RVA: 0x001E3171 File Offset: 0x001E1371
	private static AdventureRemakeModel AdventureRemakeModel
	{
		get
		{
			return SingletonObject.getInstance<AdventureRemakeModel>();
		}
	}

	// Token: 0x06003B9E RID: 15262 RVA: 0x001E3178 File Offset: 0x001E1378
	public static bool CheckMaybeExist(Location location)
	{
		bool crossArchiveLockMoveTime = MapElementBase.MapModel.CrossArchiveLockMoveTime;
		bool result;
		if (crossArchiveLockMoveTime)
		{
			result = false;
		}
		else
		{
			bool flag = MapElementBase.MapModel.ViewMode == WorldMapModel.EViewMode.Info && MapElementBase.MapModel.CurrentAreaId == location.AreaId;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = location.AreaId != MapElementBase.MapModel.ShowingAreaId;
				if (flag2)
				{
					result = false;
				}
				else
				{
					MapBlockData blockData = MapElementBase.MapModel.GetBlockData(location);
					bool flag3 = blockData == null || !blockData.Visible;
					if (flag3)
					{
						result = false;
					}
					else
					{
						AdventureBlockCacheData adventureBlockCacheData = MapElementAdventureMajorEvent.AdventureRemakeModel.TryGetLocationAdventureRemake(location);
						bool flag4 = adventureBlockCacheData == null || !adventureBlockCacheData.AnyMajorEvent;
						if (flag4)
						{
							result = false;
						}
						else
						{
							AdventureMajorEvent majorEvent;
							bool flag5 = MapElementAdventureMajorEvent.AdventureRemakeModel.AdventureMajorEventDict.TryGetValue(adventureBlockCacheData.MajorEventId, out majorEvent);
							result = (flag5 && majorEvent.StatusType.IsActive());
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x17000608 RID: 1544
	// (get) Token: 0x06003B9F RID: 15263 RVA: 0x001E3266 File Offset: 0x001E1466
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.Adventure;
		}
	}

	// Token: 0x06003BA0 RID: 15264 RVA: 0x001E326C File Offset: 0x001E146C
	public override void Scale(float wheel)
	{
		float scale = Mathf.Sqrt(wheel + 0.73f) * 0.76f;
		this.root.SetGlobalScale(Vector3.one * (0.005f * scale));
	}

	// Token: 0x06003BA1 RID: 15265 RVA: 0x001E32AC File Offset: 0x001E14AC
	protected override void OnRefresh()
	{
		AdventureBlockCacheData adventureSiteData = MapElementAdventureMajorEvent.AdventureRemakeModel.TryGetLocationAdventureRemake(base.BlockLocation);
		this.RefreshBySite(adventureSiteData, base.BlockLocation);
	}

	// Token: 0x06003BA2 RID: 15266 RVA: 0x001E32D9 File Offset: 0x001E14D9
	protected override void OnCollect()
	{
		this._adventureBlockCacheData = null;
	}

	// Token: 0x06003BA3 RID: 15267 RVA: 0x001E32E4 File Offset: 0x001E14E4
	public void RefreshBySite(AdventureBlockCacheData adventureBlockCacheData, Location adventureLocation)
	{
		base.gameObject.SetActive(adventureBlockCacheData != null && adventureBlockCacheData.AnyMajorEvent);
		bool flag = adventureBlockCacheData == null;
		if (!flag)
		{
			this._adventureBlockCacheData = adventureBlockCacheData;
			CButton advBtn = this.btn;
			TooltipInvoker tips;
			bool flag2 = advBtn.TryGetComponent<TooltipInvoker>(out tips);
			if (flag2)
			{
				bool flag3 = adventureBlockCacheData.AnyMajorEvent && MapElementAdventureMajorEvent.AdventureRemakeModel.AdventureMajorEventDict.ContainsKey(adventureBlockCacheData.MajorEventId);
				if (flag3)
				{
					AdventureMajorEvent obj = MapElementAdventureMajorEvent.AdventureRemakeModel.AdventureMajorEventDict[adventureBlockCacheData.MajorEventId];
					AdventureMajorEventData data = AdventureRemakeModel.Core.GetAdventureMajorEventData(obj.CoreId);
					string title = (data.Name != null) ? ((data.Name != null) ? data.Name.ColorReplace() : "") : "";
					string desc = (data.Desc != null) ? ((data.Desc != null) ? data.Desc.ColorReplace() : "") : "";
					tips.PresetParam = new string[]
					{
						title,
						desc
					};
				}
			}
			advBtn.gameObject.SetActive(true);
			advBtn.ClearAndAddListener(new Action(this.OnClickAdventure));
			this.UpdateRemainingMonths();
		}
	}

	// Token: 0x06003BA4 RID: 15268 RVA: 0x001E3424 File Offset: 0x001E1624
	private void OnClickAdventure()
	{
		MapDomainMethod.AsyncCall.IsContinuousMovingBreak(null, delegate(int offset, RawDataPool dataPoll)
		{
			bool hasEvent = false;
			Serializer.Deserialize(dataPoll, offset, ref hasEvent);
			bool flag = hasEvent;
			if (!flag)
			{
				bool flag2 = this._adventureBlockCacheData == null;
				if (!flag2)
				{
					WorldMapModel mapModel = MapElementBase.MapModel;
					bool isMapMoving = MapElementBase.IsMapMoving;
					if (!isMapMoving)
					{
						bool flag3 = mapModel.CurrentAreaId != base.BlockLocation.AreaId || mapModel.CurrentBlockId != base.BlockLocation.BlockId;
						if (flag3)
						{
							GEvent.OnEvent(UiEvents.OnClickMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", base.BlockLocation));
						}
						else
						{
							ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
							argBox.Clear();
							argBox.Set("MajorEventId", this._adventureBlockCacheData.MajorEventId);
							GEvent.OnEvent(UiEvents.AdventureRemakeIconClick, argBox);
						}
					}
				}
			}
		});
	}

	// Token: 0x06003BA5 RID: 15269 RVA: 0x001E343C File Offset: 0x001E163C
	private void UpdateRemainingMonths()
	{
		AdventureRemakeModel model = MapElementAdventureMajorEvent.AdventureRemakeModel;
		bool flag = !model.AdventureMajorEventDict.Keys.Contains(this._adventureBlockCacheData.MajorEventId);
		if (!flag)
		{
			AdventureMajorEvent majorEvent = model.AdventureMajorEventDict[this._adventureBlockCacheData.MajorEventId];
			int remainingMonths = majorEvent.CalcRemainMonths(SingletonObject.getInstance<BasicGameData>().CurrDate);
			GameObject timeContainer = this.goTimeBg;
			timeContainer.SetActive(remainingMonths >= 0);
			bool activeSelf = timeContainer.activeSelf;
			if (activeSelf)
			{
				this.txtMeshTime.text = remainingMonths.ToString();
			}
		}
	}

	// Token: 0x04002AF0 RID: 10992
	private AdventureBlockCacheData _adventureBlockCacheData;

	// Token: 0x04002AF1 RID: 10993
	[SerializeField]
	private CButton btn;

	// Token: 0x04002AF2 RID: 10994
	[SerializeField]
	private RectTransform root;

	// Token: 0x04002AF3 RID: 10995
	[SerializeField]
	private TextMeshProUGUI txtMeshTime;

	// Token: 0x04002AF4 RID: 10996
	[SerializeField]
	private GameObject goTimeBg;
}
