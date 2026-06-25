using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000094 RID: 148
public class TemplatedContainer : MonoBehaviour
{
	// Token: 0x0600053C RID: 1340 RVA: 0x000239C8 File Offset: 0x00021BC8
	public static TemplatedContainer.ButtonOrderType GetButtonOrderType(sbyte lifeSkillType)
	{
		if (!true)
		{
		}
		TemplatedContainer.ButtonOrderType result;
		switch (lifeSkillType)
		{
		case 6:
			result = TemplatedContainer.ButtonOrderType.Forging;
			goto IL_59;
		case 7:
			result = TemplatedContainer.ButtonOrderType.Woodworking;
			goto IL_59;
		case 8:
			result = TemplatedContainer.ButtonOrderType.Medicine;
			goto IL_59;
		case 9:
			result = TemplatedContainer.ButtonOrderType.Toxicology;
			goto IL_59;
		case 10:
			result = TemplatedContainer.ButtonOrderType.Weaving;
			goto IL_59;
		case 11:
			result = TemplatedContainer.ButtonOrderType.Jade;
			goto IL_59;
		case 14:
			result = TemplatedContainer.ButtonOrderType.Cooking;
			goto IL_59;
		}
		result = TemplatedContainer.ButtonOrderType.Forging;
		IL_59:
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x00023A3C File Offset: 0x00021C3C
	public void ResetState()
	{
		this._childCount = 0;
		foreach (object obj in this.container)
		{
			Transform child = (Transform)obj;
			child.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x00023AA8 File Offset: 0x00021CA8
	public CButton CreateBtn(TemplatedContainer.ButtonOrderType buttonType, LanguageKey nameKey, string iconName, Action click)
	{
		return this.CreateBtn(buttonType, nameKey.Tr(), iconName, click);
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x00023ACC File Offset: 0x00021CCC
	public CButton CreateBtn(TemplatedContainer.ButtonOrderType buttonType, string nameStr, string iconName, Action click)
	{
		bool flag = this._childObjects.Count > this._childCount;
		GameObject btnGo;
		if (flag)
		{
			btnGo = this._childObjects[this._childCount];
		}
		else
		{
			btnGo = Object.Instantiate<GameObject>(this.template, this.container);
			this._childObjects.Add(btnGo);
		}
		btnGo.gameObject.SetActive(true);
		btnGo.name = buttonType.ToInt().ToString();
		this._childCount++;
		CButton btn = btnGo.GetComponent<CButton>();
		CImage btnIcon = btnGo.GetComponent<CImage>();
		TooltipInvoker mouseTip = btnGo.GetComponent<TooltipInvoker>();
		btnIcon.SetSprite(iconName + "0", false, null);
		TooltipInvoker tooltipInvoker = mouseTip;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		mouseTip.Type = TipType.SingleDesc;
		mouseTip.IsLanguageKey = false;
		mouseTip.RuntimeParam.Set("arg0", nameStr);
		btn.transition = Selectable.Transition.SpriteSwap;
		Sprite[] spriteArray = new Sprite[4];
		AtlasInfo.Instance.GetSprite(iconName + "1", delegate(Sprite sp)
		{
			spriteArray[0] = sp;
			spriteArray[2] = sp;
		});
		AtlasInfo.Instance.GetSprite(iconName + "2", delegate(Sprite sp)
		{
			spriteArray[1] = sp;
		});
		AtlasInfo.Instance.GetSprite(iconName + "3", delegate(Sprite sp)
		{
			spriteArray[3] = sp;
		});
		SpriteState spriteState = btn.spriteState;
		spriteState.highlightedSprite = spriteArray[0];
		spriteState.pressedSprite = null;
		spriteState.selectedSprite = spriteArray[2];
		spriteState.disabledSprite = spriteArray[3];
		btn.spriteState = spriteState;
		btn.onClick.ResetListener(click);
		return btn;
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x00023CAC File Offset: 0x00021EAC
	public void SortBtn()
	{
		this._childObjects.Sort(delegate(GameObject a, GameObject b)
		{
			int orderA;
			int.TryParse(a.name, out orderA);
			int orderB;
			int.TryParse(b.name, out orderB);
			bool flag = orderA != orderB;
			int result;
			if (flag)
			{
				result = orderA.CompareTo(orderB);
			}
			else
			{
				result = 0;
			}
			return result;
		});
		for (int i = this._childObjects.Count - 1; i >= 0; i--)
		{
			this._childObjects[i].transform.SetSiblingIndex(0);
		}
	}

	// Token: 0x0400043F RID: 1087
	[SerializeField]
	public RectTransform container;

	// Token: 0x04000440 RID: 1088
	[SerializeField]
	public GameObject template;

	// Token: 0x04000441 RID: 1089
	private int _childCount;

	// Token: 0x04000442 RID: 1090
	private List<GameObject> _childObjects = new List<GameObject>();

	// Token: 0x02001105 RID: 4357
	public enum ButtonOrderType
	{
		// Token: 0x04009531 RID: 38193
		SettlementTreasury,
		// Token: 0x04009532 RID: 38194
		Prison,
		// Token: 0x04009533 RID: 38195
		Bounty,
		// Token: 0x04009534 RID: 38196
		Law,
		// Token: 0x04009535 RID: 38197
		Sect,
		// Token: 0x04009536 RID: 38198
		MerchantBuilding,
		// Token: 0x04009537 RID: 38199
		WuHuZhenBao,
		// Token: 0x04009538 RID: 38200
		Cooking,
		// Token: 0x04009539 RID: 38201
		Medicine,
		// Token: 0x0400953A RID: 38202
		Toxicology,
		// Token: 0x0400953B RID: 38203
		Weaving,
		// Token: 0x0400953C RID: 38204
		Woodworking,
		// Token: 0x0400953D RID: 38205
		Jade,
		// Token: 0x0400953E RID: 38206
		Forging,
		// Token: 0x0400953F RID: 38207
		KungfuPracticeRoom,
		// Token: 0x04009540 RID: 38208
		PracticeCombatSkill,
		// Token: 0x04009541 RID: 38209
		Cricket,
		// Token: 0x04009542 RID: 38210
		StoneRoom,
		// Token: 0x04009543 RID: 38211
		JiaoPool,
		// Token: 0x04009544 RID: 38212
		TeaHouseCaravan,
		// Token: 0x04009545 RID: 38213
		SamsaraPlatform,
		// Token: 0x04009546 RID: 38214
		ChickenCoop,
		// Token: 0x04009547 RID: 38215
		AssignChicken,
		// Token: 0x04009548 RID: 38216
		ChickenCoopEvent,
		// Token: 0x04009549 RID: 38217
		SwapSoul,
		// Token: 0x0400954A RID: 38218
		MonkSoul
	}
}
