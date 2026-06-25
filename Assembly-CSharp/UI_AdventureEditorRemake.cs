using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using Game.Views.Adventure;
using GameData.Adventure;
using GameData.Adventure.Editor;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200019C RID: 412
public class UI_AdventureEditorRemake : UIBase, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
{
	// Token: 0x17000283 RID: 643
	// (get) Token: 0x060016F3 RID: 5875 RVA: 0x0008C6F0 File Offset: 0x0008A8F0
	public bool LightingPreviewEnabled
	{
		get
		{
			return this._lightingPreviewEnabled;
		}
	}

	// Token: 0x17000284 RID: 644
	// (get) Token: 0x060016F4 RID: 5876 RVA: 0x0008C6F8 File Offset: 0x0008A8F8
	public bool PointLightPreviewEnabled
	{
		get
		{
			return this._pointLightPreviewEnabled;
		}
	}

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x060016F5 RID: 5877 RVA: 0x0008C700 File Offset: 0x0008A900
	public AdventureBlockIndex PointLightPreviewIndex
	{
		get
		{
			return this._pointLightPreviewIndex;
		}
	}

	// Token: 0x17000286 RID: 646
	// (get) Token: 0x060016F6 RID: 5878 RVA: 0x0008C708 File Offset: 0x0008A908
	public float WorldAzimuthAngle
	{
		get
		{
			return (this._lightingManager != null) ? this._lightingManager.GlobalAzimuthAngle : this._defaultGlobalAzimuthAngle;
		}
	}

	// Token: 0x060016F7 RID: 5879 RVA: 0x0008C72C File Offset: 0x0008A92C
	public override void OnInit(ArgumentBox argsBox)
	{
		this._editingPath = string.Empty;
		this._lightingPreviewEnabled = true;
		this._pointLightPreviewEnabled = true;
		this._pointLightPreviewIndex = AdventureBlockIndex.Center;
		AdventureEditorKit.BlackBoard.ResetToEmpty();
		this.elementArea.RedirectAndClear();
		UnityEvent unityEvent = this.onActivate;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		this.RefreshLightingState();
	}

	// Token: 0x060016F8 RID: 5880 RVA: 0x0008C790 File Offset: 0x0008A990
	private void OnEnable()
	{
		bool flag = !AdventureEditorKit.CheckCorePath();
		if (!flag)
		{
			Tester.Assert(this._loadedElements == null, "Unload failed by unknown reason.");
			this._loadedElements = base.GetComponentsInChildren<IAdventureEditorBlackBoardElement>(true);
			foreach (IAdventureEditorBlackBoardElement element in this._loadedElements)
			{
				bool autoLoad = element.AutoLoad;
				if (autoLoad)
				{
					AdventureEditorKit.BlackBoard.Register(element);
				}
			}
		}
	}

	// Token: 0x060016F9 RID: 5881 RVA: 0x0008C820 File Offset: 0x0008AA20
	private void OnDisable()
	{
		this.Unload();
	}

	// Token: 0x060016FA RID: 5882 RVA: 0x0008C82C File Offset: 0x0008AA2C
	private void Unload()
	{
		bool flag = this._loadedElements == null;
		if (!flag)
		{
			bool enabled = this.previewPointLight.enabled;
			if (enabled)
			{
				this.previewPointLight.enabled = false;
			}
			this.ResetPreviewPointLightTransform();
			this.ClearStaticPreviewLights();
			foreach (IAdventureEditorBlackBoardElement element in this._loadedElements)
			{
				bool autoLoad = element.AutoLoad;
				if (autoLoad)
				{
					AdventureEditorKit.BlackBoard.Unregister(element);
				}
			}
			this._loadedElements = null;
		}
	}

	// Token: 0x060016FB RID: 5883 RVA: 0x0008C8D0 File Offset: 0x0008AAD0
	private void ClearStaticPreviewLights()
	{
		foreach (AdventurePointLight light in this._staticPreviewLights)
		{
			bool flag = light != null;
			if (flag)
			{
				Object.Destroy(light.gameObject);
			}
		}
		this._staticPreviewLights.Clear();
	}

	// Token: 0x17000287 RID: 647
	// (get) Token: 0x060016FC RID: 5884 RVA: 0x0008C944 File Offset: 0x0008AB44
	bool IAdventureBlackBoardElement<EAdventureEditType>.LoadOnRegister
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060016FD RID: 5885 RVA: 0x0008C948 File Offset: 0x0008AB48
	void IAdventureBlackBoardElement<EAdventureEditType>.Load(EAdventureEditType editType)
	{
		bool flag = editType.Contains(EAdventureEditType.All) || editType.Contains(EAdventureEditType.Basic) || editType.Contains(EAdventureEditType.BlockViewMode);
		if (flag)
		{
			this.RefreshLightingState();
		}
	}

	// Token: 0x060016FE RID: 5886 RVA: 0x0008C980 File Offset: 0x0008AB80
	private void RefreshLightingState()
	{
		if (this._lightingManager == null)
		{
			this._lightingManager = base.GetComponent<AdventureLightingManager>();
		}
		this.CacheLightingDefaults();
		this.ApplyLightingData(AdventureEditorKit.BlackBoard.Editing);
		bool isSimulateMode = AdventureEditorKit.BlackBoard.ViewMode == EBlockViewMode.Simulate;
		bool enableLighting = isSimulateMode && this._lightingPreviewEnabled;
		this._lightingManager.EnableCustomLighting = enableLighting;
		this.previewPointLight.BlockIndex = this._pointLightPreviewIndex;
		bool pointLightEnabled = enableLighting && this._pointLightPreviewEnabled && this.UpdatePreviewPointLightTransform();
		this.previewPointLight.gameObject.SetActive(pointLightEnabled);
		this.previewPointLight.enabled = pointLightEnabled;
		bool flag = !enableLighting;
		if (flag)
		{
			this.ResetPreviewPointLightTransform();
			this.ClearStaticPreviewLights();
		}
		else
		{
			if (this._normalMapBinder == null)
			{
				this._normalMapBinder = base.GetComponent<AdventureNormalMapBinder>();
			}
			AdventureNormalMapBinder normalMapBinder = this._normalMapBinder;
			if (normalMapBinder != null)
			{
				normalMapBinder.TryBindAllAtlases();
			}
			this.RefreshStaticPreviewLights();
		}
	}

	// Token: 0x060016FF RID: 5887 RVA: 0x0008CA70 File Offset: 0x0008AC70
	public void SetLightingPreviewEnabled(bool enabled)
	{
		bool flag = this._lightingPreviewEnabled == enabled;
		if (!flag)
		{
			this._lightingPreviewEnabled = enabled;
			this.RefreshLightingState();
		}
	}

	// Token: 0x06001700 RID: 5888 RVA: 0x0008CA9C File Offset: 0x0008AC9C
	public void SetPointLightPreviewEnabled(bool enabled)
	{
		bool flag = this._pointLightPreviewEnabled == enabled;
		if (!flag)
		{
			this._pointLightPreviewEnabled = enabled;
			this.RefreshLightingState();
		}
	}

	// Token: 0x06001701 RID: 5889 RVA: 0x0008CAC8 File Offset: 0x0008ACC8
	public void SetPointLightPreviewGridCoordinate(AdventureBlockIndex index)
	{
		bool flag = this._pointLightPreviewIndex == index;
		if (!flag)
		{
			this._pointLightPreviewIndex = index;
			this.RefreshLightingState();
		}
	}

	// Token: 0x06001702 RID: 5890 RVA: 0x0008CAF8 File Offset: 0x0008ACF8
	public void SetWorldAzimuthAngle(float angle)
	{
		this.CacheLightingDefaults();
		float normalizedAngle = UI_AdventureEditorRemake.NormalizeAngle(angle);
		bool flag = Mathf.Approximately(this.WorldAzimuthAngle, normalizedAngle);
		if (!flag)
		{
			this._lightingManager.GlobalAzimuthAngle = normalizedAngle;
			this.RefreshLightingState();
		}
	}

	// Token: 0x06001703 RID: 5891 RVA: 0x0008CB3C File Offset: 0x0008AD3C
	private void CacheLightingDefaults()
	{
		bool lightingDefaultsInitialized = this._lightingDefaultsInitialized;
		if (!lightingDefaultsInitialized)
		{
			this._defaultGlobalColor = this._lightingManager.GlobalColor;
			this._defaultGlobalIntensity = this._lightingManager.GlobalIntensity;
			this._defaultGlobalIncidenceAngle = this._lightingManager.GlobalIncidenceAngle;
			this._defaultGlobalAzimuthAngle = this._lightingManager.GlobalAzimuthAngle;
			this._defaultRotationAnglePerStep = this._lightingManager.RotationAnglePerStep;
			this._defaultPointLightColor = this.previewPointLight.LightColor;
			this._defaultPointLightIntensity = this.previewPointLight.Intensity;
			this._defaultPointLightVirtualZ = this.previewPointLight.VirtualZ;
			Transform previewTransform = this.previewPointLight.transform;
			this._defaultPreviewPointLightParent = base.transform;
			this._defaultPreviewPointLightLocalPosition = previewTransform.localPosition;
			this._defaultPreviewPointLightLocalRotation = previewTransform.localRotation;
			this._defaultPreviewPointLightLocalScale = previewTransform.localScale;
			this._lightingDefaultsInitialized = true;
		}
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x0008CC24 File Offset: 0x0008AE24
	private void ApplyLightingData(AdventureSnapshot snapshot)
	{
		this.ApplyWorldLighting(snapshot);
		this.ApplyPointLighting(snapshot);
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x0008CC38 File Offset: 0x0008AE38
	private void ApplyWorldLighting(AdventureSnapshot snapshot)
	{
		bool flag = ((snapshot != null) ? snapshot.LightingWorld : null) == null || !UI_AdventureEditorRemake.HasWorldLightingData(snapshot.LightingWorld);
		if (flag)
		{
			this._lightingManager.GlobalColor = this._defaultGlobalColor;
			this._lightingManager.GlobalIntensity = this._defaultGlobalIntensity;
			this._lightingManager.GlobalIncidenceAngle = this._defaultGlobalIncidenceAngle;
		}
		else
		{
			this._lightingManager.GlobalColor = UI_AdventureEditorRemake.TryParseColor(snapshot.LightingWorld.ColorInHex, this._defaultGlobalColor);
			this._lightingManager.GlobalIntensity = snapshot.LightingWorld.Strength;
			this._lightingManager.GlobalIncidenceAngle = Mathf.Clamp(snapshot.LightingWorld.Height, 0f, 90f);
		}
		this._lightingManager.GlobalAzimuthAngle = UI_AdventureEditorRemake.NormalizeAngle(this._lightingManager.GlobalAzimuthAngle);
		this._lightingManager.RotationAnglePerStep = ((snapshot == null) ? this._defaultRotationAnglePerStep : (snapshot.LightingRotate ? ((float)snapshot.LightingRotateAngle) : 0f));
	}

	// Token: 0x06001706 RID: 5894 RVA: 0x0008CD48 File Offset: 0x0008AF48
	private void ApplyPointLighting(AdventureSnapshot snapshot)
	{
		bool flag = ((snapshot != null) ? snapshot.LightingTaiwu : null) == null || !UI_AdventureEditorRemake.HasPointLightingData(snapshot.LightingTaiwu);
		if (flag)
		{
			this.previewPointLight.LightColor = this._defaultPointLightColor;
			this.previewPointLight.Intensity = this._defaultPointLightIntensity;
			this.previewPointLight.VirtualZ = this._defaultPointLightVirtualZ;
		}
		else
		{
			this.previewPointLight.LightColor = UI_AdventureEditorRemake.TryParseColor(snapshot.LightingTaiwu.ColorInHex, this._defaultPointLightColor);
			this.previewPointLight.Intensity = snapshot.LightingTaiwu.Strength;
			this.previewPointLight.VirtualZ = snapshot.LightingTaiwu.Height;
		}
	}

	// Token: 0x06001707 RID: 5895 RVA: 0x0008CDFC File Offset: 0x0008AFFC
	private static bool HasWorldLightingData(AdventureLightData lightData)
	{
		return !string.IsNullOrEmpty(lightData.ColorInHex) || !Mathf.Approximately(lightData.Strength, 0f) || !Mathf.Approximately(lightData.Height, 0f);
	}

	// Token: 0x06001708 RID: 5896 RVA: 0x0008CE44 File Offset: 0x0008B044
	private static bool HasPointLightingData(AdventureLightData lightData)
	{
		return !string.IsNullOrEmpty(lightData.ColorInHex) || !Mathf.Approximately(lightData.Strength, 0f) || !Mathf.Approximately(lightData.Height, 0f);
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x0008CE8C File Offset: 0x0008B08C
	private bool UpdatePreviewPointLightTransform()
	{
		AdventureEditorMicro targetMicro = this.FindMicroByLightingGridIndex(this._pointLightPreviewIndex);
		bool flag = targetMicro == null;
		bool result;
		if (flag)
		{
			this.ResetPreviewPointLightTransform();
			result = false;
		}
		else
		{
			Transform previewTransform = this.previewPointLight.transform;
			previewTransform.SetParent(targetMicro.LightingPreviewAnchor, false);
			previewTransform.localPosition = Vector3.zero;
			previewTransform.localRotation = Quaternion.identity;
			previewTransform.localScale = Vector3.one;
			result = true;
		}
		return result;
	}

	// Token: 0x0600170A RID: 5898 RVA: 0x0008CF04 File Offset: 0x0008B104
	private void ResetPreviewPointLightTransform()
	{
		bool flag = !this._lightingDefaultsInitialized;
		if (!flag)
		{
			Transform previewTransform = this.previewPointLight.transform;
			bool flag2 = !previewTransform.gameObject.activeInHierarchy;
			if (!flag2)
			{
				previewTransform.SetParent(this._defaultPreviewPointLightParent, false);
				previewTransform.localPosition = this._defaultPreviewPointLightLocalPosition;
				previewTransform.localRotation = this._defaultPreviewPointLightLocalRotation;
				previewTransform.localScale = this._defaultPreviewPointLightLocalScale;
				previewTransform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600170B RID: 5899 RVA: 0x0008CF84 File Offset: 0x0008B184
	private void RefreshStaticPreviewLights()
	{
		this.ClearStaticPreviewLights();
		List<AdventurePointLightSnapshot> lightingPoints = AdventureEditorKit.BlackBoard.Editing.LightingPoints;
		bool flag = lightingPoints == null || lightingPoints.Count == 0;
		if (!flag)
		{
			foreach (AdventurePointLightSnapshot snapshot in lightingPoints)
			{
				AdventureEditorMicro micro = this.FindMicroByLightingGridIndex(snapshot.Index);
				bool flag2 = micro == null;
				if (!flag2)
				{
					GameObject lightObj = new GameObject(string.Format("StaticPointLight_{0}", snapshot.Index));
					lightObj.transform.SetParent(micro.LightingPreviewAnchor, false);
					lightObj.transform.localPosition = Vector3.zero;
					lightObj.transform.localRotation = Quaternion.identity;
					lightObj.transform.localScale = Vector3.one;
					AdventurePointLight pointLight = lightObj.AddComponent<AdventurePointLight>();
					pointLight.LightColor = UI_AdventureEditorRemake.TryParseColor(snapshot.LightData.ColorInHex, this._defaultPointLightColor);
					pointLight.Intensity = snapshot.LightData.Strength;
					pointLight.VirtualZ = snapshot.LightData.Height;
					pointLight.Range = (float)Mathf.Abs(snapshot.Range);
					pointLight.NoRangeClamp = (snapshot.Range < 0);
					pointLight.BlockIndex = snapshot.Index;
					pointLight.Priority = 7;
					pointLight.enabled = true;
					this._staticPreviewLights.Add(pointLight);
				}
			}
		}
	}

	// Token: 0x0600170C RID: 5900 RVA: 0x0008D134 File Offset: 0x0008B334
	private AdventureEditorMicro FindMicroByLightingGridIndex(AdventureBlockIndex index)
	{
		AdventureEditorMicro[] micros = base.GetComponentsInChildren<AdventureEditorMicro>(true);
		foreach (AdventureEditorMicro micro in micros)
		{
			bool flag = micro.LightingGridIndex == index;
			if (flag)
			{
				return micro;
			}
		}
		return null;
	}

	// Token: 0x0600170D RID: 5901 RVA: 0x0008D180 File Offset: 0x0008B380
	private static Color TryParseColor(string colorInHex, Color fallback)
	{
		bool flag = string.IsNullOrEmpty(colorInHex);
		Color result;
		if (flag)
		{
			result = fallback;
		}
		else
		{
			string htmlColor = colorInHex.StartsWith("#") ? colorInHex : ("#" + colorInHex);
			Color parsedColor;
			result = (ColorUtility.TryParseHtmlString(htmlColor, out parsedColor) ? parsedColor : fallback);
		}
		return result;
	}

	// Token: 0x0600170E RID: 5902 RVA: 0x0008D1CC File Offset: 0x0008B3CC
	private static float NormalizeAngle(float angle)
	{
		angle %= 360f;
		bool flag = angle < 0f;
		if (flag)
		{
			angle += 360f;
		}
		return angle;
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x0008D200 File Offset: 0x0008B400
	private void Update()
	{
		bool flag = UIManager.Instance.IsElementActive(UIElement.Dialog);
		if (!flag)
		{
			bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
			if (flag2)
			{
				this.ShowExitDialog();
			}
			else
			{
				bool flag3 = !AdventureEditorKit.GetControlKey;
				if (!flag3)
				{
					bool keyUp = Input.GetKeyUp(KeyCode.P);
					if (keyUp)
					{
						AdventureEditorKit.ResetCorePath();
						AdventureEditorKit.BlackBoard.ResetToEmpty();
						this.elementArea.RedirectAndClear();
					}
					bool keyUp2 = Input.GetKeyUp(KeyCode.S);
					if (keyUp2)
					{
						this.Save();
					}
					bool keyUp3 = Input.GetKeyUp(KeyCode.L);
					if (keyUp3)
					{
						this.Load();
					}
					bool keyUp4 = Input.GetKeyUp(KeyCode.I);
					if (keyUp4)
					{
						this.ImportArt();
					}
					bool keyUp5 = Input.GetKeyUp(KeyCode.G);
					if (keyUp5)
					{
						this.Export();
					}
				}
			}
		}
	}

	// Token: 0x06001710 RID: 5904 RVA: 0x0008D2D4 File Offset: 0x0008B4D4
	private void ShowExitDialog()
	{
		DialogCmd dialogCmd = new DialogCmd();
		dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_AdventureEditor_Exit);
		dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_AdventureEditor_Exit_Desc);
		dialogCmd.Yes = delegate()
		{
			UIManager.Instance.StackBack(null);
		};
		DialogCmd cmd = dialogCmd;
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.SetObject("Cmd", cmd);
		UIElement.Dialog.SetOnInitArgs(box);
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x0008D35C File Offset: 0x0008B55C
	private void Save()
	{
		string path = this._editingPath;
		bool flag = AdventureEditorKit.GetShiftKey || string.IsNullOrEmpty(path);
		if (flag)
		{
			path = LocalDialog.SelectSaveFilePath("Adventure Blueprint Files(*.advbp)\0*.advbp\0", AdventureEditorKit.CoreDirectory);
		}
		bool flag2 = string.IsNullOrEmpty(path);
		if (!flag2)
		{
			this._editingPath = path;
			bool flag3 = !path.EndsWith(".advbp");
			if (flag3)
			{
				path += ".advbp";
			}
			AdventureSnapshot snapshot = AdventureEditorKit.BlackBoard.Editing.Clone();
			AdventureSnapshot data;
			snapshot.Id = (AdventureSnapshot.TryLoadFromFile(path, out data) ? data.Id : AdventureEditorKit.GetNewBlueprintId());
			snapshot.SaveToFile(path);
			GEvent.OnEvent(UiEvents.AdventureRemakeEditorStatusSaved, new ArgumentBox().Set("Path", path));
		}
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x0008D420 File Offset: 0x0008B620
	private void Load()
	{
		string path = LocalDialog.SelectLoadFilePath("Adventure Blueprint Files(*.advbp)\0*.advbp\0", AdventureEditorKit.CoreDirectory);
		AdventureSnapshot snapshot;
		bool flag = !AdventureSnapshot.TryLoadFromFile(path, out snapshot);
		if (!flag)
		{
			AdventureEditorKit.BlackBoard.Reload(snapshot);
			AdventureEditorKit.BlackBoard.ResetGroupIndex();
			this._editingPath = path;
			GEvent.OnEvent(UiEvents.AdventureRemakeEditorStatusLoaded, new ArgumentBox().Set("Path", path));
		}
	}

	// Token: 0x06001713 RID: 5907 RVA: 0x0008D490 File Offset: 0x0008B690
	private void ImportArt()
	{
		string path = LocalDialog.SelectLoadFilePath("Adventure Blueprint Files(*.advbp)\0*.advbp\0", AdventureEditorKit.CoreDirectory);
		AdventureSnapshot snapshot;
		bool flag = !AdventureSnapshot.TryLoadFromFile(path, out snapshot);
		if (flag)
		{
			this.ImportArtFailed(LanguageKey.LK_Adventure_Editor_Import_Art_Failed_FileNotLoaded.TrFormat(path));
		}
		else
		{
			bool flag2 = AdventureEditorKit.BlackBoard.Editing.Size != snapshot.Size;
			if (flag2)
			{
				LanguageKey contentTemplate = LanguageKey.LK_Adventure_Editor_Import_Art_Failed_SizeMismatch;
				this.ImportArtFailed(contentTemplate.TrFormat(AdventureEditorKit.BlackBoard.Editing.Size.ToString(), snapshot.Size.ToString()));
			}
			else
			{
				AdventureEditorKit.BlackBoard.MakeEdit<AdventureSnapshot>(new AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>.EditAction<AdventureSnapshot>(this.ImportArtSuccess), EAdventureEditType.BlockVisible, snapshot);
			}
		}
	}

	// Token: 0x06001714 RID: 5908 RVA: 0x0008D540 File Offset: 0x0008B740
	private void ImportArtSuccess(AdventureSnapshot target, AdventureSnapshot art)
	{
		target.LightingTaiwu.MergeFrom(art.LightingTaiwu);
		target.LightingWorld.MergeFrom(art.LightingWorld);
		target.LightingRotate = art.LightingRotate;
		target.LightingRotateAngle = art.LightingRotateAngle;
		using (IEnumerator<AdventureBlockSnapshot> enumerator = target.Blocks.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				AdventureBlockSnapshot block = enumerator.Current;
				AdventureBlockSnapshot artBlock = art.Blocks.FirstOrDefault((AdventureBlockSnapshot x) => x.Index == block.Index);
				bool flag = artBlock == null;
				if (!flag)
				{
					block.Height = artBlock.Height;
					block.Icon = artBlock.Icon;
					block.Decorates.ClearAndAddRange(artBlock.Decorates);
				}
			}
		}
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x0008D630 File Offset: 0x0008B830
	private void ImportArtFailed(string content)
	{
		DialogCmd cmd = new DialogCmd
		{
			Title = LanguageKey.LK_Adventure_Editor_Import_Art_Failed_Title.Tr(),
			Content = content,
			Type = 2
		};
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06001716 RID: 5910 RVA: 0x0008D690 File Offset: 0x0008B890
	private void Export()
	{
		Exception exception;
		try
		{
			AdventureEditorRuntimeImporter.ExportToContext(AdventureEditorKit.CoreRoot);
			exception = null;
		}
		catch (Exception e)
		{
			exception = e;
		}
		bool flag = exception != null;
		if (flag)
		{
			Debug.LogWarning("Export failed by exception " + exception.Message + "\nstack trace: " + exception.StackTrace);
		}
		DialogCmd cmd = new DialogCmd
		{
			Title = LanguageKey.LK_Adventure_Editor_Export_Title.Tr(),
			Content = ((exception == null) ? LanguageKey.LK_Adventure_Editor_Export_Content_Successful : LanguageKey.LK_Adventure_Editor_Export_Content_Failed).Tr(),
			Type = 2
		};
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06001717 RID: 5911 RVA: 0x0008D754 File Offset: 0x0008B954
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		string text = btnName;
		string a = text;
		if (a == "Load")
		{
			this.Load();
		}
	}

	// Token: 0x04001283 RID: 4739
	[SerializeField]
	private AdventureEditorElementArea elementArea;

	// Token: 0x04001284 RID: 4740
	[SerializeField]
	private AdventurePointLight previewPointLight;

	// Token: 0x04001285 RID: 4741
	[SerializeField]
	private UnityEvent onActivate;

	// Token: 0x04001286 RID: 4742
	private IReadOnlyList<IAdventureEditorBlackBoardElement> _loadedElements;

	// Token: 0x04001287 RID: 4743
	private string _editingPath;

	// Token: 0x04001288 RID: 4744
	private AdventureLightingManager _lightingManager;

	// Token: 0x04001289 RID: 4745
	private AdventureNormalMapBinder _normalMapBinder;

	// Token: 0x0400128A RID: 4746
	private readonly List<AdventurePointLight> _staticPreviewLights = new List<AdventurePointLight>();

	// Token: 0x0400128B RID: 4747
	private bool _lightingPreviewEnabled = true;

	// Token: 0x0400128C RID: 4748
	private bool _pointLightPreviewEnabled = true;

	// Token: 0x0400128D RID: 4749
	private AdventureBlockIndex _pointLightPreviewIndex;

	// Token: 0x0400128E RID: 4750
	private bool _lightingDefaultsInitialized;

	// Token: 0x0400128F RID: 4751
	private Color _defaultGlobalColor;

	// Token: 0x04001290 RID: 4752
	private float _defaultGlobalIntensity;

	// Token: 0x04001291 RID: 4753
	private float _defaultGlobalIncidenceAngle;

	// Token: 0x04001292 RID: 4754
	private float _defaultGlobalAzimuthAngle;

	// Token: 0x04001293 RID: 4755
	private float _defaultRotationAnglePerStep;

	// Token: 0x04001294 RID: 4756
	private Color _defaultPointLightColor;

	// Token: 0x04001295 RID: 4757
	private float _defaultPointLightIntensity;

	// Token: 0x04001296 RID: 4758
	private float _defaultPointLightVirtualZ;

	// Token: 0x04001297 RID: 4759
	private Transform _defaultPreviewPointLightParent;

	// Token: 0x04001298 RID: 4760
	private Vector3 _defaultPreviewPointLightLocalPosition;

	// Token: 0x04001299 RID: 4761
	private Quaternion _defaultPreviewPointLightLocalRotation;

	// Token: 0x0400129A RID: 4762
	private Vector3 _defaultPreviewPointLightLocalScale;
}
