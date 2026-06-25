using System;
using UnityEngine;

namespace PostProcessGlow
{
	// Token: 0x020005FC RID: 1532
	[RequireComponent(typeof(CImage))]
	public class GlowCopyCImage : MonoBehaviour, IGlowCopy
	{
		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x0600484B RID: 18507 RVA: 0x0021E445 File Offset: 0x0021C645
		public Transform Transform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x0600484C RID: 18508 RVA: 0x0021E44D File Offset: 0x0021C64D
		public GameObject GameObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x0600484D RID: 18509 RVA: 0x0021E455 File Offset: 0x0021C655
		// (set) Token: 0x0600484E RID: 18510 RVA: 0x0021E45D File Offset: 0x0021C65D
		public Color GlowColor { get; private set; }

		// Token: 0x0600484F RID: 18511 RVA: 0x0021E468 File Offset: 0x0021C668
		public void SetAsGlowColor()
		{
			CImage imageComponent = base.GetComponent<CImage>();
			imageComponent.color = this.GlowColor;
		}

		// Token: 0x06004850 RID: 18512 RVA: 0x0021E48C File Offset: 0x0021C68C
		public void SetAsOriginalColor()
		{
			CImage imageComponent = base.GetComponent<CImage>();
			imageComponent.color = this._originalColor;
		}

		// Token: 0x06004851 RID: 18513 RVA: 0x0021E4B0 File Offset: 0x0021C6B0
		public void Sync(IGlowSource source)
		{
			GlowSourceCImage sourceCImage = source as GlowSourceCImage;
			bool flag = sourceCImage == null;
			if (flag)
			{
				Debug.LogError("GlowCopyCImage can only sync with GlowSourceCImage.");
			}
			else
			{
				CImage imageComponent = base.GetComponent<CImage>();
				CImage sourceImageComponent = sourceCImage.ImageComponent;
				imageComponent.sprite = sourceImageComponent.sprite;
				imageComponent.type = sourceImageComponent.type;
				imageComponent.fillMethod = sourceImageComponent.fillMethod;
				imageComponent.fillOrigin = sourceImageComponent.fillOrigin;
				imageComponent.fillAmount = sourceImageComponent.fillAmount;
				imageComponent.preserveAspect = sourceImageComponent.preserveAspect;
				imageComponent.fillClockwise = sourceImageComponent.fillClockwise;
				imageComponent.pixelsPerUnitMultiplier = sourceImageComponent.pixelsPerUnitMultiplier;
				imageComponent.material = sourceImageComponent.material;
				imageComponent.raycastTarget = sourceImageComponent.raycastTarget;
				imageComponent.maskable = sourceImageComponent.maskable;
				imageComponent.color = sourceImageComponent.color;
				this._originalColor = sourceImageComponent.color;
				this.GlowColor = sourceCImage.GlowColor;
			}
		}

		// Token: 0x040031F1 RID: 12785
		private Color _originalColor;
	}
}
