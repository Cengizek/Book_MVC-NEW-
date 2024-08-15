using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository
{
    public class ProductRepository :Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }

       

        public void Update(Product obj)
        {
            _db.Products.Update(obj);
            //var objFromDb = _db.Products.FirstOrDefault(u=>u.Id == obj.Id);
            // if (objFromDb != null)
            // {
            //     objFromDb.Title = objFromDb.Title;
            //     objFromDb.ISBN = objFromDb.ISBN;
            //     objFromDb.Price = objFromDb.Price;
            //     objFromDb.Price50 = objFromDb.Price50;
            //     objFromDb.Price100 = objFromDb.Price100;
            //     objFromDb.ListPrice = objFromDb.ListPrice;
            //     objFromDb.Description = objFromDb.Description;
            //     objFromDb.CategoryId = objFromDb.CategoryId;
            //     objFromDb.Author = objFromDb.Author;
            //     objFromDb.ProductImages = objFromDb.ProductImages;
            // }
        }
    }
}

