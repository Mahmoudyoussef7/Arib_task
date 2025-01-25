using Core.Entities;
using Core.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Arib_task.Controllers;

[Authorize]
public class TaskController(
    IGenericRepository<Core.Entities.Task> _taskRepository,
    IGenericRepository<Employee> _employeeRepository,
    UserManager<AppUser> userManager) : Controller
{

    public async Task<IActionResult> Add()
    {
        var user = await _userManager.GetUserAsync(User.Identity.Name);
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }

        var employees = await _employeeRepository.ListAllByConditionAsync(e => e.ManagerId == user.Id);
        ViewBag.Employees = new SelectList(employees, "Id", "FullName");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(Core.Entities.Task model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var employee = await _employeeRepository.GetByIdAsync(model.AssignedToEmployeeId);
            if (employee == null || employee.ManagerId != user.Id)
            {
                return Unauthorized(); // Ensure the manager can only assign tasks to their employees
            }

            model.CreatedAt = DateTime.Now;
            model.Status = "TODO";

            await _taskRepository.AddAsync(model);
            return RedirectToAction(nameof(ListTasks));
        }
        return View(model);
    }

    public async Task<IActionResult> Index(int employeeId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null || employee.ManagerId != user.Id)
        {
            return Unauthorized(); 
        }

        var tasks = await _taskRepository.ListAllByConditionAsync(t => t.AssignedToEmployeeId == employeeId);
        return View(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(int taskId, TaskStatus newStatus)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var employee = task.Employee;
        if (employee.ManagerId != user.Id)
        {
            return Unauthorized(); 
        }

        task.Status = newStatus;
        await _taskRepository.UpdateAsync(task);

        return RedirectToAction(nameof(ListTasks), new { employeeId = employee.Id });
    }
}