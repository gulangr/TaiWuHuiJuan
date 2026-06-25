using System;
using CharacterDataMonitor;
using Config;
using Game.Components.Avatar;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.World;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005C7 RID: 1479
	public class CharacterAvatar : CharacterUIElement
	{
		// Token: 0x170008D8 RID: 2264
		// (get) Token: 0x0600462B RID: 17963 RVA: 0x0020E96B File Offset: 0x0020CB6B
		private AvatarInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as AvatarInfoMonitor;
			}
		}

		// Token: 0x170008D9 RID: 2265
		// (get) Token: 0x0600462C RID: 17964 RVA: 0x0020E978 File Offset: 0x0020CB78
		public bool IsCharacterDead
		{
			get
			{
				return this.Item.Character.IsDead;
			}
		}

		// Token: 0x0600462D RID: 17965 RVA: 0x0020E98C File Offset: 0x0020CB8C
		public CharacterAvatar(Game.Components.Avatar.Avatar avatar, bool canShowGrave = true)
		{
			this._avatar = avatar;
			this.CanShowGrave = canShowGrave;
			bool flag = avatar != null;
			if (flag)
			{
				this._avatar.ResetToBlank(false);
			}
		}

		// Token: 0x0600462E RID: 17966 RVA: 0x0020E9C7 File Offset: 0x0020CBC7
		internal override void BindEvent()
		{
			this.Item.AddOnAvatarDataChangeEventListener(new Action(this.FillElement));
		}

		// Token: 0x0600462F RID: 17967 RVA: 0x0020E9E3 File Offset: 0x0020CBE3
		public override void UnbindEvent()
		{
			this.Item.RemoveOnAvatarDataChangeEventListener(new Action(this.FillElement));
		}

		// Token: 0x06004630 RID: 17968 RVA: 0x0020EA00 File Offset: 0x0020CC00
		public override void FillElement()
		{
			bool flag = null == this._avatar;
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				bool flag2 = !this._avatar.gameObject.activeSelf;
				if (!flag2)
				{
					CharacterItem config = Character.Instance[this.Item.TemplateId];
					bool flag3 = this.Item.Character.IsDead && this.CanShowGrave;
					if (flag3)
					{
						bool flag4 = this._avatar.Size == AvatarSize.Small;
						if (flag4)
						{
							ResLoader.LoadModOrGameResource<Texture2D>(CharacterAvatar.SmallSizeGraveResPath, delegate(Texture2D tex)
							{
								this._avatar.Refresh(tex);
								bool flag19 = this.OnFillAvatar != null;
								if (flag19)
								{
									this.OnFillAvatar();
								}
							}, null);
						}
						else
						{
							ResLoader.LoadModOrGameResource<Texture2D>(CharacterAvatar.GraveResPath, delegate(Texture2D tex)
							{
								this._avatar.Refresh(tex);
								bool flag19 = this.OnFillAvatar != null;
								if (flag19)
								{
									this.OnFillAvatar();
								}
							}, null);
						}
					}
					else
					{
						bool flag5 = !string.IsNullOrEmpty(config.FixedAvatarName) && config.RandomEnemyId < 0;
						if (flag5)
						{
							string sizeFolder = CharacterAvatar.GetAvatarSizeFolder(this._avatar.Size);
							string avatarAssetName = config.FixedAvatarName;
							bool isSpecialUseStatic = false;
							bool flag6 = SingletonObject.getInstance<TutorialChapterModel>().InGuiding && this.Item.TemplateId == 908;
							if (flag6)
							{
								bool huanxinSurprised = SingletonObject.getInstance<TutorialChapterModel>().HuanxinSurprised;
								if (huanxinSurprised)
								{
									avatarAssetName = "NpcFace_jingyahuanxin";
								}
								bool huanxinDying = SingletonObject.getInstance<TutorialChapterModel>().HuanxinDying;
								if (huanxinDying)
								{
									avatarAssetName = "NpcFace_binsihuanxin";
								}
							}
							else
							{
								bool flag7 = this.Item.XiangshuType == 3;
								if (flag7)
								{
									sbyte xiangshuAvatarId = XiangshuAvatarIds.GetXiangshuAvatarIdByCharacterTemplateId(this.Item.TemplateId);
									avatarAssetName = XiangshuAvatarIds.GetJuniorXiangshuAvatarName(xiangshuAvatarId);
								}
								else
								{
									bool flag8 = this.Item.XiangshuType == 4;
									if (flag8)
									{
										avatarAssetName = XiangshuAvatarIds.GetPuppetAvatarName(this.Item.TemplateId);
										isSpecialUseStatic = true;
									}
								}
							}
							string spineName = null;
							string skinName = null;
							bool flag9 = avatarAssetName == "NpcFace_huanxin";
							if (flag9)
							{
								spineName = config.FixedAvatarSpineName;
								skinName = config.FixedAvatarSpineSkin;
							}
							else
							{
								bool flag10 = avatarAssetName == "NpcFace_jingyahuanxin";
								if (flag10)
								{
									spineName = "NpcFace/huanxin_jingya";
								}
								else
								{
									bool flag11 = avatarAssetName == "NpcFace_binsihuanxin";
									if (flag11)
									{
										spineName = "NpcFace/huanxin_binsi";
									}
									else
									{
										bool flag12 = !string.IsNullOrEmpty(config.FixedAvatarSpineName);
										if (flag12)
										{
											spineName = config.FixedAvatarSpineName;
											skinName = config.FixedAvatarSpineSkin;
										}
									}
								}
							}
							bool flag13 = !string.IsNullOrEmpty(spineName) && this._avatar.Size != AvatarSize.Small && this._avatar.PreferDynamicAvatar && !isSpecialUseStatic;
							if (flag13)
							{
								this._avatar.RefreshAsSpine(spineName, skinName);
							}
							else
							{
								string resPath = CharacterAvatar.GetNpcFaceResPath(sizeFolder, avatarAssetName);
								ResLoader.LoadModOrGameResource<Texture2D>(resPath, delegate(Texture2D tex)
								{
									this._avatar.Refresh(tex);
									bool flag19 = this.OnFillAvatar != null;
									if (flag19)
									{
										this.OnFillAvatar();
									}
								}, delegate(string path)
								{
									this._avatar.Refresh(this.Item.AvatarData, this.Item.AvatarAge);
								});
							}
						}
						else
						{
							EventModel eventModel = SingletonObject.getInstance<EventModel>();
							AvatarData copiedData = this.Item.AvatarData;
							bool flag14 = eventModel.NeedShowAsMarriageLook1(base.CharacterId);
							if (flag14)
							{
								copiedData = new AvatarData();
								copiedData.Copy(this.Item.AvatarData);
								copiedData.ChangeToMarriageStyle1();
							}
							bool flag15 = eventModel.NeedShowAsMarriageLook2(base.CharacterId);
							if (flag15)
							{
								copiedData = new AvatarData();
								copiedData.Copy(this.Item.AvatarData);
								copiedData.ChangeToMarriageStyle2();
							}
							bool flag16 = eventModel.NeedShowShixiangBarbarianMasterCloth(base.CharacterId);
							if (flag16)
							{
								copiedData = new AvatarData();
								copiedData.Copy(this.Item.AvatarData);
								copiedData.ChangeToShixiangBarbarianMaster();
							}
							short taiwuClothingDisplayId;
							bool flag17 = eventModel.TryGetTaiwuClothingDisplayId(base.CharacterId, out taiwuClothingDisplayId);
							if (flag17)
							{
								copiedData = new AvatarData();
								copiedData.Copy(this.Item.AvatarData);
								copiedData.ClothDisplayId = taiwuClothingDisplayId;
							}
							copiedData.ShowBlush = eventModel.NeedShowBlush(base.CharacterId);
							copiedData.ShowJieqingMask = eventModel.NeedShowJieqingMask(base.CharacterId);
							eventModel.CheckAvatarClothDisplayIdForEvent(base.CharacterId, copiedData, null);
							this._avatar.Refresh(copiedData, this.Item.AvatarAge);
							bool flag18 = this.OnFillAvatar != null;
							if (flag18)
							{
								SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, this.OnFillAvatar);
							}
						}
					}
				}
			}
		}

		// Token: 0x06004631 RID: 17969 RVA: 0x0020EE34 File Offset: 0x0020D034
		public override void ResetToEmpty()
		{
			bool flag = null != this._avatar;
			if (flag)
			{
				this._avatar.ResetToBlank(false);
			}
		}

		// Token: 0x06004632 RID: 17970 RVA: 0x0020EE60 File Offset: 0x0020D060
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AvatarInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x06004633 RID: 17971 RVA: 0x0020EE84 File Offset: 0x0020D084
		public static string GetAvatarSizeFolder(AvatarSize avatarSize)
		{
			bool flag = avatarSize == AvatarSize.Normal;
			string result;
			if (flag)
			{
				result = "NormalFace";
			}
			else
			{
				bool flag2 = avatarSize == AvatarSize.Small;
				if (flag2)
				{
					result = "SmallFace";
				}
				else
				{
					result = "BigFace";
				}
			}
			return result;
		}

		// Token: 0x06004634 RID: 17972 RVA: 0x0020EEBC File Offset: 0x0020D0BC
		public static string GetNpcFaceResPath(string sizeFolder, string avatarAssetName)
		{
			return "NpcFace/" + sizeFolder + "/" + avatarAssetName;
		}

		// Token: 0x040030AD RID: 12461
		public Action OnFillAvatar;

		// Token: 0x040030AE RID: 12462
		public bool CanShowGrave;

		// Token: 0x040030AF RID: 12463
		public static readonly string GraveResPath = "NpcFace/NormalFace/NPCFace_tomb";

		// Token: 0x040030B0 RID: 12464
		public static readonly string SmallSizeGraveResPath = "NpcFace/SmallFace/NPCFace_tomb";

		// Token: 0x040030B1 RID: 12465
		private readonly Game.Components.Avatar.Avatar _avatar;
	}
}
