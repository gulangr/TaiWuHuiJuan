using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class MainUiCustomButton : MonoBehaviour
{
	// Token: 0x06001761 RID: 5985 RVA: 0x0008F999 File Offset: 0x0008DB99
	private void Awake()
	{
	}

	// Token: 0x06001762 RID: 5986 RVA: 0x0008F99C File Offset: 0x0008DB9C
	public void Init(Action openCharacterAction, Action openEquipmentAction, Action openInventoryAction, Action openEquipCombatSkillAction, Action openSkillBreakAction, Action openHealAction, Func<bool> forcedCanOpenCharacterMenuFunction)
	{
		this._openCharacterMenuAction = openCharacterAction;
		this._openEquipmentAction = openEquipmentAction;
		this._openInventoryAction = openInventoryAction;
		this._openEquipCombatSkillAction = openEquipCombatSkillAction;
		this._openSkillBreakAction = openSkillBreakAction;
		this._openHealAction = openHealAction;
		this._forcedCanOpenCharacterMenuFunction = forcedCanOpenCharacterMenuFunction;
	}

	// Token: 0x06001763 RID: 5987 RVA: 0x0008F9D4 File Offset: 0x0008DBD4
	public void RefreshButtons(List<sbyte> mainUiCustomButtonList)
	{
		bool flag = mainUiCustomButtonList == null;
		if (!flag)
		{
			for (int i = mainUiCustomButtonList.Count; i < this.customButtons.Length; i++)
			{
				this.customButtons[i].gameObject.SetActive(false);
			}
			for (int j = 0; j < mainUiCustomButtonList.Count; j++)
			{
				this.RefreshButton(j, mainUiCustomButtonList[j], this.customButtons[j]);
			}
			this.UpdateSettingButtonPosition();
		}
	}

	// Token: 0x06001764 RID: 5988 RVA: 0x0008FA58 File Offset: 0x0008DC58
	private void UpdateSettingButtonPosition()
	{
		bool flag = this.settingButton == null;
		if (!flag)
		{
			RectTransform settingButtonRect = this.settingButton.GetComponent<RectTransform>();
			bool flag2 = settingButtonRect == null;
			if (!flag2)
			{
				float rightmostX = 0f;
				for (int i = 0; i < this.customButtons.Length; i++)
				{
					CButtonObsolete button = this.customButtons[i];
					bool flag3 = button == null || !button.gameObject.activeInHierarchy;
					if (!flag3)
					{
						RectTransform buttonRect = button.GetComponent<RectTransform>();
						bool flag4 = buttonRect == null;
						if (!flag4)
						{
							float buttonRightX = buttonRect.anchoredPosition.x + buttonRect.rect.width;
							bool flag5 = buttonRightX > rightmostX;
							if (flag5)
							{
								rightmostX = buttonRightX;
							}
						}
					}
				}
				Vector2 currentPos = settingButtonRect.anchoredPosition;
				settingButtonRect.anchoredPosition = new Vector2(rightmostX + this.settingButtonMarginLeft, currentPos.y);
			}
		}
	}

	// Token: 0x06001765 RID: 5989 RVA: 0x0008FB5C File Offset: 0x0008DD5C
	private void RefreshButton(int i, sbyte templateId, CButtonObsolete button)
	{
		MainUiCustomButtonItem config = Config.MainUiCustomButton.Instance[templateId];
		TooltipInvoker tip = button.GetComponent<TooltipInvoker>();
		TooltipInvoker tooltipInvoker = tip;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tip.RuntimeParam.Set("arg0", config.Name);
		button.ClearAndAddListener(delegate
		{
			this.OnButtonClick(templateId);
		});
		button.interactable = this.GetButtonInteractable(templateId);
	}

	// Token: 0x06001766 RID: 5990 RVA: 0x0008FBEC File Offset: 0x0008DDEC
	private bool GetButtonInteractable(sbyte templateId)
	{
		bool canOpenCharacterMenu = this._forcedCanOpenCharacterMenuFunction();
		TutorialChapterModel tutorialModel = SingletonObject.getInstance<TutorialChapterModel>();
		switch (templateId)
		{
		case 0:
			return canOpenCharacterMenu;
		case 1:
			return canOpenCharacterMenu;
		case 2:
			return canOpenCharacterMenu && UI_Bottom.CanOpenCharacterMenuByLegacyAndTutorial() && tutorialModel.GetFunctionStatus(8);
		case 3:
		case 4:
		case 6:
			break;
		case 5:
			return canOpenCharacterMenu && UI_Bottom.CanOpenCharacterMenuByLegacyAndTutorial() && tutorialModel.GetFunctionStatus(7);
		case 7:
			return canOpenCharacterMenu && UI_Bottom.CanOpenCharacterMenuByLegacyAndTutorial() && tutorialModel.GetFunctionStatus(4);
		default:
			if (templateId == 11)
			{
				return canOpenCharacterMenu;
			}
			break;
		}
		return false;
	}

	// Token: 0x06001767 RID: 5991 RVA: 0x0008FCA0 File Offset: 0x0008DEA0
	private void OnButtonClick(sbyte templateId)
	{
		switch (templateId)
		{
		case 0:
		{
			Action openCharacterMenuAction = this._openCharacterMenuAction;
			if (openCharacterMenuAction != null)
			{
				openCharacterMenuAction();
			}
			break;
		}
		case 1:
		{
			Action openEquipmentAction = this._openEquipmentAction;
			if (openEquipmentAction != null)
			{
				openEquipmentAction();
			}
			break;
		}
		case 2:
		{
			Action openInventoryAction = this._openInventoryAction;
			if (openInventoryAction != null)
			{
				openInventoryAction();
			}
			break;
		}
		case 3:
		case 4:
		case 6:
			break;
		case 5:
		{
			Action openSkillBreakAction = this._openSkillBreakAction;
			if (openSkillBreakAction != null)
			{
				openSkillBreakAction();
			}
			break;
		}
		case 7:
		{
			Action openEquipCombatSkillAction = this._openEquipCombatSkillAction;
			if (openEquipCombatSkillAction != null)
			{
				openEquipCombatSkillAction();
			}
			break;
		}
		default:
			if (templateId == 11)
			{
				Action openHealAction = this._openHealAction;
				if (openHealAction != null)
				{
					openHealAction();
				}
			}
			break;
		}
	}

	// Token: 0x040012D3 RID: 4819
	[SerializeField]
	private CButtonObsolete[] customButtons;

	// Token: 0x040012D4 RID: 4820
	[SerializeField]
	private CButtonObsolete settingButton;

	// Token: 0x040012D5 RID: 4821
	[SerializeField]
	private float settingButtonMarginLeft = 18f;

	// Token: 0x040012D6 RID: 4822
	private Action _openCharacterMenuAction;

	// Token: 0x040012D7 RID: 4823
	private Action _openEquipmentAction;

	// Token: 0x040012D8 RID: 4824
	private Action _openInventoryAction;

	// Token: 0x040012D9 RID: 4825
	private Action _openEquipCombatSkillAction;

	// Token: 0x040012DA RID: 4826
	private Action _openSkillBreakAction;

	// Token: 0x040012DB RID: 4827
	private Action _openHealAction;

	// Token: 0x040012DC RID: 4828
	private Func<bool> _forcedCanOpenCharacterMenuFunction;
}
