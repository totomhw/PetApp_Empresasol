using System;
using System.Collections.Generic;

namespace PetApp_Empresa.Models;

public partial class CarritoAccesorio
{
    public int CarritoAccesorioId { get; set; }

    public int CarritoId { get; set; }

    public int AccesorioId { get; set; }

    public virtual Accesorio Accesorio { get; set; } = null!;

    public virtual CarritoDeCompra Carrito { get; set; } = null!;
}
