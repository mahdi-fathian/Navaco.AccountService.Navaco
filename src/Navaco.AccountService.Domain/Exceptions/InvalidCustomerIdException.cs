namespace Navaco.AccountService.Domain.Exceptions;

public class InvalidCustomerIdException : DomainException
{
    public InvalidCustomerIdException()
        : base("شناسه مشتری نامعتبر است.") { }
}
