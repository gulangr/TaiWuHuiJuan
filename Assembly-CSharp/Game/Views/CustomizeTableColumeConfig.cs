using System;

namespace Game.Views
{
	// Token: 0x020006E6 RID: 1766
	public class CustomizeTableColumeConfig
	{
		// Token: 0x17000A59 RID: 2649
		// (get) Token: 0x060053D0 RID: 21456 RVA: 0x0026D662 File Offset: 0x0026B862
		public string ElementLocalName
		{
			get
			{
				return string.IsNullOrEmpty(this.ElementConfigName) ? ((this.ElementNameKey == LanguageKey.Invalid) ? string.Empty : LocalStringManager.Get(this.ElementNameKey)) : this.ElementConfigName;
			}
		}

		// Token: 0x060053D1 RID: 21457 RVA: 0x0026D694 File Offset: 0x0026B894
		public CustomizeTableColumeConfig(int columeId, int elementType, LanguageKey elementNameKey, float width, bool canSort = true)
		{
			this.ColumeId = columeId;
			this.ElementType = elementType;
			this.ElementNameKey = elementNameKey;
			this.Width = width;
			this.CanSort = canSort;
			this.ElementConfigName = string.Empty;
		}

		// Token: 0x060053D2 RID: 21458 RVA: 0x0026D6CE File Offset: 0x0026B8CE
		public CustomizeTableColumeConfig(int columeId, int elementType, string elementConfigName, float width, bool canSort = true)
		{
			this.ColumeId = columeId;
			this.ElementType = elementType;
			this.ElementConfigName = elementConfigName;
			this.Width = width;
			this.CanSort = canSort;
		}

		// Token: 0x040038B1 RID: 14513
		public int ColumeId;

		// Token: 0x040038B2 RID: 14514
		public int ElementType;

		// Token: 0x040038B3 RID: 14515
		public LanguageKey ElementNameKey;

		// Token: 0x040038B4 RID: 14516
		public float Width;

		// Token: 0x040038B5 RID: 14517
		public bool CanSort;

		// Token: 0x040038B6 RID: 14518
		public string ElementConfigName;
	}
}
