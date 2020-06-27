using System;
namespace CamundaClient.Common.Exceptions
{
    public class EngineException : Exception
    {
        public EngineException(string message) : base(message)
        {
        }

        public EngineException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
