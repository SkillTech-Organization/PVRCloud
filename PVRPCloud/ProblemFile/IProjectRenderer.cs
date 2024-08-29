using PVRPCloud.Requests;

namespace PVRPCloud.ProblemFile;

public interface IProjectRenderer
{
    string Render(Project project);
}
