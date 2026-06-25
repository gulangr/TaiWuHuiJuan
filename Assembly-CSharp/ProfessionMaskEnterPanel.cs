using System;
using TMPro;
using UnityEngine;

// Token: 0x02000403 RID: 1027
public class ProfessionMaskEnterPanel : MonoBehaviour
{
	// Token: 0x1700063B RID: 1595
	// (get) Token: 0x06003D3A RID: 15674 RVA: 0x001ECF76 File Offset: 0x001EB176
	public CRawImage Background
	{
		get
		{
			return this.background;
		}
	}

	// Token: 0x1700063C RID: 1596
	// (get) Token: 0x06003D3B RID: 15675 RVA: 0x001ECF7E File Offset: 0x001EB17E
	public CRawImage Appearance
	{
		get
		{
			return this.appearance;
		}
	}

	// Token: 0x1700063D RID: 1597
	// (get) Token: 0x06003D3C RID: 15676 RVA: 0x001ECF86 File Offset: 0x001EB186
	public TextMeshProUGUI NameLabel
	{
		get
		{
			return this.nameLabel;
		}
	}

	// Token: 0x1700063E RID: 1598
	// (get) Token: 0x06003D3D RID: 15677 RVA: 0x001ECF8E File Offset: 0x001EB18E
	public TextMeshProUGUI DescLabel
	{
		get
		{
			return this.descLabel;
		}
	}

	// Token: 0x1700063F RID: 1599
	// (get) Token: 0x06003D3E RID: 15678 RVA: 0x001ECF96 File Offset: 0x001EB196
	public CImage SkillIcon
	{
		get
		{
			return this.skillIcon;
		}
	}

	// Token: 0x17000640 RID: 1600
	// (get) Token: 0x06003D3F RID: 15679 RVA: 0x001ECF9E File Offset: 0x001EB19E
	public CanvasGroup SkillGroup
	{
		get
		{
			return this.skillGroup;
		}
	}

	// Token: 0x17000641 RID: 1601
	// (get) Token: 0x06003D40 RID: 15680 RVA: 0x001ECFA6 File Offset: 0x001EB1A6
	public RectTransform AppearanceMoveRoot
	{
		get
		{
			return this.appearanceMoveRoot;
		}
	}

	// Token: 0x17000642 RID: 1602
	// (get) Token: 0x06003D41 RID: 15681 RVA: 0x001ECFAE File Offset: 0x001EB1AE
	public RectTransform SkillGroupMoveRoot
	{
		get
		{
			return this.skillGroupMoveRoot;
		}
	}

	// Token: 0x17000643 RID: 1603
	// (get) Token: 0x06003D42 RID: 15682 RVA: 0x001ECFB6 File Offset: 0x001EB1B6
	// (set) Token: 0x06003D43 RID: 15683 RVA: 0x001ECFBE File Offset: 0x001EB1BE
	public Vector3 DefaultAppearancePosition { get; private set; }

	// Token: 0x17000644 RID: 1604
	// (get) Token: 0x06003D44 RID: 15684 RVA: 0x001ECFC7 File Offset: 0x001EB1C7
	// (set) Token: 0x06003D45 RID: 15685 RVA: 0x001ECFCF File Offset: 0x001EB1CF
	public Vector3 DefaultSkillGroupPosition { get; private set; }

	// Token: 0x06003D46 RID: 15686 RVA: 0x001ECFD8 File Offset: 0x001EB1D8
	public void ShowSkillEffect(int index)
	{
		for (int i = 0; i < this.skillEffects.Length; i++)
		{
			bool flag = this.skillEffects[i] != null;
			if (flag)
			{
				this.skillEffects[i].SetActive(i == index);
			}
		}
	}

	// Token: 0x06003D47 RID: 15687 RVA: 0x001ED024 File Offset: 0x001EB224
	public void HideAllSkillEffects()
	{
		for (int i = 0; i < this.skillEffects.Length; i++)
		{
			bool flag = this.skillEffects[i] != null;
			if (flag)
			{
				this.skillEffects[i].SetActive(false);
			}
		}
	}

	// Token: 0x06003D48 RID: 15688 RVA: 0x001ED06C File Offset: 0x001EB26C
	private void Awake()
	{
		this.DefaultAppearancePosition = this.appearance.transform.position;
		this.DefaultSkillGroupPosition = this.skillGroup.transform.position;
	}

	// Token: 0x04002C0D RID: 11277
	[SerializeField]
	private CRawImage background;

	// Token: 0x04002C0E RID: 11278
	[SerializeField]
	private CRawImage appearance;

	// Token: 0x04002C0F RID: 11279
	[SerializeField]
	private TextMeshProUGUI nameLabel;

	// Token: 0x04002C10 RID: 11280
	[SerializeField]
	private TextMeshProUGUI descLabel;

	// Token: 0x04002C11 RID: 11281
	[SerializeField]
	private CImage skillIcon;

	// Token: 0x04002C12 RID: 11282
	[SerializeField]
	private CanvasGroup skillGroup;

	// Token: 0x04002C13 RID: 11283
	[SerializeField]
	private RectTransform appearanceMoveRoot;

	// Token: 0x04002C14 RID: 11284
	[SerializeField]
	private RectTransform skillGroupMoveRoot;

	// Token: 0x04002C15 RID: 11285
	[SerializeField]
	private GameObject[] skillEffects = new GameObject[4];
}
