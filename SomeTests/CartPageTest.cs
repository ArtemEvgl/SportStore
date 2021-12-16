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


            //act

            CartModel cartModel = new CartModel(mockRepo.Object, cartTest);

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

            //act
            CartModel cartModel = new CartModel(mockStore.Object, cartTest);

            cartModel.OnPost(1, "myUrl");

            //assert
            Assert.Single(cartTest.Lines);
            Assert.Equal("P1", cartTest.Lines.First().Product.Name);
            Assert.Equal(1, cartTest.Lines.First().Quantity);
        }
    }
}
