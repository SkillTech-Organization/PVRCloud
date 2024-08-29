using System.Text;

namespace PVRPCloud.ProblemFile;

public sealed class EnginePropertiesRenderer
{
    public StringBuilder Render(string projectName)
    {
        StringBuilder sb = new();

        sb.AppendLine($"""setProblemFile("{projectName}")""");
        sb.AppendLine("setEngineParameters(0, 1, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0)");
        sb.AppendLine("runEngine()");

        return sb;
    }
}
