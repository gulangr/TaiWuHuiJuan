using System;
using DG.Tweening;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x020003F6 RID: 1014
public class MapElementZhujianThief : MapElementBase
{
	// Token: 0x06003CF2 RID: 15602 RVA: 0x001EA8C8 File Offset: 0x001E8AC8
	public static bool CheckMaybeExist(Location location)
	{
		bool flag = MapElementBase.MapModel.ViewMode == WorldMapModel.EViewMode.Info && MapElementBase.MapModel.CurrentAreaId == location.AreaId;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = MapElementBase.MapModel.ShowingAreaId != location.AreaId;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = MapElementBase.MapModel.CurrentAreaId != location.AreaId;
				if (flag3)
				{
					result = false;
				}
				else
				{
					bool flag4 = MapElementBase.MapModel.SectZhujianThiefList == null;
					if (flag4)
					{
						result = false;
					}
					else
					{
						foreach (SectStoryThiefData data in MapElementBase.MapModel.SectZhujianThiefList)
						{
							bool flag5 = data.AreaId != location.AreaId;
							if (!flag5)
							{
								int index = data.ThiefBlockIds.IndexOf(location.BlockId);
								bool flag6 = index < 0;
								if (!flag6)
								{
									bool flag7 = data.ThiefTriggered[index];
									if (!flag7)
									{
										return true;
									}
								}
							}
						}
						result = false;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x17000633 RID: 1587
	// (get) Token: 0x06003CF3 RID: 15603 RVA: 0x001EAA00 File Offset: 0x001E8C00
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.Cricket;
		}
	}

	// Token: 0x06003CF4 RID: 15604 RVA: 0x001EAA04 File Offset: 0x001E8C04
	public override void Scale(float wheel)
	{
		float cricketScale = Mathf.Pow(1f / wheel, 1.375f) * wheel;
		base.transform.localScale = Vector3.one * cricketScale;
	}

	// Token: 0x06003CF5 RID: 15605 RVA: 0x001EAA3D File Offset: 0x001E8C3D
	protected override void OnCreate()
	{
		this.btnThief.ClearAndAddListener(new Action(this.OnClickThief));
	}

	// Token: 0x06003CF6 RID: 15606 RVA: 0x001EAA58 File Offset: 0x001E8C58
	protected override void OnRefresh()
	{
		short blockId;
		bool flag = MapElementBase.MapModel.ThiefChangedInArea.TryGetValue(base.BlockLocation.BlockId, out blockId);
		if (flag)
		{
			this.JumpAuto(blockId);
		}
		else
		{
			this.PlayAuto();
		}
		base.gameObject.SetActive(true);
		this.btnThief.GetComponent<CInputEventImage>().IgnoreDrag = MapElementBase.MapModel.CurrentLocation.Equals(base.BlockLocation);
	}

	// Token: 0x06003CF7 RID: 15607 RVA: 0x001EAACD File Offset: 0x001E8CCD
	protected override void OnCollect()
	{
		this.sing.StopLoopSing();
	}

	// Token: 0x06003CF8 RID: 15608 RVA: 0x001EAADC File Offset: 0x001E8CDC
	private void PlayAuto()
	{
		this.ShowSing();
		this.btnThief.gameObject.SetActive(base.BlockData.Visible);
	}

	// Token: 0x06003CF9 RID: 15609 RVA: 0x001EAB04 File Offset: 0x001E8D04
	private void JumpAuto(short blockId)
	{
		this.PlayAuto();
		Location location = new Location(base.BlockLocation.AreaId, blockId);
		Vector2 srcPos = this.PosGenerator(location);
		base.transform.localPosition = srcPos;
		Vector2 dstPos = this.PosGenerator(base.BlockLocation);
		base.transform.DOLocalJump(dstPos, 1f, 1, 0.6f, false);
	}

	// Token: 0x06003CFA RID: 15610 RVA: 0x001EAB7B File Offset: 0x001E8D7B
	private void ShowSing()
	{
		this.sing.SetCricketData(0, 0);
		this.sing.Sing(true, 10f);
	}

	// Token: 0x06003CFB RID: 15611 RVA: 0x001EABA0 File Offset: 0x001E8DA0
	private void OnClickThief()
	{
		bool isMapMoving = MapElementBase.IsMapMoving;
		if (!isMapMoving)
		{
			bool flag = MapElementBase.MapModel.CurrentLocation != base.BlockLocation;
			if (flag)
			{
				GEvent.OnEvent(UiEvents.OnClickMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", base.BlockLocation));
			}
			else
			{
				CommandManager.AddCommandMethodCall(EPriority.CallMethodNormal, 20, 15, new CallMethodRespHandler(this.HandlerTryTriggerThiefCatch), null);
			}
		}
	}

	// Token: 0x06003CFC RID: 15612 RVA: 0x001EAC10 File Offset: 0x001E8E10
	private void HandlerTryTriggerThiefCatch(int offset, RawDataPool pool)
	{
		int catchThiefTimes = 0;
		Serializer.Deserialize(pool, offset, ref catchThiefTimes);
		bool flag = catchThiefTimes < 0;
		if (!flag)
		{
			UIElement.CatchThief.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CatchThiefTimes", catchThiefTimes));
			UIManager.Instance.ShowUI(UIElement.CatchThief, true);
		}
	}

	// Token: 0x04002BAB RID: 11179
	[SerializeField]
	private CButton btnThief;

	// Token: 0x04002BAC RID: 11180
	[SerializeField]
	private CricketSing sing;
}
