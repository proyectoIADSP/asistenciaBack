using asistenciaBack.Membership.Application.Commands;
using asistenciaBack.Membership.Application.Interfaces;
using asistenciaBack.Membership.Infrastructure.Persistence;

namespace asistenciaBack.Membership.Presentation;

public static class MembershipDependencyInjection
{
    public static IServiceCollection AddMembershipModule(this IServiceCollection services)
    {
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<GetMembersCommand>();
        services.AddScoped<GetInactiveMembersCommand>();
        services.AddScoped<GetMemberByIdCommand>();
        services.AddScoped<CreateMemberCommand>();
        services.AddScoped<UpdateMemberCommand>();
        services.AddScoped<DeactivateMemberCommand>();
        services.AddScoped<ActivateMemberCommand>();

        return services;
    }
}
