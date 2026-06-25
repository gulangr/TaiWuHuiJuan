using System;
using UnityEngine;

// Token: 0x0200032E RID: 814
[ExecuteInEditMode]
public class CommonPlaceHolder2 : MonoBehaviour
{
	// Token: 0x06002F40 RID: 12096 RVA: 0x00173240 File Offset: 0x00171440
	private void Awake()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
	}

	// Token: 0x06002F41 RID: 12097 RVA: 0x00173250 File Offset: 0x00171450
	private void Update()
	{
		bool flag = this.rectTransform == null;
		if (flag)
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}
		Vector2 currentSize = this.rectTransform.sizeDelta;
		bool flag2 = currentSize != this.lastSize;
		if (flag2)
		{
			this.UpdateVisibility(currentSize);
			this.lastSize = currentSize;
		}
	}

	// Token: 0x06002F42 RID: 12098 RVA: 0x001732A8 File Offset: 0x001714A8
	private void UpdateVisibility(Vector2 size)
	{
		bool flag = this.mainContent == null;
		if (!flag)
		{
			float width = size.x;
			float height = size.y;
			float minSize = Mathf.Min(width, height);
			bool flag2 = minSize < this.hideThreshold;
			if (flag2)
			{
				this.mainContent.SetActive(false);
			}
			else
			{
				this.mainContent.SetActive(true);
				bool showCorners = minSize >= this.cornerThreshold;
				bool flag3 = this.cornerImages != null;
				if (flag3)
				{
					for (int i = 0; i < this.cornerImages.Length; i++)
					{
						bool flag4 = this.cornerImages[i] != null;
						if (flag4)
						{
							this.cornerImages[i].gameObject.SetActive(showCorners);
						}
					}
				}
			}
		}
	}

	// Token: 0x04002259 RID: 8793
	[SerializeField]
	private float hideThreshold = 56f;

	// Token: 0x0400225A RID: 8794
	[SerializeField]
	private float cornerThreshold = 116f;

	// Token: 0x0400225B RID: 8795
	[SerializeField]
	private GameObject mainContent;

	// Token: 0x0400225C RID: 8796
	[SerializeField]
	private CImage[] cornerImages;

	// Token: 0x0400225D RID: 8797
	[SerializeField]
	private CImage centerImage;

	// Token: 0x0400225E RID: 8798
	private RectTransform rectTransform;

	// Token: 0x0400225F RID: 8799
	private Vector2 lastSize;
}
