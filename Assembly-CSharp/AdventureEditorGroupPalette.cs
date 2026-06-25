using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Legacy.AdventureEditor.Migrate;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200017B RID: 379
public class AdventureEditorGroupPalette : TemplatedContainerAssemblyNew
{
	// Token: 0x06001500 RID: 5376 RVA: 0x00082352 File Offset: 0x00080552
	private void Awake()
	{
		base.Rebuild<AdventureEditorGroupPaletteTemplate>(this.palette.Length, delegate(AdventureEditorGroupPaletteTemplate unit, int index)
		{
			bool flag = index == 0;
			if (flag)
			{
				unit.gameObject.SetActive(false);
			}
			else
			{
				Color color = this.palette[index];
				CToggle toggle = unit.toggle;
				unit.sign.color = color;
				toggle.onValueChanged.ResetListener(delegate(bool isOn)
				{
					if (isOn)
					{
						this._selected.Add(index);
					}
					else
					{
						this._selected.Remove(index);
					}
					this.ForceRefresh();
				});
				toggle.SetIsOnWithoutNotify(false);
				toggle.isOn = true;
			}
		});
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x00082370 File Offset: 0x00080570
	private void Start()
	{
		this.ForceRefresh();
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x0008237C File Offset: 0x0008057C
	private void Update()
	{
		bool flag = (this._groupIdFlash.Item1 = this._groupIdFlash.Item1 - Time.deltaTime) < 0f;
		if (flag)
		{
			this._groupIdFlash = new ValueTuple<float, bool>(1f, !this._groupIdFlash.Item2);
		}
		this.FlashValue = Mathf.Lerp((float)(this._groupIdFlash.Item2 ? 0 : 1), (float)(this._groupIdFlash.Item2 ? 1 : 0), 1f - this._groupIdFlash.Item1 / 1f) * 0.5f + 0.5f;
		for (int i = 0; i < this.container.childCount; i++)
		{
			Transform child = this.container.GetChild(i);
			child.localPosition = child.localPosition.SetZ(0f);
		}
	}

	// Token: 0x17000258 RID: 600
	// (get) Token: 0x06001503 RID: 5379 RVA: 0x0008245F File Offset: 0x0008065F
	// (set) Token: 0x06001504 RID: 5380 RVA: 0x00082467 File Offset: 0x00080667
	public float FlashValue { get; private set; }

	// Token: 0x17000259 RID: 601
	// (get) Token: 0x06001505 RID: 5381 RVA: 0x00082470 File Offset: 0x00080670
	public ICollection<int> Selected
	{
		get
		{
			return this._selected;
		}
	}

	// Token: 0x1700025A RID: 602
	public Color this[int index]
	{
		get
		{
			return this.palette[index];
		}
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x00082488 File Offset: 0x00080688
	public Color[] GetCurrent()
	{
		Color[] current = this.palette.ToArray<Color>();
		for (int i = 0; i < current.Length; i++)
		{
			current[i] = (this._selected.Contains(i) ? current[i] : new Color(0f, 0f, 0f, 0f));
		}
		return current;
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x000824F0 File Offset: 0x000806F0
	public void ForceRefresh()
	{
		Color[] current = this.GetCurrent();
		UnityEvent<Color[]> unityEvent = this.paletteChanged;
		if (unityEvent != null)
		{
			unityEvent.Invoke(current);
		}
	}

	// Token: 0x04001189 RID: 4489
	public UnityEvent<Color[]> paletteChanged = new UnityEvent<Color[]>();

	// Token: 0x0400118A RID: 4490
	[SerializeField]
	private Color[] palette;

	// Token: 0x0400118B RID: 4491
	private readonly HashSet<int> _selected = new HashSet<int>();

	// Token: 0x0400118C RID: 4492
	[TupleElementNames(new string[]
	{
		"Duration",
		"FadeIn"
	})]
	private ValueTuple<float, bool> _groupIdFlash;
}
