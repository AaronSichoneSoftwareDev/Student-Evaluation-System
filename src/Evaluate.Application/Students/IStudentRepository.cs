using Evaluate.Application.Students.Queries.GetActiveStudentsList;
using Evaluate.Application.Students.Queries.GetStudentById;
using Evaluate.Application.Students.Queries.GetStudentsList;
using StudentEntity = Evaluate.Domain.Entities.People.Student;

namespace Evaluate.Application.Students;

public interface IStudentRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> StudentNumberExistsAsync(string studentNumber, CancellationToken cancellationToken = default);

    Task<StudentEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<StudentEntity>> GetByIdsAsync(List<int> ids, CancellationToken cancellationToken = default);

    void Add(StudentEntity student);

    Task<StudentDetailDto?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<StudentDto>> GetListAsync(CancellationToken cancellationToken = default);

    Task<List<ActiveStudentDto>> GetActiveListAsync(CancellationToken cancellationToken = default);

    Task<Dictionary<int, string>> GetNamesByIdsAsync(List<int> studentIds, CancellationToken cancellationToken = default);
}
