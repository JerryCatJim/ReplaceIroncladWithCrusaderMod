namespace ReplaceIroncladWithCrusader.Code.ModConfig;

public struct ToggleEntry
{
	public string id { get; set; } = null;

	public string type { get; } = "toggle";

	public string key { get; set; } = null;

	public string label { get; set; } = null;

	public string description { get; set; } = null;

	public RLMCScope scope { get; set; } = RLMCScope.global;

	public ToggleEntry()
	{
	}
}
