using System;
using UnityEngine;

// Token: 0x0200004C RID: 76
[ExecuteAlways]
public class CircularLayout : MonoBehaviour
{
	// Token: 0x06000285 RID: 645 RVA: 0x0000F5F2 File Offset: 0x0000D7F2
	private void Start()
	{
		this.ArrangeChildren();
	}

	// Token: 0x06000286 RID: 646 RVA: 0x0000F5FC File Offset: 0x0000D7FC
	private void Update()
	{
	}

	// Token: 0x06000287 RID: 647 RVA: 0x0000F600 File Offset: 0x0000D800
	public void ArrangeChildren()
	{
		int childCount = base.transform.childCount;
		bool flag = childCount == 0;
		if (!flag)
		{
			float endAngle = this.LayoutOnce(childCount, this.startAngle);
			bool flag2 = this.centered;
			if (flag2)
			{
				float newStartAngle = this.startAngle - (endAngle - this.startAngle) / 2f;
				this.LayoutOnce(childCount, newStartAngle);
			}
		}
	}

	// Token: 0x06000288 RID: 648 RVA: 0x0000F660 File Offset: 0x0000D860
	private float LayoutOnce(int childCount, float startAngle)
	{
		float currentAngle = startAngle;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			bool flag = !child.gameObject.activeSelf;
			if (!flag)
			{
				CircularLayoutElement layoutElement = child.GetComponent<CircularLayoutElement>();
				bool flag2 = layoutElement == null;
				if (flag2)
				{
					Debug.LogError("CircularLayoutElement component not found in child object: " + child.name);
				}
				else
				{
					bool ignoreLayout = layoutElement.ignoreLayout;
					if (!ignoreLayout)
					{
						RectTransform childRect = child.GetComponent<RectTransform>();
						float size = layoutElement.CalcSize();
						float angleForChild = this.CalcAngleForChild(size);
						bool flag3 = this.flipClockwise;
						if (flag3)
						{
							currentAngle -= angleForChild / 2f;
						}
						else
						{
							currentAngle += angleForChild / 2f;
						}
						Vector3 position = new Vector3(Mathf.Cos(currentAngle * 0.017453292f) * this.radius, Mathf.Sin(currentAngle * 0.017453292f) * this.radius, 0f);
						childRect.anchoredPosition = position;
						float rotation = (layoutElement == null || layoutElement.sizeMode == CircularLayoutElement.SizeMode.Width) ? (currentAngle - 90f) : currentAngle;
						bool flag4 = layoutElement != null && layoutElement.lockRotation;
						if (flag4)
						{
							rotation = layoutElement.lockedRotation.z;
						}
						childRect.localEulerAngles = new Vector3(0f, 0f, rotation);
						bool flag5 = this.flipClockwise;
						if (flag5)
						{
							currentAngle -= angleForChild / 2f + this.spacingAngle;
						}
						else
						{
							currentAngle += angleForChild / 2f + this.spacingAngle;
						}
					}
				}
			}
		}
		return currentAngle;
	}

	// Token: 0x06000289 RID: 649 RVA: 0x0000F804 File Offset: 0x0000DA04
	private float CalcAngleForChild(float size)
	{
		return 2f * Mathf.Asin(size / (2f * this.radius)) * 57.29578f;
	}

	// Token: 0x04000147 RID: 327
	[Header("布局参数")]
	public float startAngle = 0f;

	// Token: 0x04000148 RID: 328
	public float radius = 100f;

	// Token: 0x04000149 RID: 329
	public float spacingAngle = 10f;

	// Token: 0x0400014A RID: 330
	[Tooltip("如果设置为真，则计算的顺逆时针方向相反")]
	public bool flipClockwise = false;

	// Token: 0x0400014B RID: 331
	public bool centered = false;
}
