using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000068 RID: 104
[ExecuteInEditMode]
public class HSVStyleRoot : MonoBehaviour
{
	// Token: 0x17000063 RID: 99
	// (get) Token: 0x0600037F RID: 895 RVA: 0x00015C08 File Offset: 0x00013E08
	private float HueShift
	{
		get
		{
			return base.enabled ? this.hueShift : 0f;
		}
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x06000380 RID: 896 RVA: 0x00015C1F File Offset: 0x00013E1F
	private float SaturationFactor
	{
		get
		{
			return base.enabled ? this.saturationFactor : 1f;
		}
	}

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x06000381 RID: 897 RVA: 0x00015C36 File Offset: 0x00013E36
	private float ValueFactor
	{
		get
		{
			return base.enabled ? this.valueFactor : 1f;
		}
	}

	// Token: 0x06000382 RID: 898 RVA: 0x00015C50 File Offset: 0x00013E50
	private void Awake()
	{
		bool flag = this.skipList.Count > 0;
		if (flag)
		{
			this._skipSet = new HashSet<MaskableGraphic>(this.skipList);
		}
		this.RefreshAutoSkip();
	}

	// Token: 0x06000383 RID: 899 RVA: 0x00015C8C File Offset: 0x00013E8C
	public void RefreshAutoSkip()
	{
		bool flag = this.autoSkipList.Count > 0;
		if (flag)
		{
			foreach (Transform trans in this.autoSkipList)
			{
				MaskableGraphic[] graphics = trans.GetComponentsInChildren<MaskableGraphic>(true);
				foreach (MaskableGraphic graphic in graphics)
				{
					if (this._skipSet == null)
					{
						this._skipSet = new HashSet<MaskableGraphic>();
					}
					this._skipSet.Add(graphic);
				}
			}
		}
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00015D3C File Offset: 0x00013F3C
	public void ResetAll()
	{
		this.ClearColorBackups();
		this._clonedMaterials.Clear();
		this._originalMaterials.Clear();
	}

	// Token: 0x06000385 RID: 901 RVA: 0x00015D5E File Offset: 0x00013F5E
	public void ClearColorBackups()
	{
		this._colorBackups.Clear();
	}

	// Token: 0x06000386 RID: 902 RVA: 0x00015D70 File Offset: 0x00013F70
	public void ApplyValueToChildren()
	{
		this._refreshedMaskSet.Clear();
		foreach (MaskableGraphic graphic in base.GetComponentsInChildren<MaskableGraphic>(true))
		{
			this.ApplyValueToOneGraphic(graphic);
		}
		MaskableGraphic selfGraphic;
		bool flag = base.TryGetComponent<MaskableGraphic>(out selfGraphic);
		if (flag)
		{
			this.ApplyValueToOneGraphic(selfGraphic);
		}
	}

	// Token: 0x06000387 RID: 903 RVA: 0x00015DC8 File Offset: 0x00013FC8
	private void ApplyValueToOneGraphic(MaskableGraphic graphic)
	{
		HashSet<MaskableGraphic> skipSet = this._skipSet;
		bool flag = skipSet != null && skipSet.Contains(graphic);
		if (!flag)
		{
			bool flag2 = graphic is TMP_SubMeshUI;
			if (!flag2)
			{
				bool flag3 = !(graphic is TextMeshProUGUI);
				if (flag3)
				{
					bool isEffective = base.enabled && (!Mathf.Approximately(this.valueFactor, 1f) || !Mathf.Approximately(this.saturationFactor, 1f) || !Mathf.Approximately(this.hueShift, 0f));
					bool flag4 = !isEffective;
					if (flag4)
					{
						Material originalMat;
						bool flag5 = this._originalMaterials.TryGetValue(graphic, out originalMat);
						if (flag5)
						{
							graphic.material = originalMat;
						}
						else
						{
							graphic.material = null;
						}
						this._clonedMaterials.Remove(graphic);
						return;
					}
					bool flag6 = !this._originalMaterials.ContainsKey(graphic) && !HSVStyleRoot.HasHSVProperty(graphic.material);
					if (flag6)
					{
						this._originalMaterials[graphic] = graphic.material;
					}
					Material originalMat2;
					this._originalMaterials.TryGetValue(graphic, out originalMat2);
					bool flag7 = originalMat2 == null || originalMat2.shader.name == "UI/Default";
					if (flag7)
					{
						bool flag8 = this.defaultHSVMaterial != null && HSVStyleRoot.HasHSVProperty(this.defaultHSVMaterial);
						if (flag8)
						{
							Material material;
							bool flag9 = !this._clonedMaterials.TryGetValue(graphic, out material);
							if (flag9)
							{
								material = new Material(this.defaultHSVMaterial);
								graphic.material = material;
								this._clonedMaterials[graphic] = material;
							}
							this.SetHsv(material);
						}
						else
						{
							Debug.LogWarning("DefaultHSVMaterial is not assigned or missing required properties, skipping " + graphic.gameObject.name + ".");
						}
					}
					else
					{
						bool flag10 = HSVStyleRoot.HasHSVProperty(originalMat2);
						if (flag10)
						{
							Material material2;
							bool flag11 = !this._clonedMaterials.TryGetValue(graphic, out material2);
							if (flag11)
							{
								material2 = new Material(originalMat2);
								graphic.material = material2;
								this._clonedMaterials[graphic] = material2;
							}
							this.SetHsv(material2);
							graphic.SetMaterialDirty();
						}
						else
						{
							Debug.LogWarning("Material on " + graphic.gameObject.name + " does not support HSV value control and will be skipped.");
						}
					}
				}
				else
				{
					TextMeshProUGUI text = graphic as TextMeshProUGUI;
					bool flag12 = text != null;
					if (flag12)
					{
						bool flag13 = !this._colorBackups.ContainsKey(graphic);
						if (flag13)
						{
							this._colorBackups[graphic] = graphic.color;
						}
						bool richText = text.richText;
						if (richText)
						{
							string rawText = text.text;
							Match match = Regex.Match(rawText, "<color=#[0-9a-fA-F]{6,8}>");
							bool success = match.Success;
							if (success)
							{
								string value = match.Value;
								string hex = value.Substring(7, value.Length - 7).TrimEnd('>');
								Color parsedColor;
								bool flag14 = ColorUtility.TryParseHtmlString(hex, out parsedColor);
								if (flag14)
								{
									text.color = parsedColor;
									text.text = Regex.Replace(rawText, "<color=#[0-9a-fA-F]{6,8}>", "", RegexOptions.IgnoreCase);
									text.text = text.text.Replace("</color>", "");
								}
							}
						}
						bool flag15 = Mathf.Approximately(this.valueFactor, 1f) && Mathf.Approximately(this.saturationFactor, 1f) && Mathf.Approximately(this.hueShift, 0f);
						if (flag15)
						{
							graphic.color = this._colorBackups[graphic];
						}
						else
						{
							Color original = this._colorBackups[graphic];
							float h;
							float s;
							float v;
							Color.RGBToHSV(original, out h, out s, out v);
							h = Mathf.Repeat(h + this.HueShift, 1f);
							s *= this.SaturationFactor;
							v *= this.ValueFactor;
							graphic.color = Color.HSVToRGB(h, Mathf.Clamp01(s), Mathf.Clamp01(v));
						}
					}
				}
				MaskableGraphic mask = this.FindMaskInParents(graphic.transform);
				bool flag16 = mask && !this._refreshedMaskSet.Contains(mask);
				if (flag16)
				{
					bool enabled = mask.enabled;
					if (enabled)
					{
						mask.enabled = false;
						mask.enabled = true;
					}
					this._refreshedMaskSet.Add(mask);
				}
			}
		}
	}

	// Token: 0x06000388 RID: 904 RVA: 0x00016236 File Offset: 0x00014436
	private void SetHsv(Material material)
	{
		material.SetFloat(HSVStyleRoot.ValueFactorId, this.ValueFactor);
		material.SetFloat(HSVStyleRoot.SaturationFactorId, this.SaturationFactor);
		material.SetFloat(HSVStyleRoot.HueShiftId, this.HueShift);
	}

	// Token: 0x06000389 RID: 905 RVA: 0x00016270 File Offset: 0x00014470
	private static bool HasHSVProperty(Material mat)
	{
		return mat.HasProperty(HSVStyleRoot.ValueFactorId) && mat.HasProperty(HSVStyleRoot.SaturationFactorId) && mat.HasProperty(HSVStyleRoot.HueShiftId);
	}

	// Token: 0x0600038A RID: 906 RVA: 0x000162AA File Offset: 0x000144AA
	public void SetHueShift(float shift)
	{
		this.hueShift = Mathf.Clamp(shift, -1f, 1f);
		this.ApplyValueToChildren();
	}

	// Token: 0x0600038B RID: 907 RVA: 0x000162CA File Offset: 0x000144CA
	public void SetSaturationFactor(float value)
	{
		this.saturationFactor = Mathf.Clamp(value, 0f, 2f);
		this.ApplyValueToChildren();
	}

	// Token: 0x0600038C RID: 908 RVA: 0x000162EA File Offset: 0x000144EA
	public void SetValueFactor(float value)
	{
		this.valueFactor = Mathf.Clamp(value, 0f, 2f);
		this.ApplyValueToChildren();
	}

	// Token: 0x0600038D RID: 909 RVA: 0x0001630C File Offset: 0x0001450C
	public float GetValueFactor()
	{
		return this.valueFactor;
	}

	// Token: 0x0600038E RID: 910 RVA: 0x00016324 File Offset: 0x00014524
	private MaskableGraphic FindMaskInParents(Transform child)
	{
		Transform current = child;
		while (current != null)
		{
			MaskableGraphic mask = current.GetComponent<MaskableGraphic>();
			bool flag = mask != null;
			MaskableGraphic result;
			if (flag)
			{
				result = mask;
			}
			else
			{
				bool flag2 = current == base.transform;
				if (!flag2)
				{
					current = current.parent;
					continue;
				}
				result = null;
			}
			return result;
		}
		return null;
	}

	// Token: 0x0600038F RID: 911 RVA: 0x0001637D File Offset: 0x0001457D
	public void OnToggleValueChanged(bool isOn)
	{
		this._isOn = isOn;
		this.RefreshValue();
	}

	// Token: 0x06000390 RID: 912 RVA: 0x0001638E File Offset: 0x0001458E
	public void OnInteractableChanged(bool interactable)
	{
		this._interactable = interactable;
		this.RefreshValue();
	}

	// Token: 0x06000391 RID: 913 RVA: 0x0001639F File Offset: 0x0001459F
	public void OnHoverChanged(bool isHover)
	{
		this._isHover = isHover;
		this.RefreshValue();
	}

	// Token: 0x06000392 RID: 914 RVA: 0x000163B0 File Offset: 0x000145B0
	private void RefreshValue()
	{
		bool interactable = this._interactable;
		if (interactable)
		{
			bool isOn = this._isOn;
			if (isOn)
			{
				this.SetDefault();
			}
			else
			{
				bool isHover = this._isHover;
				if (isHover)
				{
					this.SetDefault();
				}
				else
				{
					this.SetDefaultBlack();
				}
			}
		}
		else
		{
			this.SetDefaultGrayAndBlack();
		}
	}

	// Token: 0x06000393 RID: 915 RVA: 0x00016404 File Offset: 0x00014604
	public void SetInteractable(bool interactable)
	{
		if (interactable)
		{
			this.SetDefault();
		}
		else
		{
			this.SetDefaultGrayAndBlack();
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x00016427 File Offset: 0x00014627
	public void SetDefault()
	{
		this.hueShift = 0f;
		this.saturationFactor = 1f;
		this.valueFactor = 1f;
		this.ApplyValueToChildren();
	}

	// Token: 0x06000395 RID: 917 RVA: 0x00016454 File Offset: 0x00014654
	public void SetDefaultBlack()
	{
		bool ignoreDefaultBlack = this.IgnoreDefaultBlack;
		if (ignoreDefaultBlack)
		{
			this.SetDefault();
		}
		else
		{
			this.hueShift = 0f;
			this.saturationFactor = 1f;
			this.valueFactor = 0.5f;
			this.ApplyValueToChildren();
		}
	}

	// Token: 0x06000396 RID: 918 RVA: 0x0001649E File Offset: 0x0001469E
	public void SetDefaultGrayAndBlack()
	{
		this.hueShift = 0f;
		this.saturationFactor = 0f;
		this.valueFactor = 0.5f;
		this.ApplyValueToChildren();
	}

	// Token: 0x0400021A RID: 538
	private static readonly int HueShiftId = Shader.PropertyToID("_HueShift");

	// Token: 0x0400021B RID: 539
	private static readonly int SaturationFactorId = Shader.PropertyToID("_SaturationFactor");

	// Token: 0x0400021C RID: 540
	private static readonly int ValueFactorId = Shader.PropertyToID("_ValueFactor");

	// Token: 0x0400021D RID: 541
	[SerializeField]
	[Range(-1f, 1f)]
	private float hueShift;

	// Token: 0x0400021E RID: 542
	[SerializeField]
	[Range(0f, 2f)]
	[Header("置灰")]
	[Tooltip("一般用0")]
	private float saturationFactor = 1f;

	// Token: 0x0400021F RID: 543
	[SerializeField]
	[Range(0f, 2f)]
	[Header("置黑")]
	[Tooltip("一般用0.5")]
	private float valueFactor = 1f;

	// Token: 0x04000220 RID: 544
	public Material defaultHSVMaterial;

	// Token: 0x04000221 RID: 545
	public bool IgnoreDefaultBlack;

	// Token: 0x04000222 RID: 546
	public List<MaskableGraphic> skipList = new List<MaskableGraphic>();

	// Token: 0x04000223 RID: 547
	[Tooltip("选中节点下所有Graphic都会被跳过")]
	public List<Transform> autoSkipList = new List<Transform>();

	// Token: 0x04000224 RID: 548
	private readonly Dictionary<MaskableGraphic, Color> _colorBackups = new Dictionary<MaskableGraphic, Color>();

	// Token: 0x04000225 RID: 549
	private readonly Dictionary<MaskableGraphic, Material> _clonedMaterials = new Dictionary<MaskableGraphic, Material>();

	// Token: 0x04000226 RID: 550
	private readonly Dictionary<MaskableGraphic, Material> _originalMaterials = new Dictionary<MaskableGraphic, Material>();

	// Token: 0x04000227 RID: 551
	private HashSet<MaskableGraphic> _skipSet;

	// Token: 0x04000228 RID: 552
	private readonly HashSet<MaskableGraphic> _refreshedMaskSet = new HashSet<MaskableGraphic>();

	// Token: 0x04000229 RID: 553
	private bool _isOn;

	// Token: 0x0400022A RID: 554
	private bool _interactable;

	// Token: 0x0400022B RID: 555
	private bool _isHover;
}
