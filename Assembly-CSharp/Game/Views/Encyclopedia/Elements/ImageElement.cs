using System;
using System.Linq;
using UnityEngine;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A86 RID: 2694
	public class ImageElement : Element
	{
		// Token: 0x06008418 RID: 33816 RVA: 0x003D615C File Offset: 0x003D435C
		protected override void OnInit()
		{
			int refTemplateId = base.NodeData.ConfigItem.Inserts.First<int>();
			EncyclopediaReferenceItem refConfig = EncyclopediaReference.Instance[refTemplateId];
			Texture2D texture = ResLoader.SyncLoad<Texture2D>(refConfig.Param);
			this.image.texture = texture;
			this.image.SetNativeSize();
		}

		// Token: 0x04006529 RID: 25897
		[SerializeField]
		private CRawImage image;
	}
}
