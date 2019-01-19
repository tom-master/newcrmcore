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
        public GetEnvVariableFailException() { }

        public GetEnvVariableFailException(String message)
            : base(message) { }

        public GetEnvVariableFailException(String message, Exception inner)
            : base(message, inner) { }

        protected GetEnvVariableFailException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

    }
}