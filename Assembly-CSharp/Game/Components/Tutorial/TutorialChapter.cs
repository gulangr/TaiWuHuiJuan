using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Components.Tutorial
{
	// Token: 0x02000C8B RID: 3211
	public class TutorialChapter : MonoBehaviour
	{
		// Token: 0x0600A3B2 RID: 41906 RVA: 0x004C9239 File Offset: 0x004C7439
		private void Awake()
		{
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.hover.SetActive(true);
				this.decorateHover.SetActive(true);
				this.enableObject.GetComponent<CImage>().sprite = this.hoverSwordSprite;
			});
			this.pointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.hover.SetActive(false);
				this.decorateHover.SetActive(false);
				this.enableObject.GetComponent<CImage>().sprite = this.normalSwordSprite;
			});
		}

		// Token: 0x0600A3B3 RID: 41907 RVA: 0x004C9278 File Offset: 0x004C7478
		private void OnEnable()
		{
			this.hover.SetActive(false);
			this.decorateHover.SetActive(false);
			this.enableObject.GetComponent<CImage>().sprite = this.normalSwordSprite;
			this.finishIcon.sprite = ((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? this.finishIconSprites[0] : this.finishIconSprites[1]);
		}

		// Token: 0x040081AE RID: 33198
		[SerializeField]
		public CButton chapterBtn;

		// Token: 0x040081AF RID: 33199
		[SerializeField]
		public GameObject enableObject;

		// Token: 0x040081B0 RID: 33200
		[SerializeField]
		public GameObject disableObject;

		// Token: 0x040081B1 RID: 33201
		[SerializeField]
		public TextMeshProUGUI chapterName;

		// Token: 0x040081B2 RID: 33202
		[SerializeField]
		public TextMeshProUGUI toggleName;

		// Token: 0x040081B3 RID: 33203
		[SerializeField]
		public CRawImage cardTexture;

		// Token: 0x040081B4 RID: 33204
		[SerializeField]
		public PointerTrigger pointerTrigger;

		// Token: 0x040081B5 RID: 33205
		[SerializeField]
		public GameObject hover;

		// Token: 0x040081B6 RID: 33206
		[SerializeField]
		public GameObject finishTip;

		// Token: 0x040081B7 RID: 33207
		[SerializeField]
		public GameObject decorateHover;

		// Token: 0x040081B8 RID: 33208
		[SerializeField]
		public Sprite normalSwordSprite;

		// Token: 0x040081B9 RID: 33209
		[SerializeField]
		public Sprite hoverSwordSprite;

		// Token: 0x040081BA RID: 33210
		[SerializeField]
		public CImage finishIcon;

		// Token: 0x040081BB RID: 33211
		[SerializeField]
		public Sprite[] finishIconSprites;
	}
}
