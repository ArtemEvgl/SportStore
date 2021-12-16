using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SomeTests
{
    
    public class CartTest
    {
        [Fact]
        public void Can_Add_New_Lines()
        {
            //Arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart cart = new Cart();
            //Act
            cart.AddItem(p1, 2);
            cart.AddItem(p2, 1);
            var cartLines = cart.Lines.ToArray();
            //Assert
            Assert.Equal(2, cartLines.Count());
            Assert.Equal(p1, cartLines[0].Product);
            Assert.Equal(p2, cartLines[1].Product);
        }

        [Fact]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //Arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart cart = new Cart();
            //Act
            cart.AddItem(p1, 2);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 3);

            var cartLines = cart.Lines
                .OrderBy(x => x.CartLineID)
                .ToArray();
            //Assert
            Assert.Equal(2, cartLines.Length);
            Assert.Equal(5, cartLines[0].Quantity);
            Assert.Equal(1, cartLines[1].Quantity);

        }

        [Fact]
        public void Can_Remove_Line()
        {
            //Arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            Cart cart = new Cart();
            
            cart.AddItem(p1, 3);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 1);
            cart.AddItem(p3, 2);
            cart.AddItem(p2, 4);

            //Act
            cart.RemoveLine(p2);
            //Assert
            Assert.Empty(cart.Lines.Where(x => x.Product == p2));
            Assert.Equal(2, cart.Lines.Count());
        }

        [Fact]
        public void Can_Calculate_Total()
        {
            //Arrange
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 200M };

            Cart cart = new Cart();

            cart.AddItem(p1, 2);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 3);

            //Act
            decimal result = cart.ComputeTotalValue();

            //Assert
            Assert.Equal(700M, result);
        }

        [Fact]
        public void Can_Clear_Contents()
        {
            //Arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            Cart cart = new Cart();

            cart.AddItem(p1, 3);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 1);
            cart.AddItem(p3, 2);
            cart.AddItem(p2, 4);

            //Act
            cart.Clear();

            //Assert
            Assert.Empty(cart.Lines);
        }
    }
}
