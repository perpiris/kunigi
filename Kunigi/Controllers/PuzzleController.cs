using Kunigi.Data;
using Microsoft.AspNetCore.Mvc;

namespace Kunigi.Controllers;

[Route("puzzles")]
public class PuzzleController(DataContext context, IConfiguration configuration) : Controller
{
}