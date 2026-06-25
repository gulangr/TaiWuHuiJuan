using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Debate
{
	// Token: 0x02000AA3 RID: 2723
	public class DebateUnitGradeArea : MonoBehaviour
	{
		// Token: 0x06008592 RID: 34194 RVA: 0x003E0678 File Offset: 0x003DE878
		public void Init()
		{
			bool flag = this._unitGradeItemArray != null;
			if (!flag)
			{
				RectTransform layout = this.itemLayout;
				this._unitGradeItemArray = new DebateUnitGradeItem[layout.childCount];
				float rotationUnitAngel = -360f / (float)layout.childCount;
				sbyte i = 0;
				while ((int)i < layout.childCount)
				{
					DebateUnitGradeItem refers = layout.GetChild((int)i).GetComponent<DebateUnitGradeItem>();
					this._unitGradeItemArray[(int)i] = refers;
					Vector3 angel = new Vector3(0f, 0f, (float)i * rotationUnitAngel);
					refers.Init(i, angel);
					i += 1;
				}
			}
		}

		// Token: 0x06008593 RID: 34195 RVA: 0x003E0710 File Offset: 0x003DE910
		public void Refresh(DebateBlock block, Action<int> onSelectGradeForStrategy = null, Action<int> onPreviewCost = null, Action<Vector2Int, sbyte> onCreate = null)
		{
			DebateUnitGradeArea.<>c__DisplayClass4_0 CS$<>8__locals1 = new DebateUnitGradeArea.<>c__DisplayClass4_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.onPreviewCost = onPreviewCost;
			CS$<>8__locals1.onSelectGradeForStrategy = onSelectGradeForStrategy;
			CS$<>8__locals1.onCreate = onCreate;
			CS$<>8__locals1.block = block;
			bool flag = !base.gameObject.activeSelf;
			if (flag)
			{
				base.gameObject.SetActive(true);
			}
			base.transform.position = CS$<>8__locals1.block.RectTrans.position;
			this.SetUnitGradeAreaCost(0);
			sbyte i = 0;
			while ((int)i < this._unitGradeItemArray.Length)
			{
				DebateUnitGradeArea.<>c__DisplayClass4_1 CS$<>8__locals2 = new DebateUnitGradeArea.<>c__DisplayClass4_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.itemGrade = i;
				DebateUnitGradeItem item = this._unitGradeItemArray[(int)i];
				item.Refresh(i, new Action<int, sbyte>(CS$<>8__locals2.CS$<>8__locals1.<Refresh>g__OnEnter|0), new Action(CS$<>8__locals2.CS$<>8__locals1.<Refresh>g__OnExit|1), new Action(CS$<>8__locals2.<Refresh>g__OnClick|2));
				i += 1;
			}
		}

		// Token: 0x06008594 RID: 34196 RVA: 0x003E07FF File Offset: 0x003DE9FF
		private void SetUnitGradeAreaCost(int cost)
		{
			this.textCost.text = CommonUtils.GetDisplayStringForNum(cost, 10000);
		}

		// Token: 0x04006670 RID: 26224
		[SerializeField]
		private RectTransform itemLayout;

		// Token: 0x04006671 RID: 26225
		[SerializeField]
		private TextMeshProUGUI textCost;

		// Token: 0x04006672 RID: 26226
		private DebateUnitGradeItem[] _unitGradeItemArray;
	}
}
