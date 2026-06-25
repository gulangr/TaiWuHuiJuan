using System;
using Config;
using FrameWork.UISystem.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips
{
	// Token: 0x02000847 RID: 2119
	public class SkillRender : MonoBehaviour
	{
		// Token: 0x060066FC RID: 26364 RVA: 0x002EFCC0 File Offset: 0x002EDEC0
		public void Set(short[] skill)
		{
			this.Prepare(skill);
			bool flag = skill != null && skill.Length > 0;
			if (flag)
			{
				this.render.Rebuild<TMP_Text>(skill.Length, delegate(TMP_Text text, int i)
				{
					text.text = CombatSkill.Instance[skill[i]].Name.SetGradeColor((int)CombatSkill.Instance[skill[i]].Grade);
				});
			}
			else
			{
				this.render.Rebuild<TMP_Text>(1, delegate(TMP_Text text, int _)
				{
					text.text = LanguageKey.LK_None.Tr().SetGradeColor(0);
				});
			}
		}

		// Token: 0x060066FD RID: 26365 RVA: 0x002EFD50 File Offset: 0x002EDF50
		public void SetWithIcon(short[] skill)
		{
			this.Prepare(skill);
			bool flag = skill != null && skill.Length > 0;
			if (flag)
			{
				this.render.Rebuild<SkillWithIcon>(skill.Length, delegate(SkillWithIcon text, int i)
				{
					text.Set(skill[i]);
				});
			}
			else
			{
				this.render.Rebuild<SkillWithIcon>(1, delegate(SkillWithIcon text, int i)
				{
					text.Set(-1);
				});
			}
		}

		// Token: 0x060066FE RID: 26366 RVA: 0x002EFDE0 File Offset: 0x002EDFE0
		private void Prepare(short[] skill)
		{
			LocalStringManager.LanguageType curLanguageType = LocalStringManager.CurLanguageType;
			LocalStringManager.LanguageType languageType = curLanguageType;
			if (languageType != LocalStringManager.LanguageType.CN)
			{
				this.layout.cellSize = this.layout.cellSize.SetX(this.sizeLong);
				this.layout.constraintCount = 1;
				this.delim.gameObject.SetActive(false);
			}
			else
			{
				this.layout.constraintCount = 2;
				this.layout.cellSize = this.layout.cellSize.SetX(this.sizeShort);
				this.delim.gameObject.SetActive(skill != null && skill.Length > 0);
			}
		}

		// Token: 0x0400489D RID: 18589
		[SerializeField]
		private CImage delim;

		// Token: 0x0400489E RID: 18590
		[SerializeField]
		private GridLayoutGroup layout;

		// Token: 0x0400489F RID: 18591
		[SerializeField]
		private float sizeShort = 371f;

		// Token: 0x040048A0 RID: 18592
		[SerializeField]
		private float sizeLong = 744f;

		// Token: 0x040048A1 RID: 18593
		[SerializeField]
		private TemplatedContainerAssemblyNew render;
	}
}
