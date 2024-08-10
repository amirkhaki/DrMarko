using DrMarko.Data;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace DrMarko.Controllers;

public class ImagesController : Controller
{
    private readonly IMinioClient _minioClient;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public ImagesController(IMinioClient minioClient, ApplicationDbContext context, IConfiguration configuration)
    {
        _minioClient = minioClient;
        _context = context;
        _configuration = configuration;
    }

    public async Task<string> GetDownloadLinkAsync(string fileId)
    {
        var bucketName = _configuration["Minio:ImagesBucket"];


        var args = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileId)
            .WithExpiry(1200);

        return await _minioClient.PresignedGetObjectAsync(args);
    }

    public async Task<IActionResult> Index(int id)
    {
        var image = await _context.Image.FindAsync(id);
        if (image is null)
        {
            return NotFound();
        }

        string virtualPath = await GetDownloadLinkAsync(image.Hash);
        return Redirect(virtualPath);
    }
}
