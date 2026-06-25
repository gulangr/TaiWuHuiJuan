using System;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02001001 RID: 4097
	internal interface ITweenValue
	{
		// Token: 0x0600BAD6 RID: 47830
		void TweenValue(float floatPercentage);

		// Token: 0x17001505 RID: 5381
		// (get) Token: 0x0600BAD7 RID: 47831
		bool ignoreTimeScale { get; }

		// Token: 0x17001506 RID: 5382
		// (get) Token: 0x0600BAD8 RID: 47832
		float duration { get; }

		// Token: 0x0600BAD9 RID: 47833
		bool ValidTarget();
	}
}
