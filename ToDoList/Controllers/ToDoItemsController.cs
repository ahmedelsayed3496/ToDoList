using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ToDoList.DataAccess;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ToDoItemsController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        public IActionResult Index()
        {
            var todoItems = dbContext.ToDoItems;
            return View(todoItems);
        }
        [HttpGet]
        public IActionResult create()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Create(ToDoItem todoItem, IFormFile? file)
        {

            if (file != null && file.Length > 0)
            {

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }

                todoItem.File = fileName;
            }
            if (todoItem != null)
            {
                dbContext.ToDoItems.Add(todoItem);
                dbContext.SaveChanges();
                TempData["SuccessMessage"] = $"Created {todoItem.Title} successfully!";
                return RedirectToAction("Index");
            }
            return View(todoItem);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var todoItems = dbContext.ToDoItems.FirstOrDefault(e => e.Id == Id);
            if (todoItems != null)
            {
                return View(todoItems);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(ToDoItem todoItem, IFormFile? file)
        {
            var todoItemsDb = dbContext.ToDoItems.AsNoTracking().FirstOrDefault(e => e.Id == todoItem.Id);

            if (todoItemsDb == null)
            {
                return NotFound(); 
            }

           
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", fileName);

             
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }

             
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", todoItemsDb.File);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

             
                todoItem.File = fileName;
            }
            else
            {
              
                todoItem.File = todoItemsDb.File;
            }

          
            dbContext.ToDoItems.Update(todoItem);
            dbContext.SaveChanges();

            return RedirectToAction("Index"); 
        }

        public IActionResult Delete(int id)
        {
            var todoItem = dbContext.ToDoItems.FirstOrDefault(e => e.Id == id);

            if (todoItem != null)
            {
                
                
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", todoItem.File);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                

                
                dbContext.ToDoItems.Remove(todoItem);
                dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }


    }
}
