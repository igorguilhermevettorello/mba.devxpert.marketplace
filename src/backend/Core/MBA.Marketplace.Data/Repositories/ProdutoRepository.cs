﻿using MBA.Marketplace.Business.DTOs.Paginacao;
using MBA.Marketplace.Business.Interfaces.Repositories;
using MBA.Marketplace.Business.Models;
using MBA.Marketplace.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace MBA.Marketplace.Data.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ApplicationDbContext _context;
        public ProdutoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Produto>> ListarPorCategoriaIdAsync(Guid categoriaId, bool include)
        {
            if (include)
            {
                return await _context
                        .Produtos
                        .Include(p => p.Categoria)
                        .Include(p => p.Vendedor)
                        .Where(p => p.CategoriaId == categoriaId)
                        .ToListAsync();
            }
            else
            {
                return await _context
                        .Produtos
                        .Where(p => p.CategoriaId == categoriaId)
                        .ToListAsync();
            }
        }

        public async Task<IEnumerable<Produto>> ListarProdutosPorCategoriaOuNomeDescricaoAsync(Guid? categoriaId, string descricao)
        {
            var query = _context.Produtos.AsQueryable();

            if (categoriaId != null)
            {
                query = query.Where(p => p.CategoriaId == categoriaId);
            }

            if (descricao != null)
            {
                query = query.Where(p => p.Nome.Contains(descricao));
            }

            return await query.ToListAsync();
        }

        public async Task<ListaPaginada<Produto>> PesquisarAsync(PesquisaDeProdutos parametros)
        {
            var query = _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Vendedor)
                .Where(p => p.Ativo == true)
                .AsNoTracking()
                .AsQueryable();

            //Pesquisa dinâmica
            if (!string.IsNullOrWhiteSpace(parametros.TermoPesquisado))
            {
                query = query.Where(p =>
                    (p.Descricao.ToLower().Contains(parametros.TermoPesquisado.Trim().ToLower()))
                    || (p.Nome.ToLower().Contains(parametros.TermoPesquisado.Trim().ToLower()))
                );
            }

            if (parametros.CategoriaId != null)
            {
                query = query.Where(p => p.CategoriaId == parametros.CategoriaId);
            }

            if (parametros.VendedorId != null)
            {
                query = query.Where(p => p.VendedorId == parametros.VendedorId);
            }

            //Ordenação dinâmica
            if (!string.IsNullOrWhiteSpace(parametros.OrderBy))
            {
                var ehOrdemDecrescente = parametros.OrderBy.StartsWith("-");
                var propriedade = ehOrdemDecrescente ? parametros.OrderBy.Substring(1) : parametros.OrderBy;

                //Verificação se o campo é decimal pois SQLite não suporta linq dinâmico neste tipo de dado.
                if (parametros.OrderBy.Contains(nameof(Produto.Preco), StringComparison.CurrentCultureIgnoreCase))
                {
                    query = ehOrdemDecrescente ?
                        query.OrderByDescending(p => (double)p.Preco) :
                        query.OrderBy(p => (double)p.Preco);
                }
                else
                {
                    query = query.OrderBy($"{propriedade} {(ehOrdemDecrescente ? "descending" : "ascending")}");
                }
            }
            else
            {
                query = query.OrderBy(p => p.Id); // Ordenação padrão
            }

            return await ListaPaginada<Produto>.ListarAsync(query, parametros.NumeroDaPagina, parametros.TamanhoDaPagina);
        }

        public async Task<IEnumerable<Produto>> ListarPorVendedorIdAsync(Vendedor vendedor)
        {
            return await _context
                .Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Vendedor)
                .Where(p => p.VendedorId == vendedor.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Produto>> ListarAsync()
        {
            return await _context
                    .Produtos
                    .Include(p => p.Categoria)
                    .Include(p => p.Vendedor)
                    .ToListAsync();
        }

        public async Task<Produto> CriarAsync(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<bool> AtualizarAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AtualizarAsync(IEnumerable<Produto> produtos)
        {
            if (produtos == null || produtos.Any())
                return false;

            foreach (var produto in produtos)
            {
                _context.Produtos.Update(produto);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Produto> ObterPorIdAsync(Guid id)
        {
            return await _context
                .Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Vendedor)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<Produto?> ObterProdutoAtivoPorIdAsync(Guid id)
        {
            return await _context
                .Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Vendedor)
                .Where(p => p.Id == id && p.Ativo == true)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> RemoverAsync(Produto produto)
        {
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Produto> ObterPorIdPorVendedorIdAsync(Guid id, Vendedor vendedor)
        {
            return await _context.Produtos.Where(p => p.Id == id && p.VendedorId == vendedor.Id).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Produto>> ListarProdutosFiltroAsync(string? ordenarPor, int? limit)
        {
            var query = _context.Produtos
                                .Include(p => p.Categoria)
                                .Include(p => p.Vendedor)
                                .Where(p => p.Ativo == true)
                                .AsQueryable();

            query = query.OrderByDescending(p => p.CreatedAt);

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            return await query.ToListAsync();
        }

    }
}