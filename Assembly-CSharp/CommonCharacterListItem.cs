using System;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000321 RID: 801
public class CommonCharacterListItem : CButtonObsolete
{
	// Token: 0x17000522 RID: 1314
	// (get) Token: 0x06002ED1 RID: 11985 RVA: 0x00171178 File Offset: 0x0016F378
	// (set) Token: 0x06002ED2 RID: 11986 RVA: 0x00171180 File Offset: 0x0016F380
	public int CharacterId
	{
		get
		{
			return this._characterId;
		}
		set
		{
			this._characterId = value;
			this.Refresh();
		}
	}

	// Token: 0x06002ED3 RID: 11987 RVA: 0x00171194 File Offset: 0x0016F394
	protected override void Awake()
	{
		bool flag = this.PointerTrigger == null;
		if (flag)
		{
			this.PointerTrigger = base.GetComponent<PointerTrigger>();
		}
		bool flag2 = this.PointerTrigger != null;
		if (flag2)
		{
			this.PointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.ChangeEnterState(true);
			});
			this.PointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.ChangeEnterState(false);
			});
		}
		bool flag3 = this.HSVStyleRoot == null;
		if (flag3)
		{
			this.HSVStyleRoot = base.GetComponent<HSVStyleRoot>();
		}
	}

	// Token: 0x06002ED4 RID: 11988 RVA: 0x00171228 File Offset: 0x0016F428
	private void ChangeEnterState(bool isEnter)
	{
		this._hovering = isEnter;
		this.RefreshButtonState();
	}

	// Token: 0x06002ED5 RID: 11989 RVA: 0x00171239 File Offset: 0x0016F439
	public void SetSelect(bool isSelect)
	{
		this._selected = isSelect;
		GameObject selected = this.Selected;
		if (selected != null)
		{
			selected.SetActive(isSelect);
		}
		this.RefreshButtonState();
	}

	// Token: 0x06002ED6 RID: 11990 RVA: 0x0017125D File Offset: 0x0016F45D
	public void Click()
	{
		Button.ButtonClickedEvent onClick = base.onClick;
		if (onClick != null)
		{
			onClick.Invoke();
		}
	}

	// Token: 0x06002ED7 RID: 11991 RVA: 0x00171271 File Offset: 0x0016F471
	protected override void OnInteractableChangeInternal(bool value)
	{
		base.OnInteractableChangeInternal(value);
		this.RefreshButtonState();
	}

	// Token: 0x06002ED8 RID: 11992 RVA: 0x00171284 File Offset: 0x0016F484
	private void RefreshButtonState()
	{
		bool flag = !base.interactable;
		if (flag)
		{
			this.HSVStyleRoot.SetSaturationFactor(0f);
			this.HSVStyleRoot.SetValueFactor(0.5f);
		}
		else
		{
			bool selected = this._selected;
			if (selected)
			{
				this.HSVStyleRoot.SetDefault();
			}
			else
			{
				this.HSVStyleRoot.SetSaturationFactor(1f);
				this.HSVStyleRoot.SetValueFactor(this._hovering ? 1f : 0.5f);
			}
		}
	}

	// Token: 0x06002ED9 RID: 11993 RVA: 0x0017130E File Offset: 0x0016F50E
	protected override void OnEnable()
	{
		base.OnEnable();
		this.RefreshButtonState();
	}

	// Token: 0x06002EDA RID: 11994 RVA: 0x0017131F File Offset: 0x0016F51F
	private new void OnDisable()
	{
		this.CharacterId = -1;
	}

	// Token: 0x06002EDB RID: 11995 RVA: 0x0017132C File Offset: 0x0016F52C
	public void Refresh()
	{
		bool flag = this._characterId < 0;
		if (flag)
		{
			this.Love.SetActive(false);
			this.Hate.SetActive(false);
			this.NameLabel.text = string.Empty;
			this.Avatar.ResetToBlank(false);
		}
		else
		{
			this.RefreshByGetCharacterDisplayData();
		}
	}

	// Token: 0x06002EDC RID: 11996 RVA: 0x0017138C File Offset: 0x0016F58C
	public void RefreshByGetCharacterDisplayData()
	{
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, this.CharacterId, delegate(int offset, RawDataPool pool)
		{
			CharacterDisplayData characterDisplayData = null;
			Serializer.Deserialize(pool, offset, ref characterDisplayData);
			this.RefreshByCharacterDisplayData(characterDisplayData);
		});
	}

	// Token: 0x06002EDD RID: 11997 RVA: 0x001713A8 File Offset: 0x0016F5A8
	public void RefreshByCharacterDisplayData(CharacterDisplayData characterDisplayData)
	{
		ushort relation = characterDisplayData.RelationFromTaiwu;
		bool flag = relation == ushort.MaxValue;
		if (flag)
		{
			this.Love.SetActive(false);
			this.Hate.SetActive(false);
		}
		else
		{
			this.Love.SetActive(RelationType.HasRelation(relation, 16384));
			this.Hate.SetActive(RelationType.HasRelation(relation, 32768));
		}
		this.Avatar.Refresh(characterDisplayData, true);
		this.NameLabel.text = NameCenter.GetMonasticTitleOrDisplayName(characterDisplayData, this._characterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
	}

	// Token: 0x040021FE RID: 8702
	public Game.Components.Avatar.Avatar Avatar;

	// Token: 0x040021FF RID: 8703
	public TextMeshProUGUI NameLabel;

	// Token: 0x04002200 RID: 8704
	public GameObject Love;

	// Token: 0x04002201 RID: 8705
	public GameObject Hate;

	// Token: 0x04002202 RID: 8706
	public HSVStyleRoot HSVStyleRoot;

	// Token: 0x04002203 RID: 8707
	public PointerTrigger PointerTrigger;

	// Token: 0x04002204 RID: 8708
	public GameObject Selected;

	// Token: 0x04002205 RID: 8709
	private int _characterId;

	// Token: 0x04002206 RID: 8710
	private bool _selected = false;

	// Token: 0x04002207 RID: 8711
	private bool _hovering = false;
}
