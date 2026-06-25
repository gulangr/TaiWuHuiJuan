using System;
using System.IO;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Building;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200087D RID: 2173
	public class TooltipChicken : MouseTipBase
	{
		// Token: 0x06006868 RID: 26728 RVA: 0x002FBD20 File Offset: 0x002F9F20
		public override bool CanShowWithArgumentBox(ArgumentBox argumentBox)
		{
			int num;
			return argumentBox != null && argumentBox.Get("ChickenId", out num);
		}

		// Token: 0x06006869 RID: 26729 RVA: 0x002FBD48 File Offset: 0x002F9F48
		protected override void Init(ArgumentBox argsBox)
		{
			int chickenId;
			bool flag = !argsBox.Get("ChickenId", out chickenId);
			if (!flag)
			{
				BuildingDomainMethod.AsyncCall.GetChickenData(this, chickenId, delegate(int offset2, RawDataPool dataPool2)
				{
					GameData.Domains.Building.Chicken chicken = default(GameData.Domains.Building.Chicken);
					Serializer.Deserialize(dataPool2, offset2, ref chicken);
					this.Set(chicken);
				});
			}
		}

		// Token: 0x0600686A RID: 26730 RVA: 0x002FBD84 File Offset: 0x002F9F84
		private void Set(GameData.Domains.Building.Chicken chicken)
		{
			ChickenItem config = Config.Chicken.Instance[chicken.TemplateId];
			this.chickenNameLabel.text = config.Name;
			this.chickenGradeLabel.text = LocalStringManager.Get(string.Format("LK_Grade_1_{0}", config.Grade)).SetGradeColor((int)config.Grade);
			this.chickenHappinessLabel.text = chicken.Happiness.ToString();
			ResLoader.Load<Sprite>(Path.Combine("RemakeResources/Textures/Chicken", config.Display), delegate(Sprite sprite)
			{
				this.chickenIcon.sprite = sprite;
				this.chickenIcon.enabled = true;
			}, null, false);
			this.chickenDescLabel.text = config.Desc;
			this.personalityTypeProperty.Set("ui9_icon_building_personality_big_" + config.PersonalityType.ToString(), LanguageKey.LK_Personality.Tr(), TooltipChicken.PersonalityTypeName[(int)config.PersonalityType].Tr(), true);
			this.personalityValueProperty.SetValue(config.PersonalityValue.ToString());
			this.featherNoticeHolder.gameObject.SetActive(chicken.CanPluckFeather);
			this.featherLocationNoticeHolder.gameObject.SetActive(!SingletonObject.getInstance<WorldMapModel>().IsTaiwuOnSettlement);
		}

		// Token: 0x0600686B RID: 26731 RVA: 0x002FBEC0 File Offset: 0x002FA0C0
		private void Update()
		{
			bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.Chicken);
			}
		}

		// Token: 0x0600686D RID: 26733 RVA: 0x002FBEFB File Offset: 0x002FA0FB
		// Note: this type is marked as 'beforefieldinit'.
		static TooltipChicken()
		{
			LanguageKey[] array = new LanguageKey[7];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.9B5C548DDB9AF43939ADD251D0B074A4BF7581010D6D1416C646C79EE7C2A814).FieldHandle);
			TooltipChicken.PersonalityTypeName = array;
		}

		// Token: 0x04004A1C RID: 18972
		[SerializeField]
		private TextMeshProUGUI chickenNameLabel;

		// Token: 0x04004A1D RID: 18973
		[SerializeField]
		private TextMeshProUGUI chickenGradeLabel;

		// Token: 0x04004A1E RID: 18974
		[SerializeField]
		private TextMeshProUGUI chickenHappinessLabel;

		// Token: 0x04004A1F RID: 18975
		[SerializeField]
		private CImage chickenIcon;

		// Token: 0x04004A20 RID: 18976
		[SerializeField]
		private TextMeshProUGUI chickenDescLabel;

		// Token: 0x04004A21 RID: 18977
		[SerializeField]
		private TooltipItemProperty personalityTypeProperty;

		// Token: 0x04004A22 RID: 18978
		[SerializeField]
		private TooltipItemProperty personalityValueProperty;

		// Token: 0x04004A23 RID: 18979
		[SerializeField]
		private RectTransform featherNoticeHolder;

		// Token: 0x04004A24 RID: 18980
		[SerializeField]
		private RectTransform featherLocationNoticeHolder;

		// Token: 0x04004A25 RID: 18981
		private static readonly LanguageKey[] PersonalityTypeName;
	}
}
