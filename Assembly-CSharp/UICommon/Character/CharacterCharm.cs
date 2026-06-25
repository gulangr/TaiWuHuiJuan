using System;
using CharacterDataMonitor;
using Config;
using GameData.Domains.Character.Creation;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005C9 RID: 1481
	public class CharacterCharm : CharacterUIElement
	{
		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x06004641 RID: 17985 RVA: 0x0020F263 File Offset: 0x0020D463
		private DetailInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as DetailInfoMonitor;
			}
		}

		// Token: 0x06004642 RID: 17986 RVA: 0x0020F270 File Offset: 0x0020D470
		public CharacterCharm(Refers refers)
		{
			bool flag = refers == null;
			if (flag)
			{
				throw new Exception("refers can not be null to create CharacterCharm element!");
			}
			this._infoItem = new InfoItem(refers);
			CharacterPropertyDisplayItem config = CharacterPropertyDisplay.Instance[101];
			this._infoItem.SetInfoName(config.Name);
			TooltipInvoker mouseTip = this._infoItem.GetMouseTip();
			bool flag2 = null != mouseTip;
			if (flag2)
			{
				mouseTip.enabled = true;
				mouseTip.IsLanguageKey = false;
				mouseTip.Type = TipType.Simple;
				mouseTip.PresetParam = new string[]
				{
					config.Name,
					config.Desc
				};
			}
		}

		// Token: 0x06004643 RID: 17987 RVA: 0x0020F313 File Offset: 0x0020D513
		public CharacterCharm(InfoItem item)
		{
			this._infoItem = item;
		}

		// Token: 0x06004644 RID: 17988 RVA: 0x0020F324 File Offset: 0x0020D524
		internal override void BindEvent()
		{
			this.Item.AddAttractionListener(new Action(this.FillElement));
			AvatarInfoMonitor avatarMonitor = this._avatarMonitor;
			if (avatarMonitor != null)
			{
				avatarMonitor.AddOnAvatarDataChangeEventListener(new Action(this.FillElement));
			}
			BasicInfoMonitor basicMonitor = this._basicMonitor;
			if (basicMonitor != null)
			{
				basicMonitor.AddGenderListener(new Action(this.FillElement));
			}
		}

		// Token: 0x06004645 RID: 17989 RVA: 0x0020F38C File Offset: 0x0020D58C
		public override void UnbindEvent()
		{
			BasicInfoMonitor basicMonitor = this._basicMonitor;
			if (basicMonitor != null)
			{
				basicMonitor.RemoveGenderListener(new Action(this.FillElement));
			}
			AvatarInfoMonitor avatarMonitor = this._avatarMonitor;
			if (avatarMonitor != null)
			{
				avatarMonitor.RemoveOnAvatarDataChangeEventListener(new Action(this.FillElement));
			}
			this.Item.RemoveAttractionListener(new Action(this.FillElement));
		}

		// Token: 0x06004646 RID: 17990 RVA: 0x0020F3F4 File Offset: 0x0020D5F4
		public override void FillElement()
		{
			bool flag = !this._infoItem.HasValidElement();
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				bool flag2 = this.Item == null || !this.Item.Init || !this._avatarMonitor.Init || !this._basicMonitor.Init;
				if (!flag2)
				{
					bool isFixedPresetType = CreatingType.IsFixedPresetType(this._avatarMonitor.CreatingType);
					this._infoItem.SetInfoValue(CommonUtils.GetCharmLevelText(this.Item.Attraction, this._basicMonitor.Gender, this._avatarMonitor.AvatarAge, this._avatarMonitor.AvatarData.ClothDisplayId, isFixedPresetType, this._avatarMonitor.AvatarData.FaceVisible));
					this._infoItem.SetIcon(CommonUtils.GetCharmLevelIconLegacy(this.Item.Attraction, this._avatarMonitor.AvatarAge, this._avatarMonitor.AvatarData.ClothDisplayId, this._avatarMonitor.AvatarData.FaceVisible));
				}
			}
		}

		// Token: 0x06004647 RID: 17991 RVA: 0x0020F50C File Offset: 0x0020D70C
		public override void ResetToEmpty()
		{
			bool flag = !this._infoItem.HasValidElement();
			if (flag)
			{
				bool flag2 = this.MonitorDataItem != null;
				if (flag2)
				{
					this.UnbindEvent();
					this.MonitorDataItem = null;
				}
			}
			else
			{
				this._infoItem.SetInfoValue(string.Empty);
			}
		}

		// Token: 0x06004648 RID: 17992 RVA: 0x0020F560 File Offset: 0x0020D760
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			this._basicMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(charId, this.IsDead);
			this._avatarMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AvatarInfoMonitor>(charId, this.IsDead);
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x040030B6 RID: 12470
		private BasicInfoMonitor _basicMonitor;

		// Token: 0x040030B7 RID: 12471
		private AvatarInfoMonitor _avatarMonitor;

		// Token: 0x040030B8 RID: 12472
		private readonly InfoItem _infoItem;
	}
}
