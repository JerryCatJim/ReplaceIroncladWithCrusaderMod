using System.Collections.Generic;

namespace ReplaceIroncladWithCrusader.Code.ModConfig;

public struct RitsuLibModConfigEntity
{
	public string modId { get; set; } = "ReplaceIroncladWithCrusader";

	public string modDisplayName { get; set; } = null;

	public int modSidebarOrder { get; set; } = 50;

	public List<RLMCPage> pages { get; set; } = new List<RLMCPage>();

	public RitsuLibModConfigEntity()
	{
	}
}
