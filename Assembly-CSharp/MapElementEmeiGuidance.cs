using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using GameData.Domains.Story;
using GameData.Domains.Story.SectMainStory;
using TMPro;
using UnityEngine;

// Token: 0x020003EC RID: 1004
public class MapElementEmeiGuidance : MapElementBase
{
	// Token: 0x06003C62 RID: 15458 RVA: 0x001E6D0C File Offset: 0x001E4F0C
	public static bool CheckMaybeExist(Location location)
	{
		bool flag = MapElementBase.MapModel.SectEmeiGuidanceData == null || MapElementBase.MapModel.SectEmeiGuidanceData.Count == 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = MapElementBase.MapModel.ShowingAreaId != location.AreaId;
			result = (!flag2 && MapElementBase.MapModel.SectEmeiGuidanceData.ContainsKey(location));
		}
		return result;
	}

	// Token: 0x17000623 RID: 1571
	// (get) Token: 0x06003C63 RID: 15459 RVA: 0x001E6D74 File Offset: 0x001E4F74
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.SpecialCharacter;
		}
	}

	// Token: 0x06003C64 RID: 15460 RVA: 0x001E6D78 File Offset: 0x001E4F78
	public override void Scale(float wheel)
	{
		float scale = Mathf.Pow(1f / wheel, 1.6f) * wheel;
		base.transform.localScale = Vector3.one * scale;
	}

	// Token: 0x06003C65 RID: 15461 RVA: 0x001E6DB4 File Offset: 0x001E4FB4
	protected override void OnCreate()
	{
		this.btn.ClearAndAddListener(new Action(this.OnClickButton));
		this.btnLeft.ClearAndAddListener(new Action(this.OnClickLeft));
		this.btnRight.ClearAndAddListener(new Action(this.OnClickRight));
	}

	// Token: 0x06003C66 RID: 15462 RVA: 0x001E6E0C File Offset: 0x001E500C
	protected override void OnRefresh()
	{
		List<SectEmeiGuidanceMapData> list = MapElementBase.MapModel.SectEmeiGuidanceData[base.BlockLocation];
		bool flag = list.Count == 1;
		if (flag)
		{
			this.btnLeft.gameObject.SetActive(false);
			this.btnRight.gameObject.SetActive(false);
		}
		else
		{
			this.btnLeft.gameObject.SetActive(true);
			this.btnRight.gameObject.SetActive(true);
		}
		this._index = 0;
		this.Refresh();
	}

	// Token: 0x06003C67 RID: 15463 RVA: 0x001E6E99 File Offset: 0x001E5099
	protected override void OnCollect()
	{
	}

	// Token: 0x06003C68 RID: 15464 RVA: 0x001E6E9C File Offset: 0x001E509C
	private void OnClickButton()
	{
		Dictionary<Location, List<SectEmeiGuidanceMapData>> sectEmeiGuidanceData = MapElementBase.MapModel.SectEmeiGuidanceData;
		bool flag = ((sectEmeiGuidanceData != null) ? sectEmeiGuidanceData[base.BlockLocation] : null) == null;
		if (!flag)
		{
			bool flag2 = MapElementBase.MapModel.SectEmeiGuidanceData[base.BlockLocation].Count == 0;
			if (!flag2)
			{
				StoryDomainMethod.Call.OnClickEmeiGuidance(MapElementBase.MapModel.SectEmeiGuidanceData[base.BlockLocation][this._index].Data.CharId);
			}
		}
	}

	// Token: 0x06003C69 RID: 15465 RVA: 0x001E6F24 File Offset: 0x001E5124
	private void OnClickLeft()
	{
		bool flag = this._index == 0;
		if (!flag)
		{
			this._index--;
			this.Refresh();
		}
	}

	// Token: 0x06003C6A RID: 15466 RVA: 0x001E6F58 File Offset: 0x001E5158
	private void OnClickRight()
	{
		Dictionary<Location, List<SectEmeiGuidanceMapData>> sectEmeiGuidanceData = MapElementBase.MapModel.SectEmeiGuidanceData;
		bool flag = ((sectEmeiGuidanceData != null) ? sectEmeiGuidanceData[base.BlockLocation] : null) == null;
		if (!flag)
		{
			bool flag2 = MapElementBase.MapModel.SectEmeiGuidanceData[base.BlockLocation].Count == 1;
			if (!flag2)
			{
				bool flag3 = this._index == MapElementBase.MapModel.SectEmeiGuidanceData[base.BlockLocation].Count - 1;
				if (!flag3)
				{
					this._index++;
					this.Refresh();
				}
			}
		}
	}

	// Token: 0x06003C6B RID: 15467 RVA: 0x001E6FF0 File Offset: 0x001E51F0
	private void Refresh()
	{
		Dictionary<Location, List<SectEmeiGuidanceMapData>> sectEmeiGuidanceData = MapElementBase.MapModel.SectEmeiGuidanceData;
		bool flag = ((sectEmeiGuidanceData != null) ? sectEmeiGuidanceData[base.BlockLocation] : null) == null;
		if (!flag)
		{
			List<SectEmeiGuidanceMapData> list = MapElementBase.MapModel.SectEmeiGuidanceData[base.BlockLocation];
			int count = list.Count;
			bool flag2 = this._index >= count;
			if (flag2)
			{
				this._index = 0;
			}
			this.btnLeft.interactable = (this._index != 0);
			this.btnRight.interactable = (this._index != list.Count - 1);
			SectEmeiGuidanceMapData data = list[this._index];
			this.icon.SetSprite(string.Format("{0}{1}", "sp_icon_combatskill_2_", data.CombatSkillType), false, null);
			this.icon.SetColor(CommonUtils.GetFiveElementColor(1));
			this.textLabel.text = ((data.Data.Point == 0) ? NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, false, false) : string.Format("{0} + {1}", NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, false, false), data.Data.Point));
			this.btn.interactable = base.BlockLocation.Equals(SingletonObject.getInstance<WorldMapModel>().CurrentLocation);
		}
	}

	// Token: 0x04002B61 RID: 11105
	[SerializeField]
	private CImage icon;

	// Token: 0x04002B62 RID: 11106
	[SerializeField]
	private TooltipInvoker tips;

	// Token: 0x04002B63 RID: 11107
	[SerializeField]
	private TextMeshProUGUI textLabel;

	// Token: 0x04002B64 RID: 11108
	[SerializeField]
	private CButton btn;

	// Token: 0x04002B65 RID: 11109
	[SerializeField]
	private CButton btnLeft;

	// Token: 0x04002B66 RID: 11110
	[SerializeField]
	private CButton btnRight;

	// Token: 0x04002B67 RID: 11111
	private int _index = 0;
}
