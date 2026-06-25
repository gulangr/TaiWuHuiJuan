using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Coffee.UIExtensions;
using UnityEngine;

namespace Game.Components.Avatar
{
	// Token: 0x02000F82 RID: 3970
	public class AvatarEffectHelper : ISingletonInit, IDisposable
	{
		// Token: 0x0600B64A RID: 46666 RVA: 0x00530374 File Offset: 0x0052E574
		public void Dispose()
		{
			bool flag = null != this._poolRoot;
			if (flag)
			{
				Object.Destroy(this._poolRoot.gameObject);
			}
		}

		// Token: 0x0600B64B RID: 46667 RVA: 0x005303A8 File Offset: 0x0052E5A8
		public void Init()
		{
			GameObject rootObject = new GameObject
			{
				name = "AvatarEffectPoolRoot"
			};
			Object.DontDestroyOnLoad(rootObject);
			this._poolRoot = rootObject.transform;
			this._poolRoot.localPosition = new Vector3(10000f, 10000f, 0f);
		}

		// Token: 0x0600B64C RID: 46668 RVA: 0x005303FC File Offset: 0x0052E5FC
		public void GetParticle(GameObject avatar, string clothEffectName, Transform parent)
		{
			ValueTuple<GameObject, string> key = new ValueTuple<GameObject, string>(avatar, clothEffectName);
			GameObject uiParticleObject;
			bool flag = this._effectPool.TryGetValue(key, out uiParticleObject);
			if (flag)
			{
				uiParticleObject.transform.SetParent(parent);
				uiParticleObject.transform.localPosition = Vector3.zero;
				uiParticleObject.transform.localRotation = Quaternion.identity;
				uiParticleObject.transform.localScale = Vector3.one;
				RectTransform rectTransform = uiParticleObject.GetComponent<RectTransform>();
				rectTransform.sizeDelta = Vector2.zero;
				UIParticle clothParticle = uiParticleObject.GetComponent<UIParticle>();
				Coroutine oldCoroutine;
				bool flag2 = this._effectDelayCoroutineDict.TryGetValue(key, out oldCoroutine);
				if (flag2)
				{
					SingletonObject.getInstance<YieldHelper>().StopCoroutine(oldCoroutine);
				}
				Coroutine coroutine = SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
				{
					clothParticle.scale = clothParticle.transform.localScale.x;
					Transform effect = uiParticleObject.transform.GetChild(0);
					effect.transform.localScale = new Vector3(1f / clothParticle.transform.localScale.x, 1f / clothParticle.transform.localScale.y, 1f / clothParticle.transform.localScale.z);
				});
				this._effectDelayCoroutineDict[key] = coroutine;
			}
			else
			{
				uiParticleObject = new GameObject("AvatarEffect_" + clothEffectName);
				uiParticleObject.transform.SetParent(parent);
				uiParticleObject.transform.localPosition = Vector3.zero;
				uiParticleObject.transform.localRotation = Quaternion.identity;
				uiParticleObject.transform.localScale = Vector3.one;
				RectTransform rectTransform2 = uiParticleObject.AddComponent<RectTransform>();
				rectTransform2.sizeDelta = Vector2.zero;
				this._effectPool.Add(key, uiParticleObject);
				uiParticleObject.AddComponent<UIParticle>();
				UIParticle uiParticle = uiParticleObject.GetComponent<UIParticle>();
				uiParticle.scale = 144f;
				ResLoader.Load<GameObject>("RemakeResources/Particle/UIEffectPrefabs/Avatar/" + clothEffectName, delegate(GameObject prefab)
				{
					GameObject effectObj = Object.Instantiate<GameObject>(prefab, uiParticle.transform, true);
					effectObj.name = clothEffectName;
					effectObj.transform.localPosition = Vector3.zero;
					effectObj.SetActive(true);
					uiParticle.RefreshParticles();
					Coroutine oldCoroutine2;
					bool flag3 = this._effectDelayCoroutineDict.TryGetValue(key, out oldCoroutine2);
					if (flag3)
					{
						SingletonObject.getInstance<YieldHelper>().StopCoroutine(oldCoroutine2);
					}
					Coroutine coroutine2 = SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
					{
						uiParticle.scale = uiParticle.transform.localScale.x;
						effectObj.transform.localScale = new Vector3(1f / uiParticle.transform.localScale.x, 1f / uiParticle.transform.localScale.y, 1f / uiParticle.transform.localScale.z);
					});
					this._effectDelayCoroutineDict[key] = coroutine2;
				}, delegate(string path)
				{
					GLog.Error("Failed to load Resource " + path);
					uiParticle.gameObject.SetActive(false);
				}, false);
			}
		}

		// Token: 0x0600B64D RID: 46669 RVA: 0x00530658 File Offset: 0x0052E858
		public void ReturnParticle(GameObject avatar, string nodeName)
		{
			string effectName = nodeName.Substring("AvatarEffect_".Length);
			ValueTuple<GameObject, string> key = new ValueTuple<GameObject, string>(avatar, effectName);
			GameObject effect;
			bool flag = this._effectPool.TryGetValue(key, out effect);
			if (flag)
			{
				effect.transform.SetParent(this._poolRoot);
				effect.transform.localPosition = Vector3.zero;
				effect.transform.localRotation = Quaternion.identity;
				effect.transform.localScale = Vector3.one;
			}
		}

		// Token: 0x0600B64E RID: 46670 RVA: 0x005306DC File Offset: 0x0052E8DC
		public bool IsAvatarEffect(string nodeName)
		{
			return nodeName.StartsWith("AvatarEffect_");
		}

		// Token: 0x04008D72 RID: 36210
		private Transform _poolRoot;

		// Token: 0x04008D73 RID: 36211
		[TupleElementNames(new string[]
		{
			"avatar",
			"clothEffectName"
		})]
		private readonly Dictionary<ValueTuple<GameObject, string>, GameObject> _effectPool = new Dictionary<ValueTuple<GameObject, string>, GameObject>();

		// Token: 0x04008D74 RID: 36212
		[TupleElementNames(new string[]
		{
			"avatar",
			"clothEffectName"
		})]
		private readonly Dictionary<ValueTuple<GameObject, string>, Coroutine> _effectDelayCoroutineDict = new Dictionary<ValueTuple<GameObject, string>, Coroutine>();

		// Token: 0x04008D75 RID: 36213
		private const string EffectNodeNamePrefix = "AvatarEffect_";
	}
}
