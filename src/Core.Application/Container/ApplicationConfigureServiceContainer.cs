using Core.Application.Contracts.Interfaces;
using Core.Application.Repositories;
using Core.Application.Repositories.Context;
using Core.Application.Services;
using Autofac;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Container;

public class ApplicationConfigureServiceContainer : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        AddApplicationServices(builder);
        AddRepositories(builder);
        AddDatabase(builder);
        AddAutoMapper(builder);
    }

    private void AddApplicationServices(ContainerBuilder builder)
    {
        builder.RegisterType<AccountService>()
            .As<IAccountService>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<UserService>()
            .As<IUserService>()
            .InstancePerLifetimeScope();
    }
    
    private void AddRepositories(ContainerBuilder builder)
    {
        builder.RegisterType<AccountRepository>()
            .As<IAccountRepository>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<UserRepository>()
            .As<IUserRepository>()
            .InstancePerLifetimeScope();
    }

    private void AddDatabase(ContainerBuilder builder)
    {
        var options = new DbContextOptionsBuilder<BankContext>().UseInMemoryDatabase("test").Options;
        
        //var options = new DbContextOptionsBuilder<BlogContext>().UseSqlServer(_configuration.GetConnectionString("BlogContext")).Options;
        //builder.Register(b => new BlogContextFactory(options)).SingleInstance();

        builder
            .RegisterType<BankContext>()
            .WithParameter("options", options)
            .SingleInstance();
    }
    
    private static void AddAutoMapper(ContainerBuilder builder)
    {
        builder.Register(context => new MapperConfiguration(cfg =>
        {
            cfg.AddMaps("Core.Application");
        })).AsSelf().SingleInstance();

        builder.Register(c =>
            {
                //This resolves a new context that can be used later.
                var context = c.Resolve<IComponentContext>();
                var config = context.Resolve<MapperConfiguration>();
                return config.CreateMapper(context.Resolve);
            })
            .As<IMapper>()
            .InstancePerLifetimeScope();

    }
}
