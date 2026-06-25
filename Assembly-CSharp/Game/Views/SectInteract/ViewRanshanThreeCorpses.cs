using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Story.SectMainStory;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009B7 RID: 2487
	public class ViewRanshanThreeCorpses : UIBase
	{
		// Token: 0x06007888 RID: 30856 RVA: 0x003813FC File Offset: 0x0037F5FC
		public override void OnInit(ArgumentBox argsBox)
		{
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
		}

		// Token: 0x06007889 RID: 30857 RVA: 0x00381430 File Offset: 0x0037F630
		private void Awake()
		{
			this.InitBookKeepTips();
			for (int i = 0; i < this.corpses.childCount; i++)
			{
				this.corpses.GetChild(i).GetComponent<RanshanCorpse>().Init(i, this._bookKeepTipsData, new Action<ItemKey, int, int>(this.OnKeepingBookChange), new Action<int>(this.OnGiveUpClick));
			}
			this.giveUp.Init(new Action<int, sbyte, sbyte>(this.OnGiveUpConfirm));
		}

		// Token: 0x0600788A RID: 30858 RVA: 0x003814B0 File Offset: 0x0037F6B0
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "ButtonCloseView" == btnName;
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x0600788B RID: 30859 RVA: 0x003814E0 File Offset: 0x0037F6E0
		public override void QuickHide()
		{
			bool activeSelf = this.giveUp.gameObject.activeSelf;
			if (activeSelf)
			{
				this.giveUp.gameObject.SetActive(false);
			}
			else
			{
				base.QuickHide();
			}
		}

		// Token: 0x0600788C RID: 30860 RVA: 0x0038151E File Offset: 0x0037F71E
		private void RequestData()
		{
			ExtraDomainMethod.AsyncCall.GetSectRanshanThreeCorpsesData(this, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._data);
				List<sbyte> canKeepBooks = this._data.CanKeepBooks;
				int totalBookCount = (canKeepBooks != null) ? canKeepBooks.Count : 0;
				Dictionary<int, sbyte> legendaryBookOwners = this._data.LegendaryBookOwners;
				int totalTargetCount = (legendaryBookOwners != null) ? legendaryBookOwners.Count : 0;
				for (int i = 0; i < this.corpses.childCount; i++)
				{
					SectStoryThreeCorpsesCharacter data = this._data.ThreeCorpses[i];
					CharacterDisplayData corpseData = this._data.CharacterDisplayData[this._data.ThreeCorpses[i].Id];
					CharacterDisplayData targetData = this._data.CharacterDisplayData.GetValueOrDefault(this._data.ThreeCorpses[i].TargetOwner, null);
					Transform corpse = this.corpses.GetChild(i);
					corpse.gameObject.SetActive(true);
					corpse.GetComponent<RanshanCorpse>().Set(corpseData, targetData, data, totalBookCount, totalTargetCount);
					corpse.gameObject.SetActive(data.IsGoodEnd);
				}
				this.Element.ShowAfterRefresh();
			});
			CharacterDomainMethod.AsyncCall.GetCharacterItemsDisplayData(this, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool pool)
			{
				CharacterItemsDisplayData displayData = new CharacterItemsDisplayData();
				Serializer.Deserialize(pool, offset, ref displayData);
				this._resource = displayData.Resources;
				this._exp = displayData.Exp;
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x0600788D RID: 30861 RVA: 0x00381554 File Offset: 0x0037F754
		private void OnGiveUpConfirm(int corpseIndex, sbyte bookType, sbyte notch)
		{
			SectStoryThreeCorpsesCharacter data = this._data.ThreeCorpses[corpseIndex];
			short templateId = data.TemplateId;
			ExtraDomainMethod.Call.SetRanshanThreeCorpsesCharacterTarget(templateId, bookType, notch);
			this.ShowBookKeepBubble(ViewRanshanThreeCorpses.BookKeepingBubbleType.StartGiveUp, corpseIndex, -1);
			this.RequestData();
		}

		// Token: 0x0600788E RID: 30862 RVA: 0x00381595 File Offset: 0x0037F795
		private void OnGiveUpClick(int corpseIndex)
		{
			this.giveUp.Set(this._data, corpseIndex, this._resource, this._exp);
			this.giveUp.SelectChar(delegate
			{
				this.giveUp.gameObject.SetActive(true);
			});
		}

		// Token: 0x0600788F RID: 30863 RVA: 0x003815D0 File Offset: 0x0037F7D0
		private void OnKeepingBookChange(ItemKey itemKey, int corpseIndex, int slotIndex)
		{
			sbyte bookType = this.GetLegendaryBookType(itemKey);
			int corpseIndex2 = -1;
			bool flag = bookType != -1;
			if (flag)
			{
				for (int i = 0; i < this.corpses.childCount; i++)
				{
					bool flag2 = this.corpses.GetChild(i).GetComponent<RanshanCorpse>().TryRemove(bookType);
					if (flag2)
					{
						corpseIndex2 = i;
						break;
					}
				}
			}
			this.corpses.GetChild(corpseIndex).GetComponent<RanshanCorpse>().SetType(bookType, slotIndex);
			for (int j = 0; j < this.corpses.childCount; j++)
			{
				this.corpses.GetChild(j).GetComponent<RanshanCorpse>().RefreshSlots();
			}
			ExtraDomainMethod.Call.ApplyRanshanThreeCorpsesLegendaryBookKeepingResult(this._data.ThreeCorpses[0].LegendaryBooks, this._data.ThreeCorpses[1].LegendaryBooks, this._data.ThreeCorpses[2].LegendaryBooks);
			ViewRanshanThreeCorpses.BookKeepingBubbleType type = (corpseIndex2 >= 0) ? ViewRanshanThreeCorpses.BookKeepingBubbleType.Move : ((bookType != -1) ? ViewRanshanThreeCorpses.BookKeepingBubbleType.Save : ViewRanshanThreeCorpses.BookKeepingBubbleType.Load);
			this.ShowBookKeepBubble(type, corpseIndex, corpseIndex2);
		}

		// Token: 0x06007890 RID: 30864 RVA: 0x003816F0 File Offset: 0x0037F8F0
		private void ShowBookKeepBubble(ViewRanshanThreeCorpses.BookKeepingBubbleType bubbleType, int corpseIndex1, int corpseIndex2)
		{
			string bubbleText = null;
			short templateId = this._data.ThreeCorpses[corpseIndex1].TemplateId;
			LanguageKey key = ViewRanshanThreeCorpses.CorpseBubbleConfig[templateId][(int)bubbleType];
			switch (bubbleType)
			{
			case ViewRanshanThreeCorpses.BookKeepingBubbleType.Save:
			case ViewRanshanThreeCorpses.BookKeepingBubbleType.Load:
			case ViewRanshanThreeCorpses.BookKeepingBubbleType.StartGiveUp:
			case ViewRanshanThreeCorpses.BookKeepingBubbleType.StopGiveUp:
				bubbleText = LocalStringManager.Get(key);
				break;
			case ViewRanshanThreeCorpses.BookKeepingBubbleType.Move:
			{
				CharacterDisplayData sourceData = this._data.CharacterDisplayData[this._data.ThreeCorpses[corpseIndex2].Id];
				string sourceName = NameCenter.GetMonasticTitleOrDisplayName(sourceData, false);
				bubbleText = LocalStringManager.GetFormat(key, sourceName);
				break;
			}
			}
			this.corpses.GetChild(corpseIndex1).GetComponent<RanshanCorpse>().ShowBubble(bubbleText);
		}

		// Token: 0x06007891 RID: 30865 RVA: 0x003817A8 File Offset: 0x0037F9A8
		private sbyte GetLegendaryBookType(ItemKey key)
		{
			return key.Equals(ItemKey.Invalid) ? -1 : ((sbyte)(key.TemplateId - 240));
		}

		// Token: 0x06007892 RID: 30866 RVA: 0x003817D8 File Offset: 0x0037F9D8
		private void InitBookKeepTips()
		{
			this._bookKeepTipsData.Clear();
			this._bookKeepTipsData.Add(new GeneralLineData(3, new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Keeping_Tips_Desc1)
			}, null));
			this._bookKeepTipsData.Add(new GeneralLineData(6, new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Keeping_Tips_SubTitle1),
				LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Keeping_Tips_SubDesc1)
			}, new List<object>
			{
				120f
			}));
			this._bookKeepTipsData.Add(new GeneralLineData(6, new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Keeping_Tips_SubTitle2),
				LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Keeping_Tips_SubDesc2)
			}, new List<object>
			{
				120f
			}));
		}

		// Token: 0x06007894 RID: 30868 RVA: 0x003818D0 File Offset: 0x0037FAD0
		// Note: this type is marked as 'beforefieldinit'.
		static ViewRanshanThreeCorpses()
		{
			Dictionary<short, LanguageKey[]> dictionary = new Dictionary<short, LanguageKey[]>();
			Dictionary<short, LanguageKey[]> dictionary2 = dictionary;
			short key = 698;
			LanguageKey[] array = new LanguageKey[5];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.A52698D7D4C9D40F49C84C2903BE995CC84FCA37C741CFE40EB4DB13342937F5).FieldHandle);
			dictionary2.Add(key, array);
			Dictionary<short, LanguageKey[]> dictionary3 = dictionary;
			short key2 = 699;
			LanguageKey[] array2 = new LanguageKey[5];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.31EEEDF0C3484EF7CA07C5D55D460AEED7F5FD9045FF8022DBDA88361889581B).FieldHandle);
			dictionary3.Add(key2, array2);
			Dictionary<short, LanguageKey[]> dictionary4 = dictionary;
			short key3 = 700;
			LanguageKey[] array3 = new LanguageKey[5];
			RuntimeHelpers.InitializeArray(array3, fieldof(<PrivateImplementationDetails>.0DD59078819C1207F405B8A95D74F910208170AF7160D516ED37154B2FB75825).FieldHandle);
			dictionary4.Add(key3, array3);
			ViewRanshanThreeCorpses.CorpseBubbleConfig = dictionary;
		}

		// Token: 0x04005B3D RID: 23357
		public Transform corpses;

		// Token: 0x04005B3E RID: 23358
		public RanshanGiveUpBook giveUp;

		// Token: 0x04005B3F RID: 23359
		private SectRanshanThreeCorpsesData _data;

		// Token: 0x04005B40 RID: 23360
		private ResourceInts _resource;

		// Token: 0x04005B41 RID: 23361
		private int _exp;

		// Token: 0x04005B42 RID: 23362
		private readonly List<GeneralLineData> _bookKeepTipsData = new List<GeneralLineData>();

		// Token: 0x04005B43 RID: 23363
		private static readonly Dictionary<short, LanguageKey[]> CorpseBubbleConfig;

		// Token: 0x02001EFD RID: 7933
		private enum BookKeepingBubbleType
		{
			// Token: 0x0400CBD7 RID: 52183
			Save,
			// Token: 0x0400CBD8 RID: 52184
			Load,
			// Token: 0x0400CBD9 RID: 52185
			Move,
			// Token: 0x0400CBDA RID: 52186
			StartGiveUp,
			// Token: 0x0400CBDB RID: 52187
			StopGiveUp
		}
	}
}
