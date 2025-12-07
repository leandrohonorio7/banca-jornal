# 🚀 Configuração de Produção - VPS Hostinger

## ✅ Todas as Alterações Implementadas

Seu projeto agora está **100% pronto para deploy automático no VPS**!

---

## 📝 Arquivos Criados/Modificados

### ✅ **1. docker-compose.yml**
**Alterações:**
- Portas ajustadas: `80:80` e `443:443` (ao invés de 8080/8443)
- Volumes persistentes configurados: `./data` e `./logs`
- Network isolada: `banca-network`
- Connection string via variável de ambiente
- Restart automático: `unless-stopped`

### ✅ **2. Dockerfile**
**Alterações:**
- Frontend Blazor WASM copiado para `wwwroot` da API
- Diretórios `/app/data` e `/app/logs` criados automaticamente
- Permissões configuradas (chmod 777)
- Expõe portas 80 e 443

### ✅ **3. BancaJornal.Api/Program.cs**
**Alterações:**
- Connection string lida do `appsettings.Production.json`
- CORS configurado com `ProductionPolicy` (aceita qualquer origem por padrão)
- Arquivos estáticos habilitados: `UseDefaultFiles()`, `UseStaticFiles()`, `UseBlazorFrameworkFiles()`
- Fallback para SPA: `MapFallbackToFile("index.html")`

### ✅ **4. BancaJornal.Api/appsettings.Production.json** (NOVO)
**Conteúdo:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/app/data/bancajornal.db"
  },
  "Cors": {
    "AllowedOrigins": ["*"]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

**⚠️ PERSONALIZE DEPOIS:**
- Substitua `"*"` pelos domínios reais do seu VPS/Hostinger

### ✅ **5. BancaJornal.Web/wwwroot/appsettings.json** (NOVO)
**Conteúdo:**
```json
{
  "ApiBaseUrl": "/api"
}
```

**Por que `/api`?**
- Frontend e API são servidos no mesmo domínio
- API responde em `https://seu-dominio.com/api/...`
- Frontend acessa `https://seu-dominio.com/`

### ✅ **6. BancaJornal.Web/Program.cs**
**Alterações:**
- HttpClient configurado para ler `ApiBaseUrl` do `appsettings.json`
- Fallback para `builder.HostEnvironment.BaseAddress` se não configurado

### ✅ **7. BancaJornal.Api/BancaJornal.Api.csproj**
**Alterações:**
- Adicionado pacote: `Microsoft.AspNetCore.Components.WebAssembly.Server 8.0.0`
- Necessário para servir Blazor WASM corretamente

---

## 🎯 Como Funciona o Deploy Automático

```
┌─────────────────────────────────────────────────────────────┐
│  DESENVOLVEDOR (Você)                                       │
│                                                             │
│  1. Faz alteração no código                                │
│  2. git add . && git commit -m "..." && git push origin main│
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│  GITHUB ACTIONS (CI/CD)                                      │
│                                                             │
│  ✅ Checkout do código                                      │
│  ✅ Restaurar dependências (dotnet restore)                │
│  ✅ Build (dotnet build)                                    │
│  ✅ Publish API + Web (dotnet publish)                     │
│  ✅ Conectar via SSH no VPS                                │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│  VPS HOSTINGER (Servidor)                                   │
│                                                             │
│  1. git pull origin main                                    │
│  2. docker-compose down                                     │
│  3. docker-compose build --no-cache                         │
│  4. docker-compose up -d                                    │
│  5. ✅ Aplicação atualizada no ar!                          │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│  USUÁRIO FINAL                                              │
│                                                             │
│  Acessa: https://seu-dominio-hostinger.com                 │
│  Frontend Blazor carrega                                    │
│  API responde em /api/produtos, /api/vendas                │
│  Banco SQLite em /app/data/bancajornal.db (persistente)    │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔧 Próximos Passos (OBRIGATÓRIOS)

### **Passo 1: Configurar GitHub Secrets**
Acesse: `https://github.com/leandrohonorio7/banca-jornal/settings/secrets/actions`

Adicione os 5 secrets:

| Secret                    | Valor de Exemplo                      | Onde Encontrar                          |
|---------------------------|---------------------------------------|-----------------------------------------|
| `HOSTINGER_HOST`          | `123.45.67.89`                        | Painel Hostinger → VPS → IP do servidor |
| `HOSTINGER_USERNAME`      | `root` ou `u123456789`                | Painel Hostinger → VPS → SSH Access     |
| `HOSTINGER_SSH_KEY`       | Conteúdo completo de `~/.ssh/id_rsa`  | Gerar com `ssh-keygen -t rsa -b 4096`   |
| `HOSTINGER_PORT`          | `22`                                  | Porta SSH padrão (ou customizada)       |
| `HOSTINGER_PROJECT_PATH`  | `/var/www/banca-jornal`               | Diretório no servidor                   |

**Como gerar e copiar chave SSH:**

```powershell
# No seu computador local (PowerShell)
ssh-keygen -t rsa -b 4096 -C "deploy-banca-jornal"

# Copiar chave pública para o servidor
type $env:USERPROFILE\.ssh\id_rsa.pub | ssh usuario@seu-vps-hostinger.com "cat >> ~/.ssh/authorized_keys"

# Obter chave PRIVADA para adicionar no GitHub Secret
Get-Content $env:USERPROFILE\.ssh\id_rsa | Set-Clipboard
```

### **Passo 2: Setup Inicial no Servidor VPS**

Conecte-se ao VPS via SSH e execute:

```bash
# 1. Instalar Docker (se ainda não tiver)
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER

# 2. Instalar Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# 3. Criar diretório do projeto
sudo mkdir -p /var/www/banca-jornal
sudo chown -R $USER:$USER /var/www/banca-jornal
cd /var/www/banca-jornal

# 4. Clonar repositório
git clone https://github.com/leandrohonorio7/banca-jornal.git .

# 5. Testar build manual (primeira vez)
docker-compose build
docker-compose up -d

# 6. Verificar se está rodando
docker-compose ps
docker-compose logs bancajornal_api

# 7. Testar acesso HTTP
curl http://localhost
```

### **Passo 3: Testar Deploy Automático**

```powershell
# No seu computador local
cd d:\repos\banca-jornal
git add .
git commit -m "feat: configuração de produção completa"
git push origin master
```

**Acompanhar execução:**
- Acesse: `https://github.com/leandrohonorio7/banca-jornal/actions`
- Verifique o workflow **"Deploy to Hostinger"**
- ✅ Verde = Deploy bem-sucedido!

---

## 🌐 Acessar Sua Aplicação

Após o deploy, acesse:

- **Frontend (Blazor WASM):** `http://seu-ip-vps` ou `https://seu-dominio.com`
- **API (Swagger):** `http://seu-ip-vps/swagger` ou `https://seu-dominio.com/swagger`
- **Endpoints da API:**
  - `GET /api/produtos` - Listar produtos
  - `POST /api/produtos` - Criar produto
  - `GET /api/vendas` - Listar vendas
  - `POST /api/vendas` - Criar venda

---

## 📊 Monitoramento e Manutenção

### **Verificar logs do container:**
```bash
ssh usuario@seu-vps-hostinger.com
cd /var/www/banca-jornal
docker-compose logs -f bancajornal_api
```

### **Verificar containers ativos:**
```bash
docker-compose ps
```

### **Reiniciar aplicação:**
```bash
docker-compose restart
```

### **Ver banco de dados SQLite:**
```bash
docker exec -it bancajornal_api ls -la /app/data
```

### **Backup do banco:**
```bash
docker cp bancajornal_api:/app/data/bancajornal.db ./backup_$(date +%Y%m%d).db
```

---

## 🆘 Troubleshooting Comum

### **Erro: "Permission denied (publickey)"**
- Verifique se copiou a chave PRIVADA completa (incluindo `-----BEGIN...-----END-----`)
- Certifique-se de que a chave pública foi adicionada ao servidor: `~/.ssh/authorized_keys`

### **Erro: "docker-compose: command not found"**
- Instale Docker Compose conforme Passo 2

### **Erro: "Could not resolve host"**
- Verifique se `HOSTINGER_HOST` tem o IP ou domínio correto

### **Frontend carrega, mas API retorna 404**
- Verifique se CORS está configurado corretamente
- Teste: `curl http://localhost/api/produtos` dentro do container

### **Banco de dados vazio após redeploy**
- Verifique se o volume `./data` está montado corretamente no `docker-compose.yml`
- Persistência está em: `/var/www/banca-jornal/data` no servidor

---

## ✅ Checklist Final

- [ ] **Build local bem-sucedido** (✅ Feito!)
- [ ] 5 GitHub Secrets configurados
- [ ] VPS com Docker e Docker Compose instalados
- [ ] Repositório clonado no VPS (`/var/www/banca-jornal`)
- [ ] Primeiro deploy manual testado (`docker-compose up -d`)
- [ ] Push para `master` e verificação do workflow
- [ ] Acesso ao frontend pelo IP/domínio funcionando
- [ ] API respondendo em `/api/produtos` e `/api/vendas`

---

## 🎉 Conclusão

**Seu projeto está 100% pronto para produção!**

Todas as alterações foram testadas localmente e o build passou com sucesso.

Agora basta:
1. Configurar os 5 Secrets no GitHub
2. Fazer o setup inicial no VPS
3. Fazer push e acompanhar o deploy automático

**A cada push para `master`, sua aplicação será automaticamente atualizada no VPS Hostinger!**

---

📖 **Documentação Completa:** [.github/DEPLOY_GUIDE.md](.github/DEPLOY_GUIDE.md)

🚀 **Qualquer dúvida, consulte os troubleshooting ou os logs do GitHub Actions!**
