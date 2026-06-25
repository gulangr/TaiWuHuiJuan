using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CA1 RID: 3233
	[RequireComponent(typeof(CToggle))]
	public class FilterToggle : MonoBehaviour
	{
		// Token: 0x1700112B RID: 4395
		// (get) Token: 0x0600A47A RID: 42106 RVA: 0x004CC8A6 File Offset: 0x004CAAA6
		private CToggle Toggle
		{
			get
			{
				return base.GetComponent<CToggle>();
			}
		}

		// Token: 0x1700112C RID: 4396
		// (get) Token: 0x0600A47B RID: 42107 RVA: 0x004CC8AE File Offset: 0x004CAAAE
		public bool IsSelected
		{
			get
			{
				return this.Toggle.isOn;
			}
		}

		// Token: 0x0600A47C RID: 42108 RVA: 0x004CC8BB File Offset: 0x004CAABB
		public void Refresh(FilterToggleConfig config)
		{
			this.Refresh(config.TipContent);
			this.RefreshToggleImages(config.ToggleImageBase);
		}

		// Token: 0x0600A47D RID: 42109 RVA: 0x004CC8D8 File Offset: 0x004CAAD8
		public void Refresh(StringKey tipContent)
		{
			this.tip.Type = TipType.SingleDesc;
			TooltipInvoker tooltipInvoker = this.tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			string tipContentString = tipContent.GetString();
			this.tip.RuntimeParam.Set("arg0", tipContentString);
		}

		// Token: 0x0600A47E RID: 42110 RVA: 0x004CC930 File Offset: 0x004CAB30
		private void RefreshToggleImages(string toggleImageBase)
		{
			bool flag = string.IsNullOrEmpty(toggleImageBase);
			if (!flag)
			{
				string folderPath = "RemakeResources/UIGraphics5.0/Ui9Common/";
				CImage background = this.Toggle.targetGraphic as CImage;
				bool flag2 = background != null;
				if (flag2)
				{
					background.SetSprite(toggleImageBase + "_0", false, null);
				}
				CImage checkmark = this.Toggle.graphic as CImage;
				bool flag3 = checkmark != null;
				if (flag3)
				{
					checkmark.SetSprite(toggleImageBase + "_4", false, null);
				}
				Action<Sprite> <>9__1;
				ResLoader.Load<Sprite>(folderPath + toggleImageBase + "_1", delegate(Sprite hoverSprite)
				{
					string assetPath = folderPath + toggleImageBase + "_4";
					Action<Sprite> onLoad;
					if ((onLoad = <>9__1) == null)
					{
						onLoad = (<>9__1 = delegate(Sprite selectedSprite)
						{
							ResLoader.Load<Sprite>(folderPath + toggleImageBase + "_3", delegate(Sprite disabledSprite)
							{
								this.Toggle.transition = Selectable.Transition.SpriteSwap;
								this.Toggle.spriteState = new SpriteState
								{
									highlightedSprite = null,
									pressedSprite = selectedSprite,
									selectedSprite = selectedSprite,
									disabledSprite = disabledSprite
								};
							}, null, false);
						});
					}
					ResLoader.Load<Sprite>(assetPath, onLoad, null, false);
				}, null, false);
			}
		}

		// Token: 0x0600A47F RID: 42111 RVA: 0x004CCA03 File Offset: 0x004CAC03
		public void SetInteractable(bool interactable)
		{
			this.Toggle.interactable = interactable;
		}

		// Token: 0x04008238 RID: 33336
		[SerializeField]
		private TooltipInvoker tip;
	}
}
