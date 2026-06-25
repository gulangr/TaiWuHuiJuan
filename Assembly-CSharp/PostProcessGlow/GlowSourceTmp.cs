using System;
using TMPro;
using UnityEngine;

namespace PostProcessGlow
{
	// Token: 0x02000601 RID: 1537
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class GlowSourceTmp : MonoBehaviour, IGlowSource
	{
		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x0600487E RID: 18558 RVA: 0x0021F325 File Offset: 0x0021D525
		// (set) Token: 0x0600487F RID: 18559 RVA: 0x0021F32D File Offset: 0x0021D52D
		public TextMeshProUGUI TextComponent { get; private set; }

		// Token: 0x06004880 RID: 18560 RVA: 0x0021F336 File Offset: 0x0021D536
		private void Awake()
		{
			this.TextComponent = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x06004881 RID: 18561 RVA: 0x0021F346 File Offset: 0x0021D546
		private void OnEnable()
		{
			GlowTargetController instance = GlowTargetController.Instance;
			if (instance != null)
			{
				instance.Register(this);
			}
			this.TextComponent.enabled = false;
		}

		// Token: 0x06004882 RID: 18562 RVA: 0x0021F368 File Offset: 0x0021D568
		private void OnDisable()
		{
			GlowTargetController instance = GlowTargetController.Instance;
			if (instance != null)
			{
				instance.Unregister(this);
			}
		}

		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x06004883 RID: 18563 RVA: 0x0021F37D File Offset: 0x0021D57D
		public Color GlowColor
		{
			get
			{
				return this.glowColor;
			}
		}

		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x06004884 RID: 18564 RVA: 0x0021F385 File Offset: 0x0021D585
		public Transform Transform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x06004885 RID: 18565 RVA: 0x0021F390 File Offset: 0x0021D590
		public IGlowCopy MakeCopy(Transform parent)
		{
			GameObject copyGo = Object.Instantiate<GameObject>(base.gameObject, parent);
			copyGo.name = base.gameObject.name + "_GlowCopy";
			copyGo.layer = parent.gameObject.layer;
			Component[] components = copyGo.GetComponents<Component>();
			foreach (Component component in components)
			{
				bool flag = component is RectTransform || component is CanvasRenderer || component is TextMeshProUGUI || component is GlowCopyTmp;
				if (!flag)
				{
					Object.Destroy(component);
				}
			}
			GlowCopyTmp glowCopy = copyGo.AddComponent<GlowCopyTmp>();
			glowCopy.GetComponent<TextMeshProUGUI>().enabled = true;
			return glowCopy;
		}

		// Token: 0x04003212 RID: 12818
		[Header("Glow Color")]
		[SerializeField]
		private Color glowColor = Color.white;
	}
}
