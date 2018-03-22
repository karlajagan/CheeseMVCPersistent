using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using System.Collections.Generic;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class CheeseController : Controller
    {
        private CheeseDbContext context;

        public CheeseController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            IList<Cheese> cheeses = context.Cheeses.Include(c => c.Category).ToList();

            return View(cheeses);
        }

        public IActionResult Add()
        {
            AddCheeseViewModel addCheeseViewModel =
                new AddCheeseViewModel(context.Categories.ToList());
            return View(addCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddCheeseViewModel addCheeseViewModel)
        {
            if (ModelState.IsValid)
            {
                CheeseCategory newCheeseCategory =
                    context.Categories.Single(c => c.ID == addCheeseViewModel.
                    CategoryID);
                // Add the new cheese to my existing cheeses
                Cheese newCheese = new Cheese
                {
                    Name = addCheeseViewModel.Name,
                    Description = addCheeseViewModel.Description,
                    Category = newCheeseCategory
                };

                context.Cheeses.Add(newCheese);
                context.SaveChanges();

                return Redirect("/Cheese");
            }

            return View(addCheeseViewModel);
        }

        public IActionResult Remove()
        {
            IList<Cheese> cheeses = context.Cheeses.Include(c => c.Category).ToList();
            return View(cheeses);
        }

        [HttpPost]
        public IActionResult Remove(int[] cheeseIds)
        {
            foreach (int cheeseId in cheeseIds)
            {
                List<CheeseMenu> cheeseMenu = context.CheeseMenus
                    .Where(cm => cm.CheeseID == cheeseId).ToList();

                foreach (CheeseMenu cheesemenu in cheeseMenu)
                {
                    context.CheeseMenus.Remove(cheesemenu);
                }
                context.SaveChanges();
                Cheese theCheese = context.Cheeses.Single(c => c.ID == cheeseId);
                context.Cheeses.Remove(theCheese);
            }
            context.SaveChanges();
            ViewBag.cheeses = context.Cheeses.ToList();            
            return Redirect("/");
        }

        public IActionResult Edit(int cheeseId)
        {
            Cheese cheeseToUpdate = context.Cheeses.Single(c => c.ID == cheeseId);          
            return View(cheeseToUpdate);
        }

        [HttpPost]
        public IActionResult Edit(Cheese toUpdateCheese)
        {
            Cheese updateCheese = context.Cheeses
                .Find(toUpdateCheese.ID);

            updateCheese.Name = toUpdateCheese.Name;
            updateCheese.Description = toUpdateCheese.Description;
            context.SaveChanges();

            return Redirect("/Cheese");
        }
    }
}
