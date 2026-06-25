using System;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Common
{
	// Token: 0x02000F8C RID: 3980
	public class CommonPageReadStates : MonoBehaviour
	{
		// Token: 0x0600B71A RID: 46874 RVA: 0x00537304 File Offset: 0x00535504
		public void Refresh(LifeSkillItem data, sbyte[] readingProgress)
		{
			this.SwitchCountMode(CommonPageReadStates.CountMode.Five);
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

		// Token: 0x0600B71B RID: 46875 RVA: 0x005373A0 File Offset: 0x005355A0
		public void Refresh(CombatSkillDisplayData data)
		{
			this.SwitchCountMode(CommonPageReadStates.CountMode.Five);
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

		// Token: 0x0600B71C RID: 46876 RVA: 0x00537544 File Offset: 0x00535744
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
				CommonPageReadStates.CountMode mode = isCombatBook ? CommonPageReadStates.CountMode.Six : CommonPageReadStates.CountMode.Five;
				this.SwitchCountMode(mode);
				int count = CommonPageReadStates.GetMaxCount(mode);
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

		// Token: 0x0600B71D RID: 46877 RVA: 0x005376D8 File Offset: 0x005358D8
		public void Refresh(sbyte[] readingProgress, sbyte[] pageType, bool isCombatBook, bool isBrokenOut)
		{
			CommonPageReadStates.CountMode mode = isCombatBook ? CommonPageReadStates.CountMode.Six : CommonPageReadStates.CountMode.Five;
			this.SwitchCountMode(mode);
			int count = CommonPageReadStates.GetMaxCount(mode);
			for (int i = 0; i < count; i++)
			{
				CImage icon = base.transform.GetChild(i).GetComponent<CImage>();
				bool flag = readingProgress == null || i >= readingProgress.Length;
				if (flag)
				{
					icon.sprite = this.stateSprites[0];
				}
				else if (isCombatBook)
				{
					bool flag2 = i == 0;
					if (flag2)
					{
						icon.sprite = ((readingProgress[i] == 100) ? this.stateSprites[1] : this.stateSprites[0]);
					}
					else
					{
						bool flag3 = isBrokenOut && pageType != null && pageType.CheckIndex(i);
						if (flag3)
						{
							Sprite ballSprite = (pageType[i] == 0) ? this.stateSprites[2] : this.stateSprites[3];
							icon.sprite = ((readingProgress[i] == 100) ? ballSprite : this.stateSprites[0]);
						}
						else
						{
							icon.sprite = ((readingProgress[i] == 100) ? this.stateSprites[1] : this.stateSprites[0]);
						}
					}
				}
				else
				{
					icon.sprite = ((readingProgress[i] == 100) ? this.stateSprites[1] : this.stateSprites[0]);
				}
			}
		}

		// Token: 0x0600B71E RID: 46878 RVA: 0x00537824 File Offset: 0x00535A24
		private void SwitchCountMode(CommonPageReadStates.CountMode mode)
		{
			int objCount = base.transform.childCount;
			int maxCount = CommonPageReadStates.GetMaxCount(mode);
			byte i = 0;
			while ((int)i < objCount)
			{
				Transform obj = base.transform.GetChild((int)i);
				obj.gameObject.SetActive((int)i < maxCount);
				i += 1;
			}
			base.GetComponent<HorizontalLayoutGroup>().spacing = (float)this.GetSpacing(mode);
		}

		// Token: 0x0600B71F RID: 46879 RVA: 0x0053788C File Offset: 0x00535A8C
		private static int GetMaxCount(CommonPageReadStates.CountMode mode)
		{
			return (mode == CommonPageReadStates.CountMode.Five) ? 5 : 6;
		}

		// Token: 0x0600B720 RID: 46880 RVA: 0x005378A8 File Offset: 0x00535AA8
		private int GetSpacing(CommonPageReadStates.CountMode mode)
		{
			return (mode == CommonPageReadStates.CountMode.Five) ? this.SixModeSpacing : this.FiveModeSpacing;
		}

		// Token: 0x04008E35 RID: 36405
		[SerializeField]
		[Header("页面状态Sprite引用")]
		private Sprite[] stateSprites = new Sprite[4];

		// Token: 0x04008E36 RID: 36406
		private const int FiveModeMaxCount = 5;

		// Token: 0x04008E37 RID: 36407
		private const int SixModeMaxCount = 6;

		// Token: 0x04008E38 RID: 36408
		[SerializeField]
		private int FiveModeSpacing = 6;

		// Token: 0x04008E39 RID: 36409
		[SerializeField]
		private int SixModeSpacing = 1;

		// Token: 0x04008E3A RID: 36410
		private CImage[] _cachedPageImages;

		// Token: 0x020025D4 RID: 9684
		private enum CountMode
		{
			// Token: 0x0400E943 RID: 59715
			Five,
			// Token: 0x0400E944 RID: 59716
			Six
		}
	}
}
