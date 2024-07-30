using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.ViewModels.Puzzle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Controllers;

public class PuzzlesController(DataContext context) : Controller
{
    public async Task<IActionResult> List(int gameId)
    {
        var puzzles = await context.Puzzles
            .Where(p => p.GameId == gameId)
            .Select(p => new PuzzleViewModel
            {
                Id = p.Id,
                Question = p.Question,
                Answer = p.Answer,
                GameId = p.GameId
            })
            .ToListAsync();

        ViewBag.GameId = gameId;
        return View(puzzles);
    }

    // GET: Puzzle/Create/5
    public IActionResult Create(int gameId)
    {
        return View(new PuzzleViewModel { GameId = gameId });
    }

    // POST: Puzzle/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PuzzleViewModel model)
    {
        if (ModelState.IsValid)
        {
            var puzzle = new Puzzle
            {
                Question = model.Question,
                Answer = model.Answer,
                GameId = model.GameId
            };
            context.Add(puzzle);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(List), new { gameId = model.GameId });
        }

        return View(model);
    }

    // GET: Puzzle/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var puzzle = await context.Puzzles.FindAsync(id);
        if (puzzle == null)
        {
            return NotFound();
        }

        return View(new PuzzleViewModel
        {
            Id = puzzle.Id,
            Question = puzzle.Question,
            Answer = puzzle.Answer,
            GameId = puzzle.GameId
        });
    }

    // POST: Puzzle/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PuzzleViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var puzzle = await context.Puzzles.FindAsync(id);
                puzzle.Question = model.Question;
                puzzle.Answer = model.Answer;
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PuzzleExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(List), new { gameId = model.GameId });
        }

        return View(model);
    }

    // GET: Puzzle/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var puzzle = await context.Puzzles.FindAsync(id);
        if (puzzle == null)
        {
            return NotFound();
        }

        return View(new PuzzleViewModel
        {
            Id = puzzle.Id,
            Question = puzzle.Question,
            Answer = puzzle.Answer,
            GameId = puzzle.GameId
        });
    }

    // POST: Puzzle/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var puzzle = await context.Puzzles.FindAsync(id);
        context.Puzzles.Remove(puzzle);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(List), new { gameId = puzzle.GameId });
    }

    private bool PuzzleExists(int id)
    {
        return context.Puzzles.Any(e => e.Id == id);
    }
}