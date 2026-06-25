using System;
using System.Collections;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Display;
using GameData.Domains.LifeRecord;
using TMPro;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F54 RID: 3924
	public abstract class NameButtonAnalyzerBase : MonoBehaviour
	{
		// Token: 0x17001464 RID: 5220
		// (get) Token: 0x0600B3D1 RID: 46033
		public abstract string FullBtnKey { get; }

		// Token: 0x17001465 RID: 5221
		// (get) Token: 0x0600B3D2 RID: 46034
		public abstract string LeftBtnKey { get; }

		// Token: 0x17001466 RID: 5222
		// (get) Token: 0x0600B3D3 RID: 46035
		public abstract string RightBtnKey { get; }

		// Token: 0x0600B3D4 RID: 46036 RVA: 0x0051D4BC File Offset: 0x0051B6BC
		public void Set(TransferableRecordDataBase collection, params TMP_Text[] texts)
		{
			bool flag = !base.gameObject.activeInHierarchy;
			if (!flag)
			{
				base.StopAllCoroutines();
				this.Clear();
				foreach (TMP_Text text in texts)
				{
					base.StartCoroutine(this.GenerateButton(text, collection));
				}
			}
		}

		// Token: 0x0600B3D5 RID: 46037 RVA: 0x0051D510 File Offset: 0x0051B710
		public IEnumerator GenerateButton(TMP_Text text, TransferableRecordDataBase collection)
		{
			yield return new WaitForEndOfFrame();
			int num;
			for (int i = 0; i < text.textInfo.linkCount; i = num + 1)
			{
				TMP_LinkInfo linkInfo = text.textInfo.linkInfo[i];
				string id = linkInfo.GetLinkID();
				bool flag = !id.StartsWith("character_");
				if (!flag)
				{
					string text2 = id;
					int charId;
					bool flag2 = int.TryParse(text2.Substring(10, text2.Length - 10), out charId);
					if (flag2)
					{
						this.GenerateButton(collection, charId, text, linkInfo, text.textInfo.characterInfo);
					}
					linkInfo = default(TMP_LinkInfo);
					id = null;
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x0600B3D6 RID: 46038 RVA: 0x0051D530 File Offset: 0x0051B730
		public void GenerateButton(TransferableRecordDataBase collection, int charId, TMP_Text parent, TMP_LinkInfo linkInfo, TMP_CharacterInfo[] characterInfo)
		{
			NameAndLifeRelatedData charName;
			bool flag = !collection.CharNames.TryGetValue(charId, out charName);
			if (flag)
			{
				charId = -1;
			}
			int linkStartIndex = linkInfo.linkTextfirstCharacterIndex;
			int linkLength = linkInfo.linkTextLength - 1;
			TMP_CharacterInfo charInfoStart = characterInfo[linkStartIndex];
			TMP_CharacterInfo charInfoEnd = characterInfo[linkStartIndex + linkLength];
			while (charInfoStart.character == ' ')
			{
				linkStartIndex++;
				charInfoStart = characterInfo[linkStartIndex];
			}
			while (charInfoEnd.character == ' ')
			{
				linkLength--;
				charInfoEnd = characterInfo[linkStartIndex + linkLength];
			}
			bool flag2 = charInfoStart.lineNumber == charInfoEnd.lineNumber;
			if (flag2)
			{
				CButton button = this.GenerateSingleButton(this.FullBtnKey, charId, charName, parent, charInfoStart, charInfoEnd);
				button.name = string.Format("{0}", charId);
				this.FullBtn.Add(button.gameObject);
			}
			else
			{
				bool leftPart = true;
				TMP_CharacterInfo prevCharInfo = charInfoStart;
				for (int i = 1; i < linkLength; i++)
				{
					TMP_CharacterInfo charInfo = characterInfo[linkStartIndex + i];
					bool flag3 = charInfo.lineNumber != charInfoStart.lineNumber;
					if (flag3)
					{
						CButton btn = this.GenerateSingleButton(leftPart ? this.LeftBtnKey : this.RightBtnKey, charId, charName, parent, charInfoStart, prevCharInfo);
						bool flag4 = leftPart;
						if (flag4)
						{
							btn.name = string.Format("{0}", charId);
							this.LeftBtn.Add(btn.gameObject);
						}
						else
						{
							btn.name = string.Format("{0}", charId);
							this.RightBtn.Add(btn.gameObject);
						}
						leftPart = false;
						charInfoStart = charInfo;
					}
					prevCharInfo = charInfo;
				}
			}
		}

		// Token: 0x0600B3D7 RID: 46039 RVA: 0x0051D700 File Offset: 0x0051B900
		public CButton GenerateSingleButton(string btnName, int charId, NameAndLifeRelatedData charName, TMP_Text parent, TMP_CharacterInfo charInfoA, TMP_CharacterInfo charInfoB)
		{
			NameButton btn = PoolManager.GetObject<NameButton>(btnName);
			btn.Set(charId, charName);
			int lineNum = charInfoA.lineNumber;
			float btnWidth = charInfoB.topRight.x - charInfoA.topLeft.x + 15f;
			btn.rectTransform.SetWidth(btnWidth);
			HorizontalAlignmentOptions horizontalAlignment = parent.horizontalAlignment;
			if (!true)
			{
			}
			float num;
			switch (horizontalAlignment)
			{
			case HorizontalAlignmentOptions.Left:
				num = 0f;
				goto IL_92;
			case HorizontalAlignmentOptions.Center:
				num = 0.5f;
				goto IL_92;
			case HorizontalAlignmentOptions.Right:
				num = 1f;
				goto IL_92;
			}
			num = 0f;
			IL_92:
			if (!true)
			{
			}
			float x = num;
			VerticalAlignmentOptions verticalAlignment = parent.verticalAlignment;
			if (!true)
			{
			}
			float y;
			if (verticalAlignment != VerticalAlignmentOptions.Top)
			{
				if (verticalAlignment != VerticalAlignmentOptions.Middle)
				{
					if (verticalAlignment != VerticalAlignmentOptions.Bottom)
					{
						y = 0.5f;
					}
					else
					{
						y = 1f;
					}
				}
				else
				{
					y = 0.5f;
				}
			}
			else
			{
				y = 0f;
			}
			if (!true)
			{
			}
			Vector2 fontAnchor = new Vector2(x, y);
			btn.rectTransform.SetParent(parent.transform, false);
			btn.rectTransform.pivot = new Vector2(0.5f, 0.5f);
			btn.rectTransform.anchoredPosition = fontAnchor * parent.rectTransform.rect.size + btn.rectTransform.pivot * (charInfoA.bottomLeft + charInfoB.topRight);
			btn.rectTransform.SetParent(parent.transform.parent, true);
			btn.rectTransform.localScale = Vector3.one;
			btn.rectTransform.SetAsFirstSibling();
			return btn;
		}

		// Token: 0x0600B3D8 RID: 46040 RVA: 0x0051D8C8 File Offset: 0x0051BAC8
		public void Clear()
		{
			foreach (GameObject btn in this.FullBtn)
			{
				PoolManager.Destroy(this.FullBtnKey, btn);
			}
			foreach (GameObject btn2 in this.LeftBtn)
			{
				PoolManager.Destroy(this.LeftBtnKey, btn2);
			}
			foreach (GameObject btn3 in this.RightBtn)
			{
				PoolManager.Destroy(this.RightBtnKey, btn3);
			}
			this.FullBtn.Clear();
			this.LeftBtn.Clear();
			this.RightBtn.Clear();
		}

		// Token: 0x04008BDA RID: 35802
		public List<GameObject> FullBtn;

		// Token: 0x04008BDB RID: 35803
		public List<GameObject> LeftBtn;

		// Token: 0x04008BDC RID: 35804
		public List<GameObject> RightBtn;

		// Token: 0x04008BDD RID: 35805
		private const float IncreaseWidth = 15f;

		// Token: 0x04008BDE RID: 35806
		private const float DeltaHeight = -6f;
	}
}
