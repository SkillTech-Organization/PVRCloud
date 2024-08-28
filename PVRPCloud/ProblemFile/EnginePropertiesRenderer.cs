namespace PVRPCloud.ProblemFile;

public sealed class EnginePropertiesRenderer
{
    public string Render(string projectName)
    {
        return $"""
        setProblemFile("{projectName}")
        setEngineParameters(0, 1, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0)
        runEngine()
        """;
    }
}
