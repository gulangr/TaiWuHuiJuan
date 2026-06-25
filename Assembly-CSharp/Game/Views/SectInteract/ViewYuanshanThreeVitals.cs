using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UI;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Story.SectMainStory;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009BC RID: 2492
	public class ViewYuanshanThreeVitals : UIBase
	{
		// Token: 0x17000D6F RID: 3439
		// (get) Token: 0x060078C3 RID: 30915 RVA: 0x00382F61 File Offset: 0x00381161
		private string SoundPrefix
		{
			get
			{
				return this._data.IsGoodEnd ? "sancai" : "sanmo";
			}
		}

		// Token: 0x17000D70 RID: 3440
		// (get) Token: 0x060078C4 RID: 30916 RVA: 0x00382F7C File Offset: 0x0038117C
		private Transform VitalParent
		{
			get
			{
				return this._data.IsGoodEnd ? this.vitalsGood : this.vitalsBad;
			}
		}

		// Token: 0x060078C5 RID: 30917 RVA: 0x00382F99 File Offset: 0x00381199
		public override void OnInit(ArgumentBox argsBox)
		{
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
		}

		// Token: 0x060078C6 RID: 30918 RVA: 0x00382FC4 File Offset: 0x003811C4
		private void Awake()
		{
			for (int i = 0; i < this.vitalsGood.childCount; i++)
			{
				this.vitalsGood.GetChild(i).GetComponent<YuanshanVital>().Init(i, new Action<int>(this.OnVitalLeave), new Action<int>(this.OnStartTransfer));
				this.vitalsBad.GetChild(i).GetComponent<YuanshanVital>().Init(i, new Action<int>(this.OnVitalLeave), new Action<int>(this.OnStartTransfer));
			}
			this.transferInfection.Init(new Action<int, int, int>(this.OnConfirmTransfer));
		}

		// Token: 0x060078C7 RID: 30919 RVA: 0x00383066 File Offset: 0x00381266
		private void OnEnable()
		{
			GEvent.Add(UiEvents.PlayVitalAnim, new GEvent.Callback(this.PlayVitalAnim));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}

		// Token: 0x060078C8 RID: 30920 RVA: 0x003830A0 File Offset: 0x003812A0
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.PlayVitalAnim, new GEvent.Callback(this.PlayVitalAnim));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 0.2f, 100);
		}

		// Token: 0x060078C9 RID: 30921 RVA: 0x003830FC File Offset: 0x003812FC
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "ButtonCloseView" == btnName;
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x060078CA RID: 30922 RVA: 0x0038312C File Offset: 0x0038132C
		public override void QuickHide()
		{
			bool activeSelf = this.transferInfection.gameObject.activeSelf;
			if (activeSelf)
			{
				UIMaskManager manager = SingletonObject.getInstance<UIMaskManager>();
				manager.DetachMask(this.transferInfection.transform);
				this.transferInfection.gameObject.SetActive(false);
			}
			else
			{
				base.QuickHide();
			}
		}

		// Token: 0x060078CB RID: 30923 RVA: 0x00383182 File Offset: 0x00381382
		private void RequestData()
		{
			ExtraDomainMethod.AsyncCall.GetThreeVitalsCharDataList(this, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._threeVitalsCharDataList);
			});
			ExtraDomainMethod.AsyncCall.GetSectYuanshanThreeVitalsData(this, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._data);
				for (int i = 0; i < this.vitalsGood.childCount; i++)
				{
					this.GetVital(i).Set(this._data.IsGoodEnd, this._threeVitalsCharDataList[i], this._data.ThreeVitals[i]);
					bool flag = !this._data.ThreeVitals[i].HasPlayedComeAnim;
					if (flag)
					{
						this.PlayVitalComeAnim(i);
					}
				}
				this.PlayAmbience();
				this.good.SetActive(this._data.IsGoodEnd);
				this.bad.SetActive(!this._data.IsGoodEnd);
			});
		}

		// Token: 0x060078CC RID: 30924 RVA: 0x003831AC File Offset: 0x003813AC
		private void PlayAmbience()
		{
			string targetAmbience = this.SoundPrefix + this._soundSuffix[0];
			string currentAmbience = AudioManager.Instance.GetPlayingAmbience();
			bool flag = targetAmbience == currentAmbience;
			if (flag)
			{
				AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 0f, 100);
			}
			AudioManager.Instance.PlayAmbience(targetAmbience, 1f, 100);
		}

		// Token: 0x060078CD RID: 30925 RVA: 0x00383210 File Offset: 0x00381410
		private void OnTopUiChanged(ArgumentBox argBox)
		{
			bool flag = !UIManager.Instance.IsFocusElement(UIElement.ThreeVitals);
			if (flag)
			{
				bool isActiveAndEnabled = this.transferInfection.isActiveAndEnabled;
				if (isActiveAndEnabled)
				{
					this._isDisplayingTransfer = true;
					UIMaskManager manager = SingletonObject.getInstance<UIMaskManager>();
					manager.DetachMask(this.transferInfection.transform);
					this.transferInfection.gameObject.SetActive(false);
				}
			}
			else
			{
				bool isDisplayingTransfer = this._isDisplayingTransfer;
				if (isDisplayingTransfer)
				{
					this._isDisplayingTransfer = false;
					UIMaskManager manager2 = SingletonObject.getInstance<UIMaskManager>();
					manager2.AttachMaskTo(this.transferInfection.transform);
					this.transferInfection.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x060078CE RID: 30926 RVA: 0x003832BB File Offset: 0x003814BB
		private void OnVitalLeave(int vitalIndex)
		{
			TaiwuEventDomainMethod.Call.OnClickDeportButton(vitalIndex, this._data.IsGoodEnd);
		}

		// Token: 0x060078CF RID: 30927 RVA: 0x003832D0 File Offset: 0x003814D0
		private void OnStartTransfer(int vitalIndex)
		{
			UIMaskManager manager = SingletonObject.getInstance<UIMaskManager>();
			manager.AttachMaskTo(this.transferInfection.transform);
			this.transferInfection.Set(this._data, this._threeVitalsCharDataList, vitalIndex);
			this.transferInfection.gameObject.SetActive(true);
		}

		// Token: 0x060078D0 RID: 30928 RVA: 0x00383324 File Offset: 0x00381524
		private void OnConfirmTransfer(int vitalIndex, int targetCharId, int value)
		{
			this.GetVital(vitalIndex).Set(this._data.IsGoodEnd, this._threeVitalsCharDataList[vitalIndex], this._data.ThreeVitals[vitalIndex]);
			AudioManager.Instance.PlaySound(this.SoundPrefix + this._soundSuffix[3], false, false);
			ExtraDomainMethod.Call.TransferInfectionBetweenVitalAndCharacter(targetCharId, (SectStoryThreeVitalsCharacterType)vitalIndex, value);
		}

		// Token: 0x060078D1 RID: 30929 RVA: 0x00383390 File Offset: 0x00381590
		private void PlayVitalAnim(ArgumentBox box)
		{
			int type;
			box.Get("Type", out type);
			bool isInPrison;
			box.Get("IsInPrison", out isInPrison);
			bool flag = isInPrison;
			if (flag)
			{
				this.PlayVitalLeaveAnim(type);
			}
			else
			{
				this.PlayVitalComeAnim(type);
			}
		}

		// Token: 0x060078D2 RID: 30930 RVA: 0x003833D4 File Offset: 0x003815D4
		private void PlayVitalLeaveAnim(int index)
		{
			for (int i = 0; i < this._data.ThreeVitals.Count; i++)
			{
				bool flag = i != index && !this._data.ThreeVitals[i].IsInPrison;
				if (flag)
				{
					this.ShowBubble(i);
				}
			}
			AudioManager.Instance.PlaySound(this.SoundPrefix + this._soundSuffix[2], false, false);
			this.GetVital(index).Leave();
			this._data.ThreeVitals[index].IsInPrison = true;
		}

		// Token: 0x060078D3 RID: 30931 RVA: 0x00383474 File Offset: 0x00381674
		private void PlayVitalComeAnim(int index)
		{
			AudioManager.Instance.PlaySound(this.SoundPrefix + this._soundSuffix[1], false, false);
			ExtraDomainMethod.Call.SetVitalHasPlayedComeAnim(this._data.ThreeVitals[index].VitalType, true);
			this.GetVital(index).Come();
			this._data.ThreeVitals[index].IsInPrison = false;
		}

		// Token: 0x060078D4 RID: 30932 RVA: 0x003834E4 File Offset: 0x003816E4
		private void ShowBubble(int index)
		{
			int textIndex = this._data.IsGoodEnd ? 0 : 1;
			this.GetVital(index).ShowBubble(ViewYuanshanThreeVitals.VitalBubbleConfig[this.GetTypeByIndex(index)][textIndex].Tr());
		}

		// Token: 0x060078D5 RID: 30933 RVA: 0x00383529 File Offset: 0x00381729
		private YuanshanVital GetVital(int index)
		{
			return this.VitalParent.GetChild(index).GetComponent<YuanshanVital>();
		}

		// Token: 0x060078D6 RID: 30934 RVA: 0x0038353C File Offset: 0x0038173C
		private SectStoryThreeVitalsCharacterType GetTypeByIndex(int index)
		{
			return (SectStoryThreeVitalsCharacterType)index;
		}

		// Token: 0x04005B73 RID: 23411
		public Transform vitalsGood;

		// Token: 0x04005B74 RID: 23412
		public Transform vitalsBad;

		// Token: 0x04005B75 RID: 23413
		public GameObject good;

		// Token: 0x04005B76 RID: 23414
		public GameObject bad;

		// Token: 0x04005B77 RID: 23415
		public YuanshanTransferInfection transferInfection;

		// Token: 0x04005B78 RID: 23416
		private SectYuanshanThreeVitalsData _data;

		// Token: 0x04005B79 RID: 23417
		private List<CharacterDisplayData> _threeVitalsCharDataList = new List<CharacterDisplayData>();

		// Token: 0x04005B7A RID: 23418
		private static readonly Dictionary<SectStoryThreeVitalsCharacterType, LanguageKey[]> VitalBubbleConfig = new Dictionary<SectStoryThreeVitalsCharacterType, LanguageKey[]>
		{
			{
				SectStoryThreeVitalsCharacterType.Heaven,
				new LanguageKey[]
				{
					LanguageKey.LK_ThreeVitals_Speak_BanishOther_EssenceSky,
					LanguageKey.LK_ThreeVitals_Speak_BanishOther_DemonSky
				}
			},
			{
				SectStoryThreeVitalsCharacterType.Earth,
				new LanguageKey[]
				{
					LanguageKey.LK_ThreeVitals_Speak_BanishOther_EssenceEarth,
					LanguageKey.LK_ThreeVitals_Speak_BanishOther_DemonEarth
				}
			},
			{
				SectStoryThreeVitalsCharacterType.Human,
				new LanguageKey[]
				{
					LanguageKey.LK_ThreeVitals_Speak_BanishOther_EssenceHuman,
					LanguageKey.LK_ThreeVitals_Speak_BanishOther_DemonHuman
				}
			}
		};

		// Token: 0x04005B7B RID: 23419
		private readonly string[] _soundSuffix = new string[]
		{
			"_ambience",
			"_appear",
			"_disappear",
			"_absorb"
		};

		// Token: 0x04005B7C RID: 23420
		private bool _isDisplayingTransfer;
	}
}
