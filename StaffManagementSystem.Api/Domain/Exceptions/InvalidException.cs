using Xeptions;

namespace StaffManagementSystem.Api.Domain.Exceptions
{
    public class InvalidException : Xeption
    {
        public InvalidException(string message)
            : base(message)
        { }
    }
}
