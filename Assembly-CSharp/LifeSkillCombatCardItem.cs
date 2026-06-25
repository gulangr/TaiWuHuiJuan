using System;
using Config;
using UnityEngine;

// Token: 0x02000246 RID: 582
public class LifeSkillCombatCardItem : Refers
{
	// Token: 0x170003F8 RID: 1016
	// (get) Token: 0x060025EB RID: 9707 RVA: 0x00116340 File Offset: 0x00114540
	public DebateStrategyItem CardConfig
	{
		get
		{
			return this.CardView.CardConfig;
		}
	}

	// Token: 0x170003F9 RID: 1017
	// (get) Token: 0x060025EC RID: 9708 RVA: 0x0011634D File Offset: 0x0011454D
	public bool CanSelect
	{
		get
		{
			return this.CardView.Button.interactable;
		}
	}

	// Token: 0x170003FA RID: 1018
	// (get) Token: 0x060025ED RID: 9709 RVA: 0x0011635F File Offset: 0x0011455F
	// (set) Token: 0x060025EE RID: 9710 RVA: 0x00116367 File Offset: 0x00114567
	public bool Selected { get; private set; }

	// Token: 0x170003FB RID: 1019
	// (get) Token: 0x060025EF RID: 9711 RVA: 0x00116370 File Offset: 0x00114570
	// (set) Token: 0x060025F0 RID: 9712 RVA: 0x00116378 File Offset: 0x00114578
	public bool Visible { get; private set; }

	// Token: 0x060025F1 RID: 9713 RVA: 0x00116384 File Offset: 0x00114584
	public void SetVisible(bool visible, bool includeView = true)
	{
		this.Visible = visible;
		base.gameObject.SetActive(visible);
		bool flag = !this.Visible;
		if (flag)
		{
			bool selected = this.Selected;
			if (selected)
			{
				this.SetSelect(false, true);
			}
		}
		if (includeView)
		{
			this.CardView.gameObject.SetActive(visible);
		}
	}

	// Token: 0x060025F2 RID: 9714 RVA: 0x001163E0 File Offset: 0x001145E0
	public void SetSelect(bool select, bool hasScale = true)
	{
		this.CardView.SetSelected(select, hasScale);
		this.Selected = select;
	}

	// Token: 0x04001C0C RID: 7180
	[SerializeField]
	public LifeSkillCombatCardView CardView;
}
