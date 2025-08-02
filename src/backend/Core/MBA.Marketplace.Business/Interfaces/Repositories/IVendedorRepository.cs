﻿using MBA.Marketplace.Business.Models;

namespace MBA.Marketplace.Business.Interfaces.Repositories
{
    public interface IVendedorRepository
    {
        Task<bool> CriarAsync(Vendedor vendedor);
        Task<Vendedor?> ObterPorUsuarioIdAsync(string usuarioId);
        Task<IEnumerable<Vendedor>> ListarAsync();

    }
}
