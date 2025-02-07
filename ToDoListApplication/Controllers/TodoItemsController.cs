﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListApplication.Data;
using ToDoListApplication.Models;

namespace ToDoListApplication.Controllers
{
    [Authorize]
    public class TodoItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TodoItemsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoItemsController(ApplicationDbContext context, ILogger<TodoItemsController> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: TodoItems
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("TodoItems Index sayfası açıldı.");
            var userId = _userManager.GetUserId(User);
            var todoItems = await _context.TodoItems
                                          .Where(t => t.UserId == userId)
                                          .ToListAsync();
            return View(todoItems);
        }

        // GET: TodoItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details eylemi için id null olarak sağlandı.");
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var todoItem = await _context.TodoItems
                                         .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (todoItem == null)
            {
                _logger.LogWarning("Id {Id} ile TodoItem bulunamadı.", id);
                return NotFound();
            }

            _logger.LogInformation("Id {Id} ile TodoItem detayları görüntülendi.", id);
            return View(todoItem);
        }

        // GET: TodoItems/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Create sayfası açıldı.");
            return View();
        }

        // POST: TodoItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,IsCompleted")] TodoItemModel todoItem)
        {
            todoItem.UserId = _userManager.GetUserId(User); // Kullanıcının kimliğini al ve UserId'yi ayarla

            if (ModelState.IsValid)
            {
                _context.Add(todoItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Yeni TodoItem oluşturuldu: {Title}", todoItem.Title);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Model geçersiz: {ModelState}", ModelState);
            return View(todoItem);
        }

        // GET: TodoItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit eylemi için id null olarak sağlandı.");
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var todoItem = await _context.TodoItems
                                         .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (todoItem == null)
            {
                _logger.LogWarning("Id {Id} ile TodoItem bulunamadı.", id);
                return NotFound();
            }

            _logger.LogInformation("Id {Id} ile TodoItem düzenleme sayfası açıldı.", id);
            return View(todoItem);
        }

        // POST: TodoItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IsCompleted")] TodoItemModel todoItem)
        {
            if (id != todoItem.Id)
            {
                _logger.LogWarning("Id {Id} ile TodoItem eşleşmiyor.", id);
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var originalTodoItem = await _context.TodoItems
                                                 .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (originalTodoItem == null)
            {
                _logger.LogWarning("Id {Id} ile TodoItem bulunamadı.", id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    originalTodoItem.Title = todoItem.Title;
                    originalTodoItem.IsCompleted = todoItem.IsCompleted;

                    _context.Update(originalTodoItem);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("TodoItem güncellendi: {Title}", originalTodoItem.Title);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoItemExists(todoItem.Id))
                    {
                        _logger.LogError("DbUpdateConcurrencyException: Id {Id} ile TodoItem bulunamadı.", todoItem.Id);
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Model geçersiz: {ModelState}", ModelState);
            return View(todoItem);
        }

        // GET: TodoItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete eylemi için id null olarak sağlandı.");
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var todoItem = await _context.TodoItems
                                         .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (todoItem == null)
            {
                _logger.LogWarning("Id {Id} ile TodoItem bulunamadı.", id);
                return NotFound();
            }

            _logger.LogInformation("Id {Id} ile TodoItem silme sayfası açıldı.", id);
            return View(todoItem);
        }

        // POST: TodoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var todoItem = await _context.TodoItems
                                         .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (todoItem != null)
            {
                _context.TodoItems.Remove(todoItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Id {Id} ile TodoItem silindi.", id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TodoItemExists(int id)
        {
            var userId = _userManager.GetUserId(User);
            return _context.TodoItems.Any(e => e.Id == id && e.UserId == userId);
        }
    }
}
