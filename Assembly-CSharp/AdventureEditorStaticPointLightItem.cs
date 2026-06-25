using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.AdventureEditor.Migrate;
using GameData.Adventure;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000184 RID: 388
public class AdventureEditorStaticPointLightItem : AdventureEditorPointLightComponent
{
	// Token: 0x060015EF RID: 5615 RVA: 0x00088254 File Offset: 0x00086454
	private void Awake()
	{
		base.OnInit();
		this.gridXInput.onEndEdit.AddListener(new UnityAction<string>(this.OnGridChanged));
		this.gridYInput.onEndEdit.AddListener(new UnityAction<string>(this.OnGridChanged));
		this.gridIInput.onEndEdit.AddListener(new UnityAction<string>(this.OnGridChanged));
		this.rangeInput.OnValueChanged += this.OnRangeChanged;
		this.noRangeClampToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnNoRangeClampChanged));
		this.deleteBtn.ClearAndAddListener(delegate
		{
			Action onDelete = this._onDelete;
			if (onDelete != null)
			{
				onDelete();
			}
		});
	}

	// Token: 0x060015F0 RID: 5616 RVA: 0x0008830D File Offset: 0x0008650D
	public void Setup(int index, Action onDelete)
	{
		this._index = index;
		this._onDelete = onDelete;
		this.RefreshItem();
	}

	// Token: 0x060015F1 RID: 5617 RVA: 0x00088325 File Offset: 0x00086525
	public void UpdateIndex(int index)
	{
		this._index = index;
	}

	// Token: 0x060015F2 RID: 5618 RVA: 0x0008832F File Offset: 0x0008652F
	public void RefreshItem()
	{
		this.ReloadFromBlackBoard();
		this.RefreshExtra();
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x00088340 File Offset: 0x00086540
	private void RefreshExtra()
	{
		List<AdventurePointLightSnapshot> list = AdventureEditorKit.BlackBoard.Editing.LightingPoints;
		bool flag = this._index < 0 || this._index >= list.Count;
		if (!flag)
		{
			AdventurePointLightSnapshot snapshot = list[this._index];
			this._isRefreshing = true;
			this.gridXInput.SetTextWithoutNotify(snapshot.Index.X.ToString());
			this.gridYInput.SetTextWithoutNotify(snapshot.Index.Y.ToString());
			this.gridIInput.SetTextWithoutNotify(snapshot.Index.I.ToString());
			this.rangeInput.Set((float)Mathf.Abs(snapshot.Range));
			this.noRangeClampToggle.isOn = (snapshot.Range < 0);
			this._isRefreshing = false;
		}
	}

	// Token: 0x060015F4 RID: 5620 RVA: 0x00088420 File Offset: 0x00086620
	protected override AdventureLightData GetTargetLightData()
	{
		List<AdventurePointLightSnapshot> list = AdventureEditorKit.BlackBoard.Editing.LightingPoints;
		bool flag = this._index < 0 || this._index >= list.Count;
		AdventureLightData result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = list[this._index].LightData;
		}
		return result;
	}

	// Token: 0x060015F5 RID: 5621 RVA: 0x00088478 File Offset: 0x00086678
	protected override void ApplyColor(string hex)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot s)
		{
			s.LightingPoints[this._index].LightData.ColorInHex = hex;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x000884B4 File Offset: 0x000866B4
	protected override void ApplyIntensity(float value)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot s)
		{
			s.LightingPoints[this._index].LightData.Strength = value;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x000884F0 File Offset: 0x000866F0
	protected override void ApplyVirtualZ(float value)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot s)
		{
			s.LightingPoints[this._index].LightData.Height = value;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x060015F8 RID: 5624 RVA: 0x0008852C File Offset: 0x0008672C
	private void OnGridChanged(string value)
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			List<AdventurePointLightSnapshot> list = AdventureEditorKit.BlackBoard.Editing.LightingPoints;
			bool flag = this._index < 0 || this._index >= list.Count;
			if (!flag)
			{
				AdventureBlockIndex idx = list[this._index].Index;
				int x;
				bool flag2 = int.TryParse(this.gridXInput.text, out x);
				if (flag2)
				{
					idx = idx.SetX(x);
				}
				int y;
				bool flag3 = int.TryParse(this.gridYInput.text, out y);
				if (flag3)
				{
					idx = idx.SetY(y);
				}
				int i;
				bool flag4 = int.TryParse(this.gridIInput.text, out i);
				if (flag4)
				{
					idx = idx.SetI(i);
				}
				AdventureBlockIndex captured = idx;
				AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot s)
				{
					s.LightingPoints[this._index].Index = captured;
				}, EAdventureEditType.Basic);
				this.gridXInput.SetTextWithoutNotify(captured.X.ToString());
				this.gridYInput.SetTextWithoutNotify(captured.Y.ToString());
				this.gridIInput.SetTextWithoutNotify(captured.I.ToString());
			}
		}
	}

	// Token: 0x060015F9 RID: 5625 RVA: 0x0008867C File Offset: 0x0008687C
	private void OnRangeChanged()
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			int signedValue = this.noRangeClampToggle.isOn ? (-(int)this.rangeInput.Value) : ((int)this.rangeInput.Value);
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot s)
			{
				s.LightingPoints[this._index].Range = signedValue;
			}, EAdventureEditType.Basic);
		}
	}

	// Token: 0x060015FA RID: 5626 RVA: 0x000886EC File Offset: 0x000868EC
	private void OnNoRangeClampChanged(bool enabled)
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			int absValue = (int)this.rangeInput.Value;
			int signedValue = enabled ? (-absValue) : absValue;
			AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot s)
			{
				s.LightingPoints[this._index].Range = signedValue;
			}, EAdventureEditType.Basic);
		}
	}

	// Token: 0x040011FC RID: 4604
	[SerializeField]
	private CButton deleteBtn;

	// Token: 0x040011FD RID: 4605
	[SerializeField]
	private TMP_InputField gridXInput;

	// Token: 0x040011FE RID: 4606
	[SerializeField]
	private TMP_InputField gridYInput;

	// Token: 0x040011FF RID: 4607
	[SerializeField]
	private TMP_InputField gridIInput;

	// Token: 0x04001200 RID: 4608
	[SerializeField]
	private InputCSlider rangeInput;

	// Token: 0x04001201 RID: 4609
	[SerializeField]
	private CToggle noRangeClampToggle;

	// Token: 0x04001202 RID: 4610
	private int _index;

	// Token: 0x04001203 RID: 4611
	private Action _onDelete;
}
