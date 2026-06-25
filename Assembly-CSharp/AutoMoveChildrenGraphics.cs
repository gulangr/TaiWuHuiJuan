using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000049 RID: 73
public class AutoMoveChildrenGraphics : MonoBehaviour
{
	// Token: 0x06000271 RID: 625 RVA: 0x0000E8D1 File Offset: 0x0000CAD1
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06000272 RID: 626 RVA: 0x0000E8DC File Offset: 0x0000CADC
	public void Init()
	{
		this._graphicsPools = new Dictionary<string, List<Graphic>>();
		this.Stop();
		this._prefabList.AddRange(base.transform.GetComponentsInTopChildren(true));
		foreach (Graphic graphic in this._prefabList)
		{
			string prefabKey = "automove_" + graphic.name + "_" + graphic.GetHashCode().ToString();
			graphic.rectTransform.anchorMin = new Vector2(0f, 0f);
			graphic.rectTransform.anchorMax = new Vector2(0f, 0f);
			graphic.rectTransform.pivot = new Vector2(0f, 0f);
			graphic.gameObject.SetActive(false);
			this._prefabKeyList.Add(prefabKey);
			this._graphicsPools.Add(prefabKey, new List<Graphic>
			{
				graphic
			});
		}
	}

	// Token: 0x06000273 RID: 627 RVA: 0x0000EA08 File Offset: 0x0000CC08
	public void Stop()
	{
		this._activeGraphics.ForEach(delegate(Graphic e)
		{
			bool flag = null != e;
			if (flag)
			{
				Object.Destroy(e.gameObject);
			}
		});
		this._activeGraphics.Clear();
		this.ClearAll();
		this._prefabKeyList.Clear();
		this._prefabList.Clear();
		base.StopAllCoroutines();
	}

	// Token: 0x06000274 RID: 628 RVA: 0x0000EA73 File Offset: 0x0000CC73
	private void OnEnable()
	{
		this.Emit();
	}

	// Token: 0x06000275 RID: 629 RVA: 0x0000EA7D File Offset: 0x0000CC7D
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000276 RID: 630 RVA: 0x0000EA88 File Offset: 0x0000CC88
	public void Emit()
	{
		bool flag = this._prefabKeyList.Count <= 0;
		if (!flag)
		{
			bool flag2 = this._activeGraphics.Count >= this.MaxCount;
			if (!flag2)
			{
				string prefabKey = this._prefabKeyList[Random.Range(0, this._prefabKeyList.Count)];
				Graphic graphic = this.GetPoolGraphic(prefabKey);
				bool flag3 = null == graphic;
				if (flag3)
				{
					Debug.LogWarning("no graphic available,key = " + prefabKey);
				}
				else
				{
					RectTransform graphicRectTrans = graphic.rectTransform;
					Vector2 graphicSize = graphicRectTrans.sizeDelta;
					Vector2 parentSize = base.GetComponent<RectTransform>().rect.size;
					float randScale = Random.Range(this.ScaleRange.x, this.ScaleRange.y);
					float randDirection = Random.Range(this.DirectionRange.x, this.DirectionRange.y);
					graphic.transform.SetParent(base.transform, false);
					graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, Random.Range(this.AlphaRange.x, this.AlphaRange.y));
					graphic.SetNativeSize();
					graphicRectTrans.localScale = new Vector3(randScale, randScale, graphicRectTrans.localScale.z);
					bool flag4 = randDirection < 45f || randDirection >= 315f;
					Vector2 startPos;
					Vector2 endPos;
					if (flag4)
					{
						startPos.x = -graphicSize.x;
						startPos.y = (parentSize.y - graphicSize.y) * Random.Range(this.StartYRange.x, this.StartYRange.y);
						endPos.x = parentSize.x;
						endPos.y = startPos.y + parentSize.x * Mathf.Tan(randDirection);
					}
					else
					{
						bool flag5 = 45f <= randDirection && randDirection < 135f;
						if (flag5)
						{
							startPos.x = (parentSize.x - graphicSize.x) * Random.Range(this.StartXRange.x, this.StartXRange.y);
							startPos.y = -graphicSize.y;
							endPos.x = startPos.x - parentSize.y * Mathf.Tan(randDirection - 90f);
							endPos.y = parentSize.y;
						}
						else
						{
							bool flag6 = 135f <= randDirection && randDirection < 225f;
							if (flag6)
							{
								startPos.x = parentSize.x;
								startPos.y = (parentSize.y - graphicSize.y) * Random.Range(this.StartYRange.x, this.StartYRange.y);
								endPos.x = -graphicSize.x;
								endPos.y = startPos.y - parentSize.x * Mathf.Tan(randDirection - 180f);
							}
							else
							{
								startPos.x = (parentSize.x - graphicSize.x) * Random.Range(this.StartXRange.x, this.StartXRange.y);
								startPos.y = parentSize.y;
								endPos.x = startPos.x + parentSize.x * Mathf.Tan(randDirection - 270f);
								endPos.y = -graphicSize.y;
							}
						}
					}
					graphicRectTrans.anchoredPosition = startPos;
					graphicRectTrans.DOAnchorPos(endPos, Random.Range(this.MoveTimeRange.x, this.MoveTimeRange.y), false).SetEase(Ease.Linear).OnComplete(delegate
					{
						bool flag7 = -1 != this.MaxCount && this._activeGraphics.Count >= this.MaxCount;
						if (flag7)
						{
							bool activeInHierarchy2 = this.gameObject.activeInHierarchy;
							if (activeInHierarchy2)
							{
								this.StartCoroutine(this.EmitNext());
							}
						}
						this.ReturnGraphic(prefabKey, graphic);
						this._activeGraphics.Remove(graphic);
					});
					graphic.gameObject.SetActive(true);
					this._activeGraphics.Add(graphic);
					bool activeInHierarchy = base.gameObject.activeInHierarchy;
					if (activeInHierarchy)
					{
						base.StartCoroutine(this.EmitNext());
					}
				}
			}
		}
	}

	// Token: 0x06000277 RID: 631 RVA: 0x0000EEE0 File Offset: 0x0000D0E0
	private IEnumerator EmitNext()
	{
		yield return new WaitForSeconds(Random.Range(this.EmitRateRange.x, this.EmitRateRange.y));
		this.Emit();
		yield break;
	}

	// Token: 0x06000278 RID: 632 RVA: 0x0000EEF0 File Offset: 0x0000D0F0
	private Graphic GetPoolGraphic(string key)
	{
		List<Graphic> graphicList;
		bool flag = this._graphicsPools.TryGetValue(key, out graphicList);
		Graphic result;
		if (flag)
		{
			bool flag2 = graphicList.Count > 1;
			if (flag2)
			{
				int lastIndex = graphicList.Count - 1;
				Graphic graphic = graphicList[lastIndex];
				graphicList.RemoveAt(lastIndex);
				result = graphic;
			}
			else
			{
				result = Object.Instantiate<Graphic>(graphicList[0]);
			}
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x06000279 RID: 633 RVA: 0x0000EF58 File Offset: 0x0000D158
	private void ReturnGraphic(string key, Graphic graphic)
	{
		List<Graphic> graphicList;
		bool flag = this._graphicsPools.TryGetValue(key, out graphicList);
		if (flag)
		{
			graphicList.Add(graphic);
		}
		else
		{
			Object.Destroy(graphic.gameObject);
		}
	}

	// Token: 0x0600027A RID: 634 RVA: 0x0000EF94 File Offset: 0x0000D194
	private void ClearAll()
	{
		foreach (KeyValuePair<string, List<Graphic>> pair in this._graphicsPools)
		{
			bool flag = pair.Value.Count > 1;
			if (flag)
			{
				for (int i = 1; i < pair.Value.Count; i++)
				{
					Object.Destroy(pair.Value[i].gameObject);
				}
			}
		}
		this._graphicsPools.Clear();
	}

	// Token: 0x04000131 RID: 305
	public Vector2 EmitRateRange = new Vector2(0f, 0f);

	// Token: 0x04000132 RID: 306
	[Tooltip("范围(0-1)")]
	public Vector2 AlphaRange = new Vector2(1f, 1f);

	// Token: 0x04000133 RID: 307
	[Tooltip("范围(0-1)")]
	public Vector2 ScaleRange = new Vector2(1f, 1f);

	// Token: 0x04000134 RID: 308
	[Tooltip("范围(0-360)")]
	public Vector2 DirectionRange = new Vector2(0f, 0f);

	// Token: 0x04000135 RID: 309
	[Tooltip("范围(0-1)")]
	public Vector2 StartXRange = new Vector2(0f, 1f);

	// Token: 0x04000136 RID: 310
	[Tooltip("范围(0-1)")]
	public Vector2 StartYRange = new Vector2(0f, 1f);

	// Token: 0x04000137 RID: 311
	public Vector2 MoveTimeRange = new Vector2(1f, 5f);

	// Token: 0x04000138 RID: 312
	public int MaxCount = -1;

	// Token: 0x04000139 RID: 313
	private List<Graphic> _prefabList = new List<Graphic>();

	// Token: 0x0400013A RID: 314
	private List<string> _prefabKeyList = new List<string>();

	// Token: 0x0400013B RID: 315
	private List<Graphic> _activeGraphics = new List<Graphic>();

	// Token: 0x0400013C RID: 316
	private Dictionary<string, List<Graphic>> _graphicsPools;
}
