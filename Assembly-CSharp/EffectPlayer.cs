using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using DG.Tweening;
using FrameWork.Tools;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200005F RID: 95
public class EffectPlayer : MonoBehaviour
{
	// Token: 0x0600031E RID: 798 RVA: 0x00013291 File Offset: 0x00011491
	private void Awake()
	{
		this._effectPoolMap = new Dictionary<string, PoolItem>();
		this._uiParticle = base.GetComponent<UIParticle>();
	}

	// Token: 0x0600031F RID: 799 RVA: 0x000132AC File Offset: 0x000114AC
	private PoolItem GetEffectPool(string effectName)
	{
		PoolItem poolItem;
		bool flag = this._effectPoolMap.TryGetValue(effectName, out poolItem);
		PoolItem result;
		if (flag)
		{
			result = poolItem;
		}
		else
		{
			GameObject srcObject = Array.Find<GameObject>(this.EffectCore, (GameObject e) => e.name == effectName);
			Assert.IsNotNull<GameObject>(srcObject, effectName + " not exist in effect core");
			poolItem = new PoolItem(effectName, srcObject);
			this._effectPoolMap.Add(effectName, poolItem);
			result = poolItem;
		}
		return result;
	}

	// Token: 0x06000320 RID: 800 RVA: 0x0001333C File Offset: 0x0001153C
	private GameObject GetEffectObject(string effectName)
	{
		PoolItem poolItem = this.GetEffectPool(effectName);
		GameObject result = poolItem.GetObject();
		result.name = effectName;
		return result;
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00013368 File Offset: 0x00011568
	public void ReturnEffectObject(GameObject effectObject)
	{
		bool flag = null == effectObject;
		if (!flag)
		{
			PoolItem poolItem;
			this._effectPoolMap.TryGetValue(effectObject.name, out poolItem);
			Assert.IsNotNull<PoolItem>(poolItem, effectObject.name + " is not from EffectCore,return failed");
			poolItem.DestroyObject(effectObject);
		}
	}

	// Token: 0x06000322 RID: 802 RVA: 0x000133B6 File Offset: 0x000115B6
	private IEnumerator PlayFlyEffectCoroutine(EffectBezierFlyCommandUGUI cmd)
	{
		DOVirtual.Float(0f, 1f, cmd.Duration, delegate(float stepValue)
		{
			cmd.EffectObject.transform.localPosition = BezierMath.Bezier2(cmd.FromPosition, cmd.ControlPosition, cmd.ToPosition, stepValue);
		}).SetEase(cmd.EaseType).SetUpdate(true).SetAutoKill(true);
		yield return new WaitForSeconds(cmd.Duration);
		yield return null;
		Action onShowComplete = cmd.OnShowComplete;
		if (onShowComplete != null)
		{
			onShowComplete();
		}
		AudioManager.Instance.PlaySound("ui_art_scoreget", false, false);
		this.ReturnEffectObject(cmd.EffectObject);
		yield break;
	}

	// Token: 0x06000323 RID: 803 RVA: 0x000133CC File Offset: 0x000115CC
	private IEnumerator CollectEffectObjectCoroutine(GameObject effectObj, float delay)
	{
		bool flag = null == effectObj;
		if (flag)
		{
			yield break;
		}
		yield return new WaitForSeconds(delay);
		this.ReturnEffectObject(effectObj);
		yield break;
	}

	// Token: 0x06000324 RID: 804 RVA: 0x000133EC File Offset: 0x000115EC
	public EffectBezierFlyCommandUGUI AddFlyEffectCommand(RectTransform from, RectTransform to, string effectKey, Func<Vector2, Vector2, Vector2> getBezierControlPos = null)
	{
		Vector3 localFromPos = base.transform.InverseTransformPoint(from.position);
		Vector3 localToPos = base.transform.InverseTransformPoint(to.position);
		Vector3 controlPos = localFromPos;
		bool flag = getBezierControlPos != null;
		if (flag)
		{
			controlPos = getBezierControlPos(localFromPos, localToPos);
		}
		EffectBezierFlyCommandUGUI cmd = new EffectBezierFlyCommandUGUI();
		cmd.FromPosition = localFromPos;
		cmd.ToPosition = localToPos;
		cmd.ControlPosition = controlPos;
		cmd.EffectObject = this.GetEffectObject(effectKey);
		cmd.EffectObject.transform.SetParent(base.transform, false);
		cmd.EffectObject.transform.position = from.position;
		cmd.EffectObject.SetActive(true);
		bool flag2 = this._uiParticle;
		if (flag2)
		{
			this._uiParticle.RefreshParticles();
			float scale = 1f / this._uiParticle.scale;
			cmd.EffectObject.transform.localScale = new Vector3(scale, scale, scale);
		}
		cmd.Duration = 0.8f;
		base.StartCoroutine(this.PlayFlyEffectCoroutine(cmd));
		AudioManager.Instance.PlaySound("ui_art_score", false, false);
		return cmd;
	}

	// Token: 0x06000325 RID: 805 RVA: 0x00013528 File Offset: 0x00011728
	public GameObject PlayEffectAt(Transform target, string effectName, float duration, bool flip = false)
	{
		bool flag = !base.gameObject.activeInHierarchy;
		GameObject result;
		if (flag)
		{
			result = null;
		}
		else
		{
			GameObject effectObj = this.GetEffectObject(effectName);
			effectObj.name = effectName;
			effectObj.transform.SetParent(base.transform, false);
			effectObj.transform.position = target.position;
			int scaleXFactor = flip ? -1 : 1;
			effectObj.transform.localScale = Vector3.one.SetX((float)scaleXFactor);
			effectObj.SetActive(true);
			bool flag2 = this._uiParticle;
			if (flag2)
			{
				this._uiParticle.RefreshParticles();
				float scale = 1f / this._uiParticle.scale;
				effectObj.transform.localScale = new Vector3(scale * (float)scaleXFactor, scale, scale);
				this._uiParticle.Play();
			}
			bool flag3 = duration > 0f;
			if (flag3)
			{
				base.StartCoroutine(this.CollectEffectObjectCoroutine(effectObj, duration));
			}
			result = effectObj;
		}
		return result;
	}

	// Token: 0x06000326 RID: 806 RVA: 0x0001362C File Offset: 0x0001182C
	public GameObject GetSrcEffect(string effectName)
	{
		return this.GetEffectPool(effectName).prefab;
	}

	// Token: 0x040001CD RID: 461
	public GameObject[] EffectCore;

	// Token: 0x040001CE RID: 462
	private Dictionary<string, PoolItem> _effectPoolMap;

	// Token: 0x040001CF RID: 463
	private UIParticle _uiParticle;
}
