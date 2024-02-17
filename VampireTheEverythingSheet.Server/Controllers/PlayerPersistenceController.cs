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
            //TODO: We probably need more unique IDs, and stuff
            string? data = SessionExtensions.GetString(HttpContext.Session, "CharacterData");

            if (data != null)
            {
                Newtonsoft.Json.JsonConvert.DeserializeObject<Character>(data);
            }

            Character newChar = new("testChar");

            SessionExtensions.SetString(HttpContext.Session, "CharacterData", Newtonsoft.Json.JsonConvert.SerializeObject(newChar));

            return newChar;
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
