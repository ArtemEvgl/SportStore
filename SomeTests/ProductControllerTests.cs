using Moq;
using SportStore.Models;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using SportStore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SportStore.Infrastructure;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using SportStore.Models.ViewModels;
using System;

namespace SomeTests
{
    public class ProductControllerTests
    {
        [Fact]
        public void Can_Use_Repository()
        {
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"}
            }).AsQueryable<Product>());

            HomeController controller = new HomeController(mock.Object);

            ProductListViewModel result = controller.Index(null).ViewData.Model as ProductListViewModel;


            var products = result.Products.ToArray();
            Assert.True(products.Length == 2);
            Assert.Equal("P1", products[0].Name);
            Assert.Equal("P2", products[1].Name);
        }

        [Fact]
        public void Can_Paginate()
        {
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"},
            }).AsQueryable<Product>());

            HomeController homeController = new HomeController(mock.Object);
            homeController.PageSize = 3;

            ProductListViewModel result = homeController.Index(null, 2).ViewData.Model as ProductListViewModel;


            Product[] prodArray = result.Products.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }

        [Fact]
        public void Can_Generate_Page_Links()
        {
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.SetupSequence(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("Test/Page1")
                .Returns("Test/Page2")
                .Returns("Test/Page3");

            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            urlHelperFactory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(urlHelper.Object);

            PageLinkTagHelper helper =
                new PageLinkTagHelper(urlHelperFactory.Object)
                {
                    PageModel = new PagingInfo
                    {
                        CurrentPage = 2,
                        TotalItems = 28,
                        ItemsPerPage = 10
                    },
                    PageAction = "Test"
                };

            TagHelperContext ctx = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(), "");

            var content = new Mock<TagHelperContent>();
            TagHelperOutput output = new TagHelperOutput("div",
                new TagHelperAttributeList(),
                (cache, encoder) => Task.FromResult(content.Object));

            helper.Process(ctx, output);

            Assert.Equal(@"<a href=""Test/Page1"">1</a>" 
                        + @"<a href=""Test/Page2"">2</a>"
                        + @"<a href=""Test/Page3"">3</a>", output.Content.GetContent());

        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            //Arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"},
            }).AsQueryable<Product>());

            HomeController controller = new HomeController(mock.Object) { PageSize = 3 };

            //Act
            ProductListViewModel result = controller.Index(null, 2).ViewData.Model as ProductListViewModel;

            //Assert
            PagingInfo info = result.PagingInfo;
            Assert.Equal(2, info.CurrentPage);
            Assert.Equal(3, info.ItemsPerPage);
            Assert.Equal(5, info.TotalItems);
            Assert.Equal(2, info.TotalPages);

        }

        [Fact]
        public void Can_Filter_Products()
        {
            //Arrange
            var mock = new Mock<IStoreRepository>();
            mock.Setup(x => x.Products).Returns((new Product[] { 
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"},
            }).AsQueryable<Product>);

            HomeController controller = new HomeController(mock.Object);
            //Act
            Product[] result = (controller.Index("Cat2", 1).ViewData.Model as ProductListViewModel)
                        .Products.ToArray();

            //Assert
            Assert.Equal(2, result.Length);
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[1].Category == "Cat2");            
        }


        [Fact]
        public void Generate_Category_Specific_Product_Count()
        {
            //Arrange
            Mock<IStoreRepository> store = new Mock<IStoreRepository>();
            store.Setup(x => x.Products).Returns((new Product[] {
                new Product{ProductID = 1, Category = "Cat1", Name = "P1"},
                new Product{ProductID = 2, Category = "Cat2", Name = "P2"},
                new Product{ProductID = 3, Category = "Cat1", Name = "P3"},
                new Product{ProductID = 4, Category = "Cat2", Name = "P4"},
                new Product{ProductID = 5, Category = "Cat3", Name = "P5"},
            }).AsQueryable<Product>);
            HomeController target = new HomeController(store.Object);
            target.PageSize = 3;

            Func<ViewResult, ProductListViewModel> GetModel = result => 
                result?.ViewData?.Model as ProductListViewModel;
            //Act
            int? res1 = GetModel(target.Index("Cat1"))?.PagingInfo.TotalItems;
            int? res2 = GetModel(target.Index("Cat2"))?.PagingInfo.TotalItems;
            int? res3 = GetModel(target.Index("Cat3"))?.PagingInfo.TotalItems;
            int? resAll = GetModel(target.Index(null))?.PagingInfo.TotalItems;
            //Assert
            Assert.Equal(2, res1);
            Assert.Equal(2, res2);
            Assert.Equal(1, res3);
            Assert.Equal(5, resAll);
        }
    }
}
