namespace PVRPCloud;

public sealed class DomainValidationException(List<Result> errors) : Exception
{
    public List<Result> Errors { get; } = errors;
}
