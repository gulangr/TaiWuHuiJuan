using System;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;

// Token: 0x020003E9 RID: 1001
public class MapElementCricket : MapElementBase
{
	// Token: 0x06003C3D RID: 15421 RVA: 0x001E6328 File Offset: 0x001E4528
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
					bool flag4 = MapElementBase.MapModel.CurrentBlockId == location.BlockId;
					if (flag4)
					{
						result = false;
					}
					else
					{
						bool settingState = MapElementBase.GlobalSettings.GetMapElementDisplayRuleItemState(36, true);
						bool flag5 = !settingState;
						if (flag5)
						{
							result = false;
						}
						else
						{
							CricketPlaceExtraData cricketPlaceExtraData;
							short num;
							bool flag6 = MapElementBase.MapModel.CricketPlaceExtraData.TryGetValue(location.AreaId, out cricketPlaceExtraData) && cricketPlaceExtraData != null && cricketPlaceExtraData.ExtraMapUnits != null && cricketPlaceExtraData.ExtraMapUnits.TryGetValue(location.BlockId, out num);
							if (flag6)
							{
								result = true;
							}
							else
							{
								CricketPlaceData cricketPlaceData = MapElementBase.MapModel.CricketPlaceData[(int)location.AreaId];
								bool flag7 = cricketPlaceData == null;
								if (flag7)
								{
									result = false;
								}
								else
								{
									for (int i = 0; i < cricketPlaceData.CricketBlocks.Length; i++)
									{
										bool flag8 = cricketPlaceData.CricketBlocks[i] == location.BlockId;
										if (flag8)
										{
											return !cricketPlaceData.CricketTriggered[i];
										}
									}
									result = false;
								}
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x17000620 RID: 1568
	// (get) Token: 0x06003C3E RID: 15422 RVA: 0x001E649C File Offset: 0x001E469C
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.Cricket;
		}
	}

	// Token: 0x06003C3F RID: 15423 RVA: 0x001E64A0 File Offset: 0x001E46A0
	public override void Scale(float wheel)
	{
		float cricketScale = Mathf.Pow(1f / wheel, 1.375f) * wheel;
		base.transform.localScale = Vector3.one * cricketScale;
	}

	// Token: 0x06003C40 RID: 15424 RVA: 0x001E64DC File Offset: 0x001E46DC
	protected override void OnCreate()
	{
		this.btnCricket.ClearAndAddListener(new Action(this.OnClickCricket));
		this.btnCricketExtra.ClearAndAddListener(new Action(this.OnClickCricket));
		GEvent.Add(UiEvents.OnSweepNetAmountTipsChanged, new GEvent.Callback(this.OnMouseTipChanged));
		this.ShowAnim(false);
	}

	// Token: 0x06003C41 RID: 15425 RVA: 0x001E6540 File Offset: 0x001E4740
	protected override void OnRefresh()
	{
		this.btnCricket.GetComponent<CInputEventImage>().IgnoreDrag = MapElementBase.MapModel.CurrentLocation.Equals(base.BlockLocation);
		this.btnCricketExtra.GetComponent<CInputEventImage>().IgnoreDrag = MapElementBase.MapModel.CurrentLocation.Equals(base.BlockLocation);
		short blockId;
		bool flag = MapElementBase.MapModel.CricketChangedInArea.TryGetValue(base.BlockLocation.BlockId, out blockId);
		if (flag)
		{
			this.JumpAuto(blockId);
		}
		else
		{
			this.PlayAuto();
		}
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003C42 RID: 15426 RVA: 0x001E65DE File Offset: 0x001E47DE
	protected override void OnCollect()
	{
		this.sing.StopLoopSing();
		GEvent.Remove(UiEvents.OnSweepNetAmountTipsChanged, new GEvent.Callback(this.OnMouseTipChanged));
	}

	// Token: 0x06003C43 RID: 15427 RVA: 0x001E660C File Offset: 0x001E480C
	private void OnClickCricket()
	{
		bool isMapMoving = MapElementBase.IsMapMoving;
		if (!isMapMoving)
		{
			GEvent.OnEvent(UiEvents.OnClickMapElement, EasyPool.Get<ArgumentBox>().Set<Location>("Location", base.BlockLocation));
		}
	}

	// Token: 0x06003C44 RID: 15428 RVA: 0x001E6647 File Offset: 0x001E4847
	private void PlayAuto()
	{
		this.ShowSing();
		this.ShowAnim(base.BlockData.Visible);
	}

	// Token: 0x06003C45 RID: 15429 RVA: 0x001E6664 File Offset: 0x001E4864
	private void JumpAuto(short blockId)
	{
		this.PlayAuto();
		Location location = new Location(base.BlockLocation.AreaId, blockId);
		Vector2 srcPos = this.PosGenerator(location);
		base.transform.localPosition = srcPos;
		Vector2 dstPos = this.PosGenerator(base.BlockLocation);
		base.transform.DOLocalJump(dstPos, 1f, 1, 0.6f, false);
	}

	// Token: 0x06003C46 RID: 15430 RVA: 0x001E66DC File Offset: 0x001E48DC
	private void ShowAnim(bool showed)
	{
		GameObject animObject = this.goAnim;
		GameObject animObjectExtra = this.goAnimExtra;
		CricketPlaceExtraData cricketPlaceExtraData;
		short time;
		bool flag = MapElementBase.MapModel.CricketPlaceExtraData.TryGetValue(base.BlockLocation.AreaId, out cricketPlaceExtraData) && cricketPlaceExtraData != null && cricketPlaceExtraData.ExtraMapUnits != null && cricketPlaceExtraData.ExtraMapUnits.TryGetValue(base.BlockLocation.BlockId, out time) && !cricketPlaceExtraData.IsRegularCricket(base.BlockLocation.BlockId);
		if (flag)
		{
			this.txtMeshTime.text = time.ToString();
			animObject.SetActive(false);
			animObjectExtra.SetActive(showed);
			bool isWishing = cricketPlaceExtraData.WishingCrickets != null && cricketPlaceExtraData.WishingCrickets.ContainsKey(base.BlockLocation.BlockId);
			this.animExtraDuke.SetActive(!isWishing);
			this.animExtraWishing.SetActive(isWishing);
		}
		else
		{
			animObject.SetActive(showed);
			animObjectExtra.SetActive(false);
		}
	}

	// Token: 0x06003C47 RID: 15431 RVA: 0x001E67D3 File Offset: 0x001E49D3
	private void ShowSing()
	{
		this.sing.SetCricketData(0, 0);
		this.sing.Sing(true, 10f);
	}

	// Token: 0x06003C48 RID: 15432 RVA: 0x001E67F8 File Offset: 0x001E49F8
	private void OnMouseTipChanged(ArgumentBox argbox)
	{
		string[] param;
		bool flag = argbox.Get<string[]>("tips", out param);
		if (flag)
		{
			this.tips.PresetParam = param;
		}
	}

	// Token: 0x04002B4E RID: 11086
	[SerializeField]
	private GameObject goAnim;

	// Token: 0x04002B4F RID: 11087
	[SerializeField]
	private TooltipInvoker tips;

	// Token: 0x04002B50 RID: 11088
	[SerializeField]
	private CricketSing sing;

	// Token: 0x04002B51 RID: 11089
	[SerializeField]
	private CButton btnCricket;

	// Token: 0x04002B52 RID: 11090
	[SerializeField]
	private GameObject goAnimExtra;

	// Token: 0x04002B53 RID: 11091
	[SerializeField]
	private CButton btnCricketExtra;

	// Token: 0x04002B54 RID: 11092
	[SerializeField]
	private TextMeshProUGUI txtMeshTime;

	// Token: 0x04002B55 RID: 11093
	[SerializeField]
	private GameObject animExtraDuke;

	// Token: 0x04002B56 RID: 11094
	[SerializeField]
	private GameObject animExtraWishing;
}
