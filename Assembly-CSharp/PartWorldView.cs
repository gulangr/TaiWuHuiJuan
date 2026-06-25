using System;
using Config;
using TMPro;
using UnityEngine;

// Token: 0x02000255 RID: 597
public class PartWorldView : Refers
{
	// Token: 0x06002776 RID: 10102 RVA: 0x00123054 File Offset: 0x00121254
	public void InitAreas()
	{
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		for (short i = 0; i < 135; i += 1)
		{
			short areaId = i;
			MapAreaItem areaConfig = mapModel.Areas[(int)areaId].GetConfig();
			Refers areaRefers = ((int)areaId < this.AreaHolder.childCount) ? this.AreaHolder.GetChild((int)areaId).GetComponent<Refers>() : Object.Instantiate<GameObject>(this.AreaHolder.GetChild(0).gameObject, this.AreaHolder, false).GetComponent<Refers>();
			CImage areaIcon = areaRefers.CGet<CImage>("AreaIcon");
			CButtonObsolete areaBtn = areaRefers.GetComponent<CButtonObsolete>();
			PointerTrigger pointerTrigger = areaRefers.GetComponent<PointerTrigger>();
			areaRefers.UserInt = (int)areaId;
			areaRefers.CGet<TextMeshProUGUI>("Name").text = areaConfig.Name;
			areaRefers.CGet<GameObject>("TaiwuVillageIcon").SetActive(areaId == mapModel.GetTaiwuVillageBlock().AreaId);
			areaRefers.GetComponent<RectTransform>().anchoredPosition = this.GetAnchoredPos((int)areaConfig.WorldMapPos[0], (int)areaConfig.WorldMapPos[1]);
			areaIcon.SetSprite(areaConfig.BigMapIcon, false, null);
			areaBtn.ClearAndAddListener(delegate
			{
				Action<short> onClickArea = this.OnClickArea;
				if (onClickArea != null)
				{
					onClickArea(areaId);
				}
			});
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				Action<short> onMouseEnterArea = this.OnMouseEnterArea;
				if (onMouseEnterArea != null)
				{
					onMouseEnterArea(areaId);
				}
			});
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				Action<short> onMouseExitArea = this.OnMouseExitArea;
				if (onMouseExitArea != null)
				{
					onMouseExitArea(areaId);
				}
			});
			Action<Refers, short> onAreaInit = this.OnAreaInit;
			if (onAreaInit != null)
			{
				onAreaInit(areaRefers, i);
			}
		}
	}

	// Token: 0x06002777 RID: 10103 RVA: 0x001231F8 File Offset: 0x001213F8
	public Vector2 GetAnchoredPos(int x, int y)
	{
		return new Vector2((float)(100 * x + 50 + 3 * (x - 1)), (float)(100 * y + 50 + 3 * (y - 1)));
	}

	// Token: 0x06002778 RID: 10104 RVA: 0x0012322C File Offset: 0x0012142C
	public void ResetScaleAndFocusArea(short areaId)
	{
		RectTransform parent = base.transform.parent.GetComponent<RectTransform>();
		RectTransform background = this.ScaleAndMoveRoot.GetComponent<RectTransform>();
		background.localScale = Vector3.one;
		this.ScaleAndMoveRoot.Reset();
		Vector3 parentCenterWorldPos = base.transform.TransformPoint(parent.rect.center);
		RectTransform targetArea = this.AreaHolder.GetChild((int)areaId).GetComponent<RectTransform>();
		Vector3 areaWorldPos = targetArea.position;
		Vector2 areaPivotOffset = new Vector2((0.5f - targetArea.pivot.x) * targetArea.rect.width, (0.5f - targetArea.pivot.y) * targetArea.rect.height);
		Vector3 pivotOffsetWorld = targetArea.TransformVector(areaPivotOffset);
		Vector3 areaCenterWorldPos = areaWorldPos + pivotOffsetWorld;
		Vector3 areaCenterLocalPos = parent.InverseTransformPoint(areaCenterWorldPos);
		Vector3 newBackgroundLocalPos = background.localPosition - areaCenterLocalPos;
		background.localPosition = newBackgroundLocalPos;
	}

	// Token: 0x04001CB0 RID: 7344
	public const int AreaIconSize = 100;

	// Token: 0x04001CB1 RID: 7345
	public const int AreaIconInterval = 3;

	// Token: 0x04001CB2 RID: 7346
	public RectTransform AreaHolder;

	// Token: 0x04001CB3 RID: 7347
	public MouseWheelScale ScaleAndMoveRoot;

	// Token: 0x04001CB4 RID: 7348
	public UIRectDragMove DragRoot;

	// Token: 0x04001CB5 RID: 7349
	public RectTransform StateMaskHolder;

	// Token: 0x04001CB6 RID: 7350
	public RectTransform CurrPosBottom;

	// Token: 0x04001CB7 RID: 7351
	public RectTransform CurrPosPointer;

	// Token: 0x04001CB8 RID: 7352
	public RectTransform PathHolder;

	// Token: 0x04001CB9 RID: 7353
	[NonSerialized]
	public Action<short> OnClickArea;

	// Token: 0x04001CBA RID: 7354
	[NonSerialized]
	public Action<short> OnMouseEnterArea;

	// Token: 0x04001CBB RID: 7355
	[NonSerialized]
	public Action<short> OnMouseExitArea;

	// Token: 0x04001CBC RID: 7356
	[NonSerialized]
	public Action<Refers, short> OnAreaInit;
}
