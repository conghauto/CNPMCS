using MyCompanyName.AbpZeroTemplate.Auditing;
using Shouldly;
using Xunit;

namespace MyCompanyName.AbpZeroTemplate.Tests.Auditing
{
    public class NamespaceStripper_Tests: AppTestBase
    {
        private readonly INamespaceStripper _namespaceStripper;

        public NamespaceStripper_Tests()
        {
            _namespaceStripper = Resolve<INamespaceStripper>();
        }

        [Fact]
        public void Should_Stripe_Namespace()
        {
            var controllerName = _namespaceStripper.StripNameSpace("MyCompanyName.AbpZeroTemplate.Web.Controllers.HomeController");
            controllerName.ShouldBe("HomeController");
        }

        [Theory]
        [InlineData("MyCompanyName.AbpZeroTemplate.Auditing.GenericEntityService`1[[MyCompanyName.AbpZeroTemplate.Storage.BinaryObject, MyCompanyName.AbpZeroTemplate.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null]]", "GenericEntityService<BinaryObject>")]
        [InlineData("CompanyName.ProductName.Services.Base.EntityService`6[[CompanyName.ProductName.Entity.Book, CompanyName.ProductName.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[CompanyName.ProductName.Services.Dto.Book.CreateInput, N...", "EntityService<Book, CreateInput>")]
        [InlineData("MyCompanyName.AbpZeroTemplate.Auditing.XEntityService`1[MyCompanyName.AbpZeroTemplate.Auditing.AService`5[[MyCompanyName.AbpZeroTemplate.Storage.BinaryObject, MyCompanyName.AbpZeroTemplate.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[MyCompanyName.AbpZeroTemplate.Storage.TestObject, MyCompanyName.AbpZeroTemplate.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],]]", "XEntityService<AService<BinaryObject, TestObject>>")]
        public void Should_Stripe_Generic_Namespace(string serviceName, string result)
        {
            var genericServiceName = _namespaceStripper.StripNameSpace(serviceName);
            genericServiceName.ShouldBe(result);
        }
    }
}
