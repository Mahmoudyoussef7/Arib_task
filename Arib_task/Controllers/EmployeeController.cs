using Arib_task.DTOs;
using Core.Entities;
using Core.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arib_task.Controllers;

[Authorize]
public class EmployeeController(IGenericRepository<Employee> _EmpRepository) : Controller
{
    
    private static int _idCounter = 1;

    public IActionResult Index()
    {
        return View(_EmpRepository.ListAllAsync());
    }

    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(EmployeeDTO model)
    {
        if (ModelState.IsValid)
        {
            var employee = model.Adapt<Employee>();
            var manager = await _EmpRepository.GetByConditionAsync(m => (m.FirstName + " " + m.LastName) == model.ManagerName);
            employee.ManagerId = manager.Id;

            await _EmpRepository.AddAsync(employee);
            return Json(new { success = true });
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        return Json(new { success = false, errors = string.Join("<br />", errors) });
    }

    public IActionResult Edit(int id)
    {
        var employee = _EmpRepository.GetByIdAsync(id);
        if (employee == null) return NotFound();
        return View(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EmployeeDTO model)
    {
        if (ModelState.IsValid)
        {
            var employee = await _EmpRepository.GetByIdAsync(model.Id);
            var manager = await _EmpRepository.GetByConditionAsync(m => (m.FirstName + " " + m.LastName) == (model.ManagerName));
            employee.ManagerId = manager.Id;

            if (employee != null)
            {
                employee = model.Adapt<Employee>();
                await _EmpRepository.UpdateAsync(employee);
            }
            return Json(new { success = true });
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        return Json(new { success = false, errors = string.Join("<br />", errors) });
    }

    public async Task<IActionResult> DeleteAsync(int id)
    {
        var employee = await _EmpRepository.GetByIdAsync(id);
        if (employee == null) return NotFound();
        return View(employee);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmedAsync(int id)
    {
        var employee = await _EmpRepository.GetByIdAsync(id);
        if (employee != null)
        {
            _EmpRepository.DeleteAsync(employee);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> SearchAsync(string searchString)
    {
        IReadOnlyList<Employee> results;
        if (string.IsNullOrEmpty(searchString))
        {
            results = await _EmpRepository.ListAllAsync();
            return View("Index", results);
        }
         results = await _EmpRepository
        .ListAllByConditionAsync(e => (e.FirstName + " " + e.LastName).Contains(searchString, StringComparison.OrdinalIgnoreCase));

        return View("Index", results);
    }

}
