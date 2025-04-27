using System;

namespace APBD2
{
    public class EmptyBatteryException : Exception
    {
        public EmptyBatteryException() { }

        public EmptyBatteryException(string message)
            : base(message) { }

        public EmptyBatteryException(string message, Exception inner)
            : base(message, inner) { }
    }
}