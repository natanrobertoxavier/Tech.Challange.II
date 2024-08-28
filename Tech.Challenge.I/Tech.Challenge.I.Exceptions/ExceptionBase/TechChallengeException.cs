using System.Runtime.Serialization;

namespace Tech.Challenge.I.Exceptions.ExceptionBase;
public class TechChallengeException : SystemException
{
    public TechChallengeException(string mensagem) : base(mensagem)
    {
    }

    protected TechChallengeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}