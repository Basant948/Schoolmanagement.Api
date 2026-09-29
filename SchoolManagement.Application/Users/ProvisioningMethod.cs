namespace SchoolManagement.Application.Users;

/// <summary>How a new staff/parent account is set up.</summary>
public enum ProvisioningMethod
{
    /// <summary>User gets an email and sets their own password via a link.</summary>
    Invitation = 1,

    /// <summary>Admin sets a system-generated password immediately; no email required.</summary>
    Manual = 2
}
