using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util
{
    public static class ClassHelper
    {
        public static object GetParentStaticField(object instance, string fieldName)
        {

            Type type = instance.GetType();
            var parentType = type.BaseType;
            // 从父类中获取静态属性
            //PropertyInfo staticProp = parentType.GetProperty("StaticProperty", BindingFlags.Public | BindingFlags.Static);
            FieldInfo fieldInfo = parentType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
            if (fieldInfo != null)
            {
                var value = fieldInfo.GetValue(null); // 对于静态字段，传递null作为对象
                return value;   
            }
            return null;
        }
    }
}
