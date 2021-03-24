using System;

namespace SetupReference.Database.Exceptions
{
    public class InvalidTenantIdException : ApplicationException
    {
        public InvalidTenantIdException(string message) : base(message) { }
    }
}
