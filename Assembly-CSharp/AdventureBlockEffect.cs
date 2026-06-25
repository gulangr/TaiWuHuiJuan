using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000048 RID: 72
[ExecuteAlways]
public class AdventureBlockEffect : MonoBehaviour
{
	// Token: 0x1700004D RID: 77
	// (get) Token: 0x0600026B RID: 619 RVA: 0x0000E705 File Offset: 0x0000C905
	// (set) Token: 0x0600026C RID: 620 RVA: 0x0000E710 File Offset: 0x0000C910
	public AdventureBlockEffect.ViewType CurrentViewType
	{
		get
		{
			return this._viewType;
		}
		set
		{
			bool flag = this._viewType == value;
			if (!flag)
			{
				this._viewType = value;
				Color color = this.GetViewColor(value);
				this.ReplaceColor(base.GetComponent<RectTransform>(), color);
			}
		}
	}

	// Token: 0x0600026D RID: 621 RVA: 0x0000E74C File Offset: 0x0000C94C
	private Color GetViewColor(AdventureBlockEffect.ViewType viewType)
	{
		if (!true)
		{
		}
		Color result;
		if (viewType != AdventureBlockEffect.ViewType.Near)
		{
			if (viewType != AdventureBlockEffect.ViewType.Far)
			{
				result = this.colorOutView;
			}
			else
			{
				result = this.colorFar;
			}
		}
		else
		{
			result = this.colorNear;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x0600026E RID: 622 RVA: 0x0000E790 File Offset: 0x0000C990
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

	// Token: 0x0600026F RID: 623 RVA: 0x0000E7E0 File Offset: 0x0000C9E0
	private void ReplaceNodeColor(Transform node, Color color)
	{
		ParticleSystem particle = node.GetComponent<ParticleSystem>();
		Graphic graphic = node.GetComponent<Graphic>();
		bool flag = particle != null;
		if (flag)
		{
			bool flag2 = this.particleSystemColorOverrideList != null && this.particleSystemColorOverrideList.Contains(particle);
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

	// Token: 0x0400012B RID: 299
	private AdventureBlockEffect.ViewType _viewType = AdventureBlockEffect.ViewType.Near;

	// Token: 0x0400012C RID: 300
	public ParticleSystem[] particleSystemColorOverrideList;

	// Token: 0x0400012D RID: 301
	public Color colorNear = Color.white;

	// Token: 0x0400012E RID: 302
	public Color colorFar = new Color(0.8f, 0.8f, 0.8f);

	// Token: 0x0400012F RID: 303
	public Color colorOutView = new Color(0.5f, 0.5f, 0.5f);

	// Token: 0x04000130 RID: 304
	[SerializeField]
	private AdventureBlockEffect.ViewType EditorViewType = AdventureBlockEffect.ViewType.Near;

	// Token: 0x020010C4 RID: 4292
	public enum ViewType
	{
		// Token: 0x04009453 RID: 37971
		Near,
		// Token: 0x04009454 RID: 37972
		Far,
		// Token: 0x04009455 RID: 37973
		OutView
	}
}
