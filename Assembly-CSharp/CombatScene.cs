using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

// Token: 0x0200015B RID: 347
public class CombatScene : MonoBehaviour
{
	// Token: 0x1700021D RID: 541
	// (get) Token: 0x06001307 RID: 4871 RVA: 0x00073E50 File Offset: 0x00072050
	// (set) Token: 0x06001308 RID: 4872 RVA: 0x00073E58 File Offset: 0x00072058
	public bool InTransition { get; private set; }

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x06001309 RID: 4873 RVA: 0x00073E61 File Offset: 0x00072061
	private bool CanBlend
	{
		get
		{
			bool result;
			if (this.Roots.Length == 2)
			{
				result = this.Roots.All((RectTransform r) => r);
			}
			else
			{
				result = false;
			}
			return result;
		}
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x00073E9C File Offset: 0x0007209C
	public void Awake()
	{
		bool flag = Application.isPlaying && this._initFlag;
		if (!flag)
		{
			bool flag2 = !this.CanBlend;
			if (flag2)
			{
				CombatSceneLayer[] layers = base.transform.GetComponentsInTopChildren(true);
				this._layers = new CombatSceneLayer[][]
				{
					layers
				};
			}
			else
			{
				this.Roots[0].gameObject.SetActive(true);
				this.Roots[1].gameObject.SetActive(true);
				this._layers = new CombatSceneLayer[2][];
				for (int index = 0; index < 2; index++)
				{
					RectTransform root = this.Roots[index];
					CombatSceneLayer[] layers2 = root.GetComponentsInTopChildren(true);
					this._layers[index] = layers2;
				}
			}
			this._initFlag = true;
		}
	}

	// Token: 0x0600130B RID: 4875 RVA: 0x00073F60 File Offset: 0x00072160
	private void Start()
	{
		bool flag = this.CanBlend && !this._blendPrepared;
		if (flag)
		{
			this.PrepareBlend();
		}
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x00073F90 File Offset: 0x00072190
	private void OnDestroy()
	{
		bool blendPrepared = this._blendPrepared;
		if (blendPrepared)
		{
			this.SetBlendObjectsActive(false);
			this._blendPrepared = false;
		}
		this.ClearBlend();
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x00073FC0 File Offset: 0x000721C0
	public void OnEnable()
	{
		List<int> layerActiveConfig = this._layerActiveConfig;
		bool flag = layerActiveConfig != null && layerActiveConfig.Count > 0;
		if (flag)
		{
			List<List<bool>> activeConfig = new List<List<bool>>();
			this.GetLayerActiveConfig(activeConfig);
			List<bool> layersActiveStates = activeConfig[0];
			bool flag2 = this.Roots == null || this.Roots.Length == 0;
			if (flag2)
			{
				for (int i = 0; i < this._layers[0].Length; i++)
				{
					this._layers[0][i].gameObject.SetActive(layersActiveStates[i]);
				}
			}
			else
			{
				for (int j = 0; j < this.Roots.Length; j++)
				{
					for (int k = 0; k < this._layers[j].Length; k++)
					{
						this._layers[j][k].gameObject.SetActive(layersActiveStates[k]);
					}
				}
			}
		}
		bool flag3 = !Application.isPlaying || !this.CanBlend;
		if (!flag3)
		{
			bool flag4 = !this._blendPrepared;
			if (flag4)
			{
				this.PrepareBlend();
			}
			else
			{
				this.ResetBlendState();
				this.SetBlendObjectsActive(true);
			}
		}
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x00074104 File Offset: 0x00072304
	private void OnDisable()
	{
		bool flag = !Application.isPlaying || !this._blendPrepared;
		if (!flag)
		{
			this.SetBlendObjectsActive(false);
		}
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x00074133 File Offset: 0x00072333
	private void Update()
	{
		this.TickBlend();
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x00074140 File Offset: 0x00072340
	public void UpdateSceneScale(float value, float duration)
	{
		bool flag = this._layers == null;
		if (!flag)
		{
			foreach (CombatSceneLayer[] layersInGroup in this._layers)
			{
				foreach (CombatSceneLayer layer in layersInGroup)
				{
					bool flag2 = !layer.gameObject.activeSelf;
					if (!flag2)
					{
						layer.ScaleTo(value, duration);
					}
				}
			}
		}
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x000741BC File Offset: 0x000723BC
	public void SetOffset(float offset)
	{
		bool flag = this._layers == null;
		if (!flag)
		{
			float scrollDelta = offset - this.CurOffsetValue;
			foreach (CombatSceneLayer[] layersInGroup in this._layers)
			{
				foreach (CombatSceneLayer layer in layersInGroup)
				{
					bool flag2 = !layer.gameObject.activeSelf;
					if (!flag2)
					{
						bool flag3 = 0f == offset;
						if (flag3)
						{
							layer.ResetScroll();
						}
						else
						{
							layer.Scroll(scrollDelta);
						}
					}
				}
			}
			this.CurOffsetValue = offset;
		}
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x00074262 File Offset: 0x00072462
	public void SetLayerActiveConfig(List<int> activeConfig)
	{
		this._layerActiveConfig = activeConfig;
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x0007426C File Offset: 0x0007246C
	public void GetLayerActiveConfig(List<List<bool>> resultList)
	{
		bool flag = resultList == null;
		if (flag)
		{
			throw new Exception("null list passed in to GetLayerActiveConfig!");
		}
		resultList.Clear();
		bool flag2 = this._layerActiveConfig == null;
		if (!flag2)
		{
			foreach (int activeData in this._layerActiveConfig)
			{
				List<bool> activeResult = new List<bool>(this._layers[0].Length);
				for (int i = 0; i < this._layers[0].Length; i++)
				{
					activeResult.Add((1 << i & activeData) != 0);
				}
				resultList.Add(activeResult);
			}
		}
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x00074338 File Offset: 0x00072538
	public void StartTransition(string stageKey)
	{
		bool flag = this.TransitionStageList == null || string.IsNullOrEmpty(stageKey);
		if (!flag)
		{
			for (int i = 0; i < this.TransitionStageList.Count; i++)
			{
				CombatScene.TransitionStage stage = this.TransitionStageList[i];
				bool flag2 = stage.StageKey == stageKey;
				if (flag2)
				{
					base.StartCoroutine(this.TransitionDisplay_internal(stage));
					return;
				}
			}
			Debug.LogError("Failed to display transition of key " + stageKey + ", no such key config!");
		}
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x000743C0 File Offset: 0x000725C0
	public void StartTransition(int index)
	{
		bool flag = this.TransitionStageList == null;
		if (!flag)
		{
			bool flag2 = !this.TransitionStageList.CheckIndex(index);
			if (flag2)
			{
				Debug.LogError(string.Format("Failed to display transition of index {0}, no such index config!", index));
			}
			CombatScene.TransitionStage stage = this.TransitionStageList[index];
			base.StartCoroutine(this.TransitionDisplay_internal(stage));
		}
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x00074421 File Offset: 0x00072621
	private IEnumerator TransitionDisplay_internal(CombatScene.TransitionStage stage)
	{
		this.InTransition = true;
		bool flag = stage.TransitionBeginItems != null;
		if (flag)
		{
			int num;
			for (int i = 0; i < stage.TransitionBeginItems.Count; i = num)
			{
				this.ItemTransitionAction(stage.TransitionBeginItems[i]);
				num = i + 1;
			}
		}
		yield return new WaitForSeconds(10f);
		bool flag2 = stage.TransitionBeginItems != null;
		if (flag2)
		{
			int num;
			for (int j = 0; j < stage.TransitionBeginItems.Count; j = num)
			{
				stage.TransitionBeginItems[j].Target.SetActive(false);
				num = j + 1;
			}
		}
		bool flag3 = stage.TransitionEndItems != null;
		if (flag3)
		{
			int num;
			for (int k = 0; k < stage.TransitionEndItems.Count; k = num)
			{
				this.ItemTransitionAction(stage.TransitionEndItems[k]);
				num = k + 1;
			}
		}
		this.InTransition = false;
		yield break;
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x00074438 File Offset: 0x00072638
	private void ItemTransitionAction(CombatScene.TransitionItem item)
	{
		bool flag = null == item.Target;
		if (!flag)
		{
			item.Target.SetActive(true);
			ParticleSystem particle = item.Target.GetComponent<ParticleSystem>();
			bool flag2 = null != particle;
			if (flag2)
			{
				particle.Play(true);
			}
			SkeletonGraphic skeletonGraphic = item.Target.GetComponent<SkeletonGraphic>();
			bool flag3 = null != skeletonGraphic && !string.IsNullOrEmpty(item.AnimationName);
			if (flag3)
			{
				skeletonGraphic.AnimationState.SetAnimation(0, item.AnimationName, false);
			}
		}
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x000744C4 File Offset: 0x000726C4
	private void PrepareBlend()
	{
		this.PrepareRootSpineSlot(this.Roots[0], "__1");
		this.PrepareRootRtCamera(1, this.Roots[1], "RT1");
		this._blendPrepared = true;
		this.ResetBlendState();
		this.SetBlendObjectsActive(true);
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x00074514 File Offset: 0x00072714
	private void PrepareRootSpineSlot(RectTransform root, string keyword)
	{
		SkeletonGraphic[] skeletonGraphics = root.GetComponentsInChildren<SkeletonGraphic>(true);
		foreach (SkeletonGraphic sg in skeletonGraphics)
		{
			Skeleton skeleton = sg.Skeleton;
			bool flag = skeleton == null;
			if (!flag)
			{
				foreach (Slot slot in skeleton.Slots)
				{
					bool flag2 = slot.Data.Name.Contains(keyword);
					if (flag2)
					{
						slot.A = 0f;
					}
				}
			}
		}
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x000745C8 File Offset: 0x000727C8
	private void PrepareRootRtCamera(int index, RectTransform root, string layerName)
	{
		int layerId = LayerMask.NameToLayer(layerName);
		int sortingLayerId = SortingLayer.NameToID(layerName);
		this._root2Rt = new RenderTexture(2560, 1440, 0, RenderTextureFormat.ARGB32);
		GameObject cameraObj = new GameObject(string.Format("RTCam_{0}", index + 1));
		Object.DontDestroyOnLoad(cameraObj);
		Camera rtCamera = cameraObj.AddComponent<Camera>();
		rtCamera.orthographic = true;
		rtCamera.clearFlags = CameraClearFlags.Color;
		rtCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		rtCamera.cullingMask = 1 << layerId;
		rtCamera.targetTexture = this._root2Rt;
		rtCamera.transform.position = new Vector3(0f, 0f, -10f);
		rtCamera.renderingPath = RenderingPath.Forward;
		rtCamera.allowHDR = false;
		rtCamera.allowMSAA = false;
		this._root2CameraObject = rtCamera.gameObject;
		GameObject canvasObj = new GameObject(string.Format("RTCanvas_{0}", index + 1), new Type[]
		{
			typeof(RectTransform),
			typeof(Canvas),
			typeof(CanvasScaler)
		});
		canvasObj.layer = layerId;
		Canvas rtCanvas = canvasObj.GetComponent<Canvas>();
		rtCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		rtCanvas.worldCamera = rtCamera;
		rtCanvas.sortingOrder = layerId;
		CanvasScaler canvasScaler = canvasObj.GetComponent<CanvasScaler>();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasScaler.referenceResolution = new Vector2(2560f, 1440f);
		canvasObj.transform.SetParent(rtCamera.transform);
		this.Roots[index].SetParent(canvasObj.transform);
		this.Roots[index].localScale = Vector3.one;
		GameObject rawImageObj = new GameObject(string.Format("RawImage_{0}", index + 1), new Type[]
		{
			typeof(RectTransform),
			typeof(CanvasRenderer),
			typeof(CRawImage),
			typeof(CanvasGroup),
			typeof(Canvas)
		});
		rawImageObj.transform.SetParent(base.transform);
		CRawImage rawImage = rawImageObj.GetComponent<CRawImage>();
		rawImage.texture = this._root2Rt;
		rawImage.raycastTarget = false;
		rawImage.gameObject.layer = LayerMask.NameToLayer("UI");
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			Canvas rawImageCanvas = rawImageObj.GetComponent<Canvas>();
			rawImageCanvas.overrideSorting = true;
			rawImageCanvas.sortingLayerName = "UI";
			rawImageCanvas.sortingOrder = 160;
		});
		RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(2560f, 1440f);
		rectTransform.localScale = Vector3.one;
		this._root2RawImage = rawImage;
		CombatScene.SetLayerRecursively(root, layerId, sortingLayerId);
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x000748A8 File Offset: 0x00072AA8
	private void TickBlend()
	{
		bool flag = !this.CanBlend;
		if (!flag)
		{
			this._blendValue += (float)(this._blendDirection ? 1 : -1) * (Time.deltaTime / Mathf.Abs(this._blendDirection ? this.BlendDurationTo2 : this.BlendDurationTo1));
			float blendValue = this._blendValue;
			bool flag2 = blendValue > 1f || blendValue < 0f;
			if (flag2)
			{
				this._blendDirection = !this._blendDirection;
				this._blendValue = Mathf.Clamp01(this._blendValue);
			}
			this._root2RawImage.GetComponent<CanvasGroup>().alpha = 1f - this._blendValue;
		}
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x00074968 File Offset: 0x00072B68
	private static void SetLayerRecursively(Transform t, int layerId, int sortingLayerId)
	{
		t.gameObject.layer = layerId;
		Canvas canvas = t.GetComponent<Canvas>();
		bool flag = canvas;
		if (flag)
		{
			canvas.sortingLayerID = sortingLayerId;
		}
		ParticleSystem particleSystem = t.GetComponent<ParticleSystem>();
		bool flag2 = particleSystem;
		if (flag2)
		{
			Renderer particleRenderer = particleSystem.GetComponent<Renderer>();
			bool flag3 = particleRenderer;
			if (flag3)
			{
				particleRenderer.sortingLayerID = sortingLayerId;
			}
		}
		foreach (object obj in t)
		{
			Transform c = (Transform)obj;
			CombatScene.SetLayerRecursively(c, layerId, sortingLayerId);
		}
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x00074A24 File Offset: 0x00072C24
	private void ClearBlend()
	{
		bool flag = this._root2RawImage;
		if (flag)
		{
			Object.Destroy(this._root2RawImage.gameObject);
			this._root2RawImage = null;
		}
		bool flag2 = this._root2CameraObject;
		if (flag2)
		{
			Object.Destroy(this._root2CameraObject);
			this._root2CameraObject = null;
		}
		bool flag3 = this._root2Rt;
		if (flag3)
		{
			this._root2Rt.Release();
			Object.Destroy(this._root2Rt);
			this._root2Rt = null;
		}
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x00074AB0 File Offset: 0x00072CB0
	private void SetBlendObjectsActive(bool active)
	{
		bool flag = this.Roots != null && this.Roots.Length > 1 && this.Roots[1];
		if (flag)
		{
			this.Roots[1].gameObject.SetActive(active);
		}
		bool flag2 = this._root2RawImage;
		if (flag2)
		{
			this._root2RawImage.gameObject.SetActive(active);
		}
		bool flag3 = this._root2CameraObject;
		if (flag3)
		{
			this._root2CameraObject.SetActive(active);
		}
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x00074B38 File Offset: 0x00072D38
	private void ResetBlendState()
	{
		bool useRoot = Random.Range(0, 2) == 0;
		this._blendDirection = useRoot;
		this._blendValue = (useRoot ? 0f : 1f);
		bool flag = this._root2RawImage;
		if (flag)
		{
			CanvasGroup canvasGroup = this._root2RawImage.GetComponent<CanvasGroup>();
			bool flag2 = canvasGroup;
			if (flag2)
			{
				canvasGroup.alpha = 1f - this._blendValue;
			}
		}
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x00074BA8 File Offset: 0x00072DA8
	public void RefreshSeasonalAndRandomVisibility(int month)
	{
		int adjustedMonth = month + 1;
		this.ApplySeasonalVisibilityInternal(adjustedMonth);
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x00074BC4 File Offset: 0x00072DC4
	private void ApplySeasonalVisibilityInternal(int month)
	{
		Profiler.BeginSample("CombatScene.ApplySeasonalVisibility");
		Profiler.BeginSample("CombatScene.GetSceneObjects");
		CombatSceneObject[] root1Objects = null;
		CombatSceneObject[] root2Objects = null;
		bool isBlendableScene = this.Roots != null && this.Roots.Length == 2 && this.Roots[0] != null && this.Roots[1] != null;
		bool flag = isBlendableScene;
		CombatSceneObject[] sceneObjects;
		if (flag)
		{
			root1Objects = this.Roots[0].GetComponentsInChildren<CombatSceneObject>(true);
			root2Objects = this.Roots[1].GetComponentsInChildren<CombatSceneObject>(true);
			int count = (root1Objects != null) ? root1Objects.Length : 0;
			int count2 = (root2Objects != null) ? root2Objects.Length : 0;
			sceneObjects = new CombatSceneObject[count + count2];
			bool flag2 = count > 0;
			if (flag2)
			{
				Array.Copy(root1Objects, 0, sceneObjects, 0, count);
			}
			bool flag3 = count2 > 0;
			if (flag3)
			{
				Array.Copy(root2Objects, 0, sceneObjects, count, count2);
			}
		}
		else
		{
			sceneObjects = base.GetComponentsInChildren<CombatSceneObject>(true);
		}
		Profiler.EndSample();
		bool flag4 = sceneObjects == null || sceneObjects.Length == 0;
		if (flag4)
		{
			Profiler.EndSample();
		}
		else
		{
			int currentMonth = Mathf.Clamp(month, 1, 12);
			Transform root2 = isBlendableScene ? this.Roots[1].transform : null;
			this._cachedRoot1Groups.Clear();
			this._cachedRoot2Groups.Clear();
			List<CombatSceneObject> independentObjects = new List<CombatSceneObject>();
			Profiler.BeginSample("CombatScene.ClassifyObjects");
			bool flag5 = isBlendableScene;
			if (flag5)
			{
				bool flag6 = root1Objects != null;
				if (flag6)
				{
					foreach (CombatSceneObject obj in root1Objects)
					{
						bool flag7 = obj == null;
						if (!flag7)
						{
							bool flag8 = obj.GroupId > 0;
							if (flag8)
							{
								CombatScene.GroupRuntimeState state;
								bool flag9 = !this._cachedRoot1Groups.TryGetValue(obj.GroupId, out state);
								if (flag9)
								{
									state = new CombatScene.GroupRuntimeState(obj.GroupId);
									this._cachedRoot1Groups.Add(obj.GroupId, state);
								}
								state.AddMember(obj);
							}
							else
							{
								bool flag10 = !obj.gameObject.activeSelf;
								if (!flag10)
								{
									independentObjects.Add(obj);
								}
							}
						}
					}
				}
				bool flag11 = root2Objects != null;
				if (flag11)
				{
					foreach (CombatSceneObject obj2 in root2Objects)
					{
						bool flag12 = obj2 == null;
						if (!flag12)
						{
							bool flag13 = obj2.GroupId > 0;
							if (flag13)
							{
								CombatScene.GroupRuntimeState state2;
								bool flag14 = !this._cachedRoot2Groups.TryGetValue(obj2.GroupId, out state2);
								if (flag14)
								{
									state2 = new CombatScene.GroupRuntimeState(obj2.GroupId);
									this._cachedRoot2Groups.Add(obj2.GroupId, state2);
								}
								state2.AddMember(obj2);
							}
							else
							{
								bool flag15 = !obj2.gameObject.activeSelf;
								if (!flag15)
								{
									independentObjects.Add(obj2);
								}
							}
						}
					}
				}
			}
			else
			{
				foreach (CombatSceneObject obj3 in sceneObjects)
				{
					bool flag16 = obj3 == null;
					if (!flag16)
					{
						bool flag17 = obj3.GroupId > 0;
						if (flag17)
						{
							CombatScene.GroupRuntimeState state3;
							bool flag18 = !this._cachedRoot1Groups.TryGetValue(obj3.GroupId, out state3);
							if (flag18)
							{
								state3 = new CombatScene.GroupRuntimeState(obj3.GroupId);
								this._cachedRoot1Groups.Add(obj3.GroupId, state3);
							}
							state3.AddMember(obj3);
						}
						else
						{
							bool flag19 = !obj3.gameObject.activeSelf;
							if (!flag19)
							{
								independentObjects.Add(obj3);
							}
						}
					}
				}
			}
			Profiler.EndSample();
			Profiler.BeginSample("CombatScene.ProcessIndependentObjects");
			foreach (CombatSceneObject obj4 in independentObjects)
			{
				bool monthAllowed = obj4.IsMonthAllowed(currentMonth);
				bool flag20 = !monthAllowed;
				if (flag20)
				{
					obj4.gameObject.SetActive(false);
				}
				else
				{
					bool probabilityPassed = true;
					bool useAppearanceProbability = obj4.UseAppearanceProbability;
					if (useAppearanceProbability)
					{
						float probabilityRoll = Random.value;
						probabilityPassed = (probabilityRoll <= obj4.AppearanceProbability);
					}
					bool flag21 = !probabilityPassed;
					if (flag21)
					{
						obj4.gameObject.SetActive(false);
					}
					else
					{
						obj4.gameObject.SetActive(true);
					}
				}
			}
			Profiler.EndSample();
			Profiler.BeginSample("CombatScene.ProcessRoot1Groups");
			this._cachedRoot1Choices.Clear();
			this.ProcessGroups(this._cachedRoot1Groups, currentMonth, this._cachedRoot1Choices);
			Profiler.EndSample();
			bool flag22 = isBlendableScene && root2 != null;
			if (flag22)
			{
				Profiler.BeginSample("CombatScene.ProcessRoot2Groups");
				this.ProcessGroupsWithSync(this._cachedRoot2Groups, currentMonth, this._cachedRoot1Choices, "Root2");
				Profiler.EndSample();
			}
			Profiler.EndSample();
		}
	}

	// Token: 0x06001322 RID: 4898 RVA: 0x000750D0 File Offset: 0x000732D0
	private void ProcessGroups(Dictionary<int, CombatScene.GroupRuntimeState> groups, int currentMonth, Dictionary<int, CombatSceneObject> choices)
	{
		List<CombatSceneObject> validMembers = new List<CombatSceneObject>();
		bool combatWeatherOn = SingletonObject.getInstance<GlobalSettings>().CombatWeather;
		foreach (CombatScene.GroupRuntimeState state in groups.Values)
		{
			validMembers.Clear();
			bool flag = !combatWeatherOn;
			if (flag)
			{
				bool isWeatherGroup = false;
				foreach (CombatSceneObject member in state.Members)
				{
					bool flag2 = member == null;
					if (!flag2)
					{
						bool flag3 = CombatScene.IsWeatherNodeName(member.name);
						if (flag3)
						{
							isWeatherGroup = true;
							break;
						}
					}
				}
				bool flag4 = isWeatherGroup;
				if (flag4)
				{
					foreach (CombatSceneObject member2 in state.Members)
					{
						bool flag5 = member2 != null;
						if (flag5)
						{
							member2.gameObject.SetActive(false);
						}
					}
					continue;
				}
			}
			foreach (CombatSceneObject member3 in state.Members)
			{
				bool flag6 = member3 == null;
				if (!flag6)
				{
					bool flag7 = !member3.IsMonthAllowed(currentMonth);
					if (!flag7)
					{
						validMembers.Add(member3);
					}
				}
			}
			bool flag8 = validMembers.Count == 0;
			if (flag8)
			{
				foreach (CombatSceneObject member4 in state.Members)
				{
					bool flag9 = member4 != null;
					if (flag9)
					{
						member4.gameObject.SetActive(false);
					}
				}
			}
			else
			{
				int randomIndex = Random.Range(0, validMembers.Count);
				CombatSceneObject selectedForProbabilityCheck = validMembers[randomIndex];
				bool probabilityPassed = true;
				bool useAppearanceProbability = selectedForProbabilityCheck.UseAppearanceProbability;
				if (useAppearanceProbability)
				{
					float probabilityRoll = Random.value;
					probabilityPassed = (probabilityRoll <= selectedForProbabilityCheck.AppearanceProbability);
				}
				bool flag10 = !probabilityPassed;
				if (flag10)
				{
					foreach (CombatSceneObject member5 in state.Members)
					{
						bool flag11 = member5 != null;
						if (flag11)
						{
							member5.gameObject.SetActive(false);
						}
					}
				}
				else
				{
					int chosenIndex = Random.Range(0, validMembers.Count);
					CombatSceneObject chosen = validMembers[chosenIndex];
					choices[state.GroupId] = chosen;
					foreach (CombatSceneObject member6 in state.Members)
					{
						bool flag12 = member6 != null;
						if (flag12)
						{
							member6.gameObject.SetActive(member6 == chosen);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001323 RID: 4899 RVA: 0x00075498 File Offset: 0x00073698
	private static bool IsWeatherNodeName(string name)
	{
		return name == "xiaoyu" || name == "dayu" || name == "xiaoxue" || name == "daxue";
	}

	// Token: 0x06001324 RID: 4900 RVA: 0x000754E0 File Offset: 0x000736E0
	private void ProcessGroupsWithSync(Dictionary<int, CombatScene.GroupRuntimeState> groups, int currentMonth, Dictionary<int, CombatSceneObject> root1Choices, string contextTag)
	{
		bool combatWeatherOn = SingletonObject.getInstance<GlobalSettings>().CombatWeather;
		foreach (CombatScene.GroupRuntimeState state in groups.Values)
		{
			bool isWeatherGroup = false;
			foreach (CombatSceneObject i in state.Members)
			{
				bool flag = i == null;
				if (!flag)
				{
					bool flag2 = CombatScene.IsWeatherNodeName(i.name);
					if (flag2)
					{
						isWeatherGroup = true;
						break;
					}
				}
			}
			bool flag3 = !combatWeatherOn && isWeatherGroup;
			if (flag3)
			{
				foreach (CombatSceneObject member in state.Members)
				{
					bool flag4 = member != null;
					if (flag4)
					{
						member.gameObject.SetActive(false);
					}
				}
			}
			else
			{
				CombatSceneObject root1Chosen;
				bool flag5 = root1Choices.TryGetValue(state.GroupId, out root1Chosen);
				if (flag5)
				{
					string chosenName = root1Chosen.name;
					string layerName = root1Chosen.transform.parent.name;
					CombatSceneObject root2Corresponding = null;
					foreach (CombatSceneObject member2 in state.Members)
					{
						bool flag6 = member2 == null;
						if (!flag6)
						{
							bool flag7 = member2.name == chosenName && member2.transform.parent.name == layerName;
							if (flag7)
							{
								root2Corresponding = member2;
								break;
							}
						}
					}
					bool flag8 = root2Corresponding != null;
					if (flag8)
					{
						foreach (CombatSceneObject member3 in state.Members)
						{
							bool flag9 = member3 != null;
							if (flag9)
							{
								member3.gameObject.SetActive(member3 == root2Corresponding);
							}
						}
						continue;
					}
				}
				bool flag10 = isWeatherGroup;
				if (flag10)
				{
					foreach (CombatSceneObject member4 in state.Members)
					{
						bool flag11 = member4 != null;
						if (flag11)
						{
							member4.gameObject.SetActive(false);
						}
					}
				}
				else
				{
					Dictionary<int, CombatSceneObject> dummyChoices = new Dictionary<int, CombatSceneObject>();
					this.ProcessGroups(new Dictionary<int, CombatScene.GroupRuntimeState>
					{
						{
							state.GroupId,
							state
						}
					}, currentMonth, dummyChoices);
				}
			}
		}
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x00075828 File Offset: 0x00073A28
	public bool GmSwitchToWeatherNode(string nodeName)
	{
		bool flag = string.IsNullOrEmpty(nodeName);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool isBlendableScene = this.Roots != null && this.Roots.Length == 2 && this.Roots[0] != null && this.Roots[1] != null;
			bool flag2 = isBlendableScene;
			CombatSceneObject[] sceneObjects;
			if (flag2)
			{
				CombatSceneObject[] root1Objects = this.Roots[0].GetComponentsInChildren<CombatSceneObject>(true);
				CombatSceneObject[] root2Objects = this.Roots[1].GetComponentsInChildren<CombatSceneObject>(true);
				int count = (root1Objects != null) ? root1Objects.Length : 0;
				int count2 = (root2Objects != null) ? root2Objects.Length : 0;
				sceneObjects = new CombatSceneObject[count + count2];
				bool flag3 = count > 0;
				if (flag3)
				{
					Array.Copy(root1Objects, 0, sceneObjects, 0, count);
				}
				bool flag4 = count2 > 0;
				if (flag4)
				{
					Array.Copy(root2Objects, 0, sceneObjects, count, count2);
				}
			}
			else
			{
				sceneObjects = base.GetComponentsInChildren<CombatSceneObject>(true);
			}
			bool flag5 = sceneObjects == null || sceneObjects.Length == 0;
			if (flag5)
			{
				result = false;
			}
			else
			{
				Transform root = isBlendableScene ? this.Roots[0].transform : null;
				Transform root2 = isBlendableScene ? this.Roots[1].transform : null;
				foreach (CombatSceneObject obj in sceneObjects)
				{
					bool flag6 = obj == null;
					if (!flag6)
					{
						bool flag7 = obj.name == "xiaoyu" || obj.name == "dayu" || obj.name == "xiaoxue" || obj.name == "daxue";
						if (flag7)
						{
							obj.gameObject.SetActive(false);
						}
					}
				}
				Dictionary<int, List<CombatSceneObject>> root1Groups = new Dictionary<int, List<CombatSceneObject>>();
				Dictionary<int, List<CombatSceneObject>> root2Groups = new Dictionary<int, List<CombatSceneObject>>();
				Dictionary<int, List<CombatSceneObject>> singleGroups = new Dictionary<int, List<CombatSceneObject>>();
				foreach (CombatSceneObject obj2 in sceneObjects)
				{
					bool flag8 = obj2 == null;
					if (!flag8)
					{
						bool flag9 = obj2.GroupId <= 0;
						if (!flag9)
						{
							bool flag10 = !isBlendableScene;
							Dictionary<int, List<CombatSceneObject>> groups;
							if (flag10)
							{
								groups = singleGroups;
							}
							else
							{
								int idx = CombatScene.<GmSwitchToWeatherNode>g__GetRootIndex|54_0(obj2, root, root2);
								groups = ((idx == 1) ? root2Groups : root1Groups);
							}
							List<CombatSceneObject> list;
							bool flag11 = !groups.TryGetValue(obj2.GroupId, out list);
							if (flag11)
							{
								list = new List<CombatSceneObject>();
								groups.Add(obj2.GroupId, list);
							}
							list.Add(obj2);
						}
					}
				}
				bool found = false;
				bool flag12 = !isBlendableScene;
				if (flag12)
				{
					foreach (List<CombatSceneObject> members in singleGroups.Values)
					{
						CombatSceneObject chosen = null;
						for (int j = 0; j < members.Count; j++)
						{
							CombatSceneObject member = members[j];
							bool flag13 = member == null;
							if (!flag13)
							{
								bool flag14 = member.name == nodeName;
								if (flag14)
								{
									chosen = member;
									break;
								}
							}
						}
						bool flag15 = chosen == null;
						if (!flag15)
						{
							found = true;
							for (int k = 0; k < members.Count; k++)
							{
								CombatSceneObject member2 = members[k];
								bool flag16 = member2 != null;
								if (flag16)
								{
									member2.gameObject.SetActive(member2 == chosen);
								}
							}
						}
					}
					result = found;
				}
				else
				{
					HashSet<int> allGroupIds = new HashSet<int>();
					foreach (int gid in root1Groups.Keys)
					{
						allGroupIds.Add(gid);
					}
					foreach (int gid2 in root2Groups.Keys)
					{
						allGroupIds.Add(gid2);
					}
					foreach (int groupId in allGroupIds)
					{
						List<CombatSceneObject> root1Members;
						root1Groups.TryGetValue(groupId, out root1Members);
						List<CombatSceneObject> root2Members;
						root2Groups.TryGetValue(groupId, out root2Members);
						CombatSceneObject chosenRoot = null;
						bool flag17 = root1Members != null;
						if (flag17)
						{
							for (int l = 0; l < root1Members.Count; l++)
							{
								CombatSceneObject member3 = root1Members[l];
								bool flag18 = member3 == null;
								if (!flag18)
								{
									bool flag19 = member3.name == nodeName;
									if (flag19)
									{
										chosenRoot = member3;
										break;
									}
								}
							}
						}
						CombatSceneObject chosenRoot2 = null;
						bool flag20 = root2Members != null;
						if (flag20)
						{
							string parentName = (chosenRoot != null && chosenRoot.transform.parent != null) ? chosenRoot.transform.parent.name : null;
							bool flag21 = !string.IsNullOrEmpty(parentName);
							if (flag21)
							{
								for (int m = 0; m < root2Members.Count; m++)
								{
									CombatSceneObject member4 = root2Members[m];
									bool flag22 = member4 == null;
									if (!flag22)
									{
										bool flag23 = member4.name == nodeName && member4.transform.parent != null && member4.transform.parent.name == parentName;
										if (flag23)
										{
											chosenRoot2 = member4;
											break;
										}
									}
								}
							}
							bool flag24 = chosenRoot2 == null;
							if (flag24)
							{
								for (int n = 0; n < root2Members.Count; n++)
								{
									CombatSceneObject member5 = root2Members[n];
									bool flag25 = member5 == null;
									if (!flag25)
									{
										bool flag26 = member5.name == nodeName;
										if (flag26)
										{
											chosenRoot2 = member5;
											break;
										}
									}
								}
							}
						}
						bool flag27 = chosenRoot == null && chosenRoot2 == null;
						if (!flag27)
						{
							found = true;
							bool flag28 = root1Members != null && chosenRoot != null;
							if (flag28)
							{
								for (int i2 = 0; i2 < root1Members.Count; i2++)
								{
									CombatSceneObject member6 = root1Members[i2];
									bool flag29 = member6 != null;
									if (flag29)
									{
										member6.gameObject.SetActive(member6 == chosenRoot);
									}
								}
							}
							bool flag30 = root2Members != null && chosenRoot2 != null;
							if (flag30)
							{
								for (int i3 = 0; i3 < root2Members.Count; i3++)
								{
									CombatSceneObject member7 = root2Members[i3];
									bool flag31 = member7 != null;
									if (flag31)
									{
										member7.gameObject.SetActive(member7 == chosenRoot2);
									}
								}
							}
						}
					}
					result = found;
				}
			}
		}
		return result;
	}

	// Token: 0x06001326 RID: 4902 RVA: 0x00075F88 File Offset: 0x00074188
	public bool GmForceNoWeather()
	{
		bool isBlendableScene = this.Roots != null && this.Roots.Length == 2 && this.Roots[0] != null && this.Roots[1] != null;
		List<CombatSceneObject> list = new List<CombatSceneObject>();
		bool flag = isBlendableScene;
		if (flag)
		{
			list.AddRange(this.Roots[0].GetComponentsInChildren<CombatSceneObject>(true));
			list.AddRange(this.Roots[1].GetComponentsInChildren<CombatSceneObject>(true));
		}
		else
		{
			list.AddRange(base.GetComponentsInChildren<CombatSceneObject>(true));
		}
		bool flag2 = list.Count == 0;
		bool result;
		if (flag2)
		{
			result = false;
		}
		else
		{
			bool found = false;
			for (int i = 0; i < list.Count; i++)
			{
				CombatSceneObject obj = list[i];
				bool flag3 = obj == null;
				if (!flag3)
				{
					bool flag4 = obj.name == "xiaoyu" || obj.name == "dayu" || obj.name == "xiaoxue" || obj.name == "daxue";
					if (flag4)
					{
						found = true;
						obj.gameObject.SetActive(false);
					}
				}
			}
			result = found;
		}
		return result;
	}

	// Token: 0x06001328 RID: 4904 RVA: 0x00076108 File Offset: 0x00074308
	[CompilerGenerated]
	internal static int <GmSwitchToWeatherNode>g__GetRootIndex|54_0(CombatSceneObject obj, Transform r1, Transform r2)
	{
		bool flag = obj == null;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			bool flag2 = r1 == null || r2 == null;
			if (flag2)
			{
				result = -1;
			}
			else
			{
				Transform cur = obj.transform;
				while (cur != null)
				{
					bool flag3 = cur == r1;
					if (flag3)
					{
						return 0;
					}
					bool flag4 = cur == r2;
					if (flag4)
					{
						return 1;
					}
					cur = cur.parent;
				}
				result = -1;
			}
		}
		return result;
	}

	// Token: 0x0400100B RID: 4107
	public float CurOffsetValue;

	// Token: 0x0400100C RID: 4108
	[SerializeField]
	private List<int> _layerActiveConfig;

	// Token: 0x0400100D RID: 4109
	public List<CombatScene.TransitionStage> TransitionStageList;

	// Token: 0x0400100E RID: 4110
	[Tooltip("只支持2个，不设置视为关闭过渡功能")]
	public RectTransform[] Roots;

	// Token: 0x0400100F RID: 4111
	[Tooltip("多少秒从root2过渡到root1")]
	public float BlendDurationTo1;

	// Token: 0x04001010 RID: 4112
	[Tooltip("多少秒从root1过渡到root2")]
	public float BlendDurationTo2;

	// Token: 0x04001012 RID: 4114
	private CombatSceneLayer[][] _layers;

	// Token: 0x04001013 RID: 4115
	private bool _initFlag = false;

	// Token: 0x04001014 RID: 4116
	private float _blendValue;

	// Token: 0x04001015 RID: 4117
	private bool _blendDirection;

	// Token: 0x04001016 RID: 4118
	private bool _blendPrepared;

	// Token: 0x04001017 RID: 4119
	private const int RootCount = 2;

	// Token: 0x04001018 RID: 4120
	private RenderTexture _root2Rt;

	// Token: 0x04001019 RID: 4121
	private CRawImage _root2RawImage;

	// Token: 0x0400101A RID: 4122
	private GameObject _root2CameraObject;

	// Token: 0x0400101B RID: 4123
	private readonly Dictionary<int, CombatScene.GroupRuntimeState> _cachedRoot1Groups = new Dictionary<int, CombatScene.GroupRuntimeState>();

	// Token: 0x0400101C RID: 4124
	private readonly Dictionary<int, CombatScene.GroupRuntimeState> _cachedRoot2Groups = new Dictionary<int, CombatScene.GroupRuntimeState>();

	// Token: 0x0400101D RID: 4125
	private readonly Dictionary<int, CombatSceneObject> _cachedRoot1Choices = new Dictionary<int, CombatSceneObject>();

	// Token: 0x02001232 RID: 4658
	[Serializable]
	public struct TransitionItem
	{
		// Token: 0x040099D4 RID: 39380
		public GameObject Target;

		// Token: 0x040099D5 RID: 39381
		public string AnimationName;
	}

	// Token: 0x02001233 RID: 4659
	[Serializable]
	public struct TransitionStage
	{
		// Token: 0x040099D6 RID: 39382
		public List<CombatScene.TransitionItem> TransitionBeginItems;

		// Token: 0x040099D7 RID: 39383
		public List<CombatScene.TransitionItem> TransitionEndItems;

		// Token: 0x040099D8 RID: 39384
		public string StageKey;
	}

	// Token: 0x02001234 RID: 4660
	private sealed class GroupRuntimeState
	{
		// Token: 0x0600C501 RID: 50433 RVA: 0x0057DD6C File Offset: 0x0057BF6C
		public GroupRuntimeState(int groupId)
		{
			this.GroupId = groupId;
		}

		// Token: 0x17001617 RID: 5655
		// (get) Token: 0x0600C502 RID: 50434 RVA: 0x0057DD88 File Offset: 0x0057BF88
		public int GroupId { get; }

		// Token: 0x17001618 RID: 5656
		// (get) Token: 0x0600C503 RID: 50435 RVA: 0x0057DD90 File Offset: 0x0057BF90
		public List<CombatSceneObject> Members { get; } = new List<CombatSceneObject>();

		// Token: 0x0600C504 RID: 50436 RVA: 0x0057DD98 File Offset: 0x0057BF98
		public void AddMember(CombatSceneObject obj)
		{
			this.Members.Add(obj);
		}
	}
}
