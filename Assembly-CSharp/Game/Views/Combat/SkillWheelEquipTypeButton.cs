using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Combat
{
	// Token: 0x02000B1D RID: 2845
	public class SkillWheelEquipTypeButton : MonoBehaviour
	{
		// Token: 0x17000F6C RID: 3948
		// (get) Token: 0x06008BA8 RID: 35752 RVA: 0x0040822E File Offset: 0x0040642E
		// (set) Token: 0x06008BA9 RID: 35753 RVA: 0x00408236 File Offset: 0x00406436
		public sbyte EquipType { get; private set; }

		// Token: 0x17000F6D RID: 3949
		// (get) Token: 0x06008BAA RID: 35754 RVA: 0x0040823F File Offset: 0x0040643F
		public Sprite TypeIcon
		{
			get
			{
				CImage cimage = this.icon;
				return (cimage != null) ? cimage.sprite : null;
			}
		}

		// Token: 0x17000F6E RID: 3950
		// (get) Token: 0x06008BAB RID: 35755 RVA: 0x00408253 File Offset: 0x00406453
		public Color32 TypeTextColor
		{
			get
			{
				TextMeshProUGUI textMeshProUGUI = this.text;
				return (textMeshProUGUI != null) ? textMeshProUGUI.color : Color.white;
			}
		}

		// Token: 0x14000084 RID: 132
		// (add) Token: 0x06008BAC RID: 35756 RVA: 0x00408270 File Offset: 0x00406470
		// (remove) Token: 0x06008BAD RID: 35757 RVA: 0x004082A8 File Offset: 0x004064A8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<SkillWheelEquipTypeButton> OnClicked;

		// Token: 0x06008BAE RID: 35758 RVA: 0x004082E0 File Offset: 0x004064E0
		private void Awake()
		{
			bool flag = this.button != null;
			if (flag)
			{
				this.button.onClick.AddListener(new UnityAction(this.OnClick));
			}
		}

		// Token: 0x06008BAF RID: 35759 RVA: 0x00408320 File Offset: 0x00406520
		public void Initialize(sbyte equipType, string typeName)
		{
			this.EquipType = equipType;
			bool flag = this.text != null;
			if (flag)
			{
				this.text.text = typeName;
			}
		}

		// Token: 0x06008BB0 RID: 35760 RVA: 0x00408355 File Offset: 0x00406555
		public void SetSelected(bool selected)
		{
			this.selectedMark.SetActive(selected);
		}

		// Token: 0x06008BB1 RID: 35761 RVA: 0x00408365 File Offset: 0x00406565
		private void OnClick()
		{
			Action<SkillWheelEquipTypeButton> onClicked = this.OnClicked;
			if (onClicked != null)
			{
				onClicked(this);
			}
		}

		// Token: 0x04006AEC RID: 27372
		[Header("UI组件")]
		[SerializeField]
		private Button button;

		// Token: 0x04006AED RID: 27373
		[SerializeField]
		private CImage icon;

		// Token: 0x04006AEE RID: 27374
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04006AEF RID: 27375
		[SerializeField]
		private GameObject selectedMark;
	}
}
