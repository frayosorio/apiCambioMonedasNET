using apiCambiosMoneda.Aplicacion.Servicios;
using apiCambiosMoneda.Core.Interfaces.Repositorios;
using apiCambiosMoneda.Core.Interfaces.Servicios;
using apiCambiosMoneda.Dominio.Entidades;
using Moq;

namespace apiCambiosMoneda.Test
{
    public class CambioMonedaServicioTests
    {
        private readonly Mock<IMonedaRepositorio> _monedaRepositorioMock;
        private readonly IMonedaServicio _monedaServicio;

        public CambioMonedaServicioTests()
        {
            _monedaRepositorioMock = new Mock<IMonedaRepositorio>();
            _monedaServicio = new MonedaServicio(_monedaRepositorioMock.Object);
        }

        [Fact]
        public async void ObtenerHistorialCambios_DeberiaRetornarListaDeCambios()
        {
            // Arrange
            var idMoneda = 1;
            var desde = new DateTime(2023, 01, 01);
            var hasta = new DateTime(2024, 01, 01);

            var listaCambios = new List<CambioMoneda>
                                    {
                                        new CambioMoneda { IdMoneda = idMoneda, Fecha = new DateTime(2023, 06, 01), Cambio = 1.2f },
                                        new CambioMoneda { IdMoneda = idMoneda, Fecha = new DateTime(2023, 12, 01), Cambio = 1.3f }
                                    }.AsEnumerable();

            _monedaRepositorioMock.Setup(repositorio => repositorio.ObtenerHistorialCambios(idMoneda, desde, hasta))
                .ReturnsAsync(listaCambios);

            // Act
            var resultado = await _monedaServicio.ObtenerHistorialCambios(idMoneda, desde, hasta);

            // Assert
            Assert.Equal(2, resultado.ToList().Count);
            Assert.Equal(1.2f, resultado.First().Cambio);
            Assert.Equal(1.3f, resultado.Last().Cambio);
        }

        [Fact]
        public async void ObtenerCambioActual_DeberiaRetornarUltimoCambio()
        {
            // Arrange
            var idMoneda = 1;
            var cambioMonedaActual = new CambioMoneda { IdMoneda = idMoneda, Fecha = DateTime.Now, Cambio = 1.4f };

            _monedaRepositorioMock.Setup(repo => repo.ObtenerCambioActual(idMoneda))
                .ReturnsAsync(cambioMonedaActual);

            // Act
            var resultado = await _monedaServicio.ObtenerCambioActual(idMoneda);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1.4f, resultado.Cambio);
        }

        [Fact]
        public async void ObtenerPaisesPorMoneda_DeberiaRetornarListaDePaises()
        {
            // Arrange
            var idMoneda = 1;
            var listaPaises = new List<Pais>
                {
                    new Pais { Id = 1, Nombre = "Argentina", IdMoneda = idMoneda },
                    new Pais { Id = 2, Nombre = "Brasil", IdMoneda = idMoneda }
                };

            _monedaRepositorioMock.Setup(repo => repo.ObtenerPaisesPorMoneda(idMoneda))
                .ReturnsAsync(listaPaises);

            // Act
            var resultado = await _monedaServicio.ObtenerPaisesPorMoneda(idMoneda);

            // Assert
            Assert.Equal(2, resultado.ToList().Count);
            Assert.Equal("Argentina", resultado.First().Nombre);
            Assert.Equal("Brasil", resultado.Last().Nombre);
        }

        [Fact]
        public async Task ObtenerCambioActual_NoDeberiaEncontrarCambioActual_YRetornarNull()
        {
            // Arrange
            var idMoneda = 1;

            // Mockear el repositorio para que retorne null cuando no se encuentra cambio actual
            _monedaRepositorioMock.Setup(repositorio => repositorio.ObtenerCambioActual(idMoneda))
                .ReturnsAsync((CambioMoneda)null);  // Retorna null

            // Act
            var resultado = await _monedaServicio.ObtenerCambioActual(idMoneda);

            // Assert
            Assert.Null(resultado);  // Verifica que el resultado sea nulo

            // Verifica que el método del repositorio fue llamado una vez
            _monedaRepositorioMock.Verify(repositorio => repositorio.ObtenerCambioActual(idMoneda), Times.Once);
        }


    }
}
