using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Feign.Reflection
{
#if DEBUG&&NET45
    public
#endif
     class DynamicAssembly
    {
        AssemblyBuilder _assemblyBuilder;
        ModuleBuilder _moduleBuilder;
        string _guid = Guid.NewGuid().ToString("N").ToUpper();

#if DEBUG&&NET45
        public bool DEBUG_MODE = false;
        public string AssemblyName = "Feign.Debug.dll";
#endif

        public AssemblyBuilder AssemblyBuilder
        {
            get
            {
                EnsureAssemblyBuilder();
                return _assemblyBuilder;
            }
        }
        public ModuleBuilder ModuleBuilder
        {
            get
            {
                EnsureModuleBuilder();
                return _moduleBuilder;
            }
        }
        void EnsureAssemblyBuilder()
        {
            if (_assemblyBuilder == null)
            {
#if DEBUG&&NET45
                if (DEBUG_MODE)
                {
                    _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(_guid), AssemblyBuilderAccess.RunAndSave);
                }
                else
                {
                    _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(_guid), AssemblyBuilderAccess.Run);
                }
#else
                _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(_guid), AssemblyBuilderAccess.Run);
#endif
            }
        }
        void EnsureModuleBuilder()
        {
            EnsureAssemblyBuilder();
            if (_moduleBuilder == null)
            {
#if DEBUG&&NET45
                if (DEBUG_MODE)
                {
                    _moduleBuilder = _assemblyBuilder.DefineDynamicModule("MainModule", AssemblyName);
                }
                else
                {
                    _moduleBuilder = _assemblyBuilder.DefineDynamicModule("MainModule");
                }
#else
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule("MainModule");
#endif
            }
        }

    }
}
