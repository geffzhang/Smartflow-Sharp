using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Smartflow
{

    /// <summary>
    /// https://docs.microsoft.com/zh-cn/dotnet/framework/reflection-and-codedom/how-to-define-a-generic-type-with-reflection-emit
    /// https://docs.microsoft.com/zh-cn/dotnet/api/system.reflection.emit.typebuilder.definegenericparameters?view=netframework-4.6.2 List<T>
    /// </summary>
    public class TypeCreator
    {
        /// <summary>
        /// https://docs.microsoft.com/zh-cn/dotnet/api/system.reflection.emit.assemblybuilder?view=netframework-4.7.2
        /// </summary>
        /// <param name="className"></param>
        /// <param name="dicProperties"></param>
        /// <returns></returns>
        public static Type Creator(string className, Dictionary<string, Type> dicProperties)
        {
            AssemblyName assemblyName = new AssemblyName("DynamicAssemblyProxy");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder mb = assemblyBuilder.DefineDynamicModule(assemblyName.Name, false);
            TypeBuilder typeBuilder = mb.DefineType(className, TypeAttributes.Public);
            foreach (string propertyName in dicProperties.Keys)
            {
                PropertyCreator(typeBuilder, propertyName, dicProperties[propertyName]);
            }
            return typeBuilder.CreateType();
        }

        private static void PropertyCreator(TypeBuilder typeBuilder, string propertyName, Type declare)
        {
            FieldBuilder fb = typeBuilder.DefineField("m_" + propertyName, declare, FieldAttributes.Private);

            fb.SetConstant(TypeCreator.DefaultValue(declare));

            PropertyBuilder pb = typeBuilder.DefineProperty(propertyName, System.Reflection.PropertyAttributes.HasDefault, CallingConventions.Standard, declare, null);
            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            MethodBuilder mbGetAccessor = typeBuilder.DefineMethod("get_" + propertyName, getSetAttr, declare, Type.EmptyTypes);
            ILGenerator fieldGetIL = mbGetAccessor.GetILGenerator();

            fieldGetIL.Emit(OpCodes.Ldarg_0);
            fieldGetIL.Emit(OpCodes.Ldfld, fb);
            fieldGetIL.Emit(OpCodes.Ret);

            MethodBuilder mbSetAccessor = typeBuilder.DefineMethod("set_" + propertyName, getSetAttr, null, new Type[] { declare });
            ILGenerator fieldSetIL = mbSetAccessor.GetILGenerator();
            fieldSetIL.Emit(OpCodes.Ldarg_0);
            fieldSetIL.Emit(OpCodes.Ldarg_1);
            fieldSetIL.Emit(OpCodes.Stfld, fb);
            fieldSetIL.Emit(OpCodes.Ret);
            pb.SetGetMethod(mbGetAccessor);
            pb.SetSetMethod(mbSetAccessor);

        }

        private static object DefaultValue(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }
}
