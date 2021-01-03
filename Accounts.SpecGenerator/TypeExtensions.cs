using System;

namespace Accounts.SpecGenerator
{
    public static class TypeExtensions
    {
        public static bool IsSubclassOfRawGeneric(this Type type, Type baseType)
        {
            while (type != typeof(object) && type.BaseType is not null)
            {
                var typeDefinition = type.IsGenericType 
                    ? type.GetGenericTypeDefinition() 
                    : type;

                if (typeDefinition == baseType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }
    }
}