using System;
using Config;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Make
{
	// Token: 0x02000962 RID: 2402
	public class WeaveClothItem : MonoBehaviour
	{
		// Token: 0x06007358 RID: 29528 RVA: 0x0035982C File Offset: 0x00357A2C
		public void Set(AvatarData avatarData, ItemDisplayData itemDisplayData, int count, string name, short displayAge, short requiredAttainment, short currentAttainment)
		{
			this.avatar.Refresh(avatarData, displayAge);
			this.avatar.RefreshAsClothTree(this.avatar.Data, displayAge, this.facelessHeadSprites, this.facelessSkinColor.ColorToHexString("#"));
			this.countLabel.text = LanguageKey.LK_Making_Weave_Material_Count.TrFormat(count.ToString());
			this.nameLabel.text = name;
			sbyte grade = Clothing.Instance[itemDisplayData.Key.TemplateId].Grade;
			this.gradeLine.SetColor(Colors.Instance.GradeColors[(int)grade]);
			LifeSkillTypeItem lifeSkillConfig = LifeSkillType.Instance[10];
			string attainmentColor = (currentAttainment >= requiredAttainment) ? "brightblue" : "brightred";
			this.item.Set(lifeSkillConfig.Icon, lifeSkillConfig.Name, requiredAttainment.ToString().SetColor(attainmentColor), true);
			TooltipInvoker tip = base.GetComponent<TooltipInvoker>();
			tip.enabled = true;
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Set<ItemDisplayData>("ItemData", itemDisplayData);
		}

		// Token: 0x04005589 RID: 21897
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x0400558A RID: 21898
		[SerializeField]
		private TextMeshProUGUI countLabel;

		// Token: 0x0400558B RID: 21899
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x0400558C RID: 21900
		[SerializeField]
		private CImage gradeLine;

		// Token: 0x0400558D RID: 21901
		[SerializeField]
		private TooltipItemProperty item;

		// Token: 0x0400558E RID: 21902
		[SerializeField]
		private Sprite[] facelessHeadSprites = new Sprite[6];

		// Token: 0x0400558F RID: 21903
		[SerializeField]
		private Color facelessSkinColor;
	}
}
