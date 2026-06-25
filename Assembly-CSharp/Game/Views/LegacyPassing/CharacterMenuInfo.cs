using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.LegacyPassing
{
	// Token: 0x0200098F RID: 2447
	public class CharacterMenuInfo : MonoBehaviour
	{
		// Token: 0x060075FB RID: 30203 RVA: 0x00370657 File Offset: 0x0036E857
		public void Set(CharacterMenuInfoDisplayData characterData, CombatSkillShorts combatSkillQualifications, CombatSkillShorts combatSkillAttainments, LifeSkillShorts lifeSkillQualifications, LifeSkillShorts lifeSkillAttainments)
		{
			this._characterData = characterData;
			this._combatSkillQualifications = combatSkillQualifications;
			this._combatSkillAttainments = combatSkillAttainments;
			this._lifeSkillQualifications = lifeSkillQualifications;
			this._lifeSkillAttainments = lifeSkillAttainments;
			this.RefreshAll();
		}

		// Token: 0x060075FC RID: 30204 RVA: 0x00370686 File Offset: 0x0036E886
		private void Awake()
		{
			this.btn.onClick.ResetListener(delegate()
			{
				this.ctg.Set(this.index, false);
			});
		}

		// Token: 0x060075FD RID: 30205 RVA: 0x003706A6 File Offset: 0x0036E8A6
		private void RefreshAll()
		{
			this.RefreshPersonalities();
			this.RefreshAvatar();
			this.RefreshFeatureScroll();
			this.RefreshQualifications();
			this.RefreshDetailInfo();
		}

		// Token: 0x060075FE RID: 30206 RVA: 0x003706CC File Offset: 0x0036E8CC
		private void RefreshDetailInfo()
		{
			this.detailInfo.Set(this._characterData);
		}

		// Token: 0x060075FF RID: 30207 RVA: 0x003706E4 File Offset: 0x0036E8E4
		private void RefreshFeatureScroll()
		{
			this.featureScroll.cellSize = ((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? this.cnCellSize : this.enCellSize);
			int i = this.featureScroll.transform.childCount;
			while (i-- > 0)
			{
				Object.Destroy(this.featureScroll.transform.GetChild(i).gameObject);
			}
			CharacterDisplayData characterDisplayData = this._characterData.CharacterDisplayData;
			foreach (short feature in CharacterMenuInfo.ProcessFeatures((characterDisplayData != null) ? characterDisplayData.FeatureIds : null))
			{
				Feature featureItem = Object.Instantiate<Feature>((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? this.smallTemplate : this.largeTemplate, this.featureScroll.transform, false);
				Feature feature2 = featureItem;
				short featureId = feature;
				CharacterDisplayData characterDisplayData2 = this._characterData.CharacterDisplayData;
				int characterId = (characterDisplayData2 != null) ? characterDisplayData2.CharacterId : -1;
				bool isTaiwu = false;
				Dictionary<short, int> temporaryFeatureLeftTimes = this._characterData.TemporaryFeatureLeftTimes;
				int left;
				feature2.Set(featureId, characterId, isTaiwu, (temporaryFeatureLeftTimes != null && temporaryFeatureLeftTimes.TryGetValue(feature, out left)) ? left : -1);
			}
		}

		// Token: 0x06007600 RID: 30208 RVA: 0x0037080C File Offset: 0x0036EA0C
		private unsafe void RefreshQualifications()
		{
			for (int i = 0; i < 14; i++)
			{
				this.combat[i].Set(CombatSkillType.Instance[i].Name, (int)(*this._combatSkillQualifications[i]), (int)(*this._combatSkillAttainments[i]));
			}
			for (int j = 0; j < 16; j++)
			{
				this.life[j].Set(Config.LifeSkillType.Instance[j].Name, (int)(*this._lifeSkillQualifications[j]), (int)(*this._lifeSkillAttainments[j]));
			}
		}

		// Token: 0x06007601 RID: 30209 RVA: 0x003708AC File Offset: 0x0036EAAC
		public static IEnumerable<short> ProcessFeatures(IEnumerable<short> featureIdIter)
		{
			IEnumerable<short> enumerable;
			if (featureIdIter == null)
			{
				enumerable = null;
			}
			else
			{
				enumerable = featureIdIter.Where(delegate(short featureId)
				{
					CharacterFeatureItem characterFeatureItem = CharacterFeature.Instance[featureId];
					return characterFeatureItem != null && !characterFeatureItem.Hidden;
				});
			}
			return enumerable ?? Enumerable.Empty<short>();
		}

		// Token: 0x06007602 RID: 30210 RVA: 0x003708E2 File Offset: 0x0036EAE2
		private void RefreshAvatar()
		{
			this.characterCircle.Set(this._characterData.CharacterDisplayData, false, false);
		}

		// Token: 0x06007603 RID: 30211 RVA: 0x003708FE File Offset: 0x0036EAFE
		private void RefreshPersonalities()
		{
			this.personalities.Set(this._characterData.CharacterDisplayData);
		}

		// Token: 0x040058C7 RID: 22727
		private CharacterMenuInfoDisplayData _characterData;

		// Token: 0x040058C8 RID: 22728
		private CombatSkillShorts _combatSkillQualifications;

		// Token: 0x040058C9 RID: 22729
		private CombatSkillShorts _combatSkillAttainments;

		// Token: 0x040058CA RID: 22730
		private LifeSkillShorts _lifeSkillQualifications;

		// Token: 0x040058CB RID: 22731
		private LifeSkillShorts _lifeSkillAttainments;

		// Token: 0x040058CC RID: 22732
		private const int PersonalitiesOffsetIsTaiwu = -40;

		// Token: 0x040058CD RID: 22733
		private const int PersonalitiesOffsetNotTaiwu = 51;

		// Token: 0x040058CE RID: 22734
		[SerializeField]
		private int index;

		// Token: 0x040058CF RID: 22735
		[SerializeField]
		private CToggleGroup ctg;

		// Token: 0x040058D0 RID: 22736
		[SerializeField]
		private CButton btn;

		// Token: 0x040058D1 RID: 22737
		[SerializeField]
		private Game.Components.Character.Personalities personalities;

		// Token: 0x040058D2 RID: 22738
		[SerializeField]
		private CharacterCircle characterCircle;

		// Token: 0x040058D3 RID: 22739
		[SerializeField]
		private GridLayoutGroup featureScroll;

		// Token: 0x040058D4 RID: 22740
		[SerializeField]
		private Feature smallTemplate;

		// Token: 0x040058D5 RID: 22741
		[SerializeField]
		private Feature largeTemplate;

		// Token: 0x040058D6 RID: 22742
		[SerializeField]
		private CharacterMenuInfoDetailInfo detailInfo;

		// Token: 0x040058D7 RID: 22743
		[SerializeField]
		private Vector2 cnCellSize = new Vector2(234f, 108f);

		// Token: 0x040058D8 RID: 22744
		[SerializeField]
		private Vector2 enCellSize = new Vector2(480f, 111f);

		// Token: 0x040058D9 RID: 22745
		[SerializeField]
		private QualificationAndAttainment[] combat;

		// Token: 0x040058DA RID: 22746
		[SerializeField]
		private QualificationAndAttainment[] life;
	}
}
