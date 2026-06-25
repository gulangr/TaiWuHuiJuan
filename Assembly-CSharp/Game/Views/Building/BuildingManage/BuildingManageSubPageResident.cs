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
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BFF RID: 3071
	public class BuildingManageSubPageResident : BuildingManageSubPage
	{
		// Token: 0x1700107F RID: 4223
		// (get) Token: 0x06009C19 RID: 39961 RVA: 0x00491E94 File Offset: 0x00490094
		// (set) Token: 0x06009C1A RID: 39962 RVA: 0x00491E9C File Offset: 0x0049009C
		public bool DataChanged { get; set; }

		// Token: 0x06009C1B RID: 39963 RVA: 0x00491EA8 File Offset: 0x004900A8
		private void OnDisable()
		{
			bool dataChanged = this.DataChanged;
			if (dataChanged)
			{
				this.DataChanged = false;
				GEvent.OnEvent(UiEvents.UpdateAllBlockInfo, null);
			}
		}

		// Token: 0x06009C1C RID: 39964 RVA: 0x00491ED8 File Offset: 0x004900D8
		private void Awake()
		{
			this.quickCheckInBtn.ClearAndAddListener(delegate
			{
				BuildingDomainMethod.Call.QuickFillResidence(this.ParentView.Element.GameDataListenerId, this.ParentView.BlockKey);
				this.ParentView.RequestData();
			});
			this.quickCheckOutBtn.ClearAndAddListener(delegate
			{
				BuildingDomainMethod.Call.RemoveAllFormResidence(this.ParentView.BlockKey);
				this.ParentView.RequestData();
			});
			this.autoCheckInToggle.onValueChanged.AddListener(delegate(bool v)
			{
				BuildingDomainMethod.Call.SetResidenceAutoCheckIn(this.ParentView.BlockKey.BuildingBlockIndex, v);
			});
			this.infinityScroll.OnItemRender += this.OnItemRender;
		}

		// Token: 0x06009C1D RID: 39965 RVA: 0x00491F4C File Offset: 0x0049014C
		private void OnItemRender(int index, GameObject obj)
		{
			BuildingManagerMemberView memberView = obj.GetComponent<BuildingManagerMemberView>();
			List<CharacterDisplayData> residences = this.DisplayData.Residences;
			CharacterDisplayData data = (residences != null && residences.CheckIndex(index)) ? this.DisplayData.Residences[index] : null;
			int characterId = (data != null) ? data.CharacterId : -1;
			memberView.SetForResident(data, index, !this._lockedSet.Contains(characterId), new Action<int>(this.<OnItemRender>g__SelectChar|14_0), new Action<int>(this.<OnItemRender>g__CancelChar|14_1), new Action<int, bool>(this.<OnItemRender>g__SetUnlockChar|14_2));
		}

		// Token: 0x06009C1E RID: 39966 RVA: 0x00491FDC File Offset: 0x004901DC
		public override void Init(ViewBuildingManage parentView)
		{
			base.Init(parentView);
			this._lastCount = -1;
		}

		// Token: 0x06009C1F RID: 39967 RVA: 0x00491FF0 File Offset: 0x004901F0
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			base.Refresh(displayData);
			sbyte level = this.ParentView.BuildingModel.GetBuildingLevel(this.ParentView.BlockKey, this.DisplayData.BlockData);
			this._capacity = BuildingScale.DefValue.ResidenceCapacity.GetLevelEffect((int)level);
			this.SetTitle();
			this.autoCheckInToggle.SetIsOnWithoutNotify(this.DisplayData.AutoCheckIn);
			this._lockedSet.Clear();
			bool flag = displayData.LockedResidences != null;
			if (flag)
			{
				foreach (int item in displayData.LockedResidences)
				{
					this._lockedSet.Add(item);
				}
			}
			this.infinityScroll.SetDataCount(this._capacity);
		}

		// Token: 0x06009C20 RID: 39968 RVA: 0x004920D8 File Offset: 0x004902D8
		private void SetTitle()
		{
			List<CharacterDisplayData> residences = this.DisplayData.Residences;
			int count = (residences != null) ? residences.Count : 0;
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

		// Token: 0x06009C21 RID: 39969 RVA: 0x004921A8 File Offset: 0x004903A8
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

		// Token: 0x06009C22 RID: 39970 RVA: 0x004921F4 File Offset: 0x004903F4
		private void SelectCharacterCallback(List<int> charIdList)
		{
			BuildingManageSubPageResident.<>c__DisplayClass19_0 CS$<>8__locals1 = new BuildingManageSubPageResident.<>c__DisplayClass19_0();
			CS$<>8__locals1.charIdList = charIdList;
			BuildingManageSubPageResident.<>c__DisplayClass19_0 CS$<>8__locals2 = CS$<>8__locals1;
			List<CharacterDisplayData> residences = this.DisplayData.Residences;
			List<int> list;
			if (residences == null)
			{
				list = null;
			}
			else
			{
				list = (from x in residences
				select x.CharacterId).ToList<int>();
			}
			CS$<>8__locals2.currentIds = (list ?? new List<int>());
			List<int> toRemove = (from id in CS$<>8__locals1.currentIds
			where id > 0 && !CS$<>8__locals1.charIdList.Contains(id)
			select id).ToList<int>();
			List<int> toAdd = (from id in CS$<>8__locals1.charIdList
			where id > 0 && !CS$<>8__locals1.currentIds.Contains(id)
			select id).ToList<int>();
			foreach (int charId in toRemove)
			{
				BuildingDomainMethod.Call.RemoveFromResidence(-1, charId, this.ParentView.BlockKey);
			}
			foreach (int charId2 in toAdd)
			{
				BuildingDomainMethod.Call.AddToResidence(charId2, this.ParentView.BlockKey);
			}
			this.ParentView.RequestData();
		}

		// Token: 0x06009C27 RID: 39975 RVA: 0x004923C8 File Offset: 0x004905C8
		[CompilerGenerated]
		private void <OnItemRender>g__SelectChar|14_0(int index)
		{
			List<int> totalData = new List<int>();
			HashSet<int> lockedData = new HashSet<int>();
			BuildingDomainMethod.AsyncCall.GetAllResidents(this.ParentView, BuildingBlockKey.Invalid, false, delegate(int offset, RawDataPool dataPool)
			{
				List<CharacterList> temp = new List<CharacterList>();
				Serializer.Deserialize(dataPool, offset, ref temp);
				for (int i = 0; i < temp.Count; i++)
				{
					totalData.AddRange(temp[i].GetCollection());
				}
			});
			BuildingDomainMethod.AsyncCall.GetLockedInResidenceIds(this.ParentView, delegate(int offset, RawDataPool dataPool)
			{
				List<int> temp = new List<int>();
				Serializer.Deserialize(dataPool, offset, ref temp);
				for (int i = 0; i < temp.Count; i++)
				{
					lockedData.Add(temp[i]);
				}
				BuildingManageSubPageResident <>4__this = this;
				int capacity = this._capacity;
				BuildingManageDisplayData displayData = this.DisplayData;
				List<int> selectedList;
				if (displayData == null)
				{
					selectedList = null;
				}
				else
				{
					List<CharacterDisplayData> residences = displayData.Residences;
					if (residences == null)
					{
						selectedList = null;
					}
					else
					{
						selectedList = (from x in residences
						select x.CharacterId).ToList<int>();
					}
				}
				<>4__this.ShowSelectCharacterUI(capacity, selectedList, lockedData, new SelectCharacterCallback(this.SelectCharacterCallback), totalData);
			});
		}

		// Token: 0x06009C28 RID: 39976 RVA: 0x00492430 File Offset: 0x00490630
		[CompilerGenerated]
		private void <OnItemRender>g__CancelChar|14_1(int index)
		{
			List<CharacterDisplayData> residences = this.DisplayData.Residences;
			bool flag = residences != null && residences.CheckIndex(index);
			if (flag)
			{
				BuildingDomainMethod.Call.RemoveFromResidence(-1, this.DisplayData.Residences[index].CharacterId, this.ParentView.BlockKey);
				this.ParentView.RequestData();
			}
		}

		// Token: 0x06009C29 RID: 39977 RVA: 0x00492490 File Offset: 0x00490690
		[CompilerGenerated]
		private void <OnItemRender>g__SetUnlockChar|14_2(int characterId, bool isUnlock)
		{
			bool flag = characterId > 0;
			if (flag)
			{
				if (isUnlock)
				{
					BuildingDomainMethod.Call.UnlockResidenceCharacter(-1, this.ParentView.BlockKey, characterId);
				}
				else
				{
					BuildingDomainMethod.Call.LockResidenceCharacter(-1, this.ParentView.BlockKey, characterId);
				}
				this.ParentView.RequestData();
			}
		}

		// Token: 0x040078E3 RID: 30947
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x040078E4 RID: 30948
		[SerializeField]
		private CButton quickCheckInBtn;

		// Token: 0x040078E5 RID: 30949
		[SerializeField]
		private CButton quickCheckOutBtn;

		// Token: 0x040078E6 RID: 30950
		[SerializeField]
		private CToggle autoCheckInToggle;

		// Token: 0x040078E7 RID: 30951
		[SerializeField]
		private InfinityScroll infinityScroll;

		// Token: 0x040078E8 RID: 30952
		private int _capacity;

		// Token: 0x040078E9 RID: 30953
		private int _lastCount = -1;

		// Token: 0x040078EA RID: 30954
		private readonly HashSet<int> _lockedSet = new HashSet<int>();
	}
}
