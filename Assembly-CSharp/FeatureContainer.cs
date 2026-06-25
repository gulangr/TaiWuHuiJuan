using System;
using Game.Components.Character;
using TMPro;
using UnityEngine;

// Token: 0x020003F9 RID: 1017
public class FeatureContainer : MonoBehaviour
{
	// Token: 0x06003D03 RID: 15619 RVA: 0x001EAD9C File Offset: 0x001E8F9C
	public void Set(short featureId, int charId = -1)
	{
		this.featureLess.SetActive(false);
		this.featureMore.SetActive(false);
		this.featObj.Set(featureId, charId, false, -1);
		this.feature.SetActive(true);
	}

	// Token: 0x06003D04 RID: 15620 RVA: 0x001EADD6 File Offset: 0x001E8FD6
	public void SetLess()
	{
		this.feature.SetActive(false);
		this.featureMore.SetActive(false);
		this.featureLess.SetActive(true);
	}

	// Token: 0x06003D05 RID: 15621 RVA: 0x001EAE00 File Offset: 0x001E9000
	public void SetMore(int count)
	{
		this.moreText.text = LanguageKey.LK_MouseTipCharacterComplete_MoreFeature_Label.TrFormat(count).ColorReplace();
		this.featureLess.SetActive(false);
		this.feature.SetActive(false);
		this.featureMore.SetActive(true);
	}

	// Token: 0x04002BB5 RID: 11189
	[SerializeField]
	private GameObject featureLess;

	// Token: 0x04002BB6 RID: 11190
	[SerializeField]
	private GameObject feature;

	// Token: 0x04002BB7 RID: 11191
	[SerializeField]
	private GameObject featureMore;

	// Token: 0x04002BB8 RID: 11192
	[SerializeField]
	private Feature featObj;

	// Token: 0x04002BB9 RID: 11193
	[SerializeField]
	private TMP_Text moreText;
}
