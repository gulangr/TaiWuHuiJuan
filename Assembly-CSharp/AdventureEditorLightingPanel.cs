using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.AdventureEditor.Migrate;
using GameData.Adventure;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200017D RID: 381
public class AdventureEditorLightingPanel : MonoBehaviour, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
{
	// Token: 0x06001531 RID: 5425 RVA: 0x00083134 File Offset: 0x00081334
	private void Awake()
	{
		this.presetDropdown.ClearOptions();
		this.presetDropdown.AddOptions(new List<string>
		{
			"Preset 1",
			"Preset 2"
		});
		this.presetDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnPresetChanged));
		this.lightingPreviewToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnLightingPreviewChanged));
		this.pointLightPreviewToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnPointLightPreviewChanged));
		this.pointLightGridXInput.onEndEdit.AddListener(new UnityAction<string>(this.OnPointLightIndexChanged));
		this.pointLightGridYInput.onEndEdit.AddListener(new UnityAction<string>(this.OnPointLightIndexChanged));
		this.pointLightGridIInput.onEndEdit.AddListener(new UnityAction<string>(this.OnPointLightIndexChanged));
		this.worldColorInput.onEndEdit.AddListener(new UnityAction<string>(this.OnWorldColorChanged));
		this.worldIntensityInput.OnValueChanged += this.OnWorldIntensityChanged;
		this.worldHeightInput.OnValueChanged += this.OnWorldHeightChanged;
		this.worldRotateToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnWorldRotateChanged));
		this.worldRotateAngleInput.OnValueChanged += this.OnWorldRotateAngleChanged;
		this.addStaticLightBtn.ClearAndAddListener(new Action(this.OnAddStaticLight));
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x000832BF File Offset: 0x000814BF
	public void TogglePanel()
	{
		this.SetPanelVisible(!base.gameObject.activeSelf);
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x000832D8 File Offset: 0x000814D8
	private void SetPanelVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
		if (visible)
		{
			this.ReloadFromBlackBoard();
		}
	}

	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06001534 RID: 5428 RVA: 0x000832FF File Offset: 0x000814FF
	bool IAdventureBlackBoardElement<EAdventureEditType>.LoadOnEdit
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000264 RID: 612
	// (get) Token: 0x06001535 RID: 5429 RVA: 0x00083302 File Offset: 0x00081502
	bool IAdventureBlackBoardElement<EAdventureEditType>.LoadOnRegister
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x00083308 File Offset: 0x00081508
	void IAdventureBlackBoardElement<EAdventureEditType>.Load(EAdventureEditType editType)
	{
		bool flag = editType.Contains(EAdventureEditType.All) || editType.Contains(EAdventureEditType.Basic) || editType.Contains(EAdventureEditType.BlockViewMode);
		if (flag)
		{
			this.ReloadFromBlackBoard();
		}
	}

	// Token: 0x06001537 RID: 5431 RVA: 0x00083340 File Offset: 0x00081540
	private void ReloadFromBlackBoard()
	{
		AdventureSnapshot snapshot = AdventureEditorKit.BlackBoard.Editing;
		AdventureBlockIndex previewCoordinate = this.editorRoot.PointLightPreviewIndex;
		this._isRefreshing = true;
		this.lightingPreviewToggle.isOn = this.editorRoot.LightingPreviewEnabled;
		this.pointLightPreviewToggle.isOn = this.editorRoot.PointLightPreviewEnabled;
		this.pointLightGridXInput.SetTextWithoutNotify(previewCoordinate.X.ToString());
		this.pointLightGridYInput.SetTextWithoutNotify(previewCoordinate.Y.ToString());
		this.pointLightGridIInput.SetTextWithoutNotify(previewCoordinate.I.ToString());
		this.worldColorInput.SetTextWithoutNotify(snapshot.LightingWorld.ColorInHex);
		this.worldIntensityInput.Set(snapshot.LightingWorld.Strength);
		this.worldHeightInput.Set(snapshot.LightingWorld.Height);
		this.worldRotateToggle.isOn = snapshot.LightingRotate;
		this.worldRotateAngleInput.Set((float)snapshot.LightingRotateAngle);
		this.worldRotateAngleInput.SetInteractable(snapshot.LightingRotate);
		this.taiwuPointLightComponent.ReloadFromBlackBoard();
		this.RebuildStaticLightItems();
		this._isRefreshing = false;
	}

	// Token: 0x06001538 RID: 5432 RVA: 0x00083478 File Offset: 0x00081678
	private void OnLightingPreviewChanged(bool enabled)
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			this.editorRoot.SetLightingPreviewEnabled(enabled);
		}
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x000834A0 File Offset: 0x000816A0
	private void OnPointLightPreviewChanged(bool enabled)
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			this.editorRoot.SetPointLightPreviewEnabled(enabled);
		}
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x000834C8 File Offset: 0x000816C8
	private void OnPointLightIndexChanged(string value)
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			AdventureBlockIndex index = this.editorRoot.PointLightPreviewIndex;
			int x;
			bool flag = int.TryParse(this.pointLightGridXInput.text, out x);
			if (flag)
			{
				index = index.SetX(x);
			}
			int y;
			bool flag2 = int.TryParse(this.pointLightGridYInput.text, out y);
			if (flag2)
			{
				index = index.SetY(y);
			}
			int i;
			bool flag3 = int.TryParse(this.pointLightGridIInput.text, out i);
			if (flag3)
			{
				index = index.SetI(i);
			}
			this.editorRoot.SetPointLightPreviewGridCoordinate(index);
			this.pointLightGridXInput.SetTextWithoutNotify(index.X.ToString());
			this.pointLightGridYInput.SetTextWithoutNotify(index.Y.ToString());
			this.pointLightGridIInput.SetTextWithoutNotify(index.I.ToString());
		}
	}

	// Token: 0x0600153B RID: 5435 RVA: 0x000835AC File Offset: 0x000817AC
	private void OnWorldColorChanged(string value)
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			string normalized;
			bool flag = !AdventureEditorLightingPanel.TryNormalizeColorHex(value, out normalized);
			if (flag)
			{
				this.worldColorInput.SetTextWithoutNotify(AdventureEditorKit.BlackBoard.Editing.LightingWorld.ColorInHex);
			}
			else
			{
				AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
				{
					snapshot.LightingWorld.ColorInHex = normalized;
				}, EAdventureEditType.Basic);
				this.worldColorInput.SetTextWithoutNotify(normalized);
			}
		}
	}

	// Token: 0x0600153C RID: 5436 RVA: 0x0008362C File Offset: 0x0008182C
	private void OnWorldIntensityChanged()
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				snapshot.LightingWorld.Strength = this.worldIntensityInput.Value;
			}, EAdventureEditType.Basic);
		}
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x00083660 File Offset: 0x00081860
	private void OnWorldHeightChanged()
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				snapshot.LightingWorld.Height = this.worldHeightInput.Value;
			}, EAdventureEditType.Basic);
		}
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x00083694 File Offset: 0x00081894
	private void OnWorldRotateChanged(bool enabled)
	{
		this.worldRotateAngleInput.SetInteractable(enabled);
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				snapshot.LightingRotate = enabled;
			}, EAdventureEditType.Basic);
		}
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x000836E8 File Offset: 0x000818E8
	private void OnWorldRotateAngleChanged()
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				snapshot.LightingRotateAngle = this.worldRotateAngleInput.ValueInt;
			}, EAdventureEditType.Basic);
		}
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x0008371C File Offset: 0x0008191C
	private void OnPresetChanged(int index)
	{
		bool flag = this._isRefreshing || index < 0 || index >= AdventureEditorLightingPanel.Presets.Length;
		if (!flag)
		{
			AdventureEditorLightingPanel.LightingPreset preset = AdventureEditorLightingPanel.Presets[index];
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot snapshot)
			{
				snapshot.LightingWorld.ColorInHex = preset.WorldColorHex;
				snapshot.LightingWorld.Strength = preset.WorldIntensity;
				snapshot.LightingWorld.Height = preset.WorldHeight;
				snapshot.LightingTaiwu.ColorInHex = preset.PointColorHex;
				snapshot.LightingTaiwu.Strength = preset.PointIntensity;
				snapshot.LightingTaiwu.Height = preset.PointVirtualZ;
			}, EAdventureEditType.Basic);
			this.ReloadFromBlackBoard();
		}
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x00083784 File Offset: 0x00081984
	private void RebuildStaticLightItems()
	{
		foreach (AdventureEditorStaticPointLightItem item in this._staticLightItems)
		{
			Object.Destroy(item.gameObject);
		}
		this._staticLightItems.Clear();
		List<AdventurePointLightSnapshot> list = AdventureEditorKit.BlackBoard.Editing.LightingPoints;
		for (int i = 0; i < list.Count; i++)
		{
			AdventureEditorStaticPointLightItem item2 = Object.Instantiate<AdventureEditorStaticPointLightItem>(this.staticLightItemPrefab, this.staticLightContainer);
			int captured = i;
			item2.Setup(i, delegate
			{
				this.RemoveStaticLightItem(captured);
			});
			this._staticLightItems.Add(item2);
			item2.gameObject.SetActive(true);
		}
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x00083870 File Offset: 0x00081A70
	private void OnAddStaticLight()
	{
		int count = AdventureEditorKit.BlackBoard.Editing.LightingPoints.Count;
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot s)
		{
			s.LightingPoints.Add(new AdventurePointLightSnapshot
			{
				Index = AdventureBlockIndex.XyToCenter(0, 0),
				LightData = new AdventureLightData
				{
					ColorInHex = "FFFFFF",
					Strength = 1f,
					Height = 60f
				},
				Range = 1
			});
		}, EAdventureEditType.Basic);
		this.RebuildStaticLightItems();
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x000838C8 File Offset: 0x00081AC8
	private void RemoveStaticLightItem(int index)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot s)
		{
			s.LightingPoints.RemoveAt(index);
		}, EAdventureEditType.Basic);
		this.RebuildStaticLightItems();
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x00083904 File Offset: 0x00081B04
	internal static bool TryNormalizeColorHex(string value, out string normalized)
	{
		string trimmed = ((value != null) ? value.Trim() : null) ?? string.Empty;
		bool flag = string.IsNullOrEmpty(trimmed);
		bool result;
		if (flag)
		{
			normalized = string.Empty;
			result = true;
		}
		else
		{
			bool flag2 = trimmed.StartsWith("#");
			if (flag2)
			{
				string text = trimmed;
				trimmed = text.Substring(1, text.Length - 1);
			}
			string htmlColor = "#" + trimmed;
			Color color;
			bool flag3 = !ColorUtility.TryParseHtmlString(htmlColor, out color);
			if (flag3)
			{
				normalized = string.Empty;
				result = false;
			}
			else
			{
				normalized = trimmed.ToUpperInvariant();
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0400119A RID: 4506
	private static readonly AdventureEditorLightingPanel.LightingPreset[] Presets = new AdventureEditorLightingPanel.LightingPreset[]
	{
		new AdventureEditorLightingPanel.LightingPreset(230f, "536F8C", 1.2f, 50f, "EAE4AC", 1.5f, 80f),
		new AdventureEditorLightingPanel.LightingPreset(230f, "536F8C", 1.1f, 50f, "EAE4AC", 1.3f, 200f)
	};

	// Token: 0x0400119B RID: 4507
	[SerializeField]
	private UI_AdventureEditorRemake editorRoot;

	// Token: 0x0400119C RID: 4508
	[SerializeField]
	private CDropdown presetDropdown;

	// Token: 0x0400119D RID: 4509
	[SerializeField]
	private CToggle lightingPreviewToggle;

	// Token: 0x0400119E RID: 4510
	[SerializeField]
	private CToggle pointLightPreviewToggle;

	// Token: 0x0400119F RID: 4511
	[SerializeField]
	private TMP_InputField pointLightGridXInput;

	// Token: 0x040011A0 RID: 4512
	[SerializeField]
	private TMP_InputField pointLightGridYInput;

	// Token: 0x040011A1 RID: 4513
	[SerializeField]
	private TMP_InputField pointLightGridIInput;

	// Token: 0x040011A2 RID: 4514
	[SerializeField]
	private TMP_InputField worldColorInput;

	// Token: 0x040011A3 RID: 4515
	[SerializeField]
	private InputCSlider worldIntensityInput;

	// Token: 0x040011A4 RID: 4516
	[SerializeField]
	private InputCSlider worldHeightInput;

	// Token: 0x040011A5 RID: 4517
	[SerializeField]
	private CToggle worldRotateToggle;

	// Token: 0x040011A6 RID: 4518
	[SerializeField]
	private InputCSlider worldRotateAngleInput;

	// Token: 0x040011A7 RID: 4519
	[SerializeField]
	private AdventureEditorPointLightComponent taiwuPointLightComponent;

	// Token: 0x040011A8 RID: 4520
	[SerializeField]
	private CButton addStaticLightBtn;

	// Token: 0x040011A9 RID: 4521
	[SerializeField]
	private Transform staticLightContainer;

	// Token: 0x040011AA RID: 4522
	[SerializeField]
	private AdventureEditorStaticPointLightItem staticLightItemPrefab;

	// Token: 0x040011AB RID: 4523
	private bool _isRefreshing;

	// Token: 0x040011AC RID: 4524
	private readonly List<AdventureEditorStaticPointLightItem> _staticLightItems = new List<AdventureEditorStaticPointLightItem>();

	// Token: 0x02001295 RID: 4757
	private readonly struct LightingPreset
	{
		// Token: 0x0600C647 RID: 50759 RVA: 0x005821DF File Offset: 0x005803DF
		public LightingPreset(float worldAzimuthAngle, string worldColorHex, float worldIntensity, float worldHeight, string pointColorHex, float pointIntensity, float pointVirtualZ)
		{
			this.WorldAzimuthAngle = worldAzimuthAngle;
			this.WorldColorHex = worldColorHex;
			this.WorldIntensity = worldIntensity;
			this.WorldHeight = worldHeight;
			this.PointColorHex = pointColorHex;
			this.PointIntensity = pointIntensity;
			this.PointVirtualZ = pointVirtualZ;
		}

		// Token: 0x1700162B RID: 5675
		// (get) Token: 0x0600C648 RID: 50760 RVA: 0x00582217 File Offset: 0x00580417
		public float WorldAzimuthAngle { get; }

		// Token: 0x1700162C RID: 5676
		// (get) Token: 0x0600C649 RID: 50761 RVA: 0x0058221F File Offset: 0x0058041F
		public string WorldColorHex { get; }

		// Token: 0x1700162D RID: 5677
		// (get) Token: 0x0600C64A RID: 50762 RVA: 0x00582227 File Offset: 0x00580427
		public float WorldIntensity { get; }

		// Token: 0x1700162E RID: 5678
		// (get) Token: 0x0600C64B RID: 50763 RVA: 0x0058222F File Offset: 0x0058042F
		public float WorldHeight { get; }

		// Token: 0x1700162F RID: 5679
		// (get) Token: 0x0600C64C RID: 50764 RVA: 0x00582237 File Offset: 0x00580437
		public string PointColorHex { get; }

		// Token: 0x17001630 RID: 5680
		// (get) Token: 0x0600C64D RID: 50765 RVA: 0x0058223F File Offset: 0x0058043F
		public float PointIntensity { get; }

		// Token: 0x17001631 RID: 5681
		// (get) Token: 0x0600C64E RID: 50766 RVA: 0x00582247 File Offset: 0x00580447
		public float PointVirtualZ { get; }
	}
}
