using System;
using Microsoft.Extensions.DependencyInjection;
using SocialiteNET.Abstractions;
using SocialiteNET.Providers.Google;

namespace SocialiteNET;

/// <summary>
/// Extensions for adding providers
/// </summary>
public static class ProviderExtensions
{
    /// <summary>
    /// Adds the Google provider
    /// </summary>
    /// <param name="builder">Socialite builder</param>
    /// <param name="configureOptions">Options configuration</param>
    /// <returns>Socialite builder</returns>
    /// <exception cref="ArgumentNullException">Thrown when parameters are null</exception>
    public static ISocialiteBuilder AddGoogle(
        this ISocialiteBuilder builder,
        Action<GoogleConfig> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);

        ArgumentNullException.ThrowIfNull(configureOptions);

        GoogleConfig options = new();
        configureOptions(options);

        builder.Services.Configure<GoogleConfig>(opt =>
        {
            opt.ClientId = options.ClientId;
            opt.ClientSecret = options.ClientSecret;
            opt.RedirectUrl = options.RedirectUrl;
            opt.Stateless = options.Stateless;
            opt.UsesPkce = options.UsesPkce;
            opt.ScopeSeparator = options.ScopeSeparator;

            foreach (string scope in options.Scopes)
            {
                if (!string.IsNullOrEmpty(scope) && !opt.Scopes.Contains(scope))
                {
                    opt.Scopes.Add(scope);
                }
            }

            foreach ((string key, string value) in options.Parameters)
            {
                opt.Parameters[key] = value;
            }
        });

        builder.Services.AddHttpClient<GoogleProvider>();
        builder.Services.AddTransient<GoogleProvider>();
        builder.Services.AddTransient<IProvider>(sp => sp.GetRequiredService<GoogleProvider>());

        builder.DriverRegistrations.Add(manager =>
            manager.RegisterDriver<GoogleProvider>("google"));

        return builder;
    }
}