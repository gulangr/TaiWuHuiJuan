using System;
using Config;
using FrameWork;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000272 RID: 626
public class MouseTipAttachedPoison : MouseTipBase
{
	// Token: 0x0600290D RID: 10509 RVA: 0x00131360 File Offset: 0x0012F560
	protected override void Init(ArgumentBox argsBox)
	{
		PoisonSlot poisonSlot;
		argsBox.Get<PoisonSlot>("PoisonSlot", out poisonSlot);
		MedicineItem medicineConfig = Medicine.Instance[poisonSlot.MedicineTemplateId];
		PoisonItem poisonConfig = Poison.Instance[medicineConfig.PoisonType];
		Refers gradeLayout = base.CGet<Refers>("GradeLayout");
		GameObject gradeObj = gradeLayout.CGet<GameObject>("GradeBack");
		gradeObj.GetComponent<CImage>().SetSprite(ItemView.GetGradeIcon(medicineConfig.Grade), false, null);
		gradeObj.GetComponentInChildren<TextMeshProUGUI>().SetText(ItemView.GetGradeText(medicineConfig.Grade), true);
		string gradeContent = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - medicineConfig.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)medicineConfig.Grade]);
		gradeLayout.CGet<TextMeshProUGUI>("Content").SetText(gradeContent, true);
		Refers typeLayout = base.CGet<Refers>("TypeLayout");
		typeLayout.CGet<CImage>("Icon").SetSprite(poisonConfig.TipsIcon, false, null);
		string typeContent = poisonConfig.Name.SetColor(poisonConfig.FontColor);
		typeLayout.CGet<TextMeshProUGUI>("Content").SetText(typeContent, true);
		Refers levelLayout = base.CGet<Refers>("LevelLayout");
		levelLayout.CGet<CImage>("Icon").SetSprite(MouseTipBase.GetPoisonLevelIcon((sbyte)medicineConfig.EffectThresholdValue), false, null);
		string levelName = LocalStringManager.Get(string.Format("LK_Poison_Level_{0}", medicineConfig.EffectThresholdValue));
		short value = poisonSlot.GetPoisonValue();
		string levelContent = LocalStringManager.GetFormat(LanguageKey.LK_Make_AttachedPoison_Level_Tip_Content, value, levelName);
		levelLayout.CGet<TextMeshProUGUI>("Content").SetText(levelContent, true);
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
		});
	}
}
