using Dominio.Entities;
using Dtos;

namespace Test.WebApi;

[TestClass]
public class CrearIncidenciaRequestTest
{
    [TestMethod]
    public void CrearIncidenciaRequest_SettersYGetters()
    {
        var atraccionId = Guid.NewGuid();
        var request = new CrearIncidenciaRequest
        {
            AtraccionId = atraccionId,
            TipoIncidencia = TipoIncidencia.Mantenimiento,
            Descripcion = "Falla en motor"
        };

        Assert.AreEqual(atraccionId, request.AtraccionId);
        Assert.AreEqual(TipoIncidencia.Mantenimiento, request.TipoIncidencia);
        Assert.AreEqual("Falla en motor", request.Descripcion);

        var nuevoId = Guid.NewGuid();
        request.AtraccionId = nuevoId;
        request.TipoIncidencia = TipoIncidencia.Rota;
        request.Descripcion = "Nueva descripción";

        Assert.AreEqual(nuevoId, request.AtraccionId);
        Assert.AreEqual(TipoIncidencia.Rota, request.TipoIncidencia);
        Assert.AreEqual("Nueva descripción", request.Descripcion);
    }
}
