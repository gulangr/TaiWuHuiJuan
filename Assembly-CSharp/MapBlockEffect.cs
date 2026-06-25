using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000077 RID: 119
[ExecuteAlways]
public class MapBlockEffect : MonoBehaviour
{
	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000447 RID: 1095 RVA: 0x0001C386 File Offset: 0x0001A586
	// (set) Token: 0x06000448 RID: 1096 RVA: 0x0001C390 File Offset: 0x0001A590
	public MapBlockEffect.DisplayState CurrentState
	{
		get
		{
			return this.state;
		}
		set
		{
			bool flag = this.state == value;
			if (!flag)
			{
				this.state = value;
				if (!true)
				{
				}
				Color color2;
				switch (value)
				{
				case MapBlockEffect.DisplayState.Light:
					color2 = this.ColorLight;
					break;
				case MapBlockEffect.DisplayState.Flame:
					color2 = Colors.Instance["sectfulongflame"];
					break;
				case MapBlockEffect.DisplayState.Half:
					color2 = this.ColorHalf;
					break;
				default:
					color2 = this.ColorDark;
					break;
				}
				if (!true)
				{
				}
				Color color = color2;
				this.ReplaceColor(this.TopRoot, color);
				this.ReplaceColor(this.DownRoot, color);
			}
		}
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x0001C420 File Offset: 0x0001A620
	private void ReplaceColor(RectTransform node, Color color)
	{
		bool flag = node != null;
		if (flag)
		{
			this.ReplaceNodeColor(node, color);
			int i = 0;
			int len = node.childCount;
			while (i < len)
			{
				Transform child = node.GetChild(i);
				this.ReplaceNodeColor(child, color);
				i++;
			}
		}
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x0001C470 File Offset: 0x0001A670
	private void ReplaceNodeColor(Transform node, Color color)
	{
		ParticleSystem particle = node.GetComponent<ParticleSystem>();
		Graphic graphic = node.GetComponent<Graphic>();
		bool flag = particle != null;
		if (flag)
		{
			bool flag2 = this.ParticleSystemColorOverrideList != null && this.ParticleSystemColorOverrideList.Contains(particle);
			if (flag2)
			{
				ParticleSystem.ColorOverLifetimeModule colorOverLifeTime = particle.colorOverLifetime;
				colorOverLifeTime.color = color;
				bool flag3 = !colorOverLifeTime.enabled;
				if (flag3)
				{
					colorOverLifeTime.enabled = true;
				}
			}
		}
		else
		{
			bool flag4 = graphic != null;
			if (flag4)
			{
				graphic.color = color;
			}
		}
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x0001C500 File Offset: 0x0001A700
	private void SetNegativeFilmEffectValue(RectTransform node, float effectValue)
	{
		foreach (RawImage rawImage in node.GetComponentsInChildren<RawImage>())
		{
			rawImage.material = new Material(rawImage.material);
			rawImage.materialForRendering.SetFloat("_NegativeFilmEffect", effectValue);
		}
		foreach (SkeletonGraphic skeletonGraphic in node.GetComponentsInChildren<SkeletonGraphic>())
		{
			Color meshColor = skeletonGraphic.color;
			Color negativeEffectColor = CommonUtils.GetNegativeEffectColor(meshColor);
			Color finallyColor = Color.Lerp(Color.white, negativeEffectColor, effectValue);
			skeletonGraphic.material = new Material(skeletonGraphic.material);
			skeletonGraphic.materialForRendering.SetColor("_Color", finallyColor);
		}
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x0001C5BC File Offset: 0x0001A7BC
	public void SetNegativeFilmEffectValue(float effectValue)
	{
		bool flag = this.TopRoot != null;
		if (flag)
		{
			this.SetNegativeFilmEffectValue(this.TopRoot, effectValue);
		}
		bool flag2 = this.DownRoot != null;
		if (flag2)
		{
			this.SetNegativeFilmEffectValue(this.DownRoot, effectValue);
		}
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x0001C60C File Offset: 0x0001A80C
	public void ChangeVisibility(bool visibility)
	{
		bool flag = this.TopRoot != null && this.TopRoot.gameObject.activeSelf != visibility;
		if (flag)
		{
			this.TopRoot.gameObject.SetActive(visibility);
		}
		bool flag2 = this.DownRoot != null && this.DownRoot.gameObject.activeSelf != visibility;
		if (flag2)
		{
			this.DownRoot.gameObject.SetActive(visibility);
		}
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x0001C694 File Offset: 0x0001A894
	private void Start()
	{
		bool flag = this.TopRoot != null;
		if (flag)
		{
			foreach (ParticleSystem particle in this.TopRoot.GetComponentsInChildren<ParticleSystem>())
			{
				Transform particleTransform = particle.transform;
				this._particleSystemScaleHelper.Add(particle, new ValueTuple<Vector3, Vector3, RectTransform>(particleTransform.localScale, particleTransform.localPosition, this.TopRoot));
			}
		}
		bool flag2 = this.DownRoot != null;
		if (flag2)
		{
			foreach (ParticleSystem particle2 in this.DownRoot.GetComponentsInChildren<ParticleSystem>())
			{
				Transform particleTransform2 = particle2.transform;
				this._particleSystemScaleHelper.Add(particle2, new ValueTuple<Vector3, Vector3, RectTransform>(particleTransform2.localScale, particleTransform2.localPosition, this.DownRoot));
			}
		}
		bool flag3 = this.state == MapBlockEffect.DisplayState.Invalid;
		if (flag3)
		{
			this.CurrentState = MapBlockEffect.DisplayState.Dark;
		}
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x0001C788 File Offset: 0x0001A988
	private void Update()
	{
		foreach (KeyValuePair<ParticleSystem, ValueTuple<Vector3, Vector3, RectTransform>> particlePair in this._particleSystemScaleHelper)
		{
			Vector3 scale = particlePair.Value.Item1;
			Transform particleTransform = particlePair.Key.transform;
			particleTransform.localScale = scale * MapBlockEffect.LossyScale;
		}
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x0001C800 File Offset: 0x0001AA00
	private void OnDestroy()
	{
		bool flag = this.TopRoot != null;
		if (flag)
		{
			Object.Destroy(this.TopRoot.gameObject);
			this.TopRoot = null;
		}
		bool flag2 = this.DownRoot != null;
		if (flag2)
		{
			Object.Destroy(this.DownRoot.gameObject);
			this.DownRoot = null;
		}
	}

	// Token: 0x040002B1 RID: 689
	[NonSerialized]
	public string EffectName;

	// Token: 0x040002B2 RID: 690
	public ParticleSystem[] ParticleSystemColorOverrideList;

	// Token: 0x040002B3 RID: 691
	public RectTransform TopRoot;

	// Token: 0x040002B4 RID: 692
	public RectTransform DownRoot;

	// Token: 0x040002B5 RID: 693
	public Color ColorLight = Color.white;

	// Token: 0x040002B6 RID: 694
	public Color ColorHalf = Color.white.SetAlpha(0.75f);

	// Token: 0x040002B7 RID: 695
	public Color ColorDark = Color.gray;

	// Token: 0x040002B8 RID: 696
	[SerializeField]
	private MapBlockEffect.DisplayState state = MapBlockEffect.DisplayState.Invalid;

	// Token: 0x040002B9 RID: 697
	[TupleElementNames(new string[]
	{
		"initialScale",
		"initialPosition",
		"additional"
	})]
	private readonly IDictionary<ParticleSystem, ValueTuple<Vector3, Vector3, RectTransform>> _particleSystemScaleHelper = new Dictionary<ParticleSystem, ValueTuple<Vector3, Vector3, RectTransform>>();

	// Token: 0x040002BA RID: 698
	public static float LossyScale = 1f;

	// Token: 0x020010F0 RID: 4336
	public enum DisplayState
	{
		// Token: 0x040094D2 RID: 38098
		Invalid,
		// Token: 0x040094D3 RID: 38099
		Light,
		// Token: 0x040094D4 RID: 38100
		Flame,
		// Token: 0x040094D5 RID: 38101
		Half,
		// Token: 0x040094D6 RID: 38102
		Dark
	}
}
