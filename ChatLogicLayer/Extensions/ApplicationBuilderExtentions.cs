using ChatLogicLayer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ChatLogicLayer.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        private static BotResponseConsumerRMQ _listener { get; set; }

        public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
        {
            _listener = app.ApplicationServices.GetService<BotResponseConsumerRMQ>();

            var lifetime = app.ApplicationServices.GetService<IApplicationLifetime>();

            lifetime.ApplicationStarted.Register(OnStarted);

            lifetime.ApplicationStopping.Register(OnStopping);

            return app;
        }

        private static void OnStarted()
        {
            _listener.Register();
        }

        private static void OnStopping()
        {
            _listener.Deregister();
        }
    }
}
