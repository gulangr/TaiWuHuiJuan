using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x020003EE RID: 1006
public class MapElementFulongFlame : MapElementBase
{
	// Token: 0x06003C74 RID: 15476 RVA: 0x001E7570 File Offset: 0x001E5770
	public static bool CheckMaybeExist(Location location)
	{
		bool flag = MapElementBase.MapModel.SectFulongInFlameAreas == null;
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
				foreach (FulongInFlameArea area in MapElementBase.MapModel.SectFulongInFlameAreas)
				{
					bool flag3 = area.AreaId == location.AreaId && area.LightedBlocks.ContainsKey(location.BlockId);
					if (flag3)
					{
						return true;
					}
				}
				result = false;
			}
		}
		return result;
	}

	// Token: 0x17000625 RID: 1573
	// (get) Token: 0x06003C75 RID: 15477 RVA: 0x001E7630 File Offset: 0x001E5830
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.FulongFlame;
		}
	}

	// Token: 0x06003C76 RID: 15478 RVA: 0x001E7634 File Offset: 0x001E5834
	public override void Scale(float wheel)
	{
		float scale = Mathf.Pow(1f / wheel, 1.6f) * wheel;
		base.transform.localScale = Vector3.one * scale;
	}

	// Token: 0x06003C77 RID: 15479 RVA: 0x001E7670 File Offset: 0x001E5870
	protected override void OnCreate()
	{
		this.btn.ClearAndAddListener(new Action(this.OnClickFulongInFlameAreaBlock));
		GEvent.Add(EEvents.OnActionPointChange, new GEvent.Callback(this.OnActionPointChanged));
		GEvent.Add(UiEvents.WorldMapPlayerBlockChange, new GEvent.Callback(this.OnTaiwuCurrLocationChanged));
		this._prevBlockId = MapElementBase.MapModel.CurrentBlockId;
	}

	// Token: 0x06003C78 RID: 15480 RVA: 0x001E76D8 File Offset: 0x001E58D8
	protected override void OnRefresh()
	{
		this.btn.GetComponent<CInputEventImage>().IgnoreDrag = MapElementBase.MapModel.CurrentLocation.Equals(base.BlockLocation);
		this.Refresh();
	}

	// Token: 0x06003C79 RID: 15481 RVA: 0x001E7716 File Offset: 0x001E5916
	protected override void OnCollect()
	{
		GEvent.Remove(EEvents.OnActionPointChange, new GEvent.Callback(this.OnActionPointChanged));
		GEvent.Remove(UiEvents.WorldMapPlayerBlockChange, new GEvent.Callback(this.OnTaiwuCurrLocationChanged));
	}

	// Token: 0x06003C7A RID: 15482 RVA: 0x001E774C File Offset: 0x001E594C
	public void OnEnter()
	{
		bool flag = base.BlockLocation.BlockId != MapElementBase.MapModel.CurrentBlockId;
		if (!flag)
		{
			this.btn.interactable = (SingletonObject.getInstance<TimeManager>().IsActionDayEnough(GlobalConfig.Instance.FulongFlameExtinguishCost) && !SingletonObject.getInstance<WorldMapModel>().IsTaiwuGroupGetMaxLevelInjuries);
			this.goFire.SetActive(false);
			bool flag2 = SingletonObject.getInstance<TimeManager>().IsActionDayEnough(GlobalConfig.Instance.FulongFlameExtinguishCost) && !SingletonObject.getInstance<WorldMapModel>().IsTaiwuGroupGetMaxLevelInjuries;
			if (flag2)
			{
				this.goExtinguish.SetActive(true);
				this.btn.GetComponent<PointerScaleAnim>().ScaleToTarget();
			}
			else
			{
				this.goExtinguishDisabled.SetActive(true);
			}
		}
	}

	// Token: 0x06003C7B RID: 15483 RVA: 0x001E7817 File Offset: 0x001E5A17
	public void OnExit()
	{
		this.goFire.SetActive(true);
		this.goExtinguish.SetActive(false);
		this.goExtinguishDisabled.SetActive(false);
		this.btn.GetComponent<PointerScaleAnim>().ScaleBack();
	}

	// Token: 0x06003C7C RID: 15484 RVA: 0x001E7854 File Offset: 0x001E5A54
	private void OnTaiwuCurrLocationChanged(ArgumentBox _)
	{
		bool flag = base.BlockLocation.BlockId == this._prevBlockId || base.BlockLocation.BlockId == MapElementBase.MapModel.CurrentBlockId;
		if (flag)
		{
			this.Refresh();
		}
		this._prevBlockId = MapElementBase.MapModel.CurrentBlockId;
	}

	// Token: 0x06003C7D RID: 15485 RVA: 0x001E78AC File Offset: 0x001E5AAC
	private void OnActionPointChanged(ArgumentBox _)
	{
		bool flag = base.BlockLocation.BlockId == MapElementBase.MapModel.CurrentBlockId;
		if (flag)
		{
			this.Refresh();
		}
	}

	// Token: 0x06003C7E RID: 15486 RVA: 0x001E78DC File Offset: 0x001E5ADC
	private void OnClickFulongInFlameAreaBlock()
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
				SingletonObject.getInstance<EventModel>().SetLockInputState(true);
				foreach (FulongInFlameArea area in MapElementBase.MapModel.SectFulongInFlameAreas)
				{
					bool flag2 = area.MineBlocks.Contains(base.BlockLocation.BlockId);
					if (flag2)
					{
						ExtraDomainMethod.Call.TriggerFulongInFlameAreaMine(base.BlockLocation.BlockId);
						AudioManager.Instance.PlaySound("SFX_fulongfire_blast", false, false);
						this.goExplodeEffect.SetActive(true);
						this.goExplode.SetActive(true);
						SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(3f, delegate
						{
							this.goExplodeEffect.SetActive(false);
						});
						return;
					}
				}
				AudioManager.Instance.PlaySound("SFX_fulongfire_miehuo", false, false);
				WorldDomainMethod.Call.AdvanceDaysInMonth(GlobalConfig.Instance.FulongFlameExtinguishCost);
				ExtraDomainMethod.AsyncCall.ExtinguishFulongInFlameArea(null, base.BlockLocation.BlockId, new AsyncMethodCallbackDelegate(this.OnBlockExtinguished));
			}
		}
	}

	// Token: 0x06003C7F RID: 15487 RVA: 0x001E7A4C File Offset: 0x001E5C4C
	private void OnBlockExtinguished(int offset, RawDataPool dataPool)
	{
		int res = 0;
		Serializer.Deserialize(dataPool, offset, ref res);
		bool flag = res >= 0;
		if (flag)
		{
			AudioManager.Instance.PlaySound("SFX_fulongfire_end", false, false);
			ExtraDomainMethod.Call.ApplyFulongInFlameAreaFullyExtinguished(res);
		}
	}

	// Token: 0x06003C80 RID: 15488 RVA: 0x001E7A8C File Offset: 0x001E5C8C
	private void Refresh()
	{
		foreach (FulongInFlameArea area in MapElementBase.MapModel.SectFulongInFlameAreas)
		{
			int mineCount;
			bool flag = base.BlockLocation.AreaId != area.AreaId || !area.LightedBlocks.TryGetValue(base.BlockLocation.BlockId, out mineCount);
			if (!flag)
			{
				bool flag2 = base.BlockLocation.BlockId == MapElementBase.MapModel.CurrentBlockId;
				if (flag2)
				{
					bool flag3 = mineCount > 0;
					if (flag3)
					{
						this.imgCount.SetSprite(string.Format("sp_number_2_{0}", mineCount), false, null);
						this.goCountBg.SetActive(true);
					}
					else
					{
						this.goCountBg.SetActive(false);
					}
					bool flag4 = area.TriggeredMineBlocks.Contains(base.BlockLocation.BlockId) || area.ExtinguishedBlocks.Contains(base.BlockLocation.BlockId);
					if (flag4)
					{
						AudioManager.Instance.StopSound("SFX_fulongfire_loop");
					}
					else
					{
						bool flag5 = !AudioManager.Instance.IsPlayingSound("SFX_fulongfire_loop");
						if (flag5)
						{
							AudioManager.Instance.PlaySound("SFX_fulongfire_loop", true, false);
						}
					}
				}
				else
				{
					this.goCountBg.SetActive(false);
				}
				bool flag6 = !base.BlockData.Visible || area.TriggeredMineBlocks.Contains(base.BlockLocation.BlockId) || area.ExtinguishedBlocks.Contains(base.BlockLocation.BlockId);
				if (flag6)
				{
					this.btn.gameObject.SetActive(false);
				}
				else
				{
					this.btn.gameObject.SetActive(true);
					this.btn.interactable = (base.BlockLocation.BlockId != MapElementBase.MapModel.CurrentBlockId || (SingletonObject.getInstance<TimeManager>().IsActionDayEnough(GlobalConfig.Instance.FulongFlameExtinguishCost) && !SingletonObject.getInstance<WorldMapModel>().IsTaiwuGroupGetMaxLevelInjuries));
				}
			}
		}
	}

	// Token: 0x04002B73 RID: 11123
	private short _prevBlockId = -1;

	// Token: 0x04002B74 RID: 11124
	[SerializeField]
	private CButton btn;

	// Token: 0x04002B75 RID: 11125
	[SerializeField]
	private CImage imgCount;

	// Token: 0x04002B76 RID: 11126
	[SerializeField]
	private GameObject goFire;

	// Token: 0x04002B77 RID: 11127
	[SerializeField]
	private GameObject goExtinguish;

	// Token: 0x04002B78 RID: 11128
	[SerializeField]
	private GameObject goExtinguishDisabled;

	// Token: 0x04002B79 RID: 11129
	[SerializeField]
	private GameObject goExplode;

	// Token: 0x04002B7A RID: 11130
	[SerializeField]
	private GameObject goExplodeEffect;

	// Token: 0x04002B7B RID: 11131
	[SerializeField]
	private GameObject goCountBg;
}
