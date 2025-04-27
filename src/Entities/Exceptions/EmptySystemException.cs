using System;

namespace APBD2
{
    public class EmptySystemException : Exception
    {
        public EmptySystemException() { }

        public EmptySystemException(string message)
            : base(message) { }

        public EmptySystemException(string message, Exception inner)
            : base(message, inner) { }
    }
}