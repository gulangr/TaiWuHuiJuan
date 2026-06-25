using System;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Global;
using GameData.Domains.Global.Inscription;
using TMPro;
using UnityEngine;

// Token: 0x02000221 RID: 545
public class InscribedCharView : Refers
{
	// Token: 0x17000365 RID: 869
	// (get) Token: 0x060022B7 RID: 8887 RVA: 0x00100CA9 File Offset: 0x000FEEA9
	// (set) Token: 0x060022B8 RID: 8888 RVA: 0x00100CB1 File Offset: 0x000FEEB1
	public InscribedCharacter Data { get; private set; }

	// Token: 0x17000366 RID: 870
	// (get) Token: 0x060022B9 RID: 8889 RVA: 0x00100CBA File Offset: 0x000FEEBA
	// (set) Token: 0x060022BA RID: 8890 RVA: 0x00100CC2 File Offset: 0x000FEEC2
	public InscribedCharacterKey Key { get; private set; }

	// Token: 0x060022BB RID: 8891 RVA: 0x00100CCC File Offset: 0x000FEECC
	public void Refresh(InscribedCharacterKey key, InscribedCharacter data, bool selected, bool included, bool canDelete, bool canInclude)
	{
		this.Key = key;
		this.Data = data;
		this.Empty.SetActive(key.Equals(InscribedCharacterKey.Invalid));
		bool flag = key.Equals(InscribedCharacterKey.Invalid);
		if (flag)
		{
			this.Name.text = LocalStringManager.Get(LanguageKey.LK_None);
			this.AvatarCom.ResetToBlank(false);
			this.DeleteButton.gameObject.SetActive(false);
			this.IncludeToggle.gameObject.SetActive(false);
		}
		else
		{
			data.Avatar.ClothDisplayId = (short)((byte)data.ClothingDisplayId);
			this.AvatarCom.Refresh(data.Avatar, data.CurrAge);
			this.Name.text = data.Surname + data.GivenName;
			this.DeleteButton.gameObject.SetActive(canDelete);
			if (canDelete)
			{
				this.DeleteButton.ClearAndAddListener(new Action(this.OnClickDeleteDialogCmd));
			}
			this.IncludeToggle.gameObject.SetActive(canInclude);
			if (canInclude)
			{
				this.IncludeToggle.onValueChanged.RemoveAllListeners();
				this.IncludeToggle.isOn = included;
				GameObject includeObj = this.IncludeToggle.transform.Find("Include").gameObject;
				includeObj.SetActive(included);
				this.IncludeToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					includeObj.SetActive(isOn);
					Action<bool> onIncludeChange = this.OnIncludeChange;
					if (onIncludeChange != null)
					{
						onIncludeChange(isOn);
					}
				});
			}
		}
		this.SetSelected(selected);
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x00100E74 File Offset: 0x000FF074
	public void SetClickEvent(Action<InscribedCharacterKey, InscribedCharacter> action)
	{
		this.ClickButton.ClearAndAddListener(delegate
		{
			action(this.Key, this.Data);
		});
	}

	// Token: 0x060022BD RID: 8893 RVA: 0x00100EB0 File Offset: 0x000FF0B0
	public void SetSelected(bool selected)
	{
		this.SelectedBorder.SetActive(selected);
		if (selected)
		{
			base.transform.Find("Hover").gameObject.SetActive(false);
		}
		bool flag = this.styleRoot == null;
		if (!flag)
		{
			if (selected)
			{
				this.styleRoot.SetDefault();
			}
			else
			{
				this.styleRoot.SetDefaultBlack();
			}
		}
	}

	// Token: 0x060022BE RID: 8894 RVA: 0x00100F20 File Offset: 0x000FF120
	private void OnClickDeleteDialogCmd()
	{
		this._delteInscriptionCharacterDialogCmd.Title = LocalStringManager.Get(LanguageKey.UI_NewGame_Tip_Delete_Mingke_Character);
		this._delteInscriptionCharacterDialogCmd.Content = LocalStringManager.Get(LanguageKey.UI_NewGame_Tip_Delete_Mingke_Character_Confirm);
		this._delteInscriptionCharacterDialogCmd.Yes = delegate()
		{
			this.DeleteCharacter();
		};
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", this._delteInscriptionCharacterDialogCmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x060022BF RID: 8895 RVA: 0x00100FA0 File Offset: 0x000FF1A0
	private void DeleteCharacter()
	{
		base.GetComponentInChildren<CButtonObsolete>().interactable = false;
		GlobalDomainMethod.Call.RemoveInscribedCharacter(this.Key);
		bool flag = this.OnClickRemoveCallback != null;
		if (flag)
		{
			this.OnClickRemoveCallback();
			this.OnClickRemoveCallback = null;
		}
		GEvent.OnEvent(EEvents.InscriptionCharacterRemove, EasyPool.Get<ArgumentBox>().SetObject("CharacterKey", this.Key));
	}

	// Token: 0x04001AA4 RID: 6820
	public GameObject SelectedBorder;

	// Token: 0x04001AA5 RID: 6821
	public HSVStyleRoot styleRoot;

	// Token: 0x04001AA8 RID: 6824
	public CButtonObsolete ClickButton;

	// Token: 0x04001AA9 RID: 6825
	public CButtonObsolete DeleteButton;

	// Token: 0x04001AAA RID: 6826
	public GameObject Empty;

	// Token: 0x04001AAB RID: 6827
	public Game.Components.Avatar.Avatar AvatarCom;

	// Token: 0x04001AAC RID: 6828
	public TextMeshProUGUI Name;

	// Token: 0x04001AAD RID: 6829
	public CToggleObsolete IncludeToggle;

	// Token: 0x04001AAE RID: 6830
	[NonSerialized]
	public Action OnClickRemoveCallback = null;

	// Token: 0x04001AAF RID: 6831
	[NonSerialized]
	public Action<bool> OnIncludeChange = null;

	// Token: 0x04001AB0 RID: 6832
	private readonly DialogCmd _delteInscriptionCharacterDialogCmd = new DialogCmd();
}
