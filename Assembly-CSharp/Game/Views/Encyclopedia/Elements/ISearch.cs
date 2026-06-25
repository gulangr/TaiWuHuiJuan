using System;
using Game.Views.Encyclopedia.Utilities;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A88 RID: 2696
	public interface ISearch
	{
		// Token: 0x0600841C RID: 33820
		void RefreshSearchResultHighlight(OptimizedHtmlPatternMatcher matcher, bool onlyTitle = false);
	}
}
