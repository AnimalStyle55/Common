using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Microsoft.Practices.Unity;

namespace LD.Common.WebApi.Dependencies
{
    /// <summary>
    /// A dependency resolver for injecting dependencies into WebApi2 controllers
    /// Add this to WebApiConfig to enable it
    /// <code>config.DependencyResolver = new UnityResolver(container);</code>
    /// 
    /// Recommendation is to inject the container into controllers and resolve 
    /// other dependencies via that container
    /// </summary>
    public class UnityResolver : IDependencyResolver
    {
        private readonly IUnityContainer Container;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container"></param>
        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            Container = container;
        }

        /// <summary>
        /// Get Service
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            try
            {
                return Container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get Services
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return Container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        /// <summary>
        /// Begin Scope
        /// </summary>
        /// <returns></returns>
        public IDependencyScope BeginScope()
        {
            var child = Container.CreateChildContainer();
            return new UnityResolver(child);
        }

        /// <summary>
        /// Dispose Container
        /// </summary>
        public void Dispose()
        {
            Container.Dispose();
        }
    }
}