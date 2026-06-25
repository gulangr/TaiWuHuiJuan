using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009D2 RID: 2514
	public class ZhujianGearMateSubPageFiveElements : ZhujianGearMateSubPage
	{
		// Token: 0x06007A6E RID: 31342 RVA: 0x0038DDC2 File Offset: 0x0038BFC2
		public override void Init(ViewZhujianGearMate parent)
		{
			base.Init(parent);
			this.InitLanguageText();
			this.InitInteractive();
		}

		// Token: 0x06007A6F RID: 31343 RVA: 0x0038DDDB File Offset: 0x0038BFDB
		protected override void OnShowDataRequest()
		{
			this.RequestGearMateData();
			base.SetContentReady();
		}

		// Token: 0x06007A70 RID: 31344 RVA: 0x0038DDEC File Offset: 0x0038BFEC
		public override void SetGearMateId(int gearMateId)
		{
			bool flag = this.GearMateId == gearMateId;
			if (!flag)
			{
				base.SetGearMateId(gearMateId);
				bool isVisible = this.IsVisible;
				if (isVisible)
				{
					this.RequestGearMateData();
				}
			}
		}

		// Token: 0x06007A71 RID: 31345 RVA: 0x0038DE24 File Offset: 0x0038C024
		private void RequestGearMateData()
		{
			ExtraDomainMethod.AsyncCall.GetGearMateById(null, this.GearMateId, delegate(int offset, RawDataPool pool)
			{
				GearMate gearMate = new GearMate();
				Serializer.Deserialize(pool, offset, ref gearMate);
				this.Refresh((sbyte)gearMate.NeiliType);
			});
		}

		// Token: 0x06007A72 RID: 31346 RVA: 0x0038DE40 File Offset: 0x0038C040
		private void Refresh(sbyte neiliType)
		{
			this._neiliType = neiliType;
			for (int i = 0; i < this.interactive.childCount; i++)
			{
				this.interactive.GetChild(i).GetChild(0).gameObject.SetActive(i == (int)this._neiliType);
			}
			NeiliTypeItem config = NeiliType.Instance[this._neiliType];
			sbyte pureType = (sbyte)config.FiveElements;
			this.icon.SetSprite(string.Format("{0}{1}", "ui9_icon_five_elements_", pureType), false, null);
			this.nameLabel.text = config.Name.SetColor(CommonUtils.GetFiveElementsColor(pureType));
		}

		// Token: 0x06007A73 RID: 31347 RVA: 0x0038DEF0 File Offset: 0x0038C0F0
		private void InitLanguageText()
		{
			string targetName = this._languageObjName.GetValueOrDefault(LocalStringManager.CurLanguageType, this._languageObjName[LocalStringManager.LanguageType.EN]);
			for (int i = 0; i < this.texts.childCount; i++)
			{
				GameObject obj = this.texts.GetChild(i).gameObject;
				obj.SetActive(targetName == obj.name);
			}
		}

		// Token: 0x06007A74 RID: 31348 RVA: 0x0038DF5C File Offset: 0x0038C15C
		private void InitInteractive()
		{
			sbyte i = 0;
			while ((int)i < this.interactive.childCount)
			{
				Transform obj = this.interactive.GetChild((int)i);
				PointerTrigger pointerTrigger = obj.GetComponent<PointerTrigger>();
				sbyte type = i;
				pointerTrigger.EnterEvent.RemoveAllListeners();
				pointerTrigger.EnterEvent.AddListener(delegate()
				{
					obj.GetChild(1).gameObject.SetActive(true);
				});
				pointerTrigger.ExitEvent.RemoveAllListeners();
				pointerTrigger.ExitEvent.AddListener(delegate()
				{
					obj.GetChild(1).gameObject.SetActive(false);
				});
				obj.GetComponent<CButton>().ClearAndAddListener(delegate
				{
					this.OnSelect(type);
				});
				TooltipInvoker component = obj.GetComponent<TooltipInvoker>();
				if (component.RuntimeParam == null)
				{
					component.RuntimeParam = new ArgumentBox().Set("neiliType", (int)i);
				}
				i += 1;
			}
		}

		// Token: 0x06007A75 RID: 31349 RVA: 0x0038E054 File Offset: 0x0038C254
		private void OnSelect(sbyte type)
		{
			ExtraDomainMethod.Call.UpgradeGearMate(this.GearMateId, 12, ItemKey.Invalid, (int)type);
			this.Refresh(type);
		}

		// Token: 0x04005CCB RID: 23755
		[SerializeField]
		protected Transform interactive;

		// Token: 0x04005CCC RID: 23756
		[SerializeField]
		protected Transform texts;

		// Token: 0x04005CCD RID: 23757
		[SerializeField]
		protected TextMeshProUGUI nameLabel;

		// Token: 0x04005CCE RID: 23758
		[SerializeField]
		protected CImage icon;

		// Token: 0x04005CCF RID: 23759
		private sbyte _neiliType;

		// Token: 0x04005CD0 RID: 23760
		private readonly Dictionary<LocalStringManager.LanguageType, string> _languageObjName = new Dictionary<LocalStringManager.LanguageType, string>
		{
			{
				LocalStringManager.LanguageType.CN,
				"CN"
			},
			{
				LocalStringManager.LanguageType.EN,
				"EN"
			}
		};
	}
}
