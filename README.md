# 🧡 Daki - Sistema de Conexão e Vitrine de Doações

O **Daki** é uma plataforma web desenvolvida com o objetivo de conectar pessoas que possuem itens em bom estado para doar com pessoas, instituições ou comunidades que realmente precisam deles. O projeto nasceu com um propósito puramente acadêmico e beneficente, promovendo a solidariedade através de uma interface simples, segura e direta.

---

## 🚀 Funcionalidades Principais (MVP)

- **Vitrine Dinâmica de Anúncios:** Exibição centralizada de itens ativos com carregamento otimizado.
- **Filtro Avançado por Categoria:** Navegação fluida que reconstrói consultas de forma otimizada no banco de dados através de filtros dinâmicos.
- **Gestão de Perfil Segura:** Atualização de dados cadastrais (Nome e WhatsApp) com verificação rigorosa de senha atual.
- **Fluxo de Match & Reserva:** Sistema de solicitação de interesse, onde o doador pode avaliar justificativas, aceitar ou recusar pedidos, alterando dinamicamente o status do anúncio (Ativo, Reservado, Concluído).
- **Tratamento de Segurança:** Criptografia de senhas utilizando Hash com BCrypt e validações robustas de formulários via Data Annotations e Expressões Regulares (Regex) para formatos de contato.

---

## 🛠️ Tecnologias Utilizadas

O projeto foi construído utilizando práticas modernas de desenvolvimento web no ecossistema Microsoft:

- **Linguagem:** C#
- **Framework Principal:** ASP.NET Core MVC (.NET 10+)
- **Persistência de Dados:** Entity Framework Core
- **Banco de Dados:** PostgreSQL (Configurável via Connection String)
- **Segurança:** BCrypt.Net (Hashing de senhas)
- **Interface e Estilização:** Bootstrap 5 & Bootstrap Icons
- **Validações:** jQuery Validation (Client-side) & ModelState (Server-side)

---

## 📐 Modelagem e Arquitetura do Sistema

O projeto segue o padrão arquitetural **MVC (Model-View-Controller)** combinado com princípios de **Domain-Driven Design (DDD)** para isolar as regras de negócio:

* **Entidades de Domínio Ricas:** A lógica e o estado da aplicação são protegidos dentro das próprias entidades (Encapsulamento com propriedades `private set` e métodos de alteração como `AtualizarDados` e `AlterarSenha`).
* **Camada de Abstração (Repository Pattern):** Desacoplamento total do acesso a dados através de interfaces.
* **ViewModels Dedicadas:** Separação estrita entre os dados que trafegam nas Views e as Entidades do Banco de Dados, evitando brechas de segurança.

### 👥 Diagrama de Casos de Uso
O diagrama abaixo ilustra as principais interações do Doador e do Interessado dentro da plataforma:

<img width="626" height="518" alt="image" src="https://github.com/user-attachments/assets/210884b8-24ba-4828-a757-bf9682e660fe" />

### 🗂️ Diagrama de Classes
A estrutura de classes de domínio e seus respectivos relacionamentos (Usuário, Anúncio, Imagem, Endereço e Interesse):

<img width="1568" height="810" alt="image" src="https://github.com/user-attachments/assets/7d8fdd5e-898b-4755-bfc1-f5578296be60" />

---

## 📦 Como Executar o Projeto

### Pré-requisitos
* .NET SDK (Versão 6.0 ou superior)
* IDE de sua preferência (Visual Studio, VS Code ou Rider)

### Passos para Execução
1. Clone o repositório em sua máquina:
   ```bash
   git clone https://github.com/Kevin439560/Sistema-de-doacao-DAKI.git

2. Navegue até a pasta do projeto web:
   ```bash
    cd Sistema-de-doacao-DAKI/Daki.Web
3. Execute o comando para restaurar as dependências e rodar a aplicação:
    ```bash
    dotnet run
4. Abra o navegador e acesse o endereço local indicado no terminal (como https://localhost:7047)

Este projeto foi desenvolvido como critério de avaliação prática para o curso de graduação em Análise e Desenvolvimento de Sistemas. O foco principal foi aplicar conceitos de Engenharia de Software, Modelagem Relacional, Segurança da Informação e Arquitetura de Sistemas em um cenário do mundo real focado no terceiro setor.
