namespace ReplaceIroncladWithCrusader.Code.ModConfig;
public struct SliderEntry
{
	public string id { get; set; } = null;

	public string type { get; } = "slider";

	public string key { get; set; } = null;

	public string label { get; set; } = null;

	public string description { get; set; } = null;

	public double min { get; set; } = 0.0;

	public double max { get; set; } = 0.0;

	public double step { get; set; } = 0.0;

	public RLMCScope scope { get; set; } = RLMCScope.global;

	public SliderEntry()
	{
	}
}
