# 🚀 Guia de Acesso - Banca Jornal

## 📍 URLs de Acesso da Aplicação

### **Produção (VPS Hostinger)**

Após o deploy, sua aplicação estará disponível nas seguintes URLs:

#### **Portas Customizadas (Segurança Aumentada):**
- **HTTP:** Porta `8090` (ao invés da padrão 80)
- **HTTPS:** Porta `8443` (ao invés da padrão 443)

| Endpoint | URL | Descrição |
|----------|-----|-----------|
| **Frontend** | `http://SEU_IP:8090/bancajornal/` | Interface Blazor WebAssembly |
| **API Docs** | `http://SEU_IP:8090/bancajornal/swagger` | Documentação Swagger UI |
| **Health Check** | `http://SEU_IP:8090/health` | Status de saúde da aplicação |
| **Produtos API** | `http://SEU_IP:8090/bancajornal/api/produtos` | CRUD de produtos (JSON) |
| **Vendas API** | `http://SEU_IP:8090/bancajornal/api/vendas` | CRUD de vendas (JSON) |

### **Desenvolvimento Local**

| Endpoint | URL | Descrição |
|----------|-----|-----------|
| **API** | `http://localhost:5000` | Backend local (sem PathBase) |
| **Web** | `http://localhost:5001` | Frontend local (sem PathBase) |
| **Swagger** | `http://localhost:5000/swagger` | Documentação local |

---

## 🔍 Como Descobrir o IP do Seu VPS

### **Método 1: Painel Hostinger**
1. Acesse: [https://hpanel.hostinger.com](https://hpanel.hostinger.com)
2. Navegue até **VPS** → Seu servidor
3. O IP será exibido na página principal do VPS

### **Método 2: Via SSH**
```bash
ssh usuario@seu-vps
curl ifconfig.me
```

### **Método 3: Via Terminal Local**
```bash
# Descobrir IP do VPS (substitua 'usuario' e 'vps-hostname')
ssh usuario@vps-hostname "curl -s ifconfig.me"
```

---

## ✅ Verificar se a Aplicação Está Rodando

### **1. Testar Health Check**
```bash
# Substitua SEU_IP pelo IP real do VPS
curl http://SEU_IP:8090/health
# Resposta esperada: "healthy"
```

### **2. Testar API de Produtos**
```bash
curl http://SEU_IP:8090/bancajornal/api/produtos
# Resposta esperada: JSON com lista de produtos (pode estar vazia inicialmente)
```

### **3. Verificar Containers Docker**
```bash
# Conectar ao VPS
ssh usuario@seu-vps

# Navegar para o diretório do projeto
cd /var/www/banca-jornal

# Ver containers ativos
docker-compose ps
# Deve mostrar:
# bancajornal-api    healthy
# bancajornal-nginx  healthy

# Ver logs em tempo real
docker-compose logs -f bancajornal-api
docker-compose logs -f nginx
```

---

## 🔧 Configurar Firewall do VPS

As portas customizadas `8090` e `8443` precisam estar abertas no firewall:

```bash
# Conectar ao VPS
ssh usuario@seu-vps

# Verificar status do firewall (UFW)
sudo ufw status

# Permitir portas customizadas
sudo ufw allow 22/tcp     # SSH (essencial!)
sudo ufw allow 8090/tcp   # HTTP customizado
sudo ufw allow 8443/tcp   # HTTPS customizado

# Ativar firewall (se não estiver ativo)
sudo ufw enable

# Verificar portas abertas
sudo netstat -tlnp | grep -E ':(8090|8443)'
```

---

## 🌐 Exemplo de Acesso Completo

Supondo que seu VPS tem IP `123.45.67.89`:

### **Frontend Blazor WebAssembly:**
```
http://123.45.67.89:8090/bancajornal/
```

### **Documentação da API (Swagger):**
```
http://123.45.67.89:8090/bancajornal/swagger
```

### **Health Check:**
```
http://123.45.67.89:8090/health
```

### **Listar Produtos (API REST):**
```
http://123.45.67.89:8090/bancajornal/api/produtos
```

### **Criar Produto (API REST):**
```bash
curl -X POST http://123.45.67.89:8090/bancajornal/api/produtos \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Jornal O Globo",
    "precoCusto": 3.50,
    "precoVenda": 5.00,
    "quantidadeEstoque": 50
  }'
```

---

## 🔒 Configurar HTTPS (Recomendado)

### **Opção 1: Com Domínio Próprio + Let's Encrypt**

Se você tem um domínio (ex: `seudominio.com`):

```bash
# Conectar ao VPS
ssh usuario@seu-vps

# Instalar Certbot
sudo apt update
sudo apt install certbot python3-certbot-nginx -y

# Editar nginx/default.conf para usar seu domínio
# Substitua "server_name _;" por "server_name seudominio.com;"

# Gerar certificado SSL
sudo certbot --nginx -d seudominio.com -d www.seudominio.com

# Renovação automática (já configurada pelo Certbot)
sudo certbot renew --dry-run
```

Após configurar SSL, acesse via:
```
https://seudominio.com:8443/bancajornal/
```

### **Opção 2: Sem Domínio (Self-Signed Certificate)**

Se você não tem domínio próprio:

```bash
# Gerar certificado auto-assinado
mkdir -p nginx/ssl
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx/ssl/privkey.pem \
  -out nginx/ssl/fullchain.pem \
  -subj "/CN=bancajornal/O=BancaJornal/C=BR"

# Descomente as linhas SSL no nginx/default.conf
# E reinicie o container
docker-compose restart nginx
```

⚠️ **Nota:** Certificados auto-assinados mostrarão aviso de segurança no navegador.

---

## 📊 Monitoramento e Logs

### **Ver Logs do Container API:**
```bash
cd /var/www/banca-jornal
docker-compose logs -f bancajornal-api
```

### **Ver Logs do Nginx:**
```bash
docker-compose logs -f nginx

# Ou diretamente do volume
tail -f nginx/logs/bancajornal_access.log
tail -f nginx/logs/bancajornal_error.log
```

### **Verificar Health Checks:**
```bash
# Health check da API
curl http://localhost/health

# Health check do Nginx (via porta pública)
curl http://SEU_IP:8090/health
```

### **Status dos Containers:**
```bash
docker-compose ps

# Deve mostrar:
# NAME                  STATUS              PORTS
# bancajornal-api       Up (healthy)        80/tcp
# bancajornal-nginx     Up (healthy)        0.0.0.0:8090->8090/tcp, 0.0.0.0:8443->8443/tcp
```

---

## 🆘 Troubleshooting

### **1. "Site não pode ser acessado"**

**Verificar containers:**
```bash
docker-compose ps
# Se algum estiver "unhealthy" ou "Exited":
docker-compose logs bancajornal-api
docker-compose restart
```

**Verificar firewall:**
```bash
sudo ufw status
sudo ufw allow 8090/tcp
```

### **2. "Connection refused na porta 8090"**

**Verificar se Nginx está ouvindo:**
```bash
sudo netstat -tlnp | grep :8090
# Deve mostrar: nginx listening on 0.0.0.0:8090
```

**Reiniciar Nginx:**
```bash
docker-compose restart nginx
```

### **3. "404 Not Found" ao acessar /bancajornal/**

**Verificar PathBase:**
```bash
# Verificar variável de ambiente no container
docker exec bancajornal-api printenv | grep PathBase
# Deve mostrar: PathBase=/bancajornal
```

**Verificar configuração do Nginx:**
```bash
docker exec bancajornal-nginx cat /etc/nginx/conf.d/default.conf
# Deve conter: location /bancajornal/ { ... }
```

### **4. Health Check Falhando**

**Testar health check manualmente:**
```bash
# Dentro do container
docker exec bancajornal-api curl http://localhost/health

# Do host
curl http://SEU_IP:8090/health
```

**Ver logs do health check:**
```bash
docker inspect bancajornal-api | grep -A 10 Health
```

---

## 🎯 Checklist de Acesso

Após o deploy, verifique:

- [ ] **Containers ativos:** `docker-compose ps` mostra "healthy"
- [ ] **Firewall configurado:** Portas 8090 e 8443 abertas
- [ ] **Health check OK:** `curl http://SEU_IP:8090/health` retorna "healthy"
- [ ] **Frontend carrega:** Acesse `http://SEU_IP:8090/bancajornal/` no navegador
- [ ] **Swagger acessível:** `http://SEU_IP:8090/bancajornal/swagger`
- [ ] **API responde:** `curl http://SEU_IP:8090/bancajornal/api/produtos`

---

## 📱 Testar em Diferentes Dispositivos

A aplicação é **responsiva** e funciona em:

```
✅ Desktop (Windows, macOS, Linux)
✅ Tablets (iPad, Android)
✅ Smartphones (iOS, Android)
```

**Testar:**
1. Abra `http://SEU_IP:8090/bancajornal/` no celular
2. Conecte-se à mesma rede Wi-Fi ou use dados móveis
3. Verifique se a interface se adapta ao tamanho da tela

---

## 🎉 Resumo Rápido

**Para acessar sua aplicação após deploy:**

1. **Descubra o IP do VPS:** Painel Hostinger ou via SSH (`curl ifconfig.me`)
2. **Acesse no navegador:** `http://SEU_IP:8090/bancajornal/`
3. **Teste a API:** `http://SEU_IP:8090/bancajornal/swagger`
4. **Verifique health:** `http://SEU_IP:8090/health`

**Portas customizadas para segurança:**
- HTTP: `8090`
- HTTPS: `8443`

**Path da aplicação:**
- `/bancajornal/` (todas as rotas partem deste prefixo)

---

📖 **Para mais detalhes sobre configuração e troubleshooting, consulte [DEPLOY_GUIDE.md](.github/DEPLOY_GUIDE.md)**
