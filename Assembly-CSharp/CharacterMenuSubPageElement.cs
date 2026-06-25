using System;
using Game.Views.CharacterMenu;
using UnityEngine;

// Token: 0x020001CD RID: 461
public class CharacterMenuSubPageElement : UIElement
{
	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x06001CBA RID: 7354 RVA: 0x000C9356 File Offset: 0x000C7556
	// (set) Token: 0x06001CBB RID: 7355 RVA: 0x000C935E File Offset: 0x000C755E
	public bool Visible { get; private set; }

	// Token: 0x06001CBC RID: 7356 RVA: 0x000C9367 File Offset: 0x000C7567
	public void RawHide()
	{
		this.Hide(false);
		base.Hide(true);
	}

	// Token: 0x06001CBD RID: 7357 RVA: 0x000C937C File Offset: 0x000C757C
	public override void Hide(bool quickHide = false)
	{
		Transform uiBaseTrans = this.UiBase.transform;
		uiBaseTrans.localPosition = 3000f * Vector3.up + Vector3.up * 1500f * (float)(base.UiBaseAs<UI_CharacterMenuSubPageBase>().Key + 1);
		this.UiBase.GetComponent<Canvas>().enabled = false;
		base.UiBaseAs<UI_CharacterMenuSubPageBase>().OnSubpageInVisible();
		base.SetElementReady(false);
		this.Visible = false;
	}

	// Token: 0x06001CBE RID: 7358 RVA: 0x000C9404 File Offset: 0x000C7604
	public override void Show()
	{
		this.UiBase.GetComponent<Canvas>().enabled = true;
		bool visible = this.Visible;
		if (!visible)
		{
			bool flag = !this.UiBase.gameObject.activeSelf;
			if (flag)
			{
				this.UiBase.gameObject.SetActive(true);
			}
			UI_CharacterMenuSubPageBase page = base.UiBaseAs<UI_CharacterMenuSubPageBase>();
			page.SetCharId(page.CharacterMenu.CurCharacterId);
			page.OnSubpageVisible();
		}
	}

	// Token: 0x06001CBF RID: 7359 RVA: 0x000C947C File Offset: 0x000C767C
	public override void ShowAfterRefresh()
	{
		bool flag = !this.UiBase.gameObject.activeInHierarchy;
		if (!flag)
		{
			bool flag2 = !(this.UiBase as UI_CharacterMenuSubPageBase).CheckState(ViewCharacterMenu.CurSubToggleIndex, ViewCharacterMenu.CurSubSubPageIndex);
			if (!flag2)
			{
				bool flag3 = !base.Ready;
				if (flag3)
				{
					Transform uiBaseTrans = this.UiBase.transform;
					uiBaseTrans.localPosition = Vector3.zero;
					base.TranslateState(EUiElementState.Ready);
					base.SetElementReady(true);
					uiBaseTrans.SetAsLastSibling();
					this.UiBase.PlayAudioIn();
					this.Visible = true;
					UI_CharacterMenuSubPageBase page = base.UiBaseAs<UI_CharacterMenuSubPageBase>();
					bool isActiveAndEnabled = page.CharacterMenu.StackView.isActiveAndEnabled;
					if (isActiveAndEnabled)
					{
						page.CharacterMenu.StackView.transform.SetAsLastSibling();
					}
				}
			}
		}
	}
}
