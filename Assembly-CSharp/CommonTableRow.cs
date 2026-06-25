using System;
using System.Collections.Generic;
using Game.Components.Avatar;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000345 RID: 837
public class CommonTableRow : CommonToggleState
{
	// Token: 0x06003101 RID: 12545 RVA: 0x00180304 File Offset: 0x0017E504
	public void InitClickEvent(bool isOn, int charId, Action<int> onClicked)
	{
		this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnRowClicked));
		this.characterId = charId;
		this._onClicked = onClicked;
		this.toggle.isOn = isOn;
		this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnRowClicked));
	}

	// Token: 0x06003102 RID: 12546 RVA: 0x00180368 File Offset: 0x0017E568
	public override void OnStateChanged()
	{
		base.OnStateChanged();
		CommonToggleState.ToggleStates currState = base.CurrState;
		bool flag = currState == CommonToggleState.ToggleStates.Highlight || currState == CommonToggleState.ToggleStates.Hover;
		if (flag)
		{
			foreach (GameObject hoverObject in this.hoverObjects)
			{
				hoverObject.SetActive(true);
			}
		}
		else
		{
			foreach (GameObject hoverObject2 in this.hoverObjects)
			{
				hoverObject2.SetActive(false);
			}
		}
	}

	// Token: 0x06003103 RID: 12547 RVA: 0x0018042C File Offset: 0x0017E62C
	public void Refresh(bool isOn)
	{
		this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnRowClicked));
		this.toggle.isOn = isOn;
		this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnRowClicked));
	}

	// Token: 0x06003104 RID: 12548 RVA: 0x00180481 File Offset: 0x0017E681
	public void OnRowClicked(bool _)
	{
		Action<int> onClicked = this._onClicked;
		if (onClicked != null)
		{
			onClicked(this.characterId);
		}
	}

	// Token: 0x040023DC RID: 9180
	public List<GameObject> hoverObjects = new List<GameObject>();

	// Token: 0x040023DD RID: 9181
	public RectTransform content;

	// Token: 0x040023DE RID: 9182
	public Game.Components.Avatar.Avatar avatar;

	// Token: 0x040023DF RID: 9183
	public int characterId;

	// Token: 0x040023E0 RID: 9184
	private Action<int> _onClicked;
}
