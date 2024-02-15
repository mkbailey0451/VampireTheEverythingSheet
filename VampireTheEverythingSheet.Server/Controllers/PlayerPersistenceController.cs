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

        // POST: PlayerPersistenceController/Create
        [HttpPost("playerpersistence/SetTrait")]
        [ValidateAntiForgeryToken]
        public ActionResult SetTrait(IFormCollection collection)
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
