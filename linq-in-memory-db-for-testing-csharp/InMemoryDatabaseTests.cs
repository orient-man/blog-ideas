using System.Linq;
using Example.DataLayer;
using FluentAssertions;
using NUnit.Framework;

namespace Infrastructure
{
    [TestFixture]
    public class InMemoryDatabaseTests
    {
        [Test]
        public void CreatesDataProvider()
        {
            // arrange
            var db = new InMemoryDatabase();

            // act
            var dataProvider = db.CreateDataProvider();

            // assert
            dataProvider.Should().NotBeNull();
            dataProvider.Products.Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public void CreatesBasicDal()
        {
            // arrange
            var db = new InMemoryDatabase();

            // act
            var dal = db.CreateBasicDal<Product>();

            // assert
            dal.Should().NotBeNull();
        }

        [Test]
        public void InsertSetsPrimaryKey()
        {
            // arrange
            var db = new InMemoryDatabase();
            var dal = db.CreateBasicDal<Product>();

            // act
            var inserted = dal.Insert(new Product());

            // assert
            inserted.Should().NotBeNull();
            inserted.ProductsID.Should().BeGreaterThan(0);
        }

        [Test]
        public void InsertWithPredefinedKey()
        {
            // arrange
            var db = new InMemoryDatabase();
            var dal = db.CreateBasicDal<Product>();

            // act
            var inserted = dal.Insert(new Product { ProductsID = 5 });

            // assert
            inserted.Should().NotBeNull();
            inserted.ProductsID.Should().Be(5);
        }

        [Test]
        public void InsertAndSelectById()
        {
            // arrange
            var db = new InMemoryDatabase();
            var dal = db.CreateBasicDal<Product>();
            var dataProvider = db.CreateDataProvider();

            // act
            var inserted = dal.Insert(new Product { ProductsName = "product" });

            // assert
            dataProvider.Products
                .FirstOrDefault(o => o.ProductsID == inserted.ProductsID)
                .Should().NotBeNull().And.Subject.ShouldBeEquivalentTo(inserted);
        }

        [Test]
        public void GenericInsert()
        {
            // arrange
            var db = new InMemoryDatabase();

            // act
            var inserted = db.Insert(new Product());

            // assert
            inserted.Should().NotBeNull();
            inserted.ProductsID.Should().BeGreaterThan(0);
        }

        [Test]
        public void GenericUpdate()
        {
            // arrange
            var db = new InMemoryDatabase();
            var dal = db.CreateBasicDal<Product>();
            var dataProvider = db.CreateDataProvider();
            var inserted = db.Insert(new Product { ProductName = "before" });

            // act
            inserted.ActionName = "after";
            dal.PersistChanges(inserted);

            // assert
            dataProvider.Actions.ShouldBeEquivalentTo(new[] { inserted });
        }

        [Test]
        public void PersistAllChanges()
        {
            // arrange
            var db = new InMemoryDatabase();
            var dal = db.CreateBasicDal<Product>();
            var dataProvider = db.CreateDataProvider();
            var inserted = new[]
            {
                db.Insert(new Product { ProductName = "first" }),
                db.Insert(new Product { ProductName = "second" })
            };

            // act
            inserted.ToList().ForEach(o => o.ProductName += "+");
            dal.PersistAllChanges(inserted);

            // assert
            dataProvider.Actions.ShouldBeEquivalentTo(inserted);
        }
    }
}