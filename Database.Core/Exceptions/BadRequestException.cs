using System;

namespace Database.Core.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException() { }

        public BadRequestException(string exception) : base(exception) { }
    }
}