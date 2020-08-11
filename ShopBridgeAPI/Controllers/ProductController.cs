using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopBridgeAPI.Entities;

namespace ShopBridgeAPI.Controllers
{
    [ApiController]
    public class ProductController : Controller
    {
        private readonly ShopDBContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductController(ShopDBContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        /* load all Image on the bais ofits path */
        [HttpGet]
        [Route("api/Product/GetImage")]
        public IActionResult GetImage(string filepath)
        {
            if (filepath == null || filepath == "" || filepath == "null")
            {
                filepath = "Resources\\Images\\noimage.png";
                Byte[] b = System.IO.File.ReadAllBytes(filepath);   //         
                return File(b, "image/png");
            }
            else
            {
                Byte[] b = System.IO.File.ReadAllBytes(filepath);            
                return File(b, "image/png");
            }

        }



        [HttpPost]
        [Route("api/Product/UploadFile")]
        public JsonResult UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Json(dbPath);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }
        /* Save itemsdetails in DB */
        [HttpPost]
        [Route("api/Product/SaveItems")]
        public IActionResult SaveItems([FromBody] ProductDetails products)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Add(products);
            try
            {

                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw;
            }


            return CreatedAtAction("GetItems", new { id = products.ID }, products);
        }
        /* load the all items on page load*/
        [HttpGet]
        [Route("api/Product/GetItems")]
        public List<ProductDetails> GetItems()
        {

            var itemList = _context.productDetails.OrderByDescending(s=>s.ID).ToList();
            return itemList.ToList();
        }

        /* Delete the perticular item from db on the basis of id */
        [HttpDelete]
        [Route("api/Product/RemoveItem")]
        public async Task<ProductDetails> RemoveItem(int Id)
        {
            var result = await _context.productDetails

               .FirstOrDefaultAsync(e => e.ID == Id);
            if (result != null)
            {
                try
                {
                    _context.productDetails.Remove(result);
                    if (result.FilePath == "" || result.FilePath == null)
                    { }
                    else
                        System.IO.File.Delete(result.FilePath);
                    await _context.SaveChangesAsync();
                    return result;

                }
                catch (DbUpdateException)
                {
                    result = null;
                    throw;
                }
            }

            return null;
        }


        /* Get item details of perticular id */
        [HttpGet]
        [Route("api/Product/GetItem")]
        public async Task<ProductDetails> GetItem(int Id)
        {
            var result = await _context.productDetails
            .FirstOrDefaultAsync(e => e.ID == Id);

            if (result != null)
            {

                return result;
            }
            return null;

        }

    }
}