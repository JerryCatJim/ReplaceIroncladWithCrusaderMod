using System.Collections.Generic;

namespace CrusaderSkin.Code.ModConfig;

public struct RitsuLibModConfigEntity
{
	public string modId { get; set; } = "CrusaderSkin";

	public string modDisplayName { get; set; } = null;

	public int modSidebarOrder { get; set; } = 50;

	public List<RLMCPage> pages { get; set; } = new List<RLMCPage>();

	public RitsuLibModConfigEntity()
	{
	}
}
