using System;
using TMPro;
using UnityEngine;

// Token: 0x0200014D RID: 333
public class bottomBtnProfessionSkill : MonoBehaviour
{
	// Token: 0x06001289 RID: 4745 RVA: 0x00070F0E File Offset: 0x0006F10E
	public void RefreshSkillType(EProfessionSkillType skillType)
	{
		this._isPassiveSkill = (skillType == EProfessionSkillType.Passive);
		this._selected = false;
		this._hovered = false;
		this.RefreshBackgroundState();
	}

	// Token: 0x0600128A RID: 4746 RVA: 0x00070F30 File Offset: 0x0006F130
	public void SetSelected(bool selected)
	{
		this._selected = selected;
		if (selected)
		{
			this._hovered = false;
		}
		this.RefreshBackgroundState();
	}

	// Token: 0x0600128B RID: 4747 RVA: 0x00070F58 File Offset: 0x0006F158
	public void ShowHover()
	{
		bool selected = this._selected;
		if (!selected)
		{
			this._hovered = true;
			this.RefreshBackgroundState();
		}
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x00070F80 File Offset: 0x0006F180
	public void HideHover()
	{
		bool selected = this._selected;
		if (!selected)
		{
			this._hovered = false;
			this.RefreshBackgroundState();
		}
	}

	// Token: 0x0600128D RID: 4749 RVA: 0x00070FA8 File Offset: 0x0006F1A8
	private void RefreshBackgroundState()
	{
		bottomBtnProfessionSkill.BackgroundState state = this._selected ? bottomBtnProfessionSkill.BackgroundState.Selected : (this._hovered ? bottomBtnProfessionSkill.BackgroundState.Hover : bottomBtnProfessionSkill.BackgroundState.Normal);
		Sprite backgroundSprite = this.GetBackgroundSprite(state, this._isPassiveSkill);
		foreach (CImage typeBg in this.typeBgs)
		{
			typeBg.sprite = backgroundSprite;
		}
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x00071004 File Offset: 0x0006F204
	private Sprite GetBackgroundSprite(bottomBtnProfessionSkill.BackgroundState state, bool isPassive)
	{
		Sprite result;
		if (isPassive)
		{
			if (state != bottomBtnProfessionSkill.BackgroundState.Hover)
			{
				if (state != bottomBtnProfessionSkill.BackgroundState.Selected)
				{
					result = this.passiveNormalBg;
				}
				else
				{
					result = this.passiveSelectedBg;
				}
			}
			else
			{
				result = this.passiveHoverBg;
			}
		}
		else if (state != bottomBtnProfessionSkill.BackgroundState.Hover)
		{
			if (state != bottomBtnProfessionSkill.BackgroundState.Selected)
			{
				result = this.activeNormalBg;
			}
			else
			{
				result = this.activeSelectedBg;
			}
		}
		else
		{
			result = this.activeHoverBg;
		}
		return result;
	}

	// Token: 0x04000FAA RID: 4010
	private bool _selected;

	// Token: 0x04000FAB RID: 4011
	private bool _hovered;

	// Token: 0x04000FAC RID: 4012
	private bool _isPassiveSkill;

	// Token: 0x04000FAD RID: 4013
	public GameObject unlocked;

	// Token: 0x04000FAE RID: 4014
	public GameObject locked;

	// Token: 0x04000FAF RID: 4015
	public CImage icon;

	// Token: 0x04000FB0 RID: 4016
	public GameObject coolDown;

	// Token: 0x04000FB1 RID: 4017
	public TextMeshProUGUI label;

	// Token: 0x04000FB2 RID: 4018
	public TextMeshProUGUI cdLabel;

	// Token: 0x04000FB3 RID: 4019
	public CImage hover;

	// Token: 0x04000FB4 RID: 4020
	public GameObject empty;

	// Token: 0x04000FB5 RID: 4021
	public GameObject content;

	// Token: 0x04000FB6 RID: 4022
	public GameObject numbers;

	// Token: 0x04000FB7 RID: 4023
	public GameObject numLeft1;

	// Token: 0x04000FB8 RID: 4024
	public GameObject numLeft2;

	// Token: 0x04000FB9 RID: 4025
	public GameObject numRight1;

	// Token: 0x04000FBA RID: 4026
	public GameObject numRight2;

	// Token: 0x04000FBB RID: 4027
	public CImage[] typeBgs;

	// Token: 0x04000FBC RID: 4028
	public Sprite activeNormalBg;

	// Token: 0x04000FBD RID: 4029
	public Sprite activeHoverBg;

	// Token: 0x04000FBE RID: 4030
	public Sprite activeSelectedBg;

	// Token: 0x04000FBF RID: 4031
	public Sprite passiveNormalBg;

	// Token: 0x04000FC0 RID: 4032
	public Sprite passiveHoverBg;

	// Token: 0x04000FC1 RID: 4033
	public Sprite passiveSelectedBg;

	// Token: 0x0200122D RID: 4653
	private enum BackgroundState
	{
		// Token: 0x040099C5 RID: 39365
		Normal,
		// Token: 0x040099C6 RID: 39366
		Hover,
		// Token: 0x040099C7 RID: 39367
		Selected
	}
}
