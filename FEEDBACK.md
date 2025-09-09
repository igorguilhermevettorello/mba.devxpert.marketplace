# Avaliação Geral

## Funcionalidade 30%

Avalie se o projeto atende a todos os requisitos funcionais definidos.
* Todas as funcionalidades propostas no escopo inicial foram implementadas.

## Qualidade do Código 20%

Considere clareza, organização e uso de padrões de codificação.

### Data
* Em `Repository<T>`:
  - possui métodos públicos que permitem modificar o estado da entidade, como `Add`, `Update` e `Remove` sem ser pela classe especializada.
  - o método `ObterPorId()` poderia usar o mesmo método `FirstOrDefault()` com ou sem _tracking_ para maior consistência.
  - o método `Dispose()` faz _dispose_ do `DbContext`, o que pode causar problemas se o contexto for compartilhado. Isso é de responsabilidade de quem o criou, no caso da injeção de dependência, o escopo do contexto é gerenciado pelo contêiner.
* Existe a abstração `Repository<T>`, mas não é aproveitada nos repositórios especializados.

### API
* As _controllers_ `ClientController` e `FavoritoController` possuem regras de negócio que deveriam estar na camada `Business`.
* Em `VendedorController` existe a dependência do `CategoriaRepository`, apesar de corretamente não utilizada, mas injetar repositório direto na controller quebra a abstração da camada de negócio.
* Em `AdminController`:
  - `IsAdmin()` é assíncrono, mas não há nenhuma operação assíncrona dentro do método.
  - `Index()` trafega todos os dados do banco de dados para a memória apenas para contar os registros. Isso deve ser otimizado.
  - `Index()` possui lógica diferente se for admin ou não. O ideal é ter um endpoint específico para admin, e o cliente chamar o endpoint correto.

### Geral

* Remover `usings` não utilizados.
* Evitar comentários desnecessários.
* Remover códigos comentados.
* Várias classes usam construtor primário e possuem campos privados desnecessários.
* Métodos devem começar com verbo no imperativo.
* Mais consistência nos tipos nulos entre _interfaces_ e implementações.

## Eficiência e Desempenho 20%

Avalie o desempenho e a eficiência das soluções implementadas.
* Existem algumas chamadas ao banco de dados que podem ser otimizadas, como mencionado na seção de Qualidade do Código.

## Inovação e Diferenciais 10%

Considere a criatividade e inovação na solução proposta.
* A solução é sólida e atende bem aos requisitos.

## Documentação e Organização 10%

Verifique a qualidade e completude da documentação, incluindo README.md.

* Em "Configuração do Banco de Dados", o passo "Rode o projeto para que a configuração do Seed crie o banco e popule com os dados básicos" é vago. Qual projeto? Quais passos específicos?
  - Consegui após executar `dotnet run` no projeto `Api`.
* Ao executar `dotnet run` em:
  - `MVC`, iniciou o projeto em http://localhost:5006, porta diferente do mencionado no `README.md`.
  - `API`, iniciou o projeto em http://localhost:5109, porta diferente do mencionado no `README.md`.
* Ao executar `ng s` em `LojaVirtual`:
  - Retornou `Error: Could not find the '@angular/build:dev-server' builder's node package.`
  - Corrigido após executar `npm install` para instalar as dependências.

## Resolução de Feedbacks 10%

Avalie a resolução dos problemas apontados na primeira avaliação de frontend

Comentários anteriormente feitos e corrigidos:
  * ✔️ Assim como o vendedor, o cliente também é uma entidade de negócios e precisa possuir um AspNetUser e um registro na tabela Cliente
  * ✔️ Upload de imagem não deveria ocorrer na responsabilidade de negócios, é uma responsabilidade de Aplicação.    
  * ️✔️ Um produto pode ser desabilitado por um admin, não existe um método para isso, logo entende-se que fica a cargo da atualização do produto, mas não existe validação de "dono" do produto abrindo uma brecha para apenas o admin ou o "dono" editar.

## Notas

| Critério                     | Peso | Nota | Nota Ponderada |
|------------------------------|------|-----:|---------------:|
| Funcionalidade               | 30%  |   10 |            3.0 |
| Qualidade do Código          | 20%  |    9 |            1.8 |
| Eficiência e Desempenho      | 20%  |    9 |            1.8 |
| Inovação e Diferenciais      | 10%  |    8 |            0.8 |
| Documentação e Organização   | 10%  |    8 |            0.8 |
| Resolução de Feedbacks       | 10%  |   10 |            1.0 |
| **Total**                    |      |      |        **9.2** |

