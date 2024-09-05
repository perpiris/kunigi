using Kunigi.Data;

namespace Kunigi.Services.Implementation;

public class PuzzleService : IPuzzleService
{
    private readonly DataContext _context;
    private readonly IMediaService _mediaService;

    public PuzzleService(DataContext context, IMediaService mediaService)
    {
        _context = context;
        _mediaService = mediaService;
    }
}