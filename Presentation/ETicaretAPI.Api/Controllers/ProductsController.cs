using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        public async Task<IActionResult> GetAll()
        {
            return Ok(_productReadRepository.GetAll(false));



            ////await _productWriteRepository.AddRangeAsync(new()
            //// {
            ////     new() {Id=Guid.NewGuid(),Name="Product 1",Price=100,CreatedDate=DateTime.UtcNow,Stock=10},
            ////     new() {Id=Guid.NewGuid(),Name="Product 2",Price=200,CreatedDate=DateTime.UtcNow,Stock=20},
            ////     new() {Id=Guid.NewGuid(),Name="Product 3",Price=300,CreatedDate=DateTime.UtcNow,Stock=30}
            //// });
            //// await _productWriteRepository.SaveAsync();
            //Product p=await _productReadRepository.GetByIdAsync("38a862c7-9bb2-4e6a-8113-4ecbcd7cc06d");
            ////tracking false şeklinde yaparsak takip edilmeyeceği için name ne yaparsak yapalım veritabanında güncellenmeyecek.
            ////Tracking kullanmak karlıdır çünkü yeri geldi mi sadece listeleme görüntüleme vs yapacaz,bir işlem olmayacak bunlarda.Boşuna tracking yapmak masraflı olur
            //p.Name = "Ahmet";
            //await _productWriteRepository.SaveAsync();
            ////Burda çektiğimiz p değeri ni write.saveasync da yazarak direk kaydettik.Bunu yapmamızı sağlaya scoped yaptığımız için.Çünkü bir tane instance oluşturmuştuk IoC de .WriteRepository de aynı instancı kullandığı için o işlemi görüp eklemeyi sağladı
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Get (string id)
        {
            return Ok(await _productReadRepository.GetByIdAsync(id, false));
        }



        [HttpPost]
        public async Task<IActionResult> Add(VM_Create_Product model)
        {
            if (ModelState.IsValid)
            {

            }


            await _productWriteRepository.AddAsync(new()
            {
                Name = model.Name,
                Price = model.Price,
                Stock = model.Stock
            });
            await _productWriteRepository.SaveAsync();
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut]
        public async Task<IActionResult> Update(VM_Update_Product model)
        {
           Product product=await _productReadRepository.GetByIdAsync(model.Id);
            product.Stock = model.Stock;
            product.Name = model.Name;
            product.Price = model.Price;
            await _productWriteRepository.SaveAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveAsync();
            return Ok();
        }
    }
}