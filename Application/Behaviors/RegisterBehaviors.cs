﻿using Application.Authentication;
using Application.Authentication.Login;
using Application.Authentication.Logout;
using Application.Authentication.Refresh;
using Application.Authentication.Register;
using Application.Users.ChangePassword;
using Application.Users.ChangeRole;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Behaviors;

internal static class RegisterBehaviors
{
    /// <summary>
    /// Adds all types of behaviors
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    internal static MediatRServiceConfiguration AddPipelineBehaviors(this MediatRServiceConfiguration config)
    {
        return config.AddValidationBehaviors();
    }
    
    /// <summary>
    /// Adds all validation behaviors.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    private static MediatRServiceConfiguration AddValidationBehaviors(this MediatRServiceConfiguration config)
    {
        // add validation pipeline here here
        return config
            .AddValidationBehavior<RegisterCommand, bool>()
            .AddValidationBehavior<LoginCommand, UserResponse>()
            .AddValidationBehavior<RefreshCommand, TokenResponse>()
            .AddValidationBehavior<LogoutCommand, bool>()
            .AddValidationBehavior<ChangeRoleCommand, bool>()
            .AddValidationBehavior<ChangePasswordCommand, bool>();
    }
    
    /// <summary>
    /// Adds specific validation behavior
    /// </summary>
    /// <param name="config"></param>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    private static MediatRServiceConfiguration AddValidationBehavior<TRequest, TResponse>(
        this MediatRServiceConfiguration config)
        where TRequest : IRequest<Result<TResponse>>
    {
        return config
            .AddBehavior<IPipelineBehavior<TRequest, Result<TResponse>>,
                ValidationPipelineBehavior<TRequest, TResponse>>();
    }
}