using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Record
{
	// Token: 0x020007BE RID: 1982
	public class CharacterNameClickLinkHandler
	{
		// Token: 0x060060D4 RID: 24788 RVA: 0x002C66B4 File Offset: 0x002C48B4
		public CharacterNameClickLinkHandler(RectTransform btnRoot, PoolItem fullBtnPool, PoolItem leftPartBtnPool, PoolItem rightPartBtnPool, Action<int> clickHandler)
		{
			this._buttonsRootRect = btnRoot;
			this._fullButtonPoolItem = fullBtnPool;
			this._leftPartButtonPoolItem = leftPartBtnPool;
			this._rightPartButtonPoolItem = rightPartBtnPool;
			this._onCharacterNameClicked = clickHandler;
		}

		// Token: 0x060060D5 RID: 24789 RVA: 0x002C66F0 File Offset: 0x002C48F0
		public void ProcessLinkInfo(TextMeshProUGUI tmpText, bool isShowTips = true)
		{
			this._generateButtonComplete = false;
			this._isShowTips = isShowTips;
			this._targetTextMeshPro = tmpText;
			TMPTextSpriteHelper spriteHelper = this._targetTextMeshPro.GetComponent<TMPTextSpriteHelper>();
			bool flag = null != spriteHelper;
			if (flag)
			{
				this._tmpSpriteParseComplete = false;
				spriteHelper.OnParseComplete = new Action(this.OnTMPSpriteHelperParseComplete);
			}
			else
			{
				this.OnTMPSpriteHelperParseComplete();
			}
		}

		// Token: 0x060060D6 RID: 24790 RVA: 0x002C6750 File Offset: 0x002C4950
		private void OnTMPSpriteHelperParseComplete()
		{
			this._tmpSpriteParseComplete = true;
			YieldHelper yieldHelper = SingletonObject.getInstance<YieldHelper>();
			bool flag = this._analyseRoutine != null;
			if (flag)
			{
				yieldHelper.StopCoroutine(this._analyseRoutine);
			}
			yieldHelper.StartCoroutine(this._analyseRoutine = this.Analyse());
		}

		// Token: 0x060060D7 RID: 24791 RVA: 0x002C679C File Offset: 0x002C499C
		private void ClearPrevButtons()
		{
			bool flag = null == this._targetTextMeshPro;
			if (!flag)
			{
				CButton[] buttons = this._buttonsRootRect.GetComponentsInTopChildren(false);
				for (int i = buttons.Length - 1; i >= 0; i--)
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

		// Token: 0x060060D8 RID: 24792 RVA: 0x002C6877 File Offset: 0x002C4A77
		private IEnumerator Analyse()
		{
			this.ClearPrevButtons();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			List<int> charIdList = new List<int>();
			int num;
			for (int i = 0; i < this._targetTextMeshPro.textInfo.linkCount; i = num + 1)
			{
				TMP_LinkInfo linkInfo = this._targetTextMeshPro.textInfo.linkInfo[i];
				string id = linkInfo.GetLinkID();
				bool flag = !id.StartsWith("character_");
				if (!flag)
				{
					string text = id;
					int charId;
					bool flag2 = int.TryParse(text.Substring(10, text.Length - 10), out charId) && !charIdList.Contains(charId);
					if (flag2)
					{
						charIdList.Add(charId);
					}
					linkInfo = default(TMP_LinkInfo);
					id = null;
				}
				num = i;
			}
			bool flag3 = charIdList.Count <= 0;
			if (flag3)
			{
				yield break;
			}
			TMP_LinkInfo[] infoLink = this._targetTextMeshPro.textInfo.linkInfo.ToArray<TMP_LinkInfo>();
			TMP_CharacterInfo[] infoCharacter = this._targetTextMeshPro.textInfo.characterInfo.ToArray<TMP_CharacterInfo>();
			IEnumerator sub = this.GenerateAllButton(charIdList, infoLink, infoCharacter);
			while (sub.MoveNext())
			{
				object obj = sub.Current;
				yield return obj;
			}
			yield break;
		}

		// Token: 0x060060D9 RID: 24793 RVA: 0x002C6888 File Offset: 0x002C4A88
		private CButton GenerateSingleButton(PoolItem poolItem, TMP_CharacterInfo charInfoA, TMP_CharacterInfo charInfoB)
		{
			GameObject btnObj = poolItem.GetObject();
			btnObj.transform.SetParent(this._buttonsRootRect, false);
			int lineNum = charInfoA.lineNumber;
			float btnWidth = charInfoB.topRight.x - charInfoA.topLeft.x + this._increaseWidth;
			float x = (charInfoA.bottomLeft.x + charInfoB.topRight.x) * 0.5f;
			float y = (float)(lineNum - 1) * (this._targetTextMeshPro.fontSize + this._targetTextMeshPro.lineSpacing) + this._targetTextMeshPro.fontSize * 0.5f - 3f;
			((RectTransform)btnObj.transform).SetWidth(btnWidth);
			((RectTransform)btnObj.transform).anchoredPosition = new Vector2(x, y);
			return btnObj.GetComponent<CButton>();
		}

		// Token: 0x060060DA RID: 24794 RVA: 0x002C6964 File Offset: 0x002C4B64
		private void SetBtnInteractionAndClickListener(CButton btn, bool interactable, int charId)
		{
			bool flag = null == btn;
			if (!flag)
			{
				btn.interactable = interactable;
				btn.onClick.RemoveAllListeners();
				if (interactable)
				{
					btn.onClick.AddListener(delegate()
					{
						Debug.Log(charId);
						Action<int> onCharacterNameClicked = this._onCharacterNameClicked;
						if (onCharacterNameClicked != null)
						{
							onCharacterNameClicked(charId);
						}
					});
				}
			}
		}

		// Token: 0x060060DB RID: 24795 RVA: 0x002C69C6 File Offset: 0x002C4BC6
		private IEnumerator GenerateAllButton(List<int> charIdList, IList<TMP_LinkInfo> infoLink, IList<TMP_CharacterInfo> infoCharacter)
		{
			CharacterNameClickLinkHandler.<>c__DisplayClass21_0 CS$<>8__locals1 = new CharacterNameClickLinkHandler.<>c__DisplayClass21_0();
			CS$<>8__locals1.charIdList = charIdList;
			CS$<>8__locals1.<>4__this = this;
			bool flag = !this._tmpSpriteParseComplete || this._generateButtonComplete;
			if (flag)
			{
				yield break;
			}
			CS$<>8__locals1.charStateMap = null;
			CharacterDomainMethod.AsyncCall.GetNameAndLifeRelatedDataList(null, CS$<>8__locals1.charIdList, delegate(int offset, RawDataPool dataPool)
			{
				List<NameAndLifeRelatedData> dataList = null;
				Serializer.Deserialize(dataPool, offset, ref dataList);
				CS$<>8__locals1.charStateMap = new Dictionary<int, NameAndLifeRelatedData>();
				for (int j = dataList.Count - 1; j >= 0; j--)
				{
					NameAndLifeRelatedData data = dataList[j];
					CS$<>8__locals1.charStateMap.Add(CS$<>8__locals1.charIdList[j], data);
				}
			});
			CS$<>8__locals1.buttonsToShow = new List<RectTransform>();
			CS$<>8__locals1.buttonsToSetup = new Dictionary<int, List<CButton>>();
			foreach (TMP_LinkInfo linkInfo in infoLink)
			{
				string id = linkInfo.GetLinkID();
				bool flag2 = !id.StartsWith("character_");
				if (!flag2)
				{
					string text = id;
					CharacterNameClickLinkHandler.<>c__DisplayClass21_1 CS$<>8__locals2;
					bool flag3 = int.TryParse(text.Substring(10, text.Length - 10), out CS$<>8__locals2.charId);
					if (flag3)
					{
						int linkStartIndex = linkInfo.linkTextfirstCharacterIndex;
						int linkLength = linkInfo.linkTextLength - 1;
						TMP_CharacterInfo charInfoStart = infoCharacter[linkStartIndex];
						TMP_CharacterInfo charInfoEnd = infoCharacter[linkStartIndex + linkLength];
						while (charInfoStart.character == ' ')
						{
							int num = linkStartIndex;
							linkStartIndex = num + 1;
							charInfoStart = infoCharacter[linkStartIndex];
						}
						while (charInfoEnd.character == ' ')
						{
							int num = linkLength;
							linkLength = num - 1;
							charInfoEnd = infoCharacter[linkStartIndex + linkLength];
						}
						CButton buttonA = null;
						CButton buttonB = null;
						bool flag4 = charInfoStart.lineNumber == charInfoEnd.lineNumber;
						if (flag4)
						{
							buttonA = this.GenerateSingleButton(this._fullButtonPoolItem, charInfoStart, charInfoEnd);
							buttonA.name = string.Format("{0}{1}", "Full_", CS$<>8__locals2.charId);
						}
						else
						{
							bool leftPart = true;
							TMP_CharacterInfo prevCharInfo = charInfoStart;
							int num;
							for (int i = 1; i < linkLength; i = num + 1)
							{
								TMP_CharacterInfo charInfo = infoCharacter[linkStartIndex + i];
								bool flag5 = charInfo.lineNumber != charInfoStart.lineNumber;
								if (flag5)
								{
									CButton btn = this.GenerateSingleButton(leftPart ? this._leftPartButtonPoolItem : this._rightPartButtonPoolItem, charInfoStart, prevCharInfo);
									bool flag6 = leftPart;
									if (flag6)
									{
										buttonA = btn;
									}
									else
									{
										buttonB = btn;
									}
									leftPart = false;
									charInfoStart = charInfo;
									btn = null;
								}
								prevCharInfo = charInfo;
								charInfo = default(TMP_CharacterInfo);
								num = i;
							}
							bool flag7 = null != buttonA;
							if (flag7)
							{
								buttonA.name = string.Format("{0}{1}", "LPart_", CS$<>8__locals2.charId);
							}
							bool flag8 = null != buttonB;
							if (flag8)
							{
								buttonB.name = string.Format("{0}{1}", "RPart_", CS$<>8__locals2.charId);
							}
							prevCharInfo = default(TMP_CharacterInfo);
						}
						CS$<>8__locals1.<GenerateAllButton>g__PrepareButton|1(buttonA, ref CS$<>8__locals2);
						CS$<>8__locals1.<GenerateAllButton>g__PrepareButton|1(buttonB, ref CS$<>8__locals2);
						yield return new WaitForEndOfFrame();
						charInfoStart = default(TMP_CharacterInfo);
						charInfoEnd = default(TMP_CharacterInfo);
						buttonA = null;
						buttonB = null;
					}
					id = null;
					linkInfo = default(TMP_LinkInfo);
				}
			}
			IEnumerator<TMP_LinkInfo> enumerator = null;
			for (;;)
			{
				bool flag9 = CS$<>8__locals1.charStateMap != null;
				if (flag9)
				{
					break;
				}
				yield return new WaitForEndOfFrame();
			}
			foreach (KeyValuePair<int, List<CButton>> keyValuePair in CS$<>8__locals1.buttonsToSetup)
			{
				int num;
				List<CButton> list;
				keyValuePair.Deconstruct(out num, out list);
				int charId = num;
				List<CButton> buttons = list;
				NameAndLifeRelatedData state;
				bool hasState = CS$<>8__locals1.charStateMap.TryGetValue(charId, out state);
				bool isAlive = hasState && state.LifeState == 0;
				foreach (CButton button in buttons)
				{
					bool flag10 = !hasState;
					if (flag10)
					{
						button.gameObject.SetActive(false);
					}
					else
					{
						this.SetBtnInteractionAndClickListener(button, isAlive, charId);
						TooltipInvoker mouseTipDisplayer = button.GetComponent<TooltipInvoker>();
						mouseTipDisplayer.enabled = this._isShowTips;
						bool isShowTips = this._isShowTips;
						if (isShowTips)
						{
							mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("arg0", isAlive ? LocalStringManager.Get(LanguageKey.LK_LIfeRecord_Character_Alive) : LocalStringManager.Get(LanguageKey.LK_LIfeRecord_Character_Dead));
						}
						mouseTipDisplayer = null;
						button = null;
					}
				}
				List<CButton>.Enumerator enumerator3 = default(List<CButton>.Enumerator);
				state = default(NameAndLifeRelatedData);
				buttons = null;
			}
			Dictionary<int, List<CButton>>.Enumerator enumerator2 = default(Dictionary<int, List<CButton>>.Enumerator);
			foreach (RectTransform rect in CS$<>8__locals1.buttonsToShow)
			{
				rect.localScale = Vector3.one;
				rect = null;
			}
			List<RectTransform>.Enumerator enumerator4 = default(List<RectTransform>.Enumerator);
			this._generateButtonComplete = true;
			yield break;
			yield break;
		}

		// Token: 0x0400431D RID: 17181
		private readonly PoolItem _fullButtonPoolItem;

		// Token: 0x0400431E RID: 17182
		private readonly PoolItem _leftPartButtonPoolItem;

		// Token: 0x0400431F RID: 17183
		private readonly PoolItem _rightPartButtonPoolItem;

		// Token: 0x04004320 RID: 17184
		private readonly RectTransform _buttonsRootRect;

		// Token: 0x04004321 RID: 17185
		private TextMeshProUGUI _targetTextMeshPro;

		// Token: 0x04004322 RID: 17186
		private bool _generateButtonComplete;

		// Token: 0x04004323 RID: 17187
		private bool _tmpSpriteParseComplete;

		// Token: 0x04004324 RID: 17188
		private bool _isShowTips;

		// Token: 0x04004325 RID: 17189
		private readonly Action<int> _onCharacterNameClicked;

		// Token: 0x04004326 RID: 17190
		private const string FullBtnNamePre = "Full_";

		// Token: 0x04004327 RID: 17191
		private const string LeftPartBtnNamePre = "LPart_";

		// Token: 0x04004328 RID: 17192
		private const string RightPartBtnNamePre = "RPart_";

		// Token: 0x04004329 RID: 17193
		private readonly float _increaseWidth = 5f;

		// Token: 0x0400432A RID: 17194
		private IEnumerator _analyseRoutine;
	}
}
