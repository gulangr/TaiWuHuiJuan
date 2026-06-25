using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200005B RID: 91
public class DisableStyleRoot : MonoBehaviour
{
	// Token: 0x060002F9 RID: 761 RVA: 0x00011C5C File Offset: 0x0000FE5C
	private Color GrayColor(Color src)
	{
		float gray = src.r * 0.3f + src.g * 0.59f + src.b * 0.11f;
		return new Color(gray, gray, gray, src.a);
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00011CA3 File Offset: 0x0000FEA3
	public void OnInteractableChanged(bool interactable)
	{
		this.SetStyleEffect(!interactable, false);
	}

	// Token: 0x060002FB RID: 763 RVA: 0x00011CB4 File Offset: 0x0000FEB4
	public void SetStyleEffect(bool isStyleUsed, bool forceApply = false)
	{
		bool flag = !forceApply && this._IsStyleEffectApplied == isStyleUsed;
		if (!flag)
		{
			this._IsStyleEffectApplied = isStyleUsed;
			Dictionary<Material, Material> cache = this._isUseUniqueCache ? this._InstanceReplacedMaterials : DisableStyleRoot._ReplacedMaterials;
			if (isStyleUsed)
			{
				IEnumerable<Graphic> graphics = base.GetComponentsInChildren<Graphic>(true);
				Rect boundingBox = default(Rect);
				using (IEnumerator<Graphic> enumerator = graphics.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Graphic graphic = enumerator.Current;
						bool flag2 = this._skipList != null && this._skipList.Contains(graphic);
						if (!flag2)
						{
							bool flag3 = graphic is TMP_SubMeshUI;
							if (!flag3)
							{
								TMP_Text text = graphic as TMP_Text;
								bool flag4 = text != null;
								if (flag4)
								{
									Color target = this.EffectTextColor;
									this._ReplacedColors[text] = text.color;
									text.color = target;
									text.overrideColorTags = true;
								}
								else
								{
									Material target2 = cache.FirstOrDefault((KeyValuePair<Material, Material> pair) => pair.Value == graphic.material).Key;
									bool flag5 = target2 != null;
									if (flag5)
									{
										graphic.material = target2;
									}
									else
									{
										bool flag6 = graphic.material.HasProperty(this._GraySwitchKey);
										if (flag6)
										{
											target2 = new Material(graphic.material)
											{
												name = "(GrayEffected)" + graphic.material.name
											};
											target2.SetInt(this._GraySwitchKey, 1);
										}
										else
										{
											target2 = this.DefaultGrayScaleMaterial;
										}
										bool flag7 = target2 != null;
										if (flag7)
										{
											cache[target2] = graphic.material;
											graphic.material = target2;
										}
									}
								}
								Rect rect = graphic.rectTransform.rect;
								bool flag8 = rect.xMin < boundingBox.xMin;
								if (flag8)
								{
									boundingBox.xMin = rect.xMin;
								}
								bool flag9 = rect.yMin < boundingBox.yMin;
								if (flag9)
								{
									boundingBox.yMin = rect.yMin;
								}
								bool flag10 = rect.xMax > boundingBox.xMax;
								if (flag10)
								{
									boundingBox.xMax = rect.xMax;
								}
								bool flag11 = rect.yMax > boundingBox.yMax;
								if (flag11)
								{
									boundingBox.yMax = rect.yMax;
								}
							}
						}
					}
				}
				bool isPlaying = Application.isPlaying;
				if (isPlaying)
				{
					bool flag12 = this._DisableMaskObject != null;
					if (flag12)
					{
						Object.Destroy(this._DisableMaskObject);
						this._DisableMaskObject = null;
					}
					GameObject disableMaskObject = new GameObject("_DisableMaskObject");
					disableMaskObject.transform.SetParent(base.transform);
					disableMaskObject.transform.localScale = Vector3.one;
					RectTransform rectTransform = disableMaskObject.AddComponent<RectTransform>();
					rectTransform.offsetMin = boundingBox.min;
					rectTransform.offsetMax = boundingBox.max;
					LayoutElement layoutElement = disableMaskObject.AddComponent<LayoutElement>();
					layoutElement.ignoreLayout = true;
					this._DisableMaskObject = disableMaskObject;
				}
				Action onDisableStyleEnter = this.OnDisableStyleEnter;
				if (onDisableStyleEnter != null)
				{
					onDisableStyleEnter();
				}
			}
			else
			{
				IEnumerable<Graphic> graphics2 = base.GetComponentsInChildren<Graphic>(true);
				foreach (Graphic graphic2 in graphics2)
				{
					bool flag13 = graphic2 is TMP_SubMeshUI;
					if (!flag13)
					{
						TMP_Text text2 = graphic2 as TMP_Text;
						bool flag14 = text2 != null;
						if (flag14)
						{
							Color source;
							bool flag15 = this._ReplacedColors.TryGetValue(text2, out source);
							if (flag15)
							{
								this._ReplacedColors.Remove(text2);
								text2.color = source;
								text2.overrideColorTags = false;
							}
						}
						else
						{
							bool flag16 = graphic2 != null;
							if (flag16)
							{
								Material source2;
								bool flag17 = cache.TryGetValue(graphic2.material, out source2) && source2 != null;
								if (flag17)
								{
									graphic2.material = source2;
								}
							}
						}
					}
				}
				bool flag18 = this._DisableMaskObject != null;
				if (flag18)
				{
					Object.Destroy(this._DisableMaskObject);
					this._DisableMaskObject = null;
				}
				Action onDisableStyleExit = this.OnDisableStyleExit;
				if (onDisableStyleExit != null)
				{
					onDisableStyleExit();
				}
			}
		}
	}

	// Token: 0x060002FC RID: 764 RVA: 0x00012188 File Offset: 0x00010388
	public void SetInteractable(bool interactable)
	{
		this.SetStyleEffect(!interactable, false);
	}

	// Token: 0x0400019D RID: 413
	public Material DefaultGrayScaleMaterial;

	// Token: 0x0400019E RID: 414
	public Color EffectTextColor = new Color(0.66f, 0.66f, 0.66f, 1f);

	// Token: 0x0400019F RID: 415
	[SerializeField]
	private bool _isUseUniqueCache;

	// Token: 0x040001A0 RID: 416
	public Action OnDisableStyleEnter;

	// Token: 0x040001A1 RID: 417
	public Action OnDisableStyleExit;

	// Token: 0x040001A2 RID: 418
	private static readonly Dictionary<Material, Material> _ReplacedMaterials = new Dictionary<Material, Material>();

	// Token: 0x040001A3 RID: 419
	private readonly Dictionary<Material, Material> _InstanceReplacedMaterials = new Dictionary<Material, Material>();

	// Token: 0x040001A4 RID: 420
	private readonly Dictionary<TMP_Text, Color> _ReplacedColors = new Dictionary<TMP_Text, Color>();

	// Token: 0x040001A5 RID: 421
	private readonly string _GraySwitchKey = "_GrayEffect";

	// Token: 0x040001A6 RID: 422
	private GameObject _DisableMaskObject = null;

	// Token: 0x040001A7 RID: 423
	private bool _IsStyleEffectApplied = false;

	// Token: 0x040001A8 RID: 424
	[SerializeField]
	public List<Graphic> _skipList;
}
