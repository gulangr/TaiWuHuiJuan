using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020000DB RID: 219
public class CharacterNameBtn
{
	// Token: 0x060007D2 RID: 2002 RVA: 0x00036911 File Offset: 0x00034B11
	public CharacterNameBtn(RectTransform btnRoot, PoolItem fullBtnPool, PoolItem leftPartBtnPool, PoolItem rightPartBtnPool, Action<int> clickHandler)
	{
		this.ButtonsRootRect = btnRoot;
		this._fullButtonPoolItem = fullBtnPool;
		this._leftPartButtonPoolItem = leftPartBtnPool;
		this._rightPartButtonPoolItem = rightPartBtnPool;
		this.OnCharacterNameClicked = clickHandler;
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0003694C File Offset: 0x00034B4C
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

	// Token: 0x060007D4 RID: 2004 RVA: 0x000369AC File Offset: 0x00034BAC
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

	// Token: 0x060007D5 RID: 2005 RVA: 0x000369F8 File Offset: 0x00034BF8
	private void ClearPrevButtons()
	{
		bool flag = null == this._targetTextMeshPro;
		if (!flag)
		{
			CButtonObsolete[] buttons = this.ButtonsRootRect.GetComponentsInTopChildren(false);
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

	// Token: 0x060007D6 RID: 2006 RVA: 0x00036AD3 File Offset: 0x00034CD3
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

	// Token: 0x060007D7 RID: 2007 RVA: 0x00036AE4 File Offset: 0x00034CE4
	private CButtonObsolete GenerateSingleButton(PoolItem poolItem, TMP_CharacterInfo charInfoA, TMP_CharacterInfo charInfoB)
	{
		GameObject btnObj = poolItem.GetObject();
		btnObj.transform.SetParent(this.ButtonsRootRect, false);
		int lineNum = charInfoA.lineNumber;
		float btnWidth = charInfoB.topRight.x - charInfoA.topLeft.x + this._increaseWidth;
		float x = (charInfoA.bottomLeft.x + charInfoB.topRight.x) * 0.5f;
		float y = (float)(lineNum - 1) * (this._targetTextMeshPro.fontSize + this._targetTextMeshPro.lineSpacing) + this._targetTextMeshPro.fontSize * 0.5f - 3f;
		((RectTransform)btnObj.transform).SetWidth(btnWidth);
		((RectTransform)btnObj.transform).anchoredPosition = new Vector2(x, y);
		return btnObj.GetComponent<CButtonObsolete>();
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x00036BC0 File Offset: 0x00034DC0
	private void SetBtnInteractionAndClickListener(CButtonObsolete btn, bool interactable, int charId)
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
					Action<int> onCharacterNameClicked = this.OnCharacterNameClicked;
					if (onCharacterNameClicked != null)
					{
						onCharacterNameClicked(charId);
					}
				});
			}
		}
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x00036C22 File Offset: 0x00034E22
	private IEnumerator GenerateAllButton(List<int> charIdList, IList<TMP_LinkInfo> infoLink, IList<TMP_CharacterInfo> infoCharacter)
	{
		CharacterNameBtn.<>c__DisplayClass21_0 CS$<>8__locals1 = new CharacterNameBtn.<>c__DisplayClass21_0();
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
		CS$<>8__locals1.buttonsToSetup = new Dictionary<int, List<CButtonObsolete>>();
		foreach (TMP_LinkInfo linkInfo in infoLink)
		{
			string id = linkInfo.GetLinkID();
			bool flag2 = !id.StartsWith("character_");
			if (!flag2)
			{
				string text = id;
				CharacterNameBtn.<>c__DisplayClass21_1 CS$<>8__locals2;
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
					CButtonObsolete buttonA = null;
					CButtonObsolete buttonB = null;
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
								CButtonObsolete btn = this.GenerateSingleButton(leftPart ? this._leftPartButtonPoolItem : this._rightPartButtonPoolItem, charInfoStart, prevCharInfo);
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
		foreach (KeyValuePair<int, List<CButtonObsolete>> keyValuePair in CS$<>8__locals1.buttonsToSetup)
		{
			int num;
			List<CButtonObsolete> list;
			keyValuePair.Deconstruct(out num, out list);
			int charId = num;
			List<CButtonObsolete> buttons = list;
			NameAndLifeRelatedData state;
			bool hasState = CS$<>8__locals1.charStateMap.TryGetValue(charId, out state);
			bool isAlive = hasState && state.LifeState == 0;
			foreach (CButtonObsolete button in buttons)
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
			List<CButtonObsolete>.Enumerator enumerator3 = default(List<CButtonObsolete>.Enumerator);
			state = default(NameAndLifeRelatedData);
			buttons = null;
		}
		Dictionary<int, List<CButtonObsolete>>.Enumerator enumerator2 = default(Dictionary<int, List<CButtonObsolete>>.Enumerator);
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

	// Token: 0x040007F8 RID: 2040
	private PoolItem _fullButtonPoolItem;

	// Token: 0x040007F9 RID: 2041
	private PoolItem _leftPartButtonPoolItem;

	// Token: 0x040007FA RID: 2042
	private PoolItem _rightPartButtonPoolItem;

	// Token: 0x040007FB RID: 2043
	private RectTransform ButtonsRootRect;

	// Token: 0x040007FC RID: 2044
	private TextMeshProUGUI _targetTextMeshPro;

	// Token: 0x040007FD RID: 2045
	private bool _generateButtonComplete;

	// Token: 0x040007FE RID: 2046
	private bool _tmpSpriteParseComplete;

	// Token: 0x040007FF RID: 2047
	private bool _isShowTips;

	// Token: 0x04000800 RID: 2048
	public Action<int> OnCharacterNameClicked;

	// Token: 0x04000801 RID: 2049
	private const string FullBtnNamePre = "Full_";

	// Token: 0x04000802 RID: 2050
	private const string LeftPartBtnNamePre = "LPart_";

	// Token: 0x04000803 RID: 2051
	private const string RightPartBtnNamePre = "RPart_";

	// Token: 0x04000804 RID: 2052
	private float _increaseWidth = 5f;

	// Token: 0x04000805 RID: 2053
	private IEnumerator _analyseRoutine;
}
