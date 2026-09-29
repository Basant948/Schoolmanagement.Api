using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Students;
using SchoolManagement.Application.Students.Dtos;
using SchoolManagement.Domain.Constants;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController(IStudentService studentService) : ControllerBase
{
    private const string ReadRoles = $"{Roles.Admin},{Roles.Teacher},{Roles.Accountant}";

    /// <summary>List students with search, status filter and paging.</summary>
    [HttpGet]
    [Authorize(Roles = ReadRoles)]
    public async Task<ActionResult<PagedResult<StudentResponse>>> GetAll(
        [FromQuery] StudentQuery query, CancellationToken cancellationToken) =>
        Ok(await studentService.GetPagedAsync(query, cancellationToken));

    [HttpGet("{id:guid}")]
    [Authorize(Roles = ReadRoles)]
    [ProducesResponseType<StudentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await studentService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType<StudentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StudentResponse>> Create(
        CreateStudentRequest request, CancellationToken cancellationToken)
    {
        var created = await studentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType<StudentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StudentResponse>> Update(
        Guid id, UpdateStudentRequest request, CancellationToken cancellationToken) =>
        Ok(await studentService.UpdateAsync(id, request, cancellationToken));

    /// <summary>Soft delete - the row stays in the database with IsDeleted = true.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await studentService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
