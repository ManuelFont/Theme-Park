using Microsoft.AspNetCore.Mvc;
using ParqueTematico.WebApi.Filtros;

namespace Test.WebApi.Filtros;

[TestClass]
public class RequiereRolAttributeTests
{
    [TestMethod]
    public void RequiereRolAttribute_Constructor_SetsPropertiesCorrectly()
    {
        string[] rolEsperado = ["Administrador"];

        var attr = new RequiereRolAttribute(rolEsperado);

        Assert.IsInstanceOfType(attr, typeof(TypeFilterAttribute));
        var args = attr.Arguments;
        Assert.IsNotNull(args);
        Assert.AreEqual(1, args.Length);
        Assert.AreEqual(rolEsperado, args[0]);
    }
}
