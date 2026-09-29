namespace SchoolManagement.Application.Common.Interfaces;

public interface IPasswordGenerator
{
    /// <summary>Generates a random password that satisfies the configured Identity password policy.</summary>
    string Generate(int length = 12);
}
