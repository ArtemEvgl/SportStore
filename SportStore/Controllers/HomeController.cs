using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Controllers
{
    public class HomeController : Controller
    {
        private IStoreRepository _storeRepository;
        public int PageSize = 3;
        public HomeController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public ViewResult Index(string category, int productPage = 1)
            => View(new ProductListViewModel {
                Products = _storeRepository.Products
                .Where(p => category == null || p.Category == category)
                .OrderBy(p => p.ProductID)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ?
                    _storeRepository.Products.Count() :
                    _storeRepository.Products.Where(x => x.Category == category).Count(),
                },
                CurrentCategory = category
            }   );
    }
}
