using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQWeb.ExcelCreate.Models;
using RabbitMQWeb.ExcelCreate.Services;

namespace RabbitMQWeb.ExcelCreate.Controllers;

[Authorize]
public class ProductController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _appDbContext;
    private readonly RabbitMQPublisher _rabbitMQPublisher;

    public ProductController(UserManager<IdentityUser> userManager, 
        AppDbContext appDbContext, RabbitMQPublisher rabbitMQPublisher)
    {
        _userManager = userManager;
        _appDbContext = appDbContext;
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> CreateProductExcel()
    {
        //önce kullanıcıyı bulalım
        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        //kullanıcı bulundu, dosya oluşturalım
        var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1,10)}";

        UserFile userFile = new()
        {
            UserId = user.Id,
            FileName = fileName,
            FileStatus = FileStatus.Creating
        };

        //veritabanında bu dosyayı oluşturalım
        await _appDbContext.UserFiles.AddAsync(userFile);

        await _appDbContext.SaveChangesAsync();

        //RabbitMq'ya mesaj gönderildi.
        _rabbitMQPublisher.Publish(new Shared.CreateExcelMessage
        {
            FileId = userFile.Id,
            UserId = userFile.UserId
        });

        TempData["StartCreatingExcel"] = true;

        return RedirectToAction("Files","Product");
    }

    public async Task<IActionResult> Files()
    {
        //önce kullanıcıyı bulalım
        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        //kullanıcıya ait dosyaları bulup return edelim
        return View(await _appDbContext.UserFiles.Where(x => x.UserId == user.Id).ToListAsync());
    }
}
