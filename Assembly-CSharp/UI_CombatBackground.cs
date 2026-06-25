using System;
using System.Collections.Generic;
using System.IO;
using Config;
using FrameWork;
using GameData.Domains.World;
using Spine;
using Spine.Unity;
using UnityEngine;

// Token: 0x020001DC RID: 476
public class UI_CombatBackground : UIBase
{
	// Token: 0x17000324 RID: 804
	// (get) Token: 0x06001F2D RID: 7981 RVA: 0x000E3169 File Offset: 0x000E1369
	// (set) Token: 0x06001F2E RID: 7982 RVA: 0x000E3170 File Offset: 0x000E1370
	public static UI_CombatBackground Instance { get; private set; }

	// Token: 0x17000325 RID: 805
	// (get) Token: 0x06001F2F RID: 7983 RVA: 0x000E3178 File Offset: 0x000E1378
	// (set) Token: 0x06001F30 RID: 7984 RVA: 0x000E3180 File Offset: 0x000E1380
	public global::CombatScene Scene { get; private set; }

	// Token: 0x06001F31 RID: 7985 RVA: 0x000E318C File Offset: 0x000E138C
	public override void OnInit(ArgumentBox argsBox)
	{
		this._inited = false;
		this.Scene = null;
		bool flag = base.transform.childCount > 0;
		if (flag)
		{
			Object.Destroy(base.transform.GetChild(0).gameObject);
		}
		CombatModel model = SingletonObject.getInstance<CombatModel>();
		List<string> prefabList = model.Scene.PrefabPath;
		string prefabPath = prefabList[Random.Range(0, prefabList.Count)];
		bool flag2 = model.Scene.HasWinterResource && TimeKit.GetCurrSeason() == 3;
		if (flag2)
		{
			prefabPath = UI_CombatBackground.GetWinterPrefabPath(prefabPath);
		}
		this.LoadScene(prefabPath);
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x000E3224 File Offset: 0x000E1424
	private void OnEnable()
	{
		GEvent.Add(UiEvents.GmReloadCombatScene, new GEvent.Callback(this.OnGmReloadCombatScene));
	}

	// Token: 0x06001F33 RID: 7987 RVA: 0x000E3243 File Offset: 0x000E1443
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.GmReloadCombatScene, new GEvent.Callback(this.OnGmReloadCombatScene));
	}

	// Token: 0x06001F34 RID: 7988 RVA: 0x000E3262 File Offset: 0x000E1462
	private void Awake()
	{
		UI_CombatBackground.Instance = this;
	}

	// Token: 0x06001F35 RID: 7989 RVA: 0x000E326C File Offset: 0x000E146C
	private void OnDestroy()
	{
		UI_CombatBackground.Instance = null;
	}

	// Token: 0x06001F36 RID: 7990 RVA: 0x000E3278 File Offset: 0x000E1478
	public void SetOffset(float x)
	{
		bool flag = !this._inited;
		if (!flag)
		{
			this.Scene.SetOffset(x);
		}
	}

	// Token: 0x06001F37 RID: 7991 RVA: 0x000E32A4 File Offset: 0x000E14A4
	public void SetPause(bool pause)
	{
		this.RefreshSkeletonArrays();
		foreach (SkeletonAnimation ani in this._skeletonAnimations)
		{
			ani.timeScale = (float)(pause ? 0 : 1);
		}
		foreach (SkeletonGraphic graphic in this._skeletonGraphics)
		{
			graphic.timeScale = (float)(pause ? 0 : 1);
		}
	}

	// Token: 0x06001F38 RID: 7992 RVA: 0x000E3310 File Offset: 0x000E1510
	private void RefreshSkeletonArrays()
	{
		bool flag = this.Scene == null || this.Scene.Roots == null;
		if (!flag)
		{
			List<SkeletonAnimation> aniList = new List<SkeletonAnimation>();
			List<SkeletonGraphic> gfxList = new List<SkeletonGraphic>();
			foreach (RectTransform root in this.Scene.Roots)
			{
				bool flag2 = root == null;
				if (!flag2)
				{
					aniList.AddRange(root.GetComponentsInChildren<SkeletonAnimation>(true));
					gfxList.AddRange(root.GetComponentsInChildren<SkeletonGraphic>(true));
				}
			}
			this._skeletonAnimations = aniList.ToArray();
			this._skeletonGraphics = gfxList.ToArray();
		}
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x000E33BC File Offset: 0x000E15BC
	public override void QuickHide()
	{
		bool flag = this.Scene && this.Scene.gameObject.activeSelf;
		if (flag)
		{
			this.Scene.gameObject.SetActive(false);
		}
		base.QuickHide();
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x000E3408 File Offset: 0x000E1608
	private void ResetAllAnimationTimes()
	{
		foreach (SkeletonAnimation ani in this._skeletonAnimations)
		{
			bool flag = ani.AnimationState == null;
			if (!flag)
			{
				ExposedList<TrackEntry> tracks = ani.AnimationState.Tracks;
				bool flag2 = tracks == null;
				if (!flag2)
				{
					foreach (TrackEntry track in tracks)
					{
						bool flag3 = track != null;
						if (flag3)
						{
							track.TrackTime = 0f;
						}
					}
				}
			}
		}
		foreach (SkeletonGraphic graphic in this._skeletonGraphics)
		{
			bool flag4 = graphic.AnimationState == null;
			if (!flag4)
			{
				ExposedList<TrackEntry> tracks2 = graphic.AnimationState.Tracks;
				bool flag5 = tracks2 == null;
				if (!flag5)
				{
					foreach (TrackEntry track2 in tracks2)
					{
						bool flag6 = track2 != null;
						if (flag6)
						{
							track2.TrackTime = 0f;
						}
					}
				}
			}
		}
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x000E3564 File Offset: 0x000E1764
	private void OnGmReloadCombatScene(ArgumentBox argumentBox)
	{
		short sceneId;
		argumentBox.Get("SceneId", out sceneId);
		int index;
		argumentBox.Get("Index", out index);
		bool forceWinter;
		argumentBox.Get("ForceWinter", out forceWinter);
		this.ReloadScene(sceneId, index, forceWinter);
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x000E35A8 File Offset: 0x000E17A8
	public void ReloadScene(short sceneId, int index, bool forceWinter = false)
	{
		CombatSceneItem sceneItem = Config.CombatScene.Instance[sceneId];
		List<string> prefabList = sceneItem.PrefabPath;
		bool flag = index < 0 || index >= prefabList.Count;
		if (flag)
		{
			Debug.LogWarning(string.Format("[ReloadScene] Scene Index out of range. expected: [0, {0}], got {1}", prefabList.Count - 1, index));
		}
		else
		{
			bool flag2 = this.Scene != null && this.Scene.gameObject != null;
			if (flag2)
			{
				Object.Destroy(this.Scene.gameObject);
				this.Scene = null;
			}
			this._inited = false;
			bool flag3 = base.transform.childCount > 0;
			if (flag3)
			{
				Object.Destroy(base.transform.GetChild(0).gameObject);
			}
			string prefabPath = prefabList[index];
			if (forceWinter)
			{
				prefabPath = UI_CombatBackground.GetWinterPrefabPath(prefabPath);
			}
			else
			{
				bool flag4 = sceneItem.HasWinterResource && TimeKit.GetCurrSeason() == 3;
				if (flag4)
				{
					prefabPath = UI_CombatBackground.GetWinterPrefabPath(prefabPath);
				}
			}
			this.LoadScene(prefabPath);
		}
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x000E36C0 File Offset: 0x000E18C0
	private static string GetWinterPrefabPath(string prefabPath)
	{
		string[] parts = prefabPath.Split('/', StringSplitOptions.None);
		for (int i = 0; i < parts.Length; i++)
		{
			parts[i] += "_winter";
		}
		return string.Join("/", parts);
	}

	// Token: 0x06001F3E RID: 7998 RVA: 0x000E3709 File Offset: 0x000E1909
	private void LoadScene(string prefabPath)
	{
		ResLoader.Load<GameObject>(Path.Combine("RemakeResources/Combat/CombatScenes/", prefabPath), new Action<GameObject>(this.OnSceneLoaded), null, false);
	}

	// Token: 0x06001F3F RID: 7999 RVA: 0x000E372C File Offset: 0x000E192C
	private void OnSceneLoaded(GameObject prefab)
	{
		GameObject go = Object.Instantiate<GameObject>(prefab);
		Transform[] children = go.GetComponentsInChildren<Transform>();
		foreach (Transform one in children)
		{
			one.gameObject.layer = LayerMask.NameToLayer("UI");
		}
		Canvas[] canvas = go.GetComponentsInChildren<Canvas>();
		foreach (Canvas one2 in canvas)
		{
			one2.sortingLayerName = "UI";
		}
		this._skeletonAnimations = go.GetComponentsInChildren<SkeletonAnimation>(true);
		this._skeletonGraphics = go.GetComponentsInChildren<SkeletonGraphic>(true);
		foreach (SkeletonAnimation ani in this._skeletonAnimations)
		{
			ani.GetComponent<MeshRenderer>().sortingLayerName = "UI";
		}
		this.SetPause(false);
		go.transform.SetParent(base.transform, false);
		this.Scene = go.GetComponent<global::CombatScene>();
		bool flag = this.Scene == null;
		if (flag)
		{
			this.Scene = go.AddComponent<global::CombatScene>();
		}
		this.Scene.Awake();
		this.Scene.UpdateSceneScale(0f, 0f);
		this.Scene.SetOffset(0f);
		sbyte month = SingletonObject.getInstance<TimeManager>().GetMonthInCurrYear();
		this.Scene.RefreshSeasonalAndRandomVisibility((int)month);
		this.ResetAllAnimationTimes();
		this._inited = true;
	}

	// Token: 0x04001788 RID: 6024
	private const string PrefabDir = "RemakeResources/Combat/CombatScenes/";

	// Token: 0x0400178B RID: 6027
	private SkeletonAnimation[] _skeletonAnimations;

	// Token: 0x0400178C RID: 6028
	private SkeletonGraphic[] _skeletonGraphics;

	// Token: 0x0400178D RID: 6029
	private bool _inited;
}
