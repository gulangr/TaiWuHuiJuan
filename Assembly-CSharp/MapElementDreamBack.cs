using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using UnityEngine;

// Token: 0x020003EB RID: 1003
public class MapElementDreamBack : MapElementBase
{
	// Token: 0x06003C54 RID: 15444 RVA: 0x001E6A0C File Offset: 0x001E4C0C
	public static bool CheckMaybeExist(Location location)
	{
		bool flag = MapElementBase.MapModel.ShowingAreaId != location.AreaId;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			List<DreamBackLocationData> dreamBackLocationData = MapElementBase.MapModel.DreamBackLocationData;
			result = (dreamBackLocationData != null && dreamBackLocationData.Any((DreamBackLocationData x) => x.Location == location));
		}
		return result;
	}

	// Token: 0x17000622 RID: 1570
	// (get) Token: 0x06003C55 RID: 15445 RVA: 0x001E6A6F File Offset: 0x001E4C6F
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.DreamBack;
		}
	}

	// Token: 0x06003C56 RID: 15446 RVA: 0x001E6A74 File Offset: 0x001E4C74
	public override void Scale(float wheel)
	{
		float scale = Mathf.Pow(1f / wheel, 1.6f) * wheel;
		base.transform.localScale = Vector3.one * scale;
	}

	// Token: 0x06003C57 RID: 15447 RVA: 0x001E6AAD File Offset: 0x001E4CAD
	protected override void OnCreate()
	{
		this.btnDreamBack.ClearAndAddListener(new Action(this.OnClickDreamBack));
		GEvent.Add(UiEvents.OnDreamBackStatusChanged, new GEvent.Callback(this.OnDreamBackStatusChanged));
	}

	// Token: 0x06003C58 RID: 15448 RVA: 0x001E6AE4 File Offset: 0x001E4CE4
	protected override void OnRefresh()
	{
		this.btnDreamBack.GetComponent<CInputEventImage>().IgnoreDrag = MapElementBase.MapModel.CurrentLocation.Equals(base.BlockLocation);
		DreamBackLocationData locationData = MapElementBase.MapModel.DreamBackLocationData.First((DreamBackLocationData x) => x.Location == base.BlockLocation);
		this.imgIcon.SetSprite(MapElementDreamBack.DreamBackLocationType2Sprites[(int)locationData.Type], false, null);
		this.UpdateInteract();
	}

	// Token: 0x06003C59 RID: 15449 RVA: 0x001E6B59 File Offset: 0x001E4D59
	protected override void OnCollect()
	{
		GEvent.Remove(UiEvents.OnDreamBackStatusChanged, new GEvent.Callback(this.OnDreamBackStatusChanged));
	}

	// Token: 0x06003C5A RID: 15450 RVA: 0x001E6B78 File Offset: 0x001E4D78
	private void OnDreamBackStatusChanged(ArgumentBox _)
	{
		this.UpdateInteract();
	}

	// Token: 0x06003C5B RID: 15451 RVA: 0x001E6B84 File Offset: 0x001E4D84
	private void OnClickDreamBack()
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
				MapDomainMethod.Call.RetrieveDreamBackLocation(base.BlockLocation);
			}
		}
	}

	// Token: 0x06003C5C RID: 15452 RVA: 0x001E6BF4 File Offset: 0x001E4DF4
	private void UpdateInteract()
	{
		bool canInteract = !MapElementBase.MapModel.CrossArchiveLockMoveTime;
		CButton btn = this.btnDreamBack;
		btn.interactable = canInteract;
		btn.GetComponent<PointerScaleAnim>().enabled = canInteract;
		this.UpdateMouseTipDisplayer(canInteract);
	}

	// Token: 0x06003C5D RID: 15453 RVA: 0x001E6C34 File Offset: 0x001E4E34
	private void UpdateMouseTipDisplayer(bool isShow)
	{
		this.mouseTipDisplayer.enabled = isShow;
		DreamBackLocationData locationData = MapElementBase.MapModel.DreamBackLocationData.First((DreamBackLocationData x) => x.Location == base.BlockLocation);
		this.mouseTipDisplayer.PresetParam[0] = LocalStringManager.Get(MapElementDreamBack.DreamBackLocationType2TipsKey[(int)((ushort)locationData.Type)]);
		this.mouseTipDisplayer.PresetParam[1] = LocalStringManager.Get(LanguageKey.LK_MapAreaMousetip_DreamBack_Tips_Content);
	}

	// Token: 0x06003C5F RID: 15455 RVA: 0x001E6CAB File Offset: 0x001E4EAB
	// Note: this type is marked as 'beforefieldinit'.
	static MapElementDreamBack()
	{
		LanguageKey[] array = new LanguageKey[3];
		RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.D247B262C26429D1215CCD6DC5021D314B40D725EE010B9CF87E3FDF714DF310).FieldHandle);
		MapElementDreamBack.DreamBackLocationType2TipsKey = array;
	}

	// Token: 0x04002B5C RID: 11100
	private static readonly string[] DreamBackLocationType2Sprites = new string[]
	{
		"map_icon_retrieve_2",
		"map_icon_retrieve_1",
		"map_icon_retrieve_0"
	};

	// Token: 0x04002B5D RID: 11101
	private static readonly LanguageKey[] DreamBackLocationType2TipsKey;

	// Token: 0x04002B5E RID: 11102
	[SerializeField]
	private CImage imgIcon;

	// Token: 0x04002B5F RID: 11103
	[SerializeField]
	private CButton btnDreamBack;

	// Token: 0x04002B60 RID: 11104
	[SerializeField]
	private TooltipInvoker mouseTipDisplayer;
}
