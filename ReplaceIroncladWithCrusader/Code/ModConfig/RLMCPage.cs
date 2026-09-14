using System.Collections.Generic;

namespace ReplaceIroncladWithCrusader.Code.ModConfig;

public struct RLMCPage
{
	public string pageId { get; set; } = null;

	public string title { get; set; } = null;

	public string description { get; set; } = null;

	public int sortOrder { get; set; } = 50;

	public List<RLMCSection> sections { get; set; } = new List<RLMCSection>();

	public RLMCPage()
	{
	}
}
