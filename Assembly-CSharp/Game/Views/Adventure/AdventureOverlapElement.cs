using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using GameData.Domains.Adventure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Adventure
{
	// Token: 0x02000C6C RID: 3180
	public class AdventureOverlapElement : MonoBehaviour
	{
		// Token: 0x0600A1E4 RID: 41444 RVA: 0x004BAFFC File Offset: 0x004B91FC
		public void RefreshDisplay(List<AdventureElement> notIgnoreElement)
		{
			this._totalCount = notIgnoreElement.Count;
			this.RefreshDisplay();
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.selfAssembly.HandleChild(delegate(GameObject refer, int index)
				{
					refer.gameObject.SetActive(true);
				});
				this.omittedRect.gameObject.SetActive(false);
				this.selfBg.SetSprite("adventure_remake_buff_base_4", false, null);
				int columnCount = (this._totalCount + this.selfLayout.constraintCount - 1) / this.selfLayout.constraintCount;
				this.self.SetWidth((float)columnCount * this._lineWidth);
			});
			this.pointerTrigger.ExitEvent.ResetListener(new Action(this.RefreshDisplay));
		}

		// Token: 0x0600A1E5 RID: 41445 RVA: 0x004BB058 File Offset: 0x004B9258
		private void RefreshDisplay()
		{
			this.selfBg.SetSprite("adventure_remake_smallbase_4", false, null);
			this.self.SetWidth(this._lineWidth);
			bool showOmitted = this._totalCount >= 4;
			this.selfAssembly.HandleChild(delegate(GameObject refer, int index)
			{
				refer.gameObject.SetActive(showOmitted ? (index <= 1) : (index < this._totalCount));
			});
			bool canExpand = this._totalCount >= 4;
			this.pointerTrigger.enabled = canExpand;
			this.omittedRect.gameObject.SetActive(showOmitted);
			bool showOmitted2 = showOmitted;
			if (showOmitted2)
			{
				this.omittedText.SetText((this._totalCount - 4 + 2).ToString(), true);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.self);
		}

		// Token: 0x04007DCA RID: 32202
		[SerializeField]
		private TemplatedContainerAssemblyNew selfAssembly;

		// Token: 0x04007DCB RID: 32203
		[SerializeField]
		private TextMeshProUGUI omittedText;

		// Token: 0x04007DCC RID: 32204
		[SerializeField]
		private RectTransform omittedRect;

		// Token: 0x04007DCD RID: 32205
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04007DCE RID: 32206
		[SerializeField]
		private GridLayoutGroup selfLayout;

		// Token: 0x04007DCF RID: 32207
		[SerializeField]
		private RectTransform self;

		// Token: 0x04007DD0 RID: 32208
		[SerializeField]
		private CImage selfBg;

		// Token: 0x04007DD1 RID: 32209
		private const int OmittedCount = 4;

		// Token: 0x04007DD2 RID: 32210
		private readonly float _lineWidth = 34f;

		// Token: 0x04007DD3 RID: 32211
		private int _totalCount;
	}
}
