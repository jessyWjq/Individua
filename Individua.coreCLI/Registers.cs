using Autofac;
using Individua.coreCLI.Comm;
using Individua.coreCLI.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Individua.coreCLI
{
    class Registers
    {
        public void Register()
        {
            var builder = new ContainerBuilder();
            //注册Samoyed指定为IDog实现
            builder.RegisterType<Samoyed>().As<IDog>();
            builder.RegisterType<TibetanMastiff>().As<IDog>();
            using (var container = builder.Build())
            {
                var dogs = container.Resolve<IEnumerable<IDog>>();
                foreach (var dog in dogs)
                {
                    Console.WriteLine($"名称：{dog.Name},品种：{dog.Breed}");
                }
            }
            TibetanMastiff d = new TibetanMastiff();
            builder.RegisterInstance(d).As<IDog>();
        }

        public void RegisterAssemblyTypes()
        {
            var builder = new ContainerBuilder();
            //注册程序集下所有类型
            builder.RegisterAssemblyTypes(typeof(Program).Assembly).AsImplementedInterfaces();
            using (var container = builder.Build())
            {
                var dogs = container.Resolve<IEnumerable<IDog>>();
                foreach (var dog in dogs)
                {
                    Console.WriteLine($"名称：{dog.Name},品种：{dog.Breed}");
                }
            }
            TibetanMastiff d = new TibetanMastiff();
            builder.RegisterInstance(d).As<IDog>();
        }
    }
}
