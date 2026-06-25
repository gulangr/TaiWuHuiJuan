using System;
using System.Collections.Generic;
using System.Linq;
using AiEditor;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Views.Adventure;
using Game.Views.Legacy.AdventureEditor.Migrate;
using GameData.Adventure;
using GameData.Adventure.Editor;
using SubEditor.AdventureCommonRefersListEditor;
using TMPro;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class AdventureEditorMicro : MonoBehaviour, ISelectAndDragComponent, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
{
	// Token: 0x1700026C RID: 620
	// (get) Token: 0x060015A6 RID: 5542 RVA: 0x00085F99 File Offset: 0x00084199
	// (set) Token: 0x060015A7 RID: 5543 RVA: 0x00085FA1 File Offset: 0x000841A1
	public AdventureBlockIndex Index { get; private set; }

	// Token: 0x1700026D RID: 621
	// (get) Token: 0x060015A8 RID: 5544 RVA: 0x00085FAA File Offset: 0x000841AA
	public AdventureBlockIndex LightingGridIndex
	{
		get
		{
			return this.Index;
		}
	}

	// Token: 0x1700026E RID: 622
	// (get) Token: 0x060015A9 RID: 5545 RVA: 0x00085FB2 File Offset: 0x000841B2
	public Transform LightingPreviewAnchor
	{
		get
		{
			return this.groundSurface.rectTransform;
		}
	}

	// Token: 0x1700026F RID: 623
	// (get) Token: 0x060015AA RID: 5546 RVA: 0x00085FBF File Offset: 0x000841BF
	RectTransform ISelectAndDragComponent.RectTransform
	{
		get
		{
			return base.GetComponent<RectTransform>();
		}
	}

	// Token: 0x060015AB RID: 5547 RVA: 0x00085FC8 File Offset: 0x000841C8
	bool ISelectAndDragComponent.OverlapsMouse()
	{
		Camera uiCamera = UIManager.Instance.UiCamera;
		Vector3 mousePos = Input.mousePosition;
		RectTransform targetRect = (this.groundSurface != null) ? this.groundSurface.rectTransform : base.GetComponent<RectTransform>();
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, mousePos, uiCamera, out localPoint);
		return Mathf.Abs(localPoint.x / 106f) + Mathf.Abs(localPoint.y / 55f) <= 1f;
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x0008604C File Offset: 0x0008424C
	bool ISelectAndDragComponent.Overlaps(Rect rect)
	{
		bool flag = this.groundSurface != null;
		Vector2 center;
		if (flag)
		{
			center = base.GetComponent<RectTransform>().anchoredPosition + this.groundSurface.rectTransform.localPosition;
		}
		else
		{
			center = base.GetComponent<RectTransform>().anchoredPosition;
		}
		float dx = Mathf.Max(new float[]
		{
			0f,
			rect.xMin - center.x,
			center.x - rect.xMax
		});
		float dy = Mathf.Max(new float[]
		{
			0f,
			rect.yMin - center.y,
			center.y - rect.yMax
		});
		return dx / 106f + dy / 55f <= 1f;
	}

	// Token: 0x060015AD RID: 5549 RVA: 0x0008611D File Offset: 0x0008431D
	private void OnEnable()
	{
		AdventureEditorKit.BlackBoard.Register(this);
		GEvent.Add(UiEvents.AdventureEditorPureModeSwitch, new GEvent.Callback(this.AdventureEditorPureMode));
		this.dialogBg.gameObject.SetActive(false);
	}

	// Token: 0x060015AE RID: 5550 RVA: 0x0008615C File Offset: 0x0008435C
	private void AdventureEditorPureMode(ArgumentBox argBox)
	{
		bool pureMode;
		argBox.Get("PureMode", out pureMode);
		this.indexText.gameObject.SetActive(pureMode);
		this.elementName.gameObject.SetActive(pureMode);
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x0008619C File Offset: 0x0008439C
	private void OnDisable()
	{
		AdventureEditorKit.BlackBoard.Unregister(this);
		GEvent.Remove(UiEvents.AdventureEditorPureModeSwitch, new GEvent.Callback(this.AdventureEditorPureMode));
	}

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x060015B0 RID: 5552 RVA: 0x000861C7 File Offset: 0x000843C7
	bool IAdventureBlackBoardElement<EAdventureEditType>.LoadOnRegister
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060015B1 RID: 5553 RVA: 0x000861CC File Offset: 0x000843CC
	void IAdventureBlackBoardElement<EAdventureEditType>.Load(EAdventureEditType editType)
	{
		bool flag = editType.Contains(EAdventureEditType.BlockVisible);
		if (flag)
		{
			this.ReloadVisible();
		}
		bool flag2 = editType.Contains(EAdventureEditType.BlockViewMode);
		if (flag2)
		{
			this.ReloadViewMode();
			this.UpdateLightingModifiers();
		}
		bool flag3 = editType.Contains(EAdventureEditType.BlockProperties);
		if (flag3)
		{
			this.CalculateAndSetPosition();
		}
	}

	// Token: 0x060015B2 RID: 5554 RVA: 0x00086219 File Offset: 0x00084419
	public void Set(AdventureBlockIndex index)
	{
		this.Set(index, index.I == 8);
	}

	// Token: 0x060015B3 RID: 5555 RVA: 0x00086230 File Offset: 0x00084430
	public void Set(AdventureBlockIndex index, bool isVolumeHost)
	{
		this.Unselect();
		this.Index = index;
		this._isVolumeHost = isVolumeHost;
		bool flag = this.indexText != null;
		if (flag)
		{
			this.indexText.text = index.ToString();
		}
		bool needActive = !base.gameObject.activeSelf;
		bool flag2 = needActive;
		if (flag2)
		{
			base.gameObject.SetActive(true);
		}
		this.CalculateAndSetPosition();
		this.HandleVolumeInstance();
		this.UpdateLightingModifiers();
		this.ReloadAll();
	}

	// Token: 0x060015B4 RID: 5556 RVA: 0x000862BC File Offset: 0x000844BC
	private void CalculateAndSetPosition()
	{
		Vector2 volumePos = AdventureEditorMicro.CalculateVolumePosition(this.Index.X, this.Index.Y);
		Vector2 microOffset = AdventureEditorMicro.CalculateMicroOffset(this.Index.I);
		float heightOffset = this.ApplyHeightAndGetOffset();
		base.GetComponent<RectTransform>().anchoredPosition = volumePos + microOffset + new Vector2(0f, heightOffset);
		this.UpdateVolumePosition(heightOffset);
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x0008632C File Offset: 0x0008452C
	private float ApplyHeightAndGetOffset()
	{
		IReadOnlyList<AdventureBlockSnapshot> currentBlocks = AdventureEditorKit.BlackBoard.CurrentGroupBlocks;
		bool flag = currentBlocks == null;
		float result;
		if (flag)
		{
			result = 0f;
		}
		else
		{
			float[] heights = AdventureHeightCalculator.ExtractHeights<AdventureBlockSnapshot>(currentBlocks, this.Index.X, this.Index.Y, (AdventureBlockSnapshot b) => b.Index, (AdventureBlockSnapshot b) => b.Height);
			AdventureHeightCalculator.HeightResult heightResult = AdventureHeightCalculator.Calculate(heights);
			bool flag2 = this.Index.I >= 0 && this.Index.I < heightResult.MicroHeights.Length;
			if (flag2)
			{
				this.SetVolumeHeight(heightResult.MicroHeights[this.Index.I]);
			}
			result = heightResult.VolumeHeight * 1840f;
		}
		return result;
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x00086414 File Offset: 0x00084614
	private void UpdateVolumePosition(float heightOffset)
	{
		bool flag = this._volumeInstance == null;
		if (!flag)
		{
			RectTransform volumeRt = this._volumeInstance.GetComponent<RectTransform>();
			volumeRt.anchoredPosition = new Vector2(0f, AdventureEditorMicro.MicroSize.y - heightOffset);
		}
	}

	// Token: 0x060015B7 RID: 5559 RVA: 0x00086460 File Offset: 0x00084660
	private static Vector2 CalculateVolumePosition(int x, int y)
	{
		float offsetX = (float)(x + y) * AdventureEditorMicro.VolumeSize.x / 2f;
		float offsetY = (float)(y - x) * AdventureEditorMicro.VolumeSize.y / 2f;
		return new Vector2(offsetX, offsetY);
	}

	// Token: 0x060015B8 RID: 5560 RVA: 0x000864A8 File Offset: 0x000846A8
	private static Vector2 CalculateMicroOffset(int microIndex)
	{
		bool flag = microIndex < 0 || microIndex >= AdventureEditorMicro.DataIndexToRenderIndex.Length;
		Vector2 result;
		if (flag)
		{
			result = Vector2.zero;
		}
		else
		{
			int renderIndex = AdventureEditorMicro.DataIndexToRenderIndex[microIndex];
			Vector2 offset = AdventureEditorMicro.MicroOffsets[renderIndex];
			result = new Vector2(offset.x * AdventureEditorMicro.MicroSize.x, offset.y * AdventureEditorMicro.MicroSize.y);
		}
		return result;
	}

	// Token: 0x060015B9 RID: 5561 RVA: 0x00086518 File Offset: 0x00084718
	private void HandleVolumeInstance()
	{
		bool flag = this._isVolumeHost && this.volumePrefab != null;
		if (flag)
		{
			bool flag2 = this._volumeInstance == null;
			if (flag2)
			{
				GameObject volumeObj = Object.Instantiate<GameObject>(this.volumePrefab, base.transform);
				this._volumeInstance = volumeObj.GetComponent<AdventureEditorVolume>();
				RectTransform volumeRt = volumeObj.GetComponent<RectTransform>();
				volumeRt.anchoredPosition = new Vector2(0f, AdventureEditorMicro.MicroSize.y);
				AdventureEditorVolume volumeInstance = this._volumeInstance;
				if (volumeInstance != null)
				{
					volumeInstance.SetVolumeRealFullHeight(1840f);
				}
			}
			AdventureEditorVolume volumeInstance2 = this._volumeInstance;
			if (volumeInstance2 != null)
			{
				volumeInstance2.Set(this.Index.X, this.Index.Y);
			}
		}
		else
		{
			bool flag3 = this._volumeInstance != null;
			if (flag3)
			{
				Object.Destroy(this._volumeInstance.gameObject);
				this._volumeInstance = null;
			}
		}
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x00086608 File Offset: 0x00084808
	public void SetVolumeHeight(float height)
	{
		bool flag = this.volumeController != null;
		if (flag)
		{
			this.volumeController.VolumeHeight = height;
		}
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x00086638 File Offset: 0x00084838
	public void Select()
	{
		bool flag = this.selectHighlight != null;
		if (flag)
		{
			this.selectHighlight.gameObject.SetActive(true);
		}
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x00086668 File Offset: 0x00084868
	public void Unselect()
	{
		bool flag = this.selectHighlight != null;
		if (flag)
		{
			this.selectHighlight.gameObject.SetActive(false);
		}
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x00086698 File Offset: 0x00084898
	public void EditElements(AdventureBlockElementsEditor editor)
	{
		AdventureEditorKit.UpdateElementCache();
		editor.TargetIndex = this.Index;
		editor.SetEditableList(null);
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x000866B5 File Offset: 0x000848B5
	public void ReloadElements()
	{
		this.ReloadVisible();
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x000866C0 File Offset: 0x000848C0
	public void SetDecorate(List<string> decorates)
	{
		this.blockDecoratesHolder.gameObject.SetActive(decorates != null && decorates.Count > 0);
		bool flag = decorates != null && decorates.Count > 0;
		if (flag)
		{
			this.blockDecoratesHolder.Rebuild<AdventureBlockDecoratesEditorTemplate>(decorates.Count, delegate(AdventureBlockDecoratesEditorTemplate refer, int index)
			{
				refer.decorate.SetSprite(decorates[index], false, null);
			});
		}
		this.RefreshDecorateLighting();
	}

	// Token: 0x060015C0 RID: 5568 RVA: 0x0008674E File Offset: 0x0008494E
	public void Reload()
	{
		this.CalculateAndSetPosition();
		this.HandleVolumeInstance();
		this.UpdateLightingModifiers();
		this.ReloadAll();
	}

	// Token: 0x060015C1 RID: 5569 RVA: 0x0008676D File Offset: 0x0008496D
	private void ReloadAll()
	{
		this.ReloadVisible();
		this.ReloadViewMode();
		this.UpdateLightingModifiers();
	}

	// Token: 0x060015C2 RID: 5570 RVA: 0x00086788 File Offset: 0x00084988
	private void ReloadVisible()
	{
		AdventureBlockSnapshot blockSnapshot = this.GetBlockSnapshot();
		bool flag = blockSnapshot == null;
		if (flag)
		{
			this.elementsHolder.gameObject.SetActive(false);
			bool flag2 = this.elementName != null;
			if (flag2)
			{
				this.elementName.gameObject.SetActive(false);
			}
		}
		else
		{
			List<int> elements = blockSnapshot.ElementCoreIds;
			this.elementsHolder.gameObject.SetActive(elements.Count > 0);
			bool flag3 = elements.Count > 0;
			if (flag3)
			{
				int elementId = elements[0];
				List<int> elementsNotIgnoreSorting = (from coreId in elements.Where(delegate(int coreId)
				{
					AdventureElementSnapshot data;
					return AdventureEditorKit.TryGetElementInAnywhere(coreId, out data) && !data.VisibleIgnoreSorting;
				})
				orderby AdventureEditorKit.GetElementFromCache(coreId).VisiblePriority
				select coreId).ToList<int>();
				List<int> elementsIgnoreSorting = (from coreId in elements.Where(delegate(int coreId)
				{
					AdventureElementSnapshot data;
					return AdventureEditorKit.TryGetElementInAnywhere(coreId, out data) && data.VisibleIgnoreSorting;
				})
				orderby AdventureEditorKit.GetElementFromCache(coreId).VisiblePriority
				select coreId).ToList<int>();
				bool flag4 = elementsNotIgnoreSorting.Count > 0;
				if (flag4)
				{
					int elementFirst = elementsNotIgnoreSorting[0];
					elementsIgnoreSorting.Add(elementFirst);
				}
				this.elementsHolder.Rebuild<AdventureMicroElementsEditorTemplate>(elementsIgnoreSorting.Count, delegate(AdventureMicroElementsEditorTemplate refer, int index)
				{
					int id = elementsIgnoreSorting[index];
					AdventureElementSnapshot data;
					AdventureEditorKit.TryGetElementInAnywhere(id, out data);
					AdventureEditorKit.SetAdventureElementIcon(refer.icon, ((data != null) ? data.Icon : null) ?? string.Empty);
					bool flag7 = ((data != null) ? data.LightData : null) != null;
					if (flag7)
					{
						AdventurePointLight elementLight = refer.icon.gameObject.GetOrAddComponent<AdventurePointLight>();
						AdventureLightData lData = data.LightData;
						Color parsedColor = Color.white;
						bool flag8 = !string.IsNullOrEmpty(lData.ColorInHex);
						if (flag8)
						{
							ColorUtility.TryParseHtmlString(lData.ColorInHex.StartsWith("#") ? lData.ColorInHex : ("#" + lData.ColorInHex), out parsedColor);
						}
						elementLight.LightColor = parsedColor;
						elementLight.Intensity = lData.Strength;
						elementLight.VirtualZ = lData.Height;
						AdventurePointLight adventurePointLight = elementLight;
						AdventureParameterSnapshot adventureParameterSnapshot = data.Parameters.FirstOrDefault((AdventureParameterSnapshot p) => p.Key == "LightPointRange");
						adventurePointLight.Range = (float)((adventureParameterSnapshot != null) ? adventureParameterSnapshot.InitialValue : 2);
						elementLight.BlockIndex = this.Index;
						elementLight.enabled = true;
					}
					else
					{
						AdventurePointLight elementLight2 = refer.icon.GetComponent<AdventurePointLight>();
						bool flag9 = elementLight2 != null;
						if (flag9)
						{
							elementLight2.enabled = false;
						}
					}
				});
				AdventureElementSnapshot elementData;
				AdventureEditorKit.TryGetElementInAnywhere(elementId, out elementData);
				bool flag5 = this.elementName != null;
				if (flag5)
				{
					this.elementName.gameObject.SetActive(true);
					this.elementName.text = ((elementData != null) ? elementData.Name : string.Empty);
				}
			}
			else
			{
				bool flag6 = this.elementName != null;
				if (flag6)
				{
					this.elementName.gameObject.SetActive(false);
				}
			}
			this.RefreshElementLighting();
		}
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x000869A8 File Offset: 0x00084BA8
	private void ReloadViewMode()
	{
		AdventureBlockSnapshot blockSnapshot = this.GetBlockSnapshot();
		bool flag = this.groundSurface != null;
		if (flag)
		{
			AdventureBlockSnapshot blockSnapshot2 = blockSnapshot;
			string icon = (blockSnapshot2 != null) ? blockSnapshot2.Icon : null;
			string iconName = (!string.IsNullOrEmpty(icon)) ? icon : "adventure_block_default";
			this.groundSurface.SetSprite(iconName, true, null);
			this.groundSurface.SetNativeSize();
			GameObject gameObject = this.blockDecoratesHolder.gameObject;
			bool active;
			if (blockSnapshot != null)
			{
				List<string> decorates = blockSnapshot.Decorates;
				if (decorates != null)
				{
					active = (decorates.Count > 0);
					goto IL_9E;
				}
			}
			active = false;
			IL_9E:
			gameObject.SetActive(active);
			bool flag2;
			if (blockSnapshot != null)
			{
				List<string> decorates = blockSnapshot.Decorates;
				if (decorates != null)
				{
					flag2 = (decorates.Count > 0);
					goto IL_CA;
				}
			}
			flag2 = false;
			IL_CA:
			bool flag3 = flag2;
			if (flag3)
			{
				this.blockDecoratesHolder.Rebuild<AdventureBlockDecoratesEditorTemplate>(blockSnapshot.Decorates.Count, delegate(AdventureBlockDecoratesEditorTemplate refer, int index)
				{
					refer.GetComponent<RectTransform>().localPosition = Vector3.zero;
					refer.decorate.SetSprite(blockSnapshot.Decorates[index], false, null);
				});
			}
		}
		this.UpdateLightingModifiers();
	}

	// Token: 0x060015C4 RID: 5572 RVA: 0x00086AB8 File Offset: 0x00084CB8
	private AdventureBlockSnapshot GetBlockSnapshot()
	{
		IReadOnlyList<AdventureBlockSnapshot> currentGroupBlocks = AdventureEditorKit.BlackBoard.CurrentGroupBlocks;
		return (currentGroupBlocks != null) ? currentGroupBlocks.FirstOrDefault((AdventureBlockSnapshot x) => x.Index == this.Index) : null;
	}

	// Token: 0x060015C5 RID: 5573 RVA: 0x00086AEC File Offset: 0x00084CEC
	private void UpdateLightingModifiers()
	{
		this.UpdateGroundLighting();
		this.UpdateVolumeLighting();
		this.RefreshDecorateLighting();
		this.RefreshElementLighting();
	}

	// Token: 0x060015C6 RID: 5574 RVA: 0x00086B0C File Offset: 0x00084D0C
	private void UpdateGroundLighting()
	{
		bool flag = this.groundSurface == null;
		if (!flag)
		{
			AdventureVertexModifier modifier = this.groundSurface.GetComponent<AdventureVertexModifier>();
			bool flag2 = modifier == null;
			if (flag2)
			{
				modifier = this.groundSurface.gameObject.AddComponent<AdventureVertexModifier>();
			}
			modifier.GridIndex = this.Index;
		}
	}

	// Token: 0x060015C7 RID: 5575 RVA: 0x00086B64 File Offset: 0x00084D64
	private void UpdateVolumeLighting()
	{
		bool flag = this.volumeController != null;
		if (flag)
		{
			this.volumeController.SetGridIndex(this.Index);
		}
		AdventureEditorVolume volumeInstance = this._volumeInstance;
		if (volumeInstance != null)
		{
			volumeInstance.RefreshLightingGridIndex(this.Index);
		}
	}

	// Token: 0x060015C8 RID: 5576 RVA: 0x00086BAC File Offset: 0x00084DAC
	private void RefreshDecorateLighting()
	{
		bool flag = this.blockDecoratesHolder == null;
		if (!flag)
		{
			this.blockDecoratesHolder.HandleChild(delegate(GameObject go, int _)
			{
				AdventureBlockDecoratesEditorTemplate refer = go.GetComponent<AdventureBlockDecoratesEditorTemplate>();
				bool flag2 = ((refer != null) ? refer.decorate : null) == null;
				if (!flag2)
				{
					AdventureVertexModifier modifier = refer.decorate.gameObject.GetOrAddComponent<AdventureVertexModifier>();
					modifier.GridIndex = this.Index;
				}
			});
		}
	}

	// Token: 0x060015C9 RID: 5577 RVA: 0x00086BE4 File Offset: 0x00084DE4
	private void RefreshElementLighting()
	{
		bool flag = this.elementsHolder == null;
		if (!flag)
		{
			this.elementsHolder.HandleChild(delegate(GameObject go, int _)
			{
				AdventureMicroElementsEditorTemplate refer = go.GetComponent<AdventureMicroElementsEditorTemplate>();
				bool flag2 = ((refer != null) ? refer.icon : null) == null;
				if (!flag2)
				{
					refer.RestoreDefaultMaterial();
					AdventureElementVertexModifier modifier = refer.icon.gameObject.GetOrAddComponent<AdventureElementVertexModifier>();
					modifier.GridIndex = this.Index;
					modifier.SetOutline(false);
				}
			});
		}
	}

	// Token: 0x040011CF RID: 4559
	private const float DiamondHalfWidth = 106f;

	// Token: 0x040011D0 RID: 4560
	private const float DiamondHalfHeight = 55f;

	// Token: 0x040011D1 RID: 4561
	private static readonly Vector2 VolumeSize = new Vector2(640f, 332f);

	// Token: 0x040011D2 RID: 4562
	private static readonly Vector2 MicroSize = new Vector2(212f, 110f);

	// Token: 0x040011D3 RID: 4563
	private static readonly Vector2[] MicroOffsets = new Vector2[]
	{
		new Vector2(0f, 1f),
		new Vector2(-0.5f, 0.5f),
		new Vector2(0.5f, 0.5f),
		new Vector2(-1f, 0f),
		new Vector2(0f, 0f),
		new Vector2(1f, 0f),
		new Vector2(-0.5f, -0.5f),
		new Vector2(0.5f, -0.5f),
		new Vector2(0f, -1f)
	};

	// Token: 0x040011D4 RID: 4564
	public static readonly int[] DataIndexToRenderIndex = new int[]
	{
		3,
		6,
		8,
		1,
		4,
		7,
		0,
		2,
		5
	};

	// Token: 0x040011D5 RID: 4565
	private const float VolumeRealFullHeight = 1840f;

	// Token: 0x040011D6 RID: 4566
	[SerializeField]
	private BlockVolumeController volumeController;

	// Token: 0x040011D7 RID: 4567
	[SerializeField]
	private CImage groundSurface;

	// Token: 0x040011D8 RID: 4568
	[SerializeField]
	private TextMeshProUGUI elementName;

	// Token: 0x040011D9 RID: 4569
	[SerializeField]
	private TextMeshProUGUI indexText;

	// Token: 0x040011DA RID: 4570
	[SerializeField]
	private CImage selectHighlight;

	// Token: 0x040011DB RID: 4571
	[SerializeField]
	private GameObject volumePrefab;

	// Token: 0x040011DC RID: 4572
	[SerializeField]
	private TemplatedContainerAssemblyNew blockDecoratesHolder;

	// Token: 0x040011DD RID: 4573
	[SerializeField]
	private TemplatedContainerAssemblyNew elementsHolder;

	// Token: 0x040011DE RID: 4574
	[SerializeField]
	private GameObject dialogBg;

	// Token: 0x040011DF RID: 4575
	private bool _isVolumeHost;

	// Token: 0x040011E0 RID: 4576
	private AdventureEditorVolume _volumeInstance;
}
