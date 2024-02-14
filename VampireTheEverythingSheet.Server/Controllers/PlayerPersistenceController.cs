using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VampireTheEverythingSheet.Server.Controllers
{
    public class PlayerPersistenceController : Controller
    {
        // GET: PlayerPersistenceController
        public ActionResult Index()
        {

            return View();
        }

        // GET: PlayerPersistenceController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PlayerPersistenceController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlayerPersistenceController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PlayerPersistenceController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PlayerPersistenceController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PlayerPersistenceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PlayerPersistenceController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
