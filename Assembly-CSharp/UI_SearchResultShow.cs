using System;
using System.Collections.Generic;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x0200039C RID: 924
public class UI_SearchResultShow : UIBase
{
	// Token: 0x0600379A RID: 14234 RVA: 0x001BFC1C File Offset: 0x001BDE1C
	private void Awake()
	{
		this.ResultScroll.OnItemRender = new Action<int, Refers>(this.OnItemRender);
	}

	// Token: 0x0600379B RID: 14235 RVA: 0x001BFC38 File Offset: 0x001BDE38
	public override void OnInit(ArgumentBox argsBox)
	{
		this.InitSolutionMap();
		argsBox.Get<List<string>>("Data", out this._showDataList);
		argsBox.Get<Dictionary<string, string>>("Descs", out this._descs);
		argsBox.Get<UI_SearchResultShow.OnSelectEvent>("OnSelect", out this._onSelect);
		bool flag = !argsBox.Get("ViewCount", out this._viewCellCount);
		if (flag)
		{
			this._viewCellCount = 7;
		}
		RectTransform prefabRectTrans = this.ResultScroll.SrcPrefab.transform as RectTransform;
		Vector2 size = Vector2.zero;
		bool flag2 = argsBox.Get<RectTransform>("Trans", out this._targetRectTrans);
		if (flag2)
		{
			bool success = false;
			Enum enumValue;
			bool flag3 = argsBox.Get("ScrollSolution", out enumValue);
			if (flag3)
			{
				ESearchResultShowSolution solution;
				bool flag4;
				if (enumValue is ESearchResultShowSolution)
				{
					solution = (ESearchResultShowSolution)enumValue;
					flag4 = true;
				}
				else
				{
					flag4 = false;
				}
				bool flag5 = flag4;
				if (flag5)
				{
					Action placeAction;
					bool flag6 = this._placeScrollSolutionMap.TryGetValue(solution, out placeAction);
					if (flag6)
					{
						success = true;
						UIElement element = this.Element;
						element.OnShowed = (Action)Delegate.Combine(element.OnShowed, placeAction);
					}
				}
			}
			bool flag7 = !success;
			if (flag7)
			{
				UIElement element2 = this.Element;
				element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(this.PlaceScrollAuto));
			}
			bool auto;
			bool flag8 = argsBox.Get("AutoSize", out auto);
			if (flag8)
			{
				bool flag9 = auto;
				if (flag9)
				{
					size.x = this._targetRectTrans.rect.size.x - 10f;
					size.y = this._targetRectTrans.rect.size.y;
				}
			}
		}
		float width;
		bool flag10 = argsBox.Get("StaticWidth", out width);
		if (flag10)
		{
			bool flag11 = null != prefabRectTrans;
			if (flag11)
			{
				size.x = width;
			}
		}
		float height;
		bool flag12 = argsBox.Get("StaticHeight", out height);
		if (flag12)
		{
			bool flag13 = null != prefabRectTrans;
			if (flag13)
			{
				size.y = height;
			}
		}
		bool flag14 = null != prefabRectTrans && size.x > 0f && size.y > 0f;
		if (flag14)
		{
			prefabRectTrans.SetSize(size);
			this.ScrollRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x + 10f);
		}
		this._cellSize = size;
	}

	// Token: 0x0600379C RID: 14236 RVA: 0x001BFE8C File Offset: 0x001BE08C
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = "Hide" == btnName;
		if (flag)
		{
			this.Hide();
		}
	}

	// Token: 0x0600379D RID: 14237 RVA: 0x001BFEB9 File Offset: 0x001BE0B9
	private void OnDisable()
	{
		this._targetRectTrans = null;
		this.ResultScroll.SetDataCount(0);
	}

	// Token: 0x0600379E RID: 14238 RVA: 0x001BFED0 File Offset: 0x001BE0D0
	private void OnEnable()
	{
		bool flag = this._showDataList.Count <= 0 || this._onSelect == null;
		if (flag)
		{
			bool flag2 = this._showDataList != null;
			if (flag2)
			{
				EasyPool.Free<List<string>>(this._showDataList);
			}
			this.Element.Hide(false);
			this._showDataList = null;
		}
		else
		{
			RectTransform prefabRect = this.ResultScroll.SrcPrefab.GetComponent<RectTransform>();
			bool flag3 = prefabRect;
			if (flag3)
			{
				this._prefabHeight = prefabRect.rect.height;
			}
			this.ResetScrollSize();
			this.RefreshList();
		}
	}

	// Token: 0x0600379F RID: 14239 RVA: 0x001BFF6C File Offset: 0x001BE16C
	private void InitSolutionMap()
	{
		bool flag = this._placeScrollSolutionMap != null;
		if (!flag)
		{
			this._placeScrollSolutionMap = new Dictionary<ESearchResultShowSolution, Action>();
			this._placeScrollSolutionMap.Add(ESearchResultShowSolution.None, new Action(this.PlaceScrollAuto));
			this._placeScrollSolutionMap.Add(ESearchResultShowSolution.RectMaxDown, new Action(this.PlaceScrollRectMaxDown));
			this._placeScrollSolutionMap.Add(ESearchResultShowSolution.RectMinUp, new Action(this.PlaceScrollRectMinUp));
		}
	}

	// Token: 0x060037A0 RID: 14240 RVA: 0x001BFFE0 File Offset: 0x001BE1E0
	public void UpdateData(List<string> newShowDataList, Dictionary<string, string> descs = null)
	{
		bool flag = this._showDataList != null && this._showDataList != newShowDataList;
		if (flag)
		{
			EasyPool.Free<List<string>>(this._showDataList);
		}
		this._showDataList = newShowDataList;
		bool flag2 = this._descs != null && this._descs != descs;
		if (flag2)
		{
			EasyPool.Free<Dictionary<string, string>>(this._descs);
		}
		this._descs = descs;
		bool flag3 = this._showDataList.Count <= 0;
		if (flag3)
		{
			this.Hide();
		}
		else
		{
			this.ResetScrollSize();
		}
	}

	// Token: 0x060037A1 RID: 14241 RVA: 0x001C0070 File Offset: 0x001BE270
	public void UpdateData(List<string> newShowDataList, Dictionary<string, string> descs, UI_SearchResultShow.OnSelectEvent onSelect)
	{
		this._onSelect = onSelect;
		this.UpdateData(newShowDataList, descs);
	}

	// Token: 0x060037A2 RID: 14242 RVA: 0x001C0084 File Offset: 0x001BE284
	public void Hide()
	{
		bool flag = this._showDataList != null;
		if (flag)
		{
			EasyPool.Free<List<string>>(this._showDataList);
			this._showDataList = null;
		}
		this._onSelect = null;
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x060037A3 RID: 14243 RVA: 0x001C00CC File Offset: 0x001BE2CC
	private void RefreshList()
	{
		bool activeSelf = base.gameObject.activeSelf;
		if (activeSelf)
		{
			this.ResultScroll.InitPageCount();
			this.ResultScroll.UpdateData(this._showDataList.Count);
		}
	}

	// Token: 0x060037A4 RID: 14244 RVA: 0x001C0110 File Offset: 0x001BE310
	private void OnItemRender(int index, Refers refer)
	{
		RectTransform referRectTrans = refer.GetComponent<RectTransform>();
		referRectTrans.SetSize(this._cellSize);
		string showText = this._showDataList[index];
		refer.CGet<TextMeshProUGUI>("ShowText").text = showText;
		refer.CGet<CButtonObsolete>("Button").ClearAndAddListener(delegate
		{
			UI_SearchResultShow.OnSelectEvent onSelect = this._onSelect;
			if (onSelect != null)
			{
				onSelect(index, this._showDataList[index]);
			}
			this.Hide();
		});
		TooltipInvoker tipDisplayer = refer.GetComponent<TooltipInvoker>();
		bool flag = tipDisplayer == null;
		if (flag)
		{
			tipDisplayer = refer.gameObject.AddComponent<TooltipInvoker>();
		}
		string desc;
		bool flag2 = this._descs != null && this._descs.TryGetValue(showText, out desc);
		if (flag2)
		{
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.PresetParam == null)
			{
				tooltipInvoker.PresetParam = new string[1];
			}
			tipDisplayer.PresetParam[0] = desc;
			tipDisplayer.enabled = true;
		}
		else
		{
			tipDisplayer.enabled = false;
		}
	}

	// Token: 0x060037A5 RID: 14245 RVA: 0x001C0200 File Offset: 0x001BE400
	private void PlaceScrollAuto()
	{
		Vector2 rectMinWorldPos = this._targetRectTrans.TransformPoint(this._targetRectTrans.rect.min);
		Vector2 rectMaxWorldPos = this._targetRectTrans.TransformPoint(this._targetRectTrans.rect.max);
		Vector2 rectMinViewportPos = UIManager.Instance.UiCamera.WorldToViewportPoint(rectMinWorldPos);
		RectTransform selfRectTransform = base.CGet<RectTransform>("SelfRectTrans");
		Vector2 pivot = Vector2.zero;
		bool flag = rectMinViewportPos.y >= 0.4f;
		if (flag)
		{
			pivot.y = 1f;
		}
		this.ScrollRectTrans.pivot = pivot;
		this.ScrollRectTrans.anchorMin = pivot;
		this.ScrollRectTrans.anchorMax = pivot;
		Vector2 minPos = selfRectTransform.InverseTransformPoint(rectMinWorldPos);
		Vector2 maxPos = selfRectTransform.InverseTransformPoint(rectMaxWorldPos);
		minPos.x -= 5f;
		bool flag2 = rectMinViewportPos.y >= 0.4f;
		if (flag2)
		{
			this.ScrollRectTrans.localPosition = minPos;
			this.ResultScroll.Direction = InfinityScrollLegacy.ScrollDirection.FromTop;
		}
		else
		{
			this.ScrollRectTrans.localPosition = new Vector2(minPos.x, maxPos.y);
			this.ResultScroll.Direction = InfinityScrollLegacy.ScrollDirection.FromBottom;
		}
		InfinityScrollLegacy resultScroll = this.ResultScroll;
		List<string> showDataList = this._showDataList;
		resultScroll.SetDataCount((showDataList != null) ? showDataList.Count : 0);
		this.ResultScroll.UpdateStyle(this.ResultScroll.Direction, this.ResultScroll.LineCount, this.ResultScroll.Gap, this.ResultScroll.Padding, this.ResultScroll.SrcPrefab);
	}

	// Token: 0x060037A6 RID: 14246 RVA: 0x001C03E0 File Offset: 0x001BE5E0
	private void PlaceScrollRectMaxDown()
	{
		Vector2 rectMaxWorldPos = this._targetRectTrans.TransformPoint(this._targetRectTrans.rect.max);
		this.ResultScroll.Direction = InfinityScrollLegacy.ScrollDirection.FromTop;
		this.ScrollRectTrans.pivot = Vector2.up;
		this.ScrollRectTrans.position = rectMaxWorldPos;
		InfinityScrollLegacy resultScroll = this.ResultScroll;
		List<string> showDataList = this._showDataList;
		resultScroll.SetDataCount((showDataList != null) ? showDataList.Count : 0);
		this.ResultScroll.UpdateStyle(this.ResultScroll.Direction, this.ResultScroll.LineCount, this.ResultScroll.Gap, this.ResultScroll.Padding, this.ResultScroll.SrcPrefab);
	}

	// Token: 0x060037A7 RID: 14247 RVA: 0x001C04A8 File Offset: 0x001BE6A8
	private void PlaceScrollRectMinUp()
	{
		Vector2 rectMaxWorldPos = this._targetRectTrans.TransformPoint(this._targetRectTrans.rect.min);
		this.ResultScroll.Direction = InfinityScrollLegacy.ScrollDirection.FromBottom;
		this.ScrollRectTrans.pivot = Vector2.right;
		this.ScrollRectTrans.position = rectMaxWorldPos;
		InfinityScrollLegacy resultScroll = this.ResultScroll;
		List<string> showDataList = this._showDataList;
		resultScroll.SetDataCount((showDataList != null) ? showDataList.Count : 0);
		this.ResultScroll.UpdateStyle(this.ResultScroll.Direction, this.ResultScroll.LineCount, this.ResultScroll.Gap, this.ResultScroll.Padding, this.ResultScroll.SrcPrefab);
	}

	// Token: 0x060037A8 RID: 14248 RVA: 0x001C0570 File Offset: 0x001BE770
	private void ResetScrollSize()
	{
		float showCount = (float)Mathf.Min(this._showDataList.Count, (int)this._viewCellCount);
		this.ScrollRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, showCount * (this._prefabHeight + this.ResultScroll.Gap.y));
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, new Action(this.RefreshList));
	}

	// Token: 0x060037A9 RID: 14249 RVA: 0x001C05D4 File Offset: 0x001BE7D4
	public static void ShowInputHint(string currVal, TMP_InputField inputField, IEnumerable<string> allPotentialOptions, Dictionary<string, string> descs = null)
	{
		List<string> optionStrings = EasyPool.Get<List<string>>();
		optionStrings.Clear();
		bool flag = string.IsNullOrEmpty(currVal);
		if (flag)
		{
			optionStrings.AddRange(allPotentialOptions);
		}
		else
		{
			foreach (string enumName in allPotentialOptions)
			{
				bool flag2 = enumName.Contains(currVal);
				if (flag2)
				{
					optionStrings.Add(enumName);
				}
			}
		}
		UI_SearchResultShow.ShowInputHint(inputField, optionStrings, descs);
	}

	// Token: 0x060037AA RID: 14250 RVA: 0x001C0658 File Offset: 0x001BE858
	private static void ShowInputHint(TMP_InputField inputField, List<string> options, Dictionary<string, string> descs = null)
	{
		bool flag = options.Count <= 0;
		if (flag)
		{
			bool flag2 = UIManager.Instance.IsElementActive(UIElement.SearchResultShow);
			if (flag2)
			{
				UIManager.Instance.HideUI(UIElement.SearchResultShow);
			}
		}
		else
		{
			bool flag3 = UIManager.Instance.IsElementActive(UIElement.SearchResultShow);
			if (flag3)
			{
				UI_SearchResultShow ui_SearchResultShow = UIElement.SearchResultShow.UiBase as UI_SearchResultShow;
				if (ui_SearchResultShow != null)
				{
					ui_SearchResultShow.UpdateData(options, descs);
				}
			}
			else
			{
				UI_SearchResultShow.OnSelectEvent onSelect = delegate(int _, string value)
				{
					inputField.SetTextWithoutNotify(value);
					TMP_InputField.SubmitEvent onEndEdit = inputField.onEndEdit;
					if (onEndEdit != null)
					{
						onEndEdit.Invoke(value);
					}
				};
				RectTransform rectTrans = inputField.GetComponent<RectTransform>();
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("Data", options);
				argBox.SetObject("Descs", descs);
				argBox.SetObject("OnSelect", onSelect);
				argBox.SetObject("Trans", rectTrans);
				argBox.Set("AutoSize", true);
				bool flag4 = rectTrans.rect.width < 200f;
				if (flag4)
				{
					argBox.Set("StaticWidth", 200f);
				}
				UIElement.SearchResultShow.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.SearchResultShow, true);
			}
		}
	}

	// Token: 0x04002835 RID: 10293
	public InfinityScrollLegacy ResultScroll;

	// Token: 0x04002836 RID: 10294
	public RectTransform ScrollRectTrans;

	// Token: 0x04002837 RID: 10295
	private List<string> _showDataList;

	// Token: 0x04002838 RID: 10296
	private Dictionary<string, string> _descs;

	// Token: 0x04002839 RID: 10297
	private UI_SearchResultShow.OnSelectEvent _onSelect;

	// Token: 0x0400283A RID: 10298
	private Vector2 _cellSize;

	// Token: 0x0400283B RID: 10299
	private RectTransform _targetRectTrans;

	// Token: 0x0400283C RID: 10300
	private sbyte _viewCellCount = 7;

	// Token: 0x0400283D RID: 10301
	private Dictionary<ESearchResultShowSolution, Action> _placeScrollSolutionMap;

	// Token: 0x0400283E RID: 10302
	private float _prefabHeight;

	// Token: 0x020017ED RID: 6125
	// (Invoke) Token: 0x0600D570 RID: 54640
	public delegate void OnSelectEvent(int index, string value);
}
