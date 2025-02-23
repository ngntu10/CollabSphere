namespace CollabSphere.Exceptions;

[Serializable]
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}
