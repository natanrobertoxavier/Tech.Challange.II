using System.Runtime.Serialization;

namespace Tech.Challenge.I.Exceptions.ExceptionBase;

[Serializable]
public class InvalidLoginException : TechChallengeException
{
    public InvalidLoginException() : base(ErrorsMessages.InvalidLogin)
    {
    }

    protected InvalidLoginException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
