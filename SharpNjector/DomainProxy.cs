using SharpNjector.Exceptions;
using System;
using System.Linq;
using System.Reflection;

namespace SharpNjector
{
    public class DomainProxy : MarshalByRefObject
    {
        public DomainProxy()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly; //TODO: Find out if unsubscription must take place.
        }

        public string LoadAssembly(byte[] assemblyBytes)
        {
            var assembly = Assembly.Load(assemblyBytes);

            return assembly.FullName;
        }

        public string LoadAssembly(string assemblyPath)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);

            return assembly.FullName;
        }

        public object CreateObject(string assemblyName, string typeName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyName);

            if (assembly == null)
                throw new AssemblyNotLoadedException();

            var type = assembly.GetType(typeName);

            var obj = Activator.CreateInstance(type);

            return obj;
        }

        public TOutput CreateObject<TOutput>(string assemblyName, string typeName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyName);

            if (assembly == null)
                throw new AssemblyNotLoadedException();

            var type = assembly.GetType(typeName);

            var obj = (TOutput)Activator.CreateInstance(type);

            return obj;
        }

        //public void Dispose()
        //{
        //    AppDomain.CurrentDomain.AssemblyResolve -= ResolveAssembly;
        //}

        private Assembly ResolveAssembly(object sender, ResolveEventArgs e)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            return loadedAssemblies.FirstOrDefault(a => a.FullName == e.Name);
        }
    }
}
