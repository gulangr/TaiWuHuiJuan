using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using GameData.Adventure;
using GameData.Domains.Adventure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Adventure
{
	// Token: 0x02000C63 RID: 3171
	public class AdventureElementStateHolder : MonoBehaviour
	{
		// Token: 0x0600A18E RID: 41358 RVA: 0x004B7B7C File Offset: 0x004B5D7C
		public void RefreshDisplay(List<AdventureParameterData> stateParamList, AdventureElement elementFirst)
		{
			this._totalCount = stateParamList.Count;
			this.selfAssembly.Rebuild<RectTransform>(this._totalCount, delegate(RectTransform refer, int index)
			{
				AdventureTaiwuStateItem stateItem = refer.GetComponent<AdventureTaiwuStateItem>();
				AdventureParameterData parameterData = stateParamList[index];
				AdventureParameterValue parameterValue = elementFirst.GetParameter(parameterData.Key);
				stateItem.SetValue(parameterData, parameterValue);
			});
			this.RefreshDisplay();
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.bg.SetSprite("adventure_remake_buff_base_4", false, null);
				int rowCount = (this._totalCount + 5 - 1) / 5;
				this.selfAssembly.HandleChild(delegate(GameObject refer, int index)
				{
					refer.gameObject.SetActive(true);
				});
				this.countText.gameObject.SetActive(false);
				this.self.SetHeight((float)rowCount * this._lineHeight);
				this.selfLayout.spacing = new Vector2(0f, 0f);
				this.selfLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
				this.selfLayout.constraintCount = 5;
				this.selfLayout.padding = new RectOffset(3, 3, 3, 3);
			});
			this.pointerTrigger.ExitEvent.ResetListener(new Action(this.RefreshDisplay));
		}

		// Token: 0x0600A18F RID: 41359 RVA: 0x004B7C18 File Offset: 0x004B5E18
		private void RefreshDisplay()
		{
			this.bg.SetSprite("adventure_remake_buff_base_3", false, null);
			this.selfLayout.padding = new RectOffset(15, 15, 0, 0);
			bool canExpand = this._totalCount > 4;
			this.pointerTrigger.enabled = canExpand;
			this.selfAssembly.HandleChild(delegate(GameObject refer, int index)
			{
				refer.gameObject.SetActive(index < 10 && index < this._totalCount);
			});
			this.self.SetHeight(this._lineHeight);
			this.countText.SetText("+" + (this._totalCount - 10).ToString(), true);
			this.countText.gameObject.SetActive(this._totalCount > 10);
			this.selfLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
			this.selfLayout.constraintCount = 1;
			this.selfLayout.spacing = ((this._totalCount <= 6) ? new Vector2(0f, 0f) : new Vector2(-20f, 0f));
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.self);
		}

		// Token: 0x04007D52 RID: 32082
		[SerializeField]
		private RectTransform self;

		// Token: 0x04007D53 RID: 32083
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04007D54 RID: 32084
		[SerializeField]
		private TextMeshProUGUI countText;

		// Token: 0x04007D55 RID: 32085
		[SerializeField]
		private GridLayoutGroup selfLayout;

		// Token: 0x04007D56 RID: 32086
		[SerializeField]
		private CImage bg;

		// Token: 0x04007D57 RID: 32087
		[SerializeField]
		private TemplatedContainerAssemblyNew selfAssembly;

		// Token: 0x04007D58 RID: 32088
		public const int OmittedCount = 10;

		// Token: 0x04007D59 RID: 32089
		public const int ExpandCount = 4;

		// Token: 0x04007D5A RID: 32090
		private readonly float _lineHeight = 32f;

		// Token: 0x04007D5B RID: 32091
		private const int ExpandLineCount = 5;

		// Token: 0x04007D5C RID: 32092
		private int _totalCount;
	}
}
