using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Features.Scanning;
using AutoMapper;

namespace App.Common.Infrastructure
{
    public static class ContainerBuilderConventionsExtensions
    {
        public static ContainerBuilder RegisterWithConventions(
            this ContainerBuilder builder,
            Assembly assembly,
            Action<IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>>
                lifetimeScope = null)
        {
            var setLifetimeScope = lifetimeScope ?? (o => o.InstancePerLifetimeScope());

            // all AutoMapper profiles as singletons
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.IsSubclassOf(typeof(Profile)))
                .As<Profile>()
                .SingleInstance();

            // all top level classes without interfaces excluding mocks/stubs
            setLifetimeScope(
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => !t.GetInterfaces().Any())
                    .Where(t => !t.IsNested)
                    .Where(t => !t.Name.EndsWith("Mock"))
                    .Where(t => !t.Name.EndsWith("Stub"))
                    .AsSelf());

            // all top level classes with interfaces excluding mocks/stubs
            setLifetimeScope(
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => !t.IsNested)
                    .Where(t => !t.Name.EndsWith("Mock"))
                    .Where(t => !t.Name.EndsWith("Stub"))
                    .Where(t => !t.IsSubclassOf(typeof(Profile)))
                    .AsImplementedInterfaces());

            return builder;
        }
    }
}