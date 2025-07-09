# Wallet API

Projeto desafio https://github.com/WL-Consultings/challenges/tree/main/backend

# Dependências
Docker instalado e rodando

# Como rodar
Na pasta do projeto executar comando:
- docker compose up -d

# Utilizando no Browser
API swagger:
- http://localhost:5000

PGADMIN4:
- http://localhost:5050/
- usuário: admin@admin.com
- senha: admin123
- Host=postgres;Port=5432;Database=WalletDb;Username=admin;Password=admin123

# Desafio
- Autenticação
    - POST /Auth/Login
        - UserIds 1 e 2 já são criados no seed
        - Colocar token da resposta no Authorize do swagger, exemplo: `Bearer ey...`
- Criar um usuário
    - POST /User
- Consultar saldo da carteira de um usuário
    - GET /User/balance
- Adicionar saldo à carteira
    - POST /Deposit
- Criar uma transferência entre usuários (carteiras)
    - POST /Transfer
- Listar transferências realizadas por um usuário, com filtro opcional por período de data
    - GET /Transfer
