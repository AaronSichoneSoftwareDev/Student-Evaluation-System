using Evaluate.Domain.Common;
using Evaluate.Domain.Enums;

namespace Evaluate.Domain.Entities.People;

public class Student : BaseAuditableEntity
{
    public string StudentNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; } = true;

    public string FullName => string.IsNullOrWhiteSpace(MiddleName)
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";

    public ICollection<StudentEnrollment> Enrollments { get; private set; } = new List<StudentEnrollment>();

    private Student()
    {
    }

    private Student(string studentNumber, string firstName, string? middleName, string lastName, DateOnly dateOfBirth, Gender gender, string? email, string? phoneNumber, string? address)
    {
        StudentNumber = studentNumber;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    public static Student Create(
        string studentNumber,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        Gender gender,
        string? middleName = null,
        string? email = null,
        string? phoneNumber = null,
        string? address = null)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            throw new ArgumentException("Student number is required.", nameof(studentNumber));
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        return new Student(
            studentNumber.Trim(),
            firstName.Trim(),
            string.IsNullOrWhiteSpace(middleName) ? null : middleName.Trim(),
            lastName.Trim(),
            dateOfBirth,
            gender,
            email?.Trim(),
            phoneNumber?.Trim(),
            address?.Trim());
    }

    public void Deactivate() => IsActive = false;

    public void Update(
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        Gender gender,
        string? middleName = null,
        string? email = null,
        string? phoneNumber = null,
        string? address = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth;
        Gender = gender;
        MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName.Trim();
        Email = email?.Trim();
        PhoneNumber = phoneNumber?.Trim();
        Address = address?.Trim();
    }
}
