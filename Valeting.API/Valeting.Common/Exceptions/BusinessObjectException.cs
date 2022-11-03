namespace Valeting.Common.Exceptions
{
    public class BusinessObjectException : Exception
    {
        public BusinessObjectException(string errorMessage) : base(errorMessage) { }
    }
}

