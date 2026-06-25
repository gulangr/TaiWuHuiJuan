using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A90 RID: 2704
	public class TextLinkBackgroundHandler
	{
		// Token: 0x06008463 RID: 33891 RVA: 0x003D9680 File Offset: 0x003D7880
		public TextLinkBackgroundHandler(RectTransform btnRoot, GameObject fullBtnPrefab, GameObject leftPartBtnPrefab, GameObject rightPartBtnPrefab)
		{
			this._buttonsRootRect = btnRoot;
			this._fullButtonPoolItem = new PoolItem(string.Empty, fullBtnPrefab);
			this._leftPartButtonPoolItem = new PoolItem(string.Empty, leftPartBtnPrefab);
			this._rightPartButtonPoolItem = new PoolItem(string.Empty, rightPartBtnPrefab);
		}

		// Token: 0x06008464 RID: 33892 RVA: 0x003D96DC File Offset: 0x003D78DC
		public void ProcessLinkInfo(TextMeshProUGUI tmpText)
		{
			this._generateButtonComplete = false;
			this._tmpSpriteParseComplete = false;
			this.ClearPrevButtons();
			this._targetTextMeshPro = tmpText;
			TMPTextSpriteHelper spriteHelper = this._targetTextMeshPro.GetComponent<TMPTextSpriteHelper>();
			bool flag = null != spriteHelper;
			if (flag)
			{
				spriteHelper.OnParseComplete = new Action(this.OnTMPSpriteHelperParseComplete);
			}
			else
			{
				this.OnTMPSpriteHelperParseComplete();
			}
		}

		// Token: 0x06008465 RID: 33893 RVA: 0x003D973C File Offset: 0x003D793C
		private void OnTMPSpriteHelperParseComplete()
		{
			this._tmpSpriteParseComplete = true;
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(3U, new Action(this.Analyse));
		}

		// Token: 0x06008466 RID: 33894 RVA: 0x003D9760 File Offset: 0x003D7960
		private void ClearPrevButtons()
		{
			bool flag = null == this._targetTextMeshPro;
			if (!flag)
			{
				CButtonObsolete[] buttons = this._buttonsRootRect.GetComponentsInTopChildren(false);
				for (int i = 0; i < buttons.Length; i++)
				{
					string btnName = buttons[i].name;
					buttons[i].onClick.RemoveAllListeners();
					bool flag2 = btnName.StartsWith("Full_");
					if (flag2)
					{
						this._fullButtonPoolItem.DestroyObject(buttons[i].gameObject);
					}
					else
					{
						bool flag3 = btnName.StartsWith("LPart_");
						if (flag3)
						{
							this._leftPartButtonPoolItem.DestroyObject(buttons[i].gameObject);
						}
						else
						{
							bool flag4 = btnName.StartsWith("RPart_");
							if (flag4)
							{
								this._rightPartButtonPoolItem.DestroyObject(buttons[i].gameObject);
							}
						}
					}
				}
			}
		}

		// Token: 0x06008467 RID: 33895 RVA: 0x003D9836 File Offset: 0x003D7A36
		private void Analyse()
		{
			this.GenerateAllButton();
		}

		// Token: 0x06008468 RID: 33896 RVA: 0x003D9840 File Offset: 0x003D7A40
		private CButtonObsolete GenerateSingleButton(PoolItem poolItem, TMP_CharacterInfo charInfoA, TMP_CharacterInfo charInfoB, int length)
		{
			GameObject btnObj = poolItem.GetObject();
			btnObj.transform.SetParent(this._buttonsRootRect, false);
			int lineNum = charInfoA.lineNumber;
			bool flag = length == 1;
			float btnWidth;
			float x;
			float y;
			if (flag)
			{
				btnWidth = charInfoA.topRight.x - charInfoA.topLeft.x + this._increaseWidth;
				x = (charInfoA.bottomLeft.x + charInfoA.topRight.x) * 0.5f;
				y = (charInfoA.bottomLeft.y + charInfoA.topRight.y) * 0.5f;
			}
			else
			{
				btnWidth = charInfoB.topRight.x - charInfoA.topLeft.x + this._increaseWidth;
				x = (charInfoA.bottomLeft.x + charInfoB.topRight.x) * 0.5f;
				y = (charInfoA.bottomLeft.y + charInfoB.topRight.y) * 0.5f;
			}
			((RectTransform)btnObj.transform).SetWidth(btnWidth);
			((RectTransform)btnObj.transform).anchoredPosition = new Vector2(x, y);
			return btnObj.GetComponent<CButtonObsolete>();
		}

		// Token: 0x06008469 RID: 33897 RVA: 0x003D9984 File Offset: 0x003D7B84
		private void GenerateAllButton()
		{
			bool flag = !this._tmpSpriteParseComplete || this._generateButtonComplete;
			if (!flag)
			{
				for (int i = 0; i < this._targetTextMeshPro.textInfo.linkCount; i++)
				{
					TMP_LinkInfo linkInfo = this._targetTextMeshPro.textInfo.linkInfo[i];
					string id = linkInfo.GetLinkID();
					int linkStartIndex = linkInfo.linkTextfirstCharacterIndex;
					int linkMaxIndex = linkInfo.linkTextLength - 1;
					TMP_CharacterInfo charInfoStart = this._targetTextMeshPro.textInfo.characterInfo[linkStartIndex];
					TMP_CharacterInfo charInfoEnd = this._targetTextMeshPro.textInfo.characterInfo[linkStartIndex + linkMaxIndex];
					while (charInfoStart.character == ' ')
					{
						linkStartIndex++;
						charInfoStart = this._targetTextMeshPro.textInfo.characterInfo[linkStartIndex];
					}
					while (charInfoEnd.character == ' ')
					{
						linkMaxIndex--;
						charInfoEnd = this._targetTextMeshPro.textInfo.characterInfo[linkStartIndex + linkMaxIndex];
					}
					CButtonObsolete buttonA = null;
					CButtonObsolete buttonB = null;
					bool flag2 = charInfoStart.lineNumber == charInfoEnd.lineNumber;
					if (flag2)
					{
						buttonA = this.GenerateSingleButton(this._fullButtonPoolItem, charInfoStart, charInfoEnd, linkMaxIndex + 1);
						buttonA.name = "Full_" + id;
					}
					else
					{
						TMP_CharacterInfo preCharacterInfo = charInfoStart;
						for (int j = 0; j <= linkMaxIndex; j++)
						{
							int mid = j;
							TMP_CharacterInfo charInfo = this._targetTextMeshPro.textInfo.characterInfo[linkStartIndex + mid];
							bool flag3 = charInfo.lineNumber != charInfoStart.lineNumber;
							if (flag3)
							{
								buttonA = this.GenerateSingleButton(this._leftPartButtonPoolItem, charInfoStart, preCharacterInfo, mid);
								buttonB = this.GenerateSingleButton(this._rightPartButtonPoolItem, charInfo, charInfoEnd, linkMaxIndex - mid + 1);
								break;
							}
							preCharacterInfo = charInfo;
						}
						bool flag4 = null != buttonA;
						if (flag4)
						{
							buttonA.name = "LPart_" + id;
						}
						bool flag5 = null != buttonB;
						if (flag5)
						{
							buttonB.name = "RPart_" + id;
						}
					}
				}
				this._generateButtonComplete = true;
			}
		}

		// Token: 0x04006572 RID: 25970
		private PoolItem _fullButtonPoolItem;

		// Token: 0x04006573 RID: 25971
		private PoolItem _leftPartButtonPoolItem;

		// Token: 0x04006574 RID: 25972
		private PoolItem _rightPartButtonPoolItem;

		// Token: 0x04006575 RID: 25973
		private RectTransform _buttonsRootRect;

		// Token: 0x04006576 RID: 25974
		private TextMeshProUGUI _targetTextMeshPro;

		// Token: 0x04006577 RID: 25975
		private bool _generateButtonComplete;

		// Token: 0x04006578 RID: 25976
		private bool _tmpSpriteParseComplete;

		// Token: 0x04006579 RID: 25977
		public Action<int> OnCharacterNameClicked;

		// Token: 0x0400657A RID: 25978
		private const string FullBtnNamePre = "Full_";

		// Token: 0x0400657B RID: 25979
		private const string LeftPartBtnNamePre = "LPart_";

		// Token: 0x0400657C RID: 25980
		private const string RightPartBtnNamePre = "RPart_";

		// Token: 0x0400657D RID: 25981
		private float _increaseWidth = 5f;
	}
}
