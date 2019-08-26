using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Naming;
using Feign;
using Feign.Cache;
using Feign.CastleWindsor;
using Feign.Discovery;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Castle.Windsor
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CastleWindsorExtensions
    {

        public static ICastleWindsorFeignBuilder AddFeignClients(this IWindsorContainer windsorContainer)
        {
            return AddFeignClients(windsorContainer, (FeignOptions)null);
        }

        public static ICastleWindsorFeignBuilder AddFeignClients(this IWindsorContainer windsorContainer, Action<IFeignOptions> setupAction)
        {
            FeignOptions options = new FeignOptions();
            setupAction?.Invoke(options);
            return AddFeignClients(windsorContainer, options);
        }

        public static ICastleWindsorFeignBuilder AddFeignClients(this IWindsorContainer windsorContainer, IFeignOptions options)
        {
            if (options == null)
            {
                options = new FeignOptions();
            }

            CastleWindsorFeignBuilder feignBuilder = new CastleWindsorFeignBuilder();

            feignBuilder.WindsorContainer = windsorContainer;
            feignBuilder.Options = options;
            feignBuilder.AddFeignClients(options);
            return feignBuilder;
        }


        internal static void RemoveComponent(this IKernel kernel, Type type)
        {
            INamingSubSystem namingSubSystem = (INamingSubSystem)kernel.GetType().GetProperty("NamingSubSystem", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(kernel);
            if (!namingSubSystem.Contains(type))
            {
                if (kernel.Parent == null)
                {
                    return;
                }
                RemoveComponent(kernel.Parent, type);
                return;
            }
            IHandler handler = kernel.GetHandler(type);
            if (handler.ComponentModel.Dependents.Length != 0)
            {
                // We can't remove this component as there are
                // others which depends on it
                return;
            }
            UnRegister(namingSubSystem, type);
            UnRegister(namingSubSystem, type.FullName);
            foreach (var service in handler.ComponentModel.Services)
            {
                var assignableHandlers = namingSubSystem.GetAssignableHandlers(service);
                if (assignableHandlers.Length > 0)
                {
                    IHandler assignableHandler = assignableHandlers[0];
                    UnRegister(namingSubSystem, assignableHandler.ComponentModel.Services.FirstOrDefault());
                    UnRegister(namingSubSystem, assignableHandler.ComponentModel.Name);
                }
                //if (assignableHandlers.Length > 0)
                //{
                //    //namingSubSystem[service] = assignableHandlers[0];
                //}
                //else
                //{
                // UnRegister(namingSubSystem, service);
                //}
            }
            foreach (ComponentModel model in handler.ComponentModel.Dependents)
            {
                RemoveDepender(model, handler.ComponentModel);
            }
            if (handler is IDisposable)
            {
                ((IDisposable)handler).Dispose();
            }
        }


        static void UnRegister(INamingSubSystem namingSubSystem, Type type)
        {
            object service2Handler = namingSubSystem.GetType().GetField("service2Handler", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(namingSubSystem);
            service2Handler.GetType().GetMethod("Remove", new Type[] { typeof(Type) }).Invoke(service2Handler, new object[] { type });
            //service2Handler.Remove(service);
            //InvalidateCache();
        }

        static void UnRegister(INamingSubSystem namingSubSystem, string key)
        {

            //if (key2Handler.TryGetValue(key, out value))
            //    allHandlers.Remove(value);
            //key2Handler.Remove(key);

            object name2Handler = namingSubSystem.GetType().GetField("name2Handler", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(namingSubSystem);
            name2Handler.GetType().GetMethod("Remove", new Type[] { typeof(string) }).Invoke(name2Handler, new object[] { key });

            //List<IHandler> allHandlers = namingSubSystem.GetType().GetField("allHandlers", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(namingSubSystem);
            //key2Handler.GetType().GetMethod("Remove", new Type[] { typeof(string) }).Invoke(key2Handler, new object[] { key });

            //service2Handler.Remove(service);
            //InvalidateCache();
        }


        static void RemoveDepender(GraphNode graphNode, GraphNode depender)
        {
            List<GraphNode> incoming = (List<GraphNode>)graphNode.GetType().GetField("Incoming", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(graphNode);
            incoming.Remove(depender);
            RemoveDependent(depender, graphNode);
        }

        static void RemoveDependent(GraphNode graphNode, GraphNode depender)
        {
            List<GraphNode> outgoing = (List<GraphNode>)graphNode.GetType().GetField("Outgoing", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(graphNode);
            outgoing.Remove(depender);
        }

        //private bool DisposeAndRemoveComponent(string key, bool ensureDisposed)
        //{
        //    if (key == null)
        //    {
        //        throw new ArgumentNullException("key");
        //    }
        //    if (!NamingSubSystem.Contains(key))
        //    {
        //        if (Parent == null)
        //        {
        //            return false;
        //        }

        //        return Parent.RemoveComponent(key);
        //    }
        //    var handler = GetHandler(key);
        //    if (handler.ComponentModel.Dependers.Length != 0)
        //    {
        //        // We can't remove this component as there are
        //        // others which depends on it
        //        if (ensureDisposed)
        //        {
        //            DisposeHandler(handler);
        //        }
        //        return false;
        //    }
        //    NamingSubSystem.UnRegister(key);
        //    var service = handler.ComponentModel.Service;
        //    var assignableHandlers = NamingSubSystem.GetAssignableHandlers(service);
        //    if (assignableHandlers.Length > 0)
        //    {
        //        NamingSubSystem[handler.ComponentModel.Service] = assignableHandlers[0];
        //    }
        //    else
        //    {
        //        NamingSubSystem.UnRegister(service);
        //    }
        //    foreach (ComponentModel model in handler.ComponentModel.Dependents)
        //    {
        //        model.RemoveDepender(handler.ComponentModel);
        //    }
        //    RaiseComponentUnregistered(key, handler);
        //    DisposeHandler(handler);
        //    return true;
        //}


    }
}
