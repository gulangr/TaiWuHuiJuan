using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200006A RID: 106
[RequireComponent(typeof(Image))]
[ExecuteAlways]
public class ImageAspectKeeper : MonoBehaviour
{
	// Token: 0x060003A7 RID: 935 RVA: 0x00016B36 File Offset: 0x00014D36
	private void Awake()
	{
		this._tickCount = Random.Range(0, 7);
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x00016B46 File Offset: 0x00014D46
	private void OnEnable()
	{
		this.UpdateSize();
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x00016B50 File Offset: 0x00014D50
	private void Update()
	{
		this._tickCount++;
		bool flag = this._tickCount >= 7;
		if (flag)
		{
			this._tickCount = 0;
			this.UpdateSize();
		}
	}

	// Token: 0x060003AA RID: 938 RVA: 0x00016B8C File Offset: 0x00014D8C
	public void Refresh()
	{
		this._tickCount = 0;
		this.UpdateSize();
	}

	// Token: 0x060003AB RID: 939 RVA: 0x00016BA0 File Offset: 0x00014DA0
	private void UpdateSize()
	{
		bool flag = this._image == null;
		if (flag)
		{
			this._image = base.GetComponent<Image>();
		}
		bool flag2 = this._rectTransform == null;
		if (flag2)
		{
			this._rectTransform = base.GetComponent<RectTransform>();
		}
		bool flag3 = this._image.sprite == null;
		if (!flag3)
		{
			float spriteW = this._image.sprite.rect.width;
			float spriteH = this._image.sprite.rect.height;
			bool flag4 = spriteH == 0f || spriteW == 0f;
			if (!flag4)
			{
				float aspect = spriteW / spriteH;
				Vector2 size = this._rectTransform.sizeDelta;
				bool flag5 = this.mode == ImageAspectKeeper.Mode.WidthControlsHeight;
				if (flag5)
				{
					size.y = size.x / aspect;
				}
				else
				{
					size.x = size.y * aspect;
				}
				this._rectTransform.sizeDelta = size;
			}
		}
	}

	// Token: 0x0400023A RID: 570
	public ImageAspectKeeper.Mode mode = ImageAspectKeeper.Mode.WidthControlsHeight;

	// Token: 0x0400023B RID: 571
	private Image _image;

	// Token: 0x0400023C RID: 572
	private RectTransform _rectTransform;

	// Token: 0x0400023D RID: 573
	private const int TickInterval = 7;

	// Token: 0x0400023E RID: 574
	private int _tickCount = 0;

	// Token: 0x020010DA RID: 4314
	public enum Mode
	{
		// Token: 0x04009496 RID: 38038
		WidthControlsHeight,
		// Token: 0x04009497 RID: 38039
		HeightControlsWidth
	}
}
