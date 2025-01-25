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
    IGenericRepository<Employee> _employeeRepository)
    : Controller
{

    public async Task<IActionResult> Add()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var employee = await _employeeRepository.GetByConditionAsync(e=>e.AppUserId==userId);

        var employees = await _employeeRepository.ListAllByConditionAsync(e => e.ManagerId == employee.Id);
        ViewBag.Employees = new SelectList(employees, "Id", "FullName");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(Core.Entities.Task model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var manager = await _employeeRepository.GetByConditionAsync(e=>e.AppUserId==userId);
            var employee = await _employeeRepository.GetByIdAsync(model.AssignedToEmployeeId);
            if (employee == null || employee.ManagerId != manager.Id)
            {
                return Unauthorized();
            }

            model.CreatedAt = DateTime.Now;
            model.Status = "TODO";

            await _taskRepository.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Index(int employeeId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var manager = await _employeeRepository.GetByConditionAsync(e => e.AppUserId == userId);
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null || employee.ManagerId != manager.Id)
        {
            return Unauthorized(); 
        }

        var tasks = await _taskRepository.ListAllByConditionAsync(t => t.AssignedToEmployeeId == employeeId);
        return View(tasks);
    }
    
    public async Task<IActionResult> ListMyTasks()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var employee = await _employeeRepository.GetByConditionAsync(e => e.AppUserId == userId);
        if (employee == null)
        {
            return Unauthorized(); 
        }

        var tasks = await _taskRepository.ListAllByConditionAsync(t => t.AssignedToEmployeeId == employee.Id);
        return View(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(int taskId, string newStatus)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, [task=>task.AssignedToEmployeeId]);
        var employee = task.AssignedToEmployee;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var manager = await _employeeRepository.GetByConditionAsync(e => e.Id == employee.ManagerId);

        if (employee.AppUserId != userId && manager.Id!=employee.ManagerId)
        {
            return Unauthorized(); 
        }

        task.Status = newStatus;
        await _taskRepository.UpdateAsync(task);

        return Json(new { success = true });
    }
}