using System.Collections.Generic;

namespace CrusaderSkin.Code.ModConfig;

public struct RLMCSection
{
	public string id { get; set; } = null;

	public string title { get; set; } = null;

	public List<object> entries { get; set; } = new List<object>();

	public RLMCSection()
	{
	}
}
