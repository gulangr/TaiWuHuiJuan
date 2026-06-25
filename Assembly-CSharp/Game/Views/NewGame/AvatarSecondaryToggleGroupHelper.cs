using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.NewGame
{
	// Token: 0x020007DE RID: 2014
	public class AvatarSecondaryToggleGroupHelper : MonoBehaviour
	{
		// Token: 0x06006236 RID: 25142 RVA: 0x002D1170 File Offset: 0x002CF370
		public void Init()
		{
			bool flag = this._toggleGroup == null;
			if (flag)
			{
				this._toggleGroup = base.GetComponent<CToggleGroup>();
				this._toggleGroup.Init(-1);
			}
		}

		// Token: 0x06006237 RID: 25143 RVA: 0x002D11AC File Offset: 0x002CF3AC
		public void Refresh(List<AvatarSecondaryToggleConfig> configs, bool isShow = true)
		{
			this._configs.Clear();
			this._configs.AddRange(configs);
			this._toggleGroup.Clear();
			CommonUtils.PrepareEnoughChildren(this.toggleContainer, this.toggleTemplate.gameObject, configs.Count, null);
			for (int i = 0; i < configs.Count; i++)
			{
				Transform toggleObj = this.toggleContainer.GetChild(i);
				CToggle toggle = toggleObj.GetComponent<CToggle>();
				AvatarSecondaryToggleConfig config = configs[i];
				this.SetToggleImages(toggle, config.ImageBase);
				if (isShow)
				{
					toggle.interactable = config.Interactable;
				}
				else
				{
					toggle.gameObject.SetActive(config.Interactable);
				}
				this._toggleGroup.Add(toggle);
			}
			bool flag = configs.Count > 0;
			if (flag)
			{
				this._toggleGroup.Init(0);
			}
		}

		// Token: 0x06006238 RID: 25144 RVA: 0x002D12A0 File Offset: 0x002CF4A0
		private void SetToggleImages(CToggle toggle, string imageBase)
		{
			bool flag = string.IsNullOrEmpty(imageBase);
			if (!flag)
			{
				CImage background = toggle.targetGraphic as CImage;
				bool flag2 = background != null;
				if (flag2)
				{
					background.SetSprite(imageBase + "_0", false, null);
				}
				ResLoader.Load<Sprite>("RemakeResources/UIGraphics5.0/Ui9NewGame/" + imageBase + "_1", delegate(Sprite hoverSprite)
				{
					ResLoader.Load<Sprite>("RemakeResources/UIGraphics5.0/Ui9NewGame/" + imageBase + "_4", delegate(Sprite selectedSprite)
					{
						ResLoader.Load<Sprite>("RemakeResources/UIGraphics5.0/Ui9NewGame/" + imageBase + "_3", delegate(Sprite disabledSprite)
						{
							toggle.transition = Selectable.Transition.SpriteSwap;
							toggle.spriteState = new SpriteState
							{
								highlightedSprite = hoverSprite,
								pressedSprite = selectedSprite,
								selectedSprite = selectedSprite,
								disabledSprite = disabledSprite
							};
							(toggle.graphic as Image).sprite = selectedSprite;
						}, null, false);
					}, null, false);
				}, null, false);
			}
		}

		// Token: 0x06006239 RID: 25145 RVA: 0x002D1330 File Offset: 0x002CF530
		public int GetActiveIndex()
		{
			return this._toggleGroup.GetActiveIndex();
		}

		// Token: 0x0600623A RID: 25146 RVA: 0x002D134D File Offset: 0x002CF54D
		public void SetActiveIndex(int index)
		{
			this._toggleGroup.Set(index, false);
		}

		// Token: 0x0600623B RID: 25147 RVA: 0x002D135E File Offset: 0x002CF55E
		public void AddOnActiveIndexChangeListener(Action<int, int> listener)
		{
			this._toggleGroup.OnActiveIndexChange += listener;
		}

		// Token: 0x04004467 RID: 17511
		[SerializeField]
		private Transform toggleContainer;

		// Token: 0x04004468 RID: 17512
		[SerializeField]
		private CToggle toggleTemplate;

		// Token: 0x04004469 RID: 17513
		private CToggleGroup _toggleGroup;

		// Token: 0x0400446A RID: 17514
		private readonly List<AvatarSecondaryToggleConfig> _configs = new List<AvatarSecondaryToggleConfig>();

		// Token: 0x0400446B RID: 17515
		private const string ImageFolderPath = "RemakeResources/UIGraphics5.0/Ui9NewGame";
	}
}
