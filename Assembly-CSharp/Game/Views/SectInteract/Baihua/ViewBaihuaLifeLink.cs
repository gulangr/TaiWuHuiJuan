using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Views.Select;
using Game.Views.Select.SelectCharacter;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Story;
using GameData.Domains.Story.SectMainStory;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Baihua
{
	// Token: 0x020009F3 RID: 2547
	public class ViewBaihuaLifeLink : UIBase
	{
		// Token: 0x17000DB4 RID: 3508
		// (get) Token: 0x06007D59 RID: 32089 RVA: 0x003A3AC2 File Offset: 0x003A1CC2
		private int Count
		{
			get
			{
				return this._isUpgraded ? 8 : 4;
			}
		}

		// Token: 0x06007D5A RID: 32090 RVA: 0x003A3AD0 File Offset: 0x003A1CD0
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x06007D5B RID: 32091 RVA: 0x003A3AD4 File Offset: 0x003A1CD4
		private void Awake()
		{
			for (int i = 0; i < 8; i++)
			{
				this.lifeGates[i].Init(i, true, new Action<bool>(this.OnClickBtnSelect), new Action<int, bool>(this.OnClickBtnCancel));
				this.deathGates[i].Init(i, false, new Action<bool>(this.OnClickBtnSelect), new Action<int, bool>(this.OnClickBtnCancel));
			}
			this.aliveText.SetSprite(string.Format("ui9_text_alive_{0}", SingletonObject.getInstance<GlobalSettings>().Language.ToLower()), false, null);
			this.deadText.SetSprite(string.Format("ui9_text_dead_{0}", SingletonObject.getInstance<GlobalSettings>().Language.ToLower()), false, null);
		}

		// Token: 0x06007D5C RID: 32092 RVA: 0x003A3B92 File Offset: 0x003A1D92
		private void OnEnable()
		{
			this.particle.Play();
			this.RequestData();
		}

		// Token: 0x06007D5D RID: 32093 RVA: 0x003A3BA8 File Offset: 0x003A1DA8
		private void OnDisable()
		{
			this._selectList.Clear();
		}

		// Token: 0x06007D5E RID: 32094 RVA: 0x003A3BB8 File Offset: 0x003A1DB8
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "ButtonCloseView" == btnName;
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007D5F RID: 32095 RVA: 0x003A3BE5 File Offset: 0x003A1DE5
		private void RequestData()
		{
			this._characterDict.Clear();
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForBaihuaLifeLink(this, delegate(int offset, RawDataPool dataPool)
			{
				List<CharacterDisplayDataForLifeLink> data = new List<CharacterDisplayDataForLifeLink>();
				Serializer.Deserialize(dataPool, offset, ref data);
				foreach (CharacterDisplayDataForLifeLink character in data)
				{
					this._characterDict[character.ListData.CharacterId] = new BaihuaLifeLinkSelectCharacterDataAdapter(character);
				}
			});
			this.RequestAndUpdate();
		}

		// Token: 0x06007D60 RID: 32096 RVA: 0x003A3C0E File Offset: 0x003A1E0E
		private void RequestAndUpdate()
		{
			StoryDomainMethod.AsyncCall.GetSectBaihuaLifeLinkDisplayData(this, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._data);
				this.UpdateNeiliType();
				this.UpdateCooldown();
				this.UpdateGates();
			});
		}

		// Token: 0x06007D61 RID: 32097 RVA: 0x003A3C24 File Offset: 0x003A1E24
		private unsafe void UpdateNeiliType()
		{
			bool flag = this._data.NeiliType < 0;
			if (flag)
			{
				this.neiliTypeTips.gameObject.SetActive(false);
				this.neiliTypeParticles.gameObject.SetActive(false);
				this.neiliTypeName.text = "";
				this.barObj.SetActive(false);
				for (int i = 0; i < this.neiliTypeParticles.childCount; i++)
				{
					this.neiliTypeParticles.GetChild(i).gameObject.SetActive(false);
				}
			}
			else
			{
				int[] rawData = new int[5];
				int rawTotal = 0;
				NeiliProportionOfFiveElements total = default(NeiliProportionOfFiveElements);
				foreach (CharacterDisplayDataForLifeLink data in this._data.CharacterDisplayData.Values)
				{
					for (int j = 0; j < 5; j++)
					{
						rawData[j] += (int)(*data.NeiliPercent[j]);
						rawTotal += (int)(*data.NeiliPercent[j]);
					}
				}
				for (int k = 0; k < 5; k++)
				{
					*total[k] = (sbyte)(rawData[k] * 100 / rawTotal);
					bool flag2 = k == (int)NeiliType.Instance[this._data.NeiliType].FiveElements;
					if (flag2)
					{
						Transform obj = this.neiliTypeParticles.GetChild(k);
						obj.gameObject.SetActive(true);
						obj.GetComponent<ParticleSystem>().Play();
					}
				}
				this.neiliTypeName.text = NeiliType.Instance[this._data.NeiliType].Name;
				this.neiliTypeTips.RuntimeParam = new ArgumentBox().Set("NeiliType", this._data.NeiliType).Set<NeiliProportionOfFiveElements>("Total", total);
				this.neiliTypeTips.Refresh(false, -1);
				this.neiliTypeParticles.gameObject.SetActive(true);
				this.neiliTypeTips.gameObject.SetActive(true);
				this.barObj.SetActive(true);
				this.RefreshColourBar(total);
			}
		}

		// Token: 0x06007D62 RID: 32098 RVA: 0x003A3E88 File Offset: 0x003A2088
		private unsafe void RefreshColourBar(NeiliProportionOfFiveElements total)
		{
			int sum = 0;
			for (int i = 0; i < 5; i++)
			{
				sum += (int)(*total[i]);
			}
			bool flag = sum < 100 && sum >= 95;
			if (flag)
			{
				sum = 100;
			}
			for (int j = 0; j < 5; j++)
			{
				sbyte num = *total[j];
				CImage colour = this.colourBar[j];
				colour.fillAmount = (float)sum * 0.01f;
				RectTransform interval = this.colourBarInterval[j];
				bool flag2 = num == 0 || num == 100;
				if (flag2)
				{
					interval.gameObject.SetActive(false);
				}
				else
				{
					interval.gameObject.SetActive(true);
					interval.localEulerAngles = new Vector3(0f, 0f, -((float)sum * 0.01f) * 360f);
				}
				sum -= (int)num;
			}
		}

		// Token: 0x06007D63 RID: 32099 RVA: 0x003A3F7C File Offset: 0x003A217C
		private void UpdateCooldown()
		{
			bool flag = this._data.Data.Cooldown <= 0;
			if (flag)
			{
				this.cooldown.SetActive(false);
			}
			else
			{
				this.cooldownText.text = LocalStringManager.GetFormat(LanguageKey.LK_Baihua_LifeLink_Cooldown_Tip, this._data.Data.Cooldown.ToString());
				this.cooldownText.GetComponent<TMPTextSpriteHelper>().Parse();
				this.cooldown.SetActive(true);
			}
		}

		// Token: 0x06007D64 RID: 32100 RVA: 0x003A4000 File Offset: 0x003A2200
		private void UpdateGates()
		{
			for (int i = 0; i < 8; i++)
			{
				this.lifeGates[i].SetDisable();
				this.deathGates[i].SetDisable();
			}
			bool flag = !this._data.Data.IsInitialized();
			if (flag)
			{
				this._isUpgraded = false;
			}
			else
			{
				SectBaihuaLifeLinkDisplayData data4 = this._data;
				if (data4.CharacterDisplayData == null)
				{
					data4.CharacterDisplayData = new Dictionary<int, CharacterDisplayDataForLifeLink>();
				}
				this._isUpgraded = (this._data.Data.LifeGateCharIds.Length == 8);
				int fiveElementsTotal = this._data.CharacterDisplayData.Count * 100;
				foreach (KeyValuePair<int, CharacterDisplayDataForLifeLink> keyValuePair in this._data.CharacterDisplayData)
				{
					int num;
					CharacterDisplayDataForLifeLink characterDisplayDataForLifeLink;
					keyValuePair.Deconstruct(out num, out characterDisplayDataForLifeLink);
					int id = num;
					CharacterDisplayDataForLifeLink data = characterDisplayDataForLifeLink;
					this._characterDict[id] = new BaihuaLifeLinkSelectCharacterDataAdapter(data);
				}
				for (int j = 0; j < this._data.Data.LifeGateCharIds.Length; j++)
				{
					int charId = this._data.Data.LifeGateCharIds[j];
					bool flag2 = charId < 0;
					if (flag2)
					{
						this.lifeGates[j].SetEmpty();
					}
					else
					{
						BaihuaLifeLinkSelectCharacterDataAdapter data2;
						this.lifeGates[j].Set(this._characterDict.TryGetValue(charId, out data2) ? data2.GetRawData() : this._data.CharacterDisplayData[charId], fiveElementsTotal);
					}
				}
				for (int k = 0; k < this._data.Data.DeathGateCharIds.Length; k++)
				{
					int charId2 = this._data.Data.DeathGateCharIds[k];
					bool flag3 = charId2 < 0;
					if (flag3)
					{
						this.deathGates[k].SetEmpty();
					}
					else
					{
						BaihuaLifeLinkSelectCharacterDataAdapter data3;
						this.deathGates[k].Set(this._characterDict.TryGetValue(charId2, out data3) ? data3.GetRawData() : this._data.CharacterDisplayData[charId2], fiveElementsTotal);
					}
				}
				this.RefreshLinkParticles();
			}
		}

		// Token: 0x06007D65 RID: 32101 RVA: 0x003A4260 File Offset: 0x003A2460
		private void UpdateSelectList()
		{
			this._selectList.Clear();
			int[] bannedIds = this._isSelectingLifeGate ? this._data.Data.DeathGateCharIds : this._data.Data.LifeGateCharIds;
			foreach (KeyValuePair<int, BaihuaLifeLinkSelectCharacterDataAdapter> keyValuePair in this._characterDict)
			{
				int num;
				BaihuaLifeLinkSelectCharacterDataAdapter baihuaLifeLinkSelectCharacterDataAdapter;
				keyValuePair.Deconstruct(out num, out baihuaLifeLinkSelectCharacterDataAdapter);
				int charId = num;
				BaihuaLifeLinkSelectCharacterDataAdapter data = baihuaLifeLinkSelectCharacterDataAdapter;
				bool flag = !bannedIds.Contains(charId);
				if (flag)
				{
					this._selectList.Add(data);
				}
			}
		}

		// Token: 0x06007D66 RID: 32102 RVA: 0x003A4318 File Offset: 0x003A2518
		private void Update()
		{
			this.root.SetActive(!UIElement.CharacterMenu.Exist);
		}

		// Token: 0x06007D67 RID: 32103 RVA: 0x003A4334 File Offset: 0x003A2534
		private void OnClickBtnCancel(int index, bool isLife)
		{
			int charId = isLife ? this._data.Data.LifeGateCharIds[index] : this._data.Data.DeathGateCharIds[index];
			bool flag = charId < 0;
			if (!flag)
			{
				StoryDomainMethod.Call.SetLifeLinkCharacter(-1, index, isLife);
				this.RequestAndUpdate();
			}
		}

		// Token: 0x06007D68 RID: 32104 RVA: 0x003A4388 File Offset: 0x003A2588
		private void OnClickBtnSelect(bool isLife)
		{
			this._isSelectingLifeGate = isLife;
			this.UpdateSelectList();
			CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.Baihua);
			config.Title = LanguageKey.LK_Baihua_LifeLink_Title.Tr();
			config.InteractionMode = ESelectCharacterInteractionMode.Slot;
			config.SelectionMode = ESelectCharacterSelectionMode.Multiple;
			config.TargetCount = this.Count;
			config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
			{
				ESelectCharacterFilterMenuId.Gender,
				ESelectCharacterFilterMenuId.BehaviorType,
				ESelectCharacterFilterMenuId.Favorability,
				ESelectCharacterFilterMenuId.Taiwu
			};
			config.InitialSelectedCharacterIds = (from id in this._isSelectingLifeGate ? this._data.Data.LifeGateCharIds : this._data.Data.DeathGateCharIds
			where id >= 0
			select id).ToList<int>();
			this.sparkParticle.SetActive(false);
			UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", this._selectList).SetObject("SelectCharacterCallback", new SelectCharacterCallback(this.OnConfirmSelect)).SetObject("SelectCharacterCancelCallback", new Action(this.OnCancelSelect)));
			UIManager.Instance.MaskUI(UIElement.SelectChar);
		}

		// Token: 0x06007D69 RID: 32105 RVA: 0x003A44D5 File Offset: 0x003A26D5
		private void OnCancelSelect()
		{
			this.sparkParticle.SetActive(true);
		}

		// Token: 0x06007D6A RID: 32106 RVA: 0x003A44E8 File Offset: 0x003A26E8
		private void OnConfirmSelect(List<int> selectedIds)
		{
			for (int i = 0; i < this.Count; i++)
			{
				StoryDomainMethod.Call.SetLifeLinkCharacter(-1, i, this._isSelectingLifeGate);
			}
			for (int j = 0; j < this.Count; j++)
			{
				StoryDomainMethod.Call.SetLifeLinkCharacter((j >= selectedIds.Count) ? -1 : selectedIds[j], j, this._isSelectingLifeGate);
			}
			this.RequestAndUpdate();
			this.sparkParticle.SetActive(true);
		}

		// Token: 0x06007D6B RID: 32107 RVA: 0x003A4564 File Offset: 0x003A2764
		private void RefreshLinkParticles()
		{
			for (int i = 0; i < 4; i++)
			{
				bool flag = this.IsLinkEmpty(true, i);
				if (flag)
				{
					this.PlayEmptyParticle(this.leftParticles, i);
				}
				else
				{
					this.PlayNonEmptyParticle(this.leftParticles, i);
				}
				bool flag2 = this.IsLinkEmpty(false, i);
				if (flag2)
				{
					this.PlayEmptyParticle(this.rightParticles, i);
				}
				else
				{
					this.PlayNonEmptyParticle(this.rightParticles, i);
				}
			}
		}

		// Token: 0x06007D6C RID: 32108 RVA: 0x003A45D8 File Offset: 0x003A27D8
		private void PlayEmptyParticle(Transform particles, int index)
		{
			particles.GetChild(0).GetChild(index).gameObject.SetActive(false);
			Transform obj = particles.GetChild(1).GetChild(index);
			obj.gameObject.SetActive(true);
			obj.GetComponent<ParticleSystem>().Play();
		}

		// Token: 0x06007D6D RID: 32109 RVA: 0x003A4628 File Offset: 0x003A2828
		private void PlayNonEmptyParticle(Transform particles, int index)
		{
			particles.GetChild(1).GetChild(index).gameObject.SetActive(false);
			Transform obj = particles.GetChild(0).GetChild(index);
			Transform appear = obj.GetChild(0);
			Transform loop = obj.GetChild(1);
			obj.gameObject.SetActive(true);
			loop.gameObject.SetActive(false);
			appear.gameObject.SetActive(true);
			appear.GetComponent<ParticleSystem>().Play();
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				appear.gameObject.SetActive(false);
				loop.gameObject.SetActive(true);
				loop.GetComponent<ParticleSystem>().Play();
			});
		}

		// Token: 0x06007D6E RID: 32110 RVA: 0x003A46D8 File Offset: 0x003A28D8
		private bool IsLinkEmpty(bool isLife, int i)
		{
			int[] data = isLife ? this._data.Data.LifeGateCharIds : this._data.Data.DeathGateCharIds;
			return data[i] < 0 && (!this._isUpgraded || data[i + 4] < 0);
		}

		// Token: 0x04005F62 RID: 24418
		[SerializeField]
		private CImage aliveText;

		// Token: 0x04005F63 RID: 24419
		[SerializeField]
		private CImage deadText;

		// Token: 0x04005F64 RID: 24420
		[SerializeField]
		private GameObject cooldown;

		// Token: 0x04005F65 RID: 24421
		[SerializeField]
		private TextMeshProUGUI cooldownText;

		// Token: 0x04005F66 RID: 24422
		[SerializeField]
		private TextMeshProUGUI neiliTypeName;

		// Token: 0x04005F67 RID: 24423
		[SerializeField]
		private TooltipInvoker neiliTypeTips;

		// Token: 0x04005F68 RID: 24424
		[SerializeField]
		private BaihuaLifeLink[] lifeGates;

		// Token: 0x04005F69 RID: 24425
		[SerializeField]
		private BaihuaLifeLink[] deathGates;

		// Token: 0x04005F6A RID: 24426
		[SerializeField]
		private ParticleSystem particle;

		// Token: 0x04005F6B RID: 24427
		[SerializeField]
		private Transform neiliTypeParticles;

		// Token: 0x04005F6C RID: 24428
		[SerializeField]
		private Transform leftParticles;

		// Token: 0x04005F6D RID: 24429
		[SerializeField]
		private Transform rightParticles;

		// Token: 0x04005F6E RID: 24430
		[SerializeField]
		private GameObject sparkParticle;

		// Token: 0x04005F6F RID: 24431
		[SerializeField]
		private GameObject root;

		// Token: 0x04005F70 RID: 24432
		[SerializeField]
		private CImage[] colourBar;

		// Token: 0x04005F71 RID: 24433
		[SerializeField]
		private RectTransform[] colourBarInterval;

		// Token: 0x04005F72 RID: 24434
		[SerializeField]
		private GameObject barObj;

		// Token: 0x04005F73 RID: 24435
		private Dictionary<int, BaihuaLifeLinkSelectCharacterDataAdapter> _characterDict = new Dictionary<int, BaihuaLifeLinkSelectCharacterDataAdapter>();

		// Token: 0x04005F74 RID: 24436
		private List<ISelectCharacterData> _selectList = new List<ISelectCharacterData>();

		// Token: 0x04005F75 RID: 24437
		private SectBaihuaLifeLinkDisplayData _data;

		// Token: 0x04005F76 RID: 24438
		private bool _isUpgraded;

		// Token: 0x04005F77 RID: 24439
		private bool _isSelectingLifeGate;
	}
}
