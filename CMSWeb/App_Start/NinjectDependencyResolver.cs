using CMSRepository;
using CMSRepository.Implementation;
using CMSService;
using CMSService.Implementation;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CMSWeb.App_Start
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind<CMSEntities>().ToSelf().InRequestScope();
            kernel.Bind<IUserRepository>().To<UserRepository>();
            kernel.Bind<IUserService>().To<UserService>();

            kernel.Bind<IEmployeeRepository>().To<EmployeeRepository>();
            kernel.Bind<IEmployeeService>().To<EmployeeService>();
        }
    }
}