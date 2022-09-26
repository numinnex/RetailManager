﻿using AutoMapper;
using Caliburn.Micro;
using RMDesktopUI.Helpers;
using RMDesktopUI.Library.API;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;
using RMDesktopUI.Models;
using RMDesktopUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RMDesktopUI
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");

        }
        // **********DEPENDENCY INJECTION SETUP*********

        private IMapper ConfigureAutoMapper()
        {

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductModel, ProductDisplayModel>();
                cfg.CreateMap<CartItemModel, CartItemDisplayModel>();

            });

            var output = config.CreateMapper();
            return output;
        }

        protected override void Configure()
        {

            var mapper = ConfigureAutoMapper();

            _container.Instance(mapper);

            _container.Instance(_container)
                .PerRequest<IProductEndPoint , ProductEndPoint>()
                .PerRequest<IUserEndPoint, UserEndPoint>()
                .PerRequest<ISaleEndPoint, SaleEndPoint>();

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<ILoggedInUserModel , LoggedInUserModel>()
                .Singleton<IConfigHelper , ConfigHelper>()
                .Singleton<IAPIHelper, APIHelper>();
                

            GetType().Assembly.GetTypes().Where(type => type.IsClass).Where(type => type.Name.EndsWith("ViewModel")).ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(viewModelType, viewModelType.ToString(), viewModelType));
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewForAsync<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
        //***********************************************

    }
}

