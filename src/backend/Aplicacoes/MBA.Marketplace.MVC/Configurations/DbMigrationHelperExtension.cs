﻿using MBA.Marketplace.Business.Models;
using MBA.Marketplace.Business.Enums;
using MBA.Marketplace.Business.Extensions;
using MBA.Marketplace.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MBA.Marketplace.Business.Interfaces.Identity;

namespace MBA.Marketplace.MVC.Configurations
{
    public static class DbMigrationHelperExtension
    {
        public static void UseDbMigrationHelper(this WebApplication app)
        {
            DbMigrationHelpers.EnsureSeedData(app).Wait();
        }
    }

    public class DbMigrationHelpers
    {
        public static async Task EnsureSeedData(WebApplication serviceScope)
        {
            var services = serviceScope.Services.CreateScope().ServiceProvider;
            await EnsureSeedData(services);
        }

        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var contextIdentity = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (env.IsDevelopment() || env.IsEnvironment("Docker") || env.IsStaging())
            {
                await context.Database.MigrateAsync();
                await contextIdentity.Database.MigrateAsync();

                await EnsureSeedRoles(contextIdentity);
                await EnsureSeedApplication(userManager, context, contextIdentity);
            }
        }

        private static async Task EnsureSeedRoles(IdentityDbContext contextIdentity)
        {
            // Verifica se já existem roles criadas
            if (contextIdentity.Roles.Any())
                return;

            // Obtém todos os valores do enum TipoUsuario
            var tiposUsuario = Enum.GetValues(typeof(TipoUsuario)).Cast<TipoUsuario>();

            foreach (var tipoUsuario in tiposUsuario)
            {
                var roleName = tipoUsuario.GetDescription();
                var normalizedRoleName = roleName.ToUpperInvariant();
                if (!contextIdentity.Roles.Any(r => r.NormalizedName == normalizedRoleName))
                {
                    await contextIdentity.Roles.AddAsync(new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = roleName,
                        NormalizedName = normalizedRoleName,
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                }
            }

            await contextIdentity.SaveChangesAsync();
        }

        private static async Task EnsureSeedApplication(UserManager<IdentityUser> userManager,  ApplicationDbContext context, IdentityDbContext contextIdentity)
        {
            var emailAdmin = "administrador@marketplace.com.br";
            if (await userManager.FindByEmailAsync(emailAdmin) == null)
            {
                var user = new IdentityUser
                {
                    UserName = "Administrador",
                    Email = emailAdmin,
                    NormalizedUserName = "Administrador",
                    NormalizedEmail = emailAdmin,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                };

                var result = await userManager.CreateAsync(user, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, TipoUsuario.Administrador.ToString().ToUpper());
                }

                await contextIdentity.SaveChangesAsync();
            }

            var vendedorNome = "Vendedor";
            var vendedorEmail = "vendedor@marketplace.com.br";
            var vendedorId = Guid.NewGuid();
            if (await userManager.FindByEmailAsync(vendedorEmail) == null) 
            {
                var vendedorSistema = new IdentityUser
                {
                    Id = vendedorId.ToString(),
                    UserName = vendedorNome,
                    NormalizedUserName = vendedorNome,
                    Email = vendedorEmail,
                    NormalizedEmail = vendedorEmail,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                };

                var result = await userManager.CreateAsync(vendedorSistema, "Vendedor@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(vendedorSistema, TipoUsuario.Vendedor.ToString().ToUpper());
                }

                await contextIdentity.SaveChangesAsync();


                await context.Vendedores.AddAsync(new Vendedor
                {
                    Id = vendedorId,
                    Nome = vendedorNome,
                    Email = vendedorEmail,
                    CreatedAt = DateTime.UtcNow
                });
            }

            Guid eletronicoId = Guid.NewGuid();
            Guid roupaId = Guid.NewGuid();
            Guid livroId = Guid.NewGuid();
            if (!context.Categorias.Any())
            {
                context.Categorias.AddRange(
                    new Categoria { Id = eletronicoId, Nome = "Eletrônicos", Descricao = "Eletrônicos em geral", CreatedAt = DateTime.Now },
                    new Categoria { Id = roupaId, Nome = "Roupas", Descricao = "Roupas em geral", CreatedAt = DateTime.Now },
                    new Categoria { Id = livroId, Nome = "Livros", Descricao = "Livros em geral", CreatedAt = DateTime.Now }
                );
            }

            if (!context.Produtos.Any())
            {
                var agora = DateTime.Now;
                context.Produtos.AddRange(
                    new Produto
                    {
                        Id = Guid.NewGuid(),
                        Nome = "Smartphone X100",
                        Descricao = "Smartphone de última geração com câmera de 108MP",
                        Imagem = "/imagens/smartphone.jpg",
                        Preco = 2999.90m,
                        Estoque = 15,
                        CategoriaId = eletronicoId,
                        VendedorId = vendedorId,
                        CreatedAt = agora,
                        UpdatedAt = agora
                    },
                    new Produto
                    {
                        Id = Guid.NewGuid(),
                        Nome = "Camisa Social Masculina",
                        Descricao = "Camisa social de algodão premium",
                        Imagem = "/imagens/camisa.jpg",
                        Preco = 129.90m,
                        Estoque = 40,
                        CategoriaId = roupaId,
                        VendedorId = vendedorId,
                        CreatedAt = agora,
                        UpdatedAt = agora
                    },
                    new Produto
                    {
                        Id = Guid.NewGuid(),
                        Nome = "Livro de Design de Software",
                        Descricao = "Um guia completo sobre padrões e arquitetura de software",
                        Imagem = "/imagens/livro.jpg",
                        Preco = 89.90m,
                        Estoque = 25,
                        CategoriaId = livroId,
                        VendedorId = vendedorId,
                        CreatedAt = agora,
                        UpdatedAt = agora
                    }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
