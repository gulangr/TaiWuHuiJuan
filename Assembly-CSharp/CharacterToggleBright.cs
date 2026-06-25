using System;
using UnityEngine;

// Token: 0x020001C6 RID: 454
[RequireComponent(typeof(CommonCharacterToggle))]
public class CharacterToggleBright : MonoBehaviour, ILanguage
{
	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x06001C0C RID: 7180 RVA: 0x000C1FBD File Offset: 0x000C01BD
	public CommonCharacterToggle CharacterToggle
	{
		get
		{
			return this.charToggle;
		}
	}

	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x06001C0D RID: 7181 RVA: 0x000C1FC5 File Offset: 0x000C01C5
	// (set) Token: 0x06001C0E RID: 7182 RVA: 0x000C1FD0 File Offset: 0x000C01D0
	public bool Interactable
	{
		get
		{
			return this.interactable;
		}
		set
		{
			CToggleObsolete ctoggleObsolete = this.charToggle;
			this.interactable = value;
			ctoggleObsolete.interactable = (value && !this.unknown && this.characterId != -1);
		}
	}

	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x06001C0F RID: 7183 RVA: 0x000C200C File Offset: 0x000C020C
	// (set) Token: 0x06001C10 RID: 7184 RVA: 0x000C2014 File Offset: 0x000C0214
	public bool Disable
	{
		get
		{
			return this.disable;
		}
		set
		{
			this.disable = value;
			if (value)
			{
				this.CharacterToggle.interactable = false;
			}
			TooltipInvoker displayer = base.GetComponent<TooltipInvoker>();
			bool flag = displayer != null;
			if (flag)
			{
				displayer.enabled = !this.disable;
			}
			PointerTrigger trigger = base.GetComponent<PointerTrigger>();
			bool flag2 = trigger != null;
			if (flag2)
			{
				trigger.enabled = !this.disable;
			}
			this.Refresh();
			bool flag3 = this.root;
			if (flag3)
			{
				bool flag4 = this.root.enabled = this.disable;
				if (flag4)
				{
					this.root.SetDefaultGrayAndBlack();
				}
			}
		}
	}

	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x06001C11 RID: 7185 RVA: 0x000C20BE File Offset: 0x000C02BE
	// (set) Token: 0x06001C12 RID: 7186 RVA: 0x000C20C8 File Offset: 0x000C02C8
	public bool Unknown
	{
		get
		{
			return this.unknown;
		}
		set
		{
			GameObject gameObject = this.unknownObj;
			this.unknown = value;
			gameObject.SetActive(value);
			this.charToggle.CharacterId = (this.unknown ? -1 : this.characterId);
			this.Interactable = this.interactable;
			this.maskTransform.sizeDelta = ((this.charToggle.interactable || (this.disable && this.characterId != -1)) ? this.knownDelta : this.unknownDelta);
			bool flag = this.unknown;
			if (flag)
			{
				this.charToggle.NameLabel.text = LanguageKey.LK_Character_Name_NotMetYet.Tr();
			}
		}
	}

	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x06001C13 RID: 7187 RVA: 0x000C2173 File Offset: 0x000C0373
	// (set) Token: 0x06001C14 RID: 7188 RVA: 0x000C217B File Offset: 0x000C037B
	public int CharacterId
	{
		get
		{
			return this.characterId;
		}
		set
		{
			this.characterId = value;
			this.charToggle.CharacterId = (this.unknown ? -1 : value);
		}
	}

	// Token: 0x06001C15 RID: 7189 RVA: 0x000C219D File Offset: 0x000C039D
	public void Refresh()
	{
		this.Unknown = this.unknown;
	}

	// Token: 0x06001C16 RID: 7190 RVA: 0x000C21AC File Offset: 0x000C03AC
	public void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		RectTransform rectTransform = this.avatar;
		if (!true)
		{
		}
		Vector2 anchoredPosition;
		if (languageType != LocalStringManager.LanguageType.CN)
		{
			if (languageType != LocalStringManager.LanguageType.EN)
			{
				anchoredPosition = this.avatar.anchoredPosition;
			}
			else
			{
				anchoredPosition = new Vector2(0f, this.avatarEnY);
			}
		}
		else
		{
			anchoredPosition = new Vector2(0f, this.avatarCnY);
		}
		if (!true)
		{
		}
		rectTransform.anchoredPosition = anchoredPosition;
		this.characterFrame.OnLanguageChange(languageType);
	}

	// Token: 0x040015DC RID: 5596
	[SerializeField]
	private HSVStyleRoot root;

	// Token: 0x040015DD RID: 5597
	[SerializeField]
	private RectTransform maskTransform;

	// Token: 0x040015DE RID: 5598
	[SerializeField]
	private RectTransform avatar;

	// Token: 0x040015DF RID: 5599
	[SerializeField]
	private CommonCharacterToggle charToggle;

	// Token: 0x040015E0 RID: 5600
	[SerializeField]
	private GameObject unknownObj;

	// Token: 0x040015E1 RID: 5601
	[SerializeField]
	private Vector2 unknownDelta;

	// Token: 0x040015E2 RID: 5602
	[SerializeField]
	private Vector2 knownDelta;

	// Token: 0x040015E3 RID: 5603
	[SerializeField]
	private CommonCharacterNameFrame characterFrame;

	// Token: 0x040015E4 RID: 5604
	[SerializeField]
	private bool unknown = true;

	// Token: 0x040015E5 RID: 5605
	[SerializeField]
	private bool interactable = true;

	// Token: 0x040015E6 RID: 5606
	[SerializeField]
	private bool disable = false;

	// Token: 0x040015E7 RID: 5607
	[SerializeField]
	private int characterId = -1;

	// Token: 0x040015E8 RID: 5608
	[SerializeField]
	private float avatarCnY;

	// Token: 0x040015E9 RID: 5609
	[SerializeField]
	private float avatarEnY;
}
