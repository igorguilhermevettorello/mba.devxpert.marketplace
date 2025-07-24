# Feedback - Avaliação Geral

## Front End

### Navegação
- Será avaliado na entrega final

### Design
- Será avaliado na entrega final

### Funcionalidade
- Será avaliado na entrega final

## Back End

### Arquitetura
  * Pontos positivos:
    - Projeto de arquitetura enxuto e coeso    
 
### Funcionalidade
  * Pontos positivos:
    - Os cases de negócio que foram implementados até o momento estão dentro do padrão esperado

  * Pontos negativos:
    - Assim como o vendedor, o cliente também é uma entidade de negócios e precisa possuir um AspNetUser e um registro na tabela Cliente  
    - Upload de imagem não deveria ocorrer na responsabilidade de negócios, é uma responsabilidade de Aplicação.
    - Um produto pode ser desabilitado por um admin, não existe um método para isso, logo entende-se que fica a cargo da atualização do produto, mas não existe validação de "dono" do produto abrindo uma brecha para apenas o admin ou o "dono" editar.

### Modelagem
  * Pontos positivos:
    - Modelagem de negócios responsável na medida certa

## Projeto

### Organização
  * Pontos positivos:
    - Organização de pastas e arquivos

  * Pontos negativos:
    - Documentação do README incompleta
    - 2 configurações identicas de Seed, DatabaseSelector (MVC e API) deveriam aproveitar o mesmo arquivo para ambos.

### Documentação
- Será avaliado na entrega final
 
### Instalação
  * Pontos positivos:
    - Uso de SQLite e Seed de dados
