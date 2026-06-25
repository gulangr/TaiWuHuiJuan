using System;
using System.Collections.Generic;
using System.Linq;
using AiEditor;
using Game.Views.Adventure;
using GameData.Adventure;
using GameData.Adventure.Editor;
using SubEditor.AdventureCommonRefersListEditor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000170 RID: 368
public class AdventureEditorBlock : MonoBehaviour, ISelectAndDragComponent, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
{
	// Token: 0x17000243 RID: 579
	// (get) Token: 0x06001455 RID: 5205 RVA: 0x0007ED52 File Offset: 0x0007CF52
	// (set) Token: 0x06001456 RID: 5206 RVA: 0x0007ED5A File Offset: 0x0007CF5A
	public AdventureBlockIndex Index { get; private set; }

	// Token: 0x06001457 RID: 5207 RVA: 0x0007ED63 File Offset: 0x0007CF63
	private void OnEnable()
	{
		AdventureEditorKit.BlackBoard.Register(this);
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x0007ED72 File Offset: 0x0007CF72
	private void OnDisable()
	{
		AdventureEditorKit.BlackBoard.Unregister(this);
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x0007ED81 File Offset: 0x0007CF81
	private void Start()
	{
		this.groupPalette.paletteChanged.AddListener(new UnityAction<Color[]>(this.OnGroupPaletteChanged));
	}

	// Token: 0x0600145A RID: 5210 RVA: 0x0007EDA1 File Offset: 0x0007CFA1
	private void OnDestroy()
	{
		this.groupPalette.paletteChanged.RemoveListener(new UnityAction<Color[]>(this.OnGroupPaletteChanged));
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x0007EDC4 File Offset: 0x0007CFC4
	private void Update()
	{
		bool isActiveAndEnabled = this.groupColor.isActiveAndEnabled;
		if (isActiveAndEnabled)
		{
			this.groupColor.color = this.groupColor.color.SetAlpha(this.groupPalette.FlashValue);
		}
	}

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x0600145C RID: 5212 RVA: 0x0007EE08 File Offset: 0x0007D008
	public RectTransform RectTransform
	{
		get
		{
			return (RectTransform)base.transform;
		}
	}

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x0600145D RID: 5213 RVA: 0x0007EE15 File Offset: 0x0007D015
	bool IAdventureEditorBlackBoardElement.AutoLoad
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000246 RID: 582
	// (get) Token: 0x0600145E RID: 5214 RVA: 0x0007EE18 File Offset: 0x0007D018
	bool IAdventureBlackBoardElement<EAdventureEditType>.LoadOnRegister
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600145F RID: 5215 RVA: 0x0007EE1C File Offset: 0x0007D01C
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
		}
		bool flag3 = editType.Contains(EAdventureEditType.Basic);
		if (flag3)
		{
			this.ReloadPointLightMarker();
		}
	}

	// Token: 0x06001460 RID: 5216 RVA: 0x0007EE60 File Offset: 0x0007D060
	public void Set(AdventureBlockIndex index)
	{
		this.Unselect();
		bool needActive = !base.gameObject.activeSelf;
		bool needReload = !needActive && this.Index != index;
		this.Index = index;
		this.text.text = this.Index.ToString();
		bool flag = needReload;
		if (flag)
		{
			this.ReloadAll();
		}
		else
		{
			bool flag2 = needActive;
			if (flag2)
			{
				base.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06001461 RID: 5217 RVA: 0x0007EEE4 File Offset: 0x0007D0E4
	private AdventureBlockSnapshot GetBlockSnapshot()
	{
		IReadOnlyList<AdventureBlockSnapshot> currentBlocks = AdventureEditorKit.BlackBoard.CurrentGroupBlocks;
		bool flag = currentBlocks == null;
		AdventureBlockSnapshot result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = currentBlocks.FirstOrDefault((AdventureBlockSnapshot x) => x.Index == this.Index);
		}
		return result;
	}

	// Token: 0x06001462 RID: 5218 RVA: 0x0007EF20 File Offset: 0x0007D120
	private AdventureBlockSnapshot GetBlockSnapshot(AdventureSnapshot snapshot)
	{
		return snapshot.Blocks.FirstOrDefault((AdventureBlockSnapshot x) => x.Index == this.Index);
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x0007EF49 File Offset: 0x0007D149
	private void ReloadAll()
	{
		this.ReloadVisible();
		this.ReloadViewMode();
	}

	// Token: 0x06001464 RID: 5220 RVA: 0x0007EF5C File Offset: 0x0007D15C
	private void ReloadVisible()
	{
		AdventureBlockSnapshot blockSnapshot = this.GetBlockSnapshot();
		bool flag = blockSnapshot == null;
		if (flag)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			base.gameObject.SetActive(true);
			this.OnGroupPaletteChanged(this.groupPalette.GetCurrent());
			this.blockType.GetComponent<TextMeshProUGUI>().text = AdventureEditorBlock.EAdventureBlockTypeNames[blockSnapshot.BlockType];
			this.ReloadElements();
			this.ReloadPointLightMarker();
		}
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x0007EFD8 File Offset: 0x0007D1D8
	private void ReloadViewMode()
	{
		bool flag = AdventureEditorKit.BlackBoard.ViewMode > EBlockViewMode.Default;
		if (flag)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			AdventureBlockSnapshot blockSnapshot = this.GetBlockSnapshot();
			CImage blockImage = base.GetComponent<CImage>();
			RectTransform blockImgRect = this.blockImg.GetComponent<RectTransform>();
			blockImgRect.SetSize(AdventureEditorBlock.GridSize);
			this.volume.SetActive(false);
			BlockVolumeController blockVolumeController = base.gameObject.GetComponent<BlockVolumeController>();
			float offsetX = this.offset.x * (float)this.Index.X + this.subOffset.x * (float)this.Index.Ix;
			float offsetY = this.offset.y * (float)this.Index.Y + this.subOffset.y * (float)this.Index.Iy;
			blockImage.SetSprite(string.Empty, false, null);
			this.RectTransform.SetSize(AdventureEditorBlock.GridSize);
			blockImage.enabled = true;
			this.RectTransform.eulerAngles = this.RectTransform.eulerAngles.SetZ(0f);
			Vector3 localScale = Vector3.one;
			this.elementIcon.rectTransform.localScale = localScale;
			this.elementName.rectTransform.localScale = localScale;
			this.text.rectTransform.localScale = localScale;
			this.text.fontSize = 25f;
			this.blockType.GetComponent<RectTransform>().localScale = localScale;
			RectTransform textRt = this.text.GetComponent<RectTransform>();
			textRt.anchoredPosition = new Vector2(0f, -50f);
			RectTransform blockTypeRt = this.blockType.GetComponent<RectTransform>();
			blockTypeRt.anchoredPosition = new Vector2(0f, -15f);
			this.RectTransform.anchoredPosition = new Vector2(offsetX, offsetY);
			bool flag2 = blockSnapshot == null;
			if (flag2)
			{
				this.elementIcon.gameObject.SetActive(false);
				this.elementName.gameObject.SetActive(false);
			}
			this.blockImg.enabled = false;
			blockImage.enabled = true;
			blockVolumeController.VolumeHeight = 0f;
			this.OnGroupPaletteChanged(this.groupPalette.GetCurrent());
			this.ReloadPointLightMarker();
		}
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x0007F22C File Offset: 0x0007D42C
	public void Select()
	{
		this.image.color = this.selectColor;
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x0007F241 File Offset: 0x0007D441
	public void Unselect()
	{
		this.image.color = this.unselectColor;
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x0007F256 File Offset: 0x0007D456
	public void EditElements(AdventureBlockElementsEditor editor)
	{
		AdventureEditorKit.UpdateElementCache();
		editor.TargetIndex = this.Index;
		editor.SetEditableList(null);
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x0007F274 File Offset: 0x0007D474
	public void OnGroupPaletteChanged(Color[] palette)
	{
		AdventureEditorBlock.<>c__DisplayClass41_0 CS$<>8__locals1 = new AdventureEditorBlock.<>c__DisplayClass41_0();
		AdventureEditorBlock.<>c__DisplayClass41_0 CS$<>8__locals2 = CS$<>8__locals1;
		AdventureBlockSnapshot blockSnapshot = this.GetBlockSnapshot();
		CS$<>8__locals2.groupIds = ((blockSnapshot != null) ? blockSnapshot.GroupIds : null);
		Color initialColor = new Color(0f, 0f, 0f, 0f);
		this.groupColor.color = palette.Where(delegate(Color _, int index)
		{
			List<int> groupIds = CS$<>8__locals1.groupIds;
			return groupIds != null && groupIds.Contains(index);
		}).Aggregate(initialColor, (Color current, Color c) => Color.Lerp(current, c, Mathf.Approximately(current.a, 0f) ? 1f : (c.a * 0.5f)));
		this.groupColor.enabled = (this.groupColor.color != initialColor && AdventureEditorKit.BlackBoard.ViewMode == EBlockViewMode.Default);
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x0007F330 File Offset: 0x0007D530
	private void ReloadPointLightMarker()
	{
		bool flag = this.pointLightMarker == null;
		if (!flag)
		{
			bool hasLight = AdventureEditorKit.BlackBoard.Editing.LightingPoints.Any((AdventurePointLightSnapshot lp) => lp.Index == this.Index);
			this.pointLightMarker.SetActive(hasLight);
		}
	}

	// Token: 0x0600146B RID: 5227 RVA: 0x0007F380 File Offset: 0x0007D580
	public void ReloadElements()
	{
		AdventureBlockSnapshot blockSnapshot = this.GetBlockSnapshot();
		List<int> elements = (blockSnapshot != null) ? blockSnapshot.ElementCoreIds : null;
		List<int> list = elements;
		List<int> list2 = list;
		if (list2 != null)
		{
			int count = list2.Count;
			if (count > 1)
			{
				this.elementName.gameObject.SetActive(true);
				this.elementName.text = LanguageKey.LK_AdventureEditor_Element_Multi.TrFormat(string.Format("x {0}", elements.Count));
				this.elementIcon.gameObject.SetActive(false);
				return;
			}
			if (count == 1)
			{
				int elementId = elements[0];
				AdventureElementSnapshot elementData;
				AdventureEditorKit.TryGetElementInAnywhere(elementId, out elementData);
				this.elementName.gameObject.SetActive(true);
				this.elementName.text = ((elementData != null) ? elementData.Name : string.Empty);
				this.elementIcon.gameObject.SetActive(true);
				AdventureEditorKit.SetAdventureElementIcon(this.elementIcon, ((elementData != null) ? elementData.Icon : null) ?? string.Empty);
				return;
			}
		}
		this.elementName.gameObject.SetActive(false);
		this.elementIcon.gameObject.SetActive(false);
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x0007F4BE File Offset: 0x0007D6BE
	public void Reload()
	{
		this.ReloadVisible();
	}

	// Token: 0x04001118 RID: 4376
	private static readonly Vector2 GridSize = new Vector2(100f, 100f);

	// Token: 0x04001119 RID: 4377
	private static readonly IDictionary<EAdventureBlockType, string> EAdventureBlockTypeNames = new Dictionary<EAdventureBlockType, string>
	{
		{
			EAdventureBlockType.In,
			"I"
		},
		{
			EAdventureBlockType.Out,
			"O"
		},
		{
			EAdventureBlockType.InOut,
			"IO"
		},
		{
			EAdventureBlockType.None,
			string.Empty
		}
	};

	// Token: 0x0400111A RID: 4378
	[SerializeField]
	private Vector2 offset = new Vector2(360f, 360f);

	// Token: 0x0400111B RID: 4379
	[SerializeField]
	private Vector2 subOffset = new Vector2(110f, 110f);

	// Token: 0x0400111C RID: 4380
	[SerializeField]
	private Image image;

	// Token: 0x0400111D RID: 4381
	[SerializeField]
	private Graphic groupColor;

	// Token: 0x0400111E RID: 4382
	[SerializeField]
	private Color selectColor;

	// Token: 0x0400111F RID: 4383
	[SerializeField]
	private Color unselectColor;

	// Token: 0x04001120 RID: 4384
	[SerializeField]
	private TextMeshProUGUI text;

	// Token: 0x04001121 RID: 4385
	[SerializeField]
	private TextMeshProUGUI elementName;

	// Token: 0x04001122 RID: 4386
	[SerializeField]
	private CImage elementIcon;

	// Token: 0x04001123 RID: 4387
	[SerializeField]
	private GameObject blockType;

	// Token: 0x04001124 RID: 4388
	[SerializeField]
	private AdventureEditorGroupPalette groupPalette;

	// Token: 0x04001125 RID: 4389
	[SerializeField]
	private CImage blockImg;

	// Token: 0x04001126 RID: 4390
	[SerializeField]
	private GameObject volume;

	// Token: 0x04001127 RID: 4391
	[SerializeField]
	private GameObject pointLightMarker;
}
