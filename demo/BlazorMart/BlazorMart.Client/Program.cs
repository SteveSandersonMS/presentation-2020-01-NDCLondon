using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorMart.Client.State;
using BlazorMart.Server;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorMart.Client
{
    public class Program
    {
        public const string BackendUrl = "https://localhost:5001";

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(services =>
            {
                // Create a gRPC-Web channel pointing to the backend server
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
                var channel = GrpcChannel.ForAddress(new Uri(BackendUrl), new GrpcChannelOptions { HttpClient = httpClient });

                // Now we can instantiate gRPC clients for this channel
                return new Inventory.InventoryClient(channel);
            });

            builder.Services.AddScoped<Cart>();

            await builder.Build().RunAsync();
        }
    }
}
