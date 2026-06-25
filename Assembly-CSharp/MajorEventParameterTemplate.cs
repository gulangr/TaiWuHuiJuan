using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;

// Token: 0x02000191 RID: 401
public class MajorEventParameterTemplate : MajorEventTemplate<AdventureParameterSnapshot>
{
	// Token: 0x1700027D RID: 637
	// (get) Token: 0x06001693 RID: 5779 RVA: 0x0008AF3B File Offset: 0x0008913B
	public override IList<AdventureParameterSnapshot> DataList
	{
		get
		{
			return this.parent.Snapshot.Parameters;
		}
	}

	// Token: 0x06001694 RID: 5780 RVA: 0x0008AF4D File Offset: 0x0008914D
	public override void RefreshAll()
	{
		this.parent.RefreshParameter();
	}

	// Token: 0x06001695 RID: 5781 RVA: 0x0008AF5B File Offset: 0x0008915B
	public override void RefreshData()
	{
		this.RefreshKey();
		this.RefreshName();
		this.RefreshDesc();
		this.RefreshIcon();
		this.RefreshComment();
		this.RefreshInitValue();
	}

	// Token: 0x06001696 RID: 5782 RVA: 0x0008AF88 File Offset: 0x00089188
	public void RefreshKey()
	{
		this.key.SetTextWithoutNotify(base.Data.Key);
	}

	// Token: 0x06001697 RID: 5783 RVA: 0x0008AFA1 File Offset: 0x000891A1
	public void RefreshName()
	{
		this.name.SetTextWithoutNotify(base.Data.Name);
	}

	// Token: 0x06001698 RID: 5784 RVA: 0x0008AFBF File Offset: 0x000891BF
	public void RefreshDesc()
	{
		this.desc.SetTextWithoutNotify(base.Data.Desc);
	}

	// Token: 0x06001699 RID: 5785 RVA: 0x0008AFDD File Offset: 0x000891DD
	public void RefreshIcon()
	{
		this.icon.SetTextWithoutNotify(base.Data.Icon);
	}

	// Token: 0x0600169A RID: 5786 RVA: 0x0008AFF6 File Offset: 0x000891F6
	public void RefreshComment()
	{
		this.comment.SetTextWithoutNotify(base.Data.Comment);
	}

	// Token: 0x0600169B RID: 5787 RVA: 0x0008B00F File Offset: 0x0008920F
	public void RefreshInitValue()
	{
		this.initialValue.SetTextWithoutNotify(base.Data.InitialValue.ToString());
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x0008B02D File Offset: 0x0008922D
	public void EditKey(string str)
	{
		base.Data.Key = str;
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x0008B03B File Offset: 0x0008923B
	public void EditName(string str)
	{
		base.Data.Name = str;
	}

	// Token: 0x0600169E RID: 5790 RVA: 0x0008B04E File Offset: 0x0008924E
	public void EditDesc(string str)
	{
		base.Data.Desc = str;
	}

	// Token: 0x0600169F RID: 5791 RVA: 0x0008B061 File Offset: 0x00089261
	public void EditIcon(string str)
	{
		base.Data.Icon = str;
	}

	// Token: 0x060016A0 RID: 5792 RVA: 0x0008B06F File Offset: 0x0008926F
	public void EditComment(string str)
	{
		base.Data.Comment = str;
	}

	// Token: 0x060016A1 RID: 5793 RVA: 0x0008B07D File Offset: 0x0008927D
	public void EditInitValue(string str)
	{
		AdventureMajorEventTool.EditInt(ref base.Data.InitialValue, str, "EditInitValue");
	}

	// Token: 0x04001258 RID: 4696
	[SerializeField]
	private TMP_InputField key;

	// Token: 0x04001259 RID: 4697
	[SerializeField]
	private new TMP_InputField name;

	// Token: 0x0400125A RID: 4698
	[SerializeField]
	private TMP_InputField desc;

	// Token: 0x0400125B RID: 4699
	[SerializeField]
	private TMP_InputField icon;

	// Token: 0x0400125C RID: 4700
	[SerializeField]
	private TMP_InputField unit;

	// Token: 0x0400125D RID: 4701
	[SerializeField]
	private TMP_InputField totalProgress;

	// Token: 0x0400125E RID: 4702
	[SerializeField]
	private TMP_InputField initialValue;

	// Token: 0x0400125F RID: 4703
	[SerializeField]
	private TMP_InputField comment;

	// Token: 0x04001260 RID: 4704
	[SerializeField]
	private CToggle isPercent;

	// Token: 0x04001261 RID: 4705
	[SerializeField]
	private CToggle isNegative;
}
