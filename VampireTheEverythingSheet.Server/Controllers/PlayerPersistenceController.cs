using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VampireTheEverythingSheet.Server.Models;

namespace VampireTheEverythingSheet.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerPersistenceController : Controller
    {

        //TODO: Somehow this is at https://localhost:5173/playerpersistence or something like that
        [HttpGet(Name = "GetCharacterData")]
        public Character GetCharacterData()
        {
            string? data = SessionExtensions.GetString(HttpContext.Session, "CharacterData");

            //TODO

            return new Character(data ?? "");
        }

        // GET: PlayerPersistenceController
        public ActionResult Index()
        {
            //TODO figure out what this is doing and why
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
