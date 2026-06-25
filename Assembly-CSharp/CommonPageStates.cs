using System;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000226 RID: 550
public class CommonPageStates : Refers
{
	// Token: 0x0600230F RID: 8975 RVA: 0x00102780 File Offset: 0x00100980
	public void Refresh(LifeSkillItem data, sbyte[] readingProgress)
	{
		this.SwitchCountMode(CommonPageStates.CountMode.Five);
		int pageCount = 5;
		int objCount = base.transform.childCount;
		byte i = 0;
		while ((int)i < objCount)
		{
			Transform obj = base.transform.GetChild((int)i);
			bool flag = (int)i >= pageCount;
			if (flag)
			{
				obj.gameObject.SetActive(false);
			}
			else
			{
				obj.GetComponent<CImage>().sprite = (data.IsPageRead(i) ? this.stateSprites[1] : this.stateSprites[0]);
				obj.gameObject.SetActive(true);
			}
			i += 1;
		}
	}

	// Token: 0x06002310 RID: 8976 RVA: 0x0010281C File Offset: 0x00100A1C
	public void Refresh(CombatSkillDisplayData data)
	{
		this.SwitchCountMode(CommonPageStates.CountMode.Five);
		int objCount = 6;
		bool flag = this._cachedPageImages == null || this._cachedPageImages.Length != objCount;
		if (flag)
		{
			this._cachedPageImages = new CImage[objCount];
			byte i = 0;
			while ((int)i < objCount)
			{
				this._cachedPageImages[(int)i] = base.transform.GetChild((int)i).GetComponent<CImage>();
				i += 1;
			}
		}
		bool brokenOut = CombatSkillStateHelper.IsBrokenOut(data.ActivationState);
		bool flag2 = brokenOut;
		if (flag2)
		{
			byte j = 1;
			while ((int)j <= objCount)
			{
				sbyte pageActiveDirection = CombatSkillStateHelper.GetPageActiveDirection(data.ActivationState, j);
				if (!true)
				{
				}
				int num;
				if (pageActiveDirection != 0)
				{
					if (pageActiveDirection != 1)
					{
						num = 1;
					}
					else
					{
						num = 3;
					}
				}
				else
				{
					num = 2;
				}
				if (!true)
				{
				}
				int spriteIndex = num;
				this._cachedPageImages[(int)(j - 1)].sprite = this.stateSprites[spriteIndex];
				this._cachedPageImages[(int)(j - 1)].gameObject.SetActive(true);
				j += 1;
			}
		}
		else
		{
			byte k = 1;
			while ((int)k <= objCount)
			{
				bool isDirectRead = CombatSkillStateHelper.IsPageRead(data.ReadingState, CombatSkillStateHelper.GetNormalPageInternalIndex(0, k));
				bool isReversRead = CombatSkillStateHelper.IsPageRead(data.ReadingState, CombatSkillStateHelper.GetNormalPageInternalIndex(1, k));
				bool isRead = isDirectRead || isReversRead;
				this._cachedPageImages[(int)(k - 1)].sprite = (isRead ? this.stateSprites[1] : this.stateSprites[0]);
				this._cachedPageImages[(int)(k - 1)].gameObject.SetActive(true);
				k += 1;
			}
		}
	}

	// Token: 0x06002311 RID: 8977 RVA: 0x001029C0 File Offset: 0x00100BC0
	public void Refresh(ItemKey currentReadingBookKey, SkillBookPageDisplayData pagesInfo)
	{
		bool flag = !currentReadingBookKey.IsValid() || pagesInfo == null;
		if (flag)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).GetComponent<CImage>().sprite = this.stateSprites[0];
			}
		}
		else
		{
			bool isCombatBook = pagesInfo.IsCombatBook;
			CommonPageStates.CountMode mode = isCombatBook ? CommonPageStates.CountMode.Six : CommonPageStates.CountMode.Five;
			this.SwitchCountMode(mode);
			int count = CommonPageStates.GetMaxCount(mode);
			for (int j = 0; j < count; j++)
			{
				CImage icon = base.transform.GetChild(j).GetComponent<CImage>();
				bool flag2 = j >= pagesInfo.ReadingProgress.Length;
				if (flag2)
				{
					icon.sprite = this.stateSprites[0];
				}
				else
				{
					bool flag3 = isCombatBook;
					if (flag3)
					{
						bool flag4 = j == 0;
						if (flag4)
						{
							icon.sprite = ((pagesInfo.ReadingProgress[j] == 100) ? this.stateSprites[1] : this.stateSprites[0]);
						}
						else
						{
							Sprite ballSprite = (pagesInfo.Type[j] == 0) ? this.stateSprites[2] : this.stateSprites[3];
							icon.sprite = ((pagesInfo.ReadingProgress[j] == 100) ? ballSprite : this.stateSprites[0]);
						}
					}
					else
					{
						icon.sprite = ((pagesInfo.ReadingProgress[j] == 100) ? this.stateSprites[1] : this.stateSprites[0]);
					}
				}
			}
		}
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x00102B54 File Offset: 0x00100D54
	private void SwitchCountMode(CommonPageStates.CountMode mode)
	{
		int objCount = base.transform.childCount;
		int maxCount = CommonPageStates.GetMaxCount(mode);
		byte i = 0;
		while ((int)i < objCount)
		{
			Transform obj = base.transform.GetChild((int)i);
			obj.gameObject.SetActive((int)i < maxCount);
			i += 1;
		}
		base.GetComponent<HorizontalLayoutGroup>().spacing = (float)this.GetSpacing(mode);
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x00102BBC File Offset: 0x00100DBC
	private static int GetMaxCount(CommonPageStates.CountMode mode)
	{
		return (mode == CommonPageStates.CountMode.Five) ? 5 : 6;
	}

	// Token: 0x06002314 RID: 8980 RVA: 0x00102BD8 File Offset: 0x00100DD8
	private int GetSpacing(CommonPageStates.CountMode mode)
	{
		return (mode == CommonPageStates.CountMode.Five) ? this.SixModeSpacing : this.FiveModeSpacing;
	}

	// Token: 0x04001AE0 RID: 6880
	[SerializeField]
	[Header("页面状态Sprite引用")]
	private Sprite[] stateSprites = new Sprite[4];

	// Token: 0x04001AE1 RID: 6881
	private const int FiveModeMaxCount = 5;

	// Token: 0x04001AE2 RID: 6882
	private const int SixModeMaxCount = 6;

	// Token: 0x04001AE3 RID: 6883
	[SerializeField]
	private int FiveModeSpacing = 6;

	// Token: 0x04001AE4 RID: 6884
	[SerializeField]
	private int SixModeSpacing = 1;

	// Token: 0x04001AE5 RID: 6885
	private CImage[] _cachedPageImages;

	// Token: 0x020014FF RID: 5375
	private enum CountMode
	{
		// Token: 0x0400A337 RID: 41783
		Five,
		// Token: 0x0400A338 RID: 41784
		Six
	}
}
