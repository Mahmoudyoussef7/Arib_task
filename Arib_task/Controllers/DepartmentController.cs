using Core.Entities;
using Core.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Arib_task.Controllers;

[Authorize]
public class DepartmentController(UserManager<AppUser> _userManager, IGenericRepository<Department> _departmentRepository, IGenericRepository<Employee> _employeeRepository)
                : Controller
{
    public async Task<IActionResult> Index()
    {
        var departments = await _departmentRepository.ListAllAsync();
        var departmentDetails = departments.Select( d => new
        {
            Department = d,
            EmployeeCount = _employeeRepository.ListAllByConditionAsync(e => e.DepartmentId == d.Id).Result.Count,
            TotalSalary = _employeeRepository.ListAllByConditionAsync(e => e.DepartmentId == d.Id).Result.Sum(e => e.Salary)
        }).ToList();

        return View(departmentDetails);
    }

    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(Department model)
    {
        if (ModelState.IsValid)
        {
            await _departmentRepository.AddAsync(model);
            return Json(new { success = true });
        }

        return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null) return NotFound();
        return View(department);
    }

    [HttpPost]
    public async Task<IActionResult> EditAsync(Department model)
    {
        if (ModelState.IsValid)
        {
            var department = await _departmentRepository.GetByIdAsync(model.Id);
            if (department == null) return NotFound();

            department.Name = model.Name;
            await _departmentRepository.UpdateAsync(department);

            return Json(new { success = true });
        }

        return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null) return NotFound();

        var employeeCount = await _employeeRepository.ListAllByConditionAsync(e => e.DepartmentId == department.Id);
        if (employeeCount.Any())
        {
            return Json(new { success = false, message = "Cannot delete department with employees assigned." });
        }

        var userId = _userManager.GetUserId(User);
        var user = await _employeeRepository.GetByConditionAsync(u => u.AppUserId == userId);

        if (user == null)
        {
            return Json(new { success = false, message = "User not found." });
        }

        bool isUserRelated = user.DepartmentId == department.Id;
        if (!isUserRelated)
        {
            return Json(new { success = false, message = "You are not authorized to delete this department." });
        }

        return View(department);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department != null)
        {
            await _departmentRepository.DeleteAsync(department);
        }

        return Json(new { success = true });
    }

    public async Task<IActionResult> SearchAsync(string searchString)
    {
        var departments = string.IsNullOrEmpty(searchString)
            ? await _departmentRepository.ListAllAsync()
            : await _departmentRepository.ListAllByConditionAsync(d => d.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));

        var departmentDetails = departments.Select(async d => new
        {
            Department = d,
            EmployeeCount =(await _employeeRepository.ListAllByConditionAsync(e => e.DepartmentId == d.Id)).Count,
            TotalSalary = (await _employeeRepository.ListAllByConditionAsync(e => e.DepartmentId == d.Id)).Sum(e => e.Salary)
        }).ToList();

        return PartialView("_DepartmentList", departmentDetails);
    }
}