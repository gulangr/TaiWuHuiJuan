using System;
using System.Collections.Generic;
using Config;
using GameData.Serializer;
using UnityEngine;

namespace AdventureEditor.Beta
{
	// Token: 0x020006A5 RID: 1701
	public abstract class EditingAdventureNode : GameData.Serializer.ICommonObjectSerializationAware
	{
		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x06004F8D RID: 20365 RVA: 0x00253884 File Offset: 0x00251A84
		public virtual string Name
		{
			get
			{
				return this.NodeGuid;
			}
		}

		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x06004F8E RID: 20366 RVA: 0x0025388C File Offset: 0x00251A8C
		protected virtual LanguageKey UnSpecifiedTerrainLanguageId
		{
			get
			{
				return LanguageKey.UI_AdventureEditor_Terrain_NotSpecified_StartNode;
			}
		}

		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x06004F8F RID: 20367 RVA: 0x00253893 File Offset: 0x00251A93
		public bool Dirty
		{
			get
			{
				return this._dirty;
			}
		}

		// Token: 0x06004F90 RID: 20368 RVA: 0x0025389B File Offset: 0x00251A9B
		public void InitializeOnDeserializing()
		{
			this.TerrainId = -1;
		}

		// Token: 0x06004F91 RID: 20369 RVA: 0x002538A5 File Offset: 0x00251AA5
		public void FinishedDeserialization()
		{
			string nodeDesc = this.NodeDesc;
			this.NodeDesc = ((nodeDesc != null) ? nodeDesc.Replace("<NL>", "\n") : null);
		}

		// Token: 0x06004F92 RID: 20370 RVA: 0x002538CC File Offset: 0x00251ACC
		public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
		{
			bool result;
			if (!(name == "MapPosition"))
			{
				proc = default(GameData.Serializer.CommonObjectSerializationMember);
				result = false;
			}
			else
			{
				proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<Dictionary<string, string>>(name, delegate(Dictionary<string, string> v)
				{
					float x;
					this.PositionOnMap.x = (float.TryParse(v["X"], out x) ? x : 0f);
					float y;
					this.PositionOnMap.y = (float.TryParse(v["Y"], out y) ? y : 0f);
				});
				result = true;
			}
			return result;
		}

		// Token: 0x06004F93 RID: 20371 RVA: 0x00253915 File Offset: 0x00251B15
		public void SetDirty(bool dirty)
		{
			this._dirty = dirty;
		}

		// Token: 0x06004F94 RID: 20372 RVA: 0x00253920 File Offset: 0x00251B20
		public override string ToString()
		{
			string nodeTerrain = LocalStringManager.Get(this.UnSpecifiedTerrainLanguageId);
			bool flag = this.TerrainId != -1;
			if (flag)
			{
				AdventureTerrainItem terrainConfig = AdventureTerrain.Instance[this.TerrainId];
				nodeTerrain = LocalStringManager.GetFormat(LanguageKey.UI_AdventureEditor_Terrain_Specified, terrainConfig.Name);
			}
			return LocalStringManager.GetFormat(LanguageKey.UI_AdventureEditor_NodeContentBase, this.Name + "\n" + this.NodeTitle, this.NodeDesc, nodeTerrain);
		}

		// Token: 0x040036C2 RID: 14018
		public string EventId;

		// Token: 0x040036C3 RID: 14019
		public int TerrainId;

		// Token: 0x040036C4 RID: 14020
		public string Key;

		// Token: 0x040036C5 RID: 14021
		public string NodeTitle;

		// Token: 0x040036C6 RID: 14022
		public ushort SortIndex;

		// Token: 0x040036C7 RID: 14023
		public Vector2 PositionOnMap;

		// Token: 0x040036C8 RID: 14024
		public string NodeGuid = Guid.NewGuid().ToString("N");

		// Token: 0x040036C9 RID: 14025
		public string NodeDesc;

		// Token: 0x040036CA RID: 14026
		private bool _dirty;
	}
}
