using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BA1 RID: 2977
	public class ViewCharacterMenuEatDetail : UIBase
	{
		// Token: 0x17000FEA RID: 4074
		// (get) Token: 0x0600939E RID: 37790 RVA: 0x0044C57C File Offset: 0x0044A77C
		public Attribute Attribute
		{
			get
			{
				return this.attributeAndInjury.Attribute;
			}
		}

		// Token: 0x17000FEB RID: 4075
		// (get) Token: 0x0600939F RID: 37791 RVA: 0x0044C589 File Offset: 0x0044A789
		public Injury Injury
		{
			get
			{
				return this.attributeAndInjury.Injury;
			}
		}

		// Token: 0x060093A0 RID: 37792 RVA: 0x0044C596 File Offset: 0x0044A796
		private void Awake()
		{
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
			this.Injury.SetAreaInteract(new Action(this.QuickHide));
		}

		// Token: 0x060093A1 RID: 37793 RVA: 0x0044C5CC File Offset: 0x0044A7CC
		public override void OnInit(ArgumentBox argsBox)
		{
			int charId;
			argsBox.Get("charId", out charId);
			this.attributeAndInjury.CharacterId = charId;
			this.eatDetailPanel.Setup(charId);
		}

		// Token: 0x060093A2 RID: 37794 RVA: 0x0044C602 File Offset: 0x0044A802
		private void OnEnable()
		{
			this.attributeAndInjury.SwitchToInjuryWithoutNotify();
			GEvent.Add(UiEvents.OnShowUsingMedicine, new GEvent.Callback(this.OnShowUsingMedicine));
			GEvent.OnEvent(UiEvents.OnShowEatDetail, null);
		}

		// Token: 0x060093A3 RID: 37795 RVA: 0x0044C63E File Offset: 0x0044A83E
		private void OnShowUsingMedicine(ArgumentBox argBox)
		{
			this.QuickHide();
		}

		// Token: 0x060093A4 RID: 37796 RVA: 0x0044C648 File Offset: 0x0044A848
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.OnShowUsingMedicine, new GEvent.Callback(this.OnShowUsingMedicine));
		}

		// Token: 0x040071AA RID: 29098
		[SerializeField]
		private AttributeAndInjuryDynamic attributeAndInjury;

		// Token: 0x040071AB RID: 29099
		[SerializeField]
		private EatDetailPanel eatDetailPanel;

		// Token: 0x040071AC RID: 29100
		[SerializeField]
		private CButton btnClose;

		// Token: 0x040071AD RID: 29101
		[SerializeField]
		private CButton btnReturn;
	}
}
