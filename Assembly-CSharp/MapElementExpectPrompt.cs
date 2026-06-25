using System;
using System.Collections.Generic;
using DG.Tweening;
using GameData.Domains.Map;
using UnityEngine;

// Token: 0x020003ED RID: 1005
public class MapElementExpectPrompt : MapElementBase
{
	// Token: 0x06003C6D RID: 15469 RVA: 0x001E715C File Offset: 0x001E535C
	public static bool CheckMaybeExist(Location location)
	{
		bool flag = MapElementBase.MapModel.ViewMode != WorldMapModel.EViewMode.Info || location.AreaId != MapElementBase.MapModel.CurrentAreaId || MapElementBase.MapModel.CurrentAreaBlockDisplayData == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			MapBlockData blockData = MapElementBase.MapModel.GetBlockData(location);
			bool flag2 = blockData == null || !blockData.Visible;
			if (flag2)
			{
				result = false;
			}
			else
			{
				MapBlockDisplayData? nullableDisplayData = MapElementBase.MapModel.TryGetViewModeData(location);
				bool flag3 = nullableDisplayData == null;
				if (flag3)
				{
					result = false;
				}
				else
				{
					MapBlockDisplayData displayData = nullableDisplayData.Value;
					GlobalSettings globalSettings = SingletonObject.getInstance<GlobalSettings>();
					int professionId = SingletonObject.getInstance<ProfessionModel>().TaiwuCurrProfessionId;
					bool flag4 = globalSettings.ShowTreasure && displayData.TreasureExpect.MaxGrade >= 3;
					if (flag4)
					{
						result = true;
					}
					else
					{
						bool flag5 = displayData.Count0 > 0 && BlockPromptCount.ParseEnabled(professionId, 0);
						if (flag5)
						{
							result = true;
						}
						else
						{
							bool flag6 = displayData.Count1 > 0 && BlockPromptCount.ParseEnabled(professionId, 1);
							if (flag6)
							{
								result = true;
							}
							else
							{
								bool flag7 = displayData.Count2 > 0 && BlockPromptCount.ParseEnabled(professionId, 2);
								result = flag7;
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x17000624 RID: 1572
	// (get) Token: 0x06003C6E RID: 15470 RVA: 0x001E7299 File Offset: 0x001E5499
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.ExpectPrompt;
		}
	}

	// Token: 0x06003C6F RID: 15471 RVA: 0x001E729C File Offset: 0x001E549C
	public override void Scale(float wheel)
	{
	}

	// Token: 0x06003C70 RID: 15472 RVA: 0x001E72A0 File Offset: 0x001E54A0
	protected override void OnRefresh()
	{
		MapBlockDisplayData? nullableDisplayData = MapElementBase.MapModel.TryGetViewModeData(base.BlockLocation);
		bool flag = nullableDisplayData == null;
		if (flag)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			this.RefreshByDisplayData(nullableDisplayData.Value);
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x06003C71 RID: 15473 RVA: 0x001E72FC File Offset: 0x001E54FC
	protected override void OnCollect()
	{
		foreach (GameObject go in this._waitingToDestroy)
		{
			Object.Destroy(go);
		}
		this._waitingToDestroy.Clear();
	}

	// Token: 0x06003C72 RID: 15474 RVA: 0x001E7360 File Offset: 0x001E5560
	private void RefreshByDisplayData(MapBlockDisplayData displayData)
	{
		int professionId = displayData.ProfessionId;
		if (!true)
		{
		}
		MapElementExpectPrompt.EProfessionPrompt eprofessionPrompt;
		if (professionId <= 1)
		{
			if (professionId == 0)
			{
				eprofessionPrompt = MapElementExpectPrompt.EProfessionPrompt.OnlyIcon;
				goto IL_6B;
			}
			if (professionId == 1)
			{
				eprofessionPrompt = MapElementExpectPrompt.EProfessionPrompt.CountBigIcon;
				goto IL_6B;
			}
		}
		else
		{
			if (professionId == 5)
			{
				eprofessionPrompt = MapElementExpectPrompt.EProfessionPrompt.LayoutCount;
				goto IL_6B;
			}
			switch (professionId)
			{
			case 10:
				eprofessionPrompt = MapElementExpectPrompt.EProfessionPrompt.CountBigIcon;
				goto IL_6B;
			case 11:
				break;
			case 12:
				eprofessionPrompt = MapElementExpectPrompt.EProfessionPrompt.LayoutCount;
				goto IL_6B;
			case 13:
				eprofessionPrompt = MapElementExpectPrompt.EProfessionPrompt.LayoutCount;
				goto IL_6B;
			default:
				if (professionId == 17)
				{
					eprofessionPrompt = MapElementExpectPrompt.EProfessionPrompt.LayoutCount;
					goto IL_6B;
				}
				break;
			}
		}
		eprofessionPrompt = MapElementExpectPrompt.EProfessionPrompt.None;
		IL_6B:
		if (!true)
		{
		}
		MapElementExpectPrompt.EProfessionPrompt type = eprofessionPrompt;
		bool professionPrompt = false;
		bool flag = type == MapElementExpectPrompt.EProfessionPrompt.OnlyIcon;
		if (flag)
		{
			professionPrompt = (displayData.Count0 > 0 && BlockPromptCount.ParseEnabled(professionId, 0));
		}
		else
		{
			bool flag2 = type == MapElementExpectPrompt.EProfessionPrompt.CountBigIcon;
			if (flag2)
			{
				professionPrompt = this.blockPromptCountBigIcon.RefreshWithActive(professionId, 0, displayData.Count0);
			}
			else
			{
				bool flag3 = type == MapElementExpectPrompt.EProfessionPrompt.LayoutCount;
				if (flag3)
				{
					bool hasCount0 = this.blockPromptCount0.RefreshWithActive(professionId, 0, displayData.Count0);
					bool hasCount = this.blockPromptCount1.RefreshWithActive(professionId, 1, displayData.Count1);
					bool hasCount2 = this.blockPromptCount2.RefreshWithActive(professionId, 2, displayData.Count2);
					professionPrompt = (hasCount0 || hasCount || hasCount2);
					bool flag4 = professionPrompt;
					if (flag4)
					{
						this.pyramidLayout.UpdateChild();
					}
				}
			}
		}
		bool flag5 = !professionPrompt;
		if (flag5)
		{
			type = MapElementExpectPrompt.EProfessionPrompt.None;
		}
		this.imgCountOnlyIcon.gameObject.SetActive(type == MapElementExpectPrompt.EProfessionPrompt.OnlyIcon);
		this.blockPromptCountBigIcon.gameObject.SetActive(type == MapElementExpectPrompt.EProfessionPrompt.CountBigIcon);
		this.pyramidLayout.gameObject.SetActive(type == MapElementExpectPrompt.EProfessionPrompt.LayoutCount);
		RectTransform treasurePrompt = this.rectTsTreasurePrompt;
		treasurePrompt.gameObject.SetActive(type == MapElementExpectPrompt.EProfessionPrompt.None);
		bool flag6 = type != MapElementExpectPrompt.EProfessionPrompt.None || this._waitingToDestroy.Count > 0;
		if (!flag6)
		{
			GameObject effect = Object.Instantiate<GameObject>((displayData.TreasureExpect.MaxGrade >= 6) ? this.goEffectTreasureGood : this.goEffectTreasureNormal, treasurePrompt, false);
			this._waitingToDestroy.Add(effect);
			treasurePrompt.localScale = Vector3.zero;
			treasurePrompt.DOScale(Vector3.one, 0.6f);
		}
	}

	// Token: 0x04002B68 RID: 11112
	[SerializeField]
	private RectTransform rectTsTreasurePrompt;

	// Token: 0x04002B69 RID: 11113
	[SerializeField]
	private GameObject goEffectTreasureGood;

	// Token: 0x04002B6A RID: 11114
	[SerializeField]
	private GameObject goEffectTreasureNormal;

	// Token: 0x04002B6B RID: 11115
	[SerializeField]
	private CImage imgCountOnlyIcon;

	// Token: 0x04002B6C RID: 11116
	[SerializeField]
	private BlockPromptCount blockPromptCount0;

	// Token: 0x04002B6D RID: 11117
	[SerializeField]
	private BlockPromptCount blockPromptCount1;

	// Token: 0x04002B6E RID: 11118
	[SerializeField]
	private BlockPromptCount blockPromptCount2;

	// Token: 0x04002B6F RID: 11119
	[SerializeField]
	private BlockPromptCount blockPromptCountBigIcon;

	// Token: 0x04002B70 RID: 11120
	[SerializeField]
	private PyramidLayout pyramidLayout;

	// Token: 0x04002B71 RID: 11121
	[SerializeField]
	private RectTransform rectTsProfessionCounts;

	// Token: 0x04002B72 RID: 11122
	private readonly List<GameObject> _waitingToDestroy = new List<GameObject>();

	// Token: 0x02001879 RID: 6265
	private enum EProfessionPrompt
	{
		// Token: 0x0400AEAB RID: 44715
		None,
		// Token: 0x0400AEAC RID: 44716
		OnlyIcon,
		// Token: 0x0400AEAD RID: 44717
		CountBigIcon,
		// Token: 0x0400AEAE RID: 44718
		LayoutCount
	}
}
