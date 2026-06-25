using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000B3 RID: 179
public class ConchShipGraphicRaycaster : GraphicRaycaster
{
	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000622 RID: 1570 RVA: 0x00029120 File Offset: 0x00027320
	public override Camera eventCamera
	{
		get
		{
			bool flag = null == this.TargetCamera;
			if (flag)
			{
				this.TargetCamera = base.eventCamera;
			}
			return this.TargetCamera;
		}
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000623 RID: 1571 RVA: 0x00029154 File Offset: 0x00027354
	private Canvas Canvas
	{
		get
		{
			bool flag = this._mCanvas == null;
			if (flag)
			{
				this._mCanvas = base.GetComponent<Canvas>();
			}
			return this._mCanvas;
		}
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x00029188 File Offset: 0x00027388
	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		bool flag = this.Canvas == null;
		if (!flag)
		{
			IList<Graphic> canvasGraphics = GraphicRegistry.GetGraphicsForCanvas(this.Canvas);
			bool flag2 = canvasGraphics == null || canvasGraphics.Count == 0;
			if (!flag2)
			{
				Camera currentEventCamera = this.eventCamera;
				bool flag3 = this.Canvas.renderMode == RenderMode.ScreenSpaceOverlay || currentEventCamera == null;
				int displayIndex;
				if (flag3)
				{
					displayIndex = this.Canvas.targetDisplay;
				}
				else
				{
					displayIndex = currentEventCamera.targetDisplay;
				}
				Vector3 eventPosition = Display.RelativeMouseAt(eventData.position);
				bool flag4 = eventPosition != Vector3.zero;
				if (flag4)
				{
					int eventDisplayIndex = (int)eventPosition.z;
					bool flag5 = eventDisplayIndex != displayIndex;
					if (flag5)
					{
						return;
					}
				}
				else
				{
					eventPosition = eventData.position;
				}
				bool flag6 = currentEventCamera == null;
				Vector2 pos;
				if (flag6)
				{
					float w = (float)Screen.width;
					float h = (float)Screen.height;
					bool flag7 = displayIndex > 0 && displayIndex < Display.displays.Length;
					if (flag7)
					{
						w = (float)Display.displays[displayIndex].systemWidth;
						h = (float)Display.displays[displayIndex].systemHeight;
					}
					pos = new Vector2(eventPosition.x / w, eventPosition.y / h);
				}
				else
				{
					pos = currentEventCamera.ScreenToViewportPoint(eventPosition);
				}
				bool flag8 = pos.x < 0f || pos.x > 1f || pos.y < 0f || pos.y > 1f;
				if (!flag8)
				{
					float hitDistance = float.MaxValue;
					Ray ray = default(Ray);
					bool flag9 = currentEventCamera != null;
					if (flag9)
					{
						ray = currentEventCamera.ScreenPointToRay(eventPosition);
					}
					bool flag10 = this.Canvas.renderMode != RenderMode.ScreenSpaceOverlay && base.blockingObjects > GraphicRaycaster.BlockingObjects.None;
					if (flag10)
					{
						bool flag11 = currentEventCamera != null;
						if (flag11)
						{
							float projectionDirection = ray.direction.z;
							float num = Mathf.Approximately(0f, projectionDirection) ? float.PositiveInfinity : Mathf.Abs((currentEventCamera.farClipPlane - currentEventCamera.nearClipPlane) / projectionDirection);
						}
					}
					this.m_RaycastResults.Clear();
					ConchShipGraphicRaycaster.Raycast(this.Canvas, currentEventCamera, eventPosition, canvasGraphics, this.m_RaycastResults);
					int totalCount = this.m_RaycastResults.Count;
					int index = 0;
					while (index < totalCount)
					{
						GameObject go = this.m_RaycastResults[index].gameObject;
						bool appendGraphic = true;
						bool ignoreReversedGraphics = base.ignoreReversedGraphics;
						if (ignoreReversedGraphics)
						{
							bool flag12 = currentEventCamera == null;
							if (flag12)
							{
								Vector3 dir = go.transform.rotation * Vector3.forward;
								appendGraphic = (Vector3.Dot(Vector3.forward, dir) > 0f);
							}
							else
							{
								Vector3 cameraFoward = currentEventCamera.transform.rotation * Vector3.forward;
								Vector3 dir2 = go.transform.rotation * Vector3.forward;
								appendGraphic = (Vector3.Dot(cameraFoward, dir2) > 0f);
							}
						}
						bool flag13 = appendGraphic;
						if (flag13)
						{
							bool flag14 = currentEventCamera == null || this.Canvas.renderMode == RenderMode.ScreenSpaceOverlay;
							float distance;
							if (flag14)
							{
								distance = 0f;
							}
							else
							{
								Transform trans = go.transform;
								Vector3 transForward = trans.forward;
								distance = Vector3.Dot(transForward, trans.position - currentEventCamera.transform.position) / Vector3.Dot(transForward, ray.direction);
								bool flag15 = distance < 0f;
								if (flag15)
								{
									goto IL_438;
								}
							}
							bool flag16 = distance >= hitDistance;
							if (!flag16)
							{
								RaycastResult castResult = new RaycastResult
								{
									gameObject = go,
									module = this,
									distance = distance,
									screenPosition = eventPosition,
									index = (float)resultAppendList.Count,
									depth = this.m_RaycastResults[index].depth,
									sortingLayer = this.Canvas.sortingLayerID,
									sortingOrder = this.Canvas.sortingOrder
								};
								resultAppendList.Add(castResult);
							}
						}
						IL_438:
						index++;
						continue;
						goto IL_438;
					}
				}
			}
		}
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x000295E4 File Offset: 0x000277E4
	private static void Raycast(Canvas canvas, Camera eventCamera, Vector2 pointerPosition, IList<Graphic> foundGraphics, List<Graphic> results)
	{
		int totalCount = foundGraphics.Count;
		Graphic upGraphic = null;
		int upIndex = -1;
		for (int i = 0; i < totalCount; i++)
		{
			Graphic graphic = foundGraphics[i];
			bool flag = !graphic.raycastTarget;
			if (!flag)
			{
				int depth = ConchShipGraphicRaycaster.GetGraphicDepth(graphic);
				bool flag2 = depth == -1 || ConchShipGraphicRaycaster.GetGraphicCanvasRenderer(graphic).cull;
				if (!flag2)
				{
					bool flag3 = !RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera);
					if (!flag3)
					{
						bool flag4 = eventCamera != null && eventCamera.WorldToScreenPoint(graphic.rectTransform.position).z > eventCamera.farClipPlane;
						if (!flag4)
						{
							bool flag5 = graphic.Raycast(pointerPosition, eventCamera);
							if (flag5)
							{
								bool flag6 = depth > upIndex;
								if (flag6)
								{
									upIndex = depth;
									upGraphic = graphic;
								}
							}
						}
					}
				}
			}
		}
		bool flag7 = upGraphic != null;
		if (flag7)
		{
			results.Add(upGraphic);
		}
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x000296E0 File Offset: 0x000278E0
	public static int GetGraphicDepth(Graphic graphic)
	{
		CImage cImage = graphic as CImage;
		bool flag = cImage != null;
		int depth;
		if (flag)
		{
			depth = cImage.depth;
		}
		else
		{
			depth = graphic.depth;
		}
		return depth;
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x00029714 File Offset: 0x00027914
	public static CanvasRenderer GetGraphicCanvasRenderer(Graphic graphic)
	{
		bool flag = graphic is CImage;
		CanvasRenderer canvasRenderer;
		if (flag)
		{
			canvasRenderer = (graphic as CImage).canvasRenderer;
		}
		else
		{
			canvasRenderer = graphic.canvasRenderer;
		}
		return canvasRenderer;
	}

	// Token: 0x0400050C RID: 1292
	public Camera TargetCamera;

	// Token: 0x0400050D RID: 1293
	private Canvas _mCanvas;

	// Token: 0x0400050E RID: 1294
	[NonSerialized]
	private List<Graphic> m_RaycastResults = new List<Graphic>();

	// Token: 0x0400050F RID: 1295
	[NonSerialized]
	private static readonly List<Graphic> s_SortedGraphics = new List<Graphic>();
}
