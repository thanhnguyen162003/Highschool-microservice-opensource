namespace TransactionService.Domain.Organization;

public static class OrganizationCmdExtensions
{
    public static OrganizationRegistered ToEvent(this RegisterOrganization cmd)
    {
        return new OrganizationRegistered()
        {
            Id = cmd.Id,
            AuditInfo = cmd.AuditInfo,
            Address = cmd.Address,
            Name = cmd.Name
        };
    }

    public static bool IsIdempotent(this RegisterOrganization cmd, OrganizationAggregateState state)
    {
        return
            cmd.Address == state.Address &&
            cmd.Name == state.Name;
    }

    public static OrganizationNameCorrected ToEvent(this CorrectOrganizationName cmd)
    {
        return new OrganizationNameCorrected()
        {
            Id = cmd.Id,
            AuditInfo = cmd.AuditInfo,
            Name = cmd.Name
        };
    }

    public static bool IsIdempotent(this CorrectOrganizationName cmd, OrganizationAggregateState state)
    {
        return cmd.Name == state.Name;
    }
}