using System;

namespace PinPayments.Extensions
{
    public static class ObjectExtensions
    {
        public static void CheckArgumentNull(this object o, string name)
        {
            if (o == null)
                throw new ArgumentNullException(name);
        }
    }
}