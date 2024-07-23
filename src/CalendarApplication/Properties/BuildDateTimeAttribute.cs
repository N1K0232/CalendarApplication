using System.Globalization;

namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly)]
public class BuildDateTimeAttribute(string value) : Attribute
{
    public DateTime DateTime => DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
}