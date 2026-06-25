using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Select;
using Game.Views.Select.SelectCharacter;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BF8 RID: 3064
	public class BuildingManageSubPageComfortableHouse : BuildingManageSubPage
	{
		// Token: 0x17001072 RID: 4210
		// (get) Token: 0x06009BC4 RID: 39876 RVA: 0x0048FB68 File Offset: 0x0048DD68
		// (set) Token: 0x06009BC5 RID: 39877 RVA: 0x0048FB70 File Offset: 0x0048DD70
		public bool DataChanged { get; set; }

		// Token: 0x06009BC6 RID: 39878 RVA: 0x0048FB7C File Offset: 0x0048DD7C
		private void OnDisable()
		{
			bool dataChanged = this.DataChanged;
			if (dataChanged)
			{
				this.DataChanged = false;
				GEvent.OnEvent(UiEvents.UpdateAllBlockInfo, null);
			}
		}

		// Token: 0x06009BC7 RID: 39879 RVA: 0x0048FBAC File Offset: 0x0048DDAC
		private void Awake()
		{
			this.quickCheckInBtn.ClearAndAddListener(delegate
			{
				BuildingDomainMethod.Call.QuickFillComfortableHouse(this.ParentView.Element.GameDataListenerId, this.ParentView.BlockKey);
				this.ParentView.RequestData();
			});
			this.quickCheckOutBtn.ClearAndAddListener(delegate
			{
				BuildingDomainMethod.Call.RemoveAllFromComfortableHouse(this.ParentView.BlockKey);
				this.ParentView.RequestData();
			});
			this.autoCheckInDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnChangeDropdown));
			this.autoCheckInDropdown.AddOptions(new List<string>
			{
				LanguageKey.LK_Building_ComfortableInfo9.Tr(),
				LanguageKey.LK_Building_ComfortableInfo5.Tr(),
				LanguageKey.LK_Building_ComfortableInfo6.Tr()
			});
			this.infinityScroll.OnItemRender += this.OnItemRender;
		}

		// Token: 0x06009BC8 RID: 39880 RVA: 0x0048FC63 File Offset: 0x0048DE63
		private void OnChangeDropdown(int index)
		{
			BuildingDomainMethod.Call.SetComfortableAutoCheckIn(this.ParentView.BlockKey.BuildingBlockIndex, index != 0);
			BuildingDomainMethod.Call.SetComfortableAutoCheckInType(this.ParentView.BlockKey, index == 1);
		}

		// Token: 0x06009BC9 RID: 39881 RVA: 0x0048FC98 File Offset: 0x0048DE98
		private void OnItemRender(int index, GameObject obj)
		{
			BuildingManagerMemberView memberView = obj.GetComponent<BuildingManagerMemberView>();
			CharacterDisplayData data = this.DisplayData.ComfortableHouses.CheckIndex(index) ? this.DisplayData.ComfortableHouses[index] : null;
			int characterId = (data != null) ? data.CharacterId : -1;
			memberView.SetForResident(data, index, !this._lockedSet.Contains(characterId), new Action<int>(this.<OnItemRender>g__SelectChar|15_0), new Action<int>(this.<OnItemRender>g__CancelChar|15_1), new Action<int, bool>(this.<OnItemRender>g__SetUnlockChar|15_2));
		}

		// Token: 0x06009BCA RID: 39882 RVA: 0x0048FD21 File Offset: 0x0048DF21
		public override void Init(ViewBuildingManage parentView)
		{
			base.Init(parentView);
			this._lastCount = -1;
		}

		// Token: 0x06009BCB RID: 39883 RVA: 0x0048FD34 File Offset: 0x0048DF34
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			base.Refresh(displayData);
			sbyte level = this.ParentView.BuildingModel.GetBuildingLevel(this.ParentView.BlockKey, this.DisplayData.BlockData);
			this._capacity = BuildingScale.DefValue.ComfortableHouseCapacity.GetLevelEffect((int)level);
			this.SetTitle();
			this.autoCheckInDropdown.SetValueWithoutNotify(displayData.AutoCheckIn ? (displayData.AutoCheckInType ? 1 : 2) : 0);
			this._lockedSet.Clear();
			bool flag = displayData.LockedComfortableHouses != null;
			if (flag)
			{
				foreach (int item in displayData.LockedComfortableHouses)
				{
					this._lockedSet.Add(item);
				}
			}
			this.infinityScroll.SetDataCount(this._capacity);
		}

		// Token: 0x06009BCC RID: 39884 RVA: 0x0048FE28 File Offset: 0x0048E028
		private void SetTitle()
		{
			List<CharacterDisplayData> comfortableHouses = this.DisplayData.ComfortableHouses;
			int count = (comfortableHouses != null) ? comfortableHouses.Count : 0;
			bool flag = this._lastCount != -1 && this._lastCount != count;
			if (flag)
			{
				this.DataChanged = true;
			}
			this._lastCount = count;
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Append(LocalStringManager.Get((this.ParentView.ConfigData.TemplateId == 46) ? LanguageKey.LK_Building_ResidentInfo : LanguageKey.LK_Building_ComfortableInfo2)).Append("  (").Append(count).Append("/").Append(this._capacity).Append(")");
			this.title.text = sb.ToString();
			EasyPool.Free<StringBuilder>(sb);
		}

		// Token: 0x06009BCD RID: 39885 RVA: 0x0048FEF8 File Offset: 0x0048E0F8
		private void ShowSelectCharacterUI(int targetCount, List<int> selectedList, HashSet<int> bannedCharacterIds, SelectCharacterCallback callback, List<int> charIds)
		{
			TaiwuDomainMethod.AsyncCall.GetVillagersForWorkDisplayData(this.ParentView, charIds, delegate(int offset, RawDataPool pool)
			{
				List<VillagerSelectCharacterDisplayData> displayData = new List<VillagerSelectCharacterDisplayData>();
				Serializer.Deserialize(pool, offset, ref displayData);
				List<ISelectCharacterData> selectList = (from item in displayData
				select new VillagerSelectCharacterDataAdapter(item)).ToList<ISelectCharacterData>();
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.Villager);
				config.InteractionMode = ESelectCharacterInteractionMode.Slot;
				config.SelectionMode = ESelectCharacterSelectionMode.Multiple;
				config.TargetCount = targetCount;
				config.InitialSelectedCharacterIds = selectedList;
				config.BannedCharacterIds = bannedCharacterIds;
				config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender,
					ESelectCharacterFilterMenuId.BehaviorType,
					ESelectCharacterFilterMenuId.Relation,
					ESelectCharacterFilterMenuId.AdoreRelation,
					ESelectCharacterFilterMenuId.EnemyRelation,
					ESelectCharacterFilterMenuId.WorkStatus,
					ESelectCharacterFilterMenuId.RoleArrangementWork,
					ESelectCharacterFilterMenuId.Identity
				};
				UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", selectList).SetObject("SelectCharacterCallback", callback));
				UIManager.Instance.MaskUI(UIElement.SelectChar);
			});
		}

		// Token: 0x06009BCE RID: 39886 RVA: 0x0048FF44 File Offset: 0x0048E144
		private void SelectCharacterCallback(List<int> charIdList)
		{
			BuildingManageSubPageComfortableHouse.<>c__DisplayClass20_0 CS$<>8__locals1 = new BuildingManageSubPageComfortableHouse.<>c__DisplayClass20_0();
			CS$<>8__locals1.charIdList = charIdList;
			BuildingManageSubPageComfortableHouse.<>c__DisplayClass20_0 CS$<>8__locals2 = CS$<>8__locals1;
			List<CharacterDisplayData> comfortableHouses = this.DisplayData.ComfortableHouses;
			List<int> currentIds;
			if (comfortableHouses == null)
			{
				currentIds = null;
			}
			else
			{
				currentIds = (from x in comfortableHouses
				select x.CharacterId).ToList<int>();
			}
			CS$<>8__locals2.currentIds = currentIds;
			List<int> currentIds2 = CS$<>8__locals1.currentIds;
			List<int> toRemove = (currentIds2 != null) ? (from id in currentIds2
			where id > 0 && !CS$<>8__locals1.charIdList.Contains(id)
			select id).ToList<int>() : null;
			List<int> charIdList2 = CS$<>8__locals1.charIdList;
			List<int> toAdd = (charIdList2 != null) ? charIdList2.Where(delegate(int id)
			{
				bool result;
				if (id > 0)
				{
					List<int> currentIds3 = CS$<>8__locals1.currentIds;
					result = (currentIds3 == null || !currentIds3.Contains(id));
				}
				else
				{
					result = false;
				}
				return result;
			}).ToList<int>() : null;
			bool flag = toRemove != null;
			if (flag)
			{
				foreach (int charId in toRemove)
				{
					BuildingDomainMethod.Call.RemoveFromComfortableHouse(-1, charId, this.ParentView.BlockKey);
				}
			}
			bool flag2 = toAdd != null;
			if (flag2)
			{
				foreach (int charId2 in toAdd)
				{
					BuildingDomainMethod.Call.AddToComfortableHouse(-1, charId2, this.ParentView.BlockKey);
				}
			}
			this.ParentView.RequestData();
		}

		// Token: 0x06009BD2 RID: 39890 RVA: 0x00490114 File Offset: 0x0048E314
		[CompilerGenerated]
		private void <OnItemRender>g__SelectChar|15_0(int index)
		{
			List<int> totalData = new List<int>();
			HashSet<int> lockedData = new HashSet<int>();
			BuildingDomainMethod.AsyncCall.GetFeastTargetCharList(this.ParentView, this.ParentView.BlockKey, delegate(int offset, RawDataPool dataPool)
			{
				List<CharacterDisplayData> temp = new List<CharacterDisplayData>();
				Serializer.Deserialize(dataPool, offset, ref temp);
				for (int i = 0; i < temp.Count; i++)
				{
					totalData.Add(temp[i].CharacterId);
				}
			});
			BuildingDomainMethod.AsyncCall.GetLockedInComfortableHouseIds(this.ParentView, delegate(int offset, RawDataPool dataPool)
			{
				List<int> temp = new List<int>();
				Serializer.Deserialize(dataPool, offset, ref temp);
				for (int i = 0; i < temp.Count; i++)
				{
					lockedData.Add(temp[i]);
				}
				BuildingManageSubPageComfortableHouse <>4__this = this;
				int capacity = this._capacity;
				List<CharacterDisplayData> comfortableHouses = this.DisplayData.ComfortableHouses;
				List<int> selectedList;
				if (comfortableHouses == null)
				{
					selectedList = null;
				}
				else
				{
					selectedList = (from x in comfortableHouses
					select x.CharacterId).ToList<int>();
				}
				<>4__this.ShowSelectCharacterUI(capacity, selectedList, lockedData, new SelectCharacterCallback(this.SelectCharacterCallback), totalData);
			});
		}

		// Token: 0x06009BD3 RID: 39891 RVA: 0x00490180 File Offset: 0x0048E380
		[CompilerGenerated]
		private void <OnItemRender>g__CancelChar|15_1(int index)
		{
			bool flag = this.DisplayData.ComfortableHouses.CheckIndex(index);
			if (flag)
			{
				BuildingDomainMethod.Call.RemoveFromComfortableHouse(-1, this.DisplayData.ComfortableHouses[index].CharacterId, this.ParentView.BlockKey);
				this.ParentView.RequestData();
			}
		}

		// Token: 0x06009BD4 RID: 39892 RVA: 0x004901DC File Offset: 0x0048E3DC
		[CompilerGenerated]
		private void <OnItemRender>g__SetUnlockChar|15_2(int characterId, bool isUnlock)
		{
			bool flag = characterId > 0;
			if (flag)
			{
				if (isUnlock)
				{
					BuildingDomainMethod.Call.UnlockComfortableHouseCharacter(-1, this.ParentView.BlockKey, characterId);
				}
				else
				{
					BuildingDomainMethod.Call.LockComfortableHouseCharacter(-1, this.ParentView.BlockKey, characterId);
				}
				this.ParentView.RequestData();
			}
		}

		// Token: 0x040078B2 RID: 30898
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x040078B3 RID: 30899
		[SerializeField]
		private CButton quickCheckInBtn;

		// Token: 0x040078B4 RID: 30900
		[SerializeField]
		private CButton quickCheckOutBtn;

		// Token: 0x040078B5 RID: 30901
		[SerializeField]
		private CDropdown autoCheckInDropdown;

		// Token: 0x040078B6 RID: 30902
		[SerializeField]
		private InfinityScroll infinityScroll;

		// Token: 0x040078B7 RID: 30903
		private int _capacity;

		// Token: 0x040078B8 RID: 30904
		private int _lastCount = -1;

		// Token: 0x040078B9 RID: 30905
		private readonly HashSet<int> _lockedSet = new HashSet<int>();
	}
}
