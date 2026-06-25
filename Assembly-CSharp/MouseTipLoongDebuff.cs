using System;
using System.Collections.Generic;
using FrameWork;
using GameData.DLC.FiveLoong;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002B6 RID: 694
public class MouseTipLoongDebuff : MouseTipBase
{
	// Token: 0x170004A8 RID: 1192
	// (get) Token: 0x06002AB4 RID: 10932 RVA: 0x00147038 File Offset: 0x00145238
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170004A9 RID: 1193
	// (get) Token: 0x06002AB5 RID: 10933 RVA: 0x0014703C File Offset: 0x0014523C
	private PoolItem ItemPool
	{
		get
		{
			bool flag = this._itemPoolInstance == null;
			if (flag)
			{
				this._itemPoolInstance = new PoolItem("ItemView", this.SrcPrefab);
			}
			return this._itemPoolInstance;
		}
	}

	// Token: 0x06002AB6 RID: 10934 RVA: 0x0014707C File Offset: 0x0014527C
	protected override void Init(ArgumentBox argsBox)
	{
		this._loongInfos = new List<LoongInfo>();
		argsBox.Get<List<LoongInfo>>("loongInfos", out this._loongInfos);
		argsBox.Get("CharId", out this._currCharId);
		for (int i = 0; i < this._loongInfos.Count; i++)
		{
			int longElement = -1;
			this._loongTemplateIdMap.TryGetValue(this._loongInfos[i].CharacterTemplateId, out longElement);
			bool flag = longElement != -1;
			if (flag)
			{
				GameObject deBuffInfo = this.ItemPool.GetObject();
				deBuffInfo.transform.SetParent(base.transform, false);
				this._gameObjects.Add(deBuffInfo);
				this.SetDebuffText(deBuffInfo, this._loongInfos[i], longElement);
			}
		}
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
		});
	}

	// Token: 0x06002AB7 RID: 10935 RVA: 0x00147164 File Offset: 0x00145364
	protected override void OnDisable()
	{
		base.OnDisable();
		bool flag = this._gameObjects != null;
		if (flag)
		{
			for (int i = 0; i < this._gameObjects.Count; i++)
			{
				this.ItemPool.RemoveObject(this._gameObjects[i]);
			}
			this._gameObjects.Clear();
		}
	}

	// Token: 0x06002AB8 RID: 10936 RVA: 0x001471C8 File Offset: 0x001453C8
	private void SetDebuffText(GameObject debuffInfo, LoongInfo loongInfo, int longElement)
	{
		ushort debuffCount = loongInfo.GetCharacterDebuffCount(this._currCharId);
		Refers refers = debuffInfo.GetComponent<Refers>();
		refers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get("LK_loong_Debuff_Mousetip_Effect_Title_" + longElement.ToString()).ColorReplace();
		refers.CGet<TextMeshProUGUI>("Effect").text = LocalStringManager.GetFormat("LK_loong_Debuff_Mousetip_Effect_" + longElement.ToString(), debuffCount).ColorReplace();
		refers.CGet<TextMeshProUGUI>("EffectDesc").text = LocalStringManager.Get("LK_loong_Debuff_Mousetip_Effect_Desc_" + longElement.ToString()).ColorReplace();
	}

	// Token: 0x04001EE6 RID: 7910
	public GameObject SrcPrefab;

	// Token: 0x04001EE7 RID: 7911
	private int _charId;

	// Token: 0x04001EE8 RID: 7912
	private readonly Dictionary<short, int> _loongTemplateIdMap = new Dictionary<short, int>
	{
		{
			246,
			0
		},
		{
			247,
			1
		},
		{
			249,
			2
		},
		{
			248,
			3
		},
		{
			250,
			4
		}
	};

	// Token: 0x04001EE9 RID: 7913
	private PoolItem _itemPoolInstance = null;

	// Token: 0x04001EEA RID: 7914
	private const string ItemViewKey = "ItemView";

	// Token: 0x04001EEB RID: 7915
	private List<LoongInfo> _loongInfos;

	// Token: 0x04001EEC RID: 7916
	private int _currCharId;

	// Token: 0x04001EED RID: 7917
	private List<GameObject> _gameObjects = new List<GameObject>();
}
