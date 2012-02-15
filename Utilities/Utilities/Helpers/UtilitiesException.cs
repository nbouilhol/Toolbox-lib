using System;

namespace Utilities
{
    [Serializable]
    public class UtilitiesException : Exception
    {
        public UtilitiesException()
            : base()
        {
        }

        public UtilitiesException(string message)
            : base(message)
        {
        }
    }
}
