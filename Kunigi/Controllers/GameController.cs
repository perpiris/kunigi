using Kunigi.Data;
using Microsoft.AspNetCore.Mvc;

namespace Kunigi.Controllers;

[Route("games")]
public class GameController(DataContext context, IConfiguration configuration) : Controller
{
    
}