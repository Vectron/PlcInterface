using System;
using System.Dynamic;

namespace PlcInterface.Ads
{
    public interface IDynamicValueConverter
    {
        object ConvertFrom(DynamicObject dynamicObject, Type type);
    }
}