using System;
using System.Collections.Generic;
using Config;
using Game.Components.Avatar;
using TMPro;
using UnityEngine;

// Token: 0x02000203 RID: 515
public class DebugAvatarView : MonoBehaviour
{
	// Token: 0x04001968 RID: 6504
	[SerializeField]
	private GameObject page1;

	// Token: 0x04001969 RID: 6505
	[SerializeField]
	private GameObject page2;

	// Token: 0x0400196A RID: 6506
	[SerializeField]
	private CButtonObsolete page1Button;

	// Token: 0x0400196B RID: 6507
	[SerializeField]
	private CButtonObsolete page2Button;

	// Token: 0x0400196C RID: 6508
	[SerializeField]
	private CButtonObsolete closeButton;

	// Token: 0x0400196D RID: 6509
	[SerializeField]
	private CDropdownLegacy lockCharmLevelDropdown;

	// Token: 0x0400196E RID: 6510
	[SerializeField]
	private CSliderLegacy ageSlider;

	// Token: 0x0400196F RID: 6511
	[SerializeField]
	private TextMeshProUGUI ageValueText;

	// Token: 0x04001970 RID: 6512
	[SerializeField]
	private CDropdownLegacy beard1Dropdown;

	// Token: 0x04001971 RID: 6513
	[SerializeField]
	private CDropdownLegacy beard2Dropdown;

	// Token: 0x04001972 RID: 6514
	private List<AvatarElementsItem> _allBeard1Items;

	// Token: 0x04001973 RID: 6515
	private List<AvatarElementsItem> _allBeard2Items;

	// Token: 0x04001974 RID: 6516
	[SerializeField]
	private CButtonObsolete forceRandomButton;

	// Token: 0x04001975 RID: 6517
	[SerializeField]
	private CButtonObsolete conditionRandomButton;

	// Token: 0x04001976 RID: 6518
	[SerializeField]
	private AvatarAdjustController controller;

	// Token: 0x04001977 RID: 6519
	[SerializeField]
	private Refers genderAdjust;

	// Token: 0x04001978 RID: 6520
	[SerializeField]
	private Refers bodyTypeAdjust;

	// Token: 0x04001979 RID: 6521
	[SerializeField]
	private CToggleObsolete bodyTypeLockToggle;

	// Token: 0x0400197A RID: 6522
	[SerializeField]
	private Game.Components.Avatar.Avatar[] previewAvatars;

	// Token: 0x0400197B RID: 6523
	[SerializeField]
	private DebugAvatarCharm debugAvatarCharm;

	// Token: 0x0400197C RID: 6524
	[SerializeField]
	private DebugAvatarDataField debugAvatarDataField;

	// Token: 0x0400197D RID: 6525
	[SerializeField]
	private RectTransform batchAvatarRoot;
}
