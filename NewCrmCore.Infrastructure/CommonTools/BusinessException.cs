using System;
using System.Runtime.Serialization;

namespace NewCrmCore.Infrastructure.CommonTools
{
    [Serializable]
    public class BusinessException : Exception
    {
        public BusinessException() { }

        public BusinessException(String message)
            : base(message) { }

        public BusinessException(String message, Exception inner)
            : base(message, inner) { }

        protected BusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class EnvVariableGetFailException : Exception
    {
        public EnvVariableGetFailException() { }

        public EnvVariableGetFailException(String message)
            : base(message) { }

        public EnvVariableGetFailException(String message, Exception inner)
            : base(message, inner) { }

        protected EnvVariableGetFailException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

    }
}