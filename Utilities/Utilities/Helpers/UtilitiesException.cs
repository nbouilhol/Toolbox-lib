using System;

namespace Utilities
{
    [Serializable]
    public class UtilitiesException : Exception
    {
        public UtilitiesException()
        {
        }

        public UtilitiesException(string message)
            : base(message)
        {
        }
    }
}