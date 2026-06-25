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

namespace Game.Views.Profession
{
	// Token: 0x020007CC RID: 1996
	public class ViewTeachCombatSkillResultConfirm : UIBase
	{
		// Token: 0x06006188 RID: 24968 RVA: 0x002CB980 File Offset: 0x002C9B80
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = !this._inited;
			if (flag)
			{
				this._displayTeachResult = new List<ViewTeachCombatSkillResultConfirm.TeachResultItemData>();
			}
			this.ReadArgs(argsBox);
			this.InitDisplayTeachResult();
			this.InitTeachCombatSkillItemView();
			this._inited = true;
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06006189 RID: 24969 RVA: 0x002CB9D4 File Offset: 0x002C9BD4
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

		// Token: 0x0600618A RID: 24970 RVA: 0x002CBB44 File Offset: 0x002C9D44
		private void ReadArgs(ArgumentBox argsBox)
		{
			bool flag = argsBox == null;
			if (!flag)
			{
				argsBox.Get<TasterUltimateResult>("TasterUltimateResult", out this._teachCombatSkillResult);
			}
		}

		// Token: 0x0600618B RID: 24971 RVA: 0x002CBB70 File Offset: 0x002C9D70
		private void SetData(int charId)
		{
			ViewTeachCombatSkillResultConfirm.<>c__DisplayClass17_0 CS$<>8__locals1 = new ViewTeachCombatSkillResultConfirm.<>c__DisplayClass17_0();
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
					ViewTeachCombatSkillResultConfirm.<>c__DisplayClass17_1 CS$<>8__locals2 = new ViewTeachCombatSkillResultConfirm.<>c__DisplayClass17_1();
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
			ViewTeachCombatSkillResultConfirm.TeachResultItemData teachResultItemData = new ViewTeachCombatSkillResultConfirm.TeachResultItemData(CS$<>8__locals1.charId, CS$<>8__locals1.characterDisplayData, readBookPageData, favorabilityChangeData, relationChangeData);
			this._displayTeachResult.Add(teachResultItemData);
		}

		// Token: 0x0600618C RID: 24972 RVA: 0x002CC034 File Offset: 0x002CA234
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

		// Token: 0x0600618D RID: 24973 RVA: 0x002CC0C8 File Offset: 0x002CA2C8
		private void AddRelation(int targetId, CharacterDisplayData characterDisplayData, TeachSkillResultRelationType teachSkillResultRelationType)
		{
			bool flag = !this._relationDataDcit.ContainsKey(targetId);
			if (flag)
			{
				this._relationDataDcit.Add(targetId, new List<ValueTuple<CharacterDisplayData, int>>());
			}
			this._relationDataDcit[targetId].Add(new ValueTuple<CharacterDisplayData, int>(characterDisplayData, (int)teachSkillResultRelationType));
		}

		// Token: 0x0600618E RID: 24974 RVA: 0x002CC114 File Offset: 0x002CA314
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

		// Token: 0x0600618F RID: 24975 RVA: 0x002CC154 File Offset: 0x002CA354
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

		// Token: 0x06006190 RID: 24976 RVA: 0x002CC190 File Offset: 0x002CA390
		private short GetBookTemplateId(int bookId)
		{
			foreach (ItemKey item in ViewTeachCombatSkillResultConfirm._bookItemKeys)
			{
				bool flag = item.Id == bookId;
				if (flag)
				{
					return item.TemplateId;
				}
			}
			return -1;
		}

		// Token: 0x06006191 RID: 24977 RVA: 0x002CC1FC File Offset: 0x002CA3FC
		private void InitTeachCombatSkillItemView()
		{
			for (int i = 0; i < this.content.transform.childCount; i++)
			{
				Transform teachSkillItem = this.content.transform.GetChild(i);
				bool flag = i < this._displayTeachResult.Count;
				if (flag)
				{
					this.OnItemRender(i, teachSkillItem.GetComponent<TeachSkillItemView>());
				}
				teachSkillItem.gameObject.SetActive(i < this._displayTeachResult.Count);
			}
		}

		// Token: 0x06006192 RID: 24978 RVA: 0x002CC278 File Offset: 0x002CA478
		private void OnItemRender(int index, TeachSkillItemView itemView)
		{
			ViewTeachCombatSkillResultConfirm.TeachResultItemData teachResultItemData = this._displayTeachResult[index];
			itemView.Refresh(teachResultItemData.CharacterDisplayData, teachResultItemData.ReadBookPageData, teachResultItemData.FavorabilityChangeData, teachResultItemData.RelationChangeData, this.GetRelation(teachResultItemData.CharId));
		}

		// Token: 0x06006193 RID: 24979 RVA: 0x002CC2C0 File Offset: 0x002CA4C0
		private void Update()
		{
			bool flag = CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				this.HideUI();
			}
		}

		// Token: 0x06006194 RID: 24980 RVA: 0x002CC30A File Offset: 0x002CA50A
		private void HideUI()
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			UIManager.Instance.HideUI(this.Element);
		}

		// Token: 0x06006195 RID: 24981 RVA: 0x002CC330 File Offset: 0x002CA530
		public static void SetBookItemKeys(List<ItemKey> bookItemKeys)
		{
			ViewTeachCombatSkillResultConfirm._bookItemKeys.Clear();
			ViewTeachCombatSkillResultConfirm._bookItemKeys.AddRange(bookItemKeys);
		}

		// Token: 0x040043A9 RID: 17321
		[SerializeField]
		private RectTransform content;

		// Token: 0x040043AA RID: 17322
		private const int BackGroundWidth = 2560;

		// Token: 0x040043AB RID: 17323
		private const int TitlePosition = 624;

		// Token: 0x040043AC RID: 17324
		private const int UpperFramePosition = 587;

		// Token: 0x040043AD RID: 17325
		private const int LowerFramePosition = -587;

		// Token: 0x040043AE RID: 17326
		private const int BackGroundHeight = 1151;

		// Token: 0x040043AF RID: 17327
		private const float Duration = 0.2f;

		// Token: 0x040043B0 RID: 17328
		private bool _inited;

		// Token: 0x040043B1 RID: 17329
		private TasterUltimateResult _teachCombatSkillResult;

		// Token: 0x040043B2 RID: 17330
		private List<ViewTeachCombatSkillResultConfirm.TeachResultItemData> _displayTeachResult;

		// Token: 0x040043B3 RID: 17331
		private static readonly List<ItemKey> _bookItemKeys = new List<ItemKey>();

		// Token: 0x040043B4 RID: 17332
		private Dictionary<int, List<ValueTuple<CharacterDisplayData, int>>> _relationDataDcit = new Dictionary<int, List<ValueTuple<CharacterDisplayData, int>>>();

		// Token: 0x040043B5 RID: 17333
		private byte _callBackCount;

		// Token: 0x02001D1A RID: 7450
		public struct TeachResultItemData
		{
			// Token: 0x0600EC43 RID: 60483 RVA: 0x006053C5 File Offset: 0x006035C5
			public TeachResultItemData(int charId, CharacterDisplayData characterDisplayData, Dictionary<short, byte> readBookPageData, Dictionary<CharacterDisplayData, bool> favorabilityChangeData, Dictionary<CharacterDisplayData, ushort> relationChangeData)
			{
				this.CharId = charId;
				this.CharacterDisplayData = characterDisplayData;
				this.ReadBookPageData = readBookPageData;
				this.FavorabilityChangeData = favorabilityChangeData;
				this.RelationChangeData = relationChangeData;
			}

			// Token: 0x0400C506 RID: 50438
			public int CharId;

			// Token: 0x0400C507 RID: 50439
			public CharacterDisplayData CharacterDisplayData;

			// Token: 0x0400C508 RID: 50440
			public Dictionary<short, byte> ReadBookPageData;

			// Token: 0x0400C509 RID: 50441
			public Dictionary<CharacterDisplayData, bool> FavorabilityChangeData;

			// Token: 0x0400C50A RID: 50442
			public Dictionary<CharacterDisplayData, ushort> RelationChangeData;
		}
	}
}
