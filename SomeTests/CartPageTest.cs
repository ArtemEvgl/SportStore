using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using SportStore.Models;
using SportStore.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SomeTests
{
    
    public class CartPageTest
    {
        [Fact]
        public void Can_Load_Cart()
        {
            //arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Mock<IStoreRepository> mockRepo = new Mock<IStoreRepository>();
            mockRepo.Setup(m => m.Products)
                .Returns((new Product[] { p1, p2})
                .AsQueryable<Product>());

            Cart cartTest = new Cart();
            cartTest.AddItem(p1, 2);
            cartTest.AddItem(p2, 1);

            Mock<ISession> mockSession = new Mock<ISession>();
            byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cartTest));
            mockSession.Setup(c => c.TryGetValue(It.IsAny<string>(), out data));

            Mock<HttpContext> mockContext = new Mock<HttpContext>();
            mockContext.SetupGet(c => c.Session).Returns(mockSession.Object);

            //act

            CartModel cartModel = new CartModel(mockRepo.Object)
            {
                PageContext = new PageContext(new ActionContext
                {
                    HttpContext = mockContext.Object,
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor()
                })
            };

            cartModel.OnGet("myUrl");

            //assert
            Assert.Equal(2, cartModel.Cart.Lines.Count());
            Assert.Equal("myUrl", cartModel.ReturnUrl);
        }

        [Fact]
        public void Can_Update_Cart()
        {
            //arrange
            Mock<IStoreRepository> mockStore = new Mock<IStoreRepository>();
            mockStore.Setup(m => m.Products).Returns((new Product[] {
                new Product { ProductID = 1, Name = "P1" }
            }).AsQueryable<Product>);

            Cart cartTest = new Cart();

            Mock<ISession> mockSession = new Mock<ISession>();
            mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((key, val) =>
                {
                    cartTest = JsonSerializer.Deserialize<Cart>(Encoding.UTF8.GetString(val));
                });

            Mock<HttpContext> mockContext = new Mock<HttpContext>();
            mockContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            //act
            CartModel cartModel = new CartModel(mockStore.Object)
            {
                PageContext = new PageContext(new ActionContext
                {
                    HttpContext = mockContext.Object,
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor()
                })
            };

            cartModel.OnPost(1, "myUrl");

            //assert
            Assert.Single(cartTest.Lines);
            Assert.Equal("P1", cartTest.Lines.First().Product.Name);
            Assert.Equal(1, cartTest.Lines.First().Quantity);
        }
    }
}
