using System;
using Config;
using FrameWork;
using Game.Views.MusicPlayer;
using TMPro;
using UnityEngine;

// Token: 0x020002C2 RID: 706
public class MouseTipMusic : MouseTipBase
{
	// Token: 0x170004B0 RID: 1200
	// (get) Token: 0x06002AE7 RID: 10983 RVA: 0x0014A86A File Offset: 0x00148A6A
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002AE8 RID: 10984 RVA: 0x0014A870 File Offset: 0x00148A70
	protected override void Init(ArgumentBox argsBox)
	{
		base.CGet<CanvasGroup>("CanvasGroup").alpha = 0f;
		short musicId;
		argsBox.Get("MusicId", out musicId);
		bool flag = musicId < 0;
		if (!flag)
		{
			Config.MusicItem musicConfig = Music.Instance[musicId];
			string title = ViewMusicPlayer.GetMusicFullName(musicId);
			base.CGet<TextMeshProUGUI>("Title").text = title;
			base.CGet<TextMeshProUGUI>("Desc").text = musicConfig.Desc;
			bool showEvaluation = SingletonObject.getInstance<MusicPlayerModel>().EvaluatedMusicList.Contains(musicId);
			base.CGet<GameObject>("EvaluationLayout").SetActive(showEvaluation);
			base.CGet<TextMeshProUGUI>("Evaluation").text = musicConfig.Evaluation;
			RectTransform addPropertyHolder = base.CGet<RectTransform>("AddPropertyHolder");
			TipsAddProperty addPropertyPrefab = base.CGet<TipsAddProperty>("AddProperty");
			int index = 0;
			index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 9, (int)musicConfig.HitRateMind, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(addPropertyHolder, addPropertyPrefab, index, 15, musicConfig.AvoidRateMind, false, false, false, true, false, false);
			for (int i = index; i < addPropertyHolder.childCount; i++)
			{
				addPropertyHolder.GetChild(i).gameObject.SetActive(false);
			}
			string tempEffect = ViewMusicPlayer.GetEffectDesc(musicId, true);
			base.CGet<TextMeshProUGUI>("TempEffect").text = tempEffect;
			base.CGet<TMPTextSpriteHelper>("SpriteHelper").OnParseComplete = new Action(this.OnParseComplete);
			base.CGet<TMPTextSpriteHelper>("SpriteHelper").Parse();
			bool showState = !SingletonObject.getInstance<MusicPlayerModel>().Interactable;
			base.CGet<GameObject>("StateLayout").gameObject.SetActive(showState);
		}
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x0014AA22 File Offset: 0x00148C22
	private void OnParseComplete()
	{
		base.CGet<CanvasGroup>("CanvasGroup").alpha = 1f;
	}
}
