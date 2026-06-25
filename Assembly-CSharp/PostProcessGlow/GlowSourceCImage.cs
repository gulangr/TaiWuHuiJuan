using System;
using UnityEngine;

namespace PostProcessGlow
{
	// Token: 0x02000600 RID: 1536
	[RequireComponent(typeof(CImage))]
	public class GlowSourceCImage : MonoBehaviour, IGlowSource
	{
		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x06004875 RID: 18549 RVA: 0x0021F1E8 File Offset: 0x0021D3E8
		// (set) Token: 0x06004876 RID: 18550 RVA: 0x0021F1F0 File Offset: 0x0021D3F0
		public CImage ImageComponent { get; private set; }

		// Token: 0x06004877 RID: 18551 RVA: 0x0021F1F9 File Offset: 0x0021D3F9
		private void Awake()
		{
			this.ImageComponent = base.GetComponent<CImage>();
		}

		// Token: 0x06004878 RID: 18552 RVA: 0x0021F209 File Offset: 0x0021D409
		private void OnEnable()
		{
			GlowTargetController instance = GlowTargetController.Instance;
			if (instance != null)
			{
				instance.Register(this);
			}
			this.ImageComponent.enabled = false;
		}

		// Token: 0x06004879 RID: 18553 RVA: 0x0021F22B File Offset: 0x0021D42B
		private void OnDisable()
		{
			GlowTargetController instance = GlowTargetController.Instance;
			if (instance != null)
			{
				instance.Unregister(this);
			}
		}

		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x0600487A RID: 18554 RVA: 0x0021F240 File Offset: 0x0021D440
		public Color GlowColor
		{
			get
			{
				return this.glowColor;
			}
		}

		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x0600487B RID: 18555 RVA: 0x0021F248 File Offset: 0x0021D448
		public Transform Transform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x0600487C RID: 18556 RVA: 0x0021F250 File Offset: 0x0021D450
		public IGlowCopy MakeCopy(Transform parent)
		{
			GameObject copyGo = Object.Instantiate<GameObject>(base.gameObject, parent);
			copyGo.name = base.gameObject.name + "_GlowCopy";
			copyGo.layer = parent.gameObject.layer;
			Component[] components = copyGo.GetComponents<Component>();
			foreach (Component component in components)
			{
				bool flag = component is RectTransform || component is CanvasRenderer || component is CImage || component is GlowCopyCImage;
				if (!flag)
				{
					Object.Destroy(component);
				}
			}
			GlowCopyCImage glowCopy = copyGo.AddComponent<GlowCopyCImage>();
			glowCopy.GetComponent<CImage>().enabled = true;
			return glowCopy;
		}

		// Token: 0x04003210 RID: 12816
		[Header("Glow Color")]
		[SerializeField]
		private Color glowColor = Color.white;
	}
}
