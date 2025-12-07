# 🚀 Guia de Configuração - Deploy Automático no Hostinger

## ✅ Workflow GitHub Actions Criado

O arquivo `.github/workflows/deploy-hostinger.yml` foi configurado com:

- ✅ Build e teste automático em cada push para `main` ou `master`
- ✅ Publicação de artefatos (API e Web)
- ✅ Deploy via SSH para Hostinger
- ✅ Docker Compose automático no servidor
- ✅ Verificação de status pós-deploy

---

## 📋 Pré-requisitos no Hostinger

### 1. **Container Hostinger**
Certifique-se de que você tem um **VPS Hostinger** ou **Business Hosting** com suporte a Docker.

> **⚠️ IMPORTANTE:** Hosting compartilhado NÃO suporta Docker! Você precisa de um VPS.

### 2. **Requisitos no Servidor**
Conecte-se ao seu VPS via SSH e verifique:

```bash
# Verificar Docker
docker --version

# Verificar Docker Compose
docker-compose --version

# Verificar Git
git --version
```

Se não estiverem instalados, instale:

```bash
# Instalar Docker (Ubuntu/Debian)
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER

# Instalar Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Instalar Git
sudo apt update && sudo apt install git -y
```

---

## 🔐 Configuração dos GitHub Secrets

### Passo 1: Acessar Configurações do Repositório
1. Acesse seu repositório no GitHub: `https://github.com/leandrohonorio7/banca-jornal`
2. Clique em **Settings** (Configurações)
3. No menu lateral, clique em **Secrets and variables** → **Actions**
4. Clique em **New repository secret**

### Passo 2: Adicionar os Secrets

#### **Secret 1: HOSTINGER_HOST**
- **Nome:** `HOSTINGER_HOST`
- **Valor:** O endereço IP ou domínio do seu VPS Hostinger
  - Exemplo: `123.45.67.89` ou `vps12345.hostinger.com`

#### **Secret 2: HOSTINGER_USERNAME**
- **Nome:** `HOSTINGER_USERNAME`
- **Valor:** Seu usuário SSH (geralmente `root` ou `u123456789`)

#### **Secret 3: HOSTINGER_SSH_KEY**
- **Nome:** `HOSTINGER_SSH_KEY`
- **Valor:** Sua chave privada SSH (conteúdo completo do arquivo `id_rsa`)

**Como obter a chave SSH:**

```bash
# No seu computador local
ssh-keygen -t rsa -b 4096 -C "deploy-banca-jornal"

# Copiar a chave pública para o servidor
ssh-copy-id usuario@seu-vps-hostinger.com

# Obter o conteúdo da chave PRIVADA para adicionar no GitHub Secret
cat ~/.ssh/id_rsa
```

**ATENÇÃO:** Copie TODO o conteúdo do arquivo, incluindo:
```
-----BEGIN OPENSSH PRIVATE KEY-----
...
-----END OPENSSH PRIVATE KEY-----
```

#### **Secret 4: HOSTINGER_PORT**
- **Nome:** `HOSTINGER_PORT`
- **Valor:** Porta SSH (geralmente `22`)

#### **Secret 5: HOSTINGER_PROJECT_PATH**
- **Nome:** `HOSTINGER_PROJECT_PATH`
- **Valor:** Caminho completo do projeto no servidor
  - Exemplo: `/home/usuario/banca-jornal` ou `/var/www/banca-jornal`

---

## 🌐 Como Identificar Seu Domínio no Hostinger

9
1. Acesse: [https://hpanel.hostinger.com](https://hpanel.hostinger.com)
2. Faça login com suas credenciais
3. Na seção **VPS**, clique no seu servidor
4. Você verá o **IP do servidor** e o **domínio temporário** (se houver)
   - Exemplo: `vps-123456.hostinger.com`

### Opção 2: Via SSH no Servidor
Conecte-se ao VPS e execute:

```bash
# Verificar IP público
curl ifconfig.me

# Verificar hostname
hostname -f
```

### Opção 3: Domínio Personalizado
Se você configurou um domínio próprio (ex: `meusite.com`):
1. Acesse o painel Hostinger → **Domínios**
2. Verifique os **DNS Records** apontando para o IP do VPS
3. Seu domínio será: `https://meusite.com` ou `https://api.meusite.com`

---

## 🔧 Configurar o Projeto no Servidor (Primeira Vez)

Conecte-se ao seu VPS via SSH e execute:

```bash
# 1. Criar diretório do projeto
mkdir -p /var/www/banca-jornal
cd /var/www/banca-jornal

# 2. Clonar o repositório
git clone https://github.com/leandrohonorio7/banca-jornal.git .

# 3. Configurar permissões
sudo chown -R $USER:$USER /var/www/banca-jornal

# 4. Testar build manual (primeira vez)
docker-compose build
docker-compose up -d

# 5. Verificar se está rodando
docker-compose ps
docker-compose logs bancajornal-api
```

---

## 🚀 Testando o Deploy Automático

### Após configurar os Secrets:

1. Faça uma alteração no código
2. Commit e push para a branch `main` ou `master`:

```powershell
git add .
git commit -m "test: trigger auto deploy"
git push origin main
```

3. Acesse: `https://github.com/leandrohonorio7/banca-jornal/actions`
4. Você verá o workflow **"Deploy to Hostinger"** executando
5. Aguarde a conclusão (✅ verde = sucesso)

---

## 📊 Monitoramento Pós-Deploy

### Verificar logs no servidor:
```bash
cd /var/www/banca-jornal
docker-compose logs -f bancajornal-api
```

### Verificar containers ativos:
```bash
docker-compose ps
```

### Reiniciar manualmente (se necessário):
```bash
docker-compose restart
```

---

## 🆘 Troubleshooting

### Erro: "Permission denied (publickey)"
- **Causa:** Chave SSH não configurada corretamente
- **Solução:** Verifique se copiou a chave PRIVADA completa no Secret `HOSTINGER_SSH_KEY`

### Erro: "docker-compose: command not found"
- **Causa:** Docker Compose não instalado no servidor
- **Solução:** Instale conforme seção "Pré-requisitos no Hostinger"

### Erro: "Could not resolve host"
- **Causa:** `HOSTINGER_HOST` incorreto
- **Solução:** Verifique o IP ou domínio no painel Hostinger

### Build falha com "no space left on device"
- **Causa:** Disco cheio no VPS
- **Solução:** Limpe containers antigos:
```bash
docker system prune -a --volumes
```

---

## 📝 Resumo dos Secrets Necessários

| Secret                    | Descrição                          | Exemplo                          |
|---------------------------|------------------------------------|----------------------------------|
| `HOSTINGER_HOST`          | IP ou domínio do VPS               | `123.45.67.89`                   |
| `HOSTINGER_USERNAME`      | Usuário SSH                        | `root` ou `u123456789`           |
| `HOSTINGER_SSH_KEY`       | Chave privada SSH (conteúdo completo) | `-----BEGIN OPENSSH...`       |
| `HOSTINGER_PORT`          | Porta SSH                          | `22`                             |
| `HOSTINGER_PROJECT_PATH`  | Caminho do projeto no servidor     | `/var/www/banca-jornal`          |

---

## ✅ Checklist de Configuração

- [ ] VPS Hostinger com Docker e Docker Compose instalados
- [ ] Repositório clonado no servidor (primeira vez manual)
- [ ] Chave SSH criada e adicionada ao servidor
- [ ] 5 Secrets configurados no GitHub
- [ ] Primeiro deploy manual testado (`docker-compose up -d`)
- [ ] Push para `main`/`master` e verificação do workflow no GitHub Actions
- [ ] Acesso ao domínio/IP do Hostinger verificado

---

**🎉 Pronto! Agora todo push para `main` ou `master` fará deploy automático no Hostinger!**

Para mais detalhes, consulte:
- [README.md](../README.md)
- [QUICK_START.md](../QUICK_START.md)
- [Documentação GitHub Actions](https://docs.github.com/en/actions)
