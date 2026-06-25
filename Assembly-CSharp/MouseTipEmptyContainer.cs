using System;
using System.Collections.Generic;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000295 RID: 661
public class MouseTipEmptyContainer : MouseTipBase
{
	// Token: 0x17000493 RID: 1171
	// (get) Token: 0x060029FA RID: 10746 RVA: 0x0013EAC4 File Offset: 0x0013CCC4
	protected override bool CanStick
	{
		get
		{
			return this._canStick;
		}
	}

	// Token: 0x060029FB RID: 10747 RVA: 0x0013EACC File Offset: 0x0013CCCC
	protected override void Init(ArgumentBox argsBox)
	{
		Func<List<RectTransform>> handler;
		bool flag = !argsBox.Get<Func<List<RectTransform>>>("GetCellListHandler", out handler);
		if (flag)
		{
			throw new Exception("MouseTipEmptyContainer: GetCellListHandler is null");
		}
		argsBox.Get("CanStick", out this._canStick);
		string title;
		argsBox.Get("Title", out title);
		base.CGet<TextMeshProUGUI>("Title").text = title;
		argsBox.Get<Action<List<RectTransform>>>("CollectCellsHandler", out this._collectHandler);
		this.TryCollect();
		this._layoutCells = handler();
		bool flag2 = !argsBox.Get("LineCount", out this._lineCount);
		if (flag2)
		{
			this._lineCount = 1;
		}
		bool flag3 = !argsBox.Get("PaddingX", out this._paddingX);
		if (flag3)
		{
			this._paddingX = 5f;
		}
		bool flag4 = !argsBox.Get("PaddingY", out this._paddingY);
		if (flag4)
		{
			this._paddingY = 5f;
		}
		bool flag5 = !argsBox.Get("SpacingX", out this._spacingX);
		if (flag5)
		{
			this._spacingX = 5f;
		}
		bool flag6 = !argsBox.Get("SpacingY", out this._spacingY);
		if (flag6)
		{
			this._spacingY = 5f;
		}
		this.LayoutCellsAndAdjustSize();
	}

	// Token: 0x060029FC RID: 10748 RVA: 0x0013EC0A File Offset: 0x0013CE0A
	protected override void OnDisable()
	{
		base.OnDisable();
		this.TryCollect();
	}

	// Token: 0x060029FD RID: 10749 RVA: 0x0013EC1C File Offset: 0x0013CE1C
	private void TryCollect()
	{
		List<RectTransform> layoutCells = this._layoutCells;
		bool flag = layoutCells == null || layoutCells.Count <= 0;
		if (!flag)
		{
			bool flag2 = this._collectHandler != null;
			if (flag2)
			{
				this._collectHandler(this._layoutCells);
			}
			else
			{
				this._layoutCells.ForEach(delegate(RectTransform e)
				{
					bool flag3 = null != e;
					if (flag3)
					{
						Object.Destroy(e.gameObject);
					}
				});
				this._layoutCells.Clear();
			}
		}
	}

	// Token: 0x060029FE RID: 10750 RVA: 0x0013ECA4 File Offset: 0x0013CEA4
	private void LayoutCellsAndAdjustSize()
	{
		List<RectTransform> layoutCells = this._layoutCells;
		bool flag = layoutCells == null || layoutCells.Count <= 0;
		if (!flag)
		{
			RectTransform layoutRoot = base.CGet<RectTransform>("Layout");
			float rootWidth = this._paddingX * 2f;
			float rootHeight = this._paddingY;
			float lineWidth = this._paddingX;
			float lineHeight = this._paddingY;
			int i = 0;
			int max = this._layoutCells.Count;
			while (i < max)
			{
				RectTransform cell = this._layoutCells[i];
				cell.SetParent(layoutRoot);
				cell.gameObject.SetActive(true);
				cell.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, lineWidth, cell.rect.width);
				cell.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, rootHeight, cell.rect.height);
				lineWidth += cell.rect.width + this._spacingX;
				lineHeight = Mathf.Max(lineHeight, cell.rect.height);
				bool flag2 = i > 0 && (i + 1) % this._lineCount == 0;
				if (flag2)
				{
					rootWidth = Mathf.Max(rootWidth, lineWidth - this._spacingX + this._paddingX);
					rootHeight += lineHeight + this._spacingY;
					lineHeight = 0f;
					lineWidth = this._paddingX;
				}
				i++;
			}
			rootWidth = Mathf.Max(rootWidth, lineWidth - this._spacingX + this._paddingX);
			rootHeight += lineHeight + this._paddingY;
			bool flag3 = lineHeight == 0f;
			if (flag3)
			{
				rootHeight -= this._spacingY;
			}
			layoutRoot.SetSize(new Vector2(rootWidth, rootHeight));
		}
	}

	// Token: 0x04001E7A RID: 7802
	private List<RectTransform> _layoutCells;

	// Token: 0x04001E7B RID: 7803
	private float _paddingX;

	// Token: 0x04001E7C RID: 7804
	private float _paddingY;

	// Token: 0x04001E7D RID: 7805
	private float _spacingX;

	// Token: 0x04001E7E RID: 7806
	private float _spacingY;

	// Token: 0x04001E7F RID: 7807
	private int _lineCount = 1;

	// Token: 0x04001E80 RID: 7808
	private bool _canStick;

	// Token: 0x04001E81 RID: 7809
	private Action<List<RectTransform>> _collectHandler;
}
