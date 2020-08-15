using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Domainr.EventStore.Mongo.Conventions
{
    internal sealed class MapReadOnlyPropertiesConvention
        : ConventionBase, IClassMapConvention
    {
        private readonly BindingFlags _bindingFlags;

        public MapReadOnlyPropertiesConvention()
            : this(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
        }

        public MapReadOnlyPropertiesConvention(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
        }

        public void Apply(BsonClassMap classMap)
        {
            var readOnlyProperties = classMap
                .ClassType
                .GetTypeInfo()
                .GetProperties(_bindingFlags)
                .Where(propInfo => IsReadOnlyProperty(classMap, propInfo))
                .ToList();

            foreach (var property in readOnlyProperties)
            {
                classMap.MapMember(property);
            }

            if (classMap.ClassType.Name.Equals("Event"))
            {
                classMap.MapField("_version");
                classMap.MapProperty("Version");
            }
        }

        private static bool IsReadOnlyProperty(BsonClassMap classMap, PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead)
            {
                return false;
            }

            if (propertyInfo.CanWrite)
            {
                return false; // already handled by default convention
            }

            if (propertyInfo.GetIndexParameters().Length != 0)
            {
                return false; // skip indexers
            }

            if (propertyInfo.Name.Equals("Version"))
            {
                return false;
            }

            var getMethodInfo = propertyInfo.GetMethod;

            // skip overridden properties (they are already included by the base class)
            if (getMethodInfo.IsVirtual && getMethodInfo.GetBaseDefinition().DeclaringType != classMap.ClassType)
            {
                return false;
            }

            return true;
        }
    }
}