using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Components.Avatar;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200007B RID: 123
public class MonolithAlphaSystem : MonoBehaviour
{
	// Token: 0x06000463 RID: 1123 RVA: 0x0001CFB0 File Offset: 0x0001B1B0
	private void Awake()
	{
		MonolithAlphaSystem.Instance = this;
		this._RenderCamera = base.GetComponent<Camera>();
		this._RenderCamera.enabled = false;
		this._RenderCanvas = base.GetComponentInChildren<Canvas>();
		base.transform.position = base.transform.position.SetX(0f);
		this.Clear();
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x0001D013 File Offset: 0x0001B213
	private void OnDestroy()
	{
		this.Clear();
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x0001D020 File Offset: 0x0001B220
	private void Clear()
	{
		bool flag = this._OnClear != null;
		if (flag)
		{
			this._OnClear();
			this._OnClear = null;
		}
		foreach (RectTransform spawn in this._Spawns)
		{
			Object.DestroyImmediate(spawn.gameObject);
		}
		this._Spawns.Clear();
		base.StopAllCoroutines();
		this._RenderCamera.enabled = false;
		foreach (KeyValuePair<Vector2, RenderTexture> pair in this._RenderTextures)
		{
			bool flag2 = pair.Value != null && pair.Value.IsCreated();
			if (flag2)
			{
				pair.Value.Release();
			}
		}
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x0001D134 File Offset: 0x0001B334
	public void StartFading(Transform target, float startValue, float endValue, float time, Action<GameObject> spawnInitializer = null, Action<CRawImage> onComplete = null)
	{
		this.Clear();
		base.StartCoroutine(this._Process(target, startValue, endValue, time, spawnInitializer, onComplete));
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x0001D154 File Offset: 0x0001B354
	public void StartFading(Game.Components.Avatar.Avatar target, float startValue, float endValue, float time, Action<CRawImage> onComplete = null)
	{
		this.StartFading(target.transform, startValue, endValue, time, delegate(GameObject dummy)
		{
			Game.Components.Avatar.Avatar avatar = dummy.GetComponent<Game.Components.Avatar.Avatar>();
			avatar.Refresh(target.Data, target.AvatarAge);
		}, onComplete);
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x0001D194 File Offset: 0x0001B394
	public CRawImage RenderCRawImage(RectTransform target)
	{
		RectTransform renderTarget = Object.Instantiate<RectTransform>(target, this._RenderCanvas.transform, false);
		Debug.Log("prepare " + base.GetType().Name + " rt");
		RenderTexture rt = new RenderTexture((int)renderTarget.rect.size.x, (int)renderTarget.rect.size.y, 32);
		Debug.Log("completed " + base.GetType().Name + " rt");
		this._RenderCamera.targetTexture = rt;
		this._RenderCamera.Render();
		CRawImage spawnImage = new GameObject().AddComponent<CRawImage>();
		RectTransform spawnTransform = spawnImage.rectTransform;
		Vector2 targetSize = new Vector2((float)((int)renderTarget.rect.size.x), (float)((int)renderTarget.rect.size.y));
		spawnTransform.pivot = renderTarget.pivot;
		spawnTransform.localPosition = renderTarget.localPosition;
		spawnTransform.localScale = renderTarget.localScale;
		spawnTransform.sizeDelta = targetSize;
		spawnImage.texture = rt;
		this._RenderCamera.targetTexture = null;
		Object.DestroyImmediate(renderTarget.gameObject);
		return spawnImage;
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x0001D2DD File Offset: 0x0001B4DD
	public void StartFading(CricketView target, float startValue, float endValue, float time, Action<CRawImage> onComplete = null)
	{
		this.StartFading(target.transform, startValue, endValue, time, delegate(GameObject dummy)
		{
			CricketView cricket = dummy.GetComponent<CricketView>();
			cricket.Inited = false;
			cricket.SetCricketData(cricket.ColorId, cricket.PartId, false, null, false);
		}, onComplete);
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0001D312 File Offset: 0x0001B512
	protected IEnumerator _Process(Transform target, float startValue, float endValue, float time, Action<GameObject> spawnInitializer, Action<CRawImage> onComplete = null)
	{
		Graphic graphic = target.GetComponent<Graphic>();
		Color rawColor = Color.white;
		GameObject targetGameObject = target.gameObject;
		RectTransform targetRect = target as RectTransform;
		bool targetVisible = targetGameObject.activeSelf;
		bool flag = targetRect == null;
		if (flag)
		{
			yield break;
		}
		Vector2 targetSize = new Vector2((float)((int)targetRect.rect.size.x), (float)((int)targetRect.rect.size.y));
		GameObject dummy = Object.Instantiate<GameObject>(targetGameObject, this._RenderCanvas.transform);
		Vector2 spawnOffset = Vector2.zero;
		RenderTexture renderTexture;
		bool flag2 = !this._RenderTextures.TryGetValue(targetSize, out renderTexture);
		if (flag2)
		{
			Debug.Log("prepare " + base.GetType().Name + " renderTexture");
			renderTexture = new RenderTexture((int)targetSize.x, (int)targetSize.y, 0);
			renderTexture.Create();
			Debug.Log("completed " + base.GetType().Name + " renderTexture");
			this._RenderTextures.Add(targetRect.rect.size, renderTexture);
		}
		else
		{
			bool flag3 = renderTexture != null && !renderTexture.IsCreated();
			if (flag3)
			{
				renderTexture.Create();
			}
		}
		this._OnClear = delegate()
		{
			bool flag5 = targetGameObject != null;
			if (flag5)
			{
			}
			bool flag6 = renderTexture != null && renderTexture.IsCreated();
			if (flag6)
			{
				renderTexture.Release();
			}
		};
		this._RenderCamera.targetTexture = renderTexture;
		RectTransform canvasRect = this._RenderCanvas.GetComponent<RectTransform>();
		canvasRect.localPosition = Vector3.zero;
		canvasRect.sizeDelta = targetSize;
		canvasRect.ForceUpdateRectTransforms();
		canvasRect = null;
		dummy.gameObject.SetActive(false);
		targetGameObject.SetActive(false);
		yield return null;
		dummy.gameObject.SetActive(true);
		RectTransform dummyRect = (RectTransform)dummy.transform;
		MonolithAlphaSystem.<_Process>g__ResetLayerMask|13_1(dummyRect, 1);
		dummyRect.localScale = Vector3.one;
		dummyRect.position = Vector3.zero;
		Rect dummyRectData = dummyRect.rect;
		spawnOffset.x = targetSize.x * 0.5f - dummyRectData.xMax;
		spawnOffset.y = targetSize.y * 0.5f - dummyRectData.yMax;
		dummyRect.localPosition += new Vector3(spawnOffset.x, spawnOffset.y, 0f);
		dummyRectData = default(Rect);
		dummyRect.ForceUpdateRectTransforms();
		dummyRect = null;
		if (spawnInitializer != null)
		{
			spawnInitializer(dummy);
		}
		yield return new WaitForSeconds(0.15f);
		this._RenderCamera.Render();
		Object.DestroyImmediate(dummy.gameObject);
		CRawImage spawnImage = new GameObject().AddComponent<CRawImage>();
		RectTransform spawnTransform = spawnImage.rectTransform;
		spawnTransform.SetParent(target.parent);
		spawnTransform.pivot = targetRect.pivot;
		spawnTransform.localPosition = targetRect.localPosition;
		spawnTransform.localScale = targetRect.localScale;
		spawnTransform.sizeDelta = targetSize;
		spawnImage.texture = renderTexture;
		this._Spawns.Add(spawnTransform);
		spawnTransform = null;
		Graphic graphic2 = spawnImage;
		Graphic graphic3 = graphic;
		graphic2.raycastTarget = (graphic3 != null && graphic3.raycastTarget);
		for (float processedTime = 0f; processedTime < time; processedTime += Time.deltaTime)
		{
			float process = processedTime / time;
			bool flag4 = spawnImage == null;
			if (flag4)
			{
				break;
			}
			spawnImage.color = new Color(rawColor.r, rawColor.g, rawColor.b, Mathf.Lerp(startValue, endValue, process));
			yield return new WaitForEndOfFrame();
		}
		if (onComplete != null)
		{
			onComplete(spawnImage);
		}
		this.Clear();
		yield break;
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x0001D37C File Offset: 0x0001B57C
	[CompilerGenerated]
	internal static void <_Process>g__ResetLayerMask|13_1(Transform t, int layer = 1)
	{
		t.gameObject.layer = layer;
		for (int i = 0; i < t.childCount; i++)
		{
			MonolithAlphaSystem.<_Process>g__ResetLayerMask|13_1(t.GetChild(i), layer);
		}
	}

	// Token: 0x040002C2 RID: 706
	public static MonolithAlphaSystem Instance;

	// Token: 0x040002C3 RID: 707
	private Dictionary<Vector2, RenderTexture> _RenderTextures = new Dictionary<Vector2, RenderTexture>();

	// Token: 0x040002C4 RID: 708
	private Camera _RenderCamera = null;

	// Token: 0x040002C5 RID: 709
	private Canvas _RenderCanvas = null;

	// Token: 0x040002C6 RID: 710
	private HashSet<RectTransform> _Spawns = new HashSet<RectTransform>();

	// Token: 0x040002C7 RID: 711
	private Action _OnClear;
}
