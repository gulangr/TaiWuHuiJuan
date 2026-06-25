using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Jieqing
{
	// Token: 0x02000D27 RID: 3367
	public class JieqingMurderMapStateMenu : DynamicDetailedFilterMenuLogic<CharacterDisplayData>
	{
		// Token: 0x170011A2 RID: 4514
		// (get) Token: 0x0600A779 RID: 42873 RVA: 0x004DF160 File Offset: 0x004DD360
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170011A3 RID: 4515
		// (get) Token: 0x0600A77A RID: 42874 RVA: 0x004DF163 File Offset: 0x004DD363
		public override int Id
		{
			get
			{
				return EJieqingMurderMenuId.MapState.ToInt();
			}
		}

		// Token: 0x170011A4 RID: 4516
		// (get) Token: 0x0600A77B RID: 42875 RVA: 0x004DF170 File Offset: 0x004DD370
		private WorldMapModel _mapModel
		{
			get
			{
				return SingletonObject.getInstance<WorldMapModel>();
			}
		}

		// Token: 0x0600A77C RID: 42876 RVA: 0x004DF178 File Offset: 0x004DD378
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_Area;
		}

		// Token: 0x0600A77D RID: 42877 RVA: 0x004DF194 File Offset: 0x004DD394
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return null;
		}

		// Token: 0x0600A77E RID: 42878 RVA: 0x004DF1A8 File Offset: 0x004DD3A8
		public override List<FilterDropdownItemConfig> GetDynamicMenuConfigs(IEnumerable<CharacterDisplayData> dataList)
		{
			HashSet<short> typeSet = new HashSet<short>();
			foreach (CharacterDisplayData data in dataList)
			{
				short areaId = data.Location.AreaId;
				bool flag = areaId >= 0 && areaId < 135;
				if (flag)
				{
					typeSet.Add((short)this._mapModel.GetStateTemplateIdByAreaId(data.Location.AreaId));
				}
			}
			this._mapStateIds.Clear();
			this._mapStateIds.AddRange(typeSet);
			this._mapStateIds.Sort();
			return (from type in this._mapStateIds
			select new FilterDropdownItemConfig
			{
				Text = StringKey.CreateDirect(MapState.Instance[(int)type].Name)
			}).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600A77F RID: 42879 RVA: 0x004DF28C File Offset: 0x004DD48C
		public override bool IsDataMatch(CharacterDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			short areaId = data.Location.AreaId;
			return areaId >= 0 && areaId < 135 && selectedIndices.Any((int index) => this._mapStateIds[index] == (short)this._mapModel.GetStateTemplateIdByAreaId(data.Location.AreaId));
		}

		// Token: 0x0400836F RID: 33647
		private readonly List<short> _mapStateIds = new List<short>();
	}
}
