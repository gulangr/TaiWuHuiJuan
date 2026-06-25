using System;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F17 RID: 3863
	[RequireComponent(typeof(CRawImage))]
	public class CharacterAttainmentOverallItem : MonoBehaviour
	{
		// Token: 0x0600B1F7 RID: 45559 RVA: 0x00510BE4 File Offset: 0x0050EDE4
		private void Awake()
		{
			this.SetAlpha(0f);
			PolygonChecker polygonChecker = this._polygonChecker;
			polygonChecker.OnEnterPolygon = (Action)Delegate.Combine(polygonChecker.OnEnterPolygon, new Action(delegate()
			{
				Action onEnter = this.OnEnter;
				if (onEnter != null)
				{
					onEnter();
				}
				this.SetFade(true);
			}));
			PolygonChecker polygonChecker2 = this._polygonChecker;
			polygonChecker2.OnLeavePolygon = (Action)Delegate.Combine(polygonChecker2.OnLeavePolygon, new Action(delegate()
			{
				Action onExit = this.OnExit;
				if (onExit != null)
				{
					onExit();
				}
				this.SetFade(false);
			}));
		}

		// Token: 0x0600B1F8 RID: 45560 RVA: 0x00510C4C File Offset: 0x0050EE4C
		private void SetAlpha(float alpha)
		{
			for (int i = 0; i < this.lightImages.Length; i++)
			{
				Color c = this.lightImages[i].color;
				c.a = alpha;
				this.lightImages[i].color = c;
			}
		}

		// Token: 0x0600B1F9 RID: 45561 RVA: 0x00510C98 File Offset: 0x0050EE98
		private void OnEnable()
		{
			this.SetAlpha(0f);
		}

		// Token: 0x0600B1FA RID: 45562 RVA: 0x00510CA8 File Offset: 0x0050EEA8
		private void SetFade(bool toShow)
		{
			for (int i = 0; i < this.lightImages.Length; i++)
			{
				this.lightImages[i].DOKill(false);
			}
			this.SetAlpha(toShow ? 0f : 1f);
			for (int j = 0; j < this.lightImages.Length; j++)
			{
				this.lightImages[j].DOFade(toShow ? 1f : 0f, 0.4f);
			}
		}

		// Token: 0x040089F0 RID: 35312
		[SerializeField]
		private PolygonChecker _polygonChecker;

		// Token: 0x040089F1 RID: 35313
		[SerializeField]
		private CRawImage[] lightImages;

		// Token: 0x040089F2 RID: 35314
		public Action OnEnter;

		// Token: 0x040089F3 RID: 35315
		public Action OnExit;
	}
}
