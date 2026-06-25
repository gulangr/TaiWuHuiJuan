using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003B0 RID: 944
public class UI_TestNewUIZyh : Refers
{
	// Token: 0x060038AF RID: 14511 RVA: 0x001C9B7C File Offset: 0x001C7D7C
	public void OnEnable()
	{
		List<CommonProgressFavaor> favorList = base.CGetList<CommonProgressFavaor>("CommonProgressFavaor");
		for (int i = 0; i < favorList.Count; i++)
		{
			favorList[i].SetProgress(Random.Range(0.5f, 1f), false);
			favorList[i].SetupByFavorabilityType((sbyte)(i - 6));
		}
		this.mapChars = base.CGetList<Refers>("char_");
		base.CGet<CToggleObsolete>("CommonStripInteractToggle").interactable = false;
	}

	// Token: 0x060038B0 RID: 14512 RVA: 0x001C9C00 File Offset: 0x001C7E00
	private void OnClickCheckBox()
	{
		GameObject icon = this.checkBox.CGet<GameObject>("Icon");
		icon.SetActive(!icon.activeSelf);
	}

	// Token: 0x060038B1 RID: 14513 RVA: 0x001C9C30 File Offset: 0x001C7E30
	private void Update()
	{
		bool keyDown = Input.GetKeyDown(KeyCode.F1);
		if (keyDown)
		{
			foreach (Refers item in this.mapChars)
			{
				CommonCharacterNameFrame nameFrame = item.CGet<CommonCharacterNameFrame>("CharacterName");
				CommonCharacterNameFrame roleFrame = item.CGet<CommonCharacterNameFrame>("CharacterRole");
				nameFrame.SetName("测试名称");
				roleFrame.SetName("测试角色");
			}
		}
		bool keyDown2 = Input.GetKeyDown(KeyCode.F2);
		if (keyDown2)
		{
			foreach (Refers item2 in this.mapChars)
			{
				CommonCharacterNameFrame nameFrame2 = item2.CGet<CommonCharacterNameFrame>("CharacterName");
				CommonCharacterNameFrame roleFrame2 = item2.CGet<CommonCharacterNameFrame>("CharacterRole");
				nameFrame2.SetName("Testtest \n namenmane");
				roleFrame2.SetName("Testtest rolenamerole");
			}
		}
	}

	// Token: 0x0400290D RID: 10509
	[SerializeField]
	private Refers checkBoxDisable;

	// Token: 0x0400290E RID: 10510
	[SerializeField]
	private Refers checkBox;

	// Token: 0x0400290F RID: 10511
	private List<Refers> mapChars;
}
