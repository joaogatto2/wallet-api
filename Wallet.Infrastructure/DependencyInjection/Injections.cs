using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallet.Core.Repositories;
using Wallet.Infrastructure.Context;
using Wallet.Infrastructure.Repositories;

namespace Wallet.Infrastructure.DependencyInjection;

public static class Injections
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WalletDbContext>();
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
    }
}