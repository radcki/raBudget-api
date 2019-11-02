using System;

namespace raBudget.Core.Exceptions
{
    public class SaveFailureException : Exception
    {
        public SaveFailureException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not saved.")
        {
        }
    }
}