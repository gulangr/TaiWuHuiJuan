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
	// Token: 0x020007CD RID: 1997
	public class ViewTeachLifeSkillResultConfirm : UIBase
	{
		// Token: 0x06006198 RID: 24984 RVA: 0x002CC36C File Offset: 0x002CA56C
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = !this._inited;
			if (flag)
			{
				this._displayTeachResult = new List<ViewTeachLifeSkillResultConfirm.TeachResultItemData>();
			}
			this.ReadArgs(argsBox);
			this.InitDisplayTeachResult();
			this.InitTeachLifeSkillItemView();
			this._inited = true;
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06006199 RID: 24985 RVA: 0x002CC3C0 File Offset: 0x002CA5C0
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

		// Token: 0x0600619A RID: 24986 RVA: 0x002CC530 File Offset: 0x002CA730
		private void ReadArgs(ArgumentBox argsBox)
		{
			bool flag = argsBox == null;
			if (!flag)
			{
				argsBox.Get<TasterUltimateResult>("TasterUltimateResult", out this._teachLifeSkillResult);
			}
		}

		// Token: 0x0600619B RID: 24987 RVA: 0x002CC55C File Offset: 0x002CA75C
		private void SetData(int charId)
		{
			ViewTeachLifeSkillResultConfirm.<>c__DisplayClass17_0 CS$<>8__locals1 = new ViewTeachLifeSkillResultConfirm.<>c__DisplayClass17_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.charId = charId;
			CS$<>8__locals1.characterDisplayData = this._teachLifeSkillResult.Characters[CS$<>8__locals1.charId];
			Dictionary<short, byte> readBookPageData = new Dictionary<short, byte>();
			Dictionary<CharacterDisplayData, bool> favorabilityChangeData = new Dictionary<CharacterDisplayData, bool>();
			Dictionary<CharacterDisplayData, ushort> relationChangeData = new Dictionary<CharacterDisplayData, ushort>();
			foreach (KeyValuePair<IntPair, byte> readBookPageEntry in this._teachLifeSkillResult.ReadBookPageData)
			{
				bool flag = readBookPageEntry.Key.First == CS$<>8__locals1.charId;
				if (flag)
				{
					short bookTempLateId = this.GetBookTemplateId(readBookPageEntry.Key.Second);
					readBookPageData.Add(bookTempLateId, readBookPageEntry.Value);
				}
			}
			foreach (KeyValuePair<IntPair, bool> favorabilityChangeEntry in this._teachLifeSkillResult.FavorabilityChangeData)
			{
				bool flag2 = favorabilityChangeEntry.Key.First == CS$<>8__locals1.charId;
				if (flag2)
				{
					favorabilityChangeData.Add(this._teachLifeSkillResult.Characters[favorabilityChangeEntry.Key.Second], favorabilityChangeEntry.Value);
				}
			}
			using (Dictionary<IntPair, ushort>.Enumerator enumerator3 = this._teachLifeSkillResult.RelationChangeData.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					ViewTeachLifeSkillResultConfirm.<>c__DisplayClass17_1 CS$<>8__locals2 = new ViewTeachLifeSkillResultConfirm.<>c__DisplayClass17_1();
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
										relationChangeData.Add(this._teachLifeSkillResult.Characters[CS$<>8__locals2.relationChangeEntry.Key.Second], CS$<>8__locals2.relationChangeEntry.Value);
										this.AddRelation(CS$<>8__locals2.relationChangeEntry.Key.Second, CS$<>8__locals2.CS$<>8__locals1.characterDisplayData, TeachSkillResultRelationType.SwornBrotherOrSister);
									}
								}
								else
								{
									this.AddRelation(CS$<>8__locals2.CS$<>8__locals1.charId, this._teachLifeSkillResult.Characters[CS$<>8__locals2.relationChangeEntry.Key.Second], TeachSkillResultRelationType.AdoptiveChild);
									this.AddRelation(CS$<>8__locals2.relationChangeEntry.Key.Second, CS$<>8__locals2.CS$<>8__locals1.characterDisplayData, TeachSkillResultRelationType.AdoptiveParent);
								}
							}
							else
							{
								this.AddRelation(CS$<>8__locals2.CS$<>8__locals1.charId, this._teachLifeSkillResult.Characters[CS$<>8__locals2.relationChangeEntry.Key.Second], TeachSkillResultRelationType.AdoptiveParent);
								this.AddRelation(CS$<>8__locals2.relationChangeEntry.Key.Second, CS$<>8__locals2.CS$<>8__locals1.characterDisplayData, TeachSkillResultRelationType.AdoptiveChild);
							}
						}
						else if (num != 8192)
						{
							if (num != 16384)
							{
								if (num == 32768)
								{
									relationChangeData.Add(this._teachLifeSkillResult.Characters[CS$<>8__locals2.relationChangeEntry.Key.Second], CS$<>8__locals2.relationChangeEntry.Value);
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
							relationChangeData.Add(this._teachLifeSkillResult.Characters[CS$<>8__locals2.relationChangeEntry.Key.Second], CS$<>8__locals2.relationChangeEntry.Value);
							this.AddRelation(CS$<>8__locals2.relationChangeEntry.Key.Second, CS$<>8__locals2.CS$<>8__locals1.characterDisplayData, TeachSkillResultRelationType.Friend);
						}
					}
				}
			}
			ViewTeachLifeSkillResultConfirm.TeachResultItemData teachResultItemData = new ViewTeachLifeSkillResultConfirm.TeachResultItemData(CS$<>8__locals1.charId, CS$<>8__locals1.characterDisplayData, readBookPageData, favorabilityChangeData, relationChangeData);
			this._displayTeachResult.Add(teachResultItemData);
		}

		// Token: 0x0600619C RID: 24988 RVA: 0x002CCA20 File Offset: 0x002CAC20
		private void InitDisplayTeachResult()
		{
			this._callBackCount = 0;
			this._displayTeachResult.Clear();
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this.SetData(taiwuCharId);
			foreach (int charId in this._teachLifeSkillResult.Characters.Keys)
			{
				bool flag = charId != taiwuCharId;
				if (flag)
				{
					this.SetData(charId);
				}
			}
		}

		// Token: 0x0600619D RID: 24989 RVA: 0x002CCAB4 File Offset: 0x002CACB4
		private void AddRelation(int targetId, CharacterDisplayData characterDisplayData, TeachSkillResultRelationType TeachSkillResultRelationType)
		{
			bool flag = !this._relationDataDcit.ContainsKey(targetId);
			if (flag)
			{
				this._relationDataDcit.Add(targetId, new List<ValueTuple<CharacterDisplayData, int>>());
			}
			this._relationDataDcit[targetId].Add(new ValueTuple<CharacterDisplayData, int>(characterDisplayData, (int)TeachSkillResultRelationType));
		}

		// Token: 0x0600619E RID: 24990 RVA: 0x002CCB00 File Offset: 0x002CAD00
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
					this.InitTeachLifeSkillItemView();
				}
			});
		}

		// Token: 0x0600619F RID: 24991 RVA: 0x002CCB40 File Offset: 0x002CAD40
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

		// Token: 0x060061A0 RID: 24992 RVA: 0x002CCB7C File Offset: 0x002CAD7C
		private short GetBookTemplateId(int bookId)
		{
			foreach (ItemKey item in ViewTeachLifeSkillResultConfirm._bookItemKeys)
			{
				bool flag = item.Id == bookId;
				if (flag)
				{
					return item.TemplateId;
				}
			}
			return -1;
		}

		// Token: 0x060061A1 RID: 24993 RVA: 0x002CCBE8 File Offset: 0x002CADE8
		private void InitTeachLifeSkillItemView()
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

		// Token: 0x060061A2 RID: 24994 RVA: 0x002CCC64 File Offset: 0x002CAE64
		private void OnItemRender(int index, TeachSkillItemView itemView)
		{
			ViewTeachLifeSkillResultConfirm.TeachResultItemData teachResultItemData = this._displayTeachResult[index];
			itemView.Refresh(teachResultItemData.CharacterDisplayData, teachResultItemData.ReadBookPageData, teachResultItemData.FavorabilityChangeData, teachResultItemData.RelationChangeData, this.GetRelation(teachResultItemData.CharId));
		}

		// Token: 0x060061A3 RID: 24995 RVA: 0x002CCCAC File Offset: 0x002CAEAC
		private void Update()
		{
			bool flag = CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				this.HideUI();
			}
		}

		// Token: 0x060061A4 RID: 24996 RVA: 0x002CCCF6 File Offset: 0x002CAEF6
		private void HideUI()
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			UIManager.Instance.HideUI(this.Element);
		}

		// Token: 0x060061A5 RID: 24997 RVA: 0x002CCD1C File Offset: 0x002CAF1C
		public static void SetBookItemKeys(List<ItemKey> bookItemKeys)
		{
			ViewTeachLifeSkillResultConfirm._bookItemKeys.Clear();
			ViewTeachLifeSkillResultConfirm._bookItemKeys.AddRange(bookItemKeys);
		}

		// Token: 0x040043B6 RID: 17334
		[SerializeField]
		private RectTransform content;

		// Token: 0x040043B7 RID: 17335
		private const int BackGroundWidth = 2560;

		// Token: 0x040043B8 RID: 17336
		private const int TitlePosition = 624;

		// Token: 0x040043B9 RID: 17337
		private const int UpperFramePosition = 587;

		// Token: 0x040043BA RID: 17338
		private const int LowerFramePosition = -587;

		// Token: 0x040043BB RID: 17339
		private const int BackGroundHeight = 1151;

		// Token: 0x040043BC RID: 17340
		private const float Duration = 0.2f;

		// Token: 0x040043BD RID: 17341
		private bool _inited;

		// Token: 0x040043BE RID: 17342
		private TasterUltimateResult _teachLifeSkillResult;

		// Token: 0x040043BF RID: 17343
		private List<ViewTeachLifeSkillResultConfirm.TeachResultItemData> _displayTeachResult;

		// Token: 0x040043C0 RID: 17344
		private static readonly List<ItemKey> _bookItemKeys = new List<ItemKey>();

		// Token: 0x040043C1 RID: 17345
		private Dictionary<int, List<ValueTuple<CharacterDisplayData, int>>> _relationDataDcit = new Dictionary<int, List<ValueTuple<CharacterDisplayData, int>>>();

		// Token: 0x040043C2 RID: 17346
		private byte _callBackCount;

		// Token: 0x02001D1F RID: 7455
		public struct TeachResultItemData
		{
			// Token: 0x0600EC4B RID: 60491 RVA: 0x00605590 File Offset: 0x00603790
			public TeachResultItemData(int charId, CharacterDisplayData characterDisplayData, Dictionary<short, byte> readBookPageData, Dictionary<CharacterDisplayData, bool> favorabilityChangeData, Dictionary<CharacterDisplayData, ushort> relationChangeData)
			{
				this.CharId = charId;
				this.CharacterDisplayData = characterDisplayData;
				this.ReadBookPageData = readBookPageData;
				this.FavorabilityChangeData = favorabilityChangeData;
				this.RelationChangeData = relationChangeData;
			}

			// Token: 0x0400C515 RID: 50453
			public int CharId;

			// Token: 0x0400C516 RID: 50454
			public CharacterDisplayData CharacterDisplayData;

			// Token: 0x0400C517 RID: 50455
			public Dictionary<short, byte> ReadBookPageData;

			// Token: 0x0400C518 RID: 50456
			public Dictionary<CharacterDisplayData, bool> FavorabilityChangeData;

			// Token: 0x0400C519 RID: 50457
			public Dictionary<CharacterDisplayData, ushort> RelationChangeData;
		}
	}
}
