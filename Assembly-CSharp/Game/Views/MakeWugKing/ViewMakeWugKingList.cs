using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.MakeWugKing
{
	// Token: 0x0200094A RID: 2378
	public class ViewMakeWugKingList : UIBase
	{
		// Token: 0x06007071 RID: 28785 RVA: 0x00341313 File Offset: 0x0033F513
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x06007072 RID: 28786 RVA: 0x00341320 File Offset: 0x0033F520
		private void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._inited = true;
				this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
			}
		}

		// Token: 0x06007073 RID: 28787 RVA: 0x0034135C File Offset: 0x0033F55C
		private void RenderElements()
		{
			int rowAmount = (WugKing.Instance.Count + this.columnAmount - 1) / this.columnAmount;
			CommonUtils.PrepareEnoughChildren(this.content, this.lineLayout.gameObject, rowAmount, null);
			int currentLineColAmount = this.columnAmount;
			int cellIndex = 0;
			for (int row = 0; row < this.content.childCount; row++)
			{
				Transform currentLine = this.content.GetChild(row);
				bool flag = row == this.content.childCount - 1;
				if (flag)
				{
					currentLineColAmount = WugKing.Instance.Count - row * this.columnAmount;
				}
				CommonUtils.PrepareEnoughChildren(currentLine, this.cellTemplate.gameObject, currentLineColAmount, null);
				this._cellDic[row] = new List<WugKingListItem>();
				for (int i = 0; i < currentLineColAmount; i++)
				{
					WugKingListItem item = currentLine.GetChild(i).GetComponent<WugKingListItem>();
					item.OnItemRender(cellIndex);
					cellIndex++;
					this._cellDic[row].Add(item);
				}
				currentLine.gameObject.SetActive(true);
			}
		}

		// Token: 0x06007074 RID: 28788 RVA: 0x0034149C File Offset: 0x0033F69C
		private void RecalculateLayout()
		{
			foreach (KeyValuePair<int, List<WugKingListItem>> linePair in this._cellDic)
			{
				this.RecalculateLineLayout(linePair.Value);
			}
		}

		// Token: 0x06007075 RID: 28789 RVA: 0x003414FC File Offset: 0x0033F6FC
		private void RecalculateLineLayout(List<WugKingListItem> value)
		{
			float maxTopHeight = 0f;
			float maxBottomHeight = 0f;
			foreach (WugKingListItem item in value)
			{
				maxTopHeight = Math.Max(maxTopHeight, item.GetTopHeight());
				maxBottomHeight = Math.Max(maxBottomHeight, item.GetBottomHeight());
			}
			foreach (WugKingListItem item2 in value)
			{
				item2.SetPlaceHolder(maxTopHeight, maxBottomHeight);
			}
		}

		// Token: 0x06007076 RID: 28790 RVA: 0x003415B4 File Offset: 0x0033F7B4
		public override void OnInit(ArgumentBox argsBox)
		{
			this.Init();
			this.RenderElements();
		}

		// Token: 0x06007077 RID: 28791 RVA: 0x003415C5 File Offset: 0x0033F7C5
		private void OnEnable()
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				for (int row = 0; row < this.content.childCount; row++)
				{
					this.content.GetChild(row).gameObject.SetActive(true);
				}
				this.RecalculateLayout();
			});
		}

		// Token: 0x0400535C RID: 21340
		[SerializeField]
		private RectTransform content;

		// Token: 0x0400535D RID: 21341
		[SerializeField]
		private RectTransform lineLayout;

		// Token: 0x0400535E RID: 21342
		[SerializeField]
		private WugKingListItem cellTemplate;

		// Token: 0x0400535F RID: 21343
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04005360 RID: 21344
		private bool _inited;

		// Token: 0x04005361 RID: 21345
		public int columnAmount = 3;

		// Token: 0x04005362 RID: 21346
		private Dictionary<int, List<WugKingListItem>> _cellDic = new Dictionary<int, List<WugKingListItem>>();
	}
}
