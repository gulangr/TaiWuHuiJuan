using System;
using Game.Views.Bottom;
using Game.Views.Map;
using GameData.Domains.Map;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020000FD RID: 253
public class MapClickReceiver : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x06000871 RID: 2161 RVA: 0x00039FEE File Offset: 0x000381EE
	public float CurrentScale
	{
		get
		{
			return this._Scale;
		}
	}

	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x06000872 RID: 2162 RVA: 0x00039FF6 File Offset: 0x000381F6
	// (set) Token: 0x06000873 RID: 2163 RVA: 0x00039FFE File Offset: 0x000381FE
	public bool ScaleListening
	{
		get
		{
			return this._ScaleListening;
		}
		set
		{
			this._ScaleListening = value;
		}
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x0003A008 File Offset: 0x00038208
	private void Awake()
	{
		this.SelfRectTransform = base.GetComponent<RectTransform>();
		this.SelfRectTransform.pivot = Vector2.up * 0.5f;
		this.EvenColumnRectTrans.SetSize(new Vector2((float)(this.BlockWidth * (this._MapSize + 1)), (float)(this.BlockHeight * (this._MapSize + 1))));
		this.OddColumnRectTrans.SetSize(new Vector2((float)(this.BlockWidth * this._MapSize), (float)(this.BlockHeight * this._MapSize)));
		this._DragParentRect = this.DragRectTransform;
		this._DragCanvasGroup = base.GetComponent<CanvasGroup>();
		bool flag = this._DragCanvasGroup == null;
		if (flag)
		{
			this._DragCanvasGroup = base.gameObject.AddComponent<CanvasGroup>();
		}
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0003A0D4 File Offset: 0x000382D4
	public void SetMapSize(int mapSize)
	{
		this._MapSize = mapSize;
		this.EvenColumnRectTrans.SetSize(new Vector2((float)(this.BlockWidth * (this._MapSize + 1)), (float)(this.BlockHeight * (this._MapSize + 1))));
		this.OddColumnRectTrans.SetSize(new Vector2((float)(this.BlockWidth * this._MapSize), (float)(this.BlockHeight * this._MapSize)));
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x0003A148 File Offset: 0x00038348
	public void SetOffset(Vector2 offset)
	{
		bool isMapSizeOdd = this._MapSize % 2 == 0;
		this.SelfRectTransform.localPosition = new Vector3((float)(-(float)this.BlockWidth) * 0.5f + offset.x + this.Offset.x, (isMapSizeOdd ? ((float)this.BlockHeight * 0.5f) : 0f) + offset.y + this.Offset.y, this.SelfRectTransform.localPosition.z);
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x0003A1D0 File Offset: 0x000383D0
	public void OnClick(bool oddColumn)
	{
		Vector3 clickWorldPos = UIManager.Instance.UiCamera.ScreenToWorldPoint(Input.mousePosition);
		Vector3 clickLocalPos = this.SelfRectTransform.InverseTransformPoint(clickWorldPos);
		Vector2Int logicalPosition = this.CalcLogicalPosition(clickLocalPos, oddColumn);
		bool flag = logicalPosition.x >= 0 && logicalPosition.x < this._MapSize && logicalPosition.y >= 0 && logicalPosition.y < this._MapSize;
		if (flag)
		{
			Action<int, int> onMapBlockClick = this.OnMapBlockClick;
			if (onMapBlockClick != null)
			{
				onMapBlockClick(logicalPosition.x, logicalPosition.y);
			}
		}
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x0003A268 File Offset: 0x00038468
	public void OnRightClick(bool oddColumn)
	{
		bool flag = !ViewBottom.IsReady;
		if (!flag)
		{
			bool flag2 = ViewWorldMap.CheckRightMouseClickHandled();
			if (!flag2)
			{
				Vector3 clickWorldPos = UIManager.Instance.UiCamera.ScreenToWorldPoint(Input.mousePosition);
				Vector3 clickLocalPos = this.SelfRectTransform.InverseTransformPoint(clickWorldPos);
				bool clickHandled = false;
				WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
				Vector2Int logicalPosition = this.CalcLogicalPosition(clickLocalPos, oddColumn);
				bool flag3;
				if (logicalPosition.x >= 0 && logicalPosition.x < this._MapSize && logicalPosition.y >= 0 && logicalPosition.y < this._MapSize && ViewBottom.OpBtnCanOperate() && mapModel.SelectedBlock != null)
				{
					MapBlockData selectedBlock = mapModel.SelectedBlock;
					Location? location = (selectedBlock != null) ? new Location?(selectedBlock.GetLocation()) : null;
					Location location2 = mapModel.CurrentBlockData.GetLocation();
					flag3 = (location != null && (location == null || location.GetValueOrDefault() == location2));
				}
				else
				{
					flag3 = false;
				}
				bool flag4 = flag3;
				if (flag4)
				{
					MapBlockData blockData = mapModel.GetBlockData(mapModel.CoordinateToLocation(mapModel.SelectedBlock.GetLocation().AreaId, (byte)logicalPosition.x, (byte)logicalPosition.y));
					bool flag5 = blockData != null && blockData.Visible;
					if (flag5)
					{
						ViewBottom.ClickOpBtn(blockData.GetLocation());
						clickHandled = true;
					}
				}
				bool flag6 = !clickHandled && mapModel.CurrentBlockData != null;
				if (flag6)
				{
					ByteCoordinate pos = mapModel.CurrentBlockData.GetBlockPos();
					Action<int, int> onMapBlockClick = this.OnMapBlockClick;
					if (onMapBlockClick != null)
					{
						onMapBlockClick((int)pos.X, (int)pos.Y);
					}
				}
			}
		}
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x0003A410 File Offset: 0x00038610
	private Vector2Int CalcLogicalPosition(Vector2 localPosition, bool oddColumn)
	{
		int columnIndex = (int)(localPosition.x / (float)this.BlockWidth);
		bool isMapSizeOdd = this._MapSize % 2 == 0;
		localPosition.y -= ((!isMapSizeOdd) ? ((float)this.BlockHeight * 0.5f) : 0f);
		int locationX = columnIndex * 2;
		int locationY = Mathf.RoundToInt(localPosition.y / (float)this.BlockHeight);
		if (oddColumn)
		{
			columnIndex = (int)((localPosition.x - (float)this.BlockWidth * 0.5f) / (float)this.BlockWidth);
			locationX = 2 * columnIndex + 1;
			bool flag = localPosition.y > 0f;
			if (flag)
			{
				locationY = Mathf.CeilToInt(localPosition.y / (float)this.BlockHeight);
			}
			else
			{
				locationY = Mathf.FloorToInt(localPosition.y / (float)this.BlockHeight);
			}
		}
		float middle = (float)locationX * 0.5f;
		int posX = (int)middle;
		int posY = (int)middle;
		bool flag2 = locationY != 0;
		if (flag2)
		{
			if (oddColumn)
			{
				posX = Mathf.RoundToInt(middle - Mathf.Sign((float)locationY) * 0.5f * (float)(2 * Mathf.Abs(locationY) - 1));
			}
			else
			{
				posX = Mathf.RoundToInt(middle - (float)locationY);
			}
			posY = locationX - posX;
		}
		return new Vector2Int(posX, posY);
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x0003A54C File Offset: 0x0003874C
	private void LateUpdate()
	{
		bool flag = !UIManager.Instance.IsFocusElement(UIElement.WorldMap);
		if (!flag)
		{
			this.OnUpdateScale();
			this.OnUpdateDragMove();
			this.OnUpdateMouse();
			bool flag2 = this._DragDoing && !Input.GetMouseButton(0);
			if (flag2)
			{
				this._DragDoing = false;
				this._DragCanvasGroup.blocksRaycasts = true;
			}
		}
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x0003A5B5 File Offset: 0x000387B5
	public void SetCurrentScale(float scale)
	{
		this._Scale = scale;
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x0003A5BF File Offset: 0x000387BF
	public void RefreshScale()
	{
		Action<float> onScaleEvent = this.OnScaleEvent;
		if (onScaleEvent != null)
		{
			onScaleEvent(this._Scale);
		}
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x0003A5DC File Offset: 0x000387DC
	private void OnUpdateScale()
	{
		bool scaleListening = this._ScaleListening;
		if (scaleListening)
		{
			bool flag = this.ExtraScaleListening != null && !this.ExtraScaleListening();
			if (!flag)
			{
				float scrollValue = Input.GetAxis("Mouse ScrollWheel");
				bool flag2 = Math.Abs(scrollValue) > 0f;
				if (flag2)
				{
					float scaleValue = Mathf.Clamp(scrollValue * 0.5f + this._Scale, 0.25f, 1f);
					bool flag3 = scrollValue < 0f && this._Scale <= 0.25f && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(3);
					if (flag3)
					{
						ConchShipCursor.Instance.AddWheelProgress(-scrollValue * 2f);
					}
					else
					{
						bool flag4 = scrollValue > 0f;
						if (flag4)
						{
							ConchShipCursor.Instance.AddWheelProgress(-1f);
						}
					}
					this._Scale = scaleValue;
					Action<float> onScaleEvent = this.OnScaleEvent;
					if (onScaleEvent != null)
					{
						onScaleEvent(scaleValue);
					}
				}
			}
		}
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0003A6D3 File Offset: 0x000388D3
	public void OnPointerEnter(PointerEventData eventData)
	{
		this._ScaleListening = true;
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0003A6DD File Offset: 0x000388DD
	public void OnPointerExit(PointerEventData eventData)
	{
		this._ScaleListening = false;
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x0003A6E7 File Offset: 0x000388E7
	public void StartScaleListening()
	{
		this._ScaleListening = true;
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x0003A6F1 File Offset: 0x000388F1
	public void StopScaleListening()
	{
		this._ScaleListening = false;
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0003A6FC File Offset: 0x000388FC
	public void OnUpdateDragMove()
	{
		bool flag = this._DragDoing && this._DragNeedUpdate;
		if (flag)
		{
			this._DragNewPosition = UIManager.Instance.MousePosToLocalPos(this._DragParentRect);
			this._DragOffset = this._DragNewPosition - this._DragLastPosition;
			this._DragLastPosition = this._DragNewPosition;
			Action<Vector2> onDragEvent = this.OnDragEvent;
			if (onDragEvent != null)
			{
				onDragEvent(this._DragOffset * (1f / this._Scale));
			}
			this._DragNeedUpdate = false;
		}
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x0003A78C File Offset: 0x0003898C
	public void OnDrag(PointerEventData eventData)
	{
		bool dragDoing = this._DragDoing;
		if (dragDoing)
		{
			Vector2 mousePos = Input.mousePosition;
			bool flag = mousePos.x < 0f || mousePos.x > (float)Screen.width || mousePos.y < 0f || mousePos.y > (float)Screen.height;
			if (!flag)
			{
				this._DragNeedUpdate = true;
			}
		}
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0003A7F8 File Offset: 0x000389F8
	public void OnBeginDrag(PointerEventData eventData)
	{
		bool flag = eventData.button == PointerEventData.InputButton.Left;
		if (flag)
		{
			this._DragOffset = Vector2.zero;
			this._DragLastPosition = UIManager.Instance.MousePosToLocalPos(this._DragParentRect);
			this._DragDoing = true;
			this._DragCanvasGroup.blocksRaycasts = false;
		}
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x0003A84A File Offset: 0x00038A4A
	public void OnEndDrag(PointerEventData eventData)
	{
		this._DragDoing = false;
		this._DragCanvasGroup.blocksRaycasts = true;
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x0003A861 File Offset: 0x00038A61
	public void StopDrag()
	{
		this._DragDoing = false;
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x0003A86B File Offset: 0x00038A6B
	public bool IsDragProcessing()
	{
		return this._DragDoing;
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x0003A874 File Offset: 0x00038A74
	private bool IsMousePositionInRange(bool oddColumn, out Vector2Int currentLogicalPosition)
	{
		Vector3 clickWorldPos = UIManager.Instance.UiCamera.ScreenToWorldPoint(Input.mousePosition);
		Vector3 clickLocalPos = this.SelfRectTransform.InverseTransformPoint(clickWorldPos);
		Vector2Int logicalPosition = this.CalcLogicalPosition(clickLocalPos, oddColumn);
		bool flag = logicalPosition.x >= 0 && logicalPosition.x < this._MapSize && logicalPosition.y >= 0 && logicalPosition.y < this._MapSize;
		bool result;
		if (flag)
		{
			currentLogicalPosition = logicalPosition;
			result = true;
		}
		else
		{
			currentLogicalPosition = default(Vector2Int);
			result = false;
		}
		return result;
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x0003A908 File Offset: 0x00038B08
	public void OnUpdateMouse()
	{
		bool flag = this._IsMouseInOddRange != null;
		if (flag)
		{
			Vector2Int logicalPosition;
			bool flag2 = this.IsMousePositionInRange(this._IsMouseInOddRange.Value, out logicalPosition);
			if (flag2)
			{
				bool flag3 = !logicalPosition.Equals(this._LastPointLogicalPosition);
				if (flag3)
				{
					Action<int, int> onMapBlockPointExit = this.OnMapBlockPointExit;
					if (onMapBlockPointExit != null)
					{
						onMapBlockPointExit(this._LastPointLogicalPosition.x, this._LastPointLogicalPosition.y);
					}
					Action<int, int> onMapBlockPointEnter = this.OnMapBlockPointEnter;
					if (onMapBlockPointEnter != null)
					{
						onMapBlockPointEnter(logicalPosition.x, logicalPosition.y);
					}
				}
				this._LastPointLogicalPosition = logicalPosition;
				Action<int, int> onMapBlockPointStay = this.OnMapBlockPointStay;
				if (onMapBlockPointStay != null)
				{
					onMapBlockPointStay(logicalPosition.x, logicalPosition.y);
				}
			}
		}
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x0003A9CC File Offset: 0x00038BCC
	public void OnPointerEnteredGridContainer(bool oddColumn)
	{
		Vector2Int logicalPosition;
		bool flag = this.IsMousePositionInRange(oddColumn, out logicalPosition);
		if (flag)
		{
			bool flag2 = !logicalPosition.Equals(this._LastPointLogicalPosition);
			if (flag2)
			{
				Action<int, int> onMapBlockPointExit = this.OnMapBlockPointExit;
				if (onMapBlockPointExit != null)
				{
					onMapBlockPointExit(this._LastPointLogicalPosition.x, this._LastPointLogicalPosition.y);
				}
				Action<int, int> onMapBlockPointEnter = this.OnMapBlockPointEnter;
				if (onMapBlockPointEnter != null)
				{
					onMapBlockPointEnter(logicalPosition.x, logicalPosition.y);
				}
			}
			this._LastPointLogicalPosition = logicalPosition;
			this._IsMouseInOddRange = new bool?(oddColumn);
			Action<int, int> onMapBlockPointStay = this.OnMapBlockPointStay;
			if (onMapBlockPointStay != null)
			{
				onMapBlockPointStay(logicalPosition.x, logicalPosition.y);
			}
		}
		else
		{
			Action<int, int> onMapBlockPointExit2 = this.OnMapBlockPointExit;
			if (onMapBlockPointExit2 != null)
			{
				onMapBlockPointExit2(this._LastPointLogicalPosition.x, this._LastPointLogicalPosition.y);
			}
			this._LastPointLogicalPosition = new Vector2Int(int.MaxValue, int.MinValue);
			this._IsMouseInOddRange = null;
		}
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x0003AACC File Offset: 0x00038CCC
	public void OnPointerExitedGridContainer(bool oddColumn)
	{
		Action<int, int> onMapBlockPointExit = this.OnMapBlockPointExit;
		if (onMapBlockPointExit != null)
		{
			onMapBlockPointExit(this._LastPointLogicalPosition.x, this._LastPointLogicalPosition.y);
		}
		this._LastPointLogicalPosition = new Vector2Int(int.MaxValue, int.MinValue);
		this._IsMouseInOddRange = null;
	}

	// Token: 0x04000B9F RID: 2975
	public RectTransform SelfRectTransform;

	// Token: 0x04000BA0 RID: 2976
	public RectTransform DragRectTransform;

	// Token: 0x04000BA1 RID: 2977
	public Vector2 Offset;

	// Token: 0x04000BA2 RID: 2978
	public RectTransform EvenColumnRectTrans;

	// Token: 0x04000BA3 RID: 2979
	public RectTransform OddColumnRectTrans;

	// Token: 0x04000BA4 RID: 2980
	public TooltipInvoker TipDisplayer;

	// Token: 0x04000BA5 RID: 2981
	public int BlockWidth = 640;

	// Token: 0x04000BA6 RID: 2982
	public int BlockHeight = 332;

	// Token: 0x04000BA7 RID: 2983
	private int _MapSize = 30;

	// Token: 0x04000BA8 RID: 2984
	private float _Scale = 1f;

	// Token: 0x04000BA9 RID: 2985
	private bool _ScaleListening = false;

	// Token: 0x04000BAA RID: 2986
	private Vector2Int _LastPointLogicalPosition;

	// Token: 0x04000BAB RID: 2987
	private bool? _IsMouseInOddRange = null;

	// Token: 0x04000BAC RID: 2988
	private bool _DragDoing = false;

	// Token: 0x04000BAD RID: 2989
	private bool _DragNeedUpdate = false;

	// Token: 0x04000BAE RID: 2990
	private CanvasGroup _DragCanvasGroup = null;

	// Token: 0x04000BAF RID: 2991
	private RectTransform _DragParentRect = null;

	// Token: 0x04000BB0 RID: 2992
	private Vector2 _DragOffset;

	// Token: 0x04000BB1 RID: 2993
	private Vector2 _DragLastPosition;

	// Token: 0x04000BB2 RID: 2994
	private Vector2 _DragNewPosition;

	// Token: 0x04000BB3 RID: 2995
	public Func<bool> ExtraScaleListening;

	// Token: 0x04000BB4 RID: 2996
	public Action<float> OnScaleEvent;

	// Token: 0x04000BB5 RID: 2997
	public Action<Vector2> OnDragEvent;

	// Token: 0x04000BB6 RID: 2998
	public Action<int, int> OnMapBlockClick;

	// Token: 0x04000BB7 RID: 2999
	public Action<int, int> OnMapBlockPointEnter;

	// Token: 0x04000BB8 RID: 3000
	public Action<int, int> OnMapBlockPointExit;

	// Token: 0x04000BB9 RID: 3001
	public Action<int, int> OnMapBlockPointStay;

	// Token: 0x04000BBA RID: 3002
	public const float MaxScale = 1f;

	// Token: 0x04000BBB RID: 3003
	public const float MinScale = 0.25f;
}
