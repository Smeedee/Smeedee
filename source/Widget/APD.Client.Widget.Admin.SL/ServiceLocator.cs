using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using APD.DomainModel.Framework.Services;
using APD.Client.Widget.Admin.SL.Services;

namespace APD.Client.Widget.Admin.SL
{
    public class ServiceLocator
    {
        private static ServiceLocator instance = new ServiceLocator();

        private Dictionary<Type, Type> serviceMappings; 

        public static ServiceLocator Instance
        {
            get { return instance;  }
        }

        static ServiceLocator()
        {
       
        }

        private ServiceLocator()
        {
            serviceMappings = new Dictionary<Type, Type>();
            serviceMappings.Add(typeof(ICheckIfResourceExists), typeof(URLCheckerProxy));
            serviceMappings.Add(typeof(ICheckIfCredentialsIsValid), typeof(VCSCredentialsCheckerProxy));
        }

        public TService GetInstance<TService>()
        {
            if (!serviceMappings.ContainsKey(typeof(TService)))
                throw new Exception("No services of that type");

            return (TService) Activator.CreateInstance(serviceMappings[typeof(TService)]);
        }

        public void RegisterType(Type from, Type to)
        {
            if (!serviceMappings.ContainsKey(from))
                serviceMappings.Add(from, null);

            serviceMappings[from] = to;
        }
    }
}
