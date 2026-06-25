using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Taiwu.Profession;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x020003AD RID: 941
public class UI_TeachCombatSkillResultConfirm : UIBase
{
	// Token: 0x0600387B RID: 14459 RVA: 0x001C7F28 File Offset: 0x001C6128
	public override void OnInit(ArgumentBox argsBox)
	{
		bool flag = !this._inited;
		if (flag)
		{
			this.InitRefers();
			this._displayTeachResult = new List<UI_TeachCombatSkillResultConfirm.TeachResultItemData>();
		}
		this.ReadArgs(argsBox);
		this.InitDisplayTeachResult();
		this.InitTeachCombatSkillItemView();
		this._inited = true;
		this.DoExpand();
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x0600387C RID: 14460 RVA: 0x001C7F88 File Offset: 0x001C6188
	private void DoExpand()
	{
		GameObject table = base.CGet<GameObject>("TableBack");
		GameObject content = base.CGet<GameObject>("ItemScrollView");
		RectTransform background = base.CGet<RectTransform>("Bg");
		RectTransform title = base.CGet<RectTransform>("TitleBack");
		RectTransform upperFrame = base.CGet<RectTransform>("OuterFrameUp");
		RectTransform lowerFrame = base.CGet<RectTransform>("OuterFrameDown");
		table.SetActive(false);
		content.SetActive(false);
		background.sizeDelta = new Vector2(2560f, 0f);
		title.transform.localPosition = Vector3.zero;
		upperFrame.transform.localPosition = Vector3.zero;
		lowerFrame.transform.localPosition = Vector3.zero;
		this.AudioIn = "ui_collect_get";
		title.DOLocalMove(new Vector3(0f, 624f, 0f), 0.2f, false);
		upperFrame.DOLocalMove(new Vector3(0f, 587f, 0f), 0.2f, false);
		lowerFrame.DOLocalMove(new Vector3(0f, -587f, 0f), 0.2f, false);
		background.DOSizeDelta(new Vector2(2560f, 1151f), 0.2f, false).SetUpdate(true).OnComplete(delegate
		{
			table.SetActive(true);
			content.SetActive(true);
		});
	}

	// Token: 0x0600387D RID: 14461 RVA: 0x001C80F8 File Offset: 0x001C62F8
	private void ReadArgs(ArgumentBox argsBox)
	{
		bool flag = argsBox == null;
		if (!flag)
		{
			argsBox.Get<List<ItemKey>>("bookItemKeys", out this._bookItemKeys);
			argsBox.Get<TasterUltimateResult>("teachCombatSkillResult", out this._teachCombatSkillResult);
		}
	}

	// Token: 0x0600387E RID: 14462 RVA: 0x001C8134 File Offset: 0x001C6334
	private void SetData(int charId)
	{
		UI_TeachCombatSkillResultConfirm.<>c__DisplayClass16_0 CS$<>8__locals1 = new UI_TeachCombatSkillResultConfirm.<>c__DisplayClass16_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.charId = charId;
		CS$<>8__locals1.characterDisplayData = this._teachCombatSkillResult.Characters[CS$<>8__locals1.charId];
		Dictionary<short, byte> readBookPageData = new Dictionary<short, byte>();
		Dictionary<CharacterDisplayData, bool> favorabilityChangeData = new Dictionary<CharacterDisplayData, bool>();
		Dictionary<CharacterDisplayData, ushort> relationChangeData = new Dictionary<CharacterDisplayData, ushort>();
		foreach (KeyValuePair<IntPair, byte> readBookPageEntry in this._teachCombatSkillResult.ReadBookPageData)
		{
			bool flag = readBookPageEntry.Key.First == CS$<>8__locals1.charId;
			if (flag)
			{
				short bookTempLateId = this.GetBookTemplateId(readBookPageEntry.Key.Second);
				readBookPageData.Add(bookTempLateId, readBookPageEntry.Value);
			}
		}
		foreach (KeyValuePair<IntPair, bool> favorabilityChangeEntry in this._teachCombatSkillResult.FavorabilityChangeData)
		{
			bool flag2 = favorabilityChangeEntry.Key.First == CS$<>8__locals1.charId;
			if (flag2)
			{
				favorabilityChangeData.Add(this._teachCombatSkillResult.Characters[favorabilityChangeEntry.Key.Second], favorabilityChangeEntry.Value);
			}
		}
		using (Dictionary<IntPair, ushort>.Enumerator enumerator3 = this._teachCombatSkillResult.RelationChangeData.GetEnumerator())
		{
			while (enumerator3.MoveNext())
			{
				UI_TeachCombatSkillResultConfirm.<>c__DisplayClass16_1 CS$<>8__locals2 = new UI_TeachCombatSkillResultConfirm.<>c__DisplayClass16_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.relationChangeEntry = enumerator3.Current;
				bool flag3 = CS$<>8__locals2.relationChangeEntry.Key.First == CS$<>8__locals2.CS$<>8__locals1.charId;
				if (flag3)
				{
					ushort value = CS$<>8__locals2.relationChangeEntry.Value;
					ushort num = value;
					if (num <= 512)
					{
						if (num != 64)
						{
							if (num != 128)
							{
								if (num == 512)
								{
									relationChangeData.Add(this._teachCombatSkillResult.Characters[CS$<>8__locals2.relationChangeEntry.Key.Second], CS$<>8__locals2.relationChangeEntry.Value);
									this.AddRelation(CS$<>8__locals2.relationChangeEntry.Key.Second, CS$<>8__locals2.CS$<>8__locals1.characterDisplayData, TeachSkillResultRelationType.SwornBrotherOrSister);
								}
							}
							else
							{
								this.AddRelation(CS$<>8__locals2.CS$<>8__locals1.charId, this._teachCombatSkillResult.Characters[CS$<>8__locals2.relationChangeEntry.Key.Second], TeachSkillResultRelationType.AdoptiveChild);
								this.AddRelation(CS$<>8__locals2.relationChangeEntry.Key.Second, CS$<>8__locals2.CS$<>8__locals1.characterDisplayData, TeachSkillResultRelationType.AdoptiveParent);
							}
						}
						else
						{
							this.AddRelation(CS$<>8__locals2.CS$<>8__locals1.charId, this._teachCombatSkillResult.Characters[CS$<>8__locals2.relationChangeEntry.Key.Second], TeachSkillResultRelationType.AdoptiveParent);
							this.AddRelation(CS$<>8__locals2.relationChangeEntry.Key.Second, CS$<>8__locals2.CS$<>8__locals1.characterDisplayData, TeachSkillResultRelationType.AdoptiveChild);
						}
					}
					else if (num != 8192)
					{
						if (num != 16384)
						{
							if (num == 32768)
							{
								relationChangeData.Add(this._teachCombatSkillResult.Characters[CS$<>8__locals2.relationChangeEntry.Key.Second], CS$<>8__locals2.relationChangeEntry.Value);
								this.AddRelation(CS$<>8__locals2.relationChangeEntry.Key.Second, CS$<>8__locals2.CS$<>8__locals1.characterDisplayData, TeachSkillResultRelationType.BeEnemy);
							}
						}
						else
						{
							this._callBackCount += 1;
							this.IsHusbandOrWife(CS$<>8__locals2.CS$<>8__locals1.charId, CS$<>8__locals2.relationChangeEntry.Key.Second, new Action<bool>(CS$<>8__locals2.<SetData>g__Action|0), this._callBackCount);
						}
					}
					else
					{
						relationChangeData.Add(this._teachCombatSkillResult.Characters[CS$<>8__locals2.relationChangeEntry.Key.Second], CS$<>8__locals2.relationChangeEntry.Value);
						this.AddRelation(CS$<>8__locals2.relationChangeEntry.Key.Second, CS$<>8__locals2.CS$<>8__locals1.characterDisplayData, TeachSkillResultRelationType.Friend);
					}
				}
			}
		}
		UI_TeachCombatSkillResultConfirm.TeachResultItemData teachResultItemData = new UI_TeachCombatSkillResultConfirm.TeachResultItemData(CS$<>8__locals1.charId, CS$<>8__locals1.characterDisplayData, readBookPageData, favorabilityChangeData, relationChangeData);
		this._displayTeachResult.Add(teachResultItemData);
	}

	// Token: 0x0600387F RID: 14463 RVA: 0x001C85F8 File Offset: 0x001C67F8
	private void InitDisplayTeachResult()
	{
		this._callBackCount = 0;
		this._displayTeachResult.Clear();
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this.SetData(taiwuCharId);
		foreach (int charId in this._teachCombatSkillResult.Characters.Keys)
		{
			bool flag = charId != taiwuCharId;
			if (flag)
			{
				this.SetData(charId);
			}
		}
	}

	// Token: 0x06003880 RID: 14464 RVA: 0x001C868C File Offset: 0x001C688C
	private void AddRelation(int targetId, CharacterDisplayData characterDisplayData, TeachSkillResultRelationType teachSkillResultRelationType)
	{
		bool flag = !this._relationDataDcit.ContainsKey(targetId);
		if (flag)
		{
			this._relationDataDcit.Add(targetId, new List<ValueTuple<CharacterDisplayData, int>>());
		}
		this._relationDataDcit[targetId].Add(new ValueTuple<CharacterDisplayData, int>(characterDisplayData, (int)teachSkillResultRelationType));
	}

	// Token: 0x06003881 RID: 14465 RVA: 0x001C86D8 File Offset: 0x001C68D8
	private void IsHusbandOrWife(int charId, int targetCharId, Action<bool> action, byte callBackCount)
	{
		CharacterDomainMethod.AsyncCall.GetRelationBetweenCharacters(this, charId, targetCharId, delegate(int offset, RawDataPool pool)
		{
			ValueTuple<ushort, ushort> result = default(ValueTuple<ushort, ushort>);
			Serializer.Deserialize(pool, offset, ref result);
			action(result.Item2 == 16384);
			bool flag = callBackCount == this._callBackCount;
			if (flag)
			{
				this.InitTeachCombatSkillItemView();
			}
		});
	}

	// Token: 0x06003882 RID: 14466 RVA: 0x001C8718 File Offset: 0x001C6918
	public List<ValueTuple<CharacterDisplayData, int>> GetRelation(int charId)
	{
		bool flag = !this._relationDataDcit.ContainsKey(charId);
		List<ValueTuple<CharacterDisplayData, int>> result;
		if (flag)
		{
			result = new List<ValueTuple<CharacterDisplayData, int>>();
		}
		else
		{
			result = this._relationDataDcit[charId];
		}
		return result;
	}

	// Token: 0x06003883 RID: 14467 RVA: 0x001C8754 File Offset: 0x001C6954
	private short GetBookTemplateId(int bookId)
	{
		foreach (ItemKey item in this._bookItemKeys)
		{
			bool flag = item.Id == bookId;
			if (flag)
			{
				return item.TemplateId;
			}
		}
		return -1;
	}

	// Token: 0x06003884 RID: 14468 RVA: 0x001C87C0 File Offset: 0x001C69C0
	private void InitTeachCombatSkillItemView()
	{
		for (int i = 0; i < this._content.transform.childCount; i++)
		{
			Transform teachSkillItem = this._content.transform.GetChild(i);
			bool flag = i < this._displayTeachResult.Count;
			if (flag)
			{
				this.OnItemRender(i, teachSkillItem.GetComponent<TeachSkillItemView>());
			}
			teachSkillItem.gameObject.SetActive(i < this._displayTeachResult.Count);
		}
	}

	// Token: 0x06003885 RID: 14469 RVA: 0x001C883C File Offset: 0x001C6A3C
	private void OnItemRender(int index, Refers refers)
	{
		UI_TeachCombatSkillResultConfirm.TeachResultItemData teachResultItemData = this._displayTeachResult[index];
		TeachSkillItemView itemView = refers as TeachSkillItemView;
		itemView.Refresh(teachResultItemData.CharacterDisplayData, teachResultItemData.ReadBookPageData, teachResultItemData.FavorabilityChangeData, teachResultItemData.RelationChangeData, this.GetRelation(teachResultItemData.CharId));
	}

	// Token: 0x06003886 RID: 14470 RVA: 0x001C888C File Offset: 0x001C6A8C
	private void Update()
	{
		bool flag = CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			this.HideUI();
		}
	}

	// Token: 0x06003887 RID: 14471 RVA: 0x001C88D6 File Offset: 0x001C6AD6
	private void HideUI()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06003888 RID: 14472 RVA: 0x001C88FC File Offset: 0x001C6AFC
	private void InitRefers()
	{
		this._content = base.CGet<RectTransform>("Content");
	}

	// Token: 0x040028F3 RID: 10483
	private const int BackGroundWidth = 2560;

	// Token: 0x040028F4 RID: 10484
	private const int TitlePosition = 624;

	// Token: 0x040028F5 RID: 10485
	private const int UpperFramePosition = 587;

	// Token: 0x040028F6 RID: 10486
	private const int LowerFramePosition = -587;

	// Token: 0x040028F7 RID: 10487
	private const int BackGroundHeight = 1151;

	// Token: 0x040028F8 RID: 10488
	private const float Duration = 0.2f;

	// Token: 0x040028F9 RID: 10489
	private bool _inited;

	// Token: 0x040028FA RID: 10490
	private TasterUltimateResult _teachCombatSkillResult;

	// Token: 0x040028FB RID: 10491
	private List<UI_TeachCombatSkillResultConfirm.TeachResultItemData> _displayTeachResult;

	// Token: 0x040028FC RID: 10492
	private List<ItemKey> _bookItemKeys = new List<ItemKey>();

	// Token: 0x040028FD RID: 10493
	private Dictionary<int, List<ValueTuple<CharacterDisplayData, int>>> _relationDataDcit = new Dictionary<int, List<ValueTuple<CharacterDisplayData, int>>>();

	// Token: 0x040028FE RID: 10494
	private byte _callBackCount;

	// Token: 0x040028FF RID: 10495
	private RectTransform _content;

	// Token: 0x02001812 RID: 6162
	public struct TeachResultItemData
	{
		// Token: 0x0600D5DB RID: 54747 RVA: 0x005BA8A6 File Offset: 0x005B8AA6
		public TeachResultItemData(int charId, CharacterDisplayData characterDisplayData, Dictionary<short, byte> readBookPageData, Dictionary<CharacterDisplayData, bool> favorabilityChangeData, Dictionary<CharacterDisplayData, ushort> relationChangeData)
		{
			this.CharId = charId;
			this.CharacterDisplayData = characterDisplayData;
			this.ReadBookPageData = readBookPageData;
			this.FavorabilityChangeData = favorabilityChangeData;
			this.RelationChangeData = relationChangeData;
		}

		// Token: 0x0400AD66 RID: 44390
		public int CharId;

		// Token: 0x0400AD67 RID: 44391
		public CharacterDisplayData CharacterDisplayData;

		// Token: 0x0400AD68 RID: 44392
		public Dictionary<short, byte> ReadBookPageData;

		// Token: 0x0400AD69 RID: 44393
		public Dictionary<CharacterDisplayData, bool> FavorabilityChangeData;

		// Token: 0x0400AD6A RID: 44394
		public Dictionary<CharacterDisplayData, ushort> RelationChangeData;
	}
}
