using ETicaretAPI.Application.Features.Commands.CreateProduct;
using ETicaretAPI.Application.Features.Queries.GetAllProduct;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ETicaretAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly private IProductWriteRepository _productWriteRepository;
        readonly private IProductReadRepository _productReadRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        readonly IMediator _mediator;

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IWebHostEnvironment webHostEnvironment, IMediator mediator)//IWebEnvironment wwwroot klasörüne erişmeyi sağlayan hızlı bir servis
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _webHostEnvironment = webHostEnvironment;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
        {
            GetAllProductQueryResponse response=await _mediator.Send(getAllProductQueryRequest);
            return Ok(response);

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
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _productReadRepository.GetByIdAsync(id, false));
        }



        [HttpPost]
        public async Task<IActionResult> Post(CreateProductCommandRequest createProductCommandRequest )
        {
            CreateProductCommandResponse response=await _mediator.Send(createProductCommandRequest);
           
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut]
        public async Task<IActionResult> Update(VM_Update_Product model)
        {
            Product product = await _productReadRepository.GetByIdAsync(model.Id);
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

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(string id)
        {

            return Ok();
        } //...com/api/products?id=123   upload yaparken ne göndereceğimiz tam kesin değil, o yüzden bu yapıyı kullandık

        [HttpGet("[Action]/{id}")]
        public async Task<IActionResult> GetProductImages(string id)// ...com/api/products/123  ne göndereceğiniz bildiğimiz için id gelecek hep o yü<den böyle kullandık
        {
            Product? product = await _productReadRepository.Table.Include(p => p.ProductImageFiles).FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));

            return Ok(product.ProductImageFiles.Select(p => new
            {
                p.Path,
                p.FileName
            }));
        }
    }
}