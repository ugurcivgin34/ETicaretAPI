using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly private IProductWriteRepository _productWriteRepository;
        readonly private IProductReadRepository _productReadRepository;

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
        }

        [HttpGet]
        public async Task  Get()
        {

            //await _productWriteRepository.AddRangeAsync(new()
            // {
            //     new() {Id=Guid.NewGuid(),Name="Product 1",Price=100,CreatedDate=DateTime.UtcNow,Stock=10},
            //     new() {Id=Guid.NewGuid(),Name="Product 2",Price=200,CreatedDate=DateTime.UtcNow,Stock=20},
            //     new() {Id=Guid.NewGuid(),Name="Product 3",Price=300,CreatedDate=DateTime.UtcNow,Stock=30}
            // });
            // await _productWriteRepository.SaveAsync();
            Product p=await _productReadRepository.GetByIdAsync("38a862c7-9bb2-4e6a-8113-4ecbcd7cc06d");
            //tracking false şeklinde yaparsak takip edilmeyeceği için name ne yaparsak yapalım veritabanında güncellenmeyecek.
            //Tracking kullanmak karlıdır çünkü yeri geldi mi sadece listeleme görüntüleme vs yapacaz,bir işlem olmayacak bunlarda.Boşuna tracking yapmak masraflı olur
            p.Name = "Ahmet";
            await _productWriteRepository.SaveAsync();
            //Burda çektiğimiz p değeri ni write.saveasync da yazarak direk kaydettik.Bunu yapmamızı sağlaya scoped yaptığımız için.Çünkü bir tane instance oluşturmuştuk IoC de .WriteRepository de aynı instancı kullandığı için o işlemi görüp eklemeyi sağladı
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> TaskGet(string id)
        {
            Product product=await _productReadRepository.GetByIdAsync(id);
            return Ok(product);
        }
    }
}
