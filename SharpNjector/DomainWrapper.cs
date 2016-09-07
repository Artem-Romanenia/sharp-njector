using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpNjector
{
    public class DomainWrapper : IDisposable
    {
        private AppDomain _domain;
        private DomainProxy _domainProxy;

        public DomainWrapper()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            var setup = new AppDomainSetup()
            {
                ApplicationBase = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location)
            };

            _domain = AppDomain.CreateDomain("SharnNjector", null, setup);

            var domainProxyType = typeof(DomainProxy);
            _domainProxy = (DomainProxy)_domain.CreateInstanceAndUnwrap(domainProxyType.Assembly.FullName, domainProxyType.FullName);
        }

        public string LoadAssembly(byte[] assemblyBytes)
        {
            return _domainProxy.LoadAssembly(assemblyBytes);
        }

        public string LoadAssembly(string assemblyPath)
        {
            return _domainProxy.LoadAssembly(assemblyPath);
        }

        public object CreateObject(string assemblyName, string typeName)
        {
            return _domainProxy.CreateObject(assemblyName, typeName);
        }

        public TOutput CreateObject<TOutput>(string assemblyName, string typeName)
        {
            return _domainProxy.CreateObject<TOutput>(assemblyName, typeName);
        }

        private Assembly ResolveAssembly(object sender, ResolveEventArgs e)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            return loadedAssemblies.FirstOrDefault(a => a.FullName == e.Name);
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= ResolveAssembly;
            AppDomain.Unload(_domain);
        }
    }
}
