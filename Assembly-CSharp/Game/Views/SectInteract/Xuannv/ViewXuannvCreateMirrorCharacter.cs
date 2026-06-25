using System;
using System.Collections.Generic;
using System.Text;
using Coffee.UIExtensions;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Story;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Xuannv
{
	// Token: 0x020009D7 RID: 2519
	public class ViewXuannvCreateMirrorCharacter : UIBase
	{
		// Token: 0x17000D99 RID: 3481
		// (get) Token: 0x06007B2B RID: 31531 RVA: 0x00393A8E File Offset: 0x00391C8E
		private int TaiwuId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000D9A RID: 3482
		// (get) Token: 0x06007B2C RID: 31532 RVA: 0x00393A9A File Offset: 0x00391C9A
		private static int Cost
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x06007B2D RID: 31533 RVA: 0x00393A9E File Offset: 0x00391C9E
		public override void OnInit(ArgumentBox argsBox)
		{
			if (this._nameCharacter == null)
			{
				this._nameCharacter = this.nameCharacterCN;
			}
		}

		// Token: 0x06007B2E RID: 31534 RVA: 0x00393AB8 File Offset: 0x00391CB8
		private void Awake()
		{
			this.genderToggleGroup.Init(-1);
			this.genderToggleGroup.OnActiveIndexChange += this.OnGenderChange;
			this.nameCharacterCN.Init(new Action<string, string>(this.OnRefreshNameCharacter), new Action(this.OnClickRandomName));
			this.nameCharacterEN.Init(new Action<string, string>(this.OnRefreshNameCharacter), new Action(this.OnClickRandomName));
			this.neiliAllocationTypes.Init();
			this.btnConfirm.ClearAndAddListener(new Action(this.OnClickConfirm));
			this.btnOpenCharMenu.ClearAndAddListener(new Action(this.OnClickBtnOpenCharMenu));
		}

		// Token: 0x06007B2F RID: 31535 RVA: 0x00393B6F File Offset: 0x00391D6F
		private void OnEnable()
		{
			this.OnLanguageChange(LocalStringManager.CurLanguageType);
			this.RequestData();
		}

		// Token: 0x06007B30 RID: 31536 RVA: 0x00393B88 File Offset: 0x00391D88
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "ButtonCloseView" == btnName;
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007B31 RID: 31537 RVA: 0x00393BB5 File Offset: 0x00391DB5
		private void RequestData()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, this.TaiwuId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._data);
				this.RefreshAvatar();
			});
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForNeiliPage(null, this.TaiwuId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._neiliData);
				this.RefreshNeili();
				this.RefreshNeiliAllocation(this._neiliData.BaseNeiliAllocation, this._neiliData.CombatNeiliAllocation, this._neiliData.NeiliAllocation);
				this.RefreshBtnConfirm();
				this.RefreshCostLabel();
			});
		}

		// Token: 0x06007B32 RID: 31538 RVA: 0x00393BEC File Offset: 0x00391DEC
		private string GetContent()
		{
			StringBuilder sb = new StringBuilder();
			string gender = (this.genderToggleGroup.GetActiveIndex() == 0) ? LanguageKey.UI_NewGame_BtnTip_SetFemale_Title.Tr() : LanguageKey.UI_NewGame_BtnTip_SetMale_Title.Tr();
			sb.AppendLine(LanguageKey.LK_CreateMirrorCharacter_Desc_2.TrFormat(this._nameCharacter.Name, gender));
			sb.AppendLine(LanguageKey.LK_CreateMirrorCharacter_Desc_3.TrFormat(NameCenter.GetMonasticTitleOrDisplayName(this._data, true)).SetColor("brightred"));
			sb.AppendLine(LanguageKey.LK_CreateMirrorCharacter_Desc_4.Tr().ColorReplace());
			return sb.ToString();
		}

		// Token: 0x06007B33 RID: 31539 RVA: 0x00393C88 File Offset: 0x00391E88
		private void RefreshAvatar()
		{
			string taiwuName = NameCenter.GetMonasticTitleOrDisplayName(this._data, true);
			this.taiwuNameLabel.text = taiwuName;
			this.genderToggleGroup.SetWithoutNotify((int)this._data.Gender);
			this.taiwuAvatar.Refresh(this._data, true);
			this.taiwuAvatar.gameObject.SetActive(true);
			this.RefreshMirrorAvatar();
			this.descLabel.text = LanguageKey.LK_CreateMirrorCharacter_Desc_1.TrFormat(taiwuName);
		}

		// Token: 0x06007B34 RID: 31540 RVA: 0x00393D0C File Offset: 0x00391F0C
		private void RefreshMirrorAvatar()
		{
			this._mirrorAvatar = new AvatarRelatedData(this._data.AvatarRelatedData)
			{
				ClothingDisplayId = 0,
				DisplayAge = 16
			};
			sbyte gender = (sbyte)this.genderToggleGroup.GetActiveIndex();
			bool transgender = gender == this.taiwuAvatar.Data.Gender;
			for (sbyte i = 0; i < 7; i += 1)
			{
				this._mirrorAvatar.AvatarData.SetGrowableElementShowingAbility(i, GameData.Domains.Character.SharedMethods.IsAbleToGrowAvatarElement(i, 0, 16, gender, transgender, null, 0));
			}
			this.mirrorAvatar.Refresh(this._mirrorAvatar);
			this.mirrorAvatar.gameObject.SetActive(true);
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06007B35 RID: 31541 RVA: 0x00393DC0 File Offset: 0x00391FC0
		private void RefreshNeili()
		{
			this.neiliLabel.Set((uint)this._neiliData.CurrNeili, 2, 3, "brightyellow");
			this.neiliMaxLabel.text = "/" + this._neiliData.MaxNeili.ToString();
		}

		// Token: 0x06007B36 RID: 31542 RVA: 0x00393E14 File Offset: 0x00392014
		private unsafe void RefreshNeiliAllocation(NeiliAllocation baseValue, NeiliAllocation combatValue, NeiliAllocation value)
		{
			for (byte i = 0; i < 4; i += 1)
			{
				short allocateValue = (combatValue.GetTotal() > 0) ? (*combatValue[(int)i]) : (*value[(int)i]);
				int displayDelta = (int)(*value[(int)i] - *baseValue[(int)i]);
				int displayValue = (int)allocateValue - displayDelta;
				this.neiliAllocationTypes.Set(i, displayValue, displayDelta, true);
			}
		}

		// Token: 0x06007B37 RID: 31543 RVA: 0x00393E80 File Offset: 0x00392080
		private void RefreshBtnConfirm()
		{
			this.btnConfirm.interactable = (!string.IsNullOrEmpty(this._nameCharacter.FamilyName) && !string.IsNullOrEmpty(this._nameCharacter.GivenName) && !NameCenter.HasInvalidCharForName(this._nameCharacter.FamilyName) && !NameCenter.HasInvalidCharForName(this._nameCharacter.GivenName) && SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays() >= ViewXuannvCreateMirrorCharacter.Cost);
		}

		// Token: 0x06007B38 RID: 31544 RVA: 0x00393EFC File Offset: 0x003920FC
		private void RefreshCostLabel()
		{
			int value = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			this.costLabel.text = string.Format("{0}/{1}", value.ToString().SetColor((value >= ViewXuannvCreateMirrorCharacter.Cost) ? "brightblue" : "brightred"), ViewXuannvCreateMirrorCharacter.Cost);
		}

		// Token: 0x06007B39 RID: 31545 RVA: 0x00393F55 File Offset: 0x00392155
		private void OnGenderChange(int _, int __)
		{
			this.RefreshMirrorAvatar();
		}

		// Token: 0x06007B3A RID: 31546 RVA: 0x00393F5F File Offset: 0x0039215F
		private void OnRefreshNameCharacter(string familyNameStr, string givenNameStr)
		{
			this.RefreshBtnConfirm();
		}

		// Token: 0x06007B3B RID: 31547 RVA: 0x00393F6C File Offset: 0x0039216C
		private void OnClickRandomName()
		{
			sbyte gender = (sbyte)this.genderToggleGroup.GetActiveIndex();
			CharacterDomainMethod.AsyncCall.GenerateRandomHanName(null, -1, -1, gender, delegate(int offset, RawDataPool dataPool)
			{
				FullName fullName = default(FullName);
				Serializer.Deserialize(dataPool, offset, ref fullName);
				ValueTuple<string, string> name = fullName.GetName(gender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
				string familyNameText = name.Item1;
				string givenNameText = name.Item2;
				this._nameCharacter.Refresh(familyNameText, givenNameText);
			});
		}

		// Token: 0x06007B3C RID: 31548 RVA: 0x00393FB4 File Offset: 0x003921B4
		private void OnClickConfirm()
		{
			DialogCmd dialogCmd = new DialogCmd
			{
				Type = 1,
				Title = LanguageKey.LK_CreateMirrorCharacter_Title.Tr(),
				Content = this.GetContent(),
				Yes = new Action(this.OnConfirm)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06007B3D RID: 31549 RVA: 0x00394028 File Offset: 0x00392228
		private unsafe void OnConfirm()
		{
			Sequence sequence = this._sequence;
			if (sequence != null)
			{
				sequence.Kill(false);
			}
			this._sequence = DOTween.Sequence();
			this._sequence.AppendCallback(delegate
			{
				this.btnConfirm.interactable = false;
			});
			this._sequence.AppendCallback(delegate
			{
				foreach (UIParticle particle in this.appearParticles)
				{
					particle.gameObject.SetActive(true);
					particle.Play();
				}
				foreach (UIParticle particle2 in this.loopParticles)
				{
					particle2.Play();
				}
			});
			this._sequence.AppendInterval(0.3f);
			this._sequence.AppendCallback(delegate
			{
				this.hidingObjects.DOFade(0f, 0.5f);
			});
			this._sequence.AppendCallback(delegate
			{
				foreach (UIParticle particle in this.appearParticles)
				{
					particle.GetComponent<CanvasGroup>().DOFade(0f, 0.4f);
				}
				foreach (UIParticle particle2 in this.loopParticles)
				{
					particle2.GetComponent<CanvasGroup>().DOFade(1f, 0.4f);
				}
			});
			for (byte i = 0; i < 4; i += 1)
			{
				byte index = i;
				TweenCallback<float> <>9__13;
				this._sequence.AppendCallback(delegate
				{
					float from = (float)(*this._neiliData.BaseNeiliAllocation[(int)index]);
					float to = 0f;
					float duration = 1.5f;
					TweenCallback<float> onVirtualUpdate;
					if ((onVirtualUpdate = <>9__13) == null)
					{
						onVirtualUpdate = (<>9__13 = delegate(float inter)
						{
							this.neiliAllocationTypes.Set(index, (int)inter, 0, true);
						});
					}
					DOVirtual.Float(from, to, duration, onVirtualUpdate);
				});
			}
			this._sequence.AppendCallback(delegate
			{
				DOVirtual.Float((float)this._neiliData.CurrNeili, 0f, 1.5f, delegate(float inter)
				{
					this.neiliLabel.Set((uint)inter, 2, 3, "brightyellow");
				});
			});
			this._sequence.AppendInterval(0.2f);
			this._sequence.AppendCallback(delegate
			{
				for (int j = 0; j < this.lineParticles.childCount; j++)
				{
					Transform obj = this.lineParticles.GetChild(j);
					obj.gameObject.SetActive(true);
					obj.GetComponent<ParticleSystem>().Play();
				}
			});
			this._sequence.AppendInterval(0.5f);
			this._sequence.AppendCallback(delegate
			{
				this.lightParticle.gameObject.SetActive(true);
			});
			this._sequence.AppendCallback(delegate
			{
				this.lightParticle.Play();
			});
			this._sequence.AppendInterval(0.3f);
			this._sequence.AppendCallback(delegate
			{
				foreach (UIParticle particle in this.loopParticles)
				{
					particle.GetComponent<CanvasGroup>().DOFade(0f, 0.4f);
				}
			});
			this._sequence.AppendCallback(delegate
			{
				this.iceReflectParticle.gameObject.SetActive(true);
			});
			this._sequence.AppendCallback(delegate
			{
				this.iceReflectParticle.Play();
			});
			this._sequence.AppendInterval(0.5f);
			this._sequence.AppendCallback(delegate
			{
				this.mirrorCanvas.DOFade(0f, 0.5f);
			});
			this._sequence.AppendInterval(0.5f);
			this._sequence.AppendCallback(new TweenCallback(this.OnAnimFinish));
			this._sequence.Play<Sequence>();
		}

		// Token: 0x06007B3E RID: 31550 RVA: 0x00394240 File Offset: 0x00392440
		private void OnAnimFinish()
		{
			WorldDomainMethod.Call.AdvanceDaysInMonth(ViewXuannvCreateMirrorCharacter.Cost);
			StoryDomainMethod.AsyncCall.CreateMirrorCharacter(null, this.genderToggleGroup.GetActiveIndex() == 1, this._nameCharacter.FamilyName, this._nameCharacter.GivenName, delegate(int offset, RawDataPool dataPool)
			{
				int charId = -1;
				Serializer.Deserialize(dataPool, offset, ref charId);
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("CharIdList", new List<int>
				{
					charId
				});
				argBox.SetObject("CloseAction", new Action(this.CloseAction));
				argBox.Set("ObtainType", 14);
				UIElement.GetItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.GetItem);
			});
		}

		// Token: 0x06007B3F RID: 31551 RVA: 0x00394290 File Offset: 0x00392490
		private void CloseAction()
		{
			this.QuickHide();
			for (int i = 0; i < this.lineParticles.childCount; i++)
			{
				this.lineParticles.GetChild(i).gameObject.SetActive(false);
			}
			foreach (UIParticle particle in this.appearParticles)
			{
				particle.gameObject.SetActive(false);
				particle.GetComponent<CanvasGroup>().alpha = 1f;
			}
			foreach (UIParticle particle2 in this.loopParticles)
			{
				particle2.GetComponent<CanvasGroup>().alpha = 0f;
			}
			this.iceReflectParticle.gameObject.SetActive(false);
			this.lightParticle.gameObject.SetActive(false);
			this.hidingObjects.alpha = 1f;
			this.mirrorCanvas.alpha = 1f;
		}

		// Token: 0x06007B40 RID: 31552 RVA: 0x00394390 File Offset: 0x00392590
		private void OnClickBtnOpenCharMenu()
		{
			bool flag = this._data == null;
			if (!flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", this._data.CharacterId);
				argBox.Set("CanOperate", true);
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			}
		}

		// Token: 0x06007B41 RID: 31553 RVA: 0x003943F4 File Offset: 0x003925F4
		public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			base.OnLanguageChange(languageType);
			this.nameCharacterCN.gameObject.SetActive(false);
			this.nameCharacterEN.gameObject.SetActive(false);
			this._nameCharacter = ((languageType == LocalStringManager.LanguageType.CN) ? this.nameCharacterCN : this.nameCharacterEN);
			this._nameCharacter.gameObject.SetActive(true);
		}

		// Token: 0x04005D7D RID: 23933
		[SerializeField]
		private Game.Components.Avatar.Avatar taiwuAvatar;

		// Token: 0x04005D7E RID: 23934
		[SerializeField]
		private Game.Components.Avatar.Avatar mirrorAvatar;

		// Token: 0x04005D7F RID: 23935
		[SerializeField]
		private TextMeshProUGUI taiwuNameLabel;

		// Token: 0x04005D80 RID: 23936
		[SerializeField]
		private CToggleGroup genderToggleGroup;

		// Token: 0x04005D81 RID: 23937
		[SerializeField]
		private NeiliAllocationTypes neiliAllocationTypes;

		// Token: 0x04005D82 RID: 23938
		[SerializeField]
		private TextMeshProUGUI descLabel;

		// Token: 0x04005D83 RID: 23939
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x04005D84 RID: 23940
		[SerializeField]
		private TextMeshProUGUI costLabel;

		// Token: 0x04005D85 RID: 23941
		[SerializeField]
		private ImageDigits neiliLabel;

		// Token: 0x04005D86 RID: 23942
		[SerializeField]
		private TextMeshProUGUI neiliMaxLabel;

		// Token: 0x04005D87 RID: 23943
		[SerializeField]
		private CanvasGroup hidingObjects;

		// Token: 0x04005D88 RID: 23944
		[SerializeField]
		private CanvasGroup mirrorCanvas;

		// Token: 0x04005D89 RID: 23945
		[SerializeField]
		private ParticleSystem iceReflectParticle;

		// Token: 0x04005D8A RID: 23946
		[SerializeField]
		private ParticleSystem lightParticle;

		// Token: 0x04005D8B RID: 23947
		[SerializeField]
		private UIParticle[] appearParticles;

		// Token: 0x04005D8C RID: 23948
		[SerializeField]
		private UIParticle[] loopParticles;

		// Token: 0x04005D8D RID: 23949
		[SerializeField]
		private Transform lineParticles;

		// Token: 0x04005D8E RID: 23950
		[SerializeField]
		private NameCharacter nameCharacterCN;

		// Token: 0x04005D8F RID: 23951
		[SerializeField]
		private NameCharacter nameCharacterEN;

		// Token: 0x04005D90 RID: 23952
		[SerializeField]
		private CButton btnOpenCharMenu;

		// Token: 0x04005D91 RID: 23953
		private NameCharacter _nameCharacter;

		// Token: 0x04005D92 RID: 23954
		private CharacterDisplayData _data;

		// Token: 0x04005D93 RID: 23955
		private CharacterDisplayDataForNeiliPage _neiliData;

		// Token: 0x04005D94 RID: 23956
		private AvatarRelatedData _mirrorAvatar;

		// Token: 0x04005D95 RID: 23957
		private Sequence _sequence;

		// Token: 0x04005D96 RID: 23958
		private const int DisplayMinCount = 2;

		// Token: 0x04005D97 RID: 23959
		private const int DisplayMaxCount = 3;

		// Token: 0x04005D98 RID: 23960
		private const int MirrorAvatarAge = 16;
	}
}
