using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using SportStore.Components;
using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SomeTests
{
    public class NavigationMenuViewComponentTest
    {

        [Fact]
        public void Can_Select_Categories()
        {
            //Arrange
            Mock<IStoreRepository> store = new Mock<IStoreRepository>();
            store.Setup(x => x.Products).Returns((new Product[] {
                new Product {Name = "P1", ProductID = 1, Category = "Apples"},
                new Product {Name = "P2", ProductID = 2, Category = "Apples"},
                new Product {Name = "P3", ProductID = 3, Category = "Plums"},
                new Product {Name = "P4", ProductID = 4, Category = "Oranges"},
            }).AsQueryable<Product>);

            NavigationMenuViewComponent target = new NavigationMenuViewComponent(store.Object);

            //Act
            string[] results = ((IEnumerable<string>)(target.Invoke() as ViewViewComponentResult).ViewData.Model).ToArray();

            //Assert
            Assert.True(Enumerable.SequenceEqual(new string[] { "Apples", "Oranges", "Plums" }, results));
        }

        [Fact]
        public void Indicates_Selected_Category()
        {
            //Arrange
            string categoryToSelect = "Apples";
            Mock<IStoreRepository> store = new Mock<IStoreRepository>();
            store.Setup(x => x.Products).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 4, Name = "P2", Category = "Oranges"},
            }).AsQueryable<Product>);

            NavigationMenuViewComponent target = new NavigationMenuViewComponent(store.Object);
            target.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext
                {
                    RouteData = new Microsoft.AspNetCore.Routing.RouteData()
                }
            };

            target.RouteData.Values["category"] = categoryToSelect;

            //Act
            string result = (string)(target.Invoke() as ViewViewComponentResult).ViewData["SelectedCategory"];

            //Assert
            Assert.Equal(categoryToSelect, result);
        }
    }
}
