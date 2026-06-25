using System;
using System.IO;
using Config;
using GameData.Domains.Building;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.Obtain
{
	// Token: 0x020007D4 RID: 2004
	public class GetChicken : MonoBehaviour
	{
		// Token: 0x060061D3 RID: 25043 RVA: 0x002CE240 File Offset: 0x002CC440
		public void Set(GameData.Domains.Building.Chicken chicken)
		{
			ChickenItem config = Config.Chicken.Instance[chicken.TemplateId];
			this.chickenName.text = config.Name;
			this.progress.SetText(Mathf.FloorToInt((float)chicken.Happiness / 100f * 100f).ToString(), true);
			this.grade.SetColor(Colors.Instance.GradeColors[(int)config.Grade]);
			ResLoader.Load<Texture2D>(Path.Combine("RemakeResources/Textures/Chicken", config.Display), delegate(Texture2D texture)
			{
				this.icon.texture = texture;
			}, null, false);
			bool flag = this.skeleton;
			if (flag)
			{
				this.skeleton.Skeleton.SetAttachment("side_body", string.Format("chicken_{0}_side", config.ChickenColor.ToInt()));
			}
		}

		// Token: 0x040043E9 RID: 17385
		[SerializeField]
		private TextMeshProUGUI chickenName;

		// Token: 0x040043EA RID: 17386
		[SerializeField]
		private CRawImage icon;

		// Token: 0x040043EB RID: 17387
		[SerializeField]
		private TextMeshProUGUI progress;

		// Token: 0x040043EC RID: 17388
		[SerializeField]
		private CImage grade;

		// Token: 0x040043ED RID: 17389
		[SerializeField]
		private SkeletonGraphic skeleton;

		// Token: 0x040043EE RID: 17390
		public const string ChickenPath = "RemakeResources/Textures/Chicken";
	}
}
